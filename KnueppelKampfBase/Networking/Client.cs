﻿using KnueppelKampfBase.Game;
using KnueppelKampfBase.Game.Objects;
using KnueppelKampfBase.Networking.Packets;
using KnueppelKampfBase.Networking.Packets.ClientPackets;
using KnueppelKampfBase.Networking.Packets.ServerPackets;
using KnueppelKampfBase.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking
{
    /// <summary>
    /// Object handling communication with server
    /// </summary>
    public class Client : IDisposable
    {
        private bool isDisposed;
        private CustomUdpClient client;
        private byte clientSalt;
        private byte serverSalt;
        private Dictionary<Type, Action<Packet>> packetCallbacks;
        private ConnectionStatus connectionStatus;
        private IngameStatus ingameStatus;
        private GameInfoPacket gameInfo;
        private long lastPacketTimestamp;
        private WorldManager manager;
        private int worldStateAck;

        private static CancellationTokenSource cts = new CancellationTokenSource();
        private static Random rnd = new Random(1312);

        /// <summary>
        /// Connection salt
        /// </summary>
        public byte XorSalt { get => (byte)(clientSalt ^ serverSalt); }
        public ConnectionStatus ConnectionStatus { get => connectionStatus; set => connectionStatus = value; }
        public IngameStatus IngameStatus { get => ingameStatus; set => ingameStatus = value; }
        public GameInfoPacket GameInfo { get => gameInfo; set => gameInfo = value; }
        public WorldManager Manager { get => manager; set => manager = value; }
        /// <summary>
        /// The last recieved and applied world state's ID
        /// </summary>
        public int WorldStateAck { get => worldStateAck; set => worldStateAck = value; }

        /// <summary>
        /// Called when a new game is initialized
        /// </summary>
        public event EventHandler<GameObject> GameInitialized;
        /// <summary>
        /// Called when running game ends
        /// </summary>
        public event EventHandler GameEnded;

        public Client(string host, WorldManager manager)
        {
            IPAddress serverIp = GetIpFromHostname(host);
            client = new CustomUdpClient();
            client.Connect(serverIp, Server.PORT);
            connectionStatus = ConnectionStatus.Disconnected;
            ingameStatus = IngameStatus.NotInGame;
            this.manager = manager;
            worldStateAck = -1;

            packetCallbacks = new Dictionary<Type, Action<Packet>>()
            {
                {
                    typeof(ChallengePacket), (Packet p) =>
                    {
                        ChallengePacket cp = (ChallengePacket)p;
                        serverSalt = cp.ServerSalt;
                        connectionStatus = ConnectionStatus.SendingResponse;
                    }
                },
                {
                    typeof(KeepClientAlivePacket), (Packet p) =>
                    {
                        lastPacketTimestamp = TimeUtils.GetTimestamp();
                        connectionStatus = ConnectionStatus.Connected;
                    }
                },
                {
                    typeof(DeclineConnectPacket), (Packet p) =>
                    {
                        connectionStatus = ConnectionStatus.Disconnected;
                    }
                },
                {
                    typeof(QueueResponsePacket), (Packet p) =>
                    {
                        lastPacketTimestamp = TimeUtils.GetTimestamp();
                        QueueResponsePacket qrp = (QueueResponsePacket)p;
                        if (qrp.GameId == -1) 
                        {
                            if (ingameStatus == IngameStatus.InGame || ingameStatus == IngameStatus.InRunningGame)
                            {
                                GameEnded?.Invoke(this, new EventArgs());
                                worldStateAck = -1;
                            }
                                
                            IngameStatus = IngameStatus.NotInGame; 
                        } 
                        else
                            IngameStatus = IngameStatus.InGame;
                    }
                },
                {
                    typeof(GameInfoPacket), (Packet p) =>
                    {
                        lastPacketTimestamp = TimeUtils.GetTimestamp();
                        gameInfo = (GameInfoPacket)p;
                    }
                },
                {
                    typeof(UpdatePacket), (Packet p) => 
                    {
                        lastPacketTimestamp = TimeUtils.GetTimestamp();
                        UpdatePacket up = (UpdatePacket)p;
                        if (up.Delta.OldId == worldStateAck) 
                        {
                            this.manager.Apply(up.Delta);
                            WorldStateAck = up.Delta.NewId;
                        }
                        GameObject playerObject = this.manager.GetObject(up.YourEntityId);
                        if (ingameStatus != IngameStatus.InRunningGame) 
                            GameInitialized?.Invoke(this, playerObject);
                        ingameStatus = IngameStatus.InRunningGame;
                    }
                }
            };
            client.PacketRecieved += PacketRecieved;
            client.StartListen();
        }

        private void PacketRecieved(object sender, Packet e)
        {
            Type t = e.GetType();
            if (packetCallbacks.ContainsKey(t))
                try
                {
                    packetCallbacks[t](e);
                }
                catch
                {
                    Console.WriteLine("Something went wrong handling package of type " + t.Name);
                }
        }

        /// <summary>
        /// Used to obtain server IP
        /// </summary>
        /// <param name="host">Hostname or IPAddress in dotted-decimal notation</param>
        /// <param name="af">The prefered AddressFamily that should be returned, defaults to Ipv4</param>
        private IPAddress GetIpFromHostname(string host, AddressFamily af = AddressFamily.InterNetwork)
        {
            IPHostEntry entry = Dns.GetHostEntry(host);
            foreach (IPAddress ip in entry.AddressList)
                if (ip.AddressFamily == af)
                    return ip;
            throw new Exception("No IP found");
        }

        /// <summary>
        /// Attemtps to connect to server
        /// </summary>
        public void StartConnecting()
        {
            if (connectionStatus != ConnectionStatus.Disconnected)
                return;
            ConnectionStatus = ConnectionStatus.SendingConnect;
            Task.Run(() =>
            {
                clientSalt = (byte)rnd.Next(byte.MaxValue);
                ConnectPacket p = new ConnectPacket(clientSalt);

                while (ConnectionStatus != ConnectionStatus.Connected)
                {
                    if (ConnectionStatus == ConnectionStatus.SendingConnect)
                    {
                        SendPacket(p);
                        Thread.Sleep(100);
                    }
                    else if (ConnectionStatus == ConnectionStatus.SendingResponse)
                    {
                        SendPacket(new ChallengeResponsePacket(clientSalt, serverSalt));
                        Thread.Sleep(100);
                    }
                    else if (ConnectionStatus == ConnectionStatus.Disconnected)
                        break;
                }
            }, cts.Token);
        }

        /// <summary>
        /// Queues for a game
        /// </summary>
        public void StartQueueing()
        {
            if (IngameStatus != IngameStatus.NotInGame)
                return;

            IngameStatus = IngameStatus.Queueing;
            Task.Run(() =>
            {
                QueuePacket qp = new QueuePacket(XorSalt);

                while (ingameStatus == IngameStatus.Queueing)
                {
                    client.Send(qp);
                    Thread.Sleep(100);
                }
                StartGettingGameInfo();
            }, cts.Token);
        }

        /// <summary>
        /// Requests game information from server and sets gameInfo property on success
        /// </summary>
        public void StartGettingGameInfo()
        {
            if (connectionStatus != ConnectionStatus.Connected || ingameStatus != IngameStatus.InGame)
                return; 
            GameInfoPacket outdated = gameInfo;
            Task.Run(() =>
            {
                GetGameInfoPacket ggip = new GetGameInfoPacket(XorSalt);
                while (gameInfo == outdated)
                {
                    client.Send(ggip);
                    Thread.Sleep(100);
                }
            }, cts.Token);
        }

        /// <summary>
        /// Checks whether no packets have been recieved for too long of a time
        /// </summary>
        public bool IsTimedOut()
        {
            if (connectionStatus != ConnectionStatus.Connected)
                return false;
            if (TimeUtils.GetTimestamp() - lastPacketTimestamp > Connection.TIME_OUT)
            {
                connectionStatus = ConnectionStatus.Disconnected;
                return true;
            }
            return false;
        }

        public void SendPacket(Packet p)
        {
            client.Send(p);
        }

        #region Disposal
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    client.Dispose();
                }

                isDisposed = true;
            }
        }

        ~Client()
        {
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Enum detailing on what stage of connecting a client is
    /// </summary>
    public enum ConnectionStatus
    {
        Disconnected = 0,
        SendingConnect = 1,
        SendingResponse = 2,
        Connected = 3
    }

    /// <summary>
    /// Enum detailing whether a client is in a game, and whether that game is running
    /// </summary>
    public enum IngameStatus
    {
        NotInGame = 0,
        Queueing,
        InGame,
        InRunningGame
    }
}

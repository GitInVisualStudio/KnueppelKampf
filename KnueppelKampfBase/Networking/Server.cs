using KnueppelKampfBase.Game;
using KnueppelKampfBase.Networking.Packets;
using KnueppelKampfBase.Networking.Packets.ClientPackets;
using KnueppelKampfBase.Networking.Packets.ServerPackets;
using KnueppelKampfBase.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking
{
    public class Server : IDisposable
    {
        private bool isDisposed;
        private bool isDoingCleanup;
        private CustomUdpClient listener;
        private Connection[] pending;
        private Connection[] connected;
        private Game[] games;

        private Dictionary<Type, Action<Packet>> packetCallbacks;
        private Dictionary<Type, Action<SaltedPacket, Connection>> connectedPacketCallbacks;

        private static CancellationTokenSource cts = new CancellationTokenSource();

        private const int PENDING_SLOTS = 64;
        private const int CONNECTED_SLOTS = 512;
        private const int GAME_SLOTS = 128;

        public const int PORT = 1337;

        /// <summary>
        /// Initializes serverobject with IP
        /// </summary>
        /// <param name="useLocalhost">Whether the server should use 127.0.0.1 ip or use its actual outgoing one</param>
        public Server(bool useLocalhost = false)
        {
            WorldManager.OnServer = true;

            isDisposed = false;
            isDoingCleanup = false;

            IPAddress ip;
            if (useLocalhost)
                ip = IPAddress.Parse("127.0.0.1");
            else
                ip = GetIP();
            //IPEndPoint iep = new IPEndPoint(ip, PORT);
            listener = new CustomUdpClient(PORT);
            listener.PacketRecieved += PacketReceived;

            pending = new Connection[PENDING_SLOTS];
            connected = new Connection[CONNECTED_SLOTS];
            games = new Game[GAME_SLOTS];

            packetCallbacks = new Dictionary<Type, Action<Packet>>()
            {
                {
                    typeof(StringPacket), (Packet p) =>
                    {
                        StringPacket sp = (StringPacket)p;
                        Console.WriteLine(sp.Content);
                    }
                },
                { typeof(ConnectPacket), (Packet p) =>
                    {
                        int connectionIndex = GetConnectionIndexFromIEP(pending, p.Sender);
                        ChallengePacket challenge;
                        ConnectPacket cp = (ConnectPacket)p;
                        if (connectionIndex == -1)
                        {
                            int i = GetFirstFreeIndex(pending);
                            if (i == -1)
                            {
                                listener.Send(new DeclineConnectPacket(), p.Sender);
                                return;
                            }
                            
                            challenge = new ChallengePacket(cp.ClientSalt);
                            pending[i] = new Connection(cp.Sender, cp.ClientSalt, challenge.ServerSalt);
                            pending[i].RefreshRecievedPacketTimestamp();
                        }
                        else
                        {
                            Connection c = pending[connectionIndex];
                            c.ClientSalt = cp.ClientSalt;
                            challenge = new ChallengePacket(c.ClientSalt, c.ServerSalt);
                            c.RefreshRecievedPacketTimestamp();
                        }
                        listener.Send(challenge, p.Sender);
                    }
                },
                {
                    typeof(ChallengeResponsePacket), (Packet p) =>
                    {
                        int connectionIndex = GetConnectionIndexFromIEP(pending, p.Sender);
                        if (connectionIndex == -1)
                            return;
                        Connection c = pending[connectionIndex];
                        ChallengeResponsePacket crp = (ChallengeResponsePacket)p;
                        if (crp.Xored == c.Xored) // response packet was correct
                        {
                            pending[connectionIndex] = null;
                            connectionIndex = GetFirstFreeIndex(connected);
                            if (connectionIndex == -1)
                            {
                                listener.Send(new DeclineConnectPacket(), p.Sender);
                                return;
                            }
                            connected[connectionIndex] = c;
                            c.RefreshRecievedPacketTimestamp();
                            KeepClientAlivePacket kcap = new KeepClientAlivePacket();
                            listener.Send(kcap, p.Sender);
                            return;
                        }
                        listener.Send(new DeclineConnectPacket(), p.Sender);
                    }
                }
            };

            connectedPacketCallbacks = new Dictionary<Type, Action<SaltedPacket, Connection>>()
            {
                {
                    typeof(KeepAlivePacket), (SaltedPacket p, Connection c) =>
                    {
                        c.RefreshRecievedPacketTimestamp();
                    }
                },
                {
                    typeof(QueuePacket), (SaltedPacket p, Connection c) =>
                    {
                        int gameIndex = GetGameIndexFromIep(p.Sender);
                        int gameId = -1;
                        if (gameIndex != -1)
                            gameId = games[gameIndex].Id;
                        else
                        {
                            lock(games)
                            {
                                foreach (Game g in games)
                                    if (g != null && g.AddConnection(c))
                                    {
                                        gameId = g.Id;
                                        break;
                                    }


                                if (gameId == -1)
                                {
                                    int newGameIndex = GetFirstFreeIndex(games);
                                    if (newGameIndex != -1)
                                    {
                                        Game g = new Game();
                                        games[newGameIndex] = g;
                                        g.AddConnection(c);
                                        gameId = g.Id;
                                        g.StartHandle(SendPacket);
                                        g.GameEnded += (object sender, EventArgs e) =>
                                        {
                                            lock (games)
                                                games[newGameIndex] = null;
                                        };
                                    }
                                }
                            }
                        }

                        QueueResponsePacket qrp = new QueueResponsePacket(gameId);
                        listener.Send(qrp, p.Sender);
                        c.RefreshSentPacketTimestamp();
                    }
                },
                {
                    typeof(GetGameInfoPacket), (SaltedPacket p, Connection c) =>
                    {
                        int gameIndex = GetGameIndexFromIep(c.Client);
                        if (gameIndex == -1)
                            return;

                        Game g = games[gameIndex];
                        GameInfoPacket ggip = new GameInfoPacket(g);
                        listener.Send(ggip, p.Sender);
                        c.RefreshSentPacketTimestamp();
                    }
                },
                {
                    typeof(InputPacket), (SaltedPacket p, Connection c) =>
                    {
                        int gameIndex = GetGameIndexFromIep(c.Client);
                        if (gameIndex == -1)
                        {
                            SendPacket(new QueueResponsePacket(-1), c.Client);
                            return;
                        }

                        Game g = games[gameIndex];
                        InputPacket inpt = (InputPacket)p;
                        g.HandleInputPacket(inpt, c);
                    }
                }
            };
        }

        /// <summary>
        /// Currently unnecessary but may become useful once deployed on actual server
        /// </summary>
        /// <returns></returns>
        private IPAddress GetIP()
        {
            IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in entry.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork && !ip.Equals(IPAddress.Parse("127.0.0.1")))
                    return ip;

            throw new Exception("No IPAddress found");
        }

        private void PacketReceived(object sender, Packet packet)
        {
            Type packetType = packet.GetType();
            if (!(packet is SaltedPacket))
            {
                if (packetCallbacks.ContainsKey(packetType))
                    packetCallbacks[packetType](packet);
            }
            else
            {
                int connectionIndex = GetConnectionIndexFromIEP(connected, packet.Sender);
                if (connectionIndex == -1)
                    return;

                Connection c = connected[connectionIndex];
                if (c == null)
                    return;
                c.RefreshRecievedPacketTimestamp();
                SaltedPacket sp = (SaltedPacket)packet;
                if (c.Xored != sp.Salt) // Packet had invalid salt
                    return;

                if (connectedPacketCallbacks.ContainsKey(packetType))
                    connectedPacketCallbacks[packetType].Invoke(sp, c);
            }
        }

        public void StartListen()
        {
            listener.StartListen();
        }

        public void StopListen()
        {
            listener.StopListen();
        }

        private void SendPacket(Packet p, IPEndPoint iep)
        {
            listener.Send(p, iep);
        }

        #region ArrayHelpers
        private int GetFirstFreeIndex(object[] array)
        {
            lock (array)
            {
                for (int i = 0; i < array.Length; i++)
                    if (array[i] == null)
                        return i;
            }
            return -1;
        }

        private int GetConnectionIndexFromIEP(Connection[] array, IPEndPoint iep)
        {
            lock (array)
            {
                for (int i = 0; i < array.Length; i++)
                    if (array[i] != null && array[i].Client.Equals(iep))
                        return i;
            }
            return -1;
        }

        private int GetGameIndexFromIep(IPEndPoint iep)
        {
            lock (games)
            {
                for (int i = 0; i < games.Length; i++)
                    if (games[i] != null && Array.Find(games[i].Connections, x => x != null && x.Client.Equals(iep)) != null)
                        return i;
            }
            return -1;
        }
        #endregion

        /// <summary>
        /// Starts a thread that times out connections or sends keep alive packets
        /// </summary>
        public void StartCleanupThread()
        {
            if (isDoingCleanup)
                return;

            isDoingCleanup = true;
            Task.Run(() =>
            {
                while (true)
                {
                    TimeoutConnections();
                    KeepClientAlivePacket kcap = new KeepClientAlivePacket();
                    lock (connected)
                    {
                        for (int i = 0; i < connected.Length; i++)
                            if (connected[i] != null && TimeUtils.GetTimestamp() - connected[i].LastSentPacketTimestamp > 1)
                                listener.Send(kcap, connected[i].Client);
                    }
                    Thread.Sleep(100);
                }
            }, cts.Token);
        }

        public void StopTimeoutThread()
        {
            cts.Cancel();
            isDoingCleanup = false;
        }

        private void TimeoutConnections()
        {
            TimeoutConnections(pending);
            TimeoutConnections(connected);
        }

        private void TimeoutConnections(Connection[] array)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] != null && array[i].IsTimedOut())
                {
                    lock (games)
                        lock (array)
                        {
                            int gameIndex = GetGameIndexFromIep(array[i].Client);
                            if (gameIndex != -1)
                            {
                                games[gameIndex].TimeoutConnection(array[i]);
                                if (games[gameIndex].GetPlayersConnected() == 0)
                                    games[gameIndex] = null;
                            }
                            array[i] = null;
                        }
                }
        }

        #region Disposal
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes this object
        /// </summary>
        /// <param name="disposing">Whether this is being called from Dispose method or from destructor</param>
        protected virtual void Dispose(bool disposing = true)
        {
            if (!isDisposed)
            {
                listener.Dispose();
                isDisposed = true;
            }
        }

        ~Server()
        {
            Dispose(false);
        }
        #endregion
    }
}

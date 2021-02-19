using KnueppelKampfBase.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking
{
    public class Client : IDisposable
    {
        private bool isDisposed;
        private CustomUdpClient client;
        private byte clientSalt;
        private byte serverSalt;
        private Dictionary<Type, Action<Packet>> packetCallbacks;
        private ConnectionStatus connectionStatus;

        private static CancellationTokenSource cts = new CancellationTokenSource();

        public Client(string host)
        {
            IPAddress serverIp = GetIpFromHostname(host);
            client = new CustomUdpClient();
            client.Connect(serverIp, Server.PORT);
            connectionStatus = ConnectionStatus.Disconnected;
            packetCallbacks = new Dictionary<Type, Action<Packet>>()
            {
                {
                    typeof(ChallengePacket), (Packet p) =>
                    {
                        ChallengePacket cp = (ChallengePacket)p;
                        serverSalt = cp.ServerSalt;
                        connectionStatus = ConnectionStatus.SendingResponse;
                        ChallengeResponsePacket crp = new ChallengeResponsePacket(clientSalt, serverSalt);
                        SendPacket(crp);
                    }
                },
                {
                    typeof(FullWorldPacket), (Packet p) =>
                    {
                        connectionStatus = ConnectionStatus.Connected;
                        Console.WriteLine("Connected!");
                    }
                }
            };
            client.StartListen();
            client.PacketRecieved += PacketRecieved;
        }

        private void PacketRecieved(object sender, Packet e)
        {
            if (packetCallbacks.ContainsKey(e.GetType()))
                packetCallbacks[e.GetType()](e);
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

        public void StartConnecting()
        {
            if (connectionStatus != ConnectionStatus.Disconnected)
                return;
            connectionStatus = ConnectionStatus.SendingConnect;
            Task.Run(() =>
            {
                while (connectionStatus != ConnectionStatus.Connected)
                {
                    if (connectionStatus == ConnectionStatus.SendingConnect)
                    {
                        ConnectPacket p = new ConnectPacket();
                        clientSalt = p.ClientSalt;
                        SendPacket(p);
                        Thread.Sleep(100);
                    }
                    else if (connectionStatus == ConnectionStatus.SendingResponse)
                    {
                        SendPacket(new ChallengeResponsePacket(clientSalt, serverSalt));
                        Thread.Sleep(100);
                    }
                    else if (connectionStatus == ConnectionStatus.Disconnected)
                        break;
                }
            }, cts.Token);
        }

        public void SendPacket(Packet p)
        {
            client.Send(p);
        }

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
    }

    enum ConnectionStatus
    {
        Disconnected = 0,
        SendingConnect = 1,
        SendingResponse = 2,
        Connected = 3
    }
}

using KnueppelKampfBase.Networking.Packets;
using KnueppelKampfBase.Networking.Packets.ClientPackets;
using KnueppelKampfBase.Networking.Packets.ServerPackets;
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
        private CustomUdpClient listener;
        private Connection[] pending;
        private Connection[] connected;

        private Dictionary<Type, Action<Packet>> packetCallbacks;
        private Dictionary<Type, Action<SaltedPacket, Connection>> connectedPacketCallbacks;

        private const int PENDING_SLOTS = 64;
        private const int CONNECTED_SLOTS = 512;

        public const int PORT = 1337;

        /// <summary>
        /// Initializes serverobject with IP
        /// </summary>
        /// <param name="useLocalhost">Whether the server should use 127.0.0.1 ip or use its actual outgoing one</param>
        public Server(bool useLocalhost = false)
        {
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
                            pending[i].RefreshPacketTimestamp();
                        }
                        else
                        {
                            Connection c = pending[connectionIndex];
                            c.ClientSalt = cp.ClientSalt;
                            challenge = new ChallengePacket(c.ClientSalt, c.ServerSalt);
                            c.RefreshPacketTimestamp();
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
                            c.RefreshPacketTimestamp();
                            FullWorldPacket fwp = new FullWorldPacket();
                            listener.Send(fwp, p.Sender);
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
                        c.RefreshPacketTimestamp();
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

        private int GetFirstFreeIndex(object[] array)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == null)
                    return i;
            return -1;
        }

        private int GetConnectionIndexFromIEP(Connection[] array, IPEndPoint iep)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] != null && array[i].Client.Equals(iep))
                    return i;
            return -1;
        }

        public void TimeoutConnections()
        {
            TimeoutConnections(pending);
            TimeoutConnections(connected);
        }

        private void TimeoutConnections(Connection[] array)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i].IsTimedOut())
                    array[i] = null;
        }

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
    }
}

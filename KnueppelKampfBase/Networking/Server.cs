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
    public class Server : IDisposable
    {
        private bool isDisposed;
        private CustomUdpClient listener;

        public const int PORT = 1337;

        public Server(bool useLocalhost = false)
        {
            IPAddress ip;
            if (useLocalhost)
                ip = IPAddress.Parse("127.0.0.1");
            else
                ip = GetIP();
            IPEndPoint iep = new IPEndPoint(ip, PORT);
            listener = new CustomUdpClient(PORT);
            listener.PacketRecieved += PacketReceived;
        }

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
            if (packet is StringPacket)
            {
                Console.WriteLine(((StringPacket)packet).Content);
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

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KnueppelKampfBase.Networking
{
    public class Client : IDisposable
    {
        private bool isDisposed;
        private CustomUdpClient client;

        public Client(string host)
        {
            IPAddress serverIp = GetIpFromHostname(host);
            client = new CustomUdpClient();
            client.Connect(serverIp, Server.PORT);
        }

        private IPAddress GetIpFromHostname(string host, AddressFamily af = AddressFamily.InterNetwork)
        {
            IPHostEntry entry = Dns.GetHostEntry(host);
            foreach (IPAddress ip in entry.AddressList)
                if (ip.AddressFamily == af)
                    return ip;
            throw new Exception("No IP found");
        }

        public void SendPacket(Packet p)
        {
            byte[] bytes = p.ToBytes();
            client.Send(bytes, bytes.Length);
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
}

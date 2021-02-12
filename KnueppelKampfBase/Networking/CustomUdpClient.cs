using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking
{
    public class CustomUdpClient : UdpClient
    {
        private CancellationTokenSource cts;
        private bool isDisposed;

        public event EventHandler<Packet> PacketRecieved;

        public CustomUdpClient() : base()
        {
            cts = new CancellationTokenSource();
        }

        public CustomUdpClient(int port) : base(port)
        {
            cts = new CancellationTokenSource();
        }

        public void StartListen()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, Server.PORT);
                    byte[] recieved = Receive(ref sender);
                    Console.WriteLine("Recieved data from " + sender.ToString());
                    Packet p = Packet.FromBytes(recieved);
                    p.Sender = sender;
                    PacketRecieved?.Invoke(this, p);
                }
            }, cts.Token);
        }

        public void StopListen()
        {
            cts.Cancel();
        }

        protected override void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    StopListen();
                    cts.Dispose();
                }
                base.Dispose(disposing);
                isDisposed = true;
            }
        }
    }
}

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
    /// UdpClient containing an event raised when a packet is recieved
    /// </summary>
    public class CustomUdpClient : UdpClient
    {
        /// <summary>
        /// Used to gracefully abort listening thread, has to be initialized!
        /// </summary>
        private CancellationTokenSource cts;
        private bool isDisposed;
        private bool isListening;

        private CancellationTokenSource Cts
        {
            get
            {
                if (cts == null)
                    cts = new CancellationTokenSource();
                return cts;
            }
        }

        public event EventHandler<Packet> PacketRecieved;

        public CustomUdpClient() : base()
        {
        }

        public CustomUdpClient(int port) : base(port)
        {
        }

        /// <summary>
        /// Starts listening thread, raising PacketRecieved events whenever a new valid Packet comes in
        /// </summary>
        public void StartListen()
        {
            if (isListening)
                return;
            isListening = true;

            Task.Run(() =>
            {
                while (true)
                {
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, Server.PORT);
                    byte[] recieved = Receive(ref sender);
                    Console.WriteLine("Recieved data from " + sender.ToString());
                    Packet p;
                    try
                    {
                        p = Packet.FromBytes(recieved);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Recieved an invalid packet");
                        continue;
                    }
                    p.Sender = sender;
                    PacketRecieved?.Invoke(this, p);
                }
            }, Cts.Token);
        }

        /// <summary>
        /// Aborts listening thread
        /// </summary>
        public void StopListen()
        {
            Cts.Cancel();
            isListening = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    StopListen();
                    Cts.Dispose();
                }
                base.Dispose(disposing);
                isDisposed = true;
            }
        }
    }
}

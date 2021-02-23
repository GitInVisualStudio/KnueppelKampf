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
        private CancellationTokenSource cts = new CancellationTokenSource();
        private bool isDisposed;
        private bool isListening;

        public event EventHandler<Packet> PacketRecieved;

        public CustomUdpClient() : base()
        {
            DontFragment = true;
        }

        public CustomUdpClient(int port) : base(port)
        {
            DontFragment = true;
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
                    byte[] recieved = Receive(ref sender); // blocks, recieves bytes and fills sender object with sender of packet
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
                    Console.WriteLine("Recieved " + p.GetType().Name +  " from " + sender.ToString());
                    PacketRecieved?.Invoke(this, p);
                }
            }, cts.Token);
        }

        /// <summary>
        /// Aborts listening thread
        /// </summary>
        public void StopListen()
        {
            cts.Cancel();
            isListening = false;
        }

        /// <summary>
        /// Sends a packet to the specified ip
        /// </summary>
        public void Send(Packet p, IPEndPoint iep)
        {
            byte[] bytes = p.ToBytes();
            Send(bytes, bytes.Length, iep);
        }

        /// <summary>
        /// Sends a packet to the connected ip
        /// </summary>

        public void Send(Packet p)
        {
            byte[] bytes = p.ToBytes();
            Send(bytes, bytes.Length);
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

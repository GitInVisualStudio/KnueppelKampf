﻿using System;
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
        /// Used to gracefully abort listening thread
        /// </summary>
        private CancellationTokenSource cts = new CancellationTokenSource();
        private bool isDisposed;
        private bool isListening;

        public event EventHandler<Packet> PacketRecieved;

        public CustomUdpClient() : base()
        {
            DontFragment = true; // prevents packets from being fragmented into multiples
        }

        public CustomUdpClient(int port) : base(port)
        {
            DontFragment = true;
        }

        public CustomUdpClient(IPEndPoint iep) : base(iep)
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
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                    byte[] recieved;
                    try
                    {
                        recieved = Receive(ref sender); // blocks, recieves bytes and fills sender object with sender of packet
                    }
                    catch (SocketException e)
                    {
                        continue; // this shouldnt fucking happen but sometimes it does :)
                    }
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

        private string PrintBytes(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes)
                result += b.ToString() + ",";
            return result;
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

using KnueppelKampfBase.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace KnueppelKampfBase.Networking
{
    class Connection
    {
        private IPEndPoint client;
        private byte clientSalt;
        private byte serverSalt;
        private byte xored;
        private float lastPacketTimestamp;

        public IPEndPoint Client { get => client; set => client = value; }
        public byte ClientSalt { get => clientSalt; set => clientSalt = value; }
        public byte ServerSalt { get => serverSalt; set => serverSalt = value; }
        public float LastPacketTimestamp { get => lastPacketTimestamp; set => lastPacketTimestamp = value; }
        public byte Xored { get => xored; set => xored = value; }

        /// <summary>
        /// Time in seconds where no packets are recieved until connection times out
        /// </summary>
        public const int TIME_OUT = 10;

        public Connection(IPEndPoint client, byte clientSalt, byte serverSalt)
        {
            this.client = client;
            this.clientSalt = clientSalt;
            this.serverSalt = serverSalt;
            xored = (byte)(serverSalt ^ clientSalt);
        }

        private float GetTimestamp()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }

        public void RefreshPacketTimestamp()
        {
            lastPacketTimestamp = GetTimestamp();
        }

        public bool IsTimedOut()
        {
            float currentTimestamp = GetTimestamp();
            return currentTimestamp - lastPacketTimestamp > TIME_OUT;
        }
    }
}

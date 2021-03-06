﻿using KnueppelKampfBase.Game;
using KnueppelKampfBase.Networking.Packets;
using KnueppelKampfBase.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace KnueppelKampfBase.Networking
{
    public class Connection
    {
        private IPEndPoint client;
        private byte clientSalt;
        private byte serverSalt;
        private byte xored;
        private long lastRecievedPacketTimestamp;
        private long lastSentPacketTimestamp;
        private WorldState lastAck;

        public IPEndPoint Client { get => client; set => client = value; }
        public byte ClientSalt { get => clientSalt; set => clientSalt = value; }
        public byte ServerSalt { get => serverSalt; set => serverSalt = value; }
        /// <summary>
        /// Timestamp at which the last packet was recieved
        /// </summary>
        public long LastRecievedPacketTimestamp { get => lastRecievedPacketTimestamp; }
        /// <summary>
        /// Timestamp at which the last packet was sent
        /// </summary>
        public long LastSentPacketTimestamp { get => lastSentPacketTimestamp; }
        /// <summary>
        /// This connection's salt
        /// </summary>
        public byte Xored { get => xored; set => xored = value; }
        /// <summary>
        /// The last world state this client acknowledged
        /// </summary>
        public WorldState LastAck { get => lastAck; set => lastAck = value; }

        /// <summary>
        /// Time in seconds where no packets are recieved until connection times out
        /// </summary>
        public const int TIME_OUT = 10;
        /// <summary>
        /// Length of this object's byte array after serialization
        /// </summary>
        public const int BYTE_LENGTH = sizeof(long) + sizeof(int);

        public Connection(IPEndPoint client, byte clientSalt, byte serverSalt)
        {
            this.client = client;
            this.clientSalt = clientSalt;
            this.serverSalt = serverSalt;
            xored = (byte)(clientSalt ^ serverSalt);
        }

        public void RefreshRecievedPacketTimestamp()
        {
            lastRecievedPacketTimestamp = TimeUtils.GetTimestamp();
        }

        public void RefreshSentPacketTimestamp()
        {
            lastSentPacketTimestamp = TimeUtils.GetTimestamp();
        }

        public bool IsTimedOut()
        {
            long currentTimestamp = TimeUtils.GetTimestamp();
            return currentTimestamp - lastRecievedPacketTimestamp > TIME_OUT;
        }

        public void ToBytes(byte[] buffer, int startingIndex)
        {
            BitConverter.GetBytes(client.Address.Address).CopyTo(buffer, startingIndex);
            BitConverter.GetBytes(client.Port).CopyTo(buffer, startingIndex + sizeof(long));
        }
    }
}

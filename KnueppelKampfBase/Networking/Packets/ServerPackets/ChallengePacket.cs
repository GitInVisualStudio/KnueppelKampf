using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Networking.Packets.ServerPackets
{
    /// <summary>
    /// Sent by server in response to ConnectPacket
    /// </summary>
    class ChallengePacket : Packet
    {
        private byte clientSalt;
        private byte serverSalt;

        public byte ClientSalt { get => clientSalt; set => clientSalt = value; }
        public byte ServerSalt { get => serverSalt; set => serverSalt = value; }

        public ChallengePacket(byte clientSalt)
        {
            this.clientSalt = clientSalt;
            this.serverSalt = (byte)rnd.Next(byte.MaxValue);
        }

        public ChallengePacket(byte clientSalt, byte serverSalt)
        {
            this.clientSalt = clientSalt;
            this.serverSalt = serverSalt;
        }

        public ChallengePacket(byte[] bytes) : base(bytes)
        {
            clientSalt = bytes[HEADER_SIZE];
            serverSalt = bytes[HEADER_SIZE + 1];
        }

        public override byte[] ToBytes()
        {
            byte[] bytes = GetHeader(HEADER_SIZE + 2);
            bytes[HEADER_SIZE] = clientSalt;
            bytes[HEADER_SIZE + 1] = serverSalt;
            return bytes;
        }
    }
}

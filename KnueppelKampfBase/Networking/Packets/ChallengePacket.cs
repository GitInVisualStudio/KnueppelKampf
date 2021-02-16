using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Networking.Packets
{
    /// <summary>
    /// Sent by server in response to ConnectPacket
    /// </summary>
    class ChallengePacket : Packet
    {
        private byte clientSalt;
        private byte serverSalt;

        public ChallengePacket(byte clientSalt)
        {
            this.clientSalt = clientSalt;
            this.serverSalt = (byte)rnd.Next(byte.MaxValue);
        }

        public ChallengePacket(byte[] bytes) : base(bytes)
        {
            clientSalt = bytes[HEADER_SIZE];
            serverSalt = bytes[HEADER_SIZE + 1];
        }

        public override byte[] ToBytes()
        {
            byte[] result = new byte[HEADER_SIZE + 2];
            byte[] header = base.ToBytes();
            header.CopyTo(result, 0);
            result[HEADER_SIZE] = clientSalt;
            result[HEADER_SIZE + 1] = serverSalt;
            return result;
        }
    }
}

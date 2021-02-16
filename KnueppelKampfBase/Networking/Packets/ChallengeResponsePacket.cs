using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Networking.Packets
{
    /// <summary>
    /// Packet sent by client in response to ChallengePacket to authenticate
    /// </summary>
    class ChallengeResponsePacket : Packet
    {
        private byte xored;

        public ChallengeResponsePacket(byte clientSalt, byte serverSalt)
        {
            xored = (byte)(clientSalt ^ serverSalt);
        }

        public ChallengeResponsePacket(byte[] bytes) : base(bytes)
        {
            xored = bytes[HEADER_SIZE];
        }

        public override byte[] ToBytes()
        {
            byte[] result = new byte[HEADER_SIZE + 1];
            byte[] header = base.ToBytes();
            header.CopyTo(result, 0);
            header[HEADER_SIZE] = xored;
            return result;
        }
    }
}

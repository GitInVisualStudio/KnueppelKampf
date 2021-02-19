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

        public byte Xored { get => xored; set => xored = value; }

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
            byte[] result = GetHeader(HEADER_SIZE + 1);
            result[HEADER_SIZE] = xored;
            return result;
        }
    }
}

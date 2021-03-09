using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Networking.Packets.ClientPackets
{
    /// <summary>
    /// Any packet sent by client after connection was established has to derive from this type, neaning
    /// any packet except ConnectPacket, ChallengeResponsePacket and, for debug purposes, StringPacket
    /// </summary>
    public abstract class SaltedPacket : Packet
    {
        private byte salt;

        public byte Salt { get => salt; set => salt = value; }

        protected new const int HEADER_SIZE = Packet.HEADER_SIZE + 1;

        public SaltedPacket(byte salt)
        {
            this.salt = salt;
        }

        public SaltedPacket(byte[] bytes) : base(bytes)
        {
            salt = bytes[Packet.HEADER_SIZE];
        }

        protected override byte[] GetHeader(int size = HEADER_SIZE)
        {
            if (size < HEADER_SIZE)
                throw new Exception("Invalid size");
            byte[] header = base.GetHeader(size);
            header[HEADER_SIZE - 1] = salt;
            return header;
        }
    }
}

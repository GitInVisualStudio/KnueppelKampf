using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Networking.Packets.ServerPackets
{
    /// <summary>
    /// Packet sent by server if its pending or connected array is full
    /// </summary>
    class DeclineConnectPacket : Packet
    {
        public DeclineConnectPacket()
        {

        }

        public DeclineConnectPacket(byte[] bytes) : base(bytes)
        {

        }

        public override byte[] ToBytes()
        {
            return GetHeader();
        }
    }
}

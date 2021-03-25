using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Networking.Packets.ClientPackets
{
    /// <summary>
    /// Packet sent by client to prevent timeout when there are no other packets to send
    /// </summary>
    public class KeepAlivePacket : SaltedPacket
    {
        public KeepAlivePacket(byte salt) : base(salt)
        {

        }

        public KeepAlivePacket(byte[] bytes) : base(bytes)
        {

        }

        public override byte[] ToBytes()
        {
            return GetHeader();
        }
    }
}

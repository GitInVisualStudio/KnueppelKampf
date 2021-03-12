using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Networking.Packets.ClientPackets
{
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

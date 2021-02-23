using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Networking.Packets
{
    class KeepAlivePacket : Packet
    {
        public KeepAlivePacket()
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

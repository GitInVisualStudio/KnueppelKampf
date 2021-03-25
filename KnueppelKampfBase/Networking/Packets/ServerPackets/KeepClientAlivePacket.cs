using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking.Packets.ServerPackets
{
    /// <summary>
    /// Packet sent by server to keep connection alive when there are no other packets to send
    /// </summary>
    public class KeepClientAlivePacket : Packet
    {
        public KeepClientAlivePacket()
        {

        }

        public KeepClientAlivePacket(byte[] bytes) : base(bytes)
        {

        }

        public override byte[] ToBytes()
        {
            return GetHeader();
        }
    }
}

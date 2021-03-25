using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking.Packets.ClientPackets
{
    /// <summary>
    /// Packet sent by client to queue for a game
    /// </summary>
    public class QueuePacket : SaltedPacket
    {
        public QueuePacket(byte salt) : base(salt)
        {

        }

        public QueuePacket(byte[] bytes) : base(bytes)
        {

        }

        public override byte[] ToBytes()
        {
            return GetHeader();
        }
    }
}

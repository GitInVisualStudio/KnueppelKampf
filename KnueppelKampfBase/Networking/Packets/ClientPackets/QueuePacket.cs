using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking.Packets.ClientPackets
{
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

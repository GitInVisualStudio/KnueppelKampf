using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking.Packets.ClientPackets
{
    /// <summary>
    /// Packet sent by client to get information about its current game
    /// </summary>
    class GetGameInfoPacket : SaltedPacket
    {
        public GetGameInfoPacket(byte salt) : base(salt)
        {

        }

        public GetGameInfoPacket(byte[] bytes) : base(bytes)
        {

        }

        public override byte[] ToBytes()
        {
            return GetHeader();
        }
    }
}

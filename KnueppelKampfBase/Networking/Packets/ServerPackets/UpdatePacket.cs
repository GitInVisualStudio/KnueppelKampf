using KnueppelKampfBase.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking.Packets.ServerPackets
{
    public class UpdatePacket : Packet // TODO
    {
        private WorldDelta delta;

        public WorldDelta Delta { get => delta; set => delta = value; }

        public UpdatePacket(WorldDelta wd)
        {
            delta = wd;
        }

        public UpdatePacket(byte[] bytes) : base(bytes)
        {
            
        }

        public override byte[] ToBytes()
        {
            return GetHeader();
        }
    }
}

using KnueppelKampfBase.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking.Packets.ServerPackets
{
    /// <summary>
    /// Packet sent by server to update a client's world manager
    /// </summary>
    public class UpdatePacket : Packet
    {
        private WorldDelta delta;
        private int yourEntityId;

        public WorldDelta Delta { get => delta; set => delta = value; }
        /// <summary>
        /// ID of the entity controlled by client
        /// </summary>
        public int YourEntityId { get => yourEntityId; set => yourEntityId = value; }

        public UpdatePacket(WorldDelta wd)
        {
            delta = wd;
        }

        public UpdatePacket(byte[] bytes) : base(bytes)
        {
            yourEntityId = BitConverter.ToInt32(bytes, HEADER_SIZE);
            delta = new WorldDelta(bytes, HEADER_SIZE + sizeof(int));
        }

        public override byte[] ToBytes()
        {
            byte[] header = GetHeader(MAX_SIZE); // just gotta hope this is enough lol // fuck it isnt
            BitConverter.GetBytes(yourEntityId).CopyTo(header, HEADER_SIZE);
            int size = delta.ToBytes(header, HEADER_SIZE + sizeof(int)) + HEADER_SIZE + sizeof(int);
            Array.Resize(ref header, size);
            return header;
        }
    }
}

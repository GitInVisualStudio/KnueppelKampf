using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Networking.Packets
{
    /// <summary>
    /// Packet containing the entire world state, sent by server
    /// </summary>
    class FullWorldPacket : Packet //TODO
    {
        private int mapId;

        public int MapId { get => mapId; set => mapId = value; }

        public FullWorldPacket()
        {
            mapId = 1312;
        }

        public FullWorldPacket(byte[] bytes) : base(bytes)
        {
            mapId = BitConverter.ToInt32(bytes, HEADER_SIZE);
        }

        public override byte[] ToBytes()
        {
            byte[] result = GetHeader(HEADER_SIZE + 4);
            BitConverter.GetBytes(mapId).CopyTo(result, HEADER_SIZE);
            return result;
        }
    }
}

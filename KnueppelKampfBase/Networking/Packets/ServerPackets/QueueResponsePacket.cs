using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking.Packets.ServerPackets
{
    public class QueueResponsePacket : Packet
    {
        private int gameId;

        public int GameId { get => gameId; set => gameId = value; }

        public QueueResponsePacket(int gameId)
        {
            this.gameId = gameId;
        }

        public QueueResponsePacket(byte[] bytes) : base(bytes)
        {
            gameId = BitConverter.ToInt32(bytes, HEADER_SIZE);
        }

        public override byte[] ToBytes()
        {
            byte[] result = GetHeader(HEADER_SIZE + sizeof(int));
            BitConverter.GetBytes(gameId).CopyTo(result, HEADER_SIZE);
            return result;
        }
    }
}

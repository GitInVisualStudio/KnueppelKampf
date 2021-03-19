using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking.Packets.ServerPackets
{
    public class GameInfoPacket : Packet
    {
        private int gameId;
        private Connection[] players;

        public GameInfoPacket(Game g)
        {
            gameId = g.Id;
            players = g.Connections;
        }

        public GameInfoPacket(byte[] bytes) : base(bytes)
        {
            gameId = BitConverter.ToInt32(bytes, HEADER_SIZE);
            int connectionsAmount = (bytes.Length - HEADER_SIZE - sizeof(int)) / Connection.BYTE_LENGTH;
            players = new Connection[connectionsAmount];
            for (int i = 0; i < players.Length; i++)
            {
                long ip = BitConverter.ToInt64(bytes, HEADER_SIZE + sizeof(int) + i * Connection.BYTE_LENGTH);
                int port = BitConverter.ToInt32(bytes, HEADER_SIZE + sizeof(int) + i * Connection.BYTE_LENGTH + sizeof(long));
                IPEndPoint iep = new IPEndPoint(ip, port);
                players[i] = new Connection(iep, 0, 0);
            }
        }

        public override byte[] ToBytes()
        {
            byte[] result = GetHeader(HEADER_SIZE + sizeof(int) + players.Length * Connection.BYTE_LENGTH);
            BitConverter.GetBytes(gameId).CopyTo(result, HEADER_SIZE);
            for (int i = 0; i < players.Length; i++)
                if (players[i] != null)
                    players[i].ToBytes(result, HEADER_SIZE + sizeof(int) + i * Connection.BYTE_LENGTH);
            return result;
        }

        public override string ToString()
        {
            string result = "GameID: " + gameId;
            for (int i = 0; i < players.Length; i++)
                result += "\nPlayer " + i + ": " + players[i].Client.ToString();

            return result;
        }
    }
}

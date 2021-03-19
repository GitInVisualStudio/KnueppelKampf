using KnueppelKampfBase.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking.Packets.ClientPackets
{
    public class InputPacket : SaltedPacket
    {
        private int worldStateAck;
        private GameAction[] actions;

        /// <summary>
        /// The id of the last WorldState the client recieved
        /// </summary>
        public int WorldStateAck { get => worldStateAck; set => worldStateAck = value; }
        public GameAction[] Actions { get => actions; set => actions = value; }

        public InputPacket(byte salt, GameAction[] actions, int worldStateAck) : base(salt)
        {
            this.actions = actions;

            this.worldStateAck = worldStateAck;
        }

        public InputPacket(byte[] bytes) : base(bytes)
        {
            worldStateAck = BitConverter.ToInt32(bytes, HEADER_SIZE);
            byte actionByte = bytes[HEADER_SIZE + sizeof(int)];
            actions = new GameAction[GetSetBits(actionByte)];
            GameAction[] values = (GameAction[])Enum.GetValues(typeof(GameAction));
            int lastSet = 0;
            foreach (GameAction action in values)
                if (((byte)action & actionByte) > 0)
                    actions[lastSet++] = action;
        }

        private int GetSetBits(byte b)
        {
            int count = 0;
            for (int i = 0; i < 8; i++)
            {
                count += b & 1;
                b = (byte)(b >> 1);
            }
            return count;
        }

        public override byte[] ToBytes()
        {
            byte[] result = GetHeader(HEADER_SIZE + sizeof(int) + 1);
            BitConverter.GetBytes(worldStateAck).CopyTo(result, HEADER_SIZE);
            for (int i = 0; i < actions.Length; i++)
                result[HEADER_SIZE + sizeof(int)] += (byte)actions[i];
            return result;
        }
    }
}

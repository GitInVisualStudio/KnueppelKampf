using KnueppelKampfBase.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking.Packets.ClientPackets
{
    /// <summary>
    /// Packet sent by client for server to handle inputs & rotation. Also contains the last recieved world state ID
    /// </summary>
    public class InputPacket : SaltedPacket
    {
        private int worldStateAck;
        private GameAction[] actions;
        private float rotation;

        /// <summary>
        /// The ID of the last WorldState the client recieved
        /// </summary>
        public int WorldStateAck { get => worldStateAck; set => worldStateAck = value; }
        public GameAction[] Actions { get => actions; set => actions = value; }
        public float Rotation { get => rotation; set => rotation = value; }

        public InputPacket(byte salt, GameAction[] actions, int worldStateAck, float rotation) : base(salt)
        {
            this.actions = actions;
            this.worldStateAck = worldStateAck;
            this.rotation = rotation;
        }

        public InputPacket(byte[] bytes) : base(bytes)
        {
            worldStateAck = BitConverter.ToInt32(bytes, HEADER_SIZE);
            rotation = BitConverter.ToSingle(bytes, HEADER_SIZE + sizeof(int));
            byte actionByte = bytes[HEADER_SIZE + sizeof(int) + sizeof(float)];
            actions = new GameAction[GetSetBits(actionByte)];
            GameAction[] values = (GameAction[])Enum.GetValues(typeof(GameAction));
            int lastSet = 0;
            foreach (GameAction action in values)
                if (((byte)action & actionByte) > 0)
                    actions[lastSet++] = action;
        }

        /// <summary>
        /// Returns the number of bits in a byte that equal 1
        /// </summary>
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
            byte[] result = GetHeader(HEADER_SIZE + sizeof(int) + sizeof(float) + 1);
            BitConverter.GetBytes(worldStateAck).CopyTo(result, HEADER_SIZE);
            BitConverter.GetBytes(rotation).CopyTo(result, HEADER_SIZE + sizeof(float));
            for (int i = 0; i < actions.Length; i++)
                result[HEADER_SIZE + sizeof(int) + sizeof(float)] += (byte)actions[i];
            return result;
        }
    }
}

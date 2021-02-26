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

        public InputPacket(byte salt, GameAction[] actions) : base(salt)
        {
            this.actions = actions;

            worldStateAck = 1337; // TODO
        }

        public InputPacket(byte[] bytes) : base(bytes)
        {
            worldStateAck = BitConverter.ToInt32(bytes, HEADER_SIZE);
            actions = new GameAction[bytes.Length - sizeof(int) - HEADER_SIZE];
            for (int i = 0; i < actions.Length; i++)
                actions[i] = (GameAction)bytes[HEADER_SIZE + sizeof(int)];
        }

        public override byte[] ToBytes()
        {
            byte[] result = GetHeader(HEADER_SIZE + sizeof(int) + actions.Length);
            BitConverter.GetBytes(worldStateAck).CopyTo(result, HEADER_SIZE);
            for (int i = 0; i < actions.Length; i++)
                result[i + HEADER_SIZE + sizeof(int)] = (byte)actions[i];
            return result;
        }
    }
}

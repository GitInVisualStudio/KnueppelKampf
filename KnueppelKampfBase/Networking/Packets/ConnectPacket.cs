using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Networking.Packets
{
    /// <summary>
    /// Packet a client sends to the server to request a connection
    /// </summary>
    public class ConnectPacket : Packet
    {
        /// <summary>
        /// Salt to be XOR-ed with server salt, which is recieved in ChallengePacket
        /// </summary>
        private byte clientSalt;

        public byte ClientSalt { get => clientSalt; set => clientSalt = value; }

        public ConnectPacket()
        {
            ClientSalt = (byte)rnd.Next(byte.MaxValue);
        }

        public ConnectPacket(byte salt)
        {
            clientSalt = salt;
        }

        public ConnectPacket(byte[] bytes) : base(bytes)
        {
            ClientSalt = bytes[HEADER_SIZE];
        }

        public override byte[] ToBytes()
        {
            byte[] result = GetHeader(MAX_SIZE);
            result[HEADER_SIZE] = ClientSalt;
            return result;
        }
    }
}

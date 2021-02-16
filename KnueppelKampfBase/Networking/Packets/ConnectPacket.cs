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
        /// Salt to be XOR-ed with server salt, recieved in ChallengePacket
        /// </summary>
        private byte clientSalt;
        
        public ConnectPacket()
        {
            clientSalt = (byte)rnd.Next(byte.MaxValue);
        }

        public ConnectPacket(byte[] bytes) : base(bytes)
        {
            clientSalt = bytes[HEADER_SIZE];
        }

        public override byte[] ToBytes()
        {
            byte[] result = new byte[MAX_SIZE];
            byte[] header = base.ToBytes();
            header.CopyTo(result, 0);
            result[HEADER_SIZE] = clientSalt;
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Networking.Packets.ClientPackets
{
    /// <summary>
    /// Packet a client sends to the server to request a connection
    /// </summary>
    public class ConnectPacket : Packet
    {
        private byte clientSalt;

        public byte ClientSalt { get => clientSalt; set => clientSalt = value; }

        public ConnectPacket(byte salt) : base()
        {
            clientSalt = salt;
        }

        public ConnectPacket(byte[] bytes) : base(bytes)
        {
            clientSalt = bytes[HEADER_SIZE];
        }

        public override byte[] ToBytes()
        {
            return GetHeader(MAX_SIZE);
        }
    }
}

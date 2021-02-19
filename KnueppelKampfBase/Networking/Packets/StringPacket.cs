using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Networking.Packets
{
    /// <summary>
    /// Simple debug packet used to send string information
    /// </summary>
    public class StringPacket : Packet
    {
        private string content;
        private static readonly Encoding encoding = Encoding.UTF8;

        public string Content { get => content; set => content = value; }

        public StringPacket(string content)
        {
            this.content = content;
        }

        public StringPacket(byte[] bytes) : base(bytes)
        {
            content = encoding.GetString(bytes, HEADER_SIZE, bytes.Length - HEADER_SIZE);
        }

        public override byte[] ToBytes()
        {
            byte[] result = GetHeader(HEADER_SIZE + content.Length);
            encoding.GetBytes(content, 0, content.Length, result, HEADER_SIZE);
            return result;
        }
    }
}

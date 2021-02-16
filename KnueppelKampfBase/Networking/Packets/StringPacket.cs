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
            content = encoding.GetString(bytes, 5, bytes.Length - 5);
        }

        public override byte[] ToBytes()
        {
            byte[] result = base.ToBytes();
            Array.Resize<byte>(ref result, HEADER_SIZE + content.Length);
            Encoding.UTF8.GetBytes(content, 0, content.Length, result, HEADER_SIZE);
            return result;
        }
    }
}

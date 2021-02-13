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
            byte packetType = GetByteFromType(GetType());
            byte[] packageId = BitConverter.GetBytes(Id);
            byte[] value = Encoding.UTF8.GetBytes(content);

            List<byte> result = new List<byte>();
            result.Add(packetType);
            result.AddRange(packageId);
            result.AddRange(value);
            return result.ToArray();
        }
    }
}

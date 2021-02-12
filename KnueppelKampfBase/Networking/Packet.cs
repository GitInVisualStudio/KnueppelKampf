using KnueppelKampfBase.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace KnueppelKampfBase.Networking
{
    public abstract class Packet
    {
        private int id;
        private IPEndPoint sender;
        protected static Type[] packetTypes;
        private static int count;

        public int Id { get => id; set => id = value; }
        public IPEndPoint Sender { get => sender; set => sender = value; }

        public Packet() 
        {
            id = count++;
        }

        public Packet(byte[] array)
        {
            id = BitConverter.ToInt32(array, 1);
        }

        public static void InitPacketTypes()
        {
            packetTypes = new List<Type>(Assembly.GetExecutingAssembly().GetTypes()).FindAll(x => x.IsSubclassOf(typeof(Packet))).ToArray();
        }

        protected byte GetByteFromType(Type t)
        {
            for (int i = 0; i < packetTypes.Length; i++)
                if (packetTypes[i] == t)
                    return (byte)i;
            throw new Exception("Type not found");
        }

        public abstract byte[] ToBytes();

        public static Packet FromBytes(byte[] bytes)
        {
            if (bytes.Length < 5)
                throw new Exception("Invalid Packet header");

            Type packetType = packetTypes[bytes[0]];
            Packet p = (Packet)packetType.GetConstructor(new Type[] { typeof(byte[]) }).Invoke(new object[] { bytes });
            return p;
        }
    }
}

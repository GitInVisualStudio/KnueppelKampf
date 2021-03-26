
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

        private static int count;
        protected static Type[] packetTypes = new List<Type>(Assembly.GetExecutingAssembly().GetTypes()).FindAll(x => x.IsSubclassOf(typeof(Packet))).ToArray();
        protected static Random rnd = new Random(1312);

        protected const int PROTOCOL_ID = 131242069;
        /// <summary>
        /// Max size in bytes a packet may have
        /// </summary>
        protected const int MAX_SIZE = 1024;
        /// <summary>
        /// Size in bytes reserved in every packet for the header
        /// </summary>
        protected const int HEADER_SIZE = 9;

        public int Id { get => id; set => id = value; }
        public IPEndPoint Sender { get => sender; set => sender = value; }

        public Packet() 
        {
            id = count++;
        }

        /// <summary>
        /// Initializes Packet based on recieved bytes. Has to be overriden in every valid packet class! Also resolves packet header
        /// </summary>
        public Packet(byte[] array)
        {
            id = BitConverter.ToInt32(array, 1);
        }

        protected byte GetByteFromType(Type t)
        {
            for (int i = 0; i < packetTypes.Length; i++)
                if (packetTypes[i] == t)
                    return (byte)i;

            throw new Exception("Type not found");
        }

        public abstract byte[] ToBytes();

        protected virtual byte[] GetHeader(int size = HEADER_SIZE)
        {
            if (size < HEADER_SIZE || size > MAX_SIZE)
                throw new Exception("Invalid size");
            byte[] result = new byte[size];
            BitConverter.GetBytes(PROTOCOL_ID).CopyTo(result, 0);
            result[4] = GetByteFromType(GetType());
            BitConverter.GetBytes(id).CopyTo(result, 5);
            return result;
        }

        public static Packet FromBytes(byte[] bytes)
        {
            if (bytes.Length < 9 || BitConverter.ToInt32(bytes, 0) != PROTOCOL_ID)
                throw new Exception("Invalid Packet header");

            Type packetType = packetTypes[bytes[4]];
            ConstructorInfo constructor = packetType.GetConstructor(new Type[] { typeof(byte[]) });
            if (constructor == null)
                throw new Exception("Invalid Packet type");
            Packet p = (Packet)constructor.Invoke(new object[] { bytes });
            return p;
        }
    }
}

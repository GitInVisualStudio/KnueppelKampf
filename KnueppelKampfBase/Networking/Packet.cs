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
        private static Type[] packetTypes;
        private static int count;

        public int Id { get => id; set => id = value; }
        public IPEndPoint Sender { get => sender; set => sender = value; }
        protected static Type[] PacketTypes
        {
            get
            {
                if (packetTypes == null)
                    InitPacketTypes();
                return packetTypes;
            }
        }

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

        /// <summary>
        /// Sets list of all PacketTypes present in assembly, to allow identification based on byte
        /// </summary>
        private static void InitPacketTypes()
        {
            packetTypes = new List<Type>(Assembly.GetExecutingAssembly().GetTypes()).FindAll(x => x.IsSubclassOf(typeof(Packet))).ToArray();
        }

        protected byte GetByteFromType(Type t)
        {
            for (int i = 0; i < PacketTypes.Length; i++)
                if (PacketTypes[i] == t)
                    return (byte)i;
            throw new Exception("Type not found");
        }

        public abstract byte[] ToBytes();

        public static Packet FromBytes(byte[] bytes)
        {
            if (bytes.Length < 5)
                throw new Exception("Invalid Packet header");

            Type packetType = PacketTypes[bytes[0]];
            ConstructorInfo constructor = packetType.GetConstructor(new Type[] { typeof(byte[]) });
            if (constructor == null)
                throw new Exception("Invalid Packet type");
            Packet p = (Packet)constructor.Invoke(new object[] { bytes });
            return p;
        }
    }
}

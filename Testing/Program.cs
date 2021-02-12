using KnueppelKampfBase.Networking;
using KnueppelKampfBase.Networking.Packets;
using System;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            Packet.InitPacketTypes();
            Console.WriteLine("Server or client? [s/c]");
            string choice = Console.ReadLine();
            if (choice.ToLower() == "s")
                Server();
            else if (choice.ToLower() == "c")
                Client();
        }

        private static void Server()
        {
            Console.WriteLine("Started server.");
            Server s = new Server(true);
            s.StartListen();
            Console.ReadLine();
            s.StopListen();
        }

        private static void Client()
        {
            Console.WriteLine("Started client");
            Client c = new Client("localhost");
            while (true)
            {
                string line = Console.ReadLine();
                if (line.ToLower() == "x")
                    break;

                StringPacket p = new StringPacket(line);
                c.SendPacket(p);
            }

            Console.WriteLine("Client over");
            Console.ReadLine();
        }
    }
}

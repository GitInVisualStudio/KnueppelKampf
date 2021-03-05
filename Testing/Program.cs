using KnueppelKampfBase.Networking;
using KnueppelKampfBase.Networking.Packets;
using System;

namespace Testing
{
    /// <summary>
    /// Simple testing program; alter as you wish but please keep most functions!
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server or client? [s/c]");
            string choice = Console.ReadLine();
            if (choice.ToLower() == "s")
                Server();
            else if (choice.ToLower() == "c")
                Client();
        }

        /// <summary>
        /// Runs a server instance
        /// </summary>
        private static void Server()
        {
            Console.WriteLine("Started server.");
            Server s = new Server(true);
            s.StartListen();
            Console.ReadLine();
            s.StopListen();
        }

        /// <summary>
        /// Runs a client instance and sends string packets from user inputs
        /// </summary>
        private static void Client()
        {
            Console.WriteLine("Started client");
            Client c = new Client("localhost");
            c.StartConnecting();

            /*
            while (true)
            {
                string line = Console.ReadLine();
                if (line.ToLower() == "x")
                    break;

                StringPacket p = new StringPacket(line);
                c.SendPacket(p);
            }
            */
            Console.WriteLine("Client over");
            Console.ReadLine();
        }
    }
}

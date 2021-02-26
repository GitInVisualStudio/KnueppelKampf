using KnueppelKampfBase.Networking;
using System;

namespace KnueppelKampfServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Server s = new Server(true))
            {
                s.StartListen();
                Console.WriteLine("Server started.");
                Console.ReadLine();
            }
        }
    }
}

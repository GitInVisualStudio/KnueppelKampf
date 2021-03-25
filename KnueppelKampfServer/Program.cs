using KnueppelKampfBase.Networking;
using System;

namespace KnueppelKampfServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server s = new Server(false);
            s.StartListen();
            s.StartCleanupThread();
            Console.WriteLine("Server started.");
            Console.WriteLine("Listening on " + s.ListeningOn);
            Console.ReadLine();
        }
    }
}

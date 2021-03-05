using KnueppelKampfBase.Networking;
using System;

namespace KnueppelKampfServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server s = Server.Instance;
            s.StartListen();
            s.StartTimeoutThread();
            Console.WriteLine("Server started.");
            Console.ReadLine();
        }
    }
}

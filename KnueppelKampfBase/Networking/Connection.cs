using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace KnueppelKampfBase.Networking
{
    class Connection
    {
        private IPEndPoint client;
        private byte clientSalt;
        private byte serverSalt;

        private static Random rnd = new Random(1312);

        public Connection(IPEndPoint client, byte clientSalt)
        {
            this.client = client;
            this.clientSalt = clientSalt;
            this.serverSalt = (byte)rnd.Next(byte.MaxValue);
        }
    }
}

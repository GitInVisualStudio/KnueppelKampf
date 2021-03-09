using KnueppelKampfBase.Game;
using KnueppelKampfBase.Game.Objects;
using KnueppelKampfBase.Networking.Packets.ServerPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking
{
    public class Game
    {
        private Connection[] connections;
        private Dictionary<Connection, Player> players;
        private WorldManager manager;
        private int id;

        private static int lastId = 1;

        private const int MAX_PLAYERS = 4;

        public int Id { get => id; set => id = value; }
        public Connection[] Connections { get => connections; }

        public Game()
        {
            id = lastId++;
            connections = new Connection[MAX_PLAYERS];
            players = new Dictionary<Connection, Player>();
            manager = new WorldManager();
        }

        private void StartGame()
        {
            players = new Dictionary<Connection, Player>();
            foreach (Connection c in Connections)
            {
                Player p = new Player();
                players[c] = p;
                manager.Entities.Add(p);
            }

            FullWorldPacket fwp = new FullWorldPacket(manager);
            for (int i = 0; i < Connections.Length; i++)
                if (connections[i] != null)
                    Server.Instance.SendPacket(fwp, Connections[i]);
        }

        public bool AddConnection(Connection c)
        {
            int index = GetFirstFreeIndex();
            if (index == -1)
                return false;
            connections[index] = c;
            c.InGame = true;
            return true;
        }

        public void TimeoutConnection(Connection c)
        {
            int index = Array.FindIndex(connections, x => x != null && x == c);
            connections[index] = null;
            manager.Entities.Remove(players[c]);
            players.Remove(c);
        }

        private int GetFirstFreeIndex()
        {
            for (int i = 0; i < connections.Length; i++)
                if (connections[i] == null)
                    return i;
            return -1;
        }
    }
}

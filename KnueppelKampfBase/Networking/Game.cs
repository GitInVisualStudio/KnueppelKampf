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
        private List<Connection> connections;
        private Dictionary<Connection, Player> players;
        private WorldManager manager;
        private int id;

        private static int lastId = 1;

        public int Id { get => id; set => id = value; }

        public Game()
        {
            id = lastId++;
            connections = new List<Connection>();
            players = new Dictionary<Connection, Player>();
            manager = new WorldManager();
        }

        private void StartGame()
        {
            players = new Dictionary<Connection, Player>();
            foreach (Connection c in connections)
            {
                Player p = new Player();
                players[c] = p;
                manager.Entities.Add(p);
            }

            FullWorldPacket fwp = new FullWorldPacket(manager);
            for (int i = 0; i < connections.Count; i++)
                Server.Instance.SendPacket(fwp, connections[i]);
        }

        public void AddConnection(Connection c)
        {
            connections.Add(c);
            c.InGame = true;
        }
    }
}

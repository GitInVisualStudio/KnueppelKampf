using KnueppelKampfBase.Game;
using KnueppelKampfBase.Game.Objects;
using KnueppelKampfBase.Networking.Packets.ServerPackets;
using KnueppelKampfBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Networking
{
    public class Game
    {
        private Connection[] connections;
        private Dictionary<Connection, Player> players;
        private WorldManager manager;
        private int id;
        private long idleSince;
        private bool isHandling;
        private bool inGame;
        private CancellationTokenSource cts;
        private List<WorldState> states;

        private static int lastId = 1;

        private const int UPDATE_SLEEP = 5;

        public int Id { get => id; set => id = value; }
        public Connection[] Connections { get => connections; }
        public long IdleSince { get => idleSince; set => idleSince = value; }
        public WorldManager Manager { get => manager; }

        public const int MAX_IDLE_TIME = 60;
        public const int MAX_PLAYERS = 4;

        public Game()
        {
            id = lastId++;
            connections = new Connection[MAX_PLAYERS];
            players = new Dictionary<Connection, Player>();
            idleSince = TimeUtils.GetTimestamp();
            cts = new CancellationTokenSource();
            isHandling = false;
            inGame = false;
        }

        public void Setup()
        {
            manager = new WorldManager();
            players = new Dictionary<Connection, Player>();
            states = new List<WorldState>();
            inGame = true;
            foreach (Connection c in Connections)
            {
                Player p = new Player();
                players[c] = p;
                Manager.Entities.Add(p);
            }
        }

        public int GetPlayersConnected()
        {
            int count = 0;
            foreach (Connection c in connections)
                if (c != null)
                    count++;
            return count;
        }

        public bool AddConnection(Connection c)
        {
            if (inGame)
                return false;
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
            if (players.ContainsKey(c))
            {
                Manager.Entities.Remove(players[c]);
                players.Remove(c);
            }
        }

        private int GetFirstFreeIndex()
        {
            for (int i = 0; i < connections.Length; i++)
                if (connections[i] == null)
                    return i;
            return -1;
        }

        public void StartHandle(Action<Packet, IPEndPoint> sendPacket)
        {
            if (isHandling)
                return;

            isHandling = true;
            Task.Run(() =>
            {
                idleSince = TimeUtils.GetTimestamp();

                while (true)
                {
                    while (true) // waits until enough players in game, game idle for long enough with 2 players
                    {
                        int playersInGame = GetPlayersConnected();
                        if (playersInGame == 0)
                            return;
                        long timestamp = TimeUtils.GetTimestamp();
                        if (playersInGame == MAX_PLAYERS || (timestamp - IdleSince > MAX_IDLE_TIME && playersInGame > 1))
                            break;
                        Console.WriteLine("Didnt start, there were " + playersInGame + "players connected");
                        Thread.Sleep(100);
                    }

                    Setup();
                    while (true)
                    {
                        if (GetPlayersConnected() == 0)
                            break;

                        WorldState ws = manager.GetState();
                        Dictionary<WorldState, WorldDelta> updates = new Dictionary<WorldState, WorldDelta>();
                        lock (connections)
                        {
                            foreach (Connection c in connections)
                            {
                                if (c == null)
                                    continue;

                                WorldDelta wd;
                                if (c.LastAck != null)
                                {
                                    if (!updates.ContainsKey(c.LastAck)) // gotta make sure we havent calculated this update before 
                                    {
                                        updates[c.LastAck] = new WorldDelta(c.LastAck, ws);
                                    }
                                    wd = updates[c.LastAck];
                                }
                                else
                                    wd = new WorldDelta(null, ws);

                                UpdatePacket up = new UpdatePacket(wd);
                                sendPacket(up, c.Client);
                                c.RefreshSentPacketTimestamp();
                            }
                        }
                        Thread.Sleep(UPDATE_SLEEP);
                    }
                }
            }, cts.Token).ContinueWith((Task t) =>
            {
                isHandling = false;
                inGame = false;
            });
        }

        public void ClientAcknowledgesWorldState(Connection c, int stateId)
        {
            if (!inGame)
                return;
            WorldState ws = states.Find(x => x.Id == stateId);
            if (ws == null)
                return;

            c.LastAck = ws;
            ws.AcknowledgedBy.Add(c);
        }
    }
}

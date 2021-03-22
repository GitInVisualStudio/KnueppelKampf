using KnueppelKampfBase.Game;
using KnueppelKampfBase.Game.Components;
using KnueppelKampfBase.Game.Objects;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Networking.Packets.ClientPackets;
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

        private const int UPDATE_SLEEP = 1000;

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

            int width = 1920;
            //unten
            manager.Entities.Add(new Floor(new Vector(500, 850), new Vector(width * 0.5f, 500)));
            manager.Entities.Add(new Floor(new Vector(500 - 75, 850 - 100), new Vector(76, 600)));
            manager.Entities.Add(new Floor(new Vector(500 + width * 0.5f - 1, 850 - 100), new Vector(76, 600)));

            //pillar
            manager.Entities.Add(new Floor(new Vector(500 - 75 - 50, 850 - 100 - 300), new Vector(75 + 100, 75)));
            manager.Entities.Add(new Floor(new Vector(500 + width * 0.5f - 50, 850 - 100 - 300), new Vector(75 + 100, 75)));


            //seiten
            manager.Entities.Add(new Floor(new Vector(50, 650), new Vector(150, 600)));
            manager.Entities.Add(new Floor(new Vector(width * 0.5f + 800, 650), new Vector(150, 600)));

            //augen
            manager.Entities.Add(new Floor(new Vector(500 + width * 0.25f - 300, 250), new Vector(150, 450)));
            manager.Entities.Add(new Floor(new Vector(500 + width * 0.25f + 150, 250), new Vector(150, 450)));

            foreach (Connection c in Connections)
            {
                Player p = new Player();
                players[c] = p;
                manager.Entities.Add(p);
            }
        }

        public int GetPlayersConnected()
        {
            int count = 0;
            lock (connections)
            {
                foreach (Connection c in connections)
                    if (c != null)
                        count++;
                return count;
            }
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
                lock (manager)
                {
                    manager.Entities.Remove(players[c]);
                }

                lock (players)
                {
                    players.Remove(c);
                }
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
                while (true)
                {
                    idleSince = TimeUtils.GetTimestamp();
                    while (true) // waits until enough players in game, game idle for long enough with 2 players
                    {
                        int playersInGame = GetPlayersConnected();
                        if (playersInGame == 0)
                            return;
                        long timestamp = TimeUtils.GetTimestamp();
                        if (playersInGame == MAX_PLAYERS || (timestamp - IdleSince > MAX_IDLE_TIME && playersInGame > 1))
                            break;
                        Thread.Sleep(100);
                    }

                    Setup();
                    while (true)
                    {
                        if (GetPlayersConnected() == 0)
                            break;

                        WorldState ws = manager.GetState();
                        states.Add(ws);
                        Dictionary<WorldState, WorldDelta> updates = new Dictionary<WorldState, WorldDelta>();
                        lock (connections)
                        {
                            foreach (Connection c in connections)
                            {
                                lock (c)
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
                                    lock (players)
                                        up.YourEntityId = players[c].Id;
                                    //byte[] debugBytes = up.ToBytes();
                                    //UpdatePacket debugPacket = new UpdatePacket(debugBytes);
                                    sendPacket(up, c.Client);
                                    c.RefreshSentPacketTimestamp();
                                }
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

        public void HandleInputPacket(InputPacket input, Connection c)
        {
            ClientAcknowledgesWorldState(c, input.WorldStateAck);
            if (players != null && players.ContainsKey(c))
                players[c].GetComponent<ControlComponent>().HandleInputs(input.Actions);
        }

        public void ClientAcknowledgesWorldState(Connection c, int stateId)
        {
            if (!inGame)
                return;
            WorldState ws;
            lock (states)
                ws = states.Find(x => x.Id == stateId);
            if (ws == null)
                return;

            lock (c)
                c.LastAck = ws;
            ws.AcknowledgedBy.Add(c);
        }
    }
}

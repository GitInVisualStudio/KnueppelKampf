﻿using KnueppelKampfBase.Game;
using KnueppelKampfBase.Game.Components;
using KnueppelKampfBase.Game.Objects;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Networking.Packets.ClientPackets;
using KnueppelKampfBase.Networking.Packets.ServerPackets;
using KnueppelKampfBase.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        /// <summary>
        /// Time in ms that passes between sent update packets
        /// </summary>
        private const int UPDATE_SLEEP = 50;
        /// <summary>
        /// This game's ID, not to confuse with its index in the server's game array
        /// </summary>
        public int Id { get => id; set => id = value; }
        public Connection[] Connections { get => connections; }
        /// <summary>
        /// Time since this game has been waiting for enough players
        /// </summary>
        public long IdleSince { get => idleSince; set => idleSince = value; }
        public WorldManager Manager { get => manager; }

        /// <summary>
        /// Maximum time in seconds this game should be idle if there are more than 1 connected players
        /// </summary>
        public const int MAX_IDLE_TIME = 60;
        /// <summary>
        /// Maximum amount of Players allowed per game
        /// </summary>
        public const int MAX_PLAYERS = 4;

        /// <summary>
        /// Called when there is only one player left alive and this game ends
        /// </summary>
        public event EventHandler GameEnded;

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
            manager.AddObject(new Floor(new Vector(500, 850), new Vector(width * 0.5f, 500)));
            manager.AddObject(new Floor(new Vector(500 - 75, 850 - 100), new Vector(76, 600)));
            manager.AddObject(new Floor(new Vector(500 + width * 0.5f - 1, 850 - 100), new Vector(76, 600)));

            //pillar
            manager.AddObject(new Floor(new Vector(500 - 75 - 50, 850 - 100 - 300), new Vector(75 + 100, 75)));
            manager.AddObject(new Floor(new Vector(500 + width * 0.5f - 50, 850 - 100 - 300), new Vector(75 + 100, 75)));

            //seiten
            manager.AddObject(new Floor(new Vector(50, 650), new Vector(150, 600)));
            manager.AddObject(new Floor(new Vector(width * 0.5f + 800, 650), new Vector(150, 600)));

            //augen
            manager.AddObject(new Floor(new Vector(500 + width * 0.25f - 300, 250), new Vector(150, 450)));
            manager.AddObject(new Floor(new Vector(500 + width * 0.25f + 150, 250), new Vector(150, 450)));

            //items
            manager.AddObject(new Item() { Position = new Vector(500 + width * 0.25f - 25, 350) });
            manager.AddObject(new Item() { Position = new Vector(500 + width * 0.25f - 25, 700) });

            Vector[] playerPositions = new Vector[] { new Vector(50 + 150 / 2f, 300), new Vector(width * 0.5f + 800 + 150 / 2f, 300), new Vector(500 - 75 - 50 + 175 / 2f, 100), new Vector(500 + width * 0.5f - 50 + 175 / 2f, 100) };
            lock (connections)
                for (int i = 0; i < Connections.Length; i++)
                {
                    Connection c = Connections[i];
                    c.LastAck = null;
                    Player p = new Player(playerPositions[i % playerPositions.Length]);
                    players[c] = p;
                    manager.AddObject(p);
                }
        }

        /// <summary>
        /// Returns count of connected players
        /// </summary>
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

        /// <summary>
        /// Tries to add connection to this game
        /// </summary>
        /// <returns>True if succeeded, false if failed</returns>
        public bool AddConnection(Connection c)
        {
            if (inGame)
                return false;
            int index = GetFirstFreeIndex();
            if (index == -1)
                return false;
            connections[index] = c;
            return true;
        }

        /// <summary>
        /// Unsets a connection and removes its entity
        /// </summary>
        public void TimeoutConnection(Connection c)
        {
            int index = Array.FindIndex(connections, x => x != null && x == c);
            connections[index] = null;
            if (players.ContainsKey(c))
            {
                lock (manager.Entities)
                {
                    manager.Entities.Remove(players[c]);
                }

                lock (players)
                {
                    players.Remove(c);
                }
            }
            if (GetPlayersConnected() == 0)
                cts.Cancel();
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
                CancellationTokenSource updateCanceller = new CancellationTokenSource(); // used to exit the manager update thread
                states = new List<WorldState>();
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
                Task.Run(() =>
                {
                    Stopwatch watch = new Stopwatch(), w = new Stopwatch();
                    w.Start();
                    watch.Start();
                    int count = 0;
                    int tpt = (int)(1000f / WorldManager.TPS);
                    while (true)
                    {
                        try
                        {
                            if (w.Elapsed.TotalSeconds >= 1)
                            {
                                Console.WriteLine("Current ticks: " + count);
                                count = 0;
                                w.Restart();
                            }
                            manager.OnUpdate();
                            while (watch.Elapsed.TotalMilliseconds < tpt)
                                Thread.Sleep(0);
                            count++;
                            watch.Restart();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }, updateCanceller.Token);

                while (true)
                {
                    lock (manager.Entities)
                    {
                        if (manager.Entities.Where(x => x.GetType() == typeof(Player)).Count() == 1) // win condition
                        {
                            GameEnded?.Invoke(this, new EventArgs());
                            break;
                        }
                    }

                    WorldState ws = manager.GetState();
                    states.Add(ws);
                    Dictionary<WorldState, WorldDelta> updates = new Dictionary<WorldState, WorldDelta>();
                    lock (connections)
                    {
                        foreach (Connection c in connections)
                        {
                            if (c == null)
                                continue;
                            lock (c)
                            {
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
                updateCanceller.Cancel();
            }, cts.Token).ContinueWith((Task t) =>
            {
                isHandling = false;
                inGame = false;
            });
        }

        /// <summary>
        /// Applies given input actions and sets acknowledge
        /// </summary>
        public void HandleInputPacket(InputPacket input, Connection c)
        {
            if (!isHandling || !inGame)
                return;
            ClientAcknowledgesWorldState(c, input.WorldStateAck);
            lock (players)
            {
                if (players != null && players.ContainsKey(c))
                {
                    Player p = players[c];
                    p.Rotation = input.Rotation;
                    p.GetComponent<ControlComponent>().HandleInputs(input.Actions);
                }
            }
        }

        /// <summary>
        /// Sets a client's last acknowleded state
        /// </summary>
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

using KnueppelKampfBase.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game
{
    /// <summary>
    /// Object representing the entire game world's state at construction time
    /// </summary>
    public class WorldState
    {
        private List<ObjectState> states;
        private List<Connection> acknowledgedBy;
        private int id;

        private static int lastId;

        public WorldState(WorldManager manager)
        {
            this.states = new List<ObjectState>();
            lock (manager.Entities)
            {
                foreach (GameObject o in manager.Entities)
                    states.Add(new ObjectState(o));
            }
            //this.states = manager.Entities.Select(o => new ObjectState(o)).ToList();
            acknowledgedBy = new List<Connection>();
            id = lastId++;
        }

        public List<ObjectState> States { get => states; set => states = value; }
        public List<Connection> AcknowledgedBy { get => acknowledgedBy; set => acknowledgedBy = value; }
        public int Id { get => id; set => id = value; }
    }
}

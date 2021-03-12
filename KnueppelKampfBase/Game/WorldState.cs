using KnueppelKampfBase.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game
{
    public class WorldState
    {
        private List<ObjectState> states;
        private WorldManager manager;
        private List<Connection> acknowledgedBy;
        private int id;

        private static int lastId;

        public WorldState(WorldManager manager)
        {
            this.manager = manager;
            this.states = manager.Entities.Select(o => new ObjectState(o)).ToList();
            acknowledgedBy = new List<Connection>();
            id = lastId++;
        }

        public List<ObjectState> States { get => states; set => states = value; }
        public List<Connection> AcknowledgedBy { get => acknowledgedBy; set => acknowledgedBy = value; }
        public int Id { get => id; set => id = value; }

        public void Apply()
        {
            manager.Entities.Clear();
            manager.Entities.AddRange(states.Select(x => x.Obj));
            foreach (ObjectState s in states)
                s.Apply();
        }
    }
}

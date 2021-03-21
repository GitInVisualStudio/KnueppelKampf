using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game
{
    public class WorldDelta
    {
        private int oldId;
        private int newId;
        private List<int> deleted;
        private List<ObjectState> spawned;
        public int EarlierId { get => oldId; set => oldId = value; }
        public int NewerId { get => newId; set => newId = value; }
        public List<int> Deleted { get => deleted; set => deleted = value; }
        public List<ObjectState> Spawned { get => spawned; set => spawned = value; }

        public WorldDelta(WorldState oldState, WorldState newState)
        {
            if (oldState == null)
                oldId = -1;
            else
                oldId = oldState.Id;
            newId = newState.Id;

            Deleted = new List<int>();
            if (oldState == null)
            {
                spawned = newState.States;
                return;
            }
            foreach(ObjectState objs in oldState.States)
                if(newState.States.Find(x => objs.Id == x.Id) == null)
                    Deleted.Add(objs.Id);

            foreach (ObjectState objs in newState.States)
                if (oldState.States.Find(x => objs.Id == x.Id) == null)
                    Spawned.Add(objs);
        }
    }
}

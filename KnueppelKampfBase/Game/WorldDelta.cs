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

        public int EarlierId { get => oldId; set => oldId = value; }
        public int NewerId { get => newId; set => newId = value; }

        public WorldDelta(WorldState oldState, WorldState newState)
        {
            if (oldState == null)
                oldId = -1;
            else
                oldId = oldState.Id;
            newId = newState.Id;
            
            // TODO
        }
    }
}

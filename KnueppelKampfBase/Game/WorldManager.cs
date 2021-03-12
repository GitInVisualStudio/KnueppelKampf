using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game
{
    public class WorldManager
    {
        private List<GameObject> entities;

        public static WorldManager Instance = new WorldManager();

        public List<GameObject> Entities { get => entities; set => entities = value; }

        public WorldManager()
        { 
            this.Entities = new List<GameObject>();
        }

        public void OnRender()
        {
            for (int i = Entities.Count - 1; i >= 0; i--)
                Entities[i].OnRender();
        }
        public void OnUpdate()
        {
            for (int i = Entities.Count - 1; i >= 0; i--)
                Entities[i].OnUpdate();
        }

        public WorldState GetState() => new WorldState(this);

        public void Apply(WorldDelta delta)
        {

        }
    }
}

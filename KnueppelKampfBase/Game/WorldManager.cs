using KnueppelKampfBase.Game.Components;
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

        public IEnumerable<T> SelectComponents<T>() where T : GameComponent
        {
            foreach(GameObject obj in entities)
            {
                T t = obj.GetComponent<T>();
                if(t != null)
                    yield return t;
            }
        }

        public void OnUpdate()
        {
            for (int i = Entities.Count - 1; i >= 0; i--)
                Entities[i].OnUpdate();

            IEnumerable<BoxComponent> boxes = SelectComponents<BoxComponent>();
            foreach (BoxComponent box in boxes)
                box.CheckCollision(boxes);
        }

        public WorldState GetState() => new WorldState(this);
    }
}

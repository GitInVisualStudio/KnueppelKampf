using KnueppelKampfBase.Game.Components;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game
{
    public class WorldManager
    {
        private List<GameObject> entities;
        private GameObject camera;
        private Vector current;
        public static WorldManager Instance = new WorldManager();
        private Vector offset;
        public List<GameObject> Entities { get => entities; set => entities = value; }
        public GameObject Camera { get => camera; set => camera = value; }
        public Vector Offset { get => offset; set => offset = value; }

        public const int TPS = 40;

        public WorldManager()
        { 
            this.Entities = new List<GameObject>();
        }

        public void OnRender()
        {
            StateManager.SetColor(37, 57, 68);
            StateManager.FillRect(new Vector(-5, -5), offset * 2.1f);

            StateManager.Push();
            if(camera != null)
                current += (-camera.Position - camera.Size / 2 + offset - current) * StateManager.delta * 3;
            StateManager.Translate(current);
            lock (entities)
                for (int i = Entities.Count - 1; i >= 0; i--)
                    Entities[i].OnRender();
            StateManager.Pop();
        }

        public IEnumerable<T> SelectComponents<T>() where T : GameComponent
        {
            lock (entities) 
                foreach(GameObject obj in entities)
                {
                    T t = obj.GetComponent<T>();
                    if(t != null)
                        yield return t;
                }
        }

        public void OnUpdate()
        {
            lock (entities)
                for (int i = Entities.Count - 1; i >= 0; i--)
                    Entities[i].OnUpdate();

            IEnumerable<BoxComponent> boxes = SelectComponents<BoxComponent>();
            foreach (BoxComponent box in boxes)
                lock (box)
                    box.CheckCollision(boxes);
        }

        public WorldState GetState() => new WorldState(this);

        public void Apply(WorldDelta delta)
        {
            lock (entities)
            {
                entities.AddRange(delta.Spawned);
                foreach (ObjectDelta od in delta.Changed)
                {
                    GameObject go = entities.Find(x => x.Id == od.EntityId);
                    if (go != null)
                        go.Apply(od);
                }
            }
        }

        public GameObject GetObject(int id)
        {
            lock (entities)
                return entities.Find(x => x.Id == id);
        }
    }
}

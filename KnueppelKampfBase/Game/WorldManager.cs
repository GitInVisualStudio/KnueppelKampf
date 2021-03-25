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
        private Vector offset;

        private static bool onServer = false;

        public List<GameObject> Entities { get => entities; set => entities = value; }
        public GameObject Camera { get => camera; set => camera = value; }
        public Vector Offset { get => offset; set => offset = value; }

        /// <summary>
        /// Whether this manager is running on a server or a client
        /// </summary>
        public static bool OnServer { get => onServer; set => onServer = value; }

        /// <summary>
        /// Number of ticks that should be simulated per second
        /// </summary>
        public const int TPS = 20;

        public WorldManager()
        { 
            this.Entities = new List<GameObject>();
        }

        public void AddObjects(IEnumerable<GameObject> objs)
        {
            foreach (GameObject obj in objs)
                AddObject(obj);
        }

        /// <summary>
        /// Adds an object to this manager's entity list and sets its manager property
        /// </summary>
        public void AddObject(GameObject obj)
        {
            lock (entities)
                entities.Add(obj);
            obj.Manager = this;
        }

        public void OnRender()
        {
            StateManager.SetColor(37, 57, 68);
            StateManager.FillRect(new Vector(-5, -5), offset * 2.1f);

            StateManager.Push();
            if (camera != null)
            {
                current += (-camera.Position - camera.Size / 2 + offset - current) * StateManager.delta * 3;
            }
                
            StateManager.Translate(current);
            lock (entities)
                for (int i = Entities.Count - 1; i >= 0; i--)
                    Entities[i].OnRender();
            StateManager.Pop();
        }

        public IEnumerable<T> SelectComponents<T>() where T : GameComponent
        {
            List<T> list = new List<T>();
            foreach(GameObject obj in entities)
            {
                T t = obj.GetComponent<T>();
                if (t != null)
                    list.Add(t);
            }
            return list;
        }

        public void OnUpdate()
        {
            lock (entities)
            {
                for (int i = Entities.Count - 1; i >= 0; i--)
                {
                    Entities[i].OnUpdate();
                    if (Entities[i].Despawn)
                        this.entities.RemoveAt(i);
                }

                IEnumerable<BoxComponent> boxes = SelectComponents<BoxComponent>();
                foreach (BoxComponent box in boxes)
                    box.CheckCollision(boxes);
            }
        }

        public WorldState GetState() => new WorldState(this);

        public void Apply(WorldDelta delta)
        {
            lock (entities)
            {
                AddObjects(delta.Spawned);
                foreach (ObjectDelta od in delta.Changed)
                {
                    GameObject go = entities.Find(x => x.Id == od.EntityId);
                    if (go != null)
                        go.Apply(od);
                }

                foreach (int deletedId in delta.Deleted)
                {
                    GameObject go = entities.Find(x => x.Id == deletedId);
                    if (go != null)
                        go.Despawn = true;
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

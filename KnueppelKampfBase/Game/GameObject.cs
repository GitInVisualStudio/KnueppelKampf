using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;
using KnueppelKampfBase.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace KnueppelKampfBase.Game
{
    public class GameObject
    {
        private static int lastId;
        private int id;
        protected Vector prevPosition;
        protected Vector position;
        protected Vector size;
        protected float rotation;
        private List<GameComponent> components;
        private WorldManager manager;
        private bool despawn = false;

        public Vector Position { get => position; set => position = value; }
        public Vector Size { get => size; set => size = value; }
        public float Rotation { get => rotation; set => rotation = value; }

        [DontSerialize]
        public float Width
        {
            get
            {
                return size.X;
            }
            set
            {
                size.X = value;
            }
        }

        [DontSerialize]
        public float Height
        {
            get
            {
                return size.Y;
            }
            set
            {
                size.Y = value;
            }
        }

        [DontSerialize]
        public float X
        {
            get
            {
                return position.X;
            }
            set
            {
                position.X = value;
            }
        }

        [DontSerialize]
        public float Y
        {
            get
            {
                return position.Y;
            }
            set
            {
                position.Y = value;
            }
        }

        public List<GameComponent> Components { get => components; set => components = value; }
        
        [DontSerialize]
        public Vector PrevPosition { get => prevPosition; set => prevPosition = value; }
        public int Id { get => id; set => id = value; }
        
        private static Type[] objectTypes = new List<Type>(Assembly.GetExecutingAssembly().GetTypes()).FindAll(x => x.IsSubclassOf(typeof(GameObject))).ToArray();
        public static Type[] ObjectTypes { get => objectTypes; }
        [DontSerialize]
        public WorldManager Manager { get => manager; set => manager = value; }

        public bool Despawn { get => despawn; set => despawn = value; }

        public GameObject()
        {
            this.Components = new List<GameComponent>();
            this.id = lastId++;
        }

        public static int GetTypeIndex(Type t)
        {
            return Array.FindIndex(objectTypes, x => t.Equals(x));
        }

        public T GetComponent<T>() where T : GameComponent
        {
            return (T)this.Components.Find(x => x is T);
        }

        public bool AddComponent<T>(T c, bool initComponent = true) where T : GameComponent
        {
            if (this.Components.Find(x => x is T) != null)
                return false;
            c.GameObject = this;
            if (initComponent)
                c.Init();
            this.Components.Add(c);
            return true;
        }

        public virtual void OnRender()
        {
            StateManager.Push();
            //von der mitte des objektes wird rotiert
            //interpolieren
            StateManager.Translate(Position + (prevPosition - position) * StateManager.partialTicks + Size / 2);
            StateManager.Rotate(Rotation);
            //NOTE: in umgekehrte richtung, damit es keine probleme gibt, falls während des durchgangs ein element entfernt wird
            for (int i = Components.Count - 1; i >= 0; i--)
                Components[i].OnRender();
            StateManager.Pop();
        }

        public virtual void OnUpdate()
        {
            this.prevPosition = position;
            //NOTE: in umgekehrte richtung, damit es keine probleme gibt, falls während des durchgangs ein element entfernt wird
            for (int i = Components.Count - 1; i >= 0; i--)
                Components[i].OnUpdate();
        }

        public void Apply(ObjectDelta od)
        {
            Type t = GetType();
            PropertyInfo[] properties = t.GetProperties();
            lock (this)
                foreach (byte key in od.ChangedProperties.Keys)
                {
                    object value = od.ChangedProperties[key];
                    properties[key].SetValue(this, value);
                }

            lock (components)
                foreach (ComponentDelta cd in od.ChangedComponents)
                {
                    Type componentStateType = cd.ComponentStateType;
                    PropertyInfo componentStateInfo = componentStateType.GetProperty("ComponentType");
                    if (componentStateInfo == null)
                        continue;
                    Type componentType = (Type)componentStateInfo.GetValue(cd);
                    GameComponent gc = Components.Find(x => x.GetType() == componentType);
                    if (gc == null)
                        continue;

                    ComponentState state = gc.GetState();
                    properties = componentStateType.GetProperties();
                    foreach (byte key in cd.ChangedProperties.Keys)
                    {
                        object value = cd.ChangedProperties[key];
                        properties[key].SetValue(state, value);
                    }
                    gc.ApplyState(state);
                }
        }
    }
}

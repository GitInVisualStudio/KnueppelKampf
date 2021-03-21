using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;
using System;
using System.Collections.Generic;
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

        public Vector Position { get => position; set => position = value; }
        public Vector Size { get => size; set => size = value; }
        public float Rotation { get => rotation; set => rotation = value; }

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
        public Vector PrevPosition { get => prevPosition; set => prevPosition = value; }
        public int Id { get => id; set => id = value; }

        public GameObject()
        {
            this.Components = new List<GameComponent>();
            this.id = lastId++;
        }

        public T GetComponent<T>() where T : GameComponent
        {
            return (T)this.Components.Find(x => x is T);
        }

        public bool AddComponent<T>(T c) where T : GameComponent
        {
            if (this.Components.Find(x => x is T) != null)
                return false;
            c.GameObject = this;
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
    }
}

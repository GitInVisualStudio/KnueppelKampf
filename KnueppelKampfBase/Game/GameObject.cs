using KnueppelKampfBase.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Game
{
    public class GameObject
    {
        private Vector position;
        private Vector size;
        private List<GameComponent> components;

        public Vector Position { get => position; set => position = value; }
        public Vector Size { get => size; set => size = value; }

        public GameObject()
        {
            this.components = new List<GameComponent>();
        }

        public T GetComponent<T>() where T : GameComponent
        {
            return (T)this.components.Find(x => x is T);
        }

        public bool AddComponent<T>(T c) where T : GameComponent
        {
            if (this.components.Find(x => x is T) != null)
                return false;
            c.GameObject = this;
            this.components.Add(c);
            return true;
        }

        public void Render()
        {
            //NOTE: in umgekehrte richtung, damit es keine probleme gibt, falls während des durchgangs ein element entfernt wird
            for (int i = components.Count - 1; i >= 0; i--)
                components[i].Render();
        }

        public void Update()
        {
            //NOTE: in umgekehrte richtung, damit es keine probleme gibt, falls während des durchgangs ein element entfernt wird
            for (int i = components.Count - 1; i >= 0; i--)
                components[i].Update();
        }
    }
}

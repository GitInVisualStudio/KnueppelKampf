using KnueppelKampfBase.Game.Components;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace KnueppelKampfBase.Game.Objects
{
    public class Player : GameObject
    {
        private Color color;
        private bool sneaking, jumping;

        public bool Sneaking { get => sneaking; set => sneaking = value; }
        public bool Jumping { get => jumping; set => jumping = value; }
        public Color Color { get => color; set => color = value; }

        public Player(Vector position = default)
        {
            this.position = position;
            this.size = new Vector(50, 100);
            color = Color.Black;
            this.AddComponent(new MoveComponent(5, 0.1f));
            this.AddComponent(new BoxComponent());
            this.AddComponent(new PlayerAnimationComponent(this));
        }

        public void Jump()
        {
            //TODO: jump
        }

        public void Sneak()
        {
            //TODO: sneak
        }

        public override void OnRender()
        {
            StateManager.SetColor(Color.Red);
            StateManager.DrawRect(this.position, this.size);
            StateManager.Push();
            //von der mitte des objektes wird rotiert
            StateManager.Translate(Position + Size / 2);
            //NOTE: in umgekehrte richtung, damit es keine probleme gibt, falls während des durchgangs ein element entfernt wird
            for (int i = Components.Count - 1; i >= 0; i--)
                Components[i].OnRender();

            StateManager.Pop();

        }
    }
}

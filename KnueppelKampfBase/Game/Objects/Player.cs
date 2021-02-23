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

        public Player()
        {
            this.AddComponent(new MoveComponent(5, 0.1f));
            this.AddComponent(new BoxComponent());
            this.AddComponent(new ControlComponent(keyDown: KeyDown));
        }

        private void KeyDown(int key)
        {
            switch (key)
            {
                case 'A':
                    break;
                case 'D':
                    break;
                case 'W':
                    break;
                case 'S':
                    break;
                case ' ':
                    //TODO: jump
                    break;
                case 15:
                    //TODO: shift
                    break;
            }
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
            StateManager.Push();
            //von der mitte des objektes wird rotiert
            StateManager.Translate(Position + Size / 2);
            StateManager.Rotate(Rotation);
            //NOTE: in umgekehrte richtung, damit es keine probleme gibt, falls während des durchgangs ein element entfernt wird
            for (int i = components.Count - 1; i >= 0; i--)
                components[i].OnRender();

            StateManager.Pop();

        }
    }
}

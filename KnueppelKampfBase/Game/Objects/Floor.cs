using KnueppelKampfBase.Game.Components;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;
using System;
using static System.Math;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game.Objects
{
    public class Floor : GameObject
    {
        public Floor()
        {
            position = default(Vector);
            size = default(Vector);
            AddComponent(new BoxComponent(HandleCollision));
        }

        public Floor(Vector position, Vector size)
        {
            this.position = position;
            this.size = size;
            AddComponent(new BoxComponent(HandleCollision));
        }

        private void HandleCollision(BoxComponent b)
        {
            if (b.GameObject is Floor)
                return;
            GameObject obj = b.GameObject;
            MoveComponent move = obj.GetComponent<MoveComponent>();

            if (move == null)
                return;

            b.OnCollision?.Invoke(this.GetComponent<BoxComponent>());

            if (move.Y > 0)
                move.OnGround = true;

            Vector delta = default;

            if (obj.Y + obj.Height > Y && obj.Y + obj.Height < Y + Height)
            {
                delta.Y = Y - obj.Height - 1;
            }

            if (obj.Y <= Y + Height && obj.Y + obj.Height > Y + Height)
            {
                delta.Y = Y + Height + 1;
            }

            if (obj.X + obj.Width > X && obj.X + obj.Width < X + Width)
            {
                delta.X = X - obj.Width - 1;
            }

            if (obj.X < X + Width && obj.X + obj.Width > X + Width)
            {
                delta.X = X + Width + 1;
            }


            if (Abs(obj.X - delta.X) > Abs(obj.Y - delta.Y))
            {
                delta.X = obj.X;
                move.Y = 0;
            }
            else if(Abs(obj.X - delta.X) < Abs(obj.Y - delta.Y))
            {
                delta.Y = obj.Y;
            }

            obj.Position = delta;
        }

        public override void OnRender()
        {
            StateManager.SetColor(Color.FromArgb(0, 20, 40));
            StateManager.FillRect(Position, Size);
            StateManager.SetColor(Color.FromArgb(189, 165, 87));
            StateManager.FillRect(Position.X - 2.5f, Position.Y - 1, Size.X + 5, 15);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}

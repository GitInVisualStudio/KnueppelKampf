using KnueppelKampfBase.Game.Components;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;
using System;
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


            if (move.Y > 0)
                move.OnGround = true;

            if (obj.Y < Y + Height && obj.Y + obj.Height > Y + Height)
            {
                obj.Y = Y + Height;
                move.Y = 0;
                return;
            }

            if (obj.Y + obj.Height > Y && obj.Y + obj.Height < Y + Height && obj.Y <= Y)
            {
                obj.Y = Y - obj.Height;
                move.Y = 0;
                return;
            }

            if (obj.X + obj.Width > X && obj.X + obj.Width < X + Width)
            {
                obj.X = X - obj.Width;
                move.Velocity *= 0.6f;
            }
                

            if (obj.X < X + Width && obj.X + obj.Width > X + Width)
            {
                obj.X = X + Width;
                move.Velocity *= 0.6f;
            }


            //obj.Position -= move.Velocity * 10f;
            //move.Velocity = default;
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

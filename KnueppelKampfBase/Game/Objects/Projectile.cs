using KnueppelKampfBase.Game.Components;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;
using KnueppelKampfBase.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace KnueppelKampfBase.Game.Objects
{
    /// <summary>
    /// ist ein projektiel der waffe
    /// </summary>
    public class Projectile : GameObject
    {
        private MoveComponent move;
        private float damage;
        private GameObject owner;

        public Projectile()
        {

        }

        public Projectile(GameObject owner, float damage)
        {
            this.owner = owner;
            this.damage = damage;
            this.position = owner.Position;
            this.size = new Vector(10, 5);
            AddComponent(move = new MoveComponent());
            AddComponent(new BoxComponent((BoxComponent b) =>
            {
                //wenn das projektil einen spieler trifft, soll dieser schaden erhalten
                if (b.GameObject == owner)
                    return;
                this.Despawn = true;
                //TODO: update health and velocity of enemy
                HealthComponent health = b.GameObject.GetComponent<HealthComponent>();
                if (health == null)
                    return;
                if (WorldManager.OnServer)
                    health.Health -= damage;
                MoveComponent move = b.GameObject.GetComponent<MoveComponent>();
                if (move == null)
                    return;
                move.Velocity += this.move.Velocity;
            }));

            //zur blickrichtung bewegen
            float x = (float)Cos(owner.Rotation * PI / 180.0f + PI/2);
            float y = (float)Sin(owner.Rotation * PI / 180.0f + PI/2);
            this.move.Velocity = new Vector(x, y) * 10f;
        }

        public override void OnRender()
        {
            base.OnRender();
            StateManager.Push();
            StateManager.Translate(Position + (PrevPosition - position) * StateManager.partialTicks + Size / 2);
            StateManager.Rotate(GetComponent<MoveComponent>().Velocity.Angle + 90.0f);

            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(-size.X / 2 * 2, -size.Y / 2 * 2, size.X * 2, size.Y * 2);

            PathGradientBrush pthGrBrush = new PathGradientBrush(path);

            pthGrBrush.CenterColor = Color.FromArgb(150, Color.Red);

            Color[] colors = { Color.FromArgb(0, Color.Red) };
            pthGrBrush.SurroundColors = colors;

            StateManager.Graphics.FillEllipse(pthGrBrush, -size.X / 2 * 2, -size.Y / 2 * 2, size.X * 2, size.Y * 2);

            StateManager.SetColor(Color.Red);
            StateManager.FillRoundRect(size / -2, size, 5, 10);
            StateManager.Pop();
        }
    }
}

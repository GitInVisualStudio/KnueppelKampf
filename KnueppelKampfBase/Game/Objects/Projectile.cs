using KnueppelKampfBase.Game.Components;
using KnueppelKampfBase.Render;
using KnueppelKampfBase.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game.Objects
{
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
            AddComponent(move = new MoveComponent());
            AddComponent(new BoxComponent((BoxComponent b) =>
            {
                if (b.GameObject != owner)
                    return;
                //TODO: update health and velocity of enemy
                HealthComponent health = b.GameObject.GetComponent<HealthComponent>();
                if (health == null)
                    return;
                health.Health -= damage;
                MoveComponent move = b.GameObject.GetComponent<MoveComponent>();
                if (move == null)
                    return;
                move.Velocity += this.move.Velocity / 2;
            }));
        }

        public override void OnRender()
        {
            base.OnRender();
            StateManager.Push();
            StateManager.Translate(position + size / 2);
            StateManager.SetColor(Color.Black);
            StateManager.Rotate(move.Velocity.Angle);
            StateManager.FillRect(size / -2, size);
            StateManager.Pop();
        }
    }
}

using KnueppelKampfBase.Game.Components;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game.Objects
{
    public class Particle : GameObject
    {
        private static Random random = new Random();
        private MoveComponent move;
        private int time;
        public Particle(GameObject owner)
        {
            this.position = owner.Position + owner.Size * (float)random.NextDouble();
            this.size = new Vector(10, 5);
            AddComponent(move = new MoveComponent());
            Vector vel = owner.GetComponent<MoveComponent>().Velocity;
            move.Velocity = vel; 
            this.rotation = vel.Angle;
            time = 20;
        }

        public override void OnRender()
        {
            StateManager.Push();
            StateManager.Translate(position);
            StateManager.Rotate(-rotation + 90);
            StateManager.SetColor(255, 255, 255, (int)(time / (float)20 * 255));
            StateManager.FillRoundRect(default, size, 4, 15);
            StateManager.Pop();
        }

        public override void OnUpdate()
        {
            move.Velocity *= 0.25f;
            base.OnUpdate();
            Despawn = time-- <= 0;
        }
    }
}

using KnueppelKampfBase.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Game.Components
{
    public class MoveComponent : GameComponent
    {
        const double MIN_VALUE = 0.001;
        private double limit;
        private double friction;
        private Vector velocity;
        public Vector Velocity { get => velocity; set => velocity = value; }
        public double Limit { get => limit; set => limit = value; }
        public double Friction { get => friction; set => friction = value; }

        public MoveComponent()
        {
            this.limit = 5;
        }

        public override void Render()
        {
            ;
        }

        public override void Update()
        {
            if(Velocity > limit)
                velocity.Length = limit;
            Velocity *= friction;
            if (Velocity < MIN_VALUE)
                velocity.Length = 0;
            this.GameObject.Position += velocity;
        }
    }
}

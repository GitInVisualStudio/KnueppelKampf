using KnueppelKampfBase.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Game.Components
{
    public class MoveComponent : GameComponent
    {
        const float MIN_VALUE = 0.001f;
        private float limit;
        private float friction;
        private Vector velocity;
        public Vector Velocity { get => velocity; set => velocity = value; }
        public float Limit { get => limit; set => limit = value; }
        public float Friction { get => friction; set => friction = value; }

        public MoveComponent(float limi, float firction)
        {
            this.limit = 5;
        }

        public override void OnRender()
        {
            ;
        }

        public override void OnUpdate()
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

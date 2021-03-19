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

        public float Length => velocity.Length;

        public MoveComponent(float limit = 5, float friction = 0.1f)
        {
            this.limit = limit;
            this.friction = friction;
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
                velocity = default;
            velocity.Y += 0.4f;
            this.GameObject.Position += velocity * 10;
        }

        public override ComponentState GetState()
        {
            return null;
        }

        public override void ApplyState(ComponentState state)
        {
            return;
        }
    }
}

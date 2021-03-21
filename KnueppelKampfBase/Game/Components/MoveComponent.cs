using KnueppelKampfBase.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Game.Components
{
    public class MoveComponent : GameComponent
    {
        const float MIN_VALUE = 0.001f;
        private bool onGround;
        private float limit;
        private float friction;
        private Vector velocity;
        public Vector Velocity { get => velocity; set => velocity = value; }
        public float Limit { get => limit; set => limit = value; }
        public float Friction { get => friction; set => friction = value; }

        public float Length => velocity.Length;

        public float X
        {
            get
            {
                return Velocity.X;
            }
            set
            {
                velocity.X = value;
            }
        }

        public float Y
        {
            get
            {
                return Velocity.Y;
            }
            set
            {
                velocity.Y = value;
            }
        }

        public bool OnGround { get => onGround; set => onGround = value; }

        public MoveComponent(float limit = 5, float friction = 0.25f)
        {
            this.limit = limit;
            this.friction = 1 - friction;
        }

        public override void OnRender()
        {
            ;
        }

        public override void OnUpdate()
        {
            if(OnGround)
                Velocity *= friction;
            OnGround = false;
            if (Velocity > limit)
                velocity.Length = limit;
            if (Velocity < MIN_VALUE)
                velocity = default;
            velocity.Y += 0.5f;
            this.GameObject.Position += velocity * 10;
        }

        public override ComponentState GetState()
        {
            return new MoveState() { Velocity = velocity, Friction = friction };
        }

        public override void ApplyState(ComponentState state)
        {
            if (!(state is MoveState))
                throw new Exception($"Invalid state for {this.GetType().Name}");
            MoveState ms = (MoveState)state;
            velocity = ms.Velocity;
            friction = ms.Friction;
        }
    }

    public class MoveState : ComponentState
    {
        private Vector velocity;
        private float friction;
        public Vector Velocity { get => velocity; set => velocity = value; }
        public float Friction { get => friction; set => friction = value; }
    }
}
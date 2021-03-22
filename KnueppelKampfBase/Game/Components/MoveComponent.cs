using KnueppelKampfBase.Math;
using KnueppelKampfBase.Utils;
using KnueppelKampfBase.Utils.Exceptions;
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

        [DontSerialize]
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

        [DontSerialize]
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
            
        }

        public override void OnUpdate()
        {
            OnGround = false;
            //if (Velocity > limit)
            //    velocity.Length = limit;
            Velocity *= friction;
            if (Velocity < MIN_VALUE)
                velocity = default;
            velocity.Y += 0.6f;
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

        public override int ToBytes(byte[] array, int startIndex)
        {
            int index = startIndex;
            GetHeader(array, index);
            index += HEADER_SIZE;
            index += ByteUtils.GetBytesAddSize(velocity, array, index);
            BitConverter.GetBytes(friction).CopyTo(array, index);
            index += sizeof(float);
            return index - startIndex;
        }

        public override GameComponent ToComponent()
        {
            return new MoveComponent(friction)
            {
                Velocity = velocity
            };
        }

        public static int FromBytes(byte[] bytes, int startIndex, out MoveState cs)
        {
            int index = startIndex;
            cs = new MoveState();
            int size = bytes[index++];
            byte[] velocityBytes = new byte[size];
            Array.Copy(bytes, index, velocityBytes, 0, size);
            cs.Velocity = (Vector)ByteUtils.FromBytes(velocityBytes, typeof(Vector));
            index += size;
            cs.Friction = BitConverter.ToSingle(bytes, index);
            index += sizeof(float);
            return index - startIndex;
        }
    }
}
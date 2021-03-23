using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game.Components
{
    public class HealthComponent : GameComponent
    {

        public const int MAX_HURTTIME = 10;

        private float health;
        private int hurttime;

        public float Health 
        { 
            get => health;
            set 
            {
                if(value < health)
                    Hurttime = MAX_HURTTIME;
                health = value;
            }
        }

        public bool Dead => health <= 0;

        public int Hurttime { get => hurttime; set => hurttime = value; }

        public HealthComponent(float health)
        {
            this.health = health;
        }

        public override void ApplyState(ComponentState state)
        {
            if (!(state is HealthState))
                throw new Exception($"Invalid state for {this.GetType().Name}");
            HealthState hs = (HealthState)state;
            Health = hs.Health;
            Hurttime = hs.Hurttime;
        }

        public override ComponentState GetState()
        {
            return new HealthState { Health = Health, Hurttime = Hurttime };
        }

        public override void OnRender()
        {
            
        }

        public override void OnUpdate()
        {
            if (Hurttime > 0)
                Hurttime--;
        }
    }

    public class HealthState : ComponentState
    {
        private float health;
        private int hurttime;
        public float Health { get => health; set => health = value; }
        public int Hurttime { get => hurttime; set => hurttime = value; }

        public override int ToBytes(byte[] array, int startIndex)
        {
            int index = startIndex;
            GetHeader(array, index);
            index += HEADER_SIZE;
            BitConverter.GetBytes(health).CopyTo(array, index);
            index += sizeof(float);
            BitConverter.GetBytes(hurttime).CopyTo(array, index);
            index += sizeof(int);
            return index - startIndex;
        }

        public override GameComponent ToComponent()
        {
            return new HealthComponent(health)
            {
                Hurttime = hurttime
            };
        }

        public static int FromBytes(byte[] bytes, int startIndex, out HealthState result)
        {
            result = new HealthState();
            result.Health = BitConverter.ToSingle(bytes, startIndex);
            startIndex += sizeof(float);
            result.Hurttime = BitConverter.ToInt32(bytes, startIndex);
            startIndex += sizeof(int);
            return sizeof(float) + sizeof(int);
        }
    }
}

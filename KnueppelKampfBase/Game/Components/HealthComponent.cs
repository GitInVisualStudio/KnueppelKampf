using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game.Components
{
    public class HealthComponent : GameComponent
    {

        private const int MAX_HURTTIME = 10;

        private float health;
        private int hurttime;

        public float Health 
        { 
            get => health;
            set 
            {
                if(value < health)
                    hurttime = MAX_HURTTIME;
                health = value;
            }
        }

        public bool Dead => health <= 0;

        public override void ApplyState(ComponentState state)
        {
            if (!(state is HealthState))
                throw new Exception($"Invalid state for {this.GetType().Name}");
            HealthState hs = (HealthState)state;
            Health = hs.Health;
            hurttime = hs.Hurttime;
        }

        public override ComponentState GetState()
        {
            return new HealthState { Health = Health, Hurttime = hurttime };
        }

        public override void OnRender()
        {
            
        }

        public override void OnUpdate()
        {
            if (hurttime > 0)
                hurttime--;
        }
    }

    public class HealthState : ComponentState
    {
        private float health;
        private int hurttime;
        public float Health { get => health; set => health = value; }
        public int Hurttime { get => hurttime; set => hurttime = value; }
    }
}

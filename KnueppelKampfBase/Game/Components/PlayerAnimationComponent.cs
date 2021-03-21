using KnueppelKampfBase.Game.Objects;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Math;

namespace KnueppelKampfBase.Game.Components
{
    public class PlayerAnimationComponent : GameComponent
    {
        private MoveComponent move;
        private HealthComponent health;
        private Player player;
        private float state;
        private float leftLeg, rightLeg;
        public PlayerAnimationComponent()
        {
        }

        public override void Init()
        {
            base.Init();
            this.player = (Player)GameObject;

            this.move = GameObject.GetComponent<MoveComponent>();
            this.health = GameObject.GetComponent<HealthComponent>();
            if (move == null)
                throw new Exception($"GameObject: {GameObject} hat kein component {typeof(MoveComponent)}");
            if (health == null)
                throw new Exception($"GameObject: {GameObject} hat kein component {typeof(HealthComponent)}");

        }

        public override void ApplyState(ComponentState state)
        {
            
        }

        public override ComponentState GetState()
        {
            return null;
        }

        public override void OnRender()
        {
            //StateManager.Translate(player.Size.X / 2, 0);
            state += move.Length * StateManager.delta * 10;
            StateManager.SetColor(player.Color);
            float delta = (HealthComponent.MAX_HURTTIME - health.Hurttime) / (float)HealthComponent.MAX_HURTTIME;
            float r = player.Color.R * delta + (1 - delta) * 255;
            float g = player.Color.G * delta;
            float b = player.Color.B * delta;
            StateManager.SetColor((int)r, (int)g, (int)b);

            int headSize = 25;
            StateManager.FillCircle(0, -player.Size.Y / 2 - (float)(-Sin(state) * 2), headSize);
            float width = player.Size.X / 5;
            float height = player.Size.Y;
            StateManager.FillRoundRect(-width / 2, -height/ 2, width, height / 2, 5, 10);
            float rot = (float)Sin(state) * 30;
            rightLeg += (rot - rightLeg) / 4 * StateManager.delta * 30;
            StateManager.Rotate(rightLeg);
            StateManager.FillRoundRect(-width / 2, -6, width, height / 2, 5, 10);
            StateManager.Rotate(-rightLeg);
            rot = (float)Sin(-state) * 30; //new value of leg rotation
            leftLeg += (rot - leftLeg) / 4 * StateManager.delta * 30;
            StateManager.Rotate(leftLeg);
            StateManager.FillRoundRect(-width / 2, -6, width, height / 2, 5, 10);
            StateManager.Rotate(-leftLeg);
        }

        public override void OnUpdate()
        {
            //return null;
        }
    }
}

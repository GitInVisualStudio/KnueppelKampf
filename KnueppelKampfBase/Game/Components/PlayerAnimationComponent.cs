using KnueppelKampfBase.Game.Objects;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;
using KnueppelKampfBase.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static System.Math;

namespace KnueppelKampfBase.Game.Components
{
    public class PlayerAnimationComponent : GameComponent
    {
        private MoveComponent move;
        private HealthComponent health;
        private ControlComponent control;
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
            this.control = GameObject.GetComponent<ControlComponent>();
            if (move == null)
                throw new Exception($"GameObject: {GameObject} hat kein component {typeof(MoveComponent)}");
            if (health == null)
                throw new Exception($"GameObject: {GameObject} hat kein component {typeof(HealthComponent)}");
            if (control == null)
                throw new Exception($"GameObject: {GameObject} hat kein component {typeof(ControlComponent)}");

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
            //RenderArm(default, b, -b);
            StateManager.Push();
            StateManager.SetColor(player.Color);
            float delta = (HealthComponent.MAX_HURTTIME - health.Hurttime) / (float)HealthComponent.MAX_HURTTIME;
            float r = player.Color.R * delta + (1 - delta) * 255;
            float g = player.Color.G * delta;
            float b = player.Color.B * delta;
            StateManager.SetColor((int)r, (int)g, (int)b);

            state += move.Length * StateManager.delta * 5;
            //WalkingAnimation((float)Sin(state));
            //WalkingAnimation((float)-Sin(state));

            int headSize = 25;
            StateManager.FillCircle(0, -player.Size.Y / 2 - (float)(-Sin(state) * 2), headSize);
            float width = player.Size.X / 5;
            float height = player.Size.Y;
            StateManager.FillRoundRect(-width / 2, -height / 2, width, height / 2, 5, 10);
            StateManager.Pop();
            //float rot = (float)Sin(state) * 30;
            //rightLeg += (rot - rightLeg) / 4 * StateManager.delta * 30;
            //StateManager.Rotate(rightLeg);
            //StateManager.FillRoundRect(-width / 2, -6, width, height / 2, 5, 10);
            //StateManager.Rotate(-rightLeg);
            //rot = (float)Sin(-state) * 30; //new value of leg rotation
            //leftLeg += (rot - leftLeg) / 4 * StateManager.delta * 30;
            //StateManager.Rotate(leftLeg);
            //StateManager.FillRoundRect(-width / 2, -6, width, height / 2, 5, 10);
            //StateManager.Rotate(-leftLeg);
        }

        private void WalkingAnimation(float d)
        {
            //StateManager.DrawRect(d * player.Size.X / 2, player.Size.Y / 2, 5, 5);
            float h = (float)Sqrt(d * d + 3);
            float alpha = (float)(Asin(d / h) * 180.0f / PI);
            float beta = (float)(Acos((h * h) / (2 * h)) * 180.0f / PI);
            float gamma = (float)(Acos((2 - h * h) / 2) * 180.0f / PI);
            if(move.X > 0)
                RenderArm(default, (alpha + beta), 180 + gamma);
            else
                RenderArm(default, -(alpha + beta), 180 - gamma);
        }

        private void RenderArm(Vector position, float a, float b)
        {
            Vector size = new Vector(player.Size.X / 5, player.Size.Y / 4);
            StateManager.Push();
            StateManager.Translate(position.X, position.Y);
            StateManager.Rotate(a);
            StateManager.FillRoundRect(-size.X / 2, 0, size.X, size.Y + 5, 5, 10);
            StateManager.Translate(0, size.Y);
            StateManager.Rotate(b);
            StateManager.FillRoundRect(-size.X / 2, 0, size.X, size.Y, 5, 10);
            StateManager.Pop();
        }

        public override void OnUpdate()
        {
            //return null;
        }
    }

    public class PlayerAnimationState : ComponentState
    {
        private static Type componentType = typeof(PlayerAnimationComponent);

        [DontSerialize]
        public static Type ComponentType { get => componentType; set => componentType = value; }
        public PlayerAnimationState()
        {

        }

        public override int ToBytes(byte[] array, int startIndex)
        {
            GetHeader(array, startIndex);
            return HEADER_SIZE;
        }

        public override GameComponent ToComponent()
        {
            return new PlayerAnimationComponent();
        }

        public static int FromBytes(byte[] bytes, int startIndex, out ComponentState cs)
        {
            cs = new PlayerAnimationState();
            return 0;
        }
    }
}

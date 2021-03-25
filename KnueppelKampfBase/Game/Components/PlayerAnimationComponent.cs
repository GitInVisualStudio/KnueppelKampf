using KnueppelKampfBase.Game.Objects;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Properties;
using KnueppelKampfBase.Render;
using KnueppelKampfBase.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using static System.Math;

namespace KnueppelKampfBase.Game.Components
{
    /// <summary>
    /// animiert eine spieler 
    /// </summary>
    public class PlayerAnimationComponent : GameComponent
    {
        private MoveComponent move;
        private HealthComponent health;
        private ControlComponent control;
        private ItemComponent item;
        private Player player;
        private float state;
        private float right, left;
        private float alpha;
        private float prev;
        private float currentHealth;
        private float time;
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
            this.item = GameObject.GetComponent<ItemComponent>();
            if (move == null)
                throw new Exception($"GameObject: {GameObject} hat kein component {typeof(MoveComponent)}");
            if (health == null)
                throw new Exception($"GameObject: {GameObject} hat kein component {typeof(HealthComponent)}");
            if (control == null)
                throw new Exception($"GameObject: {GameObject} hat kein component {typeof(ControlComponent)}");
            if(item == null)
                throw new Exception($"GameObject: {GameObject} hat kein component {typeof(ItemComponent)}");
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
            //blocking
            alpha += ((control.Blocking ? 140 : 0) - alpha) * StateManager.delta * 5;
            StateManager.SetColor(100, 0, 0, (int)alpha);
            StateManager.FillRoundRect(-player.Size / 2, player.Size);

            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(-25, -50, 50, 100);

            PathGradientBrush pthGrBrush = new PathGradientBrush(path);

            pthGrBrush.CenterColor = Color.FromArgb(100, player.Color);

            Color[] colors = { Color.FromArgb(0, player.Color) };
            pthGrBrush.SurroundColors = colors;

            StateManager.Graphics.FillEllipse(pthGrBrush, -25, -50, 50, 100);

            //hurttime
            StateManager.SetColor(player.Color);
            //float wie weit die hurttime ist 0-1
            //dementsprechend die spieler rot malen
            float delta = (HealthComponent.MAX_HURTTIME - health.Hurttime) / (float)HealthComponent.MAX_HURTTIME;

            StateManager.SetColor(200, 0, 0, (int)(255 - delta * 255));
            currentHealth += (health.Health / 10 - currentHealth) * StateManager.delta * 5;
            StateManager.FillRoundRect(-player.Width / 2, -player.Height / 2 - 20, player.Width * currentHealth, 15, 5, 15);


            float r = player.Color.R * delta + (1 - delta) * 255;
            float g = player.Color.G * delta;
            float b = player.Color.B * delta;
            StateManager.SetColor((int)r, (int)g, (int)b);


            //spieler an sich
            StateManager.Translate(0, 5);
            state += move.Length * StateManager.delta * 5;
            int headSize = 25;
            StateManager.FillCircle(0, -player.Size.Y / 2, headSize);
            float width = player.Size.X / 5;
            float height = player.Size.Y;
            StateManager.FillRoundRect(-width / 2, -height / 2, width, height / 2 + 3, 5, 10);

            //arme entsprchend der richtung animieren
            right += (-move.Velocity.Angle - right - 15 + (move.X < 0 ? -1 : 1) * item.Cooldown * -30) * StateManager.delta * 10;
            left += (-move.Velocity.Angle - left + 15 + (move.X < 0 ? -1 : 1) * item.Cooldown * -30) * StateManager.delta * 10;

            WalkingAnimation((float)Sin(state));
            WalkingAnimation((float)-Sin(state));

            //zeichnen der arme
            StateManager.Translate(0, -player.Size.Y / 2 + headSize - 10);
            StateManager.Rotate(right / 2);
            StateManager.FillRoundRect(-width / 2, 0, width, height / 2.5f, 5, 10);
            if (item.Item == Items.GUN)
                StateManager.DrawImage(Resources.pistol, -width, -5);
            StateManager.Rotate(-right / 2);
            StateManager.Rotate(left / 2);
            StateManager.FillRoundRect(-width / 2, 0, width, height / 2.5f, 5, 10);
            StateManager.Rotate(-left / 2);

            if (move.Velocity.Length > 1)
            {
                time += StateManager.delta;
                if(time > 0.25f / move.Velocity.Length)
                {
                    time = 0;
                    this.GameObject.Manager.AddObject(new Particle(this.GameObject));
                }
            }
                
        }

        /// <summary>
        /// zeichnet die beinbewegung
        /// </summary>
        /// <param name="d"></param>
        private void WalkingAnimation(float d)
        {
            //berechnet eine position auf dem boden wo der fuß stehen soll und bewegen diesen dann entsprechend
            float h = (float)Sqrt(d * d + 3); //+3 weil die höhe sqrt(3) ist
            float alpha = (float)(Asin(d / h) * 180.0f / PI);
            float beta = (float)(Acos(h / 2) * 180.0f / PI);
            float gamma = (float)(Acos((2 - h * h) / 2) * 180.0f / PI);
            float value = move.X;
            //wenn der spieler sich nicht beweget, soll die letzte richtung genommen werden
            if (value == 0)
                value = prev;
            else 
                prev = value;

            if (value < 0)
                RenderArm(default, (alpha + beta), 180 + gamma);
            else
                RenderArm(default, -(alpha + beta), 180 - gamma);
        }

        /// <summary>
        /// zeichnet einen arm mit gelenken
        /// </summary>
        /// <param name="position"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void RenderArm(Vector position, float a, float b)
        {
            Vector size = new Vector(player.Size.X / 5, player.Size.Y / 4);
            StateManager.Translate(position.X, position.Y);
            StateManager.Rotate(a);
            StateManager.FillRoundRect(-size.X / 2, 0, size.X, size.Y + 5, 5, 10);
            StateManager.Translate(0, size.Y);
            StateManager.Rotate(b);
            StateManager.FillRoundRect(-size.X / 2, 0, size.X, size.Y, 5, 10);
            StateManager.Rotate(-b);
            StateManager.Translate(0, -size.Y);
            StateManager.Rotate(-a);
            StateManager.Translate(-position.X, -position.Y);
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

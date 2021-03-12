using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Game.Components
{
    public class AnimationComponent : GameComponent
    {
        private event Action<Vector> onRender;
        private float speed;
        private Vector current;
        private Vector target;
        public Vector Target { get => target; set => target = value; }

        public AnimationComponent(float speed, Vector target, Vector current = default, Action<Vector> onRender = null)
        {
            this.onRender += onRender;
            this.speed = speed;
            this.target = target;
            if (current == default)
                this.current = target;
            else
                this.current = current;
        }

        public override void OnRender()
        {
            current += (target - current) * speed * StateManager.delta;
            onRender?.Invoke(current);
        }

        public override void OnUpdate()
        {
            
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

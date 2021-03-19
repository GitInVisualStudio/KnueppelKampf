using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Game.Components
{
    public class ControlComponent : GameComponent
    {
        private Action<int> keyPressed;
        private Action<int> keyRelease;
        private Action<int> keyDown;

        public ControlComponent(Action<int> keyPressed = null, Action<int> keyRelease= null, Action<int> keyDown = null)
        {
            this.keyDown = keyDown;
            this.keyPressed = keyPressed;
            this.keyRelease = keyRelease;
        }

        public Action<int> KeyPressed { get => keyPressed; set => keyPressed = value; }
        public Action<int> KeyRelease { get => keyRelease; set => keyRelease = value; }
        public Action<int> KeyDown { get => keyDown; set => keyDown = value; }

        public override void ApplyState(ComponentState state)
        {
            return;
        }

        public override ComponentState GetState()
        {
            return null;
        }

        public override void OnRender()
        {
            //return null;
        }

        public override void OnUpdate()
        {
            //return null;
        }
    }
}

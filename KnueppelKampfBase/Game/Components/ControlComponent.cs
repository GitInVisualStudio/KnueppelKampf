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

        public override void OnRender()
        {
            //throw new NotImplementedException();
        }

        public override void OnUpdate()
        {
            //throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Game.Components
{
    public class ClickComponent : GameComponent
    {
        private event Action<int, int> clickEvent;

        public ClickComponent(Action<int, int> clickEvent)
        {
            this.clickEvent += clickEvent;
        }

        public override void OnRender()
        {
            
        }

        public override void OnUpdate()
        {
            
        }

        public void Click(int x, int y)
        {
            //if(x < )
        }

        public override ComponentState GetState()
        {
            throw new NotImplementedException();
        }

        public override void ApplyState(ComponentState state)
        {
            throw new NotImplementedException();
        }
    }
}

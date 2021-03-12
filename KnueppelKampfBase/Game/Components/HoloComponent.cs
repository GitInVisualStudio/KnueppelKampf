using KnueppelKampfBase.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace KnueppelKampfBase.Game.Components
{
    public class HoloComponent : GameComponent
    {
        private Bitmap holo;
        public HoloComponent(Bitmap tile)
        {
            //this.holo = RenderUtils.BlurImage()
        }

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
            return;
        }

        public override void OnUpdate()
        {
            return;
        }
    }
}

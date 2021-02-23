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

        public override void OnRender()
        {
            throw new NotImplementedException();
        }

        public override void OnUpdate()
        {
            throw new NotImplementedException();
        }
    }
}

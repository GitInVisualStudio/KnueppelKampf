using KnueppelKampfBase.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;

namespace KnueppelKampfBase.Game.Components
{
    public class TileMapComponent : GameComponent
    {
        private Bitmap[] tiles;
        private int time;
        private int index;
        private int delay;
        public TileMapComponent(int delay, params Bitmap[] tiles)
        {
            this.Tiles = tiles;
            this.delay = delay;
        }
        public int Delay { get => delay; set => delay = value; }
        public Bitmap[] Tiles { get => tiles; set => tiles = value; }
        public int Index { get => index; set => index = value; }

        public Bitmap Current => this.tiles[Index];

        public override void ApplyState(ComponentState state)
        {
            throw new NotImplementedException();
        }

        public override ComponentState GetState()
        {
            throw new NotImplementedException();
        }

        public override void OnRender()
        {
            //TODO: Statemanager
            StateManager.DrawImage(Current, GameObject.Size / -2);
        }

        public override void OnUpdate()
        {
            time++;
            time %= delay;
            if (time == 0)
                index++;
        }
    }
}

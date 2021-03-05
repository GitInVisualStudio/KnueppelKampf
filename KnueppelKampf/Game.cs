using KnueppelKampfBase.Game;
using KnueppelKampfBase.Game.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnueppelKampfBase.Math;
using System.Windows.Forms;
using KnueppelKampfBase.Render;

namespace KnueppelKampf
{
    public class Game : Window
    {

        public Game(int fps, int tps) : base(fps, tps)
        {
        }

        public override void Init()
        {
            base.Init();
            this.worldManager.Entities.Add(new Player(new Vector(50.0f, 50.0f)));
        }

        public override void OnRender()
        {
            base.OnRender();
        }
    }
}

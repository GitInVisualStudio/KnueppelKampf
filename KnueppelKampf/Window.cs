using KnueppelKampf.Properties;
using KnueppelKampfBase.Render;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnueppelKampf
{
    public partial class Window : Form
    {
        public Window()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            StateManager.Update(e.Graphics);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Bitmap blure = RenderUtils.BlurImage(Resources.bild, 15, 10);
            watch.Stop();
            StateManager.SetColor(Color.Black);
            StateManager.DrawImage(blure, 0, 0);
            StateManager.DrawString("Time: " + watch.Elapsed.TotalSeconds, 10, 10);
        }
    }
}

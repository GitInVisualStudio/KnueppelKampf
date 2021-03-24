using KnueppelKampfBase.Render;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KnueppelKampfBase.Game
{
    public class Window : Form
    {
        private int fps, tps;
        private float tpt;
        private Stopwatch watch;
        private Timer fpsTimer, tpsTimer;
        protected WorldManager worldManager;
        public Window(int fps, int tps)
        {
            this.fps = fps;
            this.tps = tps;
            this.tpt = 1000.0f / tps;
            Init();
        }

        public virtual void Init()
        {

            this.DoubleBuffered = true;
            this.watch = new Stopwatch();
            this.watch.Start();

            this.fpsTimer = new Timer();
            this.fpsTimer.Interval = (int)(1000.0f / (float)fps);
            this.fpsTimer.Tick += FpsTimer_Tick;

            this.tpsTimer = new Timer();
            this.tpsTimer.Interval = (int)(1000.0f / (float)tps);
            this.tpsTimer.Tick += TpsTimer_Tick;

            this.tpsTimer.Start();
            this.fpsTimer.Start();


            this.worldManager = new WorldManager();
        }

        private void TpsTimer_Tick(object sender, EventArgs e)
        {
            this.watch.Restart();
            this.OnUpdate();
        }

        private void FpsTimer_Tick(object sender, EventArgs e)
        {
            this.Refresh();
        }

        protected  override void OnPaint(PaintEventArgs e)
        {
            Graphics g = null;
            base.OnPaint(e);
            float partialTicks = (float)(tpt - watch.Elapsed.TotalMilliseconds) / tpt;
            StateManager.partialTicks = partialTicks;
            StateManager.Update(e.Graphics);
            this.OnRender();
        }

        protected virtual void OnRender()
        {
            worldManager.OnRender();
        }

        protected virtual void OnUpdate()
        {
            worldManager.OnUpdate();
        }        
        
    }
}

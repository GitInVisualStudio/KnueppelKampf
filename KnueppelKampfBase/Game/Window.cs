﻿using KnueppelKampfBase.Render;
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
        private Stopwatch watch;
        private Timer fpsTimer, tpsTimer;
        public Window(int fps, int tps)
        {
            this.fps = fps;
            this.tps = tps;
            Init();
        }

        public virtual void Init()
        {
            this.watch = new Stopwatch();
            this.watch.Start();

            this.fpsTimer = new Timer();
            this.fpsTimer.Interval = (int)(1000.0f / (float)fps);
            this.fpsTimer.Tick += FpsTimer_Tick;

            this.tpsTimer = new Timer();
            this.tpsTimer.Interval = (int)(1000.0f / (float)tps);
            this.tpsTimer.Tick += TpsTimer_Tick;
            this.tpsTimer.Start();
        }

        private void TpsTimer_Tick(object sender, EventArgs e)
        {
            this.OnUpdate();
        }

        private void FpsTimer_Tick(object sender, EventArgs e)
        {
            this.watch.Restart();
            this.Refresh();
        }

        protected  override void OnPaint(PaintEventArgs e)
        {
            Graphics g = null;
            base.OnPaint(e);
            float partialTicks = (float)((1000.0f / tps) - watch.Elapsed.TotalMilliseconds) / (1000.0f / tps);
            StateManager.partialTicks = partialTicks;
            StateManager.Update(e.Graphics);
        }

        protected virtual void OnRender()
        {
        }

        protected virtual void OnUpdate()
        {

        }        
    }
}

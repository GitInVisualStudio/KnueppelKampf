using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Render
{
    /// <summary>
    /// Stadie des StateManagers für die einfache Handhabung der Translation/Rotation/Skalierung der Matrix
    /// Speicher alle Werte des StateManagers in einem State um vorherige States wieder herzustellen
    /// </summary>
    public class State
    {
        private Color color;
        private Font font = FontUtils.DEFAULT_FONT;
        private float scaleX = 1, scaleY = 1, translateX = 0, translateY = 0;
        private float rotation = 0;
        private Graphics graphics;
        private State prevState;

        public Font Font
        {
            get
            {
                return font;
            }

            set
            {
                font = value;
            }
        }

        public float ScaleX
        {
            get
            {
                return scaleX;
            }

            set
            {
                scaleX = value;
            }
        }

        public float ScaleY
        {
            get
            {
                return scaleY;
            }

            set
            {
                scaleY = value;
            }
        }

        public float TranslateX
        {
            get
            {
                return translateX;
            }

            set
            {
                translateX = value;
            }
        }

        public float TranslateY
        {
            get
            {
                return translateY;
            }

            set
            {
                translateY = value;
            }
        }

        public float Rotation
        {
            get
            {
                return rotation;
            }

            set
            {
                rotation = value;
            }
        }

        public State PrevState
        {
            get
            {
                return prevState;
            }

            set
            {
                prevState = value;
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        public Graphics Graphics
        {
            get
            {
                return graphics;
            }

            set
            {
                graphics = value;
            }
        }

        public State(bool createNew = false)
        {
            if (createNew)
            {
                return;
            }

            ScaleX = StateManager.ScaleX;
            ScaleY = StateManager.ScaleY;
            TranslateX = StateManager.TranslateX;
            TranslateY = StateManager.TranslateY;
            Font = StateManager.Font;
            Rotation = StateManager.Rotation;
            PrevState = StateManager.State;
            color = StateManager.Color;
            Graphics = StateManager.Graphics;
        }
    }
}
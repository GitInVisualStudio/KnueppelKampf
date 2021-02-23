using KnueppelKampfBase.Math;
using System;
using System.Collections.Generic;
using System.Text;
using ma = System.Math;

namespace KnueppelKampfBase.Game.Components
{
    public class BoxComponent : GameComponent
    {
        private Vector[] corners;
        private Vector[] original;
        public override void Init()
        {
            base.Init();
            this.corners = new Vector[4];
            original = new Vector[4] //Nicht absolut, nur die Größe sonst wird beim bewegen alles verfälscht
            {
                new Vector(-GameObject.Width / 2, -GameObject.Height / 2),
                new Vector(GameObject.Width / 2, -GameObject.Height / 2),
                new Vector(GameObject.Width / 2, GameObject.Height / 2),
                new Vector(-GameObject.Width / 2, GameObject.Height / 2)
            };
        }

        public override void OnRender()
        {
        }

        public override void OnUpdate()
        {
        }

        /// <summary>
        /// Setzt die Ecken der Box absolut, wenn die Besitzer-Entity sich bewegt oder rotiert hat
        /// </summary>
        /// <param name="angle">Der Winkel, in dem die Box gedreht ist</param>
        private void UpdateCorners()
        {
            float angle = GameObject.Rotation;
            for (int i = 0; i < corners.Length; i++)
            {
                Vector current = original[i];
                double x = GameObject.X + current.X * ma.Cos(angle) - current.Y * ma.Sin(angle);
                double y = GameObject.Y + current.X * ma.Sin(angle) + current.Y * ma.Cos(angle);
                corners[i] = new Vector((float)x, (float)y);
            }
        }

        /// <summary>
        /// Überprüft mithilfe des Separating-Axes-Theorem, ob diese und eine andere Box kollidieren
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool Collides(BoxComponent box)
        {
            float[] angles; // Array der Winkel der Achsen, die überprüft werden müssen
            UpdateCorners();
            box.UpdateCorners();
            if (GameObject.Rotation == box.GameObject.Rotation) // falls beide Boxen gleich ausgerichtet sind, sind ihre Achsen die selben
                angles = new float[] { GameObject.Rotation, GameObject.Rotation + (float)ma.PI / 2.0f };
            else
                angles = new float[] { GameObject.Rotation, GameObject.Rotation + (float)ma.PI / 2.0f, box.GameObject.Rotation, box.GameObject.Rotation + (float)ma.PI / 2.0f };

            foreach (float angle in angles)
                if (!ProjectionOverlaps(ProjectOnto(angle), box.ProjectOnto(angle)))
                    return false;

            return true;
        }

        /// <summary>
        /// Projiziert diese Box auf eine Achse
        /// </summary>
        /// <param name="angle">Der Winkel der Achse im Bogenmaß</param>
        /// <returns>Array von zwei floats, die die relativ gesehen linkste und rechteste Stelle auf der Achse darstellen</returns>
        private float[] ProjectOnto(float angle)
        {
            float min = Int32.MaxValue;
            float max = Int32.MinValue;

            foreach (Vector c in corners)
            {
                float cuttingAngle = (float)ma.Atan(c.Y / c.X) - angle;
                float projection = (float)(ma.Cos(cuttingAngle) * ma.Sqrt(ma.Pow(c.X, 2) + ma.Pow(c.Y, 2)));
                min = projection < min ? projection : min;
                max = projection > max ? projection : max;
            }

            return new float[] { min, max };
        }

        /// <summary>
        /// Überprüft, ob sich zwei Projektionen überlappen
        /// </summary>
        private bool ProjectionOverlaps(float[] a, float[] b)
        {
            if ((a[0] < b[0] && a[1] > b[0]) || (b[0] < a[0] && b[1] > a[0]))
                return true;
            return false;
        }
    }
}

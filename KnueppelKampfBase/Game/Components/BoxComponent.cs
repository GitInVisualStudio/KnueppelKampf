﻿using KnueppelKampfBase.Math;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Math;

namespace KnueppelKampfBase.Game.Components
{
    public class BoxComponent : GameComponent
    {
        private Vector[] corners;
        private Vector[] original;
        private Action<BoxComponent> onCollision;

        public Action<BoxComponent> OnCollision { get => onCollision; set => onCollision = value; }

        public BoxComponent(Action<BoxComponent> onCollision)
        {
            this.OnCollision = onCollision;
        }

        public BoxComponent()
        {
            this.OnCollision = null;
        }

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
            double angle = GameObject.Rotation;
            for (int i = 0; i < corners.Length; i++)
            {
                Vector current = original[i];
                double x = GameObject.X + GameObject.Size.X / 2 + current.X * Cos(angle) - current.Y * Sin(angle);
                double y = GameObject.Y + GameObject.Size.Y / 2 + current.X * Sin(angle) + current.Y * Cos(angle);
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
            double[] angles; // Array der Winkel der Achsen, die überprüft werden müssen
            UpdateCorners();
            box.UpdateCorners();
            if (GameObject.Rotation == box.GameObject.Rotation)
                angles = new double[] { GameObject.Rotation, GameObject.Rotation + (double)PI / 2.0f };
            else
                angles = new double[] { GameObject.Rotation, GameObject.Rotation + (double)PI / 2.0f, box.GameObject.Rotation, box.GameObject.Rotation + (double)PI / 2.0f };

            foreach (double angle in angles)
                if (!ProjectionOverlaps(ProjectOnto(angle), box.ProjectOnto(angle)))
                    return false;

            return true;
        }

        public void CheckCollision(IEnumerable<BoxComponent> boxes)
        {
            foreach (BoxComponent b in boxes)
                if (b != this && Collides(b))
                    OnCollision?.Invoke(b);
        }

        /// <summary>
        /// Projiziert diese Box auf eine Achse
        /// </summary>
        /// <param name="angle">Der Winkel der Achse im Bogenmaß</param>
        /// <returns>Array von zwei doubles, die die relativ gesehen linkste und rechteste Stelle auf der Achse darstellen</returns>
        private double[] ProjectOnto(double angle)
        {
            double min = Int32.MaxValue;
            double max = Int32.MinValue;

            foreach (Vector c in corners)
            {
                double cuttingAngle = (double)Atan(c.Y / c.X) - angle;
                double projection = (double)(Sin(cuttingAngle) * Sqrt(Pow(c.X, 2) + Pow(c.Y, 2)));
                min = projection < min ? projection : min;
                max = projection > max ? projection : max;
            }

            return new double[] { min, max };
        }


        /// <summary>
        /// Überprüft, ob sich zwei Projektionen überlappen
        /// </summary>
        private bool ProjectionOverlaps(double[] a, double[] b)
        {
            if ((a[0] < b[0] && a[1] > b[0]) || (b[0] < a[0] && b[1] > a[0]))
                return true;
            return false;
        }

        public override ComponentState GetState()
        {
            return null;
        }

        public override void ApplyState(ComponentState state)
        {
            
        }
    }
}

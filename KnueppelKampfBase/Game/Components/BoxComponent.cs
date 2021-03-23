using KnueppelKampfBase.Math;
using KnueppelKampfBase.Utils;
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
                double x = GameObject.X + GameObject.Size.X / 2 + current.X;
                double y = GameObject.Y + GameObject.Size.Y / 2 + current.Y;
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

            angles = new double[] { 0, PI / 2.0f };

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
            return new BoxState();
        }

        public override void ApplyState(ComponentState state)
        {

        }
    }

    public class BoxState : ComponentState
    {
        private static Type componentType = typeof(BoxComponent);

        [DontSerialize]
        public static Type ComponentType { get => componentType; set => componentType = value; }

        public override int ToBytes(byte[] array, int startIndex)
        {
            GetHeader(array, startIndex);
            return HEADER_SIZE;
        }

        public override GameComponent ToComponent()
        {
            return new BoxComponent();
        }

        public static int FromBytes(byte[] bytes, int startIndex, out BoxState bs)
        {
            bs = new BoxState();
            return 0;
        }
    }
}
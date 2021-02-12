using System;
using System.Collections.Generic;
using System.Text;
using m = System.Math;

namespace KnueppelKampfBase.Math
{
    public struct Vector
    {
        private double x, y;

        public Vector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        
        public double Length
        {
            get
            {
                return m.Sqrt(x * x + y * y);
            }
            set
            {
                this /= Length;
                this *= value;
            }
        }

        public static Vector operator +(Vector a, double value)
        {
            a.x += value;
            a.y += value;
            return a;
        }
        public static Vector operator -(Vector a, double value)
        {
            a.x -= value;
            a.y -= value;
            return a;
        }
        public static Vector operator *(Vector a, double value)
        {
            a.x *= value;
            a.y *= value;
            return a;
        }
        public static Vector operator /(Vector a, double value)
        {
            a.x /= value;
            a.y /= value;
            return a;
        }

        /// <summary>
        /// NOTE: vector a wird zurückgegeben und auch verändert
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector operator /(Vector a, Vector b)
        {
            a.x /= b.x;
            a.y /= b.y;
            return a;
        }
        public static Vector operator *(Vector a, Vector b)
        {
            a.x *= b.x;
            a.y *= b.y;
            return a;
        }
        public static Vector operator +(Vector a, Vector b)
        {
            a.x += b.x;
            a.y += b.y;
            return a;
        }
        public static Vector operator -(Vector a, Vector b)
        {
            a.x -= b.x;
            a.y -= b.y;
            return a;
        }

        public static bool operator >(Vector a, double value)
        {
            return a.Length > value;
        }

        public static bool operator <(Vector a, double value)
        {
            return a.Length > value;
        }
    }
}

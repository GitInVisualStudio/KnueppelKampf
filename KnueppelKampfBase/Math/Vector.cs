using System;
using System.Collections.Generic;
using System.Text;
using m = System.Math;

namespace KnueppelKampfBase.Math
{
    public struct Vector
    {
        private float x, y;

        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }
        
        public float Length
        {
            get
            {
                return (float)m.Sqrt(x * x + y * y);
            }
            set
            {
                this /= Length;
                this *= value;
            }
        }

        public static Vector operator +(Vector a, float value)
        {
            a.x += value;
            a.y += value;
            return a;
        }
        public static Vector operator -(Vector a, float value)
        {
            a.x -= value;
            a.y -= value;
            return a;
        }
        public static Vector operator *(Vector a, float value)
        {
            a.x *= value;
            a.y *= value;
            return a;
        }
        public static Vector operator /(Vector a, float value)
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

        public static bool operator >(Vector a, float value)
        {
            return a.Length > value;
        }

        public static bool operator <(Vector a, float value)
        {
            return a.Length > value;
        }
    }
}

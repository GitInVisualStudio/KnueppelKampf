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

        public float Angle
        {
            get
            {
                return (float)(-m.Atan2(x, y) * 180.0f / m.PI);
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

        public static Vector operator -(Vector a)
        {
            return a * -1;
        }

        public static bool operator >(Vector a, float value)
        {
            return a.Length > value;
        }

        public static bool operator <(Vector a, float value)
        {
            return a.Length < value;
        }

        public static bool operator >(Vector a, Vector b)
        {
            return a.x > b.x && a.y > b.y;
        }

        public static bool operator <(Vector a, Vector b)
        {
            return a.x < b.x && a.y < b.y;
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return a.x == b.x && a.x == b.x;
        }
        public static bool operator ==(Vector a, Vector b)
        {
            return a.x != b.x || a.x != b.x;
        }

        public override string ToString()
        {
            return $"{x}:{y}";
        }
    }
}

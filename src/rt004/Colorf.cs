using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    internal struct Colorf
    {
        public static Colorf WHITE => new Colorf(1, 1, 1);
        public static Colorf BLACK => new Colorf(0, 0, 0);

        public float r, g, b;

        public Colorf(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public Colorf(float gray) : this(gray, gray, gray) { }

        public Colorf Clamp()
        {
            if (r < 0) r = 0;
            if (g < 0) g = 0;
            if (b < 0) b = 0;

            if (r > 1) r = 1;
            if (g > 1) g = 1;
            if (b > 1) b = 1;

            return this;
        }

        public static Colorf operator +(Colorf a, Colorf b)
            => new(a.r + b.r, a.g + b.g, a.b + b.b);

        public static Colorf operator *(float a, Colorf c)
            => new(a * c.r, a * c.g, a * c.b);

        public static Colorf operator *(Colorf c, float a)
            => a * c;

        public static Colorf operator /(Colorf c, float a)
            => new(c.r / a, c.g / a, c.b / a);

        public static Colorf operator *(Colorf a, Colorf b)
            => new(a.r * b.r, a.g * b.g, a.b * b.b);

        public static implicit operator Colorf(Color color) => new(color.R / 255f, color.G / 255f, color.B / 255f);

        public static explicit operator float[] (Colorf color) => new float[3] { color.r, color.g, color.b };
    }
}

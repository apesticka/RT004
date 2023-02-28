using System;
using System.Collections.Generic;
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

        public static Colorf operator +(Colorf a, Colorf b)
            => new(a.r + b.r, a.g + b.g, a.b + b.b);

        public static Colorf operator *(float a, Colorf c)
            => new(a * c.r, a * c.g, a * c.b);

        public static Colorf operator *(Colorf c, float a)
            => a * c;

        public static explicit operator float[] (Colorf color) => new float[3] { color.r, color.g, color.b };
    }
}

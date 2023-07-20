using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace rt004
{
    public struct Colorf
    {
        public static Colorf WHITE => new Colorf(1, 1, 1);
        public static Colorf BLACK => new Colorf(0, 0, 0);

        [XmlAttribute("r")]
        public float r;
        [XmlAttribute("g")]
        public float g;
        [XmlAttribute("b")]
        public float b;

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

        public static implicit operator Colorf(string s)
        {
            return new Colorf(0.3f, 0.3f, 0.3f);
        }

        public override string ToString() => $"[{r},{g},{b}]";

        internal static Colorf FromString(string colorString)
        {
            if (colorString == "") return new();
            if (colorString[0] != '[' || colorString[^1] != ']') throw new FormatException($"Wrong color string: \"{colorString}\". Color string must be enclosed in brackets");
            string[] stringComponents = colorString[1..^1].Split(',');
            float[] floatComponents = new float[3];
            for (int i = 0; i < floatComponents.Length; i++)
                if (stringComponents.Length > i) floatComponents[i] = float.Parse(stringComponents[i].Trim());
            return new Colorf(floatComponents[0], floatComponents[1], floatComponents[2]);
        }
    }
}

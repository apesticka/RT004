using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace rt004
{
    public class Scene
    {
        static int RECURSION_DEPTH => Config.Instance.General.MaxDepth;

        public Shape[] shapes { get; init; }
        public Light[] lights { get; init; }
        public Camera cam { get; init; }

        public Colorf Evaluate(Ray ray, int depth = 0)
        {
            if (depth > RECURSION_DEPTH) return Colorf.BLACK;

            RayHit? closest = null;
            foreach (var shape in shapes)
            {
                RayHit? hit = shape.IntersectRay(ray);
                if (closest == null || (hit != null && hit.Value.Distance < closest.Value.Distance))
                    closest = hit;
            }


            if (closest.HasValue)
            {
                RayHit hit = closest.Value;

                Material mat = hit.Shape.Material ?? Program.DefaultMaterial;

                Colorf color = mat.Evaluate(this, hit.Point, -ray.Direction, hit.Normal, Program.AmbientIntensity, depth);

                return color;
            }
            else
            {
                return Config.Instance.General.BackgroundColor;
            }
        }

        public void Render(FloatImage image)
        {
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    //if (x == image.Width / 2 && y == image.Height / 2)
                    if (x == 50 && y == 350)
                        Console.WriteLine();

                    Ray ray = cam.GenerateRay(x, y);

                    Colorf color = Evaluate(ray);

                    image.PutPixel(x, y, (float[])color);
                }
            }
        }
    }
}

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
        public SceneGraph Graph;
        public Light[] Lights { get; init; }
        public Camera Cam { get; init; }

        public RayHit? IntersectRay(Ray ray)
        {
            return Graph.IntersectRay(ray);
        }

        public Colorf Evaluate(Ray ray, int depth = 0)
        {
            if (depth > Config.Instance.General.MaxDepth) return Colorf.BLACK;

            RayHit? closest = IntersectRay(ray);

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
            int x = 0, y = 0;
            int maxIndex = image.Width * image.Height;

            Task.Run(async () =>
            {
                int index = 0;
                while (index < maxIndex - 1)
                {
                    index = y + x * image.Height;
                    float percentage = index / (float)maxIndex;
                    await Task.Delay(500);
                    await Console.Out.WriteLineAsync(string.Format("{0:00.00%}", percentage));
                }
            });

            for (x = 0; x < image.Width; x++)
            {
                for (y = 0; y < image.Height; y++)
                {
                    Ray ray = Cam.GenerateRay(x, y);

                    Colorf color = Evaluate(ray);

                    image.PutPixel(x, y, (float[])color);
                }
            }
        }
    }
}

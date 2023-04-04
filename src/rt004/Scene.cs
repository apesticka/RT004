using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace rt004
{
    internal class Scene
    {
        public Shape[] shapes { get; init; }
        public Light[] lights { get; init; }
        public Camera cam { get; init; }

        public void Render(FloatImage image)
        {
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Ray ray = cam.GenerateRay(x, y);
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

                        Material mat = hit.Shape.material ?? Program.DefaultMaterial;

                        Colorf color = mat.Evaluate(lights, hit.Point, -ray.Direction, hit.Normal, Program.AmbientIntensity, shapes);

                        image.PutPixel(x, y, (float[])color);
                    }
                    else
                    {
                        image.PutPixel(x, y, (float[])Config.BackgroundColor);
                    }
                }
            }
        }
    }
}

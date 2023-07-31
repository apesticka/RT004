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
        public ShapeNode[] shapes { get; init; }
        public SceneConfig.GraphNode Graph;
        public Light[] lights { get; init; }
        public Camera cam { get; init; }

        public RayHit? IntersectRay(Ray ray)
        {
            return Graph.IntersectRay(ray);

            RayHit? closest = null;
            foreach (var shape in shapes)
            {
                RayHit? hit = shape.IntersectRay(ray);
                if (closest == null || (hit != null && closest.Value.Distance > hit.Value.Distance))
                    closest = hit;
            }
            return closest;
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
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Ray ray = cam.GenerateRay(x, y);

                    Colorf color = Evaluate(ray);

                    image.PutPixel(x, y, (float[])color);
                }
            }
        }
    }
}

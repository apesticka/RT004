using OpenTK.Mathematics;
using Util;
//using System.Numerics;

namespace rt004;

internal class Program
{
    const double DiffuseCoefficient = 1.0;

    static void Main(string[] args)
    {
        Config.Load(args[0]);

        List<Shape> scene = new List<Shape> {
            new Plane { point = Vector3d.Zero, normal = Vector3d.UnitY },
            new Sphere { position = new Vector3d(-1, 1, 3), radius = 1 }
        };
        Light light = new DirectionalLight(new Vector3d(-1, -1, 1), 1);
        Camera cam = Camera.Create(Vector3d.UnitY, Quaterniond.Identity, Math.PI / 2);

        // HDR image.
        FloatImage fi = new FloatImage(Config.Width, Config.Height, 3);

        for (int x = 0; x < Config.Width; x++)
        {
            for (int y = 0; y < Config.Height; y++)
            {
                Ray ray = cam.GenerateRay(x, y);
                RayHit? closest = null;
                foreach (var shape in scene)
                {
                    RayHit? hit = shape.IntersectRay(ray);
                    if (closest == null || (hit != null && hit.Value.Distance < closest.Value.Distance))
                        closest = hit;
                }

                if (closest.HasValue)
                {
                    RayHit hit = closest.Value;
                    float diff = (float)(light.GetIntensity(hit.Point) * DiffuseCoefficient * Vector3d.Dot(-light.GetDirection(hit.Point), hit.Normal));

                    fi.PutPixel(x, y, new float[] { diff, diff, diff });
                }
                else
                {
                    fi.PutPixel(x, y, Config.BackgroundColor);
                }

                //fi.PutPixel(x, y, new float[] { x / (float)Config.Width, y / (float)Config.Height, 1f });
            }
        }

        //fi.SaveHDR(fileName);   // Doesn't work well yet...
        fi.SavePFM(Config.OutputFilename);

        Console.WriteLine("HDR image is finished.");
    }
}

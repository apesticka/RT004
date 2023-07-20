using OpenTK.Mathematics;
using System.Drawing;
using Util;
//using System.Numerics;

namespace rt004;

internal class Program
{
    public static readonly Colorf AmbientIntensity = Colorf.WHITE;
    public static readonly Material DefaultMaterial = new UnlitMaterial { color = Color.Magenta };

    static void Main(string[] args)
    {
        Config.Load(args[0]);

        Material yellow = new PhongMaterial { ambient = 0.1f, diffuse = 0.8f, specular = 0.2f, highlight = 10, color = new Colorf(1, 1, 0) };
        Material blue = new PhongMaterial { ambient = 0.1f, diffuse = 0.5f, specular = 0.5f, highlight = 150, color = new Colorf(0.2f, 0.3f, 1f) };
        Material red = new PhongMaterial { ambient = 0.1f, diffuse = 0.6f, specular = 0.4f, highlight = 80, color = new Colorf(0.8f, 0.2f, 0.2f) };
        Material gold = new PhongMaterial { ambient = 0.2f, diffuse = 0.2f, specular = 0.8f, highlight = 400, color = new Colorf(0.3f, 0.2f, 0.0f) };
        Material white = new PhongMaterial { ambient = 0.1f, diffuse = 0.6f, specular = 0.4f, highlight = 80, color = new Colorf(0.9f, 0.9f, 0.9f) };


        Scene scene = new()
        {
            shapes = new Shape[] {
                new Plane { point = -Vector3d.UnitY, normal = Vector3d.UnitY, material = new PhongMaterial { ambient = 0.1f, diffuse = 0.5f, specular = 0.5f, highlight = 150, color = Color.Peru } },
                //new Sphere { position = new Vector3d(-1, 1, 3), radius = 1, color = Color.Turquoise }
                new Sphere { position = new Vector3d(0, 0, 0), radius = 1, material = yellow },
                new Sphere { position = new Vector3d(1.4, -0.7, -0.5), radius = 0.6, material = blue },
                new Sphere { position = new Vector3d(-0.7, 0.7, -0.8), radius = 0.1, material = red }
            },

            lights = new Light[] {
                //new DirectionalLight(new Vector3d(-1, -1, 1), 0.4f * Colorf.WHITE),
                new PointLight(new Vector3d(-10.0, 8.0, -1.0), 1),
                new PointLight(new Vector3d(0.0, 20.0, -3.0), Color.Red)
                //new PointLight(new Vector3d(-3, 1.5, 1.5), Color.MediumVioletRed)
            },

            cam = new Camera(new Vector3d(0.6, 0.0, -5.6), Quaterniond.Identity, Math.PI / 180 * 40)
        };

        Scene ch2scene = new()
        {
            shapes = new Shape[] {
                new Sphere { position = new Vector3d(), radius = 1.0, material = yellow },
                new Sphere { position = new Vector3d(1.4, -0.7, -0.5), radius = 0.6, material = blue },
                new Sphere { position = new Vector3d(-0.7, 0.7, -0.8), radius = 0.1, material = red }
            },

            lights = new Light[] {
                new PointLight(new Vector3d(-10.0, 8.0, -6.0), new Colorf(1.0f, 1.0f, 1.0f)),
                new PointLight(new Vector3d(0.0, 20.0, -3.0), new Colorf(0.3f, 0.3f, 0.3f))
            },

            cam = new Camera(new Vector3d(0.6, 0.0, -5.6), Quaterniond.Identity, Math.PI / 180 * 40)
        };

        Scene ch3scene = new()
        {
            shapes = new Shape[] {
                new Sphere { position = new Vector3d(0.0, 0.0, 0.0), radius = 1.0, material = yellow },
                new Sphere { position = new Vector3d(1.4, -0.7, -0.5), radius = 0.6, material = blue },
                new Sphere { position = new Vector3d(-0.7, 0.7, -0.8), radius = 0.1, material = red },
                new Sphere { position = new Vector3d(1.5, 0.6, 0.1), radius = 0.5, material = gold },
                new Plane { point = new Vector3d(0.0, -1.3, 0.0), normal = new Vector3d(0.0, 1.0, 0.0), material = white }
            },

            lights = new Light[] {
                new PointLight(new Vector3d(-10.0, 8.0, -6.0), new Colorf(1.0f, 1.0f, 1.0f)),
                new PointLight(new Vector3d(0.0, 20.0, -3.0), new Colorf(0.3f, 0.3f, 0.3f))
            },

            cam = new Camera(new Vector3d(0.6, 0.0, -5.6), Quaterniond.Identity, Math.PI / 180 * 40)
        };

        Scene testScene = new()
        {
            shapes = new Shape[] {
                new Sphere { position = new Vector3d(0.0, 0.0, 0.0), radius = 1.0, material = yellow },
                new Sphere { position = new Vector3d(1.4, -0.7, -0.5), radius = 0.6, material = blue },
                //new Sphere { position = new Vector3d(-0.7, 0.7, -0.8), radius = 0.1, material = red },
                //new Sphere { position = new Vector3d(1.5, 0.6, 0.1), radius = 0.5, material = gold },
                new Plane { point = new Vector3d(0.0, -1.3, 0.0), normal = new Vector3d(0.0, 1.0, 0.0), material = white }
            },

            lights = new Light[] {
                new PointLight(new Vector3d(-10.0, 8.0, -6.0), new Colorf(1.0f, 1.0f, 1.0f)),
                //new PointLight(new Vector3d(0.0, 20.0, -3.0), new Colorf(0.3f, 0.3f, 0.3f))
            },

            cam = new Camera(new Vector3d(0.6, 0.0, -5.6), Quaterniond.Identity, Math.PI / 180 * 40)
        };

        // HDR image.
        FloatImage fi = new FloatImage(Config.Width, Config.Height, 3);

        ch3scene.Render(fi);

        //fi.SaveHDR(fileName);   // Doesn't work well yet...
        fi.SavePFM(Config.OutputFilename);

        Console.WriteLine("HDR image is finished.");
    }
}

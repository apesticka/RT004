using OpenTK.Mathematics;
using System.Drawing;
using Util;
//using System.Numerics;

namespace rt004;

internal class Program
{
    public static Colorf AmbientIntensity = Config.Instance.Scene.Lights.Ambient.Intensity;
    public static readonly Material DefaultMaterial = new UnlitMaterial { Color = Color.Magenta };

    static void Main(string[] args)
    {
        Config.Load(args[0]);
        //Config.Load("configTest.xml");

        Material yellow = new PhongMaterial { Ambient = 0.1f, Diffuse = 0.8f, Specular = 0.2f, Highlight = 10, Color = new Colorf(1, 1, 0) };
        Material blue = new PhongMaterial { Ambient = 0.1f, Diffuse = 0.5f, Specular = 0.5f, Highlight = 150, Color = new Colorf(0.2f, 0.3f, 1f) };
        Material red = new PhongMaterial { Ambient = 0.1f, Diffuse = 0.6f, Specular = 0.4f, Highlight = 80, Color = new Colorf(0.8f, 0.2f, 0.2f) };
        Material gold = new PhongMaterial { Ambient = 0.2f, Diffuse = 0.2f, Specular = 0.8f, Highlight = 400, Color = new Colorf(0.3f, 0.2f, 0.0f) };
        Material white = new PhongMaterial { Ambient = 0.1f, Diffuse = 0.6f, Specular = 0.4f, Highlight = 80, Color = new Colorf(0.9f, 0.9f, 0.9f) };

        //Scene scene = new()
        //{
        //    shapes = new Shape[] {
        //        new Plane { point = -Vector3d.UnitY, normal = Vector3d.UnitY, Material = new PhongMaterial { Ambient = 0.1f, Diffuse = 0.5f, Specular = 0.5f, Highlight = 150, Color = Color.Peru } },
        //        //new Sphere { position = new Vector3d(-1, 1, 3), radius = 1, color = Color.Turquoise }
        //        new Sphere { Center = new Vector3d(0, 0, 0), Radius = 1, Material = yellow },
        //        new Sphere { Center = new Vector3d(1.4, -0.7, -0.5), Radius = 0.6, Material = blue },
        //        new Sphere { Center = new Vector3d(-0.7, 0.7, -0.8), Radius = 0.1, Material = red }
        //    },

        //    lights = new Light[] {
        //        //new DirectionalLight(new Vector3d(-1, -1, 1), 0.4f * Colorf.WHITE),
        //        new PointLight(new Vector3d(-10.0, 8.0, -1.0), 1),
        //        new PointLight(new Vector3d(0.0, 20.0, -3.0), Color.Red)
        //        //new PointLight(new Vector3d(-3, 1.5, 1.5), Color.MediumVioletRed)
        //    },

        //    cam = new Camera(new Vector3d(0.6, 0.0, -5.6), Quaterniond.Identity, Math.PI / 180 * 40)
        //};

        //Scene ch2scene = new()
        //{
        //    shapes = new Shape[] {
        //        new Sphere { Center = new Vector3d(), Radius = 1.0, Material = yellow },
        //        new Sphere { Center = new Vector3d(1.4, -0.7, -0.5), Radius = 0.6, Material = blue },
        //        new Sphere { Center = new Vector3d(-0.7, 0.7, -0.8), Radius = 0.1, Material = red }
        //    },

        //    lights = new Light[] {
        //        new PointLight(new Vector3d(-10.0, 8.0, -6.0), new Colorf(1.0f, 1.0f, 1.0f)),
        //        new PointLight(new Vector3d(0.0, 20.0, -3.0), new Colorf(0.3f, 0.3f, 0.3f))
        //    },

        //    cam = new Camera(new Vector3d(0.6, 0.0, -5.6), Quaterniond.Identity, Math.PI / 180 * 40)
        //};

        //Scene ch3scene = new()
        //{
        //    shapes = new Shape[] {
        //        new Sphere { Center = new Vector3d(0.0, 0.0, 0.0), Radius = 1.0, Material = yellow },
        //        new Sphere { Center = new Vector3d(1.4, -0.7, -0.5), Radius = 0.6, Material = blue },
        //        new Sphere { Center = new Vector3d(-0.7, 0.7, -0.8), Radius = 0.1, Material = red },
        //        new Sphere { Center = new Vector3d(1.5, 0.6, 0.1), Radius = 0.5, Material = gold },
        //        new Plane { point = new Vector3d(0.0, -1.3, 0.0), normal = new Vector3d(0.0, 1.0, 0.0), Material = white }
        //    },

        //    lights = new Light[] {
        //        new PointLight(new Vector3d(-10.0, 8.0, -6.0), new Colorf(1.0f, 1.0f, 1.0f)),
        //        new PointLight(new Vector3d(0.0, 20.0, -3.0), new Colorf(0.3f, 0.3f, 0.3f))
        //    },

        //    cam = new Camera(new Vector3d(0.6, 0.0, -5.6), Quaterniond.Identity, Math.PI / 180 * 40)
        //};

        //Scene testScene = new()
        //{
        //    shapes = new Shape[] {
        //        new Sphere { Center = new Vector3d(0.0, 0.0, 0.0), Radius = 1.0, Material = yellow },
        //        new Sphere { Center = new Vector3d(1.4, -0.7, -0.5), Radius = 0.6, Material = blue },
        //        //new Sphere { position = new Vector3d(-0.7, 0.7, -0.8), radius = 0.1, material = red },
        //        //new Sphere { position = new Vector3d(1.5, 0.6, 0.1), radius = 0.5, material = gold },
        //        new Plane { point = new Vector3d(0.0, -1.3, 0.0), normal = new Vector3d(0.0, 1.0, 0.0), Material = white }
        //    },

        //    lights = new Light[] {
        //        new PointLight(new Vector3d(-10.0, 8.0, -6.0), new Colorf(1.0f, 1.0f, 1.0f)),
        //        //new PointLight(new Vector3d(0.0, 20.0, -3.0), new Colorf(0.3f, 0.3f, 0.3f))
        //    },

        //    cam = new Camera(new Vector3d(0.6, 0.0, -5.6), Quaterniond.Identity, Math.PI / 180 * 40)
        //};

        // HDR image.
        FloatImage fi = new FloatImage(Config.Instance.Camera.Width, Config.Instance.Camera.Height, 3);

        Scene scene = Config.Instance.CreateScene();
        scene.Render(fi);

        //fi.SaveHDR(fileName);   // Doesn't work well yet...
        fi.SavePFM(Config.Instance.General.OutputFilename);

        Console.WriteLine("HDR image is finished.");

        //System.Diagnostics.Process.Start("demo.pfm");

        new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo("demo.pfm")
            {
                UseShellExecute = true
            }
        }.Start();
    }
}

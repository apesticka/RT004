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

        // HDR image.
        FloatImage fi = new FloatImage(Config.Instance.Camera.Width, Config.Instance.Camera.Height, 3);

        Scene scene = Config.Instance.CreateScene();
        scene.Render(fi);

        //fi.SaveHDR(fileName);   // Doesn't work well yet...
        fi.SavePFM(Config.Instance.General.OutputFilename);

        Console.WriteLine("HDR image is finished.");

        new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo("demo.pfm")
            {
                UseShellExecute = true
            }
        }.Start();
    }
}

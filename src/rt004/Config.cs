using OpenTK.Mathematics;
using System.Numerics;
using System.Xml.Serialization;

namespace rt004
{
    // TODO: shadows, reflections, fov

    [XmlRoot("config")]
    public class Config
    {
        public static Config Instance { get => instance; }
        private static Config instance;

        [XmlElement("general")]
        public GeneralConfig General;
        [XmlElement("camera")]
        public CameraConfig Camera;
        [XmlElement("materials")]
        public MaterialsConfig Materials;
        [XmlElement("scene")]
        public SceneConfig Scene;

        public static void Load(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            using FileStream fs = new FileStream(filename, FileMode.Open);

            instance = (Config)serializer.Deserialize(fs);
            Console.WriteLine("Config loaded:");
            Console.WriteLine($"    Materials: {Instance.Materials.MaterialsList.Count}");
            Console.WriteLine($"    Lights: {Instance.Scene.Lights.Lights.Length} + ambient");
            Console.WriteLine();

            if (Instance.Camera.Direction == Vector3d.Zero) throw new Exception("Camera must have a non-zero direction");
            if (Instance.Camera.Up == Vector3d.Zero) throw new Exception("Camera must have a non-zero up direction");

            instance.Materials.CreateIndex();
            Console.WriteLine("Materials index created");
            Console.WriteLine();
        }

        public Scene CreateScene()
        {
            SceneGraph graph = new SceneGraph(Scene.GraphRoot);

            graph.CalculateInverseMatrices();

            Light[] lights = Scene.Lights.Lights;
            Camera cam = Camera.CreateCamera();

            return new Scene
            {
                Graph = graph,
                Lights = lights,
                Cam = cam
            };
        }

        public static Vector3d VectorFromString(string vectorString)
        {
            if (vectorString == "") return new();
            if (vectorString[0] != '[' || vectorString[^1] != ']') throw new FormatException($"Wrong color string: \"{vectorString}\". Color string must be enclosed in brackets");
            string[] stringComponents = vectorString[1..^1].Split(',');
            float[] floatComponents = new float[3];
            for (int i = 0; i < floatComponents.Length; i++)
                if (stringComponents.Length > i) floatComponents[i] = float.Parse(stringComponents[i].Trim());
            return new Vector3d(floatComponents[0], floatComponents[1], floatComponents[2]);
        }
    }

    public class GeneralConfig
    {
        [XmlAttribute("shadows")]
        public bool Shadows;
        [XmlAttribute("reflections")]
        public bool Reflections;
        [XmlAttribute("maxdepth")]
        public int MaxDepth;
        [XmlAttribute("output")]
        public string OutputFilename;

        public Colorf BackgroundColor;

        [XmlAttribute("background")]
        public string BackgroundConfig { get => BackgroundColor.ToString(); set => BackgroundColor = Colorf.FromString(value); }
    }

    public class CameraConfig
    {
        [XmlAttribute("width")]
        public int Width;
        [XmlAttribute("height")]
        public int Height;
        [XmlAttribute("fov")]
        public double Fov;

        public Vector3d Center;
        public Vector3d Direction;
        public Vector3d Up;

        [XmlAttribute("center")]
        public string CenterConfig { get => Center.ToString(); set => Center = Config.VectorFromString(value); }
        [XmlAttribute("dir")]
        public string DirectionConfig { get => Direction.ToString(); set => Direction = Config.VectorFromString(value); }
        [XmlAttribute("up")]
        public string UpConfig { get => Up.ToString(); set => Up = Config.VectorFromString(value); }

        internal Camera CreateCamera()
        {
            return new Camera(Center, Direction, Up, Fov / 180.0 * Math.PI);
        }
    }

    public class MaterialsConfig
    {
        [XmlElement(typeof(PhongMaterial), ElementName = "phong")]
        [XmlElement(typeof(UnlitMaterial), ElementName = "unlit")]
        public List<Material> MaterialsList;

        [XmlIgnore]
        public Dictionary<string, Material> Materials;

        public void CreateIndex()
        {
            Materials = new Dictionary<string, Material>();
            foreach (Material material in MaterialsList)
            {
                Materials[material.Id] = material;
            }
        }
    }

    public class SceneConfig
    {
        [XmlElement("graph")] public SceneGraph.TransformNode GraphRoot;
        [XmlElement("lights")] public LightsConfig Lights;

        public class LightsConfig
        {
            [XmlElement("ambient")]
            public AmbientLight Ambient;

            [XmlElement(typeof(PointLight), ElementName = "point")]
            [XmlElement(typeof(DirectionalLight), ElementName = "directional")]
            public Light[] Lights;
        }
    }
}

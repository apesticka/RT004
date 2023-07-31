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
            //Console.WriteLine($"    Shapes: {Instance.Scene.Shapes.Shapes.Length}");
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
            Scene.CalculateInverseMatrices();

            ShapeNode[] shapes = Scene.GetShapes();
            Light[] lights = Scene.Lights.Lights;
            Camera cam = Camera.CreateCamera();

            return new Scene
            {
                Graph = Scene.GraphRoot,
                shapes = shapes,
                lights = lights,
                cam = cam
            };
        }

        public struct Vector3
        {
            [XmlAttribute("x")] public double X;
            [XmlAttribute("y")] public double Y;
            [XmlAttribute("z")] public double Z;

            public static implicit operator Vector3d(Vector3 v)
            {
                return new Vector3d(v.X, v.Y, v.Z);
            }

            public override string ToString()
            {
                return ((Vector3d)this).ToString();
            }
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
        [XmlElement("shadows")]
        public bool Shadows;
        [XmlElement("reflections")]
        public bool Reflections;
        [XmlElement("maxdepth")]
        public int MaxDepth;
        [XmlElement("background")]
        public Colorf BackgroundColor;
        [XmlElement("output")]
        public string OutputFilename;
    }

    public class CameraConfig
    {
        [XmlElement("width")]
        public int Width;
        [XmlElement("height")]
        public int Height;
        [XmlElement("center")]
        public Config.Vector3 Center;
        [XmlElement("dir")]
        public Config.Vector3 Direction;
        [XmlElement("up")]
        public Config.Vector3 Up;
        [XmlElement("fov")]
        public double Fov;

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
        [XmlElement("graph")] public TransformNode GraphRoot;
        [XmlElement("lights")] public LightsConfig Lights;
        [XmlElement("transform")] public TransformNode transform;

        public ShapeNode[] GetShapes()
        {
            List<ShapeNode> shapes = new List<ShapeNode>();

            void AddChildren(TransformNode node)
            {
                foreach (var child in node.Children)
                {
                    if (child is TransformNode transform) AddChildren(transform);
                    else if (child is ShapeNode shape) shapes.Add(shape);
                }
            }

            AddChildren(GraphRoot);

            return shapes.ToArray();
        }

        public void CalculateInverseMatrices()
        {
            void ProcessNode(TransformNode node)
            {
                node.InverseMatrix = node.Matrix.Inverted();

                foreach (var child in node.Children)
                {
                    if (child is TransformNode transform) ProcessNode(transform);
                }
            }

            ProcessNode(GraphRoot);
        }

        public class ShapesConfig
        {
            [XmlElement(typeof(Sphere), ElementName = "sphere")]
            [XmlElement(typeof(Plane), ElementName = "plane")]
            public ShapeNode[] Shapes;
        }

        public class LightsConfig
        {
            [XmlElement("ambient")]
            public Light Ambient;

            [XmlElement(typeof(PointLight), ElementName = "point")]
            [XmlElement(typeof(DirectionalLight), ElementName = "directional")]
            public Light[] Lights;
        }

        public abstract class GraphNode
        {
            public abstract RayHit? IntersectRay(Ray ray);
        }

        public class TransformNode : GraphNode
        {
            [XmlIgnore] public Matrix4d Matrix = Matrix4d.Identity;
            [XmlIgnore] public Matrix4d InverseMatrix;

            [XmlElement(typeof(TransformNode), ElementName = "transform")]
            [XmlElement(typeof(Sphere), ElementName = "sphere")]
            [XmlElement(typeof(Plane), ElementName = "plane")]
            public GraphNode[] Children;

            public override RayHit? IntersectRay(Ray ray)
            {
                Vector3d transformedOrigin = new Vector3d(InverseMatrix * new Vector4d(ray.Origin, 1));
                Vector3d transformedDirection = new Vector3d(InverseMatrix * new Vector4d(ray.Direction, 0));
                Ray transformedRay = new Ray { Origin = transformedOrigin, Direction = transformedDirection };

                RayHit? closest = null;
                foreach (var child in Children)
                {
                    RayHit? hit = child.IntersectRay(transformedRay);
                    if (closest == null || (hit != null && hit.Value.Distance < closest.Value.Distance))
                        closest = hit;
                }
                return closest?.Transform(Matrix, InverseMatrix);
            }

            [XmlAttribute("translate")] public string TranslateConfig
            {
                get => "";

                set
                {
                    Vector3d translationVector = Config.VectorFromString(value);
                    Console.WriteLine($"Translate by: {translationVector}");
                    Matrix4d translationMatrix = Matrix4d.CreateTranslation(translationVector);
                    translationMatrix.Transpose();
                    Matrix = translationMatrix * Matrix;
                }
            }

            [XmlAttribute("rotate")] public string RotateConfig
            {
                get => "";

                set
                {
                    Vector3d eulerAngles = Config.VectorFromString(value) * Math.PI / 180.0;
                    Console.WriteLine($"Rotate by: {eulerAngles}");
                    Matrix4d rotationMatrix = Matrix4d.CreateFromQuaternion(Quaterniond.FromEulerAngles(eulerAngles));
                    rotationMatrix.Transpose();
                    Matrix = rotationMatrix * Matrix;
                }
            }

            [XmlAttribute("scale")] public string ScaleConfig
            {
                get => "";

                set
                {
                    Vector3d scalingVector = Config.VectorFromString(value);
                    Console.WriteLine($"Scale by: {scalingVector}");
                    Matrix4d scalingMatrix = Matrix4d.Scale(scalingVector);
                    scalingMatrix.Transpose();
                    Matrix = scalingMatrix * Matrix;
                }
            }
        }
    }
}

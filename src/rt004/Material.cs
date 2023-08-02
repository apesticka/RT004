using OpenTK.Mathematics;
using System.Xml.Serialization;

namespace rt004
{
    public abstract class Material
    {
        [XmlAttribute("id")] public string Id;
        [XmlAttribute("color")] public string ColorString { get => Color.ToString(); set => Color = Colorf.FromString(value); }
        public Colorf Color;

        public abstract Colorf Evaluate(Scene scene, Vector3d point, Vector3d eye, Vector3d normal, Colorf ambient, int depth=0);
    }

    public class UnlitMaterial : Material
    {
        public override Colorf Evaluate(Scene scene, Vector3d point, Vector3d eye, Vector3d normal, Colorf ambient, int depth)
        {
            return Color;
        }
    }

    public class PhongMaterial : Material
    {
        [XmlAttribute("kA")] public float Ambient;
        [XmlAttribute("kD")] public float Diffuse;
        [XmlAttribute("kS")] public float Specular;
        [XmlAttribute("highlight")] public float Highlight;

        private void ProcessLight(ref Colorf color, Light light, Scene scene, Vector3d point, Vector3d eye, Vector3d normal, int depth)
        {
            if (!light.VisibleFrom(point, scene)) return;

            Vector3d lightDir = -light.GetDirection(point);
            Colorf lightIntensity = light.GetIntensity(point);

            Vector3d reflectionDir = 2 * normal * Vector3d.Dot(eye, normal) - eye;
            reflectionDir.Normalize();

            Ray reflectionRay = new Ray { Origin = point + 0.0001 * reflectionDir, Direction = reflectionDir };
            color += Specular * Specular * scene.Evaluate(reflectionRay, depth + 1);

            double dot = Vector3d.Dot(lightDir, normal);
            if (dot <= 0) return;

            Colorf diffuse = Diffuse * Color * lightIntensity * (float)dot;

            double specularDot = Vector3d.Dot(eye, (2 * normal * dot - lightDir).Normalized());
            Colorf specular = Specular * lightIntensity * (float)Math.Pow(specularDot, Highlight);

            color += diffuse;
            if (specularDot >= 0)
                color += specular;
        }

        public override Colorf Evaluate(Scene scene, Vector3d point, Vector3d eye, Vector3d normal, Colorf ambient, int depth)
        {
            eye.Normalize();
            normal.Normalize();

            Colorf color = Colorf.BLACK;
            color += ambient * Ambient;

            foreach (Light light in scene.Lights)
            {
                ProcessLight(ref color, light, scene, point, eye, normal, depth);
            }

            return color.Clamp();
        }
    }
}

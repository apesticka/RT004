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

        public override Colorf Evaluate(Scene scene, Vector3d point, Vector3d eye, Vector3d normal, Colorf ambient, int depth)
        {
            Colorf color = Colorf.BLACK;
            color += ambient * this.Ambient;

            foreach (Light light in scene.lights)
            {
                Vector3d lightDir = -light.GetDirection(point);

                Ray ray = new Ray { Origin = point + 0.00001 * lightDir, Direction = lightDir };
                bool found = false;
                foreach (Shape shape in scene.shapes)
                {
                    RayHit? hit = shape.IntersectRay(ray);
                    
                    if (light is PositionedLight positioned)
                    {
                        if (hit != null && hit.Value.Distance > 0 && hit.Value.Distance < (positioned.Position - point).Length)
                        {
                            found = true;
                            break;
                        }
                    }
                    else
                    {
                        if (hit != null && hit.Value.Distance > 0)
                        {
                            found = true;
                            break;
                        }
                    }
                }
                if (found) continue;
                    

                Colorf lightIntensity = light.GetIntensity(point);
                Vector3d reflectionDir = 2 * normal * Vector3d.Dot(eye, normal) - eye;
                reflectionDir.Normalize();

                Ray reflectionRay = new Ray { Origin = point + 0.00001 * reflectionDir, Direction = reflectionDir };

                color += this.Specular * this.Specular * scene.Evaluate(reflectionRay, depth + 1);

                double dot = Vector3d.Dot(lightDir, normal);
                if (dot <= 0) continue;

                Colorf diffuse = this.Diffuse * this.Color * lightIntensity * (float)dot;

                double specularDot = Vector3d.Dot(eye, 2 * normal * dot - lightDir);
                Colorf specular = this.Specular * lightIntensity * (float)Math.Pow(specularDot, Highlight);

                color += diffuse;
                if (specularDot >= 0)
                    color += specular;
            }

            return color.Clamp();
        }
    }
}

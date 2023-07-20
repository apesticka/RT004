using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    internal abstract class Material
    {
        public abstract Colorf Evaluate(Scene scene, Vector3d point, Vector3d eye, Vector3d normal, Colorf ambient, int depth=0);
    }

    internal class UnlitMaterial : Material
    {
        public Colorf color;

        public override Colorf Evaluate(Scene scene, Vector3d point, Vector3d eye, Vector3d normal, Colorf ambient, int depth)
        {
            return color;
        }
    }

    internal class PhongMaterial : Material
    {
        public float ambient, diffuse, specular;
        public float highlight;
        public Colorf color;

        public override Colorf Evaluate(Scene scene, Vector3d point, Vector3d eye, Vector3d normal, Colorf ambient, int depth)
        {
            Colorf color = Colorf.BLACK;
            color += ambient * this.ambient;

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
                        if (hit != null && hit.Value.Distance > 0 && hit.Value.Distance < (positioned.position - point).Length)
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

                color += this.specular * this.specular * scene.Evaluate(reflectionRay, depth + 1);

                double dot = Vector3d.Dot(lightDir, normal);
                if (dot <= 0) continue;

                Colorf diffuse = this.diffuse * this.color * lightIntensity * (float)dot;

                double specularDot = Vector3d.Dot(eye, 2 * normal * dot - lightDir);
                Colorf specular = this.specular * lightIntensity * (float)Math.Pow(specularDot, highlight);

                color += diffuse;
                if (specularDot >= 0)
                    color += specular;
            }

            return color.Clamp();
        }
    }
}

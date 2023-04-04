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
        public abstract Colorf Evaluate(Light[] lights, Vector3d point, Vector3d eye, Vector3d normal, Colorf ambient, Shape[] scene);
    }

    internal class UnlitMaterial : Material
    {
        public Colorf color;

        public override Colorf Evaluate(Light[] lights, Vector3d point, Vector3d eye, Vector3d normal, Colorf ambient, Shape[] scene)
        {
            return color;
        }
    }

    internal class PhongMaterial : Material
    {
        public float ambient, diffuse, specular;
        public float highlight;
        public Colorf color;

        public override Colorf Evaluate(Light[] lights, Vector3d point, Vector3d eye, Vector3d normal, Colorf ambient, Shape[] scene)
        {
            Colorf color = Colorf.BLACK;
            color += ambient * this.ambient;

            foreach (Light light in lights)
            {
                Vector3d lightDir = -light.GetDirection(point);

                Ray ray = new Ray { Origin = point + 0.00001 * lightDir, Direction = lightDir };
                bool found = false;
                foreach (Shape shape in scene)
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
                double dot = Vector3d.Dot(lightDir, normal);
                Vector3d reflection = 2 * normal * dot - lightDir;
                reflection.Normalize();

                if (dot <= 0) continue;

                Colorf diffuse = this.diffuse * this.color * lightIntensity * (float)dot;

                double specularDot = Math.Pow(Vector3d.Dot(eye, reflection), highlight);
                Colorf specular = this.specular * lightIntensity * (float)specularDot;

                color += diffuse;
                if (specularDot >= 0)
                    color += specular;
            }

            return color.Clamp();
        }
    }
}

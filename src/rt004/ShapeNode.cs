using OpenTK.Mathematics;
using System.Xml.Serialization;

namespace rt004
{
    public abstract class ShapeNode : SceneConfig.GraphNode
    {
        [XmlAttribute("material")]
        public string MaterialString;
        [XmlIgnore]
        public Material Material => Config.Instance.Materials.Materials[MaterialString];

        //public abstract RayHit? IntersectRay(Ray ray);
    }

    public sealed class Sphere : ShapeNode
    {
        [XmlIgnore] public Vector3d Center;
        [XmlAttribute("radius")] public double Radius;

        [XmlAttribute("center")]
        public string CenterConfig { get => Center.ToString(); set => Center = Config.VectorFromString(value); }

        public override RayHit? IntersectRay(Ray ray)
        {
            ray.Direction.Normalize();
            double dx = ray.Origin.X - Center.X;
            double dy = ray.Origin.Y - Center.Y;
            double dz = ray.Origin.Z - Center.Z;

            double a = (
                ray.Direction.X * ray.Direction.X +
                ray.Direction.Y * ray.Direction.Y +
                ray.Direction.Z * ray.Direction.Z
            );

            double b = 2 * (
                dx * ray.Direction.X +
                dy * ray.Direction.Y +
                dz * ray.Direction.Z
            );

            double c = (
                dx * dx +
                dy * dy +
                dz * dz -
                Radius * Radius
            );

            double D = b * b - 4 * a * c;
            if (D < 0) return null;
            double sqrtD = Math.Sqrt(D);
            double t1 = (-b - sqrtD) / (2 * a);
            double t2 = (-b + sqrtD) / (2 * a);

            if (t1 < 0 && t2 < 0) return null;

            double distance = Math.Min(t1, t2);
            if (distance < 0)
            {
                if (distance == t1) distance = t2;
                if (distance == t2) distance = t1;
            }
            Vector3d point = ray.Origin + distance * ray.Direction;
            Vector3d normal = (point - Center).Normalized();

            return new RayHit { Point = point, Normal = normal, Distance = distance, Shape = this };
        }
    }

    public sealed class Plane : ShapeNode
    {
        [XmlIgnore] public Vector3d Point;
        [XmlIgnore] public Vector3d Normal;

        [XmlAttribute("point")]
        public string PointConfig { get => Point.ToString(); set => Point = Config.VectorFromString(value); }
        [XmlAttribute("normal")]
        public string NormalConfig { get => Normal.ToString(); set => Normal = Config.VectorFromString(value); }

        public override RayHit? IntersectRay(Ray ray)
        {
            Vector3d dir = Vector3d.Normalize(ray.Direction);
            Vector3d originOffset = ray.Origin - Point;

            double num = -Vector3d.Dot(Normal, originOffset);
            double denom = Vector3d.Dot(Normal, dir);

            if (Math.Abs(denom) < 0.0001) return null;

            double t = num / denom;
            if (t >= 0)
                return new RayHit { Point = ray.Origin + dir * t, Normal = Normal, Distance = t, Shape = this };
            else
                return null;
        }
    }
}

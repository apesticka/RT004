using OpenTK.Mathematics;

namespace rt004
{
    internal abstract class Shape
    {
        public abstract RayHit? IntersectRay(Ray ray);
    }

    internal sealed class Sphere : Shape
    {
        public Vector3d position;
        public double radius;

        public override RayHit? IntersectRay(Ray ray)
        {
            ray.Direction.Normalize();
            double dx = ray.Origin.X - position.X;
            double dy = ray.Origin.Y - position.Y;
            double dz = ray.Origin.Z - position.Z;

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
                radius * radius
            );

            double D = b * b - 4 * a * c;
            if (D < 0) return null;
            double sqrtD = Math.Sqrt(D);
            double t1 = (-b - sqrtD) / (2 * a);
            double t2 = (-b + sqrtD) / (2 * a);

            double distance = Math.Min(t1, t2);
            Vector3d point = ray.Origin + distance * ray.Direction;
            Vector3d normal = (point - position).Normalized();

            return new RayHit { Point = point, Normal = normal, Distance = distance };
        }
    }

    internal sealed class Plane : Shape
    {
        public Vector3d point;
        public Vector3d normal;

        public override RayHit? IntersectRay(Ray ray)
        {
            ray.Direction.Normalize();
            double D = -Vector3d.Dot(normal, point);
            double num = D + Vector3d.Dot(ray.Origin, normal);
            double denom = Vector3d.Dot(ray.Direction, normal);
            //if (denom < 0.001) return null;

            double t = -num / denom;
            if (t >= 0)
                return new RayHit { Point = ray.Origin + ray.Direction * t, Normal = normal, Distance = t };
            else
                return null;
        }
    }
}

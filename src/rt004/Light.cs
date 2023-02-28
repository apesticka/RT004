using OpenTK.Mathematics;

namespace rt004
{
    internal abstract class Light
    {
        protected double baseIntensity;
        protected Colorf color;

        public Colorf Color => color;

        protected Light(double baseIntensity, Colorf? color = null)
        {
            this.baseIntensity = baseIntensity;
            this.color = color ?? Colorf.WHITE;
        }

        public abstract double GetIntensity(Vector3d worldPoint);

        /// <summary>
        /// Returns a *normalized* direction vector
        /// </summary>
        public abstract Vector3d GetDirection(Vector3d worldPoint);
    }

    internal class DirectionalLight : Light
    {
        Vector3d direction;

        public DirectionalLight(Vector3d direction, double intensity, Colorf? color = null) : base(intensity, color)
        {
            this.direction = direction.Normalized();
        }

        public override double GetIntensity(Vector3d worldPoint)
        {
            return baseIntensity;
        }

        public override Vector3d GetDirection(Vector3d worldPoint)
        {
            return direction;
        }
    }

    internal class PointLight : Light
    {
        Vector3d position;

        public PointLight(Vector3d position, double baseIntensity, Colorf? color = null) : base(baseIntensity, color)
        {
            this.position = position;
        }

        public override Vector3d GetDirection(Vector3d worldPoint)
        {
            return (worldPoint - position).Normalized();
        }

        public override double GetIntensity(Vector3d worldPoint)
        {
            return baseIntensity / (worldPoint - position).LengthSquared;
        }
    }
}

using OpenTK.Mathematics;

namespace rt004
{
    internal abstract class Light
    {
        protected double baseIntensity;
        protected float[] color;

        public float[] Color => color;

        protected Light(double baseIntensity, float[] color = null)
        {
            this.baseIntensity = baseIntensity;
            this.color = color ?? new float[] { 1, 1, 1 };
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

        public DirectionalLight(Vector3d direction, double intensity) : base(intensity)
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
}

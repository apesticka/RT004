using OpenTK.Mathematics;

namespace rt004
{
    internal abstract class Light
    {
        protected Colorf intensity;

        protected Light(Colorf intensity)
        {
            this.intensity = intensity;
        }

        protected Light(float baseIntensity, Colorf baseColor) : this(baseIntensity * baseColor) { }

        protected Light(float intensity) : this(intensity * Colorf.WHITE) { }

        public abstract Colorf GetIntensity(Vector3d worldPoint);

        /// <summary>
        /// Returns a *normalized* direction vector
        /// </summary>
        public abstract Vector3d GetDirection(Vector3d worldPoint);
    }

    internal abstract class PositionedLight : Light
    {
        public Vector3d position;

        protected PositionedLight(Colorf intensity) : base(intensity) { }
        protected PositionedLight(float baseIntensity, Colorf baseColor) : base(baseIntensity, baseColor) { }
        protected PositionedLight(float intensity) : base(intensity) { }
    }

    internal class DirectionalLight : Light
    {
        Vector3d direction;

        public DirectionalLight(Vector3d direction, Colorf intensity) : base(intensity)
        {
            this.direction = direction;
        }

        public DirectionalLight(Vector3d direction, float intensity) : base(intensity)
        {
            this.direction = direction.Normalized();
        }

        public DirectionalLight(Vector3d direction, float baseIntensity, Colorf baseColor) : base(baseIntensity, baseColor)
        {
            this.direction = direction;
        }

        public override Colorf GetIntensity(Vector3d worldPoint)
        {
            return intensity;
        }

        public override Vector3d GetDirection(Vector3d worldPoint)
        {
            return direction;
        }
    }

    internal class PointLight : PositionedLight
    {
        public PointLight(Vector3d position, Colorf intensity) : base(intensity)
        {
            this.position = position;
        }

        public PointLight(Vector3d position, float intensity) : base(intensity)
        {
            this.position = position;
        }

        public PointLight(Vector3d position, float baseIntensity, Colorf baseColor) : base(baseIntensity, baseColor)
        {
            this.position = position;
        }

        public override Vector3d GetDirection(Vector3d worldPoint)
        {
            return (worldPoint - position).Normalized();
        }

        public override Colorf GetIntensity(Vector3d worldPoint)
        {
            return intensity/* / (float)(worldPoint - position).LengthSquared*/;
        }
    }
}

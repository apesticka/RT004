using OpenTK.Mathematics;
using System.Drawing;
using System.Xml.Serialization;

namespace rt004
{
    public class Light
    {
        [XmlIgnore] public Colorf Intensity;

        [XmlAttribute("intensity")]
        public string IntensityConfig { get => Intensity.ToString(); set => Intensity = Colorf.FromString(value); }

        public Light() { }

        public Light(Colorf intensity)
        {
            this.Intensity = intensity;
        }

        public Light(float baseIntensity, Colorf baseColor) : this(baseIntensity * baseColor) { }

        public Light(float intensity) : this(intensity * Colorf.WHITE) { }

        public virtual Colorf GetIntensity(Vector3d worldPoint) => Intensity;

        /// <summary>
        /// Returns a *normalized* direction vector
        /// </summary>
        public virtual Vector3d GetDirection(Vector3d worldPoint) => Vector3d.Zero;

        public virtual bool VisibleFrom(Vector3d worldPoint, Scene scene) => true;
    }

    public abstract class PositionedLight : Light
    {
        [XmlIgnore] public Vector3d Position;

        [XmlAttribute("position")]
        public string PositionConfig { get => Position.ToString(); set => Position = Config.VectorFromString(value); }

        protected PositionedLight() { }
        protected PositionedLight(Colorf intensity) : base(intensity) { }
        protected PositionedLight(float baseIntensity, Colorf baseColor) : base(baseIntensity, baseColor) { }
        protected PositionedLight(float intensity) : base(intensity) { }

        public override bool VisibleFrom(Vector3d worldPoint, Scene scene)
        {
            Vector3d lightDir = -GetDirection(worldPoint);

            Ray ray = new Ray { Origin = worldPoint + 0.00001 * lightDir, Direction = lightDir };

            foreach (ShapeNode shape in scene.shapes)
            {
                RayHit? hit = shape.IntersectRay(ray);

                if (hit != null && hit.Value.Distance > 0 && hit.Value.Distance < Vector3d.Distance(Position, worldPoint))
                    return false;
            }

            return true;
        }
    }

    public class DirectionalLight : Light
    {
        [XmlIgnore] Vector3d Direction;

        [XmlAttribute("direction")]
        public string DirectionConfig { get => Direction.ToString(); set => Direction = Config.VectorFromString(value); }

        public DirectionalLight() { }

        public DirectionalLight(Vector3d direction, Colorf intensity) : base(intensity)
        {
            this.Direction = direction;
        }

        public DirectionalLight(Vector3d direction, float intensity) : base(intensity)
        {
            this.Direction = direction.Normalized();
        }

        public DirectionalLight(Vector3d direction, float baseIntensity, Colorf baseColor) : base(baseIntensity, baseColor)
        {
            this.Direction = direction;
        }

        public override Colorf GetIntensity(Vector3d worldPoint)
        {
            return Intensity;
        }

        public override Vector3d GetDirection(Vector3d worldPoint)
        {
            return Direction;
        }

        public override bool VisibleFrom(Vector3d worldPoint, Scene scene)
        {
            Vector3d lightDir = -GetDirection(worldPoint);

            Ray ray = new Ray { Origin = worldPoint + 0.00001 * lightDir, Direction = lightDir };

            foreach (ShapeNode shape in scene.shapes)
            {
                RayHit? hit = shape.IntersectRay(ray);

                if (hit != null && hit.Value.Distance > 0)
                    return false;
            }

            return true;
        }
    }

    public class PointLight : PositionedLight
    {
        public PointLight() { }

        public PointLight(Vector3d position, Colorf intensity) : base(intensity)
        {
            this.Position = position;
        }

        public PointLight(Vector3d position, float intensity) : base(intensity)
        {
            this.Position = position;
        }

        public PointLight(Vector3d position, float baseIntensity, Colorf baseColor) : base(baseIntensity, baseColor)
        {
            this.Position = position;
        }

        public override Vector3d GetDirection(Vector3d worldPoint)
        {
            return (worldPoint - Position).Normalized();
        }

        public override Colorf GetIntensity(Vector3d worldPoint)
        {
            return Intensity/* / (float)(worldPoint - position).LengthSquared*/;
        }
    }
}

using OpenTK.Mathematics;
using System.Drawing;
using System.Xml.Serialization;

namespace rt004
{
    public abstract class Light
    {
        [XmlIgnore] public Colorf Intensity;

        [XmlAttribute("intensity")]
        public string IntensityConfig { get => Intensity.ToString(); set => Intensity = Colorf.FromString(value); }

        public abstract Colorf GetIntensity(Vector3d worldPoint);

        /// <summary>
        /// Returns a *normalized* direction vector from this light to worldPoint
        /// </summary>
        public abstract Vector3d GetDirection(Vector3d worldPoint);

        /// <summary>
        /// Is this light visible from worldPoint
        /// </summary>
        public abstract bool VisibleFrom(Vector3d worldPoint, Scene scene);
    }

    public abstract class PositionedLight : Light
    {
        [XmlIgnore] public Vector3d Position;

        [XmlAttribute("position")]
        public string PositionConfig { get => Position.ToString(); set => Position = Config.VectorFromString(value); }

        public override bool VisibleFrom(Vector3d worldPoint, Scene scene)
        {
            Vector3d lightDir = -GetDirection(worldPoint);

            Ray ray = new Ray { Origin = worldPoint, Direction = lightDir };

            RayHit? hit = scene.IntersectRay(ray);

            if (hit == null) return true;

            if (Vector3d.DistanceSquared(ray.Origin, hit.Value.Point) < Vector3d.DistanceSquared(ray.Origin, Position)) return false;

            return true;
        }
    }

    public class AmbientLight : Light
    {
        public override Vector3d GetDirection(Vector3d worldPoint) => Vector3d.Zero;

        public override Colorf GetIntensity(Vector3d worldPoint) => Intensity;

        public override bool VisibleFrom(Vector3d worldPoint, Scene scene) => true;
    }

    public class DirectionalLight : Light
    {
        [XmlIgnore] Vector3d Direction;

        [XmlAttribute("direction")]
        public string DirectionConfig { get => Direction.ToString(); set => Direction = Config.VectorFromString(value); }

        public override Colorf GetIntensity(Vector3d worldPoint) => Intensity;

        public override Vector3d GetDirection(Vector3d worldPoint) => Direction;

        public override bool VisibleFrom(Vector3d worldPoint, Scene scene)
        {
            Vector3d lightDir = -GetDirection(worldPoint);

            Ray ray = new Ray { Origin = worldPoint, Direction = lightDir };

            RayHit? hit = scene.IntersectRay(ray);

            return hit == null;
        }
    }

    public class PointLight : PositionedLight
    {
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

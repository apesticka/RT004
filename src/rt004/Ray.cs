using OpenTK.Mathematics;

namespace rt004
{
    public record struct Ray
    {
        public Vector3d Origin { get; init; }
        public Vector3d Direction { get; init; }

        public Ray(Vector3d origin, Vector3d direction)
        {
            Origin = origin;
            Direction = direction;
        }
    }

    public record struct RayHit
    {
        public Vector3d Point { get; init; }
        public Vector3d Normal { get; init; }
        public ShapeNode Shape { get; init; }
    }
}

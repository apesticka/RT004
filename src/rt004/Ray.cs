using OpenTK.Mathematics;

namespace rt004
{
    internal record struct Ray
    {
        public Vector3d Origin { get; init; }
        public Vector3d Direction { get; init; }

        public Ray(Vector3d origin, Vector3d direction)
        {
            Origin = origin;
            Direction = direction;
        }
    }

    internal record struct RayHit
    {
        public Vector3d Point { get; init; }
        public Vector3d Normal { get; init; }
        public double Distance { get; init; }
    }
}

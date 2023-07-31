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
        public double Distance { get; init; }
        public ShapeNode Shape { get; init; }

        public RayHit Transform(Matrix4d direct, Matrix4d inverse) => new RayHit
        {
            Point = new Vector3d(direct * new Vector4d(Point, 1)),
            Normal = new Vector3d(Matrix4d.Transpose(inverse) * new Vector4d(Normal, 0)).Normalized(),
            Shape = Shape
        };
    }
}

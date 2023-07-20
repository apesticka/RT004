using OpenTK.Mathematics;

namespace rt004
{
    public class Camera
    {
        Vector3d position;
        double fov;
        Matrix3d rotationMatrix;
        Vector3d windowSize;

        public Camera(Vector3d position, Vector3d forward, Vector3d up, double fov)
        {
            this.position = position;

            forward = Vector3d.Normalize(forward);
            up = Vector3d.Normalize(up);

            Vector3d right = Vector3d.Cross(up, forward);
            up = Vector3d.Cross(forward, right);

            rotationMatrix = new Matrix3d(
                right.X, up.X, forward.X,
                right.Y, up.Y, forward.Y,
                right.Z, up.Z, forward.Z
            );

            this.fov = fov;
            windowSize = new Vector3d(Math.Sin(fov / 2), Math.Sin(fov / 2) * Config.Instance.Camera.Height / Config.Instance.Camera.Width, Math.Cos(fov / 2));
        }

        public Ray GenerateRay(float x, float y)
        {
            double dx = windowSize.X;
            double dy = windowSize.Y;
            double dz = windowSize.Z;

            Vector3d topLeft = rotationMatrix * new Vector3d(-dx, dy, dz);
            Vector3d topRight = rotationMatrix * new Vector3d(dx, dy, dz);
            Vector3d bottomLeft = rotationMatrix * new Vector3d(-dx, -dy, dz);
            Vector3d bottomRight = rotationMatrix * new Vector3d(dx, -dy, dz);

            Vector3d top = Vector3d.Lerp(topLeft, topRight, x / (double)Config.Instance.Camera.Width);
            Vector3d bottom = Vector3d.Lerp(bottomLeft, bottomRight, x / (double)Config.Instance.Camera.Width);
            Vector3d direction = Vector3d.Lerp(top, bottom, y / (double)Config.Instance.Camera.Height);

            return new Ray(position, direction.Normalized());
        }
    }
}

using OpenTK.Mathematics;

namespace rt004
{
    internal class Camera
    {
        Vector3d position;
        Quaterniond rotation;
        double fov;
        Matrix3d rotationMatrix;
        Vector3d windowSize;

        private static Camera instance = null;
        public static Camera Singleton => instance;

        private Camera(Vector3d position, Quaterniond rotation, double fov)
        {
            this.position = position;
            this.rotation = rotation;
            rotationMatrix = Matrix3d.CreateFromQuaternion(rotation);
            this.fov = fov;
            windowSize = new Vector3d(Math.Sin(fov / 2), Math.Sin(fov / 2) * Config.Height / Config.Width, Math.Cos(fov / 2));
        }

        public static Camera Create(Vector3d position, Quaterniond rotation, double fov)
        {
            if (instance != null)
            {
                Console.WriteLine("Warning: Cannot create a second camera.");
                return instance;
            }

            instance = new Camera(position, rotation, fov);

            return instance;
        }

        public Ray GenerateRay(float x, float y)
        {
            double dx = windowSize.X;
            double dy = windowSize.Y;
            double dz = windowSize.Z;

            Vector3d topLeft = new Vector3d(-dx, dy, dz) * rotationMatrix;
            Vector3d topRight = new Vector3d(dx, dy, dz) * rotationMatrix;
            Vector3d bottomLeft = new Vector3d(-dx, -dy, dz) * rotationMatrix;
            Vector3d bottomRight = new Vector3d(dx, -dy, dz) * rotationMatrix;

            Vector3d top = Vector3d.Lerp(topLeft, topRight, x / (double)Config.Width);
            Vector3d bottom = Vector3d.Lerp(bottomLeft, bottomRight, x / (double)Config.Width);
            Vector3d direction = Vector3d.Lerp(top, bottom, y / (double)Config.Height);

            return new Ray(position, direction);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace rt004
{
    static class MathExtensions
    {
        static Vector3d ApplyMatrix4(Matrix4d m, Vector3d v, double w) => new Vector3d(m * new Vector4d(v, w));

        public static Vector3d ApplyToPoint(this Matrix4d matrix, Vector3d point) => ApplyMatrix4(matrix, point, 1);
        public static Vector3d ApplyToVector(this Matrix4d matrix, Vector3d vector) => ApplyMatrix4(matrix, vector, 0);
    }
}

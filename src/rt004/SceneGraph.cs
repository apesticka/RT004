using OpenTK.Mathematics;
using System;
using System.Xml.Serialization;

namespace rt004
{
    public class SceneGraph
    {
        TransformNode Root;

        public SceneGraph(TransformNode root)
        {
            Root = root;
        }

        public RayHit? IntersectRay(Ray ray)
        {
            return Root.IntersectRay(ray);
        }

        public void CalculateInverseMatrices()
        {
            static void ProcessNode(TransformNode node)
            {
                node.InverseMatrix = node.Matrix.Inverted();

                foreach (var child in node.Children)
                {
                    if (child is TransformNode transform) ProcessNode(transform);
                }
            }

            ProcessNode(Root);
        }

        public abstract class Node
        {
            public abstract RayHit? IntersectRay(Ray ray);
        }

        public class TransformNode : Node
        {
            [XmlIgnore] public Matrix4d Matrix = Matrix4d.Identity;
            [XmlIgnore] public Matrix4d InverseMatrix;

            [XmlElement(typeof(TransformNode), ElementName = "transform")]
            [XmlElement(typeof(Sphere), ElementName = "sphere")]
            [XmlElement(typeof(Plane), ElementName = "plane")]
            public Node[] Children;

            public override RayHit? IntersectRay(Ray ray)
            {
                Vector3d transformedOrigin = InverseMatrix.ApplyToPoint(ray.Origin);
                Vector3d transformedDirection = InverseMatrix.ApplyToVector(ray.Direction);
                Ray transformedRay = new Ray { Origin = transformedOrigin, Direction = transformedDirection };

                RayHit? closest = null;
                foreach (var child in Children)
                {
                    RayHit? hit = child.IntersectRay(transformedRay);
                    if (closest == null || (hit != null && Vector3d.DistanceSquared(transformedOrigin, hit.Value.Point) < Vector3d.DistanceSquared(transformedOrigin, closest.Value.Point)))
                        closest = hit;
                }

                if (closest == null) return null;

                Vector3d transformedPoint = Matrix.ApplyToPoint(closest.Value.Point);
                Vector3d transformedNormal = Matrix4d.Transpose(InverseMatrix).ApplyToVector(closest.Value.Normal);

                return new RayHit { Point = transformedPoint, Normal = transformedNormal, Shape = closest.Value.Shape };
            }

            [XmlAttribute("translate")]
            public string TranslateConfig
            {
                get => "";

                set
                {
                    Vector3d translationVector = Config.VectorFromString(value);
                    Console.WriteLine($"Translate by: {translationVector}");
                    Matrix4d translationMatrix = Matrix4d.CreateTranslation(translationVector);
                    translationMatrix.Transpose();
                    Matrix = translationMatrix * Matrix;
                }
            }

            [XmlAttribute("rotate")]
            public string RotateConfig
            {
                get => "";

                set
                {
                    Vector3d eulerAngles = Config.VectorFromString(value) * Math.PI / 180.0;
                    Console.WriteLine($"Rotate by: {eulerAngles}");
                    Matrix4d rotationMatrix = Matrix4d.CreateFromQuaternion(Quaterniond.FromEulerAngles(eulerAngles));
                    rotationMatrix.Transpose();
                    Matrix = rotationMatrix * Matrix;
                }
            }

            [XmlAttribute("scale")]
            public string ScaleConfig
            {
                get => "";

                set
                {
                    Vector3d scalingVector = Config.VectorFromString(value);
                    Console.WriteLine($"Scale by: {scalingVector}");
                    Matrix4d scalingMatrix = Matrix4d.Scale(scalingVector);
                    scalingMatrix.Transpose();
                    Matrix = scalingMatrix * Matrix;
                }
            }
        }
    }
}

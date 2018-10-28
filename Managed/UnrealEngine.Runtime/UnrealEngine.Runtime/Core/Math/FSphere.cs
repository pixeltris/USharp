using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\Sphere.h

    /// <summary>
    /// Implements a basic sphere.
    /// (NOTE: Not exposed to the reflection system)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FSphere : IEquatable<FSphere>
    {
        /// <summary>
        /// The sphere's center point.
        /// </summary>
        public FVector Center;

        /// <summary>
        /// The sphere's radius.
        /// </summary>
        public float W;

        /// <summary>
        /// Creates and initializes a new sphere with the specified parameters.
        /// </summary>
        /// <param name="v">Center of sphere.</param>
        /// <param name="w">Radius of sphere.</param>
        public FSphere(FVector v, float w)
        {
            Center = v;
            W = w;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pts"Pointer to list of points this sphere must contain.></param>
        public FSphere(FVector[] pts)
        {
            if (pts.Length > 0)
            {
                FBox Box = new FBox(pts);

                this = new FSphere((Box.Min + Box.Max) / 2, 0);

                for (int i = 0; i < pts.Length; i++)
                {
                    float dist = FVector.DistSquared(pts[i], Center);

                    if (dist > W)
                    {
                        W = dist;
                    }
                }
            }
            else
            {
                this = default(FSphere);
            }
        }

        /// <summary>
        /// Check whether two spheres are the same within specified tolerance.
        /// </summary>
        /// <param name="other">The other sphere.</param>
        /// <param name="tolerance">Error Tolerance.</param>
        /// <returns>true if spheres are equal within specified tolerance, otherwise false.</returns>
        public bool Equals(FSphere other, float tolerance = FMath.KindaSmallNumber)
        {
            return Center.Equals(other.Center, tolerance) && FMath.Abs(W - other.W) <= tolerance;
        }

        /// <summary>
        /// Check whether sphere is inside of another.
        /// </summary>
        /// <param name="other">The other sphere.</param>
        /// <param name="tolerance">Error Tolerance.</param>
        /// <returns>true if sphere is inside another, otherwise false.</returns>
        public bool IsInside(FSphere other, float tolerance = FMath.KindaSmallNumber)
        {
            if (W > other.W + tolerance)
            {
                return false;
            }

            return (Center - other.Center).SizeSquared() <= FMath.Square(other.W + tolerance - W);
        }

        /// <summary>
        /// Checks whether the given location is inside this sphere.
        /// </summary>
        /// <param name="v">The location to test for inside the bounding volume.</param>
        /// <param name="tolerance">Error Tolerance.</param>
        /// <returns>true if location is inside this volume.</returns>
        public bool IsInside(FVector v, float tolerance = FMath.KindaSmallNumber)
        {
            return (Center - v).SizeSquared() <= FMath.Square(W + tolerance);
        }

        /// <summary>
        /// Test whether this sphere intersects another.
        /// </summary>
        /// <param name="other">The other sphere.</param>
        /// <param name="tolerance">Error tolerance.</param>
        /// <returns>true if spheres intersect, false otherwise.</returns>
        public bool Intersects(FSphere other, float tolerance = FMath.KindaSmallNumber)
        {
            return (Center - other.Center).SizeSquared() <= FMath.Square(FMath.Max(0.0f, other.W + W + tolerance));
        }

        /// <summary>
        /// Get result of Transforming sphere by Matrix.
        /// </summary>
        /// <param name="m">Matrix to transform by.</param>
        /// <returns>Result of transformation.</returns>
        public FSphere TransformBy(FMatrix m)
        {
            FSphere result;

            result.Center = m.TransformPosition(Center);

            FVector XAxis = new FVector(m[0, 0], m[0, 1], m[0, 2]);
            FVector YAxis = new FVector(m[1, 0], m[1, 1], m[1, 2]);
            FVector ZAxis = new FVector(m[2, 0], m[2, 1], m[2, 2]);

            result.W = FMath.Sqrt(FMath.Max(XAxis | XAxis, FMath.Max(YAxis | YAxis, ZAxis | ZAxis))) * W;

            return result;
        }

        /// <summary>
        /// Get result of Transforming sphere with Transform.
        /// </summary>
        /// <param name="m">Transform information.</param>
        /// <returns>Result of transformation.</returns>
        public FSphere TransformBy(FTransform m)
        {
            FSphere result;

            result.Center = m.TransformPosition(Center);
            result.W = m.GetMaximumAxisScale() * W;

            return result;
        }

        /// <summary>
        /// Get volume of the current sphere
        /// </summary>
        /// <returns>Volume (in Unreal units).</returns>
        public float GetVolume()
        {
            return (4.0f / 3.0f) * FMath.PI * (W * W * W);
        }

        /// <summary>
        /// Gets the result of adding two bounding volumes together.
        /// </summary>
        /// <param name="a">The first bounding volume.</param>
        /// <param name="b">The second bounding volume.</param>
        /// <returns>A new bounding volume.</returns>
        public static FSphere operator +(FSphere a, FSphere b)
        {
            if (a.W == 0.0f)
            {
                a = b;
            }
            else if (a.IsInside(b))
            {
                a = b;
            }
            else if (b.IsInside(a))
            {
                // no change
            }
            else
            {
                FSphere newSphere;

                FVector dirToOther = b.Center - a.Center;
                FVector unitDirToOther = dirToOther;
                unitDirToOther.Normalize();

                float newRadius = (dirToOther.Size() + b.W + a.W) * 0.5f;

                // find end point
                FVector end1 = b.Center + unitDirToOther * b.W;
                FVector end2 = a.Center - unitDirToOther * a.W;
                FVector newCenter = (end1 + end2) * 0.5f;

                newSphere.Center = newCenter;
                newSphere.W = newRadius;

                // make sure both are inside afterwards
                Debug.Assert(b.IsInside(newSphere, 1.0f));
                Debug.Assert(a.IsInside(newSphere, 1.0f));

                a = newSphere;
            }

            return a;
        }

        /// <summary>
        /// Compares two bounding volumes for equality.
        /// </summary>
        /// <param name="a">The first bounding volume.</param>
        /// <param name="b">The second bounding volume.</param>
        /// <returns>true if the bounding volumes are equal, false otherwise.</returns>
        public static bool operator ==(FSphere a, FSphere b)
        {
            return a.Center == b.Center && a.W == b.W;
        }

        /// <summary>
        /// Compare two bounding volumes for inequality.
        /// </summary>
        /// <param name="a">The first bounding volume.</param>
        /// <param name="b">The second bounding volume.</param>
        /// <returns>true if the bounding volumes are not equal, false otherwise.</returns>
        public static bool operator !=(FSphere a, FSphere b)
        {
            return a.Center != b.Center || a.W != b.W;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FSphere))
            {
                return false;
            }

            return Equals((FSphere)obj);
        }

        public bool Equals(FSphere other)
        {
            return Center == other.Center && W == other.W;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Center.GetHashCode() * 397) ^ W.GetHashCode();
            }
        }

        // FMath::SphereAABBIntersection
        // FMath::ComputeBoundingSphereForCone
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\BoxSphereBounds.h

    /// <summary>
    /// Structure for a combined axis aligned bounding box and bounding sphere with the same origin. (28 bytes).
    /// </summary>
    [UStruct(Flags = 0x0000E008), BlueprintType, UMetaPath("/Script/CoreUObject.BoxSphereBounds")]
    [StructLayout(LayoutKind.Sequential)]
    public struct FBoxSphereBounds : IEquatable<FBoxSphereBounds>
    {
        static bool Origin_IsValid;
        static int Origin_Offset;
        /// <summary>
        /// Holds the origin of the bounding box and sphere.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000005), UMetaPath("/Script/CoreUObject.BoxSphereBounds:Origin")]
        public FVector Origin;

        static bool BoxExtent_IsValid;
        static int BoxExtent_Offset;
        /// <summary>
        /// Holds the extent of the bounding box.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000005), UMetaPath("/Script/CoreUObject.BoxSphereBounds:BoxExtent")]
        public FVector BoxExtent;

        static bool SphereRadius_IsValid;
        static int SphereRadius_Offset;
        /// <summary>
        /// Holds the radius of the bounding sphere.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.BoxSphereBounds:SphereRadius")]
        public float SphereRadius;

        static int FBoxSphereBounds_StructSize;

        public FBoxSphereBounds Copy()
        {
            FBoxSphereBounds result = this;
            return result;
        }

        static FBoxSphereBounds()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FBoxSphereBounds)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FBoxSphereBounds));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.BoxSphereBounds");
            FBoxSphereBounds_StructSize = NativeReflection.GetStructSize(classAddress);
            Origin_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Origin");
            Origin_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Origin", Classes.UStructProperty);
            BoxExtent_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "BoxExtent");
            BoxExtent_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "BoxExtent", Classes.UStructProperty);
            SphereRadius_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "SphereRadius");
            SphereRadius_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "SphereRadius", Classes.UFloatProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FBoxSphereBounds));
        }

        /// <summary>
        /// Creates and initializes a new instance from the specified parameters.
        /// </summary>
        /// <param name="origin">origin of the bounding box and sphere.</param>
        /// <param name="boxExtent">half size of box.</param>
        /// <param name="sphereRadius">radius of the sphere.</param>
        public FBoxSphereBounds(FVector origin, FVector boxExtent, float sphereRadius)
        {
            Origin = origin;
            BoxExtent = boxExtent;
            SphereRadius = sphereRadius;

            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Creates and initializes a new instance from the given Box and Sphere.
        /// </summary>
        /// <param name="box">The bounding box.</param>
        /// <param name="sphere">The bounding sphere.</param>
        public FBoxSphereBounds(FBox box, FSphere sphere)
        {
            box.GetCenterAndExtents(out Origin, out BoxExtent);
            SphereRadius = FMath.Min(BoxExtent.Size(), (sphere.Center - Origin).Size() + sphere.W);

            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Creates and initializes a new instance the given Box.
        /// 
        /// The sphere radius is taken from the extent of the box.
        /// </summary>
        /// <param name="box">The bounding box.</param>
        public FBoxSphereBounds(FBox box)
        {
            box.GetCenterAndExtents(out Origin, out BoxExtent);
            SphereRadius = BoxExtent.Size();

            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Creates and initializes a new instance for the given sphere.
        /// </summary>
        public FBoxSphereBounds(FSphere sphere)
        {
            Origin = sphere.Center;
            BoxExtent = new FVector(sphere.W);
            SphereRadius = sphere.W;

            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Creates and initializes a new instance from the given set of points.
        /// 
        /// The sphere radius is taken from the extent of the box.
        /// </summary>
        /// <param name="points">The points to be considered for the bounding box.</param>
        public FBoxSphereBounds(FVector[] points)
        {
            FBox boundingBox = new FBox();

            // find an axis aligned bounding box for the points.
            for (uint pointIndex = 0; pointIndex < points.Length; pointIndex++)
            {
                boundingBox += points[pointIndex];
            }

            boundingBox.GetCenterAndExtents(out Origin, out BoxExtent);

            // using the center of the bounding box as the origin of the sphere, find the radius of the bounding sphere.
            SphereRadius = 0.0f;

            for (uint pointIndex = 0; pointIndex < points.Length; pointIndex++)
            {
                SphereRadius = FMath.Max(SphereRadius, (points[pointIndex] - Origin).Size());
            }

            DiagnosticCheckNaN();
        }

        /// <summary>
        /// Constructs a bounding volume containing both of the given bounding volumes.
        /// </summary>
        /// <param name="a">The first bounding volume.</param>
        /// <param name="b">The second bounding volume.</param>
        /// <returns>The combined bounding volume.</returns>
        public static FBoxSphereBounds operator +(FBoxSphereBounds a, FBoxSphereBounds b)
        {
            FBox boundingBox = new FBox();

            boundingBox += (a.Origin - a.BoxExtent);
            boundingBox += (a.Origin + a.BoxExtent);
            boundingBox += (b.Origin - b.BoxExtent);
            boundingBox += (b.Origin + b.BoxExtent);

            // build a bounding sphere from the bounding box's origin and the radii of A and B.
            FBoxSphereBounds result = new FBoxSphereBounds(boundingBox);

            result.SphereRadius = FMath.Min(result.SphereRadius, 
                FMath.Max((a.Origin - result.Origin).Size() + a.SphereRadius, (b.Origin - result.Origin).Size() + b.SphereRadius));
            result.DiagnosticCheckNaN();

            return result;
        }

        /// <summary>
        /// Compares two bounding volumes for equality.
        /// </summary>
        /// <param name="a">The first bounding volume.</param>
        /// <param name="b">The second bounding volume.</param>
        /// <returns>true if the bounding volumes are equal, false otherwise.</returns>
        public static bool operator ==(FBoxSphereBounds a, FBoxSphereBounds b)
        {
            return a.Origin == b.Origin && a.BoxExtent == b.BoxExtent && a.SphereRadius == b.SphereRadius;
        }

        /// <summary>
        /// Compares two bounding volumes for inequality.
        /// </summary>
        /// <param name="a">The first bounding volume.</param>
        /// <param name="b">The second bounding volume.</param>
        /// <returns>true if the bounding volumes are different, false otherwise.</returns>
        public static bool operator !=(FBoxSphereBounds a, FBoxSphereBounds b)
        {
            return a.Origin != b.Origin || a.BoxExtent != b.BoxExtent || a.SphereRadius != b.SphereRadius;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FBoxSphereBounds))
            {
                return false;
            }

            return Equals((FBoxSphereBounds)obj);
        }

        public bool Equals(FBoxSphereBounds other)
        {
            return Origin == other.Origin && BoxExtent == other.BoxExtent && SphereRadius == other.SphereRadius;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = Origin.GetHashCode();
                hashcode = (hashcode * 397) ^ BoxExtent.GetHashCode();
                hashcode = (hashcode * 397) ^ SphereRadius.GetHashCode();
                return hashcode;
            }
        }

        /// <summary>
        /// Calculates the squared distance from a point to a bounding box
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The distance.</returns>
        public float ComputeSquaredDistanceFromBoxToPoint(FVector point)
        {
            FVector mins = Origin - BoxExtent;
            FVector maxs = Origin + BoxExtent;
            
            return FVector.ComputeSquaredDistanceFromBoxToPoint(mins, maxs, point);
        }

        /// <summary>
        /// Test whether the spheres from two BoxSphereBounds intersect/overlap.
        /// </summary>
        /// <param name="a">First BoxSphereBounds to test.</param>
        /// <param name="b">Second BoxSphereBounds to test.</param>
        /// <param name="tolerance">Error tolerance added to test distance.</param>
        /// <returns>true if spheres intersect, false otherwise.</returns>
        public static bool SpheresIntersect(FBoxSphereBounds a, FBoxSphereBounds b, float tolerance = FMath.KindaSmallNumber)
        {
            return (a.Origin - b.Origin).SizeSquared() <= FMath.Square(FMath.Max(0.0f, a.SphereRadius + b.SphereRadius + tolerance));
        }

        /// <summary>
        /// Test whether the boxes from two BoxSphereBounds intersect/overlap.
        /// </summary>
        /// <param name="a">First BoxSphereBounds to test.</param>
        /// <param name="b">Second BoxSphereBounds to test.</param>
        /// <returns>true if boxes intersect, false otherwise.</returns>
        public static bool BoxesIntersect(FBoxSphereBounds a, FBoxSphereBounds b)
        {
            return a.GetBox().Intersect(b.GetBox());
        }

        /// <summary>
        /// Gets the bounding box.
        /// </summary>
        /// <returns>The bounding box.</returns>
        public FBox GetBox()
        {
            return new FBox(Origin - BoxExtent, Origin + BoxExtent);
        }

        /// <summary>
        /// Gets the extrema for the bounding box.
        /// </summary>
        /// <param name="extrema">1 for positive extrema from the origin, else negative</param>
        /// <returns>The boxes extrema</returns>
        public FVector GetBoxExtrema(uint extrema)
        {
            if (extrema != 0)
            {
                return Origin + BoxExtent;
            }

            return Origin - BoxExtent;
        }

        /// <summary>
        /// Gets the bounding sphere.
        /// </summary>
        /// <returns>The bounding sphere.</returns>
        public FSphere GetSphere()
        {
            return new FSphere(Origin, SphereRadius);
        }

        /// <summary>
        /// Increase the size of the box and sphere by a given size.
        /// </summary>
        /// <param name="expandAmount">The size to increase by.</param>
        /// <returns>A new box with the expanded size.</returns>
        public FBoxSphereBounds ExpandBy(float expandAmount)
        {
            return new FBoxSphereBounds(Origin, BoxExtent + expandAmount, SphereRadius + expandAmount);
        }

        /// <summary>
        /// Gets a bounding volume transformed by a matrix.
        /// </summary>
        /// <param name="m">The matrix.</param>
        /// <returns>The transformed volume.</returns>
        public FBoxSphereBounds TransformBy(FMatrix m)
        {
            FBoxSphereBounds result;

            FVector vecOrigin = Origin;
            FVector vecExtent = BoxExtent;

            FVector m0 = m.GetRow(0);
            FVector m1 = m.GetRow(1);
            FVector m2 = m.GetRow(2);
            FVector m3 = m.GetRow(3);

            FVector newOrigin = FVector.Replicate(vecOrigin, 0) * m0;
            newOrigin += (FVector.Replicate(vecOrigin, 1) * m1);
            newOrigin += (FVector.Replicate(vecOrigin, 2) * m2);
            newOrigin = newOrigin + m3;

            FVector newExtent = (FVector.Replicate(vecExtent, 0) * m0).GetAbs();
            newExtent += (FVector.Replicate(vecExtent, 1) * m1).GetAbs();
            newExtent += (FVector.Replicate(vecExtent, 2) * m2).GetAbs();

            result.BoxExtent = newExtent;
            result.Origin = newOrigin;

            FVector maxRadius = m0 * m0;
            maxRadius += m1 * m1;
            maxRadius += m2 * m2;
            maxRadius = FVector.ComponentMax(FVector.ComponentMax(maxRadius, FVector.Replicate(maxRadius, 1)), FVector.Replicate(maxRadius, 2));
            result.SphereRadius = FMath.Sqrt(maxRadius[0]) * SphereRadius;

            // For non-uniform scaling, computing sphere radius from a box results in a smaller sphere.
            float boxExtentMagnitude = FMath.Sqrt(FVector.DotProduct(newExtent, newExtent));
            result.SphereRadius = FMath.Min(result.SphereRadius, boxExtentMagnitude);

            result.DiagnosticCheckNaN();
            return result;
        }

        /// <summary>
        /// Gets a bounding volume transformed by a FTransform object.
        /// </summary>
        /// <param name="m">The FTransform object.</param>
        /// <returns>The transformed volume.</returns>
        public FBoxSphereBounds TransformBy(FTransform m)
        {
            FMatrix mat = m.ToMatrixWithScale();
            FBoxSphereBounds result = TransformBy(mat);
            return result;
        }

        public override string ToString()
        {
            return "Origin=" + Origin + ", BoxExtent=(" + BoxExtent + "), SphereRadius=(" + SphereRadius + ")";
        }

        /// <summary>
        /// Constructs a bounding volume containing both A and B.
        /// 
        /// This is a legacy version of the function used to compute primitive bounds, to avoid the need to rebuild lighting after the change.
        /// </summary>
        public FBoxSphereBounds Union(FBoxSphereBounds a, FBoxSphereBounds b)
        {
            FBox boundingBox = new FBox();

            boundingBox += (a.Origin - a.BoxExtent);
            boundingBox += (a.Origin + a.BoxExtent);
            boundingBox += (b.Origin - b.BoxExtent);
            boundingBox += (b.Origin + b.BoxExtent);

            // Build a bounding sphere from the bounding box's origin and the radii of A and B.
            FBoxSphereBounds result = new FBoxSphereBounds(boundingBox);

            result.SphereRadius = FMath.Min(result.SphereRadius, 
                FMath.Max((a.Origin - result.Origin).Size() + a.SphereRadius, (b.Origin - result.Origin).Size() + b.SphereRadius));
            result.DiagnosticCheckNaN();

            return result;
        }

        [Conditional("DEBUG")]
        public void DiagnosticCheckNaN()
        {
            if (Origin.ContainsNaN())
            {
                FMath.LogOrEnsureNanError("Origin contains NaN: " + Origin);
                Origin = FVector.ZeroVector;
            }
            if (BoxExtent.ContainsNaN())
            {
                FMath.LogOrEnsureNanError("BoxExtent contains NaN: " + BoxExtent);
                BoxExtent = FVector.ZeroVector;
            }
            if (FMath.IsNaN(SphereRadius) || !FMath.IsFinite(SphereRadius))
            {
                FMath.LogOrEnsureNanError("SphereRadius contains NaN: " + SphereRadius);
                SphereRadius = 0.0f;
            }
        }

        public bool ContainsNaN()
        {
            return Origin.ContainsNaN() || BoxExtent.ContainsNaN() || !FMath.IsFinite(SphereRadius);
        }
    }
}

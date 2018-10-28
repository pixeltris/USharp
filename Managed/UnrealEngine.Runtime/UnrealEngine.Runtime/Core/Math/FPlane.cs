using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\Plane.h

    // NOTE: The native struct uses inheritance (FPlane : FVector)

    /// <summary>
    /// Structure for three dimensional planes.
    /// 
    /// Stores the coeffecients as Xx+Yy+Zz=W.
    /// Note that this is different from many other Plane classes that use Xx+Yy+Zz+W=0.
    /// </summary>
    [UStruct(Flags = 0x0040EC38), BlueprintType, UMetaPath("/Script/CoreUObject.Plane", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FPlane : IEquatable<FPlane>
    {
        static bool X_IsValid;
        static int X_Offset;
        /// <summary>
        /// Vector's X component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector:X")]
        public float X;

        static bool Y_IsValid;
        static int Y_Offset;
        /// <summary>
        /// Vector's Y component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector:Y")]
        public float Y;

        static bool Z_IsValid;
        static int Z_Offset;
        /// <summary>
        /// Vector's Z component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector:Z")]
        public float Z;

        static bool W_IsValid;
        static int W_Offset;
        /// <summary>
        /// The w-component.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Plane:W")]
        public float W;

        static int FPlane_StructSize;

        public FPlane Copy()
        {
            FPlane result = this;
            return result;
        }

        static FPlane()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FPlane)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FPlane));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Plane");
            FPlane_StructSize = NativeReflection.GetStructSize(classAddress);
            X_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "X");
            X_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "X", Classes.UFloatProperty);
            Y_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Y");
            Y_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Y", Classes.UFloatProperty);
            Z_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Z");
            Z_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Z", Classes.UFloatProperty);
            W_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "W");
            W_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "W", Classes.UFloatProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FPlane));
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="v">4D vector to set up plane.</param>
        public FPlane(FVector4 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = v.W;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X-coefficient.</param>
        /// <param name="y">Y-coefficient.</param>
        /// <param name="z">Z-coefficient.</param>
        /// <param name="w">W-coefficient.</param>
        public FPlane(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="normal">Plane Normal Vector.</param>
        /// <param name="w">Plane W-coefficient.</param>
        public FPlane(FVector normal, float w)
        {
            X = normal.X;
            Y = normal.Y;
            Z = normal.Z;
            W = w;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="planeBase">Base point in plane.</param>
        /// <param name="planeNormal">Plane Normal Vector.</param>
        public FPlane(FVector planeBase, FVector planeNormal)
        {
            X = planeNormal.X;
            Y = planeNormal.Y;
            Z = planeNormal.Z;
            W = planeBase | planeNormal;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="a">First point in the plane.</param>
        /// <param name="b">Second point in the plane.</param>
        /// <param name="c">Third point in the plane.</param>
        public FPlane(FVector a, FVector b, FVector c)
        {
            FVector normal = ((b - a) ^ (c - a)).GetSafeNormal();
            X = normal.X;
            Y = normal.Y;
            Z = normal.Z;
            W = a | normal;
        }

        public static implicit operator FVector(FPlane plane)
        {
            return new FVector(plane.X, plane.Y, plane.Z);
        }

        /// <summary>
        /// Calculates distance between plane and a point.
        /// </summary>
        /// <param name="p">The other point.</param>
        /// <returns>>0: point is in front of the plane, &lt;0: behind, =0: on the plane.</returns>
        public float PlaneDot(FVector p)
        {
            return X * p.X + Y * p.Y + Z * p.Z - W;
        }

        /// <summary>
        /// Normalize this plane in-place if it is larger than a given tolerance. Leaves it unchanged if not.
        /// </summary>
        /// <param name="tolerance">Minimum squared length of vector for normalization.</param>
        /// <returns>true if the plane was normalized correctly, false otherwise.</returns>
        public bool Normalize(float tolerance = FMath.SmallNumber)
        {
            float squareSum = X * X + Y * Y + Z * Z;
            if (squareSum > tolerance)
            {
                float scale = FMath.InvSqrt(squareSum);
                X *= scale; Y *= scale; Z *= scale; W *= scale;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get a flipped version of the plane.
        /// </summary>
        /// <returns>A flipped version of the plane.</returns>
        public FPlane Flip()
        {
            return new FPlane(-X, -Y, -Z, -W);
        }

        /// <summary>
        /// Get the result of transforming the plane by a Matrix.
        /// </summary>
        /// <param name="m">The matrix to transform plane with.</param>
        /// <returns>The result of transform.</returns>
        public FPlane TransformBy(FMatrix m)
        {
            FMatrix tmpTA = m.TransposeAdjoint();
            float detM = m.Determinant();
            return TransformByUsingAdjointT(m, detM, tmpTA);
        }

        /// <summary>
        /// You can optionally pass in the matrices transpose-adjoint, which save it recalculating it.
        /// MSM: If we are going to save the transpose-adjoint we should also save the more expensive
        /// determinant.
        /// </summary>
        /// <param name="m">The Matrix to transform plane with.</param>
        /// <param name="detM">Determinant of Matrix.</param>
        /// <param name="ta">Transpose-adjoint of Matrix.</param>
        /// <returns>The result of transform.</returns>
        public FPlane TransformByUsingAdjointT(FMatrix m, float detM, FMatrix ta)
        {
            FVector newNorm = ta.TransformVector(this).GetSafeNormal();

            if (detM < 0.0f)
            {
                newNorm *= -1.0f;
            }

            return new FPlane(m.TransformPosition(this * W), newNorm);
        }

        /// <summary>
        /// Compares two planes for equality.
        /// </summary>
        /// <param name="a">The first plane.</param>
        /// <param name="b">The second plane.</param>
        /// <returns>true if the planes are equal, false otherwise.</returns>
        public static bool operator ==(FPlane a, FPlane b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        }

        /// <summary>
        /// Compare two planes for inequality.
        /// </summary>
        /// <param name="a">The first plane.</param>
        /// <param name="b">The second plane.</param>
        /// <returns>true if the planes are not equal, false otherwise.</returns>
        public static bool operator !=(FPlane a, FPlane b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FPlane))
            {
                return false;
            }

            return Equals((FPlane)obj);
        }

        public bool Equals(FPlane other)
        {
            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = X.GetHashCode();
                hashcode = (hashcode * 397) ^ Y.GetHashCode();
                hashcode = (hashcode * 397) ^ Z.GetHashCode();
                hashcode = (hashcode * 397) ^ W.GetHashCode();
                return hashcode;
            }
        }

        /// <summary>
        /// Checks whether two planes are equal within specified tolerance.
        /// </summary>
        /// <param name="other">The other plane.</param>
        /// <param name="tolerance">Error Tolerance.</param>
        /// <returns>true if the two planes are equal within specified tolerance, otherwise false.</returns>
        public bool Equals(FPlane other, float tolerance = FMath.KindaSmallNumber)
        {
            return
                (FMath.Abs(X - other.X) < tolerance) &&
                (FMath.Abs(Y - other.Y) < tolerance) &&
                (FMath.Abs(Z - other.Z) < tolerance) &&
                (FMath.Abs(W - other.W) < tolerance);
        }

        /// <summary>
        /// Calculates dot product of two planes.
        /// </summary>
        /// <param name="a">The first plane.</param>
        /// <param name="b">The second plane.</param>
        /// <returns>The dot product.</returns>
        public static float operator |(FPlane a, FPlane b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        }

        /// <summary>
        /// Calculates cross product of two planes.
        /// </summary>
        /// <param name="a">The first plane.</param>
        /// <param name="b">The second plane.</param>
        /// <returns>The cross product.</returns>
        public static FVector operator ^(FPlane a, FPlane b)
        {
            return ((FVector)a) ^ ((FVector)b);
        }

        /// <summary>
        /// Gets result of adding two planes.
        /// </summary>
        /// <param name="a">The first plane.</param>
        /// <param name="b">The second plane.</param>
        /// <returns>The result of the addition.</returns>
        public static FPlane operator +(FPlane a, FPlane b)
        {
            return new FPlane(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        /// <summary>
        /// Gets result of subtracting two planes.
        /// </summary>
        /// <param name="a">The first plane.</param>
        /// <param name="b">The second plane.</param>
        /// <returns>The result of the subtraction.</returns>
        public static FPlane operator -(FPlane a, FPlane b)
        {
            return new FPlane(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        /// <summary>
        /// Gets the result of dividing the given plane.
        /// </summary>
        /// <param name="plane">The plane.</param>
        /// <param name="scale">What to divide the plane by.</param>
        /// <returns>The result of the devision.</returns>
        public static FPlane operator /(FPlane plane, int scale)
        {
            float factor = 1.0f / scale;
            return new FPlane(plane.X * factor, plane.Y * factor, plane.Z * factor, plane.W * factor);
        }

        /// <summary>
        /// Gets the result of scaling the given plane.
        /// </summary>
        /// <param name="scale">The scaling factor.</param>
        /// <param name="plane">The plane.</param>        
        /// <returns>The result of scaling.</returns>
        public static FPlane operator *(float scale, FPlane plane)
        {
            return plane * scale;
        }

        /// <summary>
        /// Gets the result of scaling the given plane.
        /// </summary>
        /// <param name="plane">The plane.</param>
        /// <param name="scale">The scaling factor.</param>
        /// <returns>The result of scaling.</returns>
        public static FPlane operator *(FPlane plane, float scale)
        {
            return new FPlane(plane.X * scale, plane.Y * scale, plane.Z * scale, plane.W * scale);
        }

        /// <summary>
        /// Gets the result of multiplying two planes.
        /// </summary>
        /// <param name="a">The first plane.</param>
        /// <param name="b">The second plane.</param>
        /// <returns>The result of scaling.</returns>
        public static FPlane operator *(FPlane a, FPlane b)
        {
            return new FPlane(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
        }

        // FMath::RayPlaneIntersection
        // FMath::LinePlaneIntersection
        // FMath::IntersectPlanes3
        // FMath::IntersectPlanes2
        // FVector::MirrorByPlane
        // FVector::PointPlaneProject
        // FVector::PointPlaneProject
    }
}

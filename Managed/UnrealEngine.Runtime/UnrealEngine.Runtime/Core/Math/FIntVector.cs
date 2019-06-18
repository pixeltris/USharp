using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\IntVector.h

    /// <summary>
    /// Structure for integer vectors in 3-d space.
    /// </summary>
    [UStruct(Flags = 0x0000E838), BlueprintType, UMetaPath("/Script/CoreUObject.IntVector")]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FIntVector : IEquatable<FIntVector>
    {
        static bool X_IsValid;
        static int X_Offset;
        /// <summary>
        /// Holds the point's x-coordinate.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.IntVector:X")]
        public int X;

        static bool Y_IsValid;
        static int Y_Offset;
        /// <summary>
        /// Holds the point's y-coordinate.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.IntVector:Y")]
        public int Y;

        static bool Z_IsValid;
        static int Z_Offset;
        /// <summary>
        /// Holds the point's z-coordinate.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.IntVector:Z")]
        public int Z;

        static int FIntVector_StructSize;

        public FIntVector Copy()
        {
            FIntVector result = this;
            return result;
        }

        static FIntVector()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FIntVector)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FIntVector));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.IntVector");
            FIntVector_StructSize = NativeReflection.GetStructSize(classAddress);
            X_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "X");
            X_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "X", Classes.UIntProperty);
            Y_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Y");
            Y_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Y", Classes.UIntProperty);
            Z_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Z");
            Z_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Z", Classes.UIntProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FIntVector));
        }

        /// <summary>
        /// An int point with zeroed values.
        /// </summary>
        public static readonly FIntVector ZeroValue = new FIntVector(0, 0, 0);

        /// <summary>
        /// An int point with INDEX_NONE values.
        /// </summary>
        public static readonly FIntVector NoneValue = new FIntVector(-1, -1, -1);

        /// <summary>
        /// Creates and initializes a new instance with the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="z">The z-coordinate.</param>
        public FIntVector(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">replicated to all components</param>
        public FIntVector(int value)
        {
            X = Y = Z = value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vector">float vector converted to int</param>
        public FIntVector(FVector vector)
        {
            X = FMath.TruncToInt(vector.X);
            Y = FMath.TruncToInt(vector.Y);
            Z = FMath.TruncToInt(vector.Z);
        }

        /// <summary>
        /// Gets specific component of the point.
        /// </summary>
        /// <param name="index">the index of point component</param>
        /// <returns>Copy of the component.</returns>
        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    default:
                        throw new IndexOutOfRangeException("Invalid FIntVector index (" + index + ")");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FIntVector index (" + index + ")");
                }
            }
        }

        /// <summary>
        /// Compare two points for equality.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if the points are equal, false otherwise.</returns>
        public static bool operator ==(FIntVector a, FIntVector b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        /// <summary>
        /// Compare two points for inequality.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if the points are not equal, false otherwise.</returns>
        public static bool operator !=(FIntVector a, FIntVector b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FIntVector))
            {
                return false;
            }

            return Equals((FIntVector)obj);
        }

        public bool Equals(FIntVector other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = X.GetHashCode();
                hashcode = (hashcode * 397) ^ Y.GetHashCode();
                hashcode = (hashcode * 397) ^ Z.GetHashCode();
                return hashcode;
            }
        }

        /// <summary>
        /// Gets the result of scaling the point (multiplying each component by a value).
        /// </summary>
        /// <param name="v">The point.</param>
        /// <param name="scale">How much to scale the point by.</param>
        /// <returns>The result of scaling the point.</returns>
        public static FIntVector operator *(int scale, FIntVector v)
        {
            Multiply(ref v, scale, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of scaling the point (multiplying each component by a value).
        /// </summary>
        /// <param name="v">The point.</param>
        /// <param name="scale">How much to scale the point by.</param>
        /// <returns>The result of scaling the point.</returns>
        public static FIntVector operator *(FIntVector v, int scale)
        {
            Multiply(ref v, scale, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of scaling the point (multiplying each component by a value).
        /// </summary>
        /// <param name="v">The point.</param>
        /// <param name="scale">How much to scale the point by.</param>
        /// <returns>The result of scaling the point.</returns>
        public static FIntVector Multiply(FIntVector v, int scale)
        {
            Multiply(ref v, scale, out v);
            return v;
        }

        /// <summary>
        /// Gets the result of scaling the point (multiplying each component by a value).
        /// </summary>
        /// <param name="v">The point.</param>
        /// <param name="scale">How much to scale the point by.</param>
        /// <param name="result">The result of scaling the point.</param>
        public static void Multiply(ref FIntVector v, int scale, out FIntVector result)
        {
            result.X = v.X * scale;
            result.Y = v.Y * scale;
            result.Z = v.Z * scale;
        }

        /// <summary>
        /// Divide this point by a scalar.
        /// </summary>
        /// <param name="v">The point.</param>
        /// <param name="divisor">What to divide the point by.</param>
        /// <returns>The result of division.</returns>
        public static FIntVector operator /(FIntVector v, int divisor)
        {
            Divide(ref v, divisor, out v);
            return v;
        }

        /// <summary>
        /// Divide this point by a scalar.
        /// </summary>
        /// <param name="v">The point.</param>
        /// <param name="divisor">What to divide the point by.</param>
        /// <returns>The result of division.</returns>
        public static FIntVector Divide(FIntVector v, int divisor)
        {
            Divide(ref v, divisor, out v);
            return v;
        }

        /// <summary>
        /// Divide this point by a scalar.
        /// </summary>
        /// <param name="v">The point.</param>
        /// <param name="divisor">What to divide the point by.</param>
        /// <param name="result">The result of division.</param>
        public static void Divide(ref FIntVector v, int divisor, out FIntVector result)
        {
            result.X = v.X / divisor;
            result.Y = v.Y / divisor;
            result.Z = v.Z / divisor;
        }

        /// <summary>
        /// Gets the result of component-wise addition of two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The result of point addition.</returns>
        public static FIntVector operator +(FIntVector a, FIntVector b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of component-wise addition of two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The result of point addition.</returns>
        public static FIntVector Add(FIntVector a, FIntVector b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of component-wise addition of two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="result">The result of point addition.</param>
        public static void Add(ref FIntVector a, ref FIntVector b, out FIntVector result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
            result.Z = a.Z + b.Z;
        }

        /// <summary>
        /// Gets the result of component-wise subtraction of two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The result of point subtraction.</returns>
        public static FIntVector operator -(FIntVector a, FIntVector b)
        {
            Subtract(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of component-wise subtraction of two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The result of point subtraction.</returns>
        public static FIntVector Subtract(FIntVector a, FIntVector b)
        {
            Subtract(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of component-wise subtraction of two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="result">The result of point subtraction.</param>
        public static void Subtract(ref FIntVector a, ref FIntVector b, out FIntVector result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
            result.Z = a.Z - b.Z;
        }

        /// <summary>
        /// Is vector equal to zero.
        /// </summary>
        /// <returns>is zero</returns>
        public bool IsZero()
        {
            return this == ZeroValue;
        }

        /// <summary>
        /// Gets the maximum value in the point.
        /// </summary>
        /// <returns>The maximum value in the point.</returns>
        public float GetMax()
        {
            return FMath.Max(FMath.Max(X, Y), Z);
        }

        /// <summary>
        /// Gets the minimum value in the point.
        /// </summary>
        /// <returns>The minimum value in the point.</returns>
        public float GetMin()
        {
            return FMath.Min(FMath.Min(X, Y), Z);
        }

        /// <summary>
        /// Gets the distance of this point from (0,0,0).
        /// </summary>
        /// <returns>The distance of this point from (0,0,0).</returns>
        public int Size()
        {
            long x64 = (long)X;
            long y64 = (long)Y;
            long z64 = (long)Z;
            return (int)(FMath.Sqrt((float)(x64 * x64 + y64 * y64 + z64 * z64)));
        }

        public override string ToString()
        {
            return "X=" + X + " Y=" + Y + " Z=" + Z;
        }

        /// <summary>
        /// Divide an int point and round up the result.
        /// </summary>
        /// <param name="lhs">The int point being divided.</param>
        /// <param name="divisor">What to divide the int point by.</param>
        /// <returns>A new divided int point.</returns>
        public static FIntVector DivideAndRoundUp(FIntVector lhs, int divisor)
        {
            return new FIntVector(
                FMath.DivideAndRoundUp(lhs.X, divisor), 
                FMath.DivideAndRoundUp(lhs.Y, divisor), 
                FMath.DivideAndRoundUp(lhs.Z, divisor));
        }

        /// <summary>
        /// Gets the number of components a point has.
        /// </summary>
        /// <returns>Number of components point has.</returns>
        public static int Num()
        {
            return 3;
        }
    }
}

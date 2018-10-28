using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\IntVector.h

    /// <summary>
    /// Structure for a 4D integer vector.
    /// (NOTE: Not exposed to the unreal reflection system)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FIntVector4 : IEquatable<FIntVector4>
    {
        /// <summary>
        /// Holds the point's x-coordinate.
        /// </summary>
        public int X;

        /// <summary>
        /// Holds the point's y-coordinate.
        /// </summary>
        public int Y;

        /// <summary>
        /// Holds the point's z-coordinate.
        /// </summary>
        public int Z;

        /// <summary>
        /// Holds the point's w-coordinate.
        /// </summary>
        public int W;

        /// <summary>
        /// An int point with zeroed values.
        /// </summary>
        public static readonly FIntVector4 ZeroValue = new FIntVector4(0, 0, 0, 0);

        /// <summary>
        /// An int point with INDEX_NONE values.
        /// </summary>
        public static readonly FIntVector4 NoneValue = new FIntVector4(-1, -1, -1, -1);

        /// <summary>
        /// Creates and initializes a new instance with the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="z">The z-coordinate.</param>
        /// <param name="w">The w-coordinate.</param>
        public FIntVector4(int x, int y, int z, int w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">replicated to all components</param>
        public FIntVector4(int value)
        {
            X = Y = Z = W = value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vector">float vector converted to int</param>
        public FIntVector4(FVector4 vector)
        {
            X = FMath.TruncToInt(vector.X);
            Y = FMath.TruncToInt(vector.Y);
            Z = FMath.TruncToInt(vector.Z);
            W = FMath.TruncToInt(vector.W);
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
                    case 3: return W;
                    default:
                        throw new IndexOutOfRangeException("Invalid FIntVector4 index (" + index + ")");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    case 3: W = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FIntVector4 index (" + index + ")");
                }
            }
        }

        /// <summary>
        /// Compare two points for equality.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if the points are equal, false otherwise.</returns>
        public static bool operator ==(FIntVector4 a, FIntVector4 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        }

        /// <summary>
        /// Compare two points for inequality.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if the points are not equal, false otherwise.</returns>
        public static bool operator !=(FIntVector4 a, FIntVector4 b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FIntVector4))
            {
                return false;
            }

            return Equals((FIntVector4)obj);
        }

        public bool Equals(FIntVector4 other)
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
        /// Gets the result of scaling the point (multiplying each component by a value).
        /// </summary>
        /// <param name="v">The point.</param>
        /// <param name="scale">How much to scale the point by.</param>
        /// <returns>The result of scaling the point.</returns>
        public static FIntVector4 operator *(int scale, FIntVector4 v)
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
        public static FIntVector4 operator *(FIntVector4 v, int scale)
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
        public static FIntVector4 Multiply(FIntVector4 v, int scale)
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
        public static void Multiply(ref FIntVector4 v, int scale, out FIntVector4 result)
        {
            result.X = v.X * scale;
            result.Y = v.Y * scale;
            result.Z = v.Z * scale;
            result.W = v.W * scale;
        }

        /// <summary>
        /// Divide this point by a scalar.
        /// </summary>
        /// <param name="v">The point.</param>
        /// <param name="divisor">What to divide the point by.</param>
        /// <returns>The result of division.</returns>
        public static FIntVector4 operator /(FIntVector4 v, int divisor)
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
        public static FIntVector4 Divide(FIntVector4 v, int divisor)
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
        public static void Divide(ref FIntVector4 v, int divisor, out FIntVector4 result)
        {
            result.X = v.X / divisor;
            result.Y = v.Y / divisor;
            result.Z = v.Z / divisor;
            result.W = v.W / divisor;
        }

        /// <summary>
        /// Gets the result of component-wise addition of two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The result of point addition.</returns>
        public static FIntVector4 operator +(FIntVector4 a, FIntVector4 b)
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
        public static FIntVector4 Add(FIntVector4 a, FIntVector4 b)
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
        public static void Add(ref FIntVector4 a, ref FIntVector4 b, out FIntVector4 result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
            result.Z = a.Z + b.Z;
            result.W = a.W + b.W;
        }

        /// <summary>
        /// Gets the result of component-wise subtraction of two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The result of point subtraction.</returns>
        public static FIntVector4 operator -(FIntVector4 a, FIntVector4 b)
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
        public static FIntVector4 Subtract(FIntVector4 a, FIntVector4 b)
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
        public static void Subtract(ref FIntVector4 a, ref FIntVector4 b, out FIntVector4 result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
            result.Z = a.Z - b.Z;
            result.W = a.W - b.W;
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
            return FMath.Max(FMath.Max(FMath.Max(X, Y), Z), W);
        }

        /// <summary>
        /// Gets the minimum value in the point.
        /// </summary>
        /// <returns>The minimum value in the point.</returns>
        public float GetMin()
        {
            return FMath.Min(FMath.Min(FMath.Min(X, Y), Z), W);
        }

        /// <summary>
        /// Gets the distance of this point from (0,0,0,0).
        /// </summary>
        /// <returns>The distance of this point from (0,0,0,0).</returns>
        public int Size()
        {
            long x64 = (long)X;
            long y64 = (long)Y;
            long z64 = (long)Z;
            long w64 = (long)W;
            return (int)(FMath.Sqrt((float)(x64 * x64 + y64 * y64 + z64 * z64 + w64 * w64)));
        }

        public override string ToString()
        {
            return "X=" + X + " Y=" + Y + " Z=" + Z + " W=" + W;
        }

        /// <summary>
        /// Divide an int point and round up the result.
        /// </summary>
        /// <param name="lhs">The int point being divided.</param>
        /// <param name="divisor">What to divide the int point by.</param>
        /// <returns>A new divided int point.</returns>
        public static FIntVector4 DivideAndRoundUp(FIntVector4 lhs, int divisor)
        {
            return new FIntVector4(
                FMath.DivideAndRoundUp(lhs.X, divisor),
                FMath.DivideAndRoundUp(lhs.Y, divisor),
                FMath.DivideAndRoundUp(lhs.Z, divisor),
                FMath.DivideAndRoundUp(lhs.W, divisor));
        }

        /// <summary>
        /// Gets the number of components a point has.
        /// </summary>
        /// <returns>Number of components point has.</returns>
        public static int Num()
        {
            return 4;
        }
    }
}

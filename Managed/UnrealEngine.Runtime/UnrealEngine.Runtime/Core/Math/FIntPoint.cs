using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\IntPoint.h

    /// <summary>
    /// Structure for integer points in 2-d space.
    /// </summary>
    [UStruct(Flags = 0x0000E838), BlueprintType, UMetaPath("/Script/CoreUObject.IntPoint")]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FIntPoint : IEquatable<FIntPoint>
    {
        static bool X_IsValid;
        static int X_Offset;
        /// <summary>
        /// Holds the point's x-coordinate.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.IntPoint:X")]
        public int X;

        static bool Y_IsValid;
        static int Y_Offset;
        /// <summary>
        /// Holds the point's y-coordinate.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.IntPoint:Y")]
        public int Y;

        static int FIntPoint_StructSize;

        public FIntPoint Copy()
        {
            FIntPoint result = this;
            return result;
        }

        static FIntPoint()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FIntPoint)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FIntPoint));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.IntPoint");
            FIntPoint_StructSize = NativeReflection.GetStructSize(classAddress);
            X_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "X");
            X_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "X", Classes.UIntProperty);
            Y_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Y");
            Y_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Y", Classes.UIntProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FIntPoint));
        }

        /// <summary>
        /// An integer point with zeroed values (0,0).
        /// </summary>
        public static readonly FIntPoint ZeroValue = new FIntPoint(0, 0);

        /// <summary>
        /// An integer point with INDEX_NONE values (-1,-1).
        /// </summary>
        public static readonly FIntPoint NoneValue = new FIntPoint(-1, -1);

        /// <summary>
        /// Create and initialize a new instance with the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        public FIntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Compare two points for equality.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if the points are equal, false otherwise.</returns>
        public static bool operator ==(FIntPoint a, FIntPoint b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        /// <summary>
        /// Compare two points for inequality.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if the points are not equal, false otherwise.</returns>
        public static bool operator !=(FIntPoint a, FIntPoint b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FIntPoint))
            {
                return false;
            }

            return Equals((FIntPoint)obj);
        }

        public bool Equals(FIntPoint other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        /// <summary>
        /// Gets the result of scaling the point (multiplying each component by a value).
        /// </summary>
        /// <param name="v">The point.</param>
        /// <param name="scale">How much to scale the point by.</param>
        /// <returns>The result of scaling the point.</returns>
        public static FIntPoint operator *(int scale, FIntPoint v)
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
        public static FIntPoint operator *(FIntPoint v, int scale)
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
        public static FIntPoint Multiply(FIntPoint v, int scale)
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
        public static void Multiply(ref FIntPoint v, int scale, out FIntPoint result)
        {
            result.X = v.X * scale;
            result.Y = v.Y * scale;
        }

        /// <summary>
        /// Divide this point by a scalar.
        /// </summary>
        /// <param name="v">The point.</param>
        /// <param name="divisor">What to divide the point by.</param>
        /// <returns>The result of division.</returns>
        public static FIntPoint operator /(FIntPoint v, int divisor)
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
        public static FIntPoint Divide(FIntPoint v, int divisor)
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
        public static void Divide(ref FIntPoint v, int divisor, out FIntPoint result)
        {
            result.X = v.X / divisor;
            result.Y = v.Y / divisor;
        }

        /// <summary>
        /// Gets the result of component-wise addition of two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The result of point addition.</returns>
        public static FIntPoint operator +(FIntPoint a, FIntPoint b)
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
        public static FIntPoint Add(FIntPoint a, FIntPoint b)
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
        public static void Add(ref FIntPoint a, ref FIntPoint b, out FIntPoint result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
        }

        /// <summary>
        /// Gets the result of component-wise subtraction of two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The result of point subtraction.</returns>
        public static FIntPoint operator -(FIntPoint a, FIntPoint b)
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
        public static FIntPoint Subtract(FIntPoint a, FIntPoint b)
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
        public static void Subtract(ref FIntPoint a, ref FIntPoint b, out FIntPoint result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
        }

        /// <summary>
        /// Gets the result of component-wise division of two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The result of division.</returns>
        public static FIntPoint operator /(FIntPoint a, FIntPoint b)
        {
            Divide(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of component-wise division of two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The result of division.</returns>
        public static FIntPoint Divide(FIntPoint a, FIntPoint b)
        {
            Divide(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of component-wise division of two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="result">The result of division.</param>
        public static void Divide(ref FIntPoint a, ref FIntPoint b, out FIntPoint result)
        {
            result.X = a.X / b.X;
            result.Y = a.Y / b.Y;
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
                    default:
                        throw new IndexOutOfRangeException("Invalid FIntPoint index (" + index + ")");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FIntPoint index (" + index + ")");
                }
            }
        }

        /// <summary>
        /// Get the component-wise min of two points.
        /// </summary>
        /// <see cref="ComponentMax"/>
        /// <see cref="GetMax"/>
        public FIntPoint ComponentMin(FIntPoint other)
        {
            return new FIntPoint(FMath.Min(X, other.X), FMath.Min(Y, other.Y));
        }

        /// <summary>
        /// Get the component-wise max of two points.
        /// </summary>
        /// <see cref="ComponentMax"/>
        /// <see cref="GetMax"/>
        public FIntPoint ComponentMax(FIntPoint other)
        {
            return new FIntPoint(FMath.Max(X, other.X), FMath.Max(Y, other.Y));
        }

        /// <summary>
        /// Get the larger of the point's two components.
        /// </summary>
        /// <returns>The maximum component of the point.</returns>
        /// <see cref="GetMin"/>
        /// <see cref="Size"/>
        /// <see cref="SizeSquared"/>
        public int GetMax()
        {
            return FMath.Max(X, Y);
        }

        /// <summary>
        /// Get the smaller of the point's two components.
        /// </summary>
        /// <returns>The minimum component of the point.</returns>
        /// <see cref="GetMax"/>
        /// <see cref="Size"/>
        /// <see cref="SizeSquared"/>
        public int GetMin()
        {
            return FMath.Min(X, Y);
        }

        /// <summary>
        /// Get the distance of this point from (0,0).
        /// </summary>
        /// <returns>The distance of this point from (0,0).</returns>
        /// <see cref="GetMax"/>
        /// <see cref="GetMin"/>
        /// <see cref="SizeSquared"/>
        public int Size()
        {
            long X64 = (long)X;
            long Y64 = (long)Y;
            return (int)FMath.Sqrt((float)(X64 * X64 + Y64 * Y64));
        }

        /// <summary>
        /// Get the squared distance of this point from (0,0).
        /// </summary>
        /// <returns>The squared distance of this point from (0,0).</returns>
        /// <see cref="GetMax"/>
        /// <see cref="GetMin"/>
        /// <see cref="Size"/>
        public int SizeSquared()
        {
            return X * X + Y * Y;
        }

        public override string ToString()
        {
            return "X=" + X + " Y=" + Y;
        }

        /// <summary>
        /// Divide an int point and round up the result.
        /// </summary>
        /// <param name="lhs">The int point being divided.</param>
        /// <param name="divisor">What to divide the int point by.</param>
        /// <returns>A new divided int point.</returns>
        /// <see cref="DivideAndRoundDown"/>
        public static FIntPoint DivideAndRoundUp(FIntPoint lhs, int divisor)
        {
            return new FIntPoint(FMath.DivideAndRoundUp(lhs.X, divisor), FMath.DivideAndRoundUp(lhs.Y, divisor));
        }

        /// <summary>
        /// Divide an int point and round up the result.
        /// </summary>
        /// <param name="lhs">The int point being divided.</param>
        /// <param name="divisor">What to divide the int point by.</param>
        /// <returns>A new divided int point.</returns>
        /// <see cref="DivideAndRoundDown"/>
        public static FIntPoint DivideAndRoundUp(FIntPoint lhs, FIntPoint divisor)
        {
            return new FIntPoint(FMath.DivideAndRoundUp(lhs.X, divisor.X), FMath.DivideAndRoundUp(lhs.Y, divisor.Y));
        }

        /// <summary>
        /// Divide an int point and round down the result.
        /// </summary>
        /// <param name="lhs">The int point being divided.</param>
        /// <param name="divisor">What to divide the int point by.</param>
        /// <returns>A new divided int point.</returns>
        /// <see cref="DivideAndRoundUp(FIntPoint, int)"/>
        public static FIntPoint DivideAndRoundDown(FIntPoint lhs, int divisor)
        {
            return new FIntPoint(FMath.DivideAndRoundDown(lhs.X, divisor), FMath.DivideAndRoundDown(lhs.Y, divisor));
        }

        /// <summary>
        /// Get number of components point has.
        /// </summary>
        /// <returns>number of components point has.</returns>
        public static int Num()
        {
            return 2;
        }
    }
}

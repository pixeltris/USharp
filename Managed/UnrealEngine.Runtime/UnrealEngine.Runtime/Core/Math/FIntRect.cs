using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\IntRect.h

    /// <summary>
    /// Structure for integer rectangles in 2-d space.
    /// (NOTE: Not exposed to the unreal reflection system)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FIntRect : IEquatable<FIntRect>
    {
        /// <summary>
        /// Holds the first pixel line/row (like in Win32 RECT).
        /// </summary>
        public FIntPoint Min;

        /// <summary>
        /// Holds the last pixel line/row (like in Win32 RECT).
        /// </summary>
        public FIntPoint Max;

        public FIntRect(int x0, int y0, int x1, int y1)
        {
            Min.X = x0;
            Min.Y = y0;
            Max.X = x1;
            Max.Y = y1;
        }

        public FIntRect(FIntPoint min, FIntPoint max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Gets a specific point in this rectangle.
        /// </summary>
        /// <param name="index">Index of Point in rectangle.</param>
        /// <returns>Copy of the point.</returns>
        public FIntPoint this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Min;
                    case 1: return Max;
                    default:
                        throw new IndexOutOfRangeException("Invalid FIntRect index (" + index + ")");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: Min = value; break;
                    case 1: Max = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FIntRect index (" + index + ")");
                }
            }
        }

        /// <summary>
        /// Compares two rectangles for equality.
        /// </summary>
        /// <param name="a">The first rectangle.</param>
        /// <param name="b">The second rectangle.</param>
        /// <returns>true if the rectangles are equal, false otherwise.</returns>
        public static bool operator ==(FIntRect a, FIntRect b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        /// <summary>
        /// Compare two rectangles for inequality.
        /// </summary>
        /// <param name="a">The first rectangle.</param>
        /// <param name="b">The second rectangle.</param>
        /// <returns>true if the rectangles are not equal, false otherwise.</returns>
        public static bool operator !=(FIntRect a, FIntRect b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FIntRect))
            {
                return false;
            }

            return Equals((FIntRect)obj);
        }

        public bool Equals(FIntRect other)
        {
            return Min == other.Min && Max == other.Max;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Min.GetHashCode() * 397) ^ Max.GetHashCode();
            }
        }

        /// <summary>
        /// Gets the result of scaling the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="scale">How much to scale the rectangle by.</param>
        /// <returns>The result of scaling the rectangle.</returns>
        public static FIntRect operator *(int scale, FIntRect rect)
        {
            Multiply(ref rect, scale, out rect);
            return rect;
        }

        /// <summary>
        /// Gets the result of scaling the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="scale">How much to scale the rectangle by.</param>
        /// <returns>The result of scaling the rectangle.</returns>
        public static FIntRect operator *(FIntRect rect, int scale)
        {
            Multiply(ref rect, scale, out rect);
            return rect;
        }

        /// <summary>
        /// Gets the result of scaling the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="scale">How much to scale the rectangle by.</param>
        /// <returns>The result of scaling the rectangle.</returns>
        public static FIntRect Multiply(FIntRect rect, int scale)
        {
            Multiply(ref rect, scale, out rect);
            return rect;
        }

        /// <summary>
        /// Gets the result of scaling the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="scale">How much to scale the rectangle by.</param>
        /// <param name="result">The result of scaling the rectangle.</param>
        public static void Multiply(ref FIntRect rect, int scale, out FIntRect result)
        {
            result.Min = rect.Min * scale;
            result.Max = rect.Max * scale;
        }

        /// <summary>
        /// Gets the result of division on the given rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="scale">What to divide the rectangle by.</param>
        /// <returns>The result of the devision.</returns>
        public static FIntRect operator /(FIntRect rect, int scale)
        {
            Divide(ref rect, scale, out rect);
            return rect;
        }

        /// <summary>
        /// Gets the result of division on the given rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="scale">What to divide the rectangle by.</param>
        /// <returns>The result of the devision.</returns>
        public static FIntRect Divide(FIntRect rect, int scale)
        {
            Divide(ref rect, scale, out rect);
            return rect;
        }

        /// <summary>
        /// Gets the result of division on the given rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="scale">What to divide the rectangle by.</param>
        /// <param name="result">The result of the devision.</param>
        public static void Divide(ref FIntRect rect, int scale, out FIntRect result)
        {
            result.Min = rect.Min / scale;
            result.Max = rect.Max / scale;
        }

        /// <summary>
        /// Adds a point to the given rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="point">The point to add onto both points in the rectangle.</param>
        /// <returns>The result of the rectangle after addition.</returns>
        public static FIntRect operator +(FIntRect rect, FIntPoint point)
        {
            Add(ref rect, ref point, out rect);
            return rect;
        }

        /// <summary>
        /// Adds a point to the given rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="point">The point to add onto both points in the rectangle.</param>
        /// <returns>The result of the rectangle after addition.</returns>
        public static FIntRect Add(FIntRect rect, FIntPoint point)
        {
            Add(ref rect, ref point, out rect);
            return rect;
        }

        /// <summary>
        /// Adds a point to the given rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="point">The point to add onto both points in the rectangle.</param>
        /// <param name="result">The result of the rectangle after addition.</param>
        public static void Add(ref FIntRect rect, ref FIntPoint point, out FIntRect result)
        {
            result.Min = rect.Min + point;
            result.Max = rect.Max + point;
        }

        /// <summary>
        /// Subtracts a point from the given rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="point">The point to subtract from both points in the rectangle.</param>
        /// <returns>The result of the rectangle after subtraction.</returns>
        public static FIntRect operator -(FIntRect rect, FIntPoint point)
        {
            Subtract(ref rect, ref point, out rect);
            return rect;
        }

        /// <summary>
        /// Subtracts a point from the given rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="point">The point to subtract from both points in the rectangle.</param>
        /// <returns>The result of the rectangle after subtraction.</returns>
        public static FIntRect Subtract(FIntRect rect, FIntPoint point)
        {
            Subtract(ref rect, ref point, out rect);
            return rect;
        }

        /// <summary>
        /// Subtracts a point from the given rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="point">The point to subtract from both points in the rectangle.</param>
        /// <param name="result">The result of the rectangle after subtraction.</param>
        public static void Subtract(ref FIntRect rect, ref FIntPoint point, out FIntRect result)
        {
            result.Min = rect.Min - point;
            result.Max = rect.Max - point;
        }

        /// <summary>
        /// Gets the result of adding two rectangles together.
        /// </summary>
        /// <param name="a">The first rectangle.</param>
        /// <param name="b">The second rectangle.</param>
        /// <returns>New rectangle after both are added together.</returns>
        public static FIntRect operator +(FIntRect a, FIntRect b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of adding two rectangles together.
        /// </summary>
        /// <param name="a">The first rectangle.</param>
        /// <param name="b">The second rectangle.</param>
        /// <returns>New rectangle after both are added together.</returns>
        public static FIntRect Add(FIntRect a, FIntRect b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of adding two rectangles together.
        /// </summary>
        /// <param name="a">The first rectangle.</param>
        /// <param name="b">The second rectangle.</param>
        /// <param name="result">New rectangle after both are added together.</param>
        public static void Add(ref FIntRect a, ref FIntRect b, out FIntRect result)
        {
            result.Min = a.Min + b.Min;
            result.Max = a.Max + b.Max;
        }

        /// <summary>
        /// Gets the result of subtracting two rectangles.
        /// </summary>
        /// <param name="a">The first rectangle.</param>
        /// <param name="b">The second rectangle.</param>
        /// <returns>New rectangle after the subtraction.</returns>
        public static FIntRect operator -(FIntRect a, FIntRect b)
        {
            Subtract(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of subtracting two rectangles.
        /// </summary>
        /// <param name="a">The first rectangle.</param>
        /// <param name="b">The second rectangle.</param>
        /// <returns>New rectangle after the subtraction.</returns>
        public static FIntRect Subtract(FIntRect a, FIntRect b)
        {
            Subtract(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Gets the result of subtracting two rectangles.
        /// </summary>
        /// <param name="a">The first rectangle.</param>
        /// <param name="b">The second rectangle.</param>
        /// <param name="result">New rectangle after the subtraction.</param>
        public static void Subtract(ref FIntRect a, ref FIntRect b, out FIntRect result)
        {
            result.Min = a.Min - b.Min;
            result.Max = a.Max - b.Max;
        }

        /// <summary>
        /// Calculates the area of this rectangle.
        /// </summary>
        /// <returns>The area of this rectangle.</returns>
        public int Area()
        {
            return (Max.X - Min.X) * (Max.Y - Min.Y);
        }

        /// <summary>
        /// Creates a rectangle from the bottom part of this rectangle.
        /// </summary>
        /// <param name="height">Height of the new rectangle (&lt;= rectangles original height).</param>
        /// <returns>The new rectangle.</returns>
        public FIntRect Bottom(int height)
        {
            return new FIntRect(Min.X, FMath.Max(Min.Y, Max.Y - height), Max.X, Max.Y);
        }

        /// <summary>
        /// Clip a rectangle using the bounds of another rectangle.
        /// </summary>
        /// <param name="other">The other rectangle to clip against.</param>
        public void Clip(FIntRect other)
        {
            Min.X = FMath.Max(Min.X, other.Min.X);
            Min.Y = FMath.Max(Min.Y, other.Min.Y);
            Max.X = FMath.Min(Max.X, other.Max.X);
            Max.Y = FMath.Min(Max.Y, other.Max.Y);

            // return zero area if not overlapping
            Max.X = FMath.Max(Min.X, Max.X);
            Max.Y = FMath.Max(Min.Y, Max.Y);
        }

        /// <summary>
        /// Combines the two rectanges.
        /// </summary>
        public void Union(FIntRect other)
        {
            Min.X = FMath.Min(Min.X, other.Min.X);
            Min.Y = FMath.Min(Min.Y, other.Min.Y);
            Max.X = FMath.Max(Max.X, other.Max.X);
            Max.Y = FMath.Max(Max.Y, other.Max.Y);
        }

        /// <summary>
        /// Test whether this rectangle contains a point.
        /// </summary>
        /// <param name="point">The point to test against.</param>
        /// <returns>true if the rectangle contains the specified point,, false otherwise..</returns>
        public bool Contains(FIntPoint point)
        {
            return point.X >= Min.X && point.X < Max.X && point.Y >= Min.Y && point.Y < Max.Y;
        }

        /// <summary>
        /// Gets the Center and Extents of this rectangle.
        /// </summary>
        /// <param name="center">Will contain the center point.</param>
        /// <param name="extent">Will contain the extent.</param>
        public void GetCenterAndExtents(out FIntPoint center, out FIntPoint extent)
        {
            extent.X = (Max.X - Min.X) / 2;
            extent.Y = (Max.Y - Min.Y) / 2;

            center.X = Min.X + extent.X;
            center.Y = Min.Y + extent.Y;
        }

        /// <summary>
        /// Gets the Height of the rectangle.
        /// </summary>
        /// <returns>The Height of the rectangle.</returns>
        public int Height()
        {
            return (Max.Y - Min.Y);
        }

        /// <summary>
        /// Inflates or deflates the rectangle.
        /// </summary>
        /// <param name="amount">The amount to inflate or deflate the rectangle on each side.</param>
        public void InflateRect(int amount)
        {
            Min.X -= amount;
            Min.Y -= amount;
            Max.X += amount;
            Max.Y += amount;
        }

        /// <summary>
        /// Adds to this rectangle to include a given point.
        /// </summary>
        /// <param name="point">The point to increase the rectangle to.</param>
        public void Include(FIntPoint point)
        {
            Min.X = FMath.Min(Min.X, point.X);
            Min.Y = FMath.Min(Min.Y, point.Y);
            Max.X = FMath.Max(Max.X, point.X);
            Max.Y = FMath.Max(Max.Y, point.Y);
        }

        /// <summary>
        /// Gets a new rectangle from the inner of this one.
        /// </summary>
        /// <param name="shrink">How much to remove from each point of this rectangle.</param>
        /// <returns>New inner Rectangle.</returns>
        public FIntRect Inner(FIntPoint shrink)
        {
            return new FIntRect(Min + shrink, Max - shrink);
        }

        /// <summary>
        /// Creates a rectangle from the right hand side of this rectangle.
        /// </summary>
        /// <param name="width">Width of the new rectangle (&lt;= rectangles original width).</param>
        /// <returns>The new rectangle.</returns>
        public FIntRect Right(int width)
        {
            return new FIntRect(FMath.Max(Min.X, Max.X - width), Min.Y, Max.X, Max.Y);
        }

        /// <summary>
        /// Scales a rectangle using a floating point number.
        /// </summary>
        /// <param name="fraction">What to scale the rectangle by</param>
        /// <returns>New scaled rectangle.</returns>
        public FIntRect Scale(float fraction)
        {
            FVector2D min2D = new FVector2D(Min.X, Min.Y) * fraction;
            FVector2D max2D = new FVector2D(Max.X, Max.Y) * fraction;

            return new FIntRect(
                FMath.FloorToInt(min2D.X), FMath.FloorToInt(min2D.Y), 
                FMath.CeilToInt(max2D.X), FMath.CeilToInt(max2D.Y));
        }

        /// <summary>
        /// Gets the distance from one corner of the rectangle to the other.
        /// </summary>
        /// <returns>The distance from one corner of the rectangle to the other.</returns>
        public FIntPoint Size()
        {
            return new FIntPoint(Max.X - Min.X, Max.Y - Min.Y);
        }

        public override string ToString()
        {
            return "Min=(" + Min + ") Max=(" + Max + ")";
        }

        /// <summary>
        /// Gets the width of the rectangle.
        /// </summary>
        /// <returns>The width of the rectangle.</returns>
        public int Width()
        {
            return Max.X - Min.X;
        }

        /// <summary>
        /// Returns true if the rectangle is 0 x 0.
        /// </summary>
        /// <returns>true if the rectangle is 0 x 0.</returns>
        public bool IsEmpty()
        {
            return Width() == 0 && Height() == 0;
        }

        /// <summary>
        /// Divides a rectangle and rounds up to the nearest integer.
        /// </summary>
        /// <param name="lhs">The Rectangle to divide.</param>
        /// <param name="div">What to divide by.</param>
        /// <returns>New divided rectangle.</returns>
        public static FIntRect DivideAndRoundUp(FIntRect lhs, int div)
        {
            return DivideAndRoundUp(lhs, new FIntPoint(div, div));
        }

        /// <summary>
        /// Divides a rectangle and rounds up to the nearest integer.
        /// </summary>
        /// <param name="lhs">The Rectangle to divide.</param>
        /// <param name="div">What to divide by.</param>
        /// <returns>New divided rectangle.</returns>
        public static FIntRect DivideAndRoundUp(FIntRect lhs, FIntPoint div)
        {
            return new FIntRect(lhs.Min / div, FIntPoint.DivideAndRoundUp(lhs.Max, div));
        }

        /// <summary>
        /// Gets number of points in the Rectangle.
        /// </summary>
        /// <returns>Number of points in the Rectangle.</returns>
        public static int Num()
        {
            return 2;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\Interval.h
    // If updating a XXXXInterval class remember to generate code to update the others

    /// <summary>An int32 interval</summary>
    [UStruct(Flags = 0x0000E008), UMetaPath("/Script/CoreUObject.Int32Interval")]
    [StructLayout(LayoutKind.Sequential)]
    public struct FInt32Interval : IEquatable<FInt32Interval>
    {
        static bool Min_IsValid;
        static int Min_Offset;
        /// <summary>
        /// Holds the lower bound of the interval.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001040000201), UMetaPath("/Script/CoreUObject.Int32Interval:Min")]
        public int Min;

        static bool Max_IsValid;
        static int Max_Offset;
        /// <summary>
        /// Holds the upper bound of the interval.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001040000201), UMetaPath("/Script/CoreUObject.Int32Interval:Max")]
        public int Max;

        static int FInt32Interval_StructSize;

        public FInt32Interval Copy()
        {
            FInt32Interval result = this;
            return result;
        }

        static FInt32Interval()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FInt32Interval)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FInt32Interval));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Int32Interval");
            FInt32Interval_StructSize = NativeReflection.GetStructSize(classAddress);
            Min_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Min");
            Min_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Min", Classes.UIntProperty);
            Max_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Max");
            Max_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Max", Classes.UIntProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FInt32Interval));
        }

        public static readonly FInt32Interval Default = new FInt32Interval(int.MaxValue, int.MinValue);

        /// <summary>
        /// Creates and initializes a new interval with the specified lower and upper bounds.
        /// </summary>
        /// <param name="min">The lower bound of the constructed interval.</param>
        /// <param name="max">The upper bound of the constructed interval.</param>
        public FInt32Interval(int min, int max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Offset the interval by adding the given amount.
        /// </summary>
        /// <param name="a">The interval.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The result of the addition.</returns>
        public static FInt32Interval operator +(FInt32Interval a, int offset)
        {
            if (a.IsValid())
            {
                a.Min += offset;
                a.Max += offset;
            }
            return a;
        }

        /// <summary>
        /// Offset the interval by subtracting the given amount.
        /// </summary>
        /// <param name="a">The interval.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The result of the subtraction.</returns>
        public static FInt32Interval operator -(FInt32Interval a, int offset)
        {
            if (a.IsValid())
            {
                a.Min -= offset;
                a.Max -= offset;
            }
            return a;
        }

        /// <summary>
        /// Compares two intervals for equality.
        /// </summary>
        /// <param name="a">The first interval.</param>
        /// <param name="b">The second interval.</param>
        /// <returns>true if the intervals are equal, false otherwise.</returns>
        public static bool operator ==(FInt32Interval a, FInt32Interval b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        /// <summary>
        /// Compare two intervals for inequality.
        /// </summary>
        /// <param name="a">The first interval.</param>
        /// <param name="b">The second interval.</param>
        /// <returns>true if the intervals are not equal, false otherwise.</returns>
        public static bool operator !=(FInt32Interval a, FInt32Interval b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FInt32Interval))
            {
                return false;
            }

            return Equals((FInt32Interval)obj);
        }

        public bool Equals(FInt32Interval other)
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
        /// Computes the size of this interval.
        /// </summary>
        /// <returns>Interval size.</returns>
        public int Size()
        {
            return (Max - Min);
        }

        /// <summary>
        /// Whether interval is valid (Min &lt;= Max).
        /// </summary>
        /// <returns>false when interval is invalid, true otherwise</returns>
        public bool IsValid()
        {
            return (Min <= Max);
        }

        /// <summary>
        /// Checks whether this interval contains the specified element.
        /// </summary>
        /// <param name="element">The element to check.</param>
        /// <returns>true if the range interval the element, false otherwise.</returns>
        public bool Contains(int element)
        {
            return IsValid() && (element >= Min && element <= Max);
        }

        /// <summary>
        /// Expands this interval to both sides by the specified amount.
        /// </summary>
        /// <param name="expandAmount">The amount to expand by.</param>
        public void Expand(int expandAmount)
        {
            if (IsValid())
            {
                Min -= expandAmount;
                Max += expandAmount;
            }
        }

        /// <summary>
        /// Expands this interval if necessary to include the specified element.
        /// </summary>
        /// <param name="x">The element to include.</param>
        public void Include(int x)
        {
            if (!IsValid())
            {
                Min = x;
                Max = x;
            }
            else
            {
                if (x < Min)
                {
                    Min = x;
                }

                if (x > Max)
                {
                    Max = x;
                }
            }
        }

        /// <summary>
        /// Interval interpolation
        /// </summary>
        /// <param name="alpha">interpolation amount</param>
        /// <returns>interpolation result</returns>
        public int Interpolate(float alpha)
        {
            if (IsValid())
            {
                return Min + (int)(alpha * Size());
            }

            return 0;
        }

        /// <summary>
        /// Calculates the intersection of two intervals.
        /// </summary>
        /// <param name="a">The first interval.</param>
        /// <param name="b">The second interval.</param>
        /// <returns>The intersection.</returns>
        public FInt32Interval Intersect(FInt32Interval a, FInt32Interval b)
        {
            if (a.IsValid() && b.IsValid())
            {
                return new FInt32Interval(FMath.Max(a.Min, b.Min), FMath.Min(a.Max, b.Max));
            }

            return FInt32Interval.Default;
        }
    }
}

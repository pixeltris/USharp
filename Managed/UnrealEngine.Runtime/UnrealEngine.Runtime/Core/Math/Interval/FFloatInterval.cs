using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\Interval.h
    // If updating a XXXXInterval class remember to generate code to update the others

    /// <summary>A float interval</summary>
    [UStruct(Flags = 0x0000E008), UMetaPath("/Script/CoreUObject.FloatInterval")]
    [StructLayout(LayoutKind.Sequential)]
    public struct FFloatInterval : IEquatable<FFloatInterval>
    {
        static bool Min_IsValid;
        static int Min_Offset;
        /// <summary>
        /// Holds the lower bound of the interval.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001040000201), UMetaPath("/Script/CoreUObject.FloatInterval:Min")]
        public float Min;

        static bool Max_IsValid;
        static int Max_Offset;
        /// <summary>
        /// Holds the upper bound of the interval.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001040000201), UMetaPath("/Script/CoreUObject.FloatInterval:Max")]
        public float Max;

        static int FFloatInterval_StructSize;

        public FFloatInterval Copy()
        {
            FFloatInterval result = this;
            return result;
        }

        static FFloatInterval()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FFloatInterval)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FFloatInterval));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.FloatInterval");
            FFloatInterval_StructSize = NativeReflection.GetStructSize(classAddress);
            Min_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Min");
            Min_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Min", Classes.UFloatProperty);
            Max_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Max");
            Max_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Max", Classes.UFloatProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FFloatInterval));
        }

        public static readonly FFloatInterval Default = new FFloatInterval(float.MaxValue, float.MinValue);

        /// <summary>
        /// Creates and initializes a new interval with the specified lower and upper bounds.
        /// </summary>
        /// <param name="min">The lower bound of the constructed interval.</param>
        /// <param name="max">The upper bound of the constructed interval.</param>
        public FFloatInterval(float min, float max)
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
        public static FFloatInterval operator +(FFloatInterval a, float offset)
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
        public static FFloatInterval operator -(FFloatInterval a, float offset)
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
        public static bool operator ==(FFloatInterval a, FFloatInterval b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        /// <summary>
        /// Compare two intervals for inequality.
        /// </summary>
        /// <param name="a">The first interval.</param>
        /// <param name="b">The second interval.</param>
        /// <returns>true if the intervals are not equal, false otherwise.</returns>
        public static bool operator !=(FFloatInterval a, FFloatInterval b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FFloatInterval))
            {
                return false;
            }

            return Equals((FFloatInterval)obj);
        }

        public bool Equals(FFloatInterval other)
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
        public float Size()
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
        public bool Contains(float element)
        {
            return IsValid() && (element >= Min && element <= Max);
        }

        /// <summary>
        /// Expands this interval to both sides by the specified amount.
        /// </summary>
        /// <param name="expandAmount">The amount to expand by.</param>
        public void Expand(float expandAmount)
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
        public void Include(float x)
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
        public float Interpolate(float alpha)
        {
            if (IsValid())
            {
                return Min + (float)(alpha * Size());
            }

            return 0;
        }

        /// <summary>
        /// Calculates the intersection of two intervals.
        /// </summary>
        /// <param name="a">The first interval.</param>
        /// <param name="b">The second interval.</param>
        /// <returns>The intersection.</returns>
        public FFloatInterval Intersect(FFloatInterval a, FFloatInterval b)
        {
            if (a.IsValid() && b.IsValid())
            {
                return new FFloatInterval(FMath.Max(a.Min, b.Min), FMath.Min(a.Max, b.Max));
            }

            return FFloatInterval.Default;
        }
    }
}

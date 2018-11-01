using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\Range.h
    // If updating a XXXXRange class remember to generate code to update the others

    /// <summary>A float range</summary>
    [UStruct(Flags = 0x00000008), BlueprintType, UMetaPath("/Script/CoreUObject.FloatRange", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FFloatRange : IEquatable<FFloatRange>
    {
        static bool LowerBound_IsValid;
        static int LowerBound_Offset;
        [UProperty(Flags = (PropFlags)0x0018000000000005), UMetaPath("/Script/CoreUObject.FloatRange:LowerBound")]
        public FFloatRangeBound LowerBound;

        static bool UpperBound_IsValid;
        static int UpperBound_Offset;
        [UProperty(Flags = (PropFlags)0x0018000000000005), UMetaPath("/Script/CoreUObject.FloatRange:UpperBound")]
        public FFloatRangeBound UpperBound;

        static bool FFloatRange_IsValid;
        static int FFloatRange_StructSize;

        public FFloatRange Copy()
        {
            FFloatRange result = this;
            return result;
        }

        static FFloatRange()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FFloatRange)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FFloatRange));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.FloatRange");
            FFloatRange_StructSize = NativeReflection.GetStructSize(classAddress);
            LowerBound_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "LowerBound");
            LowerBound_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "LowerBound", Classes.UStructProperty);
            UpperBound_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "UpperBound");
            UpperBound_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "UpperBound", Classes.UStructProperty);
            FFloatRange_IsValid = classAddress != IntPtr.Zero && LowerBound_IsValid && UpperBound_IsValid;
            NativeReflection.LogStructIsValid("/Script/CoreUObject.FloatRange", FFloatRange_IsValid);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FFloatRange));
        }

        /// <summary>
        /// Create a range with a single element.
        /// 
        /// The created range is of the form [A, A].
        /// </summary>
        /// <param name="a">The element in the range.</param>
        public FFloatRange(float a)
        {
            LowerBound = FFloatRangeBound.Inclusive(a);
            UpperBound = FFloatRangeBound.Inclusive(a);
        }

        /// <summary>
        /// Create and initializes a new range with the given lower and upper bounds.
        /// 
        /// The created range is of the form [A, B).
        /// </summary>
        /// <param name="a">The range's lower bound value (inclusive).</param>
        /// <param name="b">The range's upper bound value (exclusive).</param>
        public FFloatRange(float a, float b)
        {
            LowerBound = FFloatRangeBound.Inclusive(a);
            UpperBound = FFloatRangeBound.Exclusive(b);
        }

        /// <summary>
        /// Create and initializes a new range with the given lower and upper bounds.
        /// 
        /// The created range is of the form [A, B).
        /// </summary>
        /// <param name="lowerBound">The range's lower bound value (inclusive).</param>
        /// <param name="upperBound">The range's upper bound value (exclusive).</param>
        public FFloatRange(FFloatRangeBound lowerBound, FFloatRangeBound upperBound)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        public static bool operator ==(FFloatRange a, FFloatRange b)
        {
            return a.LowerBound == b.LowerBound && a.UpperBound == b.UpperBound;
        }

        public static bool operator !=(FFloatRange a, FFloatRange b)
        {
            return a.LowerBound != b.LowerBound || a.UpperBound != b.UpperBound;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FFloatRange))
            {
                return false;
            }

            return Equals((FFloatRange)obj);
        }

        public bool Equals(FFloatRange other)
        {
            return LowerBound == other.LowerBound && UpperBound == other.UpperBound;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (LowerBound.GetHashCode() * 397) ^ UpperBound.GetHashCode();
            }
        }

        /// <summary>
        /// Check whether this range adjoins to another.
        /// 
        /// Two ranges are adjoint if they are next to each other without overlapping, i.e.
        /// 
        ///     [A, B) and [B, C) or
        ///     [A, B] and (B, C)
        /// </summary>
        /// <param name="other">The other range.</param>
        /// <returns>true if this range adjoins the other, false otherwise.</returns>
        public bool Adjoins(FFloatRange other)
        {
            if (IsEmpty() || other.IsEmpty())
            {
                return false;
            }

            if (!UpperBound.IsOpen() && !other.LowerBound.IsOpen() && UpperBound.GetValue() == other.LowerBound.GetValue())
            {
                return ((UpperBound.IsInclusive() && other.LowerBound.IsExclusive()) ||
                        (UpperBound.IsExclusive() && other.LowerBound.IsInclusive()));
            }

            if (!other.UpperBound.IsOpen() && !LowerBound.IsOpen() && other.UpperBound.GetValue() == LowerBound.GetValue())
            {
                return ((other.UpperBound.IsInclusive() && LowerBound.IsExclusive()) ||
                        (other.UpperBound.IsExclusive() && LowerBound.IsInclusive()));
            }

            return false;
        }

        /// <summary>
        /// Check whether this range conjoins the two given ranges.
        /// 
        /// A range conjoins two non-overlapping ranges if it adjoins both of them, i.e.
        ///     [B, C) conjoins the two ranges [A, B) and [C, D).
        /// </summary>
        /// <param name="x">The first range.</param>
        /// <param name="y">The second range.</param>
        /// <returns>true if this range conjoins the two ranges, false otherwise.</returns>
        public bool Conjoins(FFloatRange x, FFloatRange y)
        {
            if (x.Overlaps(y))
            {
                return false;
            }

            return (Adjoins(x) && Adjoins(y));
        }

        /// <summary>
        /// Check whether this range contains the specified element.
        /// </summary>
        /// <param name="element">The element to check.</param>
        /// <returns>true if the range contains the element, false otherwise.</returns>
        public bool Contains(float element)
        {
            return ((FFloatRangeBound.MinLower(LowerBound, new FFloatRangeBound(element)) == LowerBound) &&
                    (FFloatRangeBound.MaxUpper(UpperBound, new FFloatRangeBound(element)) == UpperBound));
        }

        /// <summary>
        /// Check whether this range contains another range.
        /// </summary>
        /// <param name="other">The range to check.</param>
        /// <returns>true if the range contains the other range, false otherwise.</returns>
        public bool Contains(FFloatRange other)
        {
            return ((FFloatRangeBound.MinLower(LowerBound, other.LowerBound) == LowerBound) &&
                    (FFloatRangeBound.MaxUpper(UpperBound, other.UpperBound) == UpperBound));
        }

        /// <summary>
        /// Check if this range is contiguous with another range.
        /// 
        /// Two ranges are contiguous if they are adjoint or overlapping.
        /// </summary>
        /// <param name="other">The other range.</param>
        /// <returns>true if the ranges are contiguous, false otherwise.</returns>
        public bool Contiguous(FFloatRange other)
        {
            return (Overlaps(other) || Adjoins(other));
        }

        /// <summary>
        /// Get the range's lower bound.
        /// </summary>
        /// <returns>Lower bound.</returns>
        public FFloatRangeBound GetLowerBound()
        {
            return LowerBound;
        }

        /// <summary>
        /// Assign the new lower bound for this range
        /// </summary>
        /// <param name="newLowerBound">The new lower bound to assign</param>
        public void SetLowerBound(FFloatRangeBound newLowerBound)
        {
            LowerBound = newLowerBound;
        }

        /// <summary>
        /// Assign the new lower bound value for this range. Current lower bound must not be open to call this method.
        /// </summary>
        /// <param name="newLowerBoundValue">The new lower bound value to assign</param>
        public void SetLowerBoundValue(float newLowerBoundValue)
        {
            LowerBound.SetValue(newLowerBoundValue);
        }

        /// <summary>
        /// Get the value of the lower bound.
        /// 
        /// Use HasLowerBound() to ensure that this range actually has a lower bound.
        /// </summary>
        /// <returns>Bound value.</returns>
        public float GetLowerBoundValue()
        {
            return LowerBound.GetValue();
        }

        /// <summary>
        /// Get the range's upper bound.
        /// </summary>
        /// <returns>Upper bound.</returns>
        public FFloatRangeBound GetUpperBound()
        {
            return UpperBound;
        }

        /// <summary>
        /// Assign the new upper bound for this range
        /// </summary>
        /// <param name="newUpperBound">The new upper bound to assign</param>
        public void SetUpperBound(FFloatRangeBound newUpperBound)
        {
            UpperBound = newUpperBound;
        }

        /// <summary>
        /// Assign the new upper bound value for this range. Current upper bound must not be open to call this method.
        /// </summary>
        /// <param name="newUpperBoundValue">The new upper bound value to assign</param>
        public void SetUpperBoundValue(float newUpperBoundValue)
        {
            UpperBound.SetValue(newUpperBoundValue);
        }

        /// <summary>
        /// Get the value of the upper bound.
        /// 
        /// Use HasUpperBound() to ensure that this range actually has an upper bound.
        /// </summary>
        /// <returns>Bound value.</returns>
        public float GetUpperBoundValue()
        {
            return UpperBound.GetValue();
        }

        /// <summary>
        /// Check whether the range has a lower bound.
        /// </summary>
        /// <returns>true if the range has a lower bound, false otherwise.</returns>
        public bool HasLowerBound()
        {
            return LowerBound.IsClosed();
        }

        /// <summary>
        /// Check whether the range has an upper bound.
        /// </summary>
        /// <returns>true if the range has an upper bound, false otherwise.</returns>
        public bool HasUpperBound()
        {
            return UpperBound.IsClosed();
        }

        /// <summary>
        /// Check whether this range is degenerate.
        /// 
        /// A range is degenerate if it contains only a single element, i.e. has the following form:
        ///     [A, A]
        /// </summary>
        /// <returns>true if the range is degenerate, false otherwise.</returns>
        /// <see cref="IsEmpty"/>
        public bool IsDegenerate()
        {
            return (LowerBound.IsInclusive() && (LowerBound == UpperBound));
        }

        /// <summary>
        /// Check whether this range is empty.
        /// 
        /// A range is empty if it contains no elements, i.e.
        ///     (A, A)
        ///     (A, A]
        ///     [A, A)
        /// </summary>
        /// <returns>true if the range is empty, false otherwise.</returns>
        /// <see cref="IsDegenerate"/>
        public bool IsEmpty()
        {
            if (LowerBound.IsClosed() && UpperBound.IsClosed())
            {
                if (LowerBound.GetValue() > UpperBound.GetValue())
                {
                    return true;
                }

                return ((LowerBound.GetValue() == UpperBound.GetValue()) && (LowerBound.IsExclusive() || UpperBound.IsExclusive()));
            }

            return false;
        }

        /// <summary>
        /// Check whether this range overlaps with another.
        /// </summary>
        /// <param name="other">The other range.</param>
        /// <returns>true if the ranges overlap, false otherwise.</returns>
        public bool Overlaps(FFloatRange other)
        {
            if (IsEmpty() || other.IsEmpty())
            {
                return false;
            }

            bool upperOpen = UpperBound.IsOpen() || other.LowerBound.IsOpen();
            bool lowerOpen = LowerBound.IsOpen() || other.UpperBound.IsOpen();

            // true in the case that the bounds are open (default)
            bool upperValid = true;
            bool lowerValid = true;

            if (!upperOpen)
            {
                bool upperGreaterThan = UpperBound.GetValue() > other.LowerBound.GetValue();
                bool upperGreaterThanOrEqualTo = UpperBound.GetValue() >= other.LowerBound.GetValue();
                bool upperBothInclusive = UpperBound.IsInclusive() && other.LowerBound.IsInclusive();

                upperValid = upperBothInclusive ? upperGreaterThanOrEqualTo : upperGreaterThan;
            }

            if (!lowerOpen)
            {
                bool lowerLessThan = LowerBound.GetValue() < other.UpperBound.GetValue();
                bool lowerLessThanOrEqualTo = LowerBound.GetValue() <= other.UpperBound.GetValue();
                bool lowerBothInclusive = LowerBound.IsInclusive() && other.UpperBound.IsInclusive();

                lowerValid = lowerBothInclusive ? lowerLessThanOrEqualTo : lowerLessThan;
            }

            return upperValid && lowerValid;
        }

        public float Size()
        {
            Debug.Assert(LowerBound.IsClosed() && UpperBound.IsClosed());

            return (UpperBound.GetValue() - LowerBound.GetValue());
        }

        /// <summary>
        /// Split the range into two ranges at the specified element.
        /// 
        /// If a range [A, C) does not contain the element X, the original range is returned.
        /// Otherwise the range is split into two ranges [A, X) and [X, C), each of which may be empty.
        /// </summary>
        /// <param name="element">The element at which to split the range.</param>
        /// <returns></returns>
        public FFloatRange[] Split(float element)
        {
            if (Contains(element))
            {
                return new FFloatRange[]
                {
                    new FFloatRange(LowerBound, FFloatRangeBound.Exclusive(element)),
                    new FFloatRange(FFloatRangeBound.Inclusive(element), UpperBound)
                };
            }
            else
            {
                return new FFloatRange[] { this };
            }
        }

        /// <summary>
        /// Calculate the difference between two ranges, i.e. X - Y.
        /// </summary>
        /// <param name="x">The first range to subtract from.</param>
        /// <param name="y">The second range to subtract with.</param>
        /// <returns>Between 0 and 2 remaining ranges.</returns>
        public static FFloatRange[] Difference(FFloatRange x, FFloatRange y)
        {
            if (x.Overlaps(y))
            {
                FFloatRange lowerRange = new FFloatRange(x.LowerBound, FFloatRangeBound.FlipInclusion(y.LowerBound));
                FFloatRange upperRange = new FFloatRange(FFloatRangeBound.FlipInclusion(y.UpperBound), x.UpperBound);

                if (!lowerRange.IsEmpty())
                {
                    if (!upperRange.IsEmpty())
                    {
                        return new FFloatRange[] { lowerRange, upperRange };
                    }
                    else
                    {
                        return new FFloatRange[] { lowerRange };
                    }
                }

                if (!upperRange.IsEmpty())
                {
                    return new FFloatRange[] { upperRange };
                }

                return new FFloatRange[0];
            }
            else
            {
                return new FFloatRange[] { x };
            }
        }

        /// <summary>
        /// Compute the hull of two ranges.
        /// 
        /// The hull is the smallest range that contains both ranges.
        /// </summary>
        /// <param name="x">The first range.</param>
        /// <param name="y">The second range.</param>
        /// <returns>The hull.</returns>
        public static FFloatRange Hull(FFloatRange x, FFloatRange y)
        {
            if (x.IsEmpty())
            {
                return y;
            }

            if (y.IsEmpty())
            {
                return x;
            }

            return new FFloatRange(
                FFloatRangeBound.MinLower(x.LowerBound, y.LowerBound),
                FFloatRangeBound.MaxUpper(x.UpperBound, y.UpperBound));
        }

        /// <summary>
        /// Compute the hull of many ranges.
        /// </summary>
        /// <param name="ranges">The ranges to hull.</param>
        /// <returns>The hull.</returns>
        public static FFloatRange Hull(FFloatRange[] ranges)
        {
            if (ranges == null || ranges.Length == 0)
            {
                return FFloatRange.Empty();
            }

            FFloatRange bounds = ranges[0];

            for (int i = 0; i < ranges.Length; i++)
            {
                bounds = Hull(bounds, ranges[i]);
            }

            return bounds;
        }

        /// <summary>
        /// Compute the intersection of two ranges.
        /// 
        /// The intersection of two ranges is the largest range that is contained by both ranges.
        /// </summary>
        /// <param name="x">The first range.</param>
        /// <param name="y">The second range.</param>
        /// <returns>The intersection, or an empty range if the ranges do not overlap.</returns>
        public static FFloatRange Intersection(FFloatRange x, FFloatRange y)
        {
            if (x.IsEmpty())
            {
                return FFloatRange.Empty();
            }

            if (y.IsEmpty())
            {
                return FFloatRange.Empty();
            }

            return new FFloatRange(
                FFloatRangeBound.MaxLower(x.LowerBound, y.LowerBound),
                FFloatRangeBound.MinUpper(x.UpperBound, y.UpperBound));
        }

        /// <summary>
        /// Compute the intersection of many ranges.
        /// </summary>
        /// <param name="ranges">The ranges to intersect.</param>
        /// <returns>The intersection.</returns>
        public static FFloatRange Intersection(FFloatRange[] ranges)
        {
            if (ranges == null || ranges.Length == 0)
            {
                return FFloatRange.Empty();
            }

            FFloatRange bounds = ranges[0];

            for (int i = 0; i < ranges.Length; i++)
            {
                bounds = Intersection(bounds, ranges[i]);
            }

            return bounds;
        }

        /// <summary>
        /// Return the union of two contiguous ranges.
        /// 
        /// A union is a range or series of ranges that contains both ranges.
        /// </summary>
        /// <param name="x">The first range.</param>
        /// <param name="y">The second range.</param>
        /// <returns>The union, or both ranges if the two ranges are not contiguous, or no ranges if both ranges are empty.</returns>
        public static FFloatRange[] Union(FFloatRange x, FFloatRange y)
        {
            if (x.Contains(y))
            {
                return new FFloatRange[]
                    {
                        new FFloatRange(
                            FFloatRangeBound.MinLower(x.LowerBound, y.LowerBound),
                            FFloatRangeBound.MaxUpper(x.UpperBound, y.UpperBound))
                    };
            }
            else
            {
                if (!x.IsEmpty())
                {
                    if (!y.IsEmpty())
                    {
                        return new FFloatRange[] { x, y };
                    }
                    else
                    {
                        return new FFloatRange[] { x };
                    }
                }

                if (!y.IsEmpty())
                {
                    return new FFloatRange[] { y };
                }

                return new FFloatRange[0];
            }
        }

        /// <summary>
        /// Create an unbounded (open) range that contains all elements of the domain.
        /// </summary>
        /// <returns>A new range.</returns>
        public static FFloatRange All()
        {
            return new FFloatRange(FFloatRangeBound.Open(), FFloatRangeBound.Open());
        }

        /// <summary>
        /// Create a left-bounded range that contains all elements greater than or equal to the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new range.</returns>
        public static FFloatRange AtLeast(float value)
        {
            return new FFloatRange(FFloatRangeBound.Inclusive(value), FFloatRangeBound.Open());
        }

        /// <summary>
        /// Create a right-bounded range that contains all elements less than or equal to the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new range.</returns>
        public static FFloatRange AtMost(float value)
        {
            return new FFloatRange(FFloatRangeBound.Open(), FFloatRangeBound.Inclusive(value));
        }

        /// <summary>
        /// Return an empty range.
        /// </summary>
        /// <returns>Empty range.</returns>
        public static FFloatRange Empty()
        {
            return new FFloatRange(FFloatRangeBound.Exclusive(default(float)), FFloatRangeBound.Exclusive(default(float)));
        }

        /// <summary>
        /// Create a range that excludes the given minimum and maximum values.
        /// </summary>
        /// <param name="min">The minimum value to be included.</param>
        /// <param name="max">The maximum value to be included.</param>
        /// <returns>A new range.</returns>
        public static FFloatRange Exclusive(float min, float max)
        {
            return new FFloatRange(FFloatRangeBound.Exclusive(min), FFloatRangeBound.Exclusive(max));
        }

        /// <summary>
        /// Create a left-bounded range that contains all elements greater than the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new range.</returns>
        public static FFloatRange GreaterThan(float value)
        {
            return new FFloatRange(FFloatRangeBound.Exclusive(value), FFloatRangeBound.Open());
        }

        /// <summary>
        /// Create a range that includes the given minimum and maximum values.
        /// </summary>
        /// <param name="min">The minimum value to be included.</param>
        /// <param name="max">The maximum value to be included.</param>
        /// <returns>A new range.</returns>
        public static FFloatRange Inclusive(float min, float max)
        {
            return new FFloatRange(FFloatRangeBound.Inclusive(min), FFloatRangeBound.Inclusive(max));
        }

        /// <summary>
        /// Create a right-bounded range that contains all elements less than the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new range.</returns>
        public static FFloatRange LessThan(float value)
        {
            return new FFloatRange(FFloatRangeBound.Open(), FFloatRangeBound.Exclusive(value));
        }
    }
}

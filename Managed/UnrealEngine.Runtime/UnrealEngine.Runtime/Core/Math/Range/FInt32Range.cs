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

    /// <summary>A int range</summary>
    [UStruct(Flags = 0x00000008), BlueprintType, UMetaPath("/Script/CoreUObject.Int32Range", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FInt32Range : IEquatable<FInt32Range>
    {
        static bool LowerBound_IsValid;
        static int LowerBound_Offset;
        [UProperty(Flags = (PropFlags)0x0018000000000005), UMetaPath("/Script/CoreUObject.Int32Range:LowerBound")]
        public FInt32RangeBound LowerBound;

        static bool UpperBound_IsValid;
        static int UpperBound_Offset;
        [UProperty(Flags = (PropFlags)0x0018000000000005), UMetaPath("/Script/CoreUObject.Int32Range:UpperBound")]
        public FInt32RangeBound UpperBound;

        static bool FInt32Range_IsValid;
        static int FInt32Range_StructSize;

        public FInt32Range Copy()
        {
            FInt32Range result = this;
            return result;
        }
        
        static FInt32Range()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FInt32Range)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FInt32Range));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Int32Range");
            FInt32Range_StructSize = NativeReflection.GetStructSize(classAddress);
            LowerBound_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "LowerBound");
            LowerBound_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "LowerBound", Classes.UStructProperty);
            UpperBound_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "UpperBound");
            UpperBound_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "UpperBound", Classes.UStructProperty);
            FInt32Range_IsValid = classAddress != IntPtr.Zero && LowerBound_IsValid && UpperBound_IsValid;
            NativeReflection.LogStructIsValid("/Script/CoreUObject.Int32Range", FInt32Range_IsValid);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FInt32Range));
        }

        /// <summary>
        /// Create a range with a single element.
        /// 
        /// The created range is of the form [A, A].
        /// </summary>
        /// <param name="a">The element in the range.</param>
        public FInt32Range(int a)
        {
            LowerBound = FInt32RangeBound.Inclusive(a);
            UpperBound = FInt32RangeBound.Inclusive(a);
        }

        /// <summary>
        /// Create and initializes a new range with the given lower and upper bounds.
        /// 
        /// The created range is of the form [A, B).
        /// </summary>
        /// <param name="a">The range's lower bound value (inclusive).</param>
        /// <param name="b">The range's upper bound value (exclusive).</param>
        public FInt32Range(int a, int b)
        {
            LowerBound = FInt32RangeBound.Inclusive(a);
            UpperBound = FInt32RangeBound.Exclusive(b);
        }

        /// <summary>
        /// Create and initializes a new range with the given lower and upper bounds.
        /// 
        /// The created range is of the form [A, B).
        /// </summary>
        /// <param name="lowerBound">The range's lower bound value (inclusive).</param>
        /// <param name="upperBound">The range's upper bound value (exclusive).</param>
        public FInt32Range(FInt32RangeBound lowerBound, FInt32RangeBound upperBound)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        public static bool operator ==(FInt32Range a, FInt32Range b)
        {
            return a.LowerBound == b.LowerBound && a.UpperBound == b.UpperBound;
        }

        public static bool operator !=(FInt32Range a, FInt32Range b)
        {
            return a.LowerBound != b.LowerBound || a.UpperBound != b.UpperBound;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FInt32Range))
            {
                return false;
            }

            return Equals((FInt32Range)obj);
        }

        public bool Equals(FInt32Range other)
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
        public bool Adjoins(FInt32Range other)
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
        public bool Conjoins(FInt32Range x, FInt32Range y)
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
        public bool Contains(int element)
        {
            return ((FInt32RangeBound.MinLower(LowerBound, new FInt32RangeBound(element)) == LowerBound) &&
                    (FInt32RangeBound.MaxUpper(UpperBound, new FInt32RangeBound(element)) == UpperBound));
        }

        /// <summary>
        /// Check whether this range contains another range.
        /// </summary>
        /// <param name="other">The range to check.</param>
        /// <returns>true if the range contains the other range, false otherwise.</returns>
        public bool Contains(FInt32Range other)
        {
            return ((FInt32RangeBound.MinLower(LowerBound, other.LowerBound) == LowerBound) &&
                    (FInt32RangeBound.MaxUpper(UpperBound, other.UpperBound) == UpperBound));
        }

        /// <summary>
        /// Check if this range is contiguous with another range.
        /// 
        /// Two ranges are contiguous if they are adjoint or overlapping.
        /// </summary>
        /// <param name="other">The other range.</param>
        /// <returns>true if the ranges are contiguous, false otherwise.</returns>
        public bool Contiguous(FInt32Range other)
        {
            return (Overlaps(other) || Adjoins(other));
        }

        /// <summary>
        /// Get the range's lower bound.
        /// </summary>
        /// <returns>Lower bound.</returns>
        /// <see cref="GetLowerBoundValue"/>
        /// <see cref="GetUpperBound"/>
        /// <see cref="HasLowerBound"/>
        public FInt32RangeBound GetLowerBound()
        {
            return LowerBound;
        }

        /// <summary>
        /// Assign the new lower bound for this range
        /// </summary>
        /// <param name="newLowerBound">The new lower bound to assign</param>
        /// <see cref="GetLowerBound"/>
        public void SetLowerBound(FInt32RangeBound newLowerBound)
        {
            LowerBound = newLowerBound;
        }

        /// <summary>
        /// Assign the new lower bound value for this range. Current lower bound must not be open to call this method.
        /// </summary>
        /// <param name="newLowerBoundValue">The new lower bound value to assign</param>
        /// <see cref="GetLowerBound"/>
        public void SetLowerBoundValue(int newLowerBoundValue)
        {
            LowerBound.SetValue(newLowerBoundValue);
        }

        /// <summary>
        /// Get the value of the lower bound.
        /// 
        /// Use HasLowerBound() to ensure that this range actually has a lower bound.
        /// </summary>
        /// <returns>Bound value.</returns>
        /// <see cref="GetLowerBound()"/>
        /// <see cref="GetUpperBoundValue"/>
        /// <see cref="HasLowerBound"/>
        public int GetLowerBoundValue()
        {
            return LowerBound.GetValue();
        }

        /// <summary>
        /// Get the range's upper bound.
        /// </summary>
        /// <returns>Upper bound.</returns>
        /// <see cref="GetLowerBound"/>
        /// <see cref="GetUpperBoundValue"/>
        /// <see cref="HasUpperBound"/>
        public FInt32RangeBound GetUpperBound()
        {
            return UpperBound;
        }

        /// <summary>
        /// Assign the new upper bound for this range
        /// </summary>
        /// <param name="newUpperBound">The new upper bound to assign</param>
        /// <see cref="GetUpperBound"/>
        public void SetUpperBound(FInt32RangeBound newUpperBound)
        {
            UpperBound = newUpperBound;
        }

        /// <summary>
        /// Assign the new upper bound value for this range. Current upper bound must not be open to call this method.
        /// </summary>
        /// <param name="newUpperBoundValue">The new upper bound value to assign</param>
        /// <see cref="GetUpperBound"/>
        public void SetUpperBoundValue(int newUpperBoundValue)
        {
            UpperBound.SetValue(newUpperBoundValue);
        }

        /// <summary>
        /// Get the value of the upper bound.
        /// 
        /// Use HasUpperBound() to ensure that this range actually has an upper bound.
        /// </summary>
        /// <returns>Bound value.</returns>
        /// <see cref="GetLowerBoundValue"/>
        /// <see cref="GetUpperBound"/>
        /// <see cref="HasUpperBound"/>
        public int GetUpperBoundValue()
        {
            return UpperBound.GetValue();
        }

        /// <summary>
        /// Check whether the range has a lower bound.
        /// </summary>
        /// <returns>true if the range has a lower bound, false otherwise.</returns>
        /// <see cref="GetLowerBound"/>
        /// <see cref="GetLowerBoundValue"/>
        /// <see cref="HasUpperBound"/>
        public bool HasLowerBound()
        {
            return LowerBound.IsClosed();
        }

        /// <summary>
        /// Check whether the range has an upper bound.
        /// </summary>
        /// <returns>true if the range has an upper bound, false otherwise.</returns>
        /// <see cref="GetUpperBound"/>
        /// <see cref="GetUpperBoundValue"/>
        /// <see cref="HasLowerBound"/>
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
        public bool Overlaps(FInt32Range other)
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

        public int Size()
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
        public FInt32Range[] Split(int element)
        {
            if (Contains(element))
            {
                return new FInt32Range[]
                {
                    new FInt32Range(LowerBound, FInt32RangeBound.Exclusive(element)),
                    new FInt32Range(FInt32RangeBound.Inclusive(element), UpperBound)
                };
            }
            else
            {
                return new FInt32Range[] { this };
            }
        }

        /// <summary>
        /// Calculate the difference between two ranges, i.e. X - Y.
        /// </summary>
        /// <param name="x">The first range to subtract from.</param>
        /// <param name="y">The second range to subtract with.</param>
        /// <returns>Between 0 and 2 remaining ranges.</returns>
        /// <see cref="Hull"/>
        /// <see cref="Intersection"/>
        /// <see cref="Union"/>
        public static FInt32Range[] Difference(FInt32Range x, FInt32Range y)
        {
            if (x.Overlaps(y))
            {
                FInt32Range lowerRange = new FInt32Range(x.LowerBound, FInt32RangeBound.FlipInclusion(y.LowerBound));
                FInt32Range upperRange = new FInt32Range(FInt32RangeBound.FlipInclusion(y.UpperBound), x.UpperBound);

                if (!lowerRange.IsEmpty())
                {
                    if (!upperRange.IsEmpty())
                    {
                        return new FInt32Range[] { lowerRange, upperRange };
                    }
                    else
                    {
                        return new FInt32Range[] { lowerRange };
                    }
                }

                if (!upperRange.IsEmpty())
                {
                    return new FInt32Range[] { upperRange };
                }

                return new FInt32Range[0];
            }
            else
            {
                return new FInt32Range[] { x };
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
        /// <see cref="Difference"/>
        /// <see cref="Intersection"/>
        /// <see cref="Union"/>
        public static FInt32Range Hull(FInt32Range x, FInt32Range y)
        {
            if (x.IsEmpty())
            {
                return y;
            }

            if (y.IsEmpty())
            {
                return x;
            }

            return new FInt32Range(
                FInt32RangeBound.MinLower(x.LowerBound, y.LowerBound),
                FInt32RangeBound.MaxUpper(x.UpperBound, y.UpperBound));
        }

        /// <summary>
        /// Compute the hull of many ranges.
        /// </summary>
        /// <param name="ranges">The ranges to hull.</param>
        /// <returns>The hull.</returns>
        /// <see cref="Difference"/>
        /// <see cref="Intersection"/>
        /// <see cref="Union"/>
        public static FInt32Range Hull(FInt32Range[] ranges)
        {
            if (ranges == null || ranges.Length == 0)
            {
                return FInt32Range.Empty();
            }

            FInt32Range bounds = ranges[0];

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
        /// <see cref="Difference"/>
        /// <see cref="Hull"/>
        /// <see cref="Union"/>
        public static FInt32Range Intersection(FInt32Range x, FInt32Range y)
        {
            if (x.IsEmpty())
            {
                return FInt32Range.Empty();
            }

            if (y.IsEmpty())
            {
                return FInt32Range.Empty();
            }

            return new FInt32Range(
                FInt32RangeBound.MaxLower(x.LowerBound, y.LowerBound),
                FInt32RangeBound.MinUpper(x.UpperBound, y.UpperBound));
        }

        /// <summary>
        /// Compute the intersection of many ranges.
        /// </summary>
        /// <param name="ranges">The ranges to intersect.</param>
        /// <returns>The intersection.</returns>
        /// <see cref="Difference"/>
        /// <see cref="Hull"/>
        /// <see cref="Union"/>
        public static FInt32Range Intersection(FInt32Range[] ranges)
        {
            if (ranges == null || ranges.Length == 0)
            {
                return FInt32Range.Empty();
            }

            FInt32Range bounds = ranges[0];

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
        /// <see cref="Difference"/>
        /// <see cref="Hull"/>
        /// <see cref="Intersection"/>
        public static FInt32Range[] Union(FInt32Range x, FInt32Range y)
        {
            if (x.Contains(y))
            {
                return new FInt32Range[]
                    {
                        new FInt32Range(
                            FInt32RangeBound.MinLower(x.LowerBound, y.LowerBound),
                            FInt32RangeBound.MaxUpper(x.UpperBound, y.UpperBound))
                    };
            }
            else
            {
                if (!x.IsEmpty())
                {
                    if (!y.IsEmpty())
                    {
                        return new FInt32Range[] { x, y };
                    }
                    else
                    {
                        return new FInt32Range[] { x };
                    }
                }

                if (!y.IsEmpty())
                {
                    return new FInt32Range[] { y };
                }

                return new FInt32Range[0];
            }
        }

        /// <summary>
        /// Create an unbounded (open) range that contains all elements of the domain.
        /// </summary>
        /// <returns>A new range.</returns>
        /// <see cref="AtLeast"/>
        /// <see cref="AtMost"/>
        /// <see cref="Empty"/>
        /// <see cref="Exclusive"/>
        /// <see cref="GreaterThan"/>
        /// <see cref="Inclusive"/>
        /// <see cref="LessThan"/>
        public static FInt32Range All()
        {
            return new FInt32Range(FInt32RangeBound.Open(), FInt32RangeBound.Open());
        }

        /// <summary>
        /// Create a left-bounded range that contains all elements greater than or equal to the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new range.</returns>
        /// <see cref="All"/>
        /// <see cref="AtMost"/>
        /// <see cref="Empty"/>
        /// <see cref="Exclusive"/>
        /// <see cref="GreaterThan"/>
        /// <see cref="Inclusive"/>
        /// <see cref="LessThan"/>
        public static FInt32Range AtLeast(int value)
        {
            return new FInt32Range(FInt32RangeBound.Inclusive(value), FInt32RangeBound.Open());
        }

        /// <summary>
        /// Create a right-bounded range that contains all elements less than or equal to the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new range.</returns>
        /// <see cref="All"/>
        /// <see cref="AtLeast"/>
        /// <see cref="Empty"/>
        /// <see cref="Exclusive"/>
        /// <see cref="GreaterThan"/>
        /// <see cref="Inclusive"/>
        /// <see cref="LessThan"/>
        public static FInt32Range AtMost(int value)
        {
            return new FInt32Range(FInt32RangeBound.Open(), FInt32RangeBound.Inclusive(value));
        }

        /// <summary>
        /// Return an empty range.
        /// </summary>
        /// <returns>Empty range.</returns>
        /// <see cref="All"/>
        /// <see cref="AtLeast"/>
        /// <see cref="AtMost"/>
        /// <see cref="Exclusive"/>
        /// <see cref="GreaterThan"/>
        /// <see cref="Inclusive"/>
        /// <see cref="LessThan"/>
        public static FInt32Range Empty()
        {
            return new FInt32Range(FInt32RangeBound.Exclusive(default(int)), FInt32RangeBound.Exclusive(default(int)));
        }

        /// <summary>
        /// Create a range that excludes the given minimum and maximum values.
        /// </summary>
        /// <param name="min">The minimum value to be included.</param>
        /// <param name="max">The maximum value to be included.</param>
        /// <returns>A new range.</returns>
        /// <see cref="All"/>
        /// <see cref="AtLeast"/>
        /// <see cref="AtMost"/>
        /// <see cref="Empty"/>
        /// <see cref="Exclusive"/>
        /// <see cref="GreaterThan"/>
        /// <see cref="Inclusive"/>
        /// <see cref="LessThan"/>
        public static FInt32Range Exclusive(int min, int max)
        {
            return new FInt32Range(FInt32RangeBound.Exclusive(min), FInt32RangeBound.Exclusive(max));
        }

        /// <summary>
        /// Create a left-bounded range that contains all elements greater than the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new range.</returns>
        /// <see cref="All"/>
        /// <see cref="AtLeast"/>
        /// <see cref="AtMost"/>
        /// <see cref="Empty"/>
        /// <see cref="Exclusive"/>
        /// <see cref="Inclusive"/>
        /// <see cref="LessThan"/>
        public static FInt32Range GreaterThan(int value)
        {
            return new FInt32Range(FInt32RangeBound.Exclusive(value), FInt32RangeBound.Open());
        }

        /// <summary>
        /// Create a range that includes the given minimum and maximum values.
        /// </summary>
        /// <param name="min">The minimum value to be included.</param>
        /// <param name="max">The maximum value to be included.</param>
        /// <returns>A new range.</returns>
        /// <see cref="All"/>
        /// <see cref="AtLeast"/>
        /// <see cref="AtMost"/>
        /// <see cref="Empty"/>
        /// <see cref="Exclusive"/>
        /// <see cref="GreaterThan"/>
        /// <see cref="LessThan"/>
        public static FInt32Range Inclusive(int min, int max)
        {
            return new FInt32Range(FInt32RangeBound.Inclusive(min), FInt32RangeBound.Inclusive(max));
        }

        /// <summary>
        /// Create a right-bounded range that contains all elements less than the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new range.</returns>
        /// <see cref="All"/>
        /// <see cref="AtLeast"/>
        /// <see cref="AtMost"/>
        /// <see cref="Empty"/>
        /// <see cref="Exclusive"/>
        /// <see cref="GreaterThan"/>
        /// <see cref="Inclusive"/>
        public static FInt32Range LessThan(int value)
        {
            return new FInt32Range(FInt32RangeBound.Open(), FInt32RangeBound.Exclusive(value));
        }
    }
}
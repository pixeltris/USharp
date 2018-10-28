using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\RangeBound.h
    // If updating a XXXXRangeBound class remember to generate code to update the others

    /// <summary>A int range bound</summary>
    [UStruct(Flags = 0x0000E008), BlueprintType, UMetaPath("/Script/CoreUObject.Int32RangeBound", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FInt32RangeBound : IEquatable<FInt32RangeBound>
    {
        static bool Type_IsValid;
        static UFieldAddress Type_PropertyAddress;
        static int Type_Offset;
        /// <summary>
        /// Holds the type of the bound.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001040000205), UMetaPath("/Script/CoreUObject.Int32RangeBound:Type")]
        public ERangeBoundTypes Type;

        static bool Value_IsValid;
        static int Value_Offset;
        /// <summary>
        /// Holds the bound's value.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001040000205), UMetaPath("/Script/CoreUObject.Int32RangeBound:Value")]
        public int Value;

        static int FInt32RangeBound_StructSize;

        public FInt32RangeBound Copy()
        {
            FInt32RangeBound result = this;
            return result;
        }

        static FInt32RangeBound()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FInt32RangeBound)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FInt32RangeBound));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Int32RangeBound");
            FInt32RangeBound_StructSize = NativeReflection.GetStructSize(classAddress);
            NativeReflectionCached.GetPropertyRef(ref Type_PropertyAddress, classAddress, "Type");
            Type_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Type");
            Type_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Type", Classes.UByteProperty);
            Value_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Value");
            Value_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Value", Classes.UFloatProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FInt32RangeBound));
        }

        /// <summary>
        /// Creates a closed bound that includes the specified value.
        /// </summary>
        /// <param name="value">The bound's value.</param>
        /// <see cref="Exclusive()"/>
        /// <see cref="Inclusive()"/>
        /// <see cref="Open()"/>
        public FInt32RangeBound(int value)
        {
            Type = ERangeBoundTypes.Inclusive;
            Value = value;
        }

        public static bool operator ==(FInt32RangeBound a, FInt32RangeBound b)
        {
            return a.Type == b.Type && (a.IsOpen() || a.Value == b.Value);
        }

        public static bool operator !=(FInt32RangeBound a, FInt32RangeBound b)
        {
            return a.Type != b.Type || (!a.IsOpen() && a.Value != b.Value);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FInt32RangeBound))
            {
                return false;
            }

            return Equals((FInt32RangeBound)obj);
        }

        public bool Equals(FInt32RangeBound other)
        {
            return Type == other.Type && (IsOpen() || Value == other.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode() * 397) ^ Value.GetHashCode();
            }
        }

        /// <summary>
        /// Gets the bound's value.
        /// 
        /// Use IsClosed() to verify that this bound is closed before calling this method.
        /// </summary>
        /// <returns>Bound value.</returns>
        /// <see cref="IsOpen()"/>
        public int GetValue()
        {
            Debug.Assert(Type != ERangeBoundTypes.Open);
            return Value;
        }

        /// <summary>
        /// Sets the bound's value, maintining the inclusivity of the bound
        /// 
        /// Use IsClosed() to verify that this bound is closed before calling this method.
        /// </summary>
        /// <param name="value">New bound value.</param>
        /// <see cref="IsClosed()"/>
        public void SetValue(int value)
        {
            Debug.Assert(Type != ERangeBoundTypes.Open);
            Value = value;
        }

        /// <summary>
        /// Checks whether the bound is closed.
        /// </summary>
        /// <returns>true if the bound is closed, false otherwise.</returns>
        public bool IsClosed()
        {
            return Type != ERangeBoundTypes.Open;
        }

        /// <summary>
        /// Checks whether the bound is exclusive.
        /// </summary>
        /// <returns>true if the bound is exclusive, false otherwise.</returns>
        public bool IsExclusive()
        {
            return Type == ERangeBoundTypes.Exclusive;
        }

        /// <summary>
        /// Checks whether the bound is inclusive.
        /// </summary>
        /// <returns>true if the bound is inclusive, false otherwise.</returns>
        public bool IsInclusive()
        {
            return Type == ERangeBoundTypes.Inclusive;
        }

        /// <summary>
        /// Checks whether the bound is open.
        /// </summary>
        /// <returns>true if the bound is open, false otherwise.</returns>
        public bool IsOpen()
        {
            return Type == ERangeBoundTypes.Open;
        }

        /// <summary>
        /// Returns a closed bound that excludes the specified value.
        /// </summary>
        /// <param name="value">The bound value.</param>
        /// <returns>An exclusive closed bound.</returns>
        public static FInt32RangeBound Exclusive(int value)
        {
            FInt32RangeBound result;

            result.Type = ERangeBoundTypes.Exclusive;
            result.Value = value;

            return result;
        }

        /// <summary>
        /// Returns a closed bound that includes the specified value.
        /// </summary>
        /// <param name="value">The bound value.</param>
        /// <returns>An inclusive closed bound.</returns>
        public static FInt32RangeBound Inclusive(int value)
        {
            FInt32RangeBound result;

            result.Type = ERangeBoundTypes.Inclusive;
            result.Value = value;

            return result;
        }

        /// <summary>
        /// Returns an open bound.
        /// </summary>
        /// <returns>An open bound.</returns>
        public static FInt32RangeBound Open()
        {
            FInt32RangeBound result;

            result.Type = ERangeBoundTypes.Open;
            result.Value = default(int);

            return result;
        }

        /// <summary>
        /// Returns the given bound with its inclusion flipped between inclusive and exclusive.
        /// 
        /// If the bound is open it is returned unchanged.
        /// </summary>
        /// <param name="bound"></param>
        /// <returns>A new bound.</returns>
        public static FInt32RangeBound FlipInclusion(FInt32RangeBound bound)
        {
            if (bound.IsExclusive())
            {
                return Inclusive(bound.Value);
            }

            if (bound.IsInclusive())
            {
                return Exclusive(bound.Value);
            }

            return bound;
        }

        /// <summary>
        /// Returns the greater of two lower bounds.
        /// </summary>
        /// <param name="a">The first lower bound.</param>
        /// <param name="b">The second lower bound.</param>
        /// <returns>The greater lower bound.</returns>
        public static FInt32RangeBound MaxLower(FInt32RangeBound a, FInt32RangeBound b)
        {
            if (a.IsOpen()) { return b; }
            if (b.IsOpen()) { return a; }
            if (a.Value > b.Value) { return a; }
            if (b.Value > a.Value) { return b; }
            if (a.IsExclusive()) { return a; }
            return b;
        }

        /// <summary>
        /// Returns the greater of two upper bounds.
        /// </summary>
        /// <param name="a">The first upper bound.</param>
        /// <param name="b">The second upper bound.</param>
        /// <returns>The greater upper bound.</returns>
        public static FInt32RangeBound MaxUpper(FInt32RangeBound a, FInt32RangeBound b)
        {
            if (a.IsOpen()) { return a; }
            if (b.IsOpen()) { return b; }
            if (a.Value > b.Value) { return a; }
            if (b.Value > a.Value) { return b; }
            if (a.IsInclusive()) { return a; }
            return b;
        }

        /// <summary>
        /// Returns the lesser of two lower bounds.
        /// </summary>
        /// <param name="a">The first lower bound.</param>
        /// <param name="b">The second lower bound.</param>
        /// <returns>The lesser lower bound.</returns>
        public static FInt32RangeBound MinLower(FInt32RangeBound a, FInt32RangeBound b)
        {
            if (a.IsOpen()) { return a; }
            if (b.IsOpen()) { return b; }
            if (a.Value < b.Value) { return a; }
            if (b.Value < a.Value) { return b; }
            return b;
        }

        /// <summary>
        /// Returns the lesser of two upper bounds.
        /// </summary>
        /// <param name="a">The first upper bound.</param>
        /// <param name="b">The second upper bound.</param>
        /// <returns>The lesser upper bound.</returns>
        public static FInt32RangeBound MinUpper(FInt32RangeBound a, FInt32RangeBound b)
        {
            if (a.IsOpen()) { return b; }
            if (b.IsOpen()) { return a; }
            if (a.Value < b.Value) { return a; }
            if (b.Value < a.Value) { return b; }
            if (a.IsExclusive()) { return a; }
            return b;
        }
    }
}
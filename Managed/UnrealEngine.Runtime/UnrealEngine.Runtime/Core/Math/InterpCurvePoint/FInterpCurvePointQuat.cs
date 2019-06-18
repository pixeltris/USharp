using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\InterpCurvePoint.h
    // If updating a FInterpCurvePointXXXX class remember to generate code to update the others

    /// <summary>
    /// Structure for interpolation points.
    /// 
    /// Interpolation points are used for describing the shape of interpolation curves.
    /// </summary>
    [UStruct(Flags = 0x00006008), BlueprintType, UMetaPath("/Script/CoreUObject.InterpCurvePointQuat")]
    [StructLayout(LayoutKind.Sequential)]
    public struct FInterpCurvePointQuat : IEquatable<FInterpCurvePointQuat>
    {
        static bool InVal_IsValid;
        static int InVal_Offset;
        /// <summary>
        /// Float input value that corresponds to this key (eg. time).
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001040000205), UMetaPath("/Script/CoreUObject.InterpCurvePointQuat:InVal")]
        public float InVal;

        // Alignment issued due to FQuat
        private long padding1;
        private int padding2;

        static bool OutVal_IsValid;
        static int OutVal_Offset;
        /// <summary>
        /// Output value of templated type when input is equal to InVal.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001040000005), UMetaPath("/Script/CoreUObject.InterpCurvePointQuat:OutVal")]
        public FQuat OutVal;

        static bool ArriveTangent_IsValid;
        static int ArriveTangent_Offset;
        /// <summary>
        /// Tangent of curve arrive this point.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001040000005), UMetaPath("/Script/CoreUObject.InterpCurvePointQuat:ArriveTangent")]
        public FQuat ArriveTangent;

        static bool LeaveTangent_IsValid;
        static int LeaveTangent_Offset;
        /// <summary>
        /// Tangent of curve leaving this point.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001040000005), UMetaPath("/Script/CoreUObject.InterpCurvePointQuat:LeaveTangent")]
        public FQuat LeaveTangent;

        static bool InterpMode_IsValid;
        static UFieldAddress InterpMode_PropertyAddress;
        static int InterpMode_Offset;
        /// <summary>
        /// Interpolation mode between this point and the next one.
        /// </summary>
        /// <see cref="EInterpCurveMode"/>
        [UProperty(Flags = (PropFlags)0x0018001040000205), UMetaPath("/Script/CoreUObject.InterpCurvePointQuat:InterpMode")]
        public EInterpCurveMode InterpMode;

        // Alignment issued due to FQuat
        private int padding3;

        static int FInterpCurvePointQuat_StructSize;

        public FInterpCurvePointQuat Copy()
        {
            FInterpCurvePointQuat result = this;
            return result;
        }

        static FInterpCurvePointQuat()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FInterpCurvePointQuat)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FInterpCurvePointQuat));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.InterpCurvePointQuat");
            FInterpCurvePointQuat_StructSize = NativeReflection.GetStructSize(classAddress);
            InVal_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "InVal");
            InVal_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "InVal", Classes.UFloatProperty);
            OutVal_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "OutVal");
            OutVal_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "OutVal", Classes.UStructProperty);
            ArriveTangent_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "ArriveTangent");
            ArriveTangent_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "ArriveTangent", Classes.UStructProperty);
            LeaveTangent_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "LeaveTangent");
            LeaveTangent_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "LeaveTangent", Classes.UStructProperty);
            NativeReflectionCached.GetPropertyRef(ref InterpMode_PropertyAddress, classAddress, "InterpMode");
            InterpMode_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "InterpMode");
            InterpMode_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "InterpMode", Classes.UByteProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FInterpCurvePointQuat));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inVal">input value that corresponds to this key</param>
        /// <param name="outVal">Output value of templated type</param>
        public FInterpCurvePointQuat(float inVal, FQuat outVal)
        {
            InVal = inVal;
            OutVal = outVal;
            ArriveTangent = default(FQuat);
            LeaveTangent = default(FQuat);
            InterpMode = EInterpCurveMode.Linear;
            padding1 = padding2 = padding3 = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inVal">input value that corresponds to this key</param>
        /// <param name="outVal">Output value of templated type</param>
        /// <param name="arriveTangent">Tangent of curve arriving at this point. </param>
        /// <param name="leaveTangent">Tangent of curve leaving from this point.</param>
        /// <param name="interpMode">interpolation mode to use</param>
        public FInterpCurvePointQuat(float inVal, FQuat outVal,
            FQuat arriveTangent, FQuat leaveTangent, EInterpCurveMode interpMode)
        {
            InVal = inVal;
            OutVal = outVal;
            ArriveTangent = arriveTangent;
            LeaveTangent = leaveTangent;
            InterpMode = interpMode;
            padding1 = padding2 = padding3 = 0;
        }

        /// <summary>
        /// Returns true if the key value is using a curve interp mode, otherwise false
        /// </summary>
        public bool IsCurveKey()
        {
            return
                InterpMode == EInterpCurveMode.CurveAuto ||
                InterpMode == EInterpCurveMode.CurveAutoClamped ||
                InterpMode == EInterpCurveMode.CurveUser ||
                InterpMode == EInterpCurveMode.CurveBreak;
        }

        public static bool operator ==(FInterpCurvePointQuat a, FInterpCurvePointQuat b)
        {
            return
                a.InVal == b.InVal &&
                a.OutVal == b.OutVal &&
                a.ArriveTangent == b.ArriveTangent &&
                a.LeaveTangent == b.LeaveTangent &&
                a.InterpMode == b.InterpMode;
        }

        public static bool operator !=(FInterpCurvePointQuat a, FInterpCurvePointQuat b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FInterpCurvePointQuat))
            {
                return false;
            }

            return Equals((FInterpCurvePointQuat)obj);
        }

        public bool Equals(FInterpCurvePointQuat other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = InVal.GetHashCode();
                hashcode = (hashcode * 397) ^ OutVal.GetHashCode();
                hashcode = (hashcode * 397) ^ ArriveTangent.GetHashCode();
                hashcode = (hashcode * 397) ^ LeaveTangent.GetHashCode();
                hashcode = (hashcode * 397) ^ InterpMode.GetHashCode();
                return hashcode;
            }
        }
    }
}

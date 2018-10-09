using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// A 4-D homogeneous vector.
    /// The full C++ class is located here: Engine\Source\Runtime\Core\Public\Math\Vector4.h
    /// </summary>
    [UStruct(Flags = 0x0000E838), BlueprintType, UMetaPath("/Script/CoreUObject.Vector4", "CoreUObject", UnrealModuleType.Engine)]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FVector4
    {
        static bool X_IsValid;
        static int X_Offset;
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector4:X")]
        public float X;

        static bool Y_IsValid;
        static int Y_Offset;
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector4:Y")]
        public float Y;

        static bool Z_IsValid;
        static int Z_Offset;
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector4:Z")]
        public float Z;

        static bool W_IsValid;
        static int W_Offset;
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector4:W")]
        public float W;

        static int FVector4_StructSize;

        public FVector4 Copy()
        {
            FVector4 result = this;
            return result;
        }

        static FVector4()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FVector4)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FVector4));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Vector4");
            FVector4_StructSize = NativeReflection.GetStructSize(classAddress);
            X_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "X");
            X_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "X", Classes.UFloatProperty);
            Y_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Y");
            Y_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Y", Classes.UFloatProperty);
            Z_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Z");
            Z_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Z", Classes.UFloatProperty);
            W_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "W");
            W_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "W", Classes.UFloatProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FVector4));
        }



        public static FVector4 FromNative(IntPtr nativeBuffer)
        {
            return BlittableTypeMarshaler<FVector4>.FromNative(nativeBuffer);
        }

        public static void ToNative(IntPtr nativeBuffer, FVector4 value)
        {
            value.ToNative(nativeBuffer);
        }

        public static FVector4 FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            return BlittableTypeMarshaler<FVector4>.FromNative(nativeBuffer + (arrayIndex * FVector4_StructSize));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, FVector4 value)
        {
            BlittableTypeMarshaler<FVector4>.ToNative(nativeBuffer, +(arrayIndex * FVector4_StructSize), value);
        }

        public void ToNative(IntPtr nativeStruct)
        {
            ToNative(nativeStruct, 0, IntPtr.Zero, this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// A point or direction FVector in 3d space.
    /// The full C++ class is located here: Engine\Source\Runtime\Core\Public\Math\Vector.h
    /// </summary>
    [UStruct(Flags = 0x0040EC38), BlueprintType, UMetaPath("/Script/CoreUObject.Vector", "CoreUObject", UnrealModuleType.Engine)]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FVector
    {
        static bool X_IsValid;
        static int X_Offset;
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector:X")]
        public float X;

        static bool Y_IsValid;
        static int Y_Offset;
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector:Y")]
        public float Y;

        static bool Z_IsValid;
        static int Z_Offset;
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector:Z")]
        public float Z;

        static int FVector_StructSize;

        public FVector Copy()
        {
            FVector result = this;
            return result;
        }

        static FVector()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FVector)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FVector));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Vector");
            FVector_StructSize = NativeReflection.GetStructSize(classAddress);
            X_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "X");
            X_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "X", Classes.UFloatProperty);
            Y_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Y");
            Y_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Y", Classes.UFloatProperty);
            Z_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Z");
            Z_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Z", Classes.UFloatProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FVector));
        }



        public static FVector FromNative(IntPtr nativeBuffer)
        {
            return BlittableTypeMarshaler<FVector>.FromNative(nativeBuffer);
        }

        public static void ToNative(IntPtr nativeBuffer, FVector value)
        {
            value.ToNative(nativeBuffer);
        }

        public static FVector FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            return BlittableTypeMarshaler<FVector>.FromNative(nativeBuffer + (arrayIndex * FVector_StructSize));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, FVector value)
        {
            BlittableTypeMarshaler<FVector>.ToNative(nativeBuffer, +(arrayIndex * FVector_StructSize), value);
        }

        public void ToNative(IntPtr nativeStruct)
        {
            ToNative(nativeStruct, 0, IntPtr.Zero, this);
        }
    }
}

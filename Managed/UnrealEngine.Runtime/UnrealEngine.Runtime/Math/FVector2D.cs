using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// A point or direction FVector in 2d space.
    /// The full C++ class is located here: Engine\Source\Runtime\Core\Public\Math\Vector2D.h
    /// </summary>
    [UStruct(Flags = 0x0040EC38), BlueprintType, UMetaPath("/Script/CoreUObject.Vector2D", "CoreUObject", UnrealModuleType.Engine)]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FVector2D
    {
        static bool X_IsValid;
        static int X_Offset;
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector2D:X")]
        public float X;

        static bool Y_IsValid;
        static int Y_Offset;
        [UProperty(Flags = (PropFlags)0x0018001041000205), UMetaPath("/Script/CoreUObject.Vector2D:Y")]
        public float Y;

        static int FVector2D_StructSize;

        public FVector2D Copy()
        {
            FVector2D result = this;
            return result;
        }

        static FVector2D()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FVector2D)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FVector2D));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Vector2D");
            FVector2D_StructSize = NativeReflection.GetStructSize(classAddress);
            X_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "X");
            X_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "X", Classes.UFloatProperty);
            Y_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Y");
            Y_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Y", Classes.UFloatProperty);
            NativeReflection.ValidateBlittableStructSize(classAddress, typeof(FVector2D));
        }




        public static FVector2D FromNative(IntPtr nativeBuffer)
        {
            return BlittableTypeMarshaler<FVector2D>.FromNative(nativeBuffer);
        }

        public static void ToNative(IntPtr nativeBuffer, FVector value)
        {
            value.ToNative(nativeBuffer);
        }

        public static FVector2D FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            return BlittableTypeMarshaler<FVector2D>.FromNative(nativeBuffer + (arrayIndex * FVector2D_StructSize));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, FVector2D value)
        {
            BlittableTypeMarshaler<FVector2D>.ToNative(nativeBuffer, +(arrayIndex * FVector2D_StructSize), value);
        }

        public void ToNative(IntPtr nativeStruct)
        {
            ToNative(nativeStruct, 0, IntPtr.Zero, this);
        }
    }
}

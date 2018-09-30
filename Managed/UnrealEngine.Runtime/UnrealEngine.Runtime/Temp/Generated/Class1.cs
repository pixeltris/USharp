using System;
using UnrealEngine.Runtime;

namespace UnrealEngine.Engine
{
    /// <summary>Structure for recording float values and displaying them as an Histogram through DrawDebugFloatHistory.</summary>
    [UMetaPath("/Script/Engine.DebugFloatHistory", "Engine", UnrealModuleType.Engine)]
    public struct FDebugFloatHistory
    {
        static bool MaxSamples_IsValid;
        static int MaxSamples_Offset;
        /// <summary>Max Samples to record.</summary>
        [UMetaPath("/Script/Engine.DebugFloatHistory:MaxSamples")]
        public float MaxSamples;

        static bool MinValue_IsValid;
        static int MinValue_Offset;
        /// <summary>Min value to record.</summary>
        [UMetaPath("/Script/Engine.DebugFloatHistory:MinValue")]
        public float MinValue;

        static bool MaxValue_IsValid;
        static int MaxValue_Offset;
        /// <summary>Max value to record.</summary>
        [UMetaPath("/Script/Engine.DebugFloatHistory:MaxValue")]
        public float MaxValue;

        static bool AutoAdjustMinMax_IsValid;
        static int AutoAdjustMinMax_Offset;
        static UFieldAddress AutoAdjustMinMax_PropertyAddress;
        /// <summary>Auto adjust Min/Max as new values are recorded?</summary>
        [UMetaPath("/Script/Engine.DebugFloatHistory:bAutoAdjustMinMax")]
        public bool AutoAdjustMinMax;

        static bool FDebugFloatHistory_IsValid;
        public static int StructSize { get; private set; }

        public FDebugFloatHistory Copy()
        {
            FDebugFloatHistory result = this;
            return result;
        }

        public static FDebugFloatHistory FromNative(IntPtr nativeBuffer)
        {
            return new FDebugFloatHistory(nativeBuffer);
        }

        public static void ToNative(IntPtr nativeBuffer, FDebugFloatHistory value)
        {
            value.ToNative(nativeBuffer);
        }

        public static FDebugFloatHistory FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, UObject owner)
        {
            return new FDebugFloatHistory(nativeBuffer + (arrayIndex * StructSize));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, UObject owner, FDebugFloatHistory value)
        {
            value.ToNative(nativeBuffer + (arrayIndex * StructSize));
        }

        public void ToNative(IntPtr nativeStruct)
        {
            if (!FDebugFloatHistory_IsValid)
            {
                return;
            }
            BlittableTypeMarshaler<float>.ToNative(IntPtr.Add(nativeStruct, MaxSamples_Offset), MaxSamples);
            BlittableTypeMarshaler<float>.ToNative(IntPtr.Add(nativeStruct, MinValue_Offset), MinValue);
            BlittableTypeMarshaler<float>.ToNative(IntPtr.Add(nativeStruct, MaxValue_Offset), MaxValue);
            BoolMarshaler.ToNative(IntPtr.Add(nativeStruct, AutoAdjustMinMax_Offset), 0, AutoAdjustMinMax_PropertyAddress.Address, AutoAdjustMinMax);
        }

        public FDebugFloatHistory(IntPtr nativeStruct)
        {
            if (!FDebugFloatHistory_IsValid)
            {
                MaxSamples = default(float);
                MinValue = default(float);
                MaxValue = default(float);
                AutoAdjustMinMax = default(bool);
                return;
            }
            MaxSamples = BlittableTypeMarshaler<float>.FromNative(IntPtr.Add(nativeStruct, MaxSamples_Offset));
            MinValue = BlittableTypeMarshaler<float>.FromNative(IntPtr.Add(nativeStruct, MinValue_Offset));
            MaxValue = BlittableTypeMarshaler<float>.FromNative(IntPtr.Add(nativeStruct, MaxValue_Offset));
            AutoAdjustMinMax = BoolMarshaler.FromNative(IntPtr.Add(nativeStruct, AutoAdjustMinMax_Offset), 0, AutoAdjustMinMax_PropertyAddress.Address);
        }

        static FDebugFloatHistory()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FDebugFloatHistory)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FDebugFloatHistory));
        }

        static void LoadNativeType()
        {
            //if (NativeReflection.CachedTypeInfo.Enabled)
            //{
            //    NativeReflection.CachedTypeInfo typeInfo = NativeReflection.CachedTypeInfo.BuildStruct("/Script/Engine.DebugFloatHistory");
            //    StructSize = typeInfo.Size;
            //    MaxSamples_IsValid = typeInfo.GetPropertyOffsetAndValidate("MaxSamples", out MaxSamples_Offset, Classes.UFloatProperty);
            //    MinValue_IsValid = typeInfo.GetPropertyOffsetAndValidate("MinValue", out MinValue_Offset, Classes.UFloatProperty);
            //    MaxValue_IsValid = typeInfo.GetPropertyOffsetAndValidate("MaxValue", out MaxValue_Offset, Classes.UFloatProperty);
            //    AutoAdjustMinMax_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("bAutoAdjustMinMax", ref AutoAdjustMinMax_PropertyAddress, out AutoAdjustMinMax_Offset, Classes.UBoolProperty);
            //    FDebugFloatHistory_IsValid = typeInfo.Exists && MaxSamples_IsValid && MaxValue_IsValid && AutoAdjustMinMax_IsValid;
            //    typeInfo.LogIsValid(FDebugFloatHistory_IsValid);
            //}
            //else
            {
                IntPtr classAddress = NativeReflection.GetStruct("/Script/Engine.DebugFloatHistory");
                StructSize = NativeReflection.GetStructSize(classAddress);
                MaxSamples_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "MaxSamples");
                MaxSamples_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "MaxSamples", Classes.UFloatProperty);
                MinValue_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "MinValue");
                MinValue_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "MinValue", Classes.UFloatProperty);
                MaxValue_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "MaxValue");
                MaxValue_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "MaxValue", Classes.UFloatProperty);
                NativeReflectionCached.GetPropertyRef(ref AutoAdjustMinMax_PropertyAddress, classAddress, "bAutoAdjustMinMax");
                AutoAdjustMinMax_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "bAutoAdjustMinMax");
                AutoAdjustMinMax_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "bAutoAdjustMinMax", Classes.UBoolProperty);
                FDebugFloatHistory_IsValid = classAddress != IntPtr.Zero && MaxSamples_IsValid && MinValue_IsValid && MaxValue_IsValid && AutoAdjustMinMax_IsValid;
                NativeReflection.LogStructIsValid("/Script/Engine.DebugFloatHistory", FDebugFloatHistory_IsValid);
            }
        }
    }
}
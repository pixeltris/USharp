using System;
using System.Runtime.InteropServices;
using UnrealEngine.Runtime;

namespace UnrealEngine.Engine
{
    /// <summary>Each elements in the grid</summary>
    [UMetaPath("/Script/Engine.EditorElement", "Engine", UnrealModuleType.Engine)]
    public struct FEditorElement
    {
        static bool Indices_IsValid;
        static UFieldAddress Indices_PropertyAddress;
        static int Indices_Offset;
        [UMetaPath("/Script/Engine.EditorElement:Indices")]
        public int[] Indices;

        static bool Weights_IsValid;
        static UFieldAddress Weights_PropertyAddress;
        static int Weights_Offset;
        [UMetaPath("/Script/Engine.EditorElement:Weights")]
        public float[] Weights;

        static bool FEditorElement_IsValid;
        public static int StructSize { get; private set; }

        public FEditorElement Copy()
        {
            FEditorElement result = this;
            return result;
        }

        public static FEditorElement FromNative(IntPtr nativeBuffer)
        {
            return new FEditorElement(nativeBuffer);
        }

        public static void ToNative(IntPtr nativeBuffer, FEditorElement value)
        {
            value.ToNative(nativeBuffer);
        }

        public static FEditorElement FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, UObject owner)
        {
            return new FEditorElement(nativeBuffer + (arrayIndex * StructSize));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, UObject owner, FEditorElement value)
        {
            value.ToNative(nativeBuffer + (arrayIndex * StructSize));
        }

        public void ToNative(IntPtr nativeStruct)
        {
            if (!FEditorElement_IsValid)
            {
                NativeReflection.LogInvalidStructAccessed("/Script/Engine.EditorElement");
                return;
            }
            TFixedSizeArrayMarshaler<int>.ToNative(IntPtr.Add(nativeStruct, Indices_Offset), 0, Indices_PropertyAddress.Address, Indices);
            TFixedSizeArrayMarshaler<float>.ToNative(IntPtr.Add(nativeStruct, Weights_Offset), 0, Weights_PropertyAddress.Address, Weights);
        }

        public FEditorElement(IntPtr nativeStruct)
        {
            if (!FEditorElement_IsValid)
            {
                NativeReflection.LogInvalidStructAccessed("/Script/Engine.EditorElement");
                Indices = null;
                Weights = null;
                return;
            }
            Indices = TFixedSizeArrayMarshaler<int>.FromNative(IntPtr.Add(nativeStruct, Indices_Offset), 0, Indices_PropertyAddress.Address);
            Weights = TFixedSizeArrayMarshaler<float>.FromNative(IntPtr.Add(nativeStruct, Weights_Offset), 0, Weights_PropertyAddress.Address);
        }

        static FEditorElement()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FEditorElement)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FEditorElement));
        }

        static void LoadNativeType()
        {
            //if (NativeReflection.CachedTypeInfo.Enabled)
            //{
            //    NativeReflection.CachedTypeInfo typeInfo = NativeReflection.CachedTypeInfo.BuildStruct("/Script/Engine.EditorElement");
            //    StructSize = typeInfo.Size;
            //    Indices_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("Indices", ref Indices_PropertyAddress, out Indices_Offset, Classes.UIntProperty);
            //    Weights_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("Weights", ref Weights_PropertyAddress, out Weights_Offset, Classes.UFloatProperty);
            //    FEditorElement_IsValid = typeInfo.Exists && Indices_IsValid && Weights_IsValid;
            //    typeInfo.LogIsValid(FEditorElement_IsValid);
            //}
            //else
            {
                IntPtr classAddress = NativeReflection.GetStruct("/Script/Engine.EditorElement");
                StructSize = NativeReflection.GetStructSize(classAddress);
                NativeReflectionCached.GetPropertyRef(ref Indices_PropertyAddress, classAddress, "Indices");
                Indices_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Indices");
                Indices_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Indices", Classes.UIntProperty);
                NativeReflectionCached.GetPropertyRef(ref Weights_PropertyAddress, classAddress, "Weights");
                Weights_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "Weights");
                Weights_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "Weights", Classes.UFloatProperty);
                FEditorElement_IsValid = classAddress != IntPtr.Zero && Indices_IsValid && Weights_IsValid;
                NativeReflection.LogStructIsValid("/Script/Engine.EditorElement", FEditorElement_IsValid);
            }
        }
    }
}

//namespace UnrealEngine.MaterialShaderQualitySettings
//{
//    [UMetaPath("/Script/MaterialShaderQualitySettings.ShaderPlatformQualitySettings", "MaterialShaderQualitySettings", UnrealModuleType.Engine)]
//    public class UShaderPlatformQualitySettings : UObject
//    {
//        static bool QualityOverrides_IsValid;
//        static UFieldAddress QualityOverrides_PropertyAddress;
//        static int QualityOverrides_Offset;
//        [UMetaPath("/Script/MaterialShaderQualitySettings.ShaderPlatformQualitySettings:QualityOverrides")]
//        public TFixedSizeArray<FMaterialQualityOverrides> QualityOverrides
//        {
//            get
//            {
//                CheckDestroyed();
//                if (!QualityOverrides_IsValid)
//                {
//                    NativeReflection.LogInvalidPropertyAccessed("/Script/MaterialShaderQualitySettings.ShaderPlatformQualitySettings:QualityOverrides");
//                    return default(TFixedSizeArray<FMaterialQualityOverrides>);
//                }
//                return new TFixedSizeArray<FMaterialQualityOverrides>(IntPtr.Add(Address, QualityOverrides_Offset), QualityOverrides_PropertyAddress, this);
//            }
//            //set
//            //{
//            //    CheckDestroyed();
//            //    if (!QualityOverrides_IsValid)
//            //    {
//            //        NativeReflection.LogInvalidPropertyAccessed("/Script/MaterialShaderQualitySettings.ShaderPlatformQualitySettings:QualityOverrides");
//            //        return;
//            //    }
//            //    FixedSizeArrayMarshaler<FMaterialQualityOverrides, BlittableTypeMarshaler<FMaterialQualityOverrides>>.ToNative(IntPtr.Add(Address, QualityOverrides_Offset), 0, QualityOverrides_PropertyAddress.Address, this, value);
//            //}
//        }
//
//        static void LoadNativeType()
//        {
//            IntPtr classAddress = NativeReflection.GetClass("/Script/MaterialShaderQualitySettings.ShaderPlatformQualitySettings");
//            NativeReflection.GetPropertyRef(ref QualityOverrides_PropertyAddress, classAddress, "QualityOverrides");
//            QualityOverrides_Offset = NativeReflection.GetPropertyOffset(classAddress, "QualityOverrides");
//            QualityOverrides_IsValid = NativeReflection.ValidatePropertyClass(classAddress, "QualityOverrides", Classes.UStructProperty);
//        }
//
//        /*static bool QualityOverrides_IsValid;
//        static UFieldAddress QualityOverrides_PropertyAddress;
//        static int QualityOverrides_Offset;
//        [UMetaPath("/Script/MaterialShaderQualitySettings.ShaderPlatformQualitySettings:QualityOverrides")]
//        public TFixedSizeArray<FMaterialQualityOverrides> QualityOverrides
//        {
//            get
//            {
//                CheckDestroyed();
//                if (!QualityOverrides_IsValid)
//                {
//                    NativeReflection.LogInvalidPropertyAccessed("/Script/MaterialShaderQualitySettings.ShaderPlatformQualitySettings:QualityOverrides");
//                    return null;
//                }
//                return FixedSizeArrayMarshaler<FMaterialQualityOverrides, BlittableTypeMarshaler<FMaterialQualityOverrides>>.FromNative(IntPtr.Add(Address, QualityOverrides_Offset), 0, QualityOverrides_PropertyAddress.Address, this);
//            }
//            set
//            {
//                QualityOverrides[3] = default(FMaterialQualityOverrides);
//
//                CheckDestroyed();
//                if (!QualityOverrides_IsValid)
//                {
//                    NativeReflection.LogInvalidPropertyAccessed("/Script/MaterialShaderQualitySettings.ShaderPlatformQualitySettings:QualityOverrides");
//                    return;
//                }
//                FixedSizeArrayMarshaler<FMaterialQualityOverrides, BlittableTypeMarshaler<FMaterialQualityOverrides>>.ToNative(IntPtr.Add(Address, QualityOverrides_Offset), 0, QualityOverrides_PropertyAddress.Address, this, value);
//            }
//        }
//
//        static void LoadNativeType()
//        {
//            IntPtr classAddress = NativeReflection.GetClass("/Script/MaterialShaderQualitySettings.ShaderPlatformQualitySettings");
//            NativeReflection.GetPropertyRef(ref QualityOverrides_PropertyAddress, classAddress, "QualityOverrides");
//            QualityOverrides_Offset = NativeReflection.GetPropertyOffset(classAddress, "QualityOverrides");
//            QualityOverrides_IsValid = NativeReflection.ValidatePropertyClass(classAddress, "QualityOverrides", Classes.UStructProperty);
//        }*/
//    }
//}

namespace UnrealEngine.MaterialShaderQualitySettings
{
    [UMetaPath("/Script/MaterialShaderQualitySettings.ShaderPlatformQualitySettings", "MaterialShaderQualitySettings", UnrealModuleType.Engine)]
    public class UShaderPlatformQualitySettings : UObject
    {
        static bool QualityOverrides_IsValid;
        static UFieldAddress QualityOverrides_PropertyAddress;
        static int QualityOverrides_Offset;
        TFixedSizeArray<FMaterialQualityOverrides> QualityOverrides_FixedSizeArrayCached;
        [UMetaPath("/Script/MaterialShaderQualitySettings.ShaderPlatformQualitySettings:QualityOverrides")]
        public TFixedSizeArray<FMaterialQualityOverrides> QualityOverrides
        {
            get
            {
                CheckDestroyed();
                if (!QualityOverrides_IsValid)
                {
                    NativeReflection.LogInvalidPropertyAccessed("/Script/MaterialShaderQualitySettings.ShaderPlatformQualitySettings:QualityOverrides");
                    return default(TFixedSizeArray<FMaterialQualityOverrides>);
                }
                if (QualityOverrides_FixedSizeArrayCached == null)
                {
                    QualityOverrides_FixedSizeArrayCached = new TFixedSizeArray<FMaterialQualityOverrides>(IntPtr.Add(Address, QualityOverrides_Offset), QualityOverrides_PropertyAddress, this);
                }
                return QualityOverrides_FixedSizeArrayCached;
            }
        }

        static UShaderPlatformQualitySettings()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(UShaderPlatformQualitySettings)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(UShaderPlatformQualitySettings));
        }

        static void LoadNativeType()
        {
            //if (NativeReflection.CachedTypeInfo.Enabled)
            //{
            //    NativeReflection.CachedTypeInfo typeInfo = NativeReflection.CachedTypeInfo.BuildClass("/Script/MaterialShaderQualitySettings.ShaderPlatformQualitySettings");
            //    QualityOverrides_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("QualityOverrides", ref QualityOverrides_PropertyAddress, out QualityOverrides_Offset, Classes.UStructProperty);
            //}
            //else
            {
                IntPtr classAddress = NativeReflection.GetClass("/Script/MaterialShaderQualitySettings.ShaderPlatformQualitySettings");
                NativeReflectionCached.GetPropertyRef(ref QualityOverrides_PropertyAddress, classAddress, "QualityOverrides");
                QualityOverrides_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "QualityOverrides");
                QualityOverrides_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "QualityOverrides", Classes.UStructProperty);
            }
        }
    }
}

namespace UnrealEngine.MaterialShaderQualitySettings
{
    /// <summary>FMaterialQualityOverrides represents the full set of possible material overrides per quality level.</summary>
    [UMetaPath("/Script/MaterialShaderQualitySettings.MaterialQualityOverrides", "MaterialShaderQualitySettings", UnrealModuleType.Engine)]
    public struct FMaterialQualityOverrides
    {
        static bool EnableOverride_IsValid;
        static UFieldAddress EnableOverride_PropertyAddress;
        static int EnableOverride_Offset;
        [UMetaPath("/Script/MaterialShaderQualitySettings.MaterialQualityOverrides:bEnableOverride")]
        public bool EnableOverride;

        static bool ForceFullyRough_IsValid;
        static UFieldAddress ForceFullyRough_PropertyAddress;
        static int ForceFullyRough_Offset;
        [UMetaPath("/Script/MaterialShaderQualitySettings.MaterialQualityOverrides:bForceFullyRough")]
        public bool ForceFullyRough;

        static bool ForceNonMetal_IsValid;
        static UFieldAddress ForceNonMetal_PropertyAddress;
        static int ForceNonMetal_Offset;
        [UMetaPath("/Script/MaterialShaderQualitySettings.MaterialQualityOverrides:bForceNonMetal")]
        public bool ForceNonMetal;

        static bool ForceDisableLMDirectionality_IsValid;
        static UFieldAddress ForceDisableLMDirectionality_PropertyAddress;
        static int ForceDisableLMDirectionality_Offset;
        [UMetaPath("/Script/MaterialShaderQualitySettings.MaterialQualityOverrides:bForceDisableLMDirectionality")]
        public bool ForceDisableLMDirectionality;

        static bool ForceLQReflections_IsValid;
        static UFieldAddress ForceLQReflections_PropertyAddress;
        static int ForceLQReflections_Offset;
        [UMetaPath("/Script/MaterialShaderQualitySettings.MaterialQualityOverrides:bForceLQReflections")]
        public bool ForceLQReflections;

        static bool MobileCSMQuality_IsValid;
        static UFieldAddress MobileCSMQuality_PropertyAddress;
        static int MobileCSMQuality_Offset;
        [UMetaPath("/Script/MaterialShaderQualitySettings.MaterialQualityOverrides:MobileCSMQuality")]
        public EMobileCSMQuality MobileCSMQuality;

        static bool FMaterialQualityOverrides_IsValid;
        public static int StructSize { get; private set; }

        public FMaterialQualityOverrides Copy()
        {
            FMaterialQualityOverrides result = this;
            return result;
        }

        public static FMaterialQualityOverrides FromNative(IntPtr nativeBuffer)
        {
            return new FMaterialQualityOverrides(nativeBuffer);
        }

        public static void ToNative(IntPtr nativeBuffer, FMaterialQualityOverrides value)
        {
            value.ToNative(nativeBuffer);
        }

        public static FMaterialQualityOverrides FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, UObject owner)
        {
            return new FMaterialQualityOverrides(nativeBuffer + (arrayIndex * StructSize));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, UObject owner, FMaterialQualityOverrides value)
        {
            value.ToNative(nativeBuffer + (arrayIndex * StructSize));
        }

        public void ToNative(IntPtr nativeStruct)
        {
            if (!FMaterialQualityOverrides_IsValid)
            {
                NativeReflection.LogInvalidStructAccessed("/Script/MaterialShaderQualitySettings.MaterialQualityOverrides");
                return;
            }
            BoolMarshaler.ToNative(IntPtr.Add(nativeStruct, EnableOverride_Offset), 0, EnableOverride_PropertyAddress.Address, EnableOverride);
            BoolMarshaler.ToNative(IntPtr.Add(nativeStruct, ForceFullyRough_Offset), 0, ForceFullyRough_PropertyAddress.Address, ForceFullyRough);
            BoolMarshaler.ToNative(IntPtr.Add(nativeStruct, ForceNonMetal_Offset), 0, ForceNonMetal_PropertyAddress.Address, ForceNonMetal);
            BoolMarshaler.ToNative(IntPtr.Add(nativeStruct, ForceDisableLMDirectionality_Offset), 0, ForceDisableLMDirectionality_PropertyAddress.Address, ForceDisableLMDirectionality);
            BoolMarshaler.ToNative(IntPtr.Add(nativeStruct, ForceLQReflections_Offset), 0, ForceLQReflections_PropertyAddress.Address, ForceLQReflections);
            EnumMarshaler<EMobileCSMQuality>.ToNative(IntPtr.Add(nativeStruct, MobileCSMQuality_Offset), 0, MobileCSMQuality_PropertyAddress.Address, MobileCSMQuality);
        }

        public FMaterialQualityOverrides(IntPtr nativeStruct)
        {
            if (!FMaterialQualityOverrides_IsValid)
            {
                NativeReflection.LogInvalidStructAccessed("/Script/MaterialShaderQualitySettings.MaterialQualityOverrides");
                EnableOverride = default(bool);
                ForceFullyRough = default(bool);
                ForceNonMetal = default(bool);
                ForceDisableLMDirectionality = default(bool);
                ForceLQReflections = default(bool);
                MobileCSMQuality = default(EMobileCSMQuality);
                return;
            }
            EnableOverride = BoolMarshaler.FromNative(IntPtr.Add(nativeStruct, EnableOverride_Offset), 0, EnableOverride_PropertyAddress.Address);
            ForceFullyRough = BoolMarshaler.FromNative(IntPtr.Add(nativeStruct, ForceFullyRough_Offset), 0, ForceFullyRough_PropertyAddress.Address);
            ForceNonMetal = BoolMarshaler.FromNative(IntPtr.Add(nativeStruct, ForceNonMetal_Offset), 0, ForceNonMetal_PropertyAddress.Address);
            ForceDisableLMDirectionality = BoolMarshaler.FromNative(IntPtr.Add(nativeStruct, ForceDisableLMDirectionality_Offset), 0, ForceDisableLMDirectionality_PropertyAddress.Address);
            ForceLQReflections = BoolMarshaler.FromNative(IntPtr.Add(nativeStruct, ForceLQReflections_Offset), 0, ForceLQReflections_PropertyAddress.Address);
            MobileCSMQuality = EnumMarshaler<EMobileCSMQuality>.FromNative(IntPtr.Add(nativeStruct, MobileCSMQuality_Offset), 0, MobileCSMQuality_PropertyAddress.Address);
        }

        static FMaterialQualityOverrides()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FMaterialQualityOverrides)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FMaterialQualityOverrides));
        }

        static void LoadNativeType()
        {
            //if (NativeReflection.CachedTypeInfo.Enabled)
            //{
            //    NativeReflection.CachedTypeInfo typeInfo = NativeReflection.CachedTypeInfo.BuildStruct("/Script/MaterialShaderQualitySettings.MaterialQualityOverrides");
            //    StructSize = typeInfo.Size;
            //    EnableOverride_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("bEnableOverride", ref EnableOverride_PropertyAddress, out EnableOverride_Offset, Classes.UBoolProperty);
            //    ForceFullyRough_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("bForceFullyRough", ref ForceFullyRough_PropertyAddress, out ForceFullyRough_Offset, Classes.UBoolProperty);
            //    ForceNonMetal_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("bForceNonMetal", ref ForceNonMetal_PropertyAddress, out ForceNonMetal_Offset, Classes.UBoolProperty);
            //    ForceDisableLMDirectionality_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("bForceDisableLMDirectionality", ref ForceDisableLMDirectionality_PropertyAddress, out ForceDisableLMDirectionality_Offset, Classes.UBoolProperty);
            //    ForceLQReflections_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("bForceLQReflections", ref ForceLQReflections_PropertyAddress, out ForceLQReflections_Offset, Classes.UBoolProperty);
            //    MobileCSMQuality_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("MobileCSMQuality", ref MobileCSMQuality_PropertyAddress, out MobileCSMQuality_Offset, Classes.UEnumProperty);
            //    FMaterialQualityOverrides_IsValid = typeInfo.Exists && EnableOverride_IsValid && ForceFullyRough_IsValid && ForceNonMetal_IsValid && ForceDisableLMDirectionality_IsValid && ForceLQReflections_IsValid && MobileCSMQuality_IsValid;
            //    typeInfo.LogIsValid(FMaterialQualityOverrides_IsValid);
            //}
            //else
            {
                IntPtr classAddress = NativeReflection.GetStruct("/Script/MaterialShaderQualitySettings.MaterialQualityOverrides");
                StructSize = NativeReflection.GetStructSize(classAddress);
                NativeReflectionCached.GetPropertyRef(ref EnableOverride_PropertyAddress, classAddress, "bEnableOverride");
                EnableOverride_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "bEnableOverride");
                EnableOverride_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "bEnableOverride", Classes.UBoolProperty);
                NativeReflectionCached.GetPropertyRef(ref ForceFullyRough_PropertyAddress, classAddress, "bForceFullyRough");
                ForceFullyRough_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "bForceFullyRough");
                ForceFullyRough_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "bForceFullyRough", Classes.UBoolProperty);
                NativeReflectionCached.GetPropertyRef(ref ForceNonMetal_PropertyAddress, classAddress, "bForceNonMetal");
                ForceNonMetal_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "bForceNonMetal");
                ForceNonMetal_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "bForceNonMetal", Classes.UBoolProperty);
                NativeReflectionCached.GetPropertyRef(ref ForceDisableLMDirectionality_PropertyAddress, classAddress, "bForceDisableLMDirectionality");
                ForceDisableLMDirectionality_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "bForceDisableLMDirectionality");
                ForceDisableLMDirectionality_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "bForceDisableLMDirectionality", Classes.UBoolProperty);
                NativeReflectionCached.GetPropertyRef(ref ForceLQReflections_PropertyAddress, classAddress, "bForceLQReflections");
                ForceLQReflections_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "bForceLQReflections");
                ForceLQReflections_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "bForceLQReflections", Classes.UBoolProperty);
                NativeReflectionCached.GetPropertyRef(ref MobileCSMQuality_PropertyAddress, classAddress, "MobileCSMQuality");
                MobileCSMQuality_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "MobileCSMQuality");
                MobileCSMQuality_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "MobileCSMQuality", Classes.UEnumProperty);
                FMaterialQualityOverrides_IsValid = classAddress != IntPtr.Zero && EnableOverride_IsValid && ForceFullyRough_IsValid && ForceNonMetal_IsValid && ForceDisableLMDirectionality_IsValid && ForceLQReflections_IsValid && MobileCSMQuality_IsValid;
                NativeReflection.LogStructIsValid("/Script/MaterialShaderQualitySettings.MaterialQualityOverrides", FMaterialQualityOverrides_IsValid);
            }
        }
    }
}

namespace UnrealEngine.MaterialShaderQualitySettings
{
    [UMetaPath("/Script/MaterialShaderQualitySettings.EMobileCSMQuality", "MaterialShaderQualitySettings", UnrealModuleType.Engine)]
    public enum EMobileCSMQuality
    {
        /// <summary>// Lowest quality, no filtering.</summary>
        NoFiltering = 0,
        /// <summary>Medium quality, 1x1 PCF filtering.</summary>
        PCF_1x1 = 1,
        /// <summary>Highest quality, 2x2 PCF filtering.</summary>
        PCF_2x2 = 2
    }
}
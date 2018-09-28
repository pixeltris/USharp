using System;
using System.Collections.Generic;
using UnrealEngine.Runtime;

namespace BlueprintTest.Pong.Test
{
    [UMetaPath("/Game/Pong/Test/TestSS.TestSS", "BlueprintTest", UnrealModuleType.Game)]
    public partial struct FTestSS
    {
        static bool MemberVar_0_IsValid;
        static UFieldAddress MemberVar_0_PropertyAddress;
        static int MemberVar_0_Offset;
        [UMetaPath("/Game/Pong/Test/TestSS.TestSS:MemberVar_0_B281C27C4F41FBF7A9E425A51E7BD044")]
        public bool MemberVar_0;

        static bool MemberVar_1_IsValid;
        static UFieldAddress MemberVar_1_PropertyAddress;
        static int MemberVar_1_Offset;
        [UMetaPath("/Game/Pong/Test/TestSS.TestSS:MemberVar_1_26_1BA5D22942BB264D88A4B2A9EEF51AC5")]
        public List<float> MemberVar_1;

        static bool MemberVar_21_IsValid;
        static int MemberVar_21_Offset;
        [UMetaPath("/Game/Pong/Test/TestSS.TestSS:MemberVar_21_25_B2CA1FFA4E95516B70C7E9A68011B260")]
        public byte MemberVar_21;

        static bool FTestSS_IsValid;
        static int StructSize { get; set; }

        public FTestSS Copy()
        {
            FTestSS result = this;
            if (this.MemberVar_1 != null)
            {
                result.MemberVar_1 = new List<float>(this.MemberVar_1);
            }
            return result;
        }

        public static FTestSS FromNative(IntPtr nativeBuffer)
        {
            return new FTestSS(nativeBuffer);
        }

        public static void ToNative(IntPtr nativeBuffer, FTestSS value)
        {
            value.ToNative(nativeBuffer);
        }

        public static FTestSS FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, UObject owner)
        {
            return new FTestSS(nativeBuffer + (arrayIndex * StructSize));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, UObject owner, FTestSS value)
        {
            value.ToNative(nativeBuffer + (arrayIndex * StructSize));
        }

        public void ToNative(IntPtr nativeStruct)
        {
            if (!FTestSS_IsValid)
            {
                NativeReflection.LogInvalidStructAccessed("/Game/Pong/Test/TestSS.TestSS");
                return;
            }
            BoolMarshaler.ToNative(IntPtr.Add(nativeStruct, MemberVar_0_Offset), 0, MemberVar_0_PropertyAddress.Address, MemberVar_0);
            TArrayCopyMarshaler<float> MemberVar_1_Marshaler = new TArrayCopyMarshaler<float>(1, MemberVar_1_PropertyAddress, CachedMarshalingDelegates<float, BlittableTypeMarshaler<float>>.FromNative, CachedMarshalingDelegates<float, BlittableTypeMarshaler<float>>.ToNative);
            MemberVar_1_Marshaler.ToNative(IntPtr.Add(nativeStruct, MemberVar_1_Offset), MemberVar_1);
            BlittableTypeMarshaler<byte>.ToNative(IntPtr.Add(nativeStruct, MemberVar_21_Offset), MemberVar_21);
        }

        public FTestSS(IntPtr nativeStruct)
        {
            if (!FTestSS_IsValid)
            {
                NativeReflection.LogInvalidStructAccessed("/Game/Pong/Test/TestSS.TestSS");
                MemberVar_0 = default(bool);
                MemberVar_1 = default(List<float>);
                MemberVar_21 = default(byte);
                return;
            }
            MemberVar_0 = BoolMarshaler.FromNative(IntPtr.Add(nativeStruct, MemberVar_0_Offset), 0, MemberVar_0_PropertyAddress.Address);
            TArrayCopyMarshaler<float> MemberVar_1_Marshaler = new TArrayCopyMarshaler<float>(1, MemberVar_1_PropertyAddress, CachedMarshalingDelegates<float, BlittableTypeMarshaler<float>>.FromNative, CachedMarshalingDelegates<float, BlittableTypeMarshaler<float>>.ToNative);
            MemberVar_1 = MemberVar_1_Marshaler.FromNative(IntPtr.Add(nativeStruct, MemberVar_1_Offset));
            MemberVar_21 = BlittableTypeMarshaler<byte>.FromNative(IntPtr.Add(nativeStruct, MemberVar_21_Offset));
        }

        static FTestSS()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FTestSS)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FTestSS));
        }

        static void LoadNativeType()
        {
            //if (NativeReflection.CachedTypeInfo.Enabled)
            //{
            //    NativeReflection.CachedTypeInfo typeInfo = NativeReflection.CachedTypeInfo.BuildStruct("/Game/Pong/Test/TestSS.TestSS");
            //    StructSize = typeInfo.Size;
            //    MemberVar_0_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("MemberVar_0_B281C27C4F41FBF7A9E425A51E7BD044", ref MemberVar_0_PropertyAddress, out MemberVar_0_Offset, Classes.UBoolProperty);
            //    MemberVar_1_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("MemberVar_1_26_1BA5D22942BB264D88A4B2A9EEF51AC5", ref MemberVar_1_PropertyAddress, out MemberVar_1_Offset, Classes.UArrayProperty);
            //    MemberVar_21_IsValid = typeInfo.GetPropertyOffsetAndValidate("MemberVar_1_26_1BA5D22942BB264D88A4B2A9EEF51AC5", out MemberVar_21_Offset, Classes.UByteProperty);
            //    FTestSS_IsValid = typeInfo.Exists && MemberVar_0_IsValid && MemberVar_1_IsValid && MemberVar_21_IsValid;
            //    typeInfo.LogIsValid(FTestSS_IsValid);
            //}
            //else
            {
                IntPtr classAddress = NativeReflection.GetStruct("/Game/Pong/Test/TestSS.TestSS");
                StructSize = NativeReflection.GetStructSize(classAddress);
                NativeReflectionCached.GetPropertyRef(ref MemberVar_0_PropertyAddress, classAddress, "MemberVar_0_B281C27C4F41FBF7A9E425A51E7BD044");
                MemberVar_0_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "MemberVar_0_B281C27C4F41FBF7A9E425A51E7BD044");
                MemberVar_0_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "MemberVar_0_B281C27C4F41FBF7A9E425A51E7BD044", Classes.UBoolProperty);
                NativeReflectionCached.GetPropertyRef(ref MemberVar_1_PropertyAddress, classAddress, "MemberVar_1_26_1BA5D22942BB264D88A4B2A9EEF51AC5");
                MemberVar_1_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "MemberVar_1_26_1BA5D22942BB264D88A4B2A9EEF51AC5");
                MemberVar_1_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "MemberVar_1_26_1BA5D22942BB264D88A4B2A9EEF51AC5", Classes.UArrayProperty);
                MemberVar_21_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "MemberVar_21_25_B2CA1FFA4E95516B70C7E9A68011B260");
                MemberVar_21_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "MemberVar_21_25_B2CA1FFA4E95516B70C7E9A68011B260", Classes.UByteProperty);
                FTestSS_IsValid = classAddress != IntPtr.Zero && MemberVar_0_IsValid && MemberVar_1_IsValid && MemberVar_21_IsValid;
                NativeReflection.LogStructIsValid("/Game/Pong/Test/TestSS.TestSS", FTestSS_IsValid);
            }
        }
    }
}
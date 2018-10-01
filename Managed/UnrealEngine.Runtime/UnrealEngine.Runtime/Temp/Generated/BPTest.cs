using System;
using System.Collections.Generic;
using UnrealEngine.CoreUObject;
using UnrealEngine.Runtime;

namespace UnrealEngine.Runtime
{
    [UMetaPath("/Script/USharp.BPTest1", "USharp", UnrealModuleType.EnginePlugin)]
    public class UBPTest1 : UObject
    {
        public class FSomeDelegate : FDelegate<FSomeDelegate.Signature>
        {
            public delegate void Signature(byte NewParam, ref string NewParam2);
        }

        //static bool FuncWithDelegate_IsValid;
        //static IntPtr FuncWithDelegate_FunctionAddress;
        //static int FuncWithDelegate_ParamsSize;
        //static bool FuncWithDelegate_SomeDelegate_IsValid;
        //static int FuncWithDelegate_SomeDelegate_Offset;
        //FSomeDelegate FuncWithDelegate_SomeDelegate_DelegateCached;
        //[UMetaPath("/Script/USharp.BPTest1:FuncWithDelegate")]
        //public void FuncWithDelegate(FSomeDelegate SomeDelegate)
        //{
        //    CheckDestroyed();
        //    if (!FuncWithDelegate_IsValid)
        //    {
        //        NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.BPTest1:FuncWithDelegate");
        //        return;
        //    }
        //    unsafe
        //    {
        //        byte* ParamsBufferAllocation = stackalloc byte[FuncWithDelegate_ParamsSize];
        //        IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
        //        FDelegate<FSomeDelegate.Signature>.Marshaler<FSomeDelegate>.ToNative(IntPtr.Add(ParamsBuffer, FuncWithDelegate_SomeDelegate_Offset), SomeDelegate);
        //
        //        NativeReflection.InvokeFunction(Address, FuncWithDelegate_FunctionAddress, ParamsBuffer, FuncWithDelegate_ParamsSize);
        //        NativeReflection.InvokeFunction_DestroyAll(FuncWithDelegate_FunctionAddress, ParamsBuffer);
        //    }
        //}

        static bool MyMap_IsValid;
        static UFieldAddress MyMap_PropertyAddress;
        static int MyMap_Offset;
        TMapReadWriteMarshaler<string, int> MyMap_MarshalerCached;
        [UMetaPath("/Script/USharp.BPTest1:MyMap")]
        public TMapReadWrite<string, int> MyMap
        {
            get
            {
                CheckDestroyed();
                if (!MyMap_IsValid)
                {
                    NativeReflection.LogInvalidPropertyAccessed("/Script/USharp.BPTest1:MyMap");
                    return default(TMapReadWrite<string, int>);
                }
                if (MyMap_MarshalerCached == null)
                {
                    MyMap_MarshalerCached = new TMapReadWriteMarshaler<string, int>(1, MyMap_PropertyAddress, CachedMarshalingDelegates<string, FStringMarshaler>.FromNative, CachedMarshalingDelegates<string, FStringMarshaler>.ToNative, CachedMarshalingDelegates<int, BlittableTypeMarshaler<int>>.FromNative, CachedMarshalingDelegates<int, BlittableTypeMarshaler<int>>.ToNative);
                }
                return MyMap_MarshalerCached.FromNative(IntPtr.Add(Address, MyMap_Offset));
            }
        }

        static bool MySet_IsValid;
        static UFieldAddress MySet_PropertyAddress;
        static int MySet_Offset;
        TSetReadWriteMarshaler<string> MySet_MarshalerCached;
        [UMetaPath("/Script/USharp.BPTest1:MySet")]
        public TSetReadWrite<string> MySet
        {
            get
            {
                CheckDestroyed();
                if (!MySet_IsValid)
                {
                    NativeReflection.LogInvalidPropertyAccessed("/Script/USharp.BPTest1:MySet");
                    return default(TSetReadWrite<string>);
                }
                if (MySet_MarshalerCached == null)
                {
                    MySet_MarshalerCached = new TSetReadWriteMarshaler<string>(1, MySet_PropertyAddress, CachedMarshalingDelegates<string, FStringMarshaler>.FromNative, CachedMarshalingDelegates<string, FStringMarshaler>.ToNative);
                }
                return MySet_MarshalerCached.FromNative(IntPtr.Add(Address, MySet_Offset));
            }
        }

        static bool CallMe6_IsValid;
        static IntPtr CallMe6_FunctionAddress;
        static int CallMe6_ParamsSize;
        static bool CallMe6_test_IsValid;
        static int CallMe6_test_Offset;
        static bool CallMe6_test2_IsValid;
        static int CallMe6_test2_Offset;
        /// <summary>UPARAM(ref) int32& IntRef, FString StrParam, FString a1, FMyStruct& a2);</summary>
        [UMetaPath("/Script/USharp.BPTest1:CallMe6")]
        public void CallMe6(FMyStructCustomCtor test, ref FMyStructCustomCtor test2)
        {
            CheckDestroyed();
            if (!CallMe6_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.BPTest1:CallMe6");
                return;
            }
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[CallMe6_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                FMyStructCustomCtor.ToNative(IntPtr.Add(ParamsBuffer, CallMe6_test_Offset), test);
                FMyStructCustomCtor.ToNative(IntPtr.Add(ParamsBuffer, CallMe6_test2_Offset), test2);

                NativeReflection.InvokeFunction(Address, CallMe6_FunctionAddress, ParamsBuffer, CallMe6_ParamsSize);

                test2 = FMyStructCustomCtor.FromNative(IntPtr.Add(ParamsBuffer, CallMe6_test2_Offset));
                NativeReflection.InvokeFunction_DestroyAll(CallMe6_FunctionAddress, ParamsBuffer);
            }
        }

        static bool CallMe5_IsValid;
        static IntPtr CallMe5_FunctionAddress;
        static int CallMe5_ParamsSize;
        static bool CallMe5_a1_IsValid;
        static UFieldAddress CallMe5_a1_PropertyAddress;
        static int CallMe5_a1_Offset;
        static bool CallMe5_a2_IsValid;
        static UFieldAddress CallMe5_a2_PropertyAddress;
        static int CallMe5_a2_Offset;
        /// <summary>UPARAM(ref) int32& IntRef, FString StrParam, FString a1, FMyStruct& a2);</summary>
        [UMetaPath("/Script/USharp.BPTest1:CallMe5")]
        public void CallMe5(ref List<string> a1, List<string> a2)
        {
            CheckDestroyed();
            if (!CallMe5_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.BPTest1:CallMe5");
                return;
            }
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[CallMe5_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                TArrayCopyMarshaler<string> CallMe5_a1_Marshaler = new TArrayCopyMarshaler<string>(1, CallMe5_a1_PropertyAddress, CachedMarshalingDelegates<string, FStringMarshaler>.FromNative, CachedMarshalingDelegates<string, FStringMarshaler>.ToNative);
                CallMe5_a1_Marshaler.ToNative(IntPtr.Add(ParamsBuffer, CallMe5_a1_Offset), a1);
                TArrayCopyMarshaler<string> CallMe5_a2_Marshaler = new TArrayCopyMarshaler<string>(1, CallMe5_a2_PropertyAddress, CachedMarshalingDelegates<string, FStringMarshaler>.FromNative, CachedMarshalingDelegates<string, FStringMarshaler>.ToNative);
                CallMe5_a2_Marshaler.ToNative(IntPtr.Add(ParamsBuffer, CallMe5_a2_Offset), a2);

                NativeReflection.InvokeFunction(Address, CallMe5_FunctionAddress, ParamsBuffer, CallMe5_ParamsSize);

                a1 = CallMe5_a1_Marshaler.FromNative(IntPtr.Add(ParamsBuffer, CallMe5_a1_Offset));
                NativeReflection.InvokeFunction_DestroyAll(CallMe5_FunctionAddress, ParamsBuffer);
            }
        }

        static bool CallMe4_IsValid;
        static IntPtr CallMe4_FunctionAddress;
        static int CallMe4_ParamsSize;
        static bool CallMe4_ReturnValue_IsValid;
        static UFieldAddress CallMe4_ReturnValue_PropertyAddress;
        static int CallMe4_ReturnValue_Offset;
        /// <summary>UPARAM(ref) int32& IntRef, FString StrParam, FString a1, FMyStruct& a2);</summary>
        [UMetaPath("/Script/USharp.BPTest1:CallMe4")]
        public List<string> CallMe4()
        {
            CheckDestroyed();
            if (!CallMe4_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.BPTest1:CallMe4");
                return default(List<string>);
            }
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[CallMe4_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);

                NativeReflection.InvokeFunction(Address, CallMe4_FunctionAddress, ParamsBuffer, CallMe4_ParamsSize);

                TArrayCopyMarshaler<string> CallMe4_ReturnValue_Marshaler = new TArrayCopyMarshaler<string>(1, CallMe4_ReturnValue_PropertyAddress, CachedMarshalingDelegates<string, FStringMarshaler>.FromNative, CachedMarshalingDelegates<string, FStringMarshaler>.ToNative);
                List<string> toReturn = CallMe4_ReturnValue_Marshaler.FromNative(IntPtr.Add(ParamsBuffer, CallMe4_ReturnValue_Offset));
                NativeReflection.InvokeFunction_DestroyAll(CallMe4_FunctionAddress, ParamsBuffer);
                return toReturn;
            }
        }

        static bool CallMe3_IsValid;
        static IntPtr CallMe3_FunctionAddress;
        static int CallMe3_ParamsSize;
        static bool CallMe3_ReturnValue_IsValid;
        static UFieldAddress CallMe3_ReturnValue_PropertyAddress;
        static int CallMe3_ReturnValue_Offset;
        [UMetaPath("/Script/USharp.BPTest1:CallMe3")]
        public List<string> CallMe3()
        {
            CheckDestroyed();
            if (!CallMe3_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.BPTest1:CallMe3");
                return default(List<string>);
            }
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[CallMe3_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);

                NativeReflection.InvokeFunction(Address, CallMe3_FunctionAddress, ParamsBuffer, CallMe3_ParamsSize);

                TArrayCopyMarshaler<string> CallMe3_ReturnValue_Marshaler = new TArrayCopyMarshaler<string>(1, CallMe3_ReturnValue_PropertyAddress, CachedMarshalingDelegates<string, FStringMarshaler>.FromNative, CachedMarshalingDelegates<string, FStringMarshaler>.ToNative);
                List<string> toReturn = CallMe3_ReturnValue_Marshaler.FromNative(IntPtr.Add(ParamsBuffer, CallMe3_ReturnValue_Offset));
                NativeReflection.InvokeFunction_DestroyAll(CallMe3_FunctionAddress, ParamsBuffer);
                return toReturn;
            }
        }

        static bool CallMe2_IsValid;
        static IntPtr CallMe2_FunctionAddress;
        static int CallMe2_ParamsSize;
        [UMetaPath("/Script/USharp.BPTest1:CallMe2")]
        public void CallMe2()
        {
            CheckDestroyed();
            if (!CallMe2_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.BPTest1:CallMe2");
                return;
            }
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[CallMe2_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);

                NativeReflection.InvokeFunction(Address, CallMe2_FunctionAddress, ParamsBuffer, CallMe2_ParamsSize);
                NativeReflection.InvokeFunction_DestroyAll(CallMe2_FunctionAddress, ParamsBuffer);
            }
        }

        static bool CallMe_IsValid;
        IntPtr CallMe_InstanceFunctionAddress;
        static IntPtr CallMe_FunctionAddress;
        static int CallMe_ParamsSize;
        [UMetaPath("/Script/USharp.BPTest1:CallMe")]
        [UFunction, BlueprintEvent]
        public void CallMe()
        {
            CheckDestroyed();
            if (!CallMe_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.BPTest1:CallMe");
                return;
            }
            if (CallMe_InstanceFunctionAddress == IntPtr.Zero)
            {
                CallMe_InstanceFunctionAddress = NativeReflection.GetFunctionFromInstance(Address, "CallMe");
            }
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[CallMe_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);

                NativeReflection.InvokeFunction(Address, CallMe_InstanceFunctionAddress, ParamsBuffer, CallMe_ParamsSize);
                NativeReflection.InvokeFunction_DestroyAll(CallMe_InstanceFunctionAddress, ParamsBuffer);
            }
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual void CallMe_Implementation()
        {
            CheckDestroyed();
            if (!CallMe_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.BPTest1:CallMe");
                return;
            }
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[CallMe_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);

                NativeReflection.InvokeFunction(Address, CallMe_FunctionAddress, ParamsBuffer, CallMe_ParamsSize);
                NativeReflection.InvokeFunction_DestroyAll(CallMe_FunctionAddress, ParamsBuffer);
            }
        }

        static UBPTest1()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(UBPTest1)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(UBPTest1));
        }

        static void LoadNativeType()
        {
            //if (NativeReflection.CachedTypeInfo.Enabled)
            //{
            //    NativeReflection.CachedTypeInfo typeInfo = NativeReflection.CachedTypeInfo.BuildClass("/Script/USharp.BPTest1");
            //    MyMap_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("MyMap", ref MyMap_PropertyAddress, out MyMap_Offset, Classes.UMapProperty);
            //    MySet_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("MySet", ref MySet_PropertyAddress, out MySet_Offset, Classes.USetProperty);
            //    NativeReflection.CachedTypeInfo CallMe6_FunctionInfo = typeInfo.GetFunction("CallMe6");
            //    CallMe6_FunctionAddress = CallMe6_FunctionInfo.Address;
            //    CallMe6_ParamsSize = CallMe6_FunctionInfo.Size;
            //    CallMe6_test_IsValid = CallMe6_FunctionInfo.GetPropertyOffsetAndValidate("test", out CallMe6_test_Offset, Classes.UStructProperty);
            //    CallMe6_test2_IsValid = CallMe6_FunctionInfo.GetPropertyOffsetAndValidate("test2", out CallMe6_test2_Offset, Classes.UStructProperty);
            //    CallMe6_IsValid = CallMe6_FunctionInfo.Exists && CallMe6_test_IsValid && CallMe6_test2_IsValid;
            //    CallMe6_FunctionInfo.LogIsValid(CallMe6_IsValid);
            //    NativeReflection.CachedTypeInfo CallMe5_FunctionInfo = typeInfo.GetFunction("CallMe5");
            //    CallMe5_FunctionAddress = CallMe5_FunctionInfo.Address;
            //    CallMe5_ParamsSize = CallMe5_FunctionInfo.Size;
            //    CallMe5_a1_IsValid = CallMe5_FunctionInfo.GetPropertyRefOffsetAndValidate("a1", ref CallMe5_a1_PropertyAddress, out CallMe5_a1_Offset, Classes.UArrayProperty);
            //    CallMe5_a2_IsValid = CallMe5_FunctionInfo.GetPropertyRefOffsetAndValidate("a2", ref CallMe5_a2_PropertyAddress, out CallMe5_a2_Offset, Classes.UArrayProperty);
            //    CallMe5_IsValid = CallMe5_FunctionInfo.Exists && CallMe5_a1_IsValid && CallMe5_a2_IsValid;
            //    CallMe5_FunctionInfo.LogIsValid(CallMe5_IsValid);
            //    NativeReflection.CachedTypeInfo CallMe4_FunctionInfo = typeInfo.GetFunction("CallMe4");
            //    CallMe4_FunctionAddress = CallMe4_FunctionInfo.Address;
            //    CallMe4_ParamsSize = CallMe4_FunctionInfo.Size;
            //    CallMe4_ReturnValue_IsValid = CallMe4_FunctionInfo.GetPropertyRefOffsetAndValidate("ReturnValue", ref CallMe4_ReturnValue_PropertyAddress, out CallMe4_ReturnValue_Offset, Classes.UArrayProperty);
            //    CallMe4_IsValid = CallMe4_FunctionInfo.Exists && CallMe4_ReturnValue_IsValid;
            //    CallMe4_FunctionInfo.LogIsValid(CallMe4_IsValid);
            //    NativeReflection.CachedTypeInfo CallMe3_FunctionInfo = typeInfo.GetFunction("CallMe3");
            //    CallMe3_FunctionAddress = CallMe3_FunctionInfo.Address;
            //    CallMe3_ParamsSize = CallMe3_FunctionInfo.Size;
            //    CallMe3_ReturnValue_IsValid = CallMe3_FunctionInfo.GetPropertyRefOffsetAndValidate("ReturnValue", ref CallMe3_ReturnValue_PropertyAddress, out CallMe3_ReturnValue_Offset, Classes.UArrayProperty);
            //    CallMe3_IsValid = CallMe3_FunctionInfo.Exists && CallMe3_ReturnValue_IsValid;
            //    CallMe3_FunctionInfo.LogIsValid(CallMe3_IsValid);
            //    NativeReflection.CachedTypeInfo CallMe2_FunctionInfo = typeInfo.GetFunction("CallMe2");
            //    CallMe2_FunctionAddress = CallMe2_FunctionInfo.Address;
            //    CallMe2_ParamsSize = CallMe2_FunctionInfo.Size;
            //    CallMe2_IsValid = CallMe2_FunctionInfo.Exists;
            //    CallMe2_FunctionInfo.LogIsValid(CallMe2_IsValid);
            //    NativeReflection.CachedTypeInfo CallMe_FunctionInfo = typeInfo.GetFunction("CallMe");
            //    CallMe_FunctionAddress = CallMe_FunctionInfo.Address;
            //    CallMe_ParamsSize = CallMe_FunctionInfo.Size;
            //    CallMe_IsValid = CallMe_FunctionInfo.Exists;
            //    CallMe_FunctionInfo.LogIsValid(CallMe_IsValid);
            //}
            //else
            {
                IntPtr classAddress = NativeReflection.GetClass("/Script/USharp.BPTest1");
                NativeReflectionCached.GetPropertyRef(ref MyMap_PropertyAddress, classAddress, "MyMap");
                MyMap_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "MyMap");
                MyMap_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "MyMap", Classes.UMapProperty);
                NativeReflectionCached.GetPropertyRef(ref MySet_PropertyAddress, classAddress, "MySet");
                MySet_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "MySet");
                MySet_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "MySet", Classes.USetProperty);
                CallMe6_FunctionAddress = NativeReflectionCached.GetFunction(classAddress, "CallMe6");
                CallMe6_ParamsSize = NativeReflection.GetFunctionParamsSize(CallMe6_FunctionAddress);
                CallMe6_test_Offset = NativeReflectionCached.GetPropertyOffset(CallMe6_FunctionAddress, "test");
                CallMe6_test_IsValid = NativeReflectionCached.ValidatePropertyClass(CallMe6_FunctionAddress, "test", Classes.UStructProperty);
                CallMe6_test2_Offset = NativeReflectionCached.GetPropertyOffset(CallMe6_FunctionAddress, "test2");
                CallMe6_test2_IsValid = NativeReflectionCached.ValidatePropertyClass(CallMe6_FunctionAddress, "test2", Classes.UStructProperty);
                CallMe6_IsValid = CallMe6_FunctionAddress != IntPtr.Zero && CallMe6_test_IsValid && CallMe6_test2_IsValid;
                NativeReflection.LogFunctionIsValid("/Script/USharp.BPTest1:CallMe6", CallMe6_IsValid);
                CallMe5_FunctionAddress = NativeReflectionCached.GetFunction(classAddress, "CallMe5");
                CallMe5_ParamsSize = NativeReflection.GetFunctionParamsSize(CallMe5_FunctionAddress);
                NativeReflectionCached.GetPropertyRef(ref CallMe5_a1_PropertyAddress, CallMe5_FunctionAddress, "a1");
                CallMe5_a1_Offset = NativeReflectionCached.GetPropertyOffset(CallMe5_FunctionAddress, "a1");
                CallMe5_a1_IsValid = NativeReflectionCached.ValidatePropertyClass(CallMe5_FunctionAddress, "a1", Classes.UArrayProperty);
                NativeReflectionCached.GetPropertyRef(ref CallMe5_a2_PropertyAddress, CallMe5_FunctionAddress, "a2");
                CallMe5_a2_Offset = NativeReflectionCached.GetPropertyOffset(CallMe5_FunctionAddress, "a2");
                CallMe5_a2_IsValid = NativeReflectionCached.ValidatePropertyClass(CallMe5_FunctionAddress, "a2", Classes.UArrayProperty);
                CallMe5_IsValid = CallMe5_FunctionAddress != IntPtr.Zero && CallMe5_a1_IsValid && CallMe5_a2_IsValid;
                NativeReflection.LogFunctionIsValid("/Script/USharp.BPTest1:CallMe5", CallMe5_IsValid);
                CallMe4_FunctionAddress = NativeReflectionCached.GetFunction(classAddress, "CallMe4");
                CallMe4_ParamsSize = NativeReflection.GetFunctionParamsSize(CallMe4_FunctionAddress);
                NativeReflectionCached.GetPropertyRef(ref CallMe4_ReturnValue_PropertyAddress, CallMe4_FunctionAddress, "ReturnValue");
                CallMe4_ReturnValue_Offset = NativeReflectionCached.GetPropertyOffset(CallMe4_FunctionAddress, "ReturnValue");
                CallMe4_ReturnValue_IsValid = NativeReflectionCached.ValidatePropertyClass(CallMe4_FunctionAddress, "ReturnValue", Classes.UArrayProperty);
                CallMe4_IsValid = CallMe4_FunctionAddress != IntPtr.Zero && CallMe4_ReturnValue_IsValid;
                NativeReflection.LogFunctionIsValid("/Script/USharp.BPTest1:CallMe4", CallMe4_IsValid);
                CallMe3_FunctionAddress = NativeReflectionCached.GetFunction(classAddress, "CallMe3");
                CallMe3_ParamsSize = NativeReflection.GetFunctionParamsSize(CallMe3_FunctionAddress);
                NativeReflectionCached.GetPropertyRef(ref CallMe3_ReturnValue_PropertyAddress, CallMe3_FunctionAddress, "ReturnValue");
                CallMe3_ReturnValue_Offset = NativeReflectionCached.GetPropertyOffset(CallMe3_FunctionAddress, "ReturnValue");
                CallMe3_ReturnValue_IsValid = NativeReflectionCached.ValidatePropertyClass(CallMe3_FunctionAddress, "ReturnValue", Classes.UArrayProperty);
                CallMe3_IsValid = CallMe3_FunctionAddress != IntPtr.Zero && CallMe3_ReturnValue_IsValid;
                NativeReflection.LogFunctionIsValid("/Script/USharp.BPTest1:CallMe3", CallMe3_IsValid);
                CallMe2_FunctionAddress = NativeReflectionCached.GetFunction(classAddress, "CallMe2");
                CallMe2_ParamsSize = NativeReflection.GetFunctionParamsSize(CallMe2_FunctionAddress);
                CallMe2_IsValid = CallMe2_FunctionAddress != IntPtr.Zero;
                NativeReflection.LogFunctionIsValid("/Script/USharp.BPTest1:CallMe2", CallMe2_IsValid);
                CallMe_FunctionAddress = NativeReflectionCached.GetFunction(classAddress, "CallMe");
                CallMe_ParamsSize = NativeReflection.GetFunctionParamsSize(CallMe_FunctionAddress);
                CallMe_IsValid = CallMe_FunctionAddress != IntPtr.Zero;
                NativeReflection.LogFunctionIsValid("/Script/USharp.BPTest1:CallMe", CallMe_IsValid);
            }
        }
    }
}

[UMetaPath("/Script/USharp.MyStructCustomCtor", "USharp", UnrealModuleType.EnginePlugin)]
public struct FMyStructCustomCtor
{
    static bool MyValue_IsValid;
    static int MyValue_Offset;
    [UMetaPath("/Script/USharp.MyStructCustomCtor:MyValue")]
    //public int MyValue;
    public string MyValue;

    static bool FMyStructCustomCtor_IsValid;
    public static int StructSize { get; private set; }

    public FMyStructCustomCtor Copy()
    {
        FMyStructCustomCtor result = this;
        return result;
    }

    public static FMyStructCustomCtor FromNative(IntPtr nativeBuffer)
    {
        return new FMyStructCustomCtor(nativeBuffer);
    }

    public static void ToNative(IntPtr nativeBuffer, FMyStructCustomCtor value)
    {
        value.ToNative(nativeBuffer);
    }

    public static FMyStructCustomCtor FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, UObject owner)
    {
        return new FMyStructCustomCtor(nativeBuffer + (arrayIndex * StructSize));
    }

    public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, UObject owner, FMyStructCustomCtor value)
    {
        value.ToNative(nativeBuffer + (arrayIndex * StructSize));
    }

    public void ToNative(IntPtr nativeStruct)
    {
        if (!FMyStructCustomCtor_IsValid)
        {
            NativeReflection.LogInvalidStructAccessed("/Script/USharp.MyStructCustomCtor");
            return;
        }
        //BlittableTypeMarshaler<int>.ToNative(IntPtr.Add(nativeStruct, MyValue_Offset), MyValue);
        FStringMarshaler.ToNative(IntPtr.Add(nativeStruct, MyValue_Offset), MyValue);
    }

    public FMyStructCustomCtor(IntPtr nativeStruct)
    {
        if (!FMyStructCustomCtor_IsValid)
        {
            NativeReflection.LogInvalidStructAccessed("/Script/USharp.MyStructCustomCtor");
            MyValue = FStringMarshaler.DefaultString;
            //MyValue = 0;
            return;
        }
        MyValue = FStringMarshaler.FromNative(IntPtr.Add(nativeStruct, MyValue_Offset));
        //MyValue = BlittableTypeMarshaler<int>.FromNative(IntPtr.Add(nativeStruct, MyValue_Offset));
    }

    static FMyStructCustomCtor()
    {
        if (UnrealTypes.CanLazyLoadNativeType(typeof(FMyStructCustomCtor)))
        {
            LoadNativeType();
        }
        UnrealTypes.OnCCtorCalled(typeof(FMyStructCustomCtor));
    }

    static void LoadNativeType()
    {
        //if (NativeReflection.CachedTypeInfo.Enabled)
        //{
        //    NativeReflection.CachedTypeInfo typeInfo = NativeReflection.CachedTypeInfo.BuildStruct("/Script/USharp.MyStructCustomCtor");
        //    StructSize = typeInfo.Size;
        //    MyValue_IsValid = typeInfo.GetPropertyOffsetAndValidate("MyValue", out MyValue_Offset, Classes.UStrProperty);
        //    FMyStructCustomCtor_IsValid = typeInfo.Exists && MyValue_IsValid;
        //    typeInfo.LogIsValid(FMyStructCustomCtor_IsValid);
        //}
        //else
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/USharp.MyStructCustomCtor");
            StructSize = NativeReflection.GetStructSize(classAddress);
            MyValue_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "MyValue");
            MyValue_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "MyValue", Classes.UStrProperty);
            //MyValue_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "MyValue", Classes.UIntProperty);
            FMyStructCustomCtor_IsValid = classAddress != IntPtr.Zero && MyValue_IsValid;
            NativeReflection.LogStructIsValid("/Script/USharp.MyStructCustomCtor", FMyStructCustomCtor_IsValid);
        }
    }
}

/*[UMetaPath("/Game/Pong/Test/NativeWrapper.NativeWrapper", "BlueprintTest", UnrealModuleType.Game)]
public struct FNativeWrapper
{
    static bool Test_IsValid;
    static int Test_Offset;
    [UMetaPath("/Game/Pong/Test/NativeWrapper.NativeWrapper:Test_2_EE2961724F89C06C4589DA8F9D0EFB2C")]
    public FMyStructCustomCtor Test;

    static bool FNativeWrapper_IsValid;
    public static int StructSize { get; private set; }

    public FNativeWrapper Copy()
    {
        FNativeWrapper result = this;
        return result;
    }

    public static FNativeWrapper FromNative(IntPtr nativeBuffer)
    {
        return new FNativeWrapper(nativeBuffer);
    }

    public static void ToNative(IntPtr nativeBuffer, FNativeWrapper value)
    {
        value.ToNative(nativeBuffer);
    }

    public static FNativeWrapper FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, UObject owner)
    {
        return new FNativeWrapper(nativeBuffer + (arrayIndex * StructSize));
    }

    public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, UObject owner, FNativeWrapper value)
    {
        value.ToNative(nativeBuffer + (arrayIndex * StructSize));
    }

    public void ToNative(IntPtr nativeStruct)
    {
        if (!FNativeWrapper_IsValid)
        {
            NativeReflection.LogInvalidStructAccessed("/Game/Pong/Test/NativeWrapper.NativeWrapper");
            return;
        }
        FMyStructCustomCtor.ToNative(IntPtr.Add(nativeStruct, Test_Offset), Test);
    }

    public FNativeWrapper(IntPtr nativeStruct)
    {
        if (!FNativeWrapper_IsValid)
        {
            NativeReflection.LogInvalidStructAccessed("/Game/Pong/Test/NativeWrapper.NativeWrapper");
            Test = default(FMyStructCustomCtor);
            return;
        }
        Test = FMyStructCustomCtor.FromNative(IntPtr.Add(nativeStruct, Test_Offset));
    }

    static void LoadNativeType()
    {
        IntPtr classAddress = NativeReflection.GetStruct("/Game/Pong/Test/NativeWrapper.NativeWrapper");
        StructSize = NativeReflection.GetStructSize(classAddress);
        Test_Offset = NativeReflection.GetPropertyOffset(classAddress, "Test_2_EE2961724F89C06C4589DA8F9D0EFB2C");
        Test_IsValid = NativeReflection.ValidatePropertyClass(classAddress, "Test_2_EE2961724F89C06C4589DA8F9D0EFB2C", Classes.UStructProperty);
        FNativeWrapper_IsValid = classAddress != IntPtr.Zero && Test_IsValid;
        NativeReflection.LogStructIsValid("/Game/Pong/Test/NativeWrapper.NativeWrapper", FNativeWrapper_IsValid);
    }
}*/

/*using System;
using UnrealEngine.CoreUObject;
using UnrealEngine.Runtime;

namespace UnrealEngine.Runtime
{
    [UMetaPath("/Script/USharp.BPTest1", "USharp", UnrealModuleType.EnginePlugin)]
    public class UBPTest1 : UObject
    {
        static bool MyMap_IsValid;
        static UFieldAddress MyMap_PropertyAddress;
        static int MyMap_Offset;
        TMapReadWriteMarshaler<string, int> MyMap_MarshalerCached;
        [UMetaPath("/Script/USharp.BPTest1:MyMap")]
        public TMapReadWrite<string, int> MyMap
        {
            get
            {
                CheckDestroyed();
                if (!MyMap_IsValid)
                {
                    NativeReflection.LogInvalidPropertyAccessed("/Script/USharp.BPTest1:MyMap");
                    return default(TMapReadWrite<string, int>);
                }
                if (MyMap_MarshalerCached == null)
                {
                    MyMap_MarshalerCached = new TMapReadWriteMarshaler<string, int>(1, MyMap_PropertyAddress, CachedMarshalingDelegates<string, FStringMarshaler>.FromNative, CachedMarshalingDelegates<string, FStringMarshaler>.ToNative, CachedMarshalingDelegates<int, BlittableTypeMarshaler<int>>.FromNative, CachedMarshalingDelegates<int, BlittableTypeMarshaler<int>>.ToNative);
                }
                return MyMap_MarshalerCached.FromNative(IntPtr.Add(Address, MyMap_Offset));
            }
        }

        static bool MySet_IsValid;
        static UFieldAddress MySet_PropertyAddress;
        static int MySet_Offset;
        TSetReadWriteMarshaler<string> MySet_MarshalerCached;
        [UMetaPath("/Script/USharp.BPTest1:MySet")]
        public TSetReadWrite<string> MySet
        {
            get
            {
                CheckDestroyed();
                if (!MySet_IsValid)
                {
                    NativeReflection.LogInvalidPropertyAccessed("/Script/USharp.BPTest1:MySet");
                    return default(TSetReadWrite<string>);
                }
                if (MySet_MarshalerCached == null)
                {
                    MySet_MarshalerCached = new TSetReadWriteMarshaler<string>(1, MySet_PropertyAddress, CachedMarshalingDelegates<string, FStringMarshaler>.FromNative, CachedMarshalingDelegates<string, FStringMarshaler>.ToNative);
                }
                return MySet_MarshalerCached.FromNative(IntPtr.Add(Address, MySet_Offset));
            }
        }

        static bool CallMe2_IsValid;
        static IntPtr CallMe2_FunctionAddress;
        static int CallMe2_ParamsSize;
        [UMetaPath("/Script/USharp.BPTest1:CallMe2")]
        public void CallMe2()
        {
            CheckDestroyed();
            if (!CallMe2_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.BPTest1:CallMe2");
                return;
            }
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[CallMe2_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);

                NativeReflection.InvokeFunction(Address, CallMe2_FunctionAddress, ParamsBuffer, CallMe2_ParamsSize);
                NativeReflection.InvokeFunction_DestroyAll(CallMe2_FunctionAddress, ParamsBuffer);
            }
        }

        static bool CallMe_IsValid;
        IntPtr CallMe_InstanceFunctionAddress;
        static IntPtr CallMe_FunctionAddress;
        static int CallMe_ParamsSize;
        [UMetaPath("/Script/USharp.BPTest1:CallMe")]
        [UFunction(EFunctionFlags.BlueprintEvent)]
        public virtual void CallMe()
        {
            CheckDestroyed();
            if (!CallMe_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.BPTest1:CallMe");
                return;
            }
            if (CallMe_InstanceFunctionAddress == IntPtr.Zero)
            {
                CallMe_InstanceFunctionAddress = NativeReflection.GetFunctionFromInstance(Address, "CallMe");
            }
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[CallMe_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);

                NativeReflection.InvokeFunction(Address, CallMe_InstanceFunctionAddress, ParamsBuffer, CallMe_ParamsSize);
                NativeReflection.InvokeFunction_DestroyAll(CallMe_InstanceFunctionAddress, ParamsBuffer);
            }
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual void CallMe_Implementation()
        {
            CheckDestroyed();
            if (!CallMe_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.BPTest1:CallMe");
                return;
            }
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[CallMe_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);

                NativeReflection.InvokeFunction(Address, CallMe_FunctionAddress, ParamsBuffer, CallMe_ParamsSize);
                NativeReflection.InvokeFunction_DestroyAll(CallMe_FunctionAddress, ParamsBuffer);
            }
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetClass("/Script/USharp.BPTest1");
            NativeReflection.GetPropertyRef(ref MyMap_PropertyAddress, classAddress, "MyMap");
            MyMap_Offset = NativeReflection.GetPropertyOffset(classAddress, "MyMap");
            MyMap_IsValid = NativeReflection.ValidatePropertyClass(classAddress, "MyMap", Classes.UMapProperty);
            NativeReflection.GetPropertyRef(ref MySet_PropertyAddress, classAddress, "MySet");
            MySet_Offset = NativeReflection.GetPropertyOffset(classAddress, "MySet");
            MySet_IsValid = NativeReflection.ValidatePropertyClass(classAddress, "MySet", Classes.USetProperty);
            CallMe2_FunctionAddress = NativeReflection.GetFunction(classAddress, "CallMe2");
            CallMe2_ParamsSize = NativeReflection.GetFunctionParamsSize(CallMe2_FunctionAddress);
            CallMe2_IsValid = CallMe2_FunctionAddress != IntPtr.Zero;
            NativeReflection.LogFunctionIsValid("/Script/USharp.BPTest1:CallMe2", CallMe2_IsValid);
            CallMe_FunctionAddress = NativeReflection.GetFunction(classAddress, "CallMe");
            CallMe_ParamsSize = NativeReflection.GetFunctionParamsSize(CallMe_FunctionAddress);
            CallMe_IsValid = CallMe_FunctionAddress != IntPtr.Zero;
            NativeReflection.LogFunctionIsValid("/Script/USharp.BPTest1:CallMe", CallMe_IsValid);
        }
    }
}*/

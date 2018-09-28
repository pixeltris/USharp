using System;
using UnrealEngine.Runtime;

namespace UnrealEngine.Plugins.USharp
{
    [UMetaPath("/Script/USharp.SomeDelegate__DelegateSignature")]
    public class FSomeDelegate : FDelegate<FSomeDelegate.Signature>
    {
        public delegate void Signature();

        public override Signature GetInvoker()
        {
            return Invoker;
        }

        static bool SomeDelegate__DelegateSignature_IsValid;
        static IntPtr SomeDelegate__DelegateSignature_FunctionAddress;
        static int SomeDelegate__DelegateSignature_ParamsSize;

        static FSomeDelegate()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FSomeDelegate)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FSomeDelegate));
        }

        static void LoadNativeType()
        {
            //if (NativeReflection.CachedTypeInfo.Enabled)
            //{
            //    NativeReflection.CachedTypeInfo typeInfo = NativeReflection.CachedTypeInfo.BuildFunction("/Script/USharp.SomeDelegate__DelegateSignature");
            //    SomeDelegate__DelegateSignature_FunctionAddress = typeInfo.Address;
            //    SomeDelegate__DelegateSignature_ParamsSize = typeInfo.Size;
            //    SomeDelegate__DelegateSignature_IsValid = typeInfo.Exists;
            //    typeInfo.LogIsValid(SomeDelegate__DelegateSignature_IsValid);
            //}
            //else
            {
                SomeDelegate__DelegateSignature_FunctionAddress = NativeReflection.GetFunction("/Script/USharp.SomeDelegate__DelegateSignature");
                SomeDelegate__DelegateSignature_ParamsSize = NativeReflection.GetFunctionParamsSize(SomeDelegate__DelegateSignature_FunctionAddress);
                SomeDelegate__DelegateSignature_IsValid = SomeDelegate__DelegateSignature_FunctionAddress != IntPtr.Zero;
                NativeReflection.LogFunctionIsValid("/Script/USharp.SomeDelegate__DelegateSignature", SomeDelegate__DelegateSignature_IsValid);
            }
        }

        private void Invoker()
        {
            if (!SomeDelegate__DelegateSignature_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.SomeDelegate__DelegateSignature");
                return;
            }
            if (IsBound)
            {
                unsafe
                {
                    byte* ParamsBufferAllocation = stackalloc byte[SomeDelegate__DelegateSignature_ParamsSize];
                    IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                    FMemory.Memzero(ParamsBuffer, SomeDelegate__DelegateSignature_ParamsSize);

                    ProcessDelegate(ParamsBuffer);
                }
            }
        }
    }

    [UMetaPath("/Script/USharp.DelegateRefTest__DelegateSignature")]
    public class FDelegateRefTest : FDelegate<FDelegateRefTest.Signature>
    {
        public delegate void Signature(ref int SomeArgument);

        public override Signature GetInvoker()
        {
            return Invoker;
        }

        static bool DelegateRefTest__DelegateSignature_IsValid;
        static IntPtr DelegateRefTest__DelegateSignature_FunctionAddress;
        static int DelegateRefTest__DelegateSignature_ParamsSize;
        static bool DelegateRefTest__DelegateSignature_SomeArgument_IsValid;
        static UFieldAddress DelegateRefTest__DelegateSignature_SomeArgument_PropertyAddress;
        static int DelegateRefTest__DelegateSignature_SomeArgument_Offset;

        static FDelegateRefTest()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FDelegateRefTest)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FDelegateRefTest));
        }

        static void LoadNativeType()
        {
            //if (NativeReflection.CachedTypeInfo.Enabled)
            //{
            //    NativeReflection.CachedTypeInfo typeInfo = NativeReflection.CachedTypeInfo.BuildFunction("/Script/USharp.DelegateRefTest__DelegateSignature");
            //    DelegateRefTest__DelegateSignature_FunctionAddress = typeInfo.Address;
            //    DelegateRefTest__DelegateSignature_ParamsSize = typeInfo.Size;
            //    DelegateRefTest__DelegateSignature_SomeArgument_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("SomeArgument", ref DelegateRefTest__DelegateSignature_SomeArgument_PropertyAddress, out DelegateRefTest__DelegateSignature_SomeArgument_Offset, Classes.UIntProperty);
            //    DelegateRefTest__DelegateSignature_IsValid = typeInfo.Exists && DelegateRefTest__DelegateSignature_SomeArgument_IsValid;
            //    typeInfo.LogIsValid(DelegateRefTest__DelegateSignature_IsValid);
            //}
            //else
            {
                DelegateRefTest__DelegateSignature_FunctionAddress = NativeReflection.GetFunction("/Script/USharp.DelegateRefTest__DelegateSignature");
                DelegateRefTest__DelegateSignature_ParamsSize = NativeReflection.GetFunctionParamsSize(DelegateRefTest__DelegateSignature_FunctionAddress);
                NativeReflectionCached.GetPropertyRef(ref DelegateRefTest__DelegateSignature_SomeArgument_PropertyAddress, DelegateRefTest__DelegateSignature_FunctionAddress, "SomeArgument");
                DelegateRefTest__DelegateSignature_SomeArgument_Offset = NativeReflectionCached.GetPropertyOffset(DelegateRefTest__DelegateSignature_FunctionAddress, "SomeArgument");
                DelegateRefTest__DelegateSignature_SomeArgument_IsValid = NativeReflectionCached.ValidatePropertyClass(DelegateRefTest__DelegateSignature_FunctionAddress, "SomeArgument", Classes.UIntProperty);
                DelegateRefTest__DelegateSignature_IsValid = DelegateRefTest__DelegateSignature_FunctionAddress != IntPtr.Zero && DelegateRefTest__DelegateSignature_SomeArgument_IsValid;
                NativeReflection.LogFunctionIsValid("/Script/USharp.DelegateRefTest__DelegateSignature", DelegateRefTest__DelegateSignature_IsValid);
            }
        }

        private void Invoker(ref int SomeArgument)
        {
            if (!DelegateRefTest__DelegateSignature_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.DelegateRefTest__DelegateSignature");
                return;
            }
            if (IsBound)
            {
                unsafe
                {
                    byte* ParamsBufferAllocation = stackalloc byte[DelegateRefTest__DelegateSignature_ParamsSize];
                    IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                    FMemory.Memzero(ParamsBuffer, DelegateRefTest__DelegateSignature_ParamsSize);
                    BlittableTypeMarshaler<int>.ToNative(IntPtr.Add(ParamsBuffer, DelegateRefTest__DelegateSignature_SomeArgument_Offset), 0, DelegateRefTest__DelegateSignature_SomeArgument_PropertyAddress.Address, SomeArgument);

                    ProcessDelegate(ParamsBuffer);

                    SomeArgument = BlittableTypeMarshaler<int>.FromNative(IntPtr.Add(ParamsBuffer, DelegateRefTest__DelegateSignature_SomeArgument_Offset), 0, DelegateRefTest__DelegateSignature_SomeArgument_PropertyAddress.Address);
                }
            }
        }
    }

    [UMetaPath("/Script/USharp.ActorDelegateTest__DelegateSignature")]
    public class FActorDelegateTest : FDelegate<FActorDelegateTest.Signature>
    {
        public delegate void Signature(int SomeArgument);

        public override Signature GetInvoker()
        {
            return Invoker;
        }

        static bool ActorDelegateTest__DelegateSignature_IsValid;
        static IntPtr ActorDelegateTest__DelegateSignature_FunctionAddress;
        static int ActorDelegateTest__DelegateSignature_ParamsSize;
        static bool ActorDelegateTest__DelegateSignature_SomeArgument_IsValid;
        static UFieldAddress ActorDelegateTest__DelegateSignature_SomeArgument_PropertyAddress;
        static int ActorDelegateTest__DelegateSignature_SomeArgument_Offset;

        static FActorDelegateTest()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FActorDelegateTest)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FActorDelegateTest));
        }

        static void LoadNativeType()
        {
            //if (NativeReflection.CachedTypeInfo.Enabled)
            //{
            //    NativeReflection.CachedTypeInfo typeInfo = NativeReflection.CachedTypeInfo.BuildFunction("/Script/USharp.ActorDelegateTest__DelegateSignature");
            //    ActorDelegateTest__DelegateSignature_FunctionAddress = typeInfo.Address;
            //    ActorDelegateTest__DelegateSignature_ParamsSize = typeInfo.Size;
            //    ActorDelegateTest__DelegateSignature_SomeArgument_IsValid = typeInfo.GetPropertyRefOffsetAndValidate("SomeArgument", ref ActorDelegateTest__DelegateSignature_SomeArgument_PropertyAddress, out ActorDelegateTest__DelegateSignature_SomeArgument_Offset, Classes.UIntProperty);
            //    ActorDelegateTest__DelegateSignature_IsValid = typeInfo.Exists && ActorDelegateTest__DelegateSignature_SomeArgument_IsValid;
            //    typeInfo.LogIsValid(ActorDelegateTest__DelegateSignature_IsValid);
            //}
            //else
            {
                ActorDelegateTest__DelegateSignature_FunctionAddress = NativeReflection.GetFunction("/Script/USharp.ActorDelegateTest__DelegateSignature");
                ActorDelegateTest__DelegateSignature_ParamsSize = NativeReflection.GetFunctionParamsSize(ActorDelegateTest__DelegateSignature_FunctionAddress);
                NativeReflectionCached.GetPropertyRef(ref ActorDelegateTest__DelegateSignature_SomeArgument_PropertyAddress, ActorDelegateTest__DelegateSignature_FunctionAddress, "SomeArgument");
                ActorDelegateTest__DelegateSignature_SomeArgument_Offset = NativeReflectionCached.GetPropertyOffset(ActorDelegateTest__DelegateSignature_FunctionAddress, "SomeArgument");
                ActorDelegateTest__DelegateSignature_SomeArgument_IsValid = NativeReflectionCached.ValidatePropertyClass(ActorDelegateTest__DelegateSignature_FunctionAddress, "SomeArgument", Classes.UIntProperty);
                ActorDelegateTest__DelegateSignature_IsValid = ActorDelegateTest__DelegateSignature_FunctionAddress != IntPtr.Zero && ActorDelegateTest__DelegateSignature_SomeArgument_IsValid;
                NativeReflection.LogFunctionIsValid("/Script/USharp.ActorDelegateTest__DelegateSignature", ActorDelegateTest__DelegateSignature_IsValid);
            }
        }

        private void Invoker(int SomeArgument)
        {
            if (!ActorDelegateTest__DelegateSignature_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.ActorDelegateTest__DelegateSignature");
                return;
            }
            if (IsBound)
            {
                unsafe
                {
                    byte* ParamsBufferAllocation = stackalloc byte[ActorDelegateTest__DelegateSignature_ParamsSize];
                    IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                    FMemory.Memzero(ParamsBuffer, ActorDelegateTest__DelegateSignature_ParamsSize);
                    BlittableTypeMarshaler<int>.ToNative(IntPtr.Add(ParamsBuffer, ActorDelegateTest__DelegateSignature_SomeArgument_Offset), 0, ActorDelegateTest__DelegateSignature_SomeArgument_PropertyAddress.Address, SomeArgument);

                    ProcessDelegate(ParamsBuffer);
                }
            }
        }
    }
}

/*namespace UnrealEngine.Plugins.USharp
{
    [UMetaPath("/Script/USharp.SimpleObject", "USharp", UnrealModuleType.EnginePlugin)]
    public class USimpleObject : UObject
    {
        static bool MyDel_IsValid;
        static int MyDel_Offset;
        FDelegateRefTest MyDel_DelegateCached;
        [UMetaPath("/Script/USharp.SimpleObject:MyDel")]
        public FDelegateRefTest MyDel
        {
            get
            {
                CheckDestroyed();
                if (!MyDel_IsValid)
                {
                    NativeReflection.LogInvalidPropertyAccessed("/Script/USharp.SimpleObject:MyDel");
                    return new FDelegateRefTest();
                }
                if (MyDel_DelegateCached == null)
                {
                    MyDel_DelegateCached = new FDelegateRefTest();
                    MyDel_DelegateCached.SetAddress(IntPtr.Add(Address, MyDel_Offset));
                }
                return MyDel_DelegateCached;
            }
        }

        static bool BindMe_IsValid;
        static IntPtr BindMe_FunctionAddress;
        static int BindMe_ParamsSize;
        static bool BindMe_InOutArg_IsValid;
        static UFieldAddress BindMe_InOutArg_PropertyAddress;
        static int BindMe_InOutArg_Offset;
        [UMetaPath("/Script/USharp.SimpleObject:BindMe")]
        public void BindMe(ref int InOutArg)
        {
            CheckDestroyed();
            if (!BindMe_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.SimpleObject:BindMe");
                return;
            }
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[BindMe_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                FMemory.Memzero(ParamsBuffer, BindMe_ParamsSize);
                BlittableTypeMarshaler<int>.ToNative(IntPtr.Add(ParamsBuffer, BindMe_InOutArg_Offset), 0, BindMe_InOutArg_PropertyAddress.Address, InOutArg);

                NativeReflection.InvokeFunction(Address, BindMe_FunctionAddress, ParamsBuffer, BindMe_ParamsSize);

                InOutArg = BlittableTypeMarshaler<int>.FromNative(IntPtr.Add(ParamsBuffer, BindMe_InOutArg_Offset), 0, BindMe_InOutArg_PropertyAddress.Address);
            }
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetClass("/Script/USharp.SimpleObject");
            MyDel_Offset = NativeReflection.GetPropertyOffset(classAddress, "MyDel");
            MyDel_IsValid = NativeReflection.ValidatePropertyClass(classAddress, "MyDel", Classes.UDelegateProperty);
            BindMe_FunctionAddress = NativeReflection.GetFunction(classAddress, "BindMe");
            BindMe_ParamsSize = NativeReflection.GetFunctionParamsSize(BindMe_FunctionAddress);
            NativeReflection.GetPropertyRef(ref BindMe_InOutArg_PropertyAddress, BindMe_FunctionAddress, "InOutArg");
            BindMe_InOutArg_Offset = NativeReflection.GetPropertyOffset(BindMe_FunctionAddress, "InOutArg");
            BindMe_InOutArg_IsValid = NativeReflection.ValidatePropertyClass(BindMe_FunctionAddress, "InOutArg", Classes.UIntProperty);
            BindMe_IsValid = BindMe_FunctionAddress != IntPtr.Zero && BindMe_InOutArg_IsValid;
            NativeReflection.LogFunctionIsValid("/Script/USharp.SimpleObject:BindMe", BindMe_IsValid);
        }
    }
}*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /*public class AHelloWorld : UnrealEngine.Engine.AActor
{
    public sbyte TestMe { get; set; }

    public TSubclassOf<UnrealEngine.Engine.AActor> TestClass { get; set; }

    public List<string> MyArray { get; set; }
    public Dictionary<float, UObject> Map { get; set; }

    public BlueprintTest.Pong.ABPUtil_C MyObj { get; set; }

    public FMyStruct S { get; set; }

    //public BlueprintTest.Pong.ABPUtil_C.FMyTestDispatcher MyTestDispatcher { get; set; }

    [UFunction(EFunctionFlags.Net | EFunctionFlags.NetValidate)]
    public int TestFunc3(string some1, ref long some2, out string some3)
    {
        some3 = null;
        return 0;
    }

    //public virtual int TestFunc3_Implementation(string some1, ref long some2, out string some3)
    //{
    //    throw new NotImplementedException();
    //}

    public bool TestFunc3_Validate(string some1, ref long some2, out string some3)
    {
        some3 = null;
        return true;
    }

    [UFunction(EFunctionFlags.BlueprintEvent)]
    public int TestFunc(out string some1, ref long some2, List<int> incorrect, out BlueprintTest.Pong.ABPUtil_C.FMyTestDispatcher despatch)
    {
        some1 = null;
        despatch = null;
        return 0;
    }

    //public int TestFunc_Implementation(out string some1, ref long some2, List<int> incorrect, out BlueprintTest.Pong.ABPUtil_C.FMyTestDispatcher despatch)
    //{
    //    throw new NotImplementedException();
    //}

    public bool TestFunc_Validate(out string some1, ref long some2, List<int> incorrect, out BlueprintTest.Pong.ABPUtil_C.FMyTestDispatcher despatch)
    {
        some1 = null;
        despatch = null;
        return false;
    }

    public BlueprintTest.Pong.ABPUtil_C.FMyTestDispatcher TestFunc2(string some1, long some2, List<int> incorrect)
    {
        return null;
    }
}

public class FMyCustomDispatcher : FMulticastDelegate<FMyCustomDispatcher.Signature>
{
    public delegate int Signature(int param1, out string param2);
}

[UStruct]
public struct FMyStructBlittable
{
    public int MyVal;
    public float MyVal2;
}

[UStruct(Tooltip = "This is an example tooltip")]
public struct FMyStruct
{
    public List<int> MyList1;

    public List<string> MyList2;

    public string MyStr;

    public int MyInt;

    public EMyEnum MyE;

    public bool BoolVal;

    //public BlueprintTest.Pong.FBlueprintArrayTest Abc;
    public BlueprintTest.Pong.FBlueprintArrayTest Abc;
}

class UMyBPTest : UBPTest1
{
    public override void CallMe()
    {
        base.CallMe();
    }

    //protected override void CallMe_Implementation()
    //{
    //    base.CallMe_Implementation();
    //}
}

[UEnum]
public enum EMyEnum
{
    Val1,
    Val2,
    Val3
}*/

    public class FMyCustomDispatcher : FMulticastDelegate<FMyCustomDispatcher.Signature>
    {
        //public delegate int Signature(int param1, out string param2);
        //public delegate void Signature(int param1, ref FNewStruct param2);
        public delegate void Signature(int param1, EMyEnum objByRef);
    }

    [BlueprintType]//, NotBlueprintable]
    [UMeta(MDClass.HideFunctions, "HelloSharp2")]//, UMeta(MDClass.AutoCollapseCategories, "Hello")]
    [UClass]
    public class ASharpActor : Engine.AActor
    {
        public FMyCustomDispatcher Sig { get; set; }

        public void MySigTestFunc(ref FMyCustomDispatcher sig)
        {
            if (sig != null)
            {
                FMessage.Log("Hello sig! " + sig.Count);
            }
        }

        [/*UProperty(Flags = PropFlags.AdvancedDisplay), */EditAnywhere, BlueprintReadWrite, Category("Hello|World")]
        public int TestVar1 { get; set; }

        [BlueprintCallable]
        public void HelloSharp1(string a1)
        {
            FMessage.Log("HelloSharp1 " + a1);
        }

        [BlueprintCallable]
        public void HelloSharp2(string a1)
        {
            FMessage.Log("HelloSharp2 " + a1);
        }

        [BlueprintCallable]
        public void TickMe1()
        {
            FMessage.Log("HelloTickMe1");
        }

        [BlueprintCallable]
        public void TickMe2()
        {
            FMessage.Log("HelloTickMe2");
        }
    }

    [UMeta(MDClass.HideFunctions, "HelloSharp2")]
    [UMeta(MDClass.AutoExpandCategories, "Hello|World")]
    public class AMakeVisible : ASharpActor
    {
        [EditAnywhere, BlueprintReadWrite]
        public int Variable123 { get; set; }

        public int Variable1234 { get; set; }
    }

    [UEnum]
    //[BlueprintType]
    public enum EMyEnum : byte
    {
        Val1,
        Val2,
        [DisplayName("Custom enum display name")]
        Val3,
        Val5 = 99
    }

    [UStruct]
    public struct FNewStruct
    {
        public int One;
        public int Two;
        public int Three;
        public EMyEnum Four;
        public FMyCustomDispatcher Sig;
    }

    [UStruct]
    public struct FNewRuntimeDefinedStruct
    {
        public int One;
        public int Two;
        public FNewStruct Three;
    }

    [UStruct]
    public struct FMySharpStruct
    {
        //public int[] MyArray;

        //public TFixedSizeArray<int> FixedSizeArray;

        public int MyVal;
        public BlueprintTest.Pong.Test.FTestSS MyVal2;
    }

    //[UStruct]
    //class SomeInvalidClass : StructAsClass
    //{
    //    [UProperty(FixedSizeArrayDim = 10)]
    //    public TFixedSizeArray<int> FixedSizeArray { get; set; }
    //}

    /*[UStruct]
    public struct FNewStruct
    {
        public int One;
        public FTestLargeStruct Two;
    }

    [UMetaPath("/Script/USharp.TestLargeStruct", "USharp", UnrealModuleType.EnginePlugin)]
    [UStruct]
    public class FTestLargeStruct : StructAsClass
    {
        static bool Val1_IsValid;
        static int Val1_Offset;
        [UMetaPath("/Script/USharp.TestLargeStruct:Val1")]
        public int Val1
        {
            get
            {
                CheckDestroyed();
                if (!Val1_IsValid)
                {
                    NativeReflection.LogInvalidPropertyAccessed("/Script/USharp.TestLargeStruct:Val1");
                    return default(int);
                }
                return BlittableTypeMarshaler<int>.FromNative(IntPtr.Add(Address, Val1_Offset));
            }
            set
            {
                CheckDestroyed();
                if (!Val1_IsValid)
                {
                    NativeReflection.LogInvalidPropertyAccessed("/Script/USharp.TestLargeStruct:Val1");
                    return;
                }
                BlittableTypeMarshaler<int>.ToNative(IntPtr.Add(Address, Val1_Offset), value);
            }
        }

        static bool Val2_IsValid;
        static int Val2_Offset;
        [UMetaPath("/Script/USharp.TestLargeStruct:Val2")]
        public float Val2
        {
            get
            {
                CheckDestroyed();
                if (!Val2_IsValid)
                {
                    NativeReflection.LogInvalidPropertyAccessed("/Script/USharp.TestLargeStruct:Val2");
                    return default(float);
                }
                return BlittableTypeMarshaler<float>.FromNative(IntPtr.Add(Address, Val2_Offset));
            }
            set
            {
                CheckDestroyed();
                if (!Val2_IsValid)
                {
                    NativeReflection.LogInvalidPropertyAccessed("/Script/USharp.TestLargeStruct:Val2");
                    return;
                }
                BlittableTypeMarshaler<float>.ToNative(IntPtr.Add(Address, Val2_Offset), value);
            }
        }

        static bool MyValue_IsValid;
        static int MyValue_Offset;
        [UMetaPath("/Script/USharp.TestLargeStruct:MyValue")]
        public string MyValue
        {
            get
            {
                CheckDestroyed();
                if (!MyValue_IsValid)
                {
                    NativeReflection.LogInvalidPropertyAccessed("/Script/USharp.TestLargeStruct:MyValue");
                    return FStringMarshaler.DefaultString;
                }
                return FStringMarshaler.FromNative(IntPtr.Add(Address, MyValue_Offset));
            }
            set
            {
                CheckDestroyed();
                if (!MyValue_IsValid)
                {
                    NativeReflection.LogInvalidPropertyAccessed("/Script/USharp.TestLargeStruct:MyValue");
                    return;
                }
                FStringMarshaler.ToNative(IntPtr.Add(Address, MyValue_Offset), value);
            }
        }

        static bool FTestLargeStruct_IsValid;
        static IntPtr FTestLargeStruct_StructAddress;

        protected override IntPtr GetStructAddress()
        {
            return FTestLargeStruct_StructAddress;
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/USharp.TestLargeStruct");
            FTestLargeStruct_StructAddress = classAddress;
            Val1_Offset = NativeReflection.GetPropertyOffset(classAddress, "Val1");
            Val1_IsValid = NativeReflection.ValidatePropertyClass(classAddress, "Val1", Classes.UIntProperty);
            Val2_Offset = NativeReflection.GetPropertyOffset(classAddress, "Val2");
            Val2_IsValid = NativeReflection.ValidatePropertyClass(classAddress, "Val2", Classes.UFloatProperty);
            MyValue_Offset = NativeReflection.GetPropertyOffset(classAddress, "MyValue");
            MyValue_IsValid = NativeReflection.ValidatePropertyClass(classAddress, "MyValue", Classes.UStrProperty);
            FTestLargeStruct_IsValid = classAddress != IntPtr.Zero && Val1_IsValid && Val2_IsValid && MyValue_IsValid;
            NativeReflection.LogStructIsValid("/Script/USharp.TestLargeStruct", FTestLargeStruct_IsValid);
        }
    }*/

    //[UStruct]
    //public class MyCustomStructAsClass : StructAsClass
    //{
    //    public int Val1 { get; set; }
    //    public int Val2 { get; set; }
    //    public string Val3 { get; set; }
    //    public List<string> MyList { get; set; }
    //
    //    [UProperty(FixedSizeArrayDim = 10)]
    //    public TFixedSizeArray<int> MyFixedSizeArray { get; set; }
    //}
    //
    //public class ClassWithStructAsClass : UObject
    //{
    //    public MyCustomStructAsClass Val1 { get; set; }
    //}

    public class UTestClassOverride : UBPTest1
    {
        [UProperty(FixedSizeArrayDim = 10)]
        public TFixedSizeArray<UObject> MyArrayTest { get; set; }

        protected override void CallMe_Implementation()
        {
            FMessage.Log("UTestClassOverride:CallMe");
            base.CallMe_Implementation();
        }

        [UFunction, BlueprintImplementedEvent]
        public int NewVirtualMethod(string val)
        {
            return 1;
        }

        protected virtual int NewVirtualMethod_Implementation(string val)
        {
            FMessage.Log("UTestClassOverride:NewVirtualMethod " + val);
            //return "1";
            return 1;
        }
    }

    public class UMyClass : UObject
    {
        public int MyVal { get; set; }
        public string MyVal2 { get; set; }
        public List<float> MyVal3 { get; set; }
        public HashSet<string> MyStringSet { get; set; }
        public EMyEnum MyE { get; set; }

        public string MyTestFunc1(ref int param1, EMyEnum param2, ref List<string> stringTest)
        {
            param1++;
            //stringTest.Add("cs_one");
            //stringTest.Add("cs_two");
            //stringTest.Add("cs_three");
            FMessage.Log("hello from MyTestFunc1: " + param1 + " " + param2);
            //foreach (string str in stringTest)
            //{
            //    FMessage.Log("stringTest: " + str);
            //}
            return "hello world";
        }

        [BlueprintCallable]
        public string MyTestFunc2(ref string inVal, out string stringTest)//int param1, BlueprintTest.Pong.Test.FTestSS s)
        {
            inVal = "hello123";
            //FMessage.Log("hello from MyTestFunc2: " + param1);
            stringTest = "MyOutStr";
            return "result val";
        }

        public void MyTestFunc3(FMyStructCustomCtor myStruct)
        {
            FMessage.Log("struct str: " + myStruct.MyValue);
        }

        public void MyTestFunc4(ref List<FMyStructCustomCtor> myStruct)
        {
            FMessage.Log("count: " + myStruct.Count);
            for (int i = 0; i < myStruct.Count; i++)
            {
                FMessage.Log("val: " + myStruct[i].MyValue);
                myStruct[i] = new FMyStructCustomCtor() { MyValue = "-" };
            }
        }

        public override void OnAssemblyUnload()
        {
            FMessage.Log("UMyClass.OnAssemblyUnload");
        }

        public override void OnAssemblyReload()
        {
            FMessage.Log("UMyClass.OnAssemblyReload");
        }
    }

    class TestCallSelf
    {
        static IntPtr StringTest_FunctionAddress;
        static int StringTest_ParamsSize;
        static int StringTest_inVal_Offset;
        static int StringTest_stringTest_Offset;
        static int StringTest_ParamResult_Offset;

        static IntPtr Test2_FunctionAddress;
        static int Test2_ParamsSize;
        static int Test2_Param1_Offset;

        public static void Call()
        {
            IntPtr classAddress = NativeReflection.GetClass("/Script/UnrealEngine_Runtime.MyClass");
            StringTest_FunctionAddress = NativeReflection.GetFunction(classAddress, "MyTestFunc2");
            StringTest_ParamsSize = NativeReflection.GetFunctionParamsSize(StringTest_FunctionAddress);
            StringTest_inVal_Offset = NativeReflection.GetPropertyOffset(StringTest_FunctionAddress, "inVal");
            StringTest_stringTest_Offset = NativeReflection.GetPropertyOffset(StringTest_FunctionAddress, "stringTest");
            StringTest_ParamResult_Offset = NativeReflection.GetPropertyOffset(StringTest_FunctionAddress, "__return");

            UMyClass myClass = UObject.NewObject<UMyClass>();
            string refVal = "inref";
            string arg = "hello world";

            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[StringTest_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                FStringMarshaler.ToNative(IntPtr.Add(ParamsBuffer, StringTest_inVal_Offset), refVal);
                //FStringMarshaler.ToNative(IntPtr.Add(ParamsBuffer, StringTest_stringTest_Offset), arg);

                NativeReflection.InvokeFunction(myClass.Address, StringTest_FunctionAddress, ParamsBuffer, StringTest_ParamsSize);

                refVal = FStringMarshaler.FromNative(IntPtr.Add(ParamsBuffer, StringTest_inVal_Offset));
                arg = FStringMarshaler.FromNative(IntPtr.Add(ParamsBuffer, StringTest_stringTest_Offset));
                string toReturn = FStringMarshaler.FromNative(IntPtr.Add(ParamsBuffer, StringTest_ParamResult_Offset));
                NativeReflection.InvokeFunction_DestroyAll(StringTest_FunctionAddress, ParamsBuffer);

            }
        }

        public static void Call2()
        {
            IntPtr classAddress = NativeReflection.GetClass("/Script/UnrealEngine_Runtime.MyClass");
            Test2_FunctionAddress = NativeReflection.GetFunction(classAddress, "MyTestFunc3");
            Test2_ParamsSize = NativeReflection.GetFunctionParamsSize(Test2_FunctionAddress);
            Test2_Param1_Offset = NativeReflection.GetPropertyOffset(Test2_FunctionAddress, "myStruct");

            UMyClass myClass = UObject.NewObject<UMyClass>();
            FMyStructCustomCtor val = StructDefault<FMyStructCustomCtor>.Value;

            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[Test2_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                FMyStructCustomCtor.ToNative(IntPtr.Add(ParamsBuffer, Test2_Param1_Offset), val);

                NativeReflection.InvokeFunction(myClass.Address, Test2_FunctionAddress, ParamsBuffer, Test2_ParamsSize);

                NativeReflection.InvokeFunction_DestroyAll(Test2_FunctionAddress, ParamsBuffer);

            }
        }
    }

    [UClass]
    class MySomeSuper : MySomeBase
    {
        public int A1 { get; set; }
        public int A2 { get; set; }

        public void CallMe()
        {
            A1 = 2;
            Debug.Assert(A1 == 2);

            A2 = 3;
            Debug.Assert(A2 == 3);

            A3 = 4;
            Debug.Assert(A3 == 4);

            A4 = 5;
            Debug.Assert(A4 == 5);
        }
    }

    [UClass]
    class MySomeBase : UObject
    {
        public int A3 { get; set; }
        public int A4 { get; set; }
    }

    //[UStruct]
    //struct Independant
    //{
    //    [UProperty, EditAnywhere, BlueprintReadWrite]
    //    public int ModifyMe1;
    //    [UProperty, EditAnywhere, BlueprintReadWrite]
    //    public int ModifyMe2;
    //    [UProperty, EditAnywhere, BlueprintReadWrite]
    //    public int ModifyMe3;
    //    [UProperty, EditAnywhere, BlueprintReadWrite]
    //    public float ModifyMe4;
    //}

    [UClass]
    class TestBitObj : UObject
    {
        [UProperty, EditAnywhere, BlueprintReadWrite]
        public BlitTest TestMe1 { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, UMeta(MDEnum.Bitmask), UMeta(MDEnum.BitmaskEnum, "BlitTest")]
        public BlitTest TestMe4 { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, UMeta(MDEnum.Bitmask), UMeta(MDEnum.BitmaskEnum, "EBlitTest")]
        public BlitTest TestMe5 { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, UMeta(MDEnum.Bitmask), UMeta(MDEnum.BitmaskEnum, "BlitTest")]
        public int TestMe2 { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, UMeta(MDEnum.Bitmask), UMeta(MDEnum.BitmaskEnum, "EBlitTest")]
        public int TestMe3 { get; set; }

        [UFunction, BlueprintCallable]
        public void TestFunction(int p1, int p2, int p3)
        {
        }

        public override void Initialize(FObjectInitializer initializer)
        {
            FMessage.Log("Hello from TestBitObj");
            base.Initialize(initializer);
        }
    }

    [UEnum, BlueprintType, UMeta(MDEnum.Bitflags)]
    enum BlitTest : byte
    {
        None = 0,
        Test1 = 1,
        Test2 = 2,
        Test3 = 4,
        Test4 = 8,
    }

    [UInterface, Blueprintable]//, CannotImplementInterfaceInBlueprint]
    //[UInterface, CannotImplementInterfaceInBlueprint]
    public interface InterfaceTest : IInterface
    {
        [BlueprintEvent, BlueprintCallable, Category("Nope")]
        void CallMe123(int a1);
    }

    [UClass]
    public class MyClassImpl : UObject, InterfaceTest
    {
        [BlueprintEvent, BlueprintCallable, Category("Rope")]//, Category("Nope")]
        public void CallMe123(int a1) { throw EventDef; }
        protected virtual void CallMe123_Implementation(int a1)
        {
            FMessage.Log("Hi from MyClassImpl " + a1);
        }
    }

    [UClass]
    public class UTestInterfaceUsage : UObject
    {
        [UProperty, EditAnywhere, BlueprintReadWrite]
        public InterfaceTest TestMe { get; set; }

        //[UProperty, EditAnywhere, BlueprintReadWrite]
        //public TSubclassOfInterface<InterfaceTest> TestMe2 { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, MustImplement("InterfaceTest")]//typeof(InterfaceTest))]
        public TSoftClass<UObject> TestMe2 { get; set; }
    }

    ////[UInterface]
    ////[USharpPath("/Script/UnrealEngine_Runtime.MyInterface")]
    //[UMetaPath("/Script/USharp.MyInterface", InterfaceImpl = typeof(IMyInterfaceImpl))]
    //interface IMyInterface : IInterface
    //{
    //    //[UFunction, BlueprintCallable]
    //    void CallMe(int arg);
    //}
    //
    //sealed class IMyInterfaceImpl : IInterfaceImpl, IMyInterface
    //{
    //    IntPtr CallMe_InstanceFunctionAddress;
    //    static IntPtr CallMe_FunctionAddress;
    //    static int CallMe_ParamsSize;
    //    static int CallMe_arg_Offset;
    //
    //    public void CallMe(int arg)
    //    {
    //        unsafe
    //        {
    //            if (CallMe_InstanceFunctionAddress == IntPtr.Zero)
    //            {
    //                CallMe_InstanceFunctionAddress = NativeReflection.GetFunctionFromInstance(GetAddress(), "CallMe");
    //            }
    //
    //            byte* ParamsBufferAllocation = stackalloc byte[CallMe_ParamsSize];
    //            IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
    //            BlittableTypeMarshaler<int>.ToNative(IntPtr.Add(ParamsBuffer, CallMe_arg_Offset), arg);
    //            NativeReflection.InvokeFunction(GetAddress(), CallMe_FunctionAddress, ParamsBuffer, CallMe_ParamsSize);
    //        }
    //    }
    //
    //    static void LoadNativeType()
    //    {
    //        IntPtr classAddress = NativeReflection.GetClass("/Script/USharp.MyInterface");
    //
    //        CallMe_FunctionAddress = NativeReflection.GetFunction(classAddress, "CallMe");
    //        CallMe_ParamsSize = NativeReflection.GetFunctionParamsSize(CallMe_FunctionAddress);
    //        CallMe_arg_Offset = NativeReflection.GetPropertyOffset(CallMe_FunctionAddress, "test");
    //    }
    //
    //    public override void OnPoolReset()
    //    {
    //        CallMe_InstanceFunctionAddress = IntPtr.Zero;
    //    }
    //}
}


/*namespace UnrealEngine.Runtime
{
    /*[Abstract, UInterface(Flags = 0x30404081), Blueprintable, UMetaPath("/Script/USharp.MyInterface", "USharp", UnrealModuleType.EnginePlugin, InterfaceImpl = typeof(IMyInterfaceImpl))]
    public interface IMyInterface : IInterface
    {
        [UFunction(Flags = 0x44080400), UMetaPath("/Script/USharp.MyInterface:CallMe")]
        void CallMe(int a1);
    }

    public sealed class IMyInterfaceImpl : IInterfaceImpl, IMyInterface
    {
        static bool CallMe_IsValid;
        IntPtr CallMe_InstanceFunctionAddress;
        static IntPtr CallMe_FunctionAddress;
        static int CallMe_ParamsSize;
        static bool CallMe_a1_IsValid;
        static UFieldAddress CallMe_a1_PropertyAddress;
        static int CallMe_a1_Offset;
        [UFunction(Flags = 0x44080400), UMetaPath("/Script/USharp.MyInterface:CallMe")]
        public void CallMe(int a1)
        {
            CheckDestroyed();
            if (!CallMe_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.MyInterface:CallMe");
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
                FMemory.Memzero(ParamsBuffer, CallMe_ParamsSize);
                BlittableTypeMarshaler<int>.ToNative(IntPtr.Add(ParamsBuffer, CallMe_a1_Offset), 0, CallMe_a1_PropertyAddress.Address, a1);

                NativeReflection.InvokeFunction(Address, CallMe_InstanceFunctionAddress, ParamsBuffer, CallMe_ParamsSize);
            }
        }

        public override void ResetInterface()
        {
            CallMe_InstanceFunctionAddress = IntPtr.Zero;
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetClass("/Script/USharp.MyInterface");
            CallMe_FunctionAddress = NativeReflection.GetFunction(classAddress, "CallMe");
            CallMe_ParamsSize = NativeReflection.GetFunctionParamsSize(CallMe_FunctionAddress);
            NativeReflection.GetPropertyRef(ref CallMe_a1_PropertyAddress, CallMe_FunctionAddress, "a1");
            CallMe_a1_Offset = NativeReflection.GetPropertyOffset(CallMe_FunctionAddress, "a1");
            CallMe_a1_IsValid = NativeReflection.ValidatePropertyClass(CallMe_FunctionAddress, "a1", Classes.UIntProperty);
            CallMe_IsValid = CallMe_FunctionAddress != IntPtr.Zero && CallMe_a1_IsValid;
            NativeReflection.LogFunctionIsValid("/Script/USharp.MyInterface:CallMe", CallMe_IsValid);
        }
    }*//*

    [Abstract, UInterface(Flags = 0x30404081), Blueprintable, UMetaPath("/Script/USharp.MyInterface", "USharp", UnrealModuleType.EnginePlugin, InterfaceImpl = typeof(IMyInterfaceImpl))]
    public interface IMyInterface : IInterface
    {
        [UFunction(Flags = 0x44080400), UMetaPath("/Script/USharp.MyInterface:CallMe")]
        void CallMe(int a1);
    }

    public sealed class IMyInterfaceImpl : IInterfaceImpl, IMyInterface
    {
        static bool CallMe_IsValid;
        IntPtr CallMe_InstanceFunctionAddress;
        static IntPtr CallMe_FunctionAddress;
        static int CallMe_ParamsSize;
        static bool CallMe_a1_IsValid;
        static UFieldAddress CallMe_a1_PropertyAddress;
        static int CallMe_a1_Offset;
        [UFunction(Flags = 0x44080400), UMetaPath("/Script/USharp.MyInterface:CallMe")]
        public void CallMe(int a1)
        {
            CheckDestroyed();
            if (!CallMe_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.MyInterface:CallMe");
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
                FMemory.Memzero(ParamsBuffer, CallMe_ParamsSize);
                BlittableTypeMarshaler<int>.ToNative(IntPtr.Add(ParamsBuffer, CallMe_a1_Offset), 0, CallMe_a1_PropertyAddress.Address, a1);

                NativeReflection.InvokeFunction(Address, CallMe_InstanceFunctionAddress, ParamsBuffer, CallMe_ParamsSize);
            }
        }

        public override void ResetInterface()
        {
            CallMe_InstanceFunctionAddress = IntPtr.Zero;
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetClass("/Script/USharp.MyInterface");
            CallMe_FunctionAddress = NativeReflection.GetFunction(classAddress, "CallMe");
            CallMe_ParamsSize = NativeReflection.GetFunctionParamsSize(CallMe_FunctionAddress);
            NativeReflection.GetPropertyRef(ref CallMe_a1_PropertyAddress, CallMe_FunctionAddress, "a1");
            CallMe_a1_Offset = NativeReflection.GetPropertyOffset(CallMe_FunctionAddress, "a1");
            CallMe_a1_IsValid = NativeReflection.ValidatePropertyClass(CallMe_FunctionAddress, "a1", Classes.UIntProperty);
            CallMe_IsValid = CallMe_FunctionAddress != IntPtr.Zero && CallMe_a1_IsValid;
            NativeReflection.LogFunctionIsValid("/Script/USharp.MyInterface:CallMe", CallMe_IsValid);
        }
    }

    [UClass(Flags = (ClassFlags)0x30400080, Config = "Engine"), BlueprintType, Blueprintable, UMetaPath("/Script/USharp.IfImpl1", "USharp", UnrealModuleType.EnginePlugin)]
    public class UIfImpl1 : UObject, IMyInterface
    {
        static bool CallMe_IsValid;
        IntPtr CallMe_InstanceFunctionAddress;
        static IntPtr CallMe_FunctionAddress;
        static int CallMe_ParamsSize;
        static bool CallMe_a1_IsValid;
        static UFieldAddress CallMe_a1_PropertyAddress;
        static int CallMe_a1_Offset;
        /// <summary>
        /// UFUNCTION(BlueprintCallable, Category = "Test", meta = (CallInEditor = "true"))
        /// virtual void CallMe(int32 a1) const override;
        /// </summary>
        [UFunction(Flags = 0x4C080C00), UMetaPath("/Script/USharp.IfImpl1:CallMe")]
        public void CallMe(int a1)
        {
            CheckDestroyed();
            if (!CallMe_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.IfImpl1:CallMe");
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
                FMemory.Memzero(ParamsBuffer, CallMe_ParamsSize);
                BlittableTypeMarshaler<int>.ToNative(IntPtr.Add(ParamsBuffer, CallMe_a1_Offset), 0, CallMe_a1_PropertyAddress.Address, a1);

                NativeReflection.InvokeFunction(Address, CallMe_InstanceFunctionAddress, ParamsBuffer, CallMe_ParamsSize);
            }
        }

        protected virtual void CallMe_Implementation(int a1)
        {
            CheckDestroyed();
            if (!CallMe_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.IfImpl1:CallMe");
                return;
            }
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[CallMe_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                FMemory.Memzero(ParamsBuffer, CallMe_ParamsSize);
                BlittableTypeMarshaler<int>.ToNative(IntPtr.Add(ParamsBuffer, CallMe_a1_Offset), 0, CallMe_a1_PropertyAddress.Address, a1);

                NativeReflection.InvokeFunction(Address, CallMe_FunctionAddress, ParamsBuffer, CallMe_ParamsSize);
            }
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetClass("/Script/USharp.IfImpl1");
            CallMe_FunctionAddress = NativeReflection.GetFunction(classAddress, "CallMe");
            CallMe_ParamsSize = NativeReflection.GetFunctionParamsSize(CallMe_FunctionAddress);
            NativeReflection.GetPropertyRef(ref CallMe_a1_PropertyAddress, CallMe_FunctionAddress, "a1");
            CallMe_a1_Offset = NativeReflection.GetPropertyOffset(CallMe_FunctionAddress, "a1");
            CallMe_a1_IsValid = NativeReflection.ValidatePropertyClass(CallMe_FunctionAddress, "a1", Classes.UIntProperty);
            CallMe_IsValid = CallMe_FunctionAddress != IntPtr.Zero && CallMe_a1_IsValid;
            NativeReflection.LogFunctionIsValid("/Script/USharp.IfImpl1:CallMe", CallMe_IsValid);
        }
    }

    [UClass(Flags = (ClassFlags)0x30400080, Config = "Engine"), BlueprintType, Blueprintable, UMetaPath("/Script/USharp.IfImpl2", "USharp", UnrealModuleType.EnginePlugin)]
    public class UIfImpl2 : UObject, IMyInterface
    {
        static bool MyI_IsValid;
        static int MyI_Offset;
        [UProperty(Flags = (PropFlags)0x002C081040000205), UMetaPath("/Script/USharp.IfImpl2:MyI")]
        protected TSubclassOf<UObject> MyI
        {
            get
            {
                CheckDestroyed();
                if (!MyI_IsValid)
                {
                    NativeReflection.LogInvalidPropertyAccessed("/Script/USharp.IfImpl2:MyI");
                    return default(TSubclassOf<UObject>);
                }
                return TSubclassOfMarshaler<UObject>.FromNative(IntPtr.Add(Address, MyI_Offset));
            }
            set
            {
                CheckDestroyed();
                if (!MyI_IsValid)
                {
                    NativeReflection.LogInvalidPropertyAccessed("/Script/USharp.IfImpl2:MyI");
                    return;
                }
                TSubclassOfMarshaler<UObject>.ToNative(IntPtr.Add(Address, MyI_Offset), value);
            }
        }

        static bool CallMe_IsValid;
        static IntPtr CallMe_FunctionAddress;
        static int CallMe_ParamsSize;
        static bool CallMe_a1_IsValid;
        static UFieldAddress CallMe_a1_PropertyAddress;
        static int CallMe_a1_Offset;
        /// <summary>UFUNCTION(BlueprintNativeEvent, BlueprintCallable, Category = "Test", meta = (CallInEditor = "true"))</summary>
        [UFunction(Flags = 0x44080400), UMetaPath("/Script/USharp.IfImpl2:CallMe")]
        public void CallMe(int a1)
        {
            CheckDestroyed();
            if (!CallMe_IsValid)
            {
                NativeReflection.LogInvalidFunctionAccessed("/Script/USharp.IfImpl2:CallMe");
                return;
            }
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[CallMe_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                FMemory.Memzero(ParamsBuffer, CallMe_ParamsSize);
                BlittableTypeMarshaler<int>.ToNative(IntPtr.Add(ParamsBuffer, CallMe_a1_Offset), 0, CallMe_a1_PropertyAddress.Address, a1);

                NativeReflection.InvokeFunction(Address, CallMe_FunctionAddress, ParamsBuffer, CallMe_ParamsSize);
            }
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetClass("/Script/USharp.IfImpl2");
            MyI_Offset = NativeReflection.GetPropertyOffset(classAddress, "MyI");
            MyI_IsValid = NativeReflection.ValidatePropertyClass(classAddress, "MyI", Classes.UClassProperty);
            CallMe_FunctionAddress = NativeReflection.GetFunction(classAddress, "CallMe");
            CallMe_ParamsSize = NativeReflection.GetFunctionParamsSize(CallMe_FunctionAddress);
            NativeReflection.GetPropertyRef(ref CallMe_a1_PropertyAddress, CallMe_FunctionAddress, "a1");
            CallMe_a1_Offset = NativeReflection.GetPropertyOffset(CallMe_FunctionAddress, "a1");
            CallMe_a1_IsValid = NativeReflection.ValidatePropertyClass(CallMe_FunctionAddress, "a1", Classes.UIntProperty);
            CallMe_IsValid = CallMe_FunctionAddress != IntPtr.Zero && CallMe_a1_IsValid;
            NativeReflection.LogFunctionIsValid("/Script/USharp.IfImpl2:CallMe", CallMe_IsValid);
        }
    }
}*/

//namespace UnrealEngine.Plugins.USharp
//{
//    [Abstract, UInterface(Flags = 0x30404081), BlueprintType, Blueprintable, UMetaPath("/Script/USharp.MyInterface", "USharp", UnrealModuleType.EnginePlugin, InterfaceImpl = typeof(IMyInterfaceImpl))]
//    public interface IMyInterface : IInterface
//    {
//        [UFunction(Flags = 0x4C080C00), UMetaPath("/Script/USharp.MyInterface:CallMe")]
//        void CallMe(int a1);
//    }
//
//    public sealed class IMyInterfaceImpl : IInterfaceImpl, IMyInterface
//    {
//        static void LoadNativeType()
//        {
//            IntPtr classAddress = NativeReflection.GetClass("/Script/USharp.MyInterface");
//        }
//    }
//}
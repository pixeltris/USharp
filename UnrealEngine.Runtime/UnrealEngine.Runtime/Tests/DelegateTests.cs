#if WITH_USHARP_TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime.Tests
{
    // There are some tests of using these delegates in FixedSizeArrayTests and SimpleFunctionParamTests

    static class DelegateTests
    {
        public static void Run()
        {
            Test_SimpleDelegate.Run();
            Test_SimpleMulticastDelegate.Run();
        }
    }

    public class Test_SimpleDelegate : FDelegate<Test_SimpleDelegate.Signature>
    {
        public delegate TSoftClass<UObject> Signature(int param1, string param2, ref double param3, out string param4);

        public static void Run()
        {
            UDelegateFunction unrealFunction = UFunction.GetDelegateSignature<Test_SimpleDelegate>();
            Tests.Assert(unrealFunction != null, "Test_SimpleDelegate");

            Tests.AssertProperty<USoftClassProperty>(unrealFunction, UFunction.ReturnValuePropName, EPropertyFlags.Parm | EPropertyFlags.OutParm | EPropertyFlags.ReturnParm);
            Tests.AssertProperty<UIntProperty>(unrealFunction, "param1", EPropertyFlags.Parm);
            Tests.AssertProperty<UStrProperty>(unrealFunction, "param2", EPropertyFlags.Parm);
            Tests.AssertProperty<UDoubleProperty>(unrealFunction, "param3", EPropertyFlags.Parm | EPropertyFlags.OutParm | EPropertyFlags.ReferenceParm);
            Tests.AssertProperty<UStrProperty>(unrealFunction, "param4", EPropertyFlags.Parm | EPropertyFlags.OutParm);
        }
    }

    public class Test_SimpleMulticastDelegate : FMulticastDelegate<Test_SimpleMulticastDelegate.Signature>
    {
        public delegate void Signature(ulong param1, string param2, ref int param3);

        public static void Run()
        {
            UDelegateFunction unrealFunction = UFunction.GetDelegateSignature<Test_SimpleMulticastDelegate>();
            Tests.Assert(unrealFunction != null, "Test_SimpleMulticastDelegate");

            Tests.AssertProperty<UUInt64Property>(unrealFunction, "param1", EPropertyFlags.Parm);
            Tests.AssertProperty<UStrProperty>(unrealFunction, "param2", EPropertyFlags.Parm);
        }
    }

    // Some delegates for fixed array tests
    public class Test_SimpleDelegateForArrayTests : FDelegate<Test_SimpleDelegateForArrayTests.Signature>
    {
        public delegate void Signature(ref int param1);
    }

    public class Test_SimpleMulticastForArrayTests : FMulticastDelegate<Test_SimpleMulticastForArrayTests.Signature>
    {
        public delegate void Signature(ref double param1);
    }
}
#endif
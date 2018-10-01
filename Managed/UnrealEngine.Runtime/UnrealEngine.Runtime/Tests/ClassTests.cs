#define WITH_USHARP_TESTS_EXPLICIT_IMPLEMENTATION_METHODS
#if WITH_USHARP_TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime.Tests
{
    // TODO: More complex scenarios when "new" is handled. Also "final" and other EFunctionFlags.
    static class ClassTests
    {
        public static void Run()
        {
            UClass class1 = UClass.GetClass<Test_SimpleClass1>();
            UClass class2 = UClass.GetClass<Test_SimpleClass2>();
            UClass class3 = UClass.GetClass<Test_SimpleClass3>();
            Tests.Assert(class1 != null, "Test_SimpleClass1");
            Tests.Assert(class1 != null, "Test_SimpleClass2");
            Tests.Assert(class1 != null, "Test_SimpleClass3");

            Test_SimpleClass1 obj1 = UObject.NewObject<Test_SimpleClass1>();
            Test_SimpleClass2 obj2 = UObject.NewObject<Test_SimpleClass2>();
            Test_SimpleClass3 obj3 = UObject.NewObject<Test_SimpleClass3>();

            TestVirtualFunc1(obj1, 0);
            TestVirtualFunc1(obj2, 1);
            TestVirtualFunc1(obj3, 2);
        }

        private static void TestVirtualFunc1(Test_SimpleClass1 obj, int type)
        {
            // Go through a full marshal of parameters and resolve the function from the UFunction
            object[] parameters = { 1, "inArg" };
            UObject.DynamicInvoke(obj, "VirtualFunc1", parameters);
            string arg = (string)parameters[1];
            
            string expectedOutArg = "Test_SimpleClass1_param2_out";
            switch (type)
            {
                case 1: expectedOutArg = "Test_SimpleClass2_param2_out"; break;
                case 2: expectedOutArg = "Test_SimpleClass3_param2_out"; break;
            }
            Tests.AssertEqual(arg, expectedOutArg, obj.GetClass(), "VirtualFunc1.parm2");
        }
    }

    class Test_SimpleClass1 : UObject
    {
#if WITH_USHARP_TESTS_EXPLICIT_IMPLEMENTATION_METHODS
        [UFunction, BlueprintEvent, BlueprintCallable]
        public string VirtualFunc1(int param1, ref string param2)
        {
            throw new NotImplementedException();
        }

        protected virtual string VirtualFunc1_Implementation(int param1, ref string param2)
        {
            int count = 1;
            if (this.IsA<Test_SimpleClass2>())
            {
                count++;
            }
            if (this.IsA<Test_SimpleClass3>())
            {
                count++;
            }
            Tests.AssertEqual(param1, count, GetClass(), "VirtualFunc1.param1");
            param2 = "Test_SimpleClass1_param2_out";
            return "Test_SimpleClass1";
        }
#else
        [UFunction(EFunctionFlags.BlueprintEvent)]
        public virtual string VirtualFunc1(int param1, ref string param2)
        {
            int count = 1;
            if (this.IsA<Test_SimpleClass2>())
            {
                count++;
            }
            if (this.IsA<Test_SimpleClass3>())
            {
                count++;
            }
            Tests.AssertEqual(param1, count, GetClass(), "VirtualFunc1.param1");
            param2 = "Test_SimpleClass1_param2_out";
            return "Test_SimpleClass1";
        }
#endif
    }

    class Test_SimpleClass2 : Test_SimpleClass1
    {
#if WITH_USHARP_TESTS_EXPLICIT_IMPLEMENTATION_METHODS       
        protected override string VirtualFunc1_Implementation(int param1, ref string param2)
        {
            int count = 1;
            if (this.IsA<Test_SimpleClass3>())
            {
                count++;
            }
            Tests.AssertEqual(param1, count, GetClass(), "VirtualFunc1.param1");
            base.VirtualFunc1_Implementation(param1 + 1, ref param2);
            param2 = "Test_SimpleClass2_param2_out";
            return "Test_SimpleClass2";
        }
#else
        public override string VirtualFunc1(int param1, ref string param2)
        {
            int count = 1;
            if (this.IsA<Test_SimpleClass3>())
            {
                count++;
            }
            Tests.AssertEqual(param1, count, GetClass(), "VirtualFunc1.param1");
            base.VirtualFunc1(param1 + 1, ref param2);
            param2 = "Test_SimpleClass2_param2_out";
            return "Test_SimpleClass2";
        }
#endif
    }

    class Test_SimpleClass3 : Test_SimpleClass2
    {
#if WITH_USHARP_TESTS_EXPLICIT_IMPLEMENTATION_METHODS
        protected override string VirtualFunc1_Implementation(int param1, ref string param2)
        {
            Tests.AssertEqual(param1, 1, GetClass(), "VirtualFunc1.param1");
            base.VirtualFunc1_Implementation(param1 + 1, ref param2);
            param2 = "Test_SimpleClass3_param2_out";
            return "Test_SimpleClass3";
        }
#else
        public override string VirtualFunc1(int param1, ref string param2)
        {
            Tests.AssertEqual(param1, 0, GetClass(), "VirtualFunc1.param1");
            base.VirtualFunc1(param1 + 1, ref param2);
            param2 = "Test_SimpleClass3_param2_out";
            return "Test_SimpleClass3";
        }
#endif
    }
}
#endif
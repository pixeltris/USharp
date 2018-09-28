#if WITH_USHARP_TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime.Tests
{
    // Basic function param tests (no virtual / override)
    static class SimpleFunctionParamTests
    {
        public static void Run()
        {
            UClass unrealClass = UClass.GetClass<Test_SimpleFuncs>();
            Tests.Assert(unrealClass != null, "Test_SimpleFuncs");

            Test_SimpleFuncs obj = UObject.NewObject<Test_SimpleFuncs>();
            TestFuncX(obj, unrealClass, 1);
            TestFuncX(obj, unrealClass, 2);
            TestFuncX(obj, unrealClass, 3);
            TestResultFunctions(obj, unrealClass);
        }

        private static UClass GetActorClass()
        {
            return UClass.GetClass("/Script/Engine.Actor");
        }

        private static void TestResultFunctions(Test_SimpleFuncs obj, UClass unrealClass)
        {
            UClass actorClass = GetActorClass();

            TestResultFuncXEquals<sbyte>(4, obj, 1);
            TestResultFuncXEquals<byte>(5, obj, 2);
            TestResultFuncXEquals<short>(6, obj, 3);
            TestResultFuncXEquals<ushort>(7, obj, 4);
            TestResultFuncXEquals<int>(8, obj, 5);
            TestResultFuncXEquals<uint>(9, obj, 6);
            TestResultFuncXEquals<long>(10, obj, 7);
            TestResultFuncXEquals<ulong>(11, obj, 8);
            TestResultFuncXEquals<float>(12.3f, obj, 9);
            TestResultFuncXEquals<double>(13.4, obj, 10);
            Tests.Assert(TestResultFuncX<Test_SimpleDelegate>(obj, 11, false).IsBound, unrealClass, "ResultFunc11");
            Tests.AssertEqual(TestResultFuncX<Test_SimpleMulticastDelegate>(obj, 12, false).Count, 3, unrealClass, "ResultFunc12");
            TestResultFuncXEquals<UObject>(actorClass, obj, 13);
            TestResultFuncXEquals<UObject>(null, obj, 13, true);
            TestResultFuncXEquals<Test_SimpleEnum>(Test_SimpleEnum.Val3, obj, 14);
            
            var fixedArrayStruct = obj.ResultFunc15();
            Tests.AssertEqual(fixedArrayStruct.Array1[0], 2, unrealClass, "ResultFunc15");
            Tests.AssertEqual(fixedArrayStruct.Array1[1], 5, unrealClass, "ResultFunc15");
            Tests.AssertEqual(fixedArrayStruct.Array1[2], 111, unrealClass, "ResultFunc15");
            
            Tests.AssertEqual(TestResultFuncX<TSubclassOf<UObject>>(obj, 16, false).Value, actorClass, unrealClass, "ResultFunc16");
            Tests.AssertEqual(TestResultFuncX<TSubclassOf<UObject>>(obj, 16, true), TSubclassOf<UObject>.Null, unrealClass, "ResultFunc16");
            Tests.AssertEqual(TestResultFuncX<TLazyObject<UObject>>(obj, 17, false).Value, actorClass, unrealClass, "ResultFunc17");
            Tests.AssertEqual(TestResultFuncX<TLazyObject<UObject>>(obj, 17, true), TLazyObject<UObject>.Null, unrealClass, "ResultFunc17_Null");
            Tests.AssertEqual(TestResultFuncX<TWeakObject<UObject>>(obj, 18, false).Value, actorClass, unrealClass, "ResultFunc18");
            Tests.AssertEqual(TestResultFuncX<TWeakObject<UObject>>(obj, 18, true), TWeakObject<UObject>.Null, unrealClass, "ResultFunc18_Null");
            Tests.AssertEqual(TestResultFuncX<TSoftClass<UObject>>(obj, 19, false).Value, actorClass, unrealClass, "ResultFunc19");
            Tests.AssertEqual(TestResultFuncX<TSoftClass<UObject>>(obj, 19, true), TSoftClass<UObject>.Null, unrealClass, "ResultFunc19_Null");
            Tests.AssertEqual(TestResultFuncX<TSoftObject<UObject>>(obj, 20, false).Value, actorClass, unrealClass, "ResultFunc20");
            Tests.AssertEqual(TestResultFuncX<TSoftObject<UObject>>(obj, 20, true), TSoftObject<UObject>.Null, unrealClass, "ResultFunc20_Null");
            Tests.AssertEqual(TestResultFuncX<string>(obj, 21, false), "result", unrealClass, "ResultFunc21");
            Tests.AssertEqual(TestResultFuncX<string>(obj, 21, true), string.Empty, unrealClass, "ResultFunc21_Null");
            Tests.AssertEqual(TestResultFuncX<FName>(obj, 22, false), new FName("result"), unrealClass, "ResultFunc22");
        }

        private static T TestResultFuncX<T>(Test_SimpleFuncs obj, int func, bool isNullResult)
        {
            string funcName = "ResultFunc" + func;
            if (isNullResult)
            {
                funcName += "_Null";
            }
            UFunction func1 = obj.GetClass().FindFunctionByName(new FName(funcName));
            Tests.AssertNotNull(func1, obj.GetClass(), funcName);

            return (T)UObject.DynamicInvoke(obj, funcName);
        }

        private static void TestResultFuncXEquals<T>(T value, Test_SimpleFuncs obj, int func)
        {
            TestResultFuncXEquals(value, obj, func, false);
        }

        private static void TestResultFuncXEquals<T>(T value, Test_SimpleFuncs obj, int func, bool isNullResult)
        {
            UClass unrealClass = obj.GetClass();
            string funcName = "ResultFunc" + func;
            if (isNullResult)
            {
                funcName += "_Null";
            }
            UFunction func1 = unrealClass.FindFunctionByName(new FName(funcName));
            Tests.AssertNotNull(func1, unrealClass, funcName);

            Tests.AssertEqual((T)UObject.DynamicInvoke(obj, funcName), value, unrealClass, funcName);
        }

        private static void TestFuncX(Test_SimpleFuncs obj, UClass unrealClass, int func)
        {
            string funcName = "Func" + func;
            UFunction func1 = unrealClass.FindFunctionByName(new FName(funcName));
            Tests.AssertNotNull(func1, unrealClass, funcName);

            Test_SimpleDelegate simpleDelegate = new Test_SimpleDelegate();
            simpleDelegate.Bind(obj.BindMe);

            Test_SimpleMulticastDelegate multicastDelegate = new Test_SimpleMulticastDelegate();
            multicastDelegate.Bind(obj.BindMeMulti1);
            multicastDelegate.Bind(obj.BindMeMulti2);

            // Use dynamic invoke so that all parameters go through a full marshal between C#/native code
            object[] parameters =
            {
                (sbyte)2,//sbyte
                (byte)3,//byte,
                (short)4,//short
                (ushort)5,//ushort
                (int)6,//int
                (uint)7,//uint
                (long)8,//long
                (ulong)9,//ulong
                (float)10.2f,//float
                (double)11.5,//double
                simpleDelegate,//delegate
                multicastDelegate,//multicast delegate
                obj,
                Test_SimpleEnum.Val3,
                new Test_FixedArrayInStruct()
                {
                    Array1 = new sbyte[2] { 2, 5 }
                },
                new TSubclassOf<UObject>(obj.GetClass()),
                new TLazyObject<UObject>(obj),
                TWeakObject<UObject>.Null,
                new TSoftClass<UObject>(obj.GetClass()),
                new TSoftObject<UObject>(obj),
                "Test123",
                new FName("321Test")
            };
            long result = (long)UObject.DynamicInvoke(obj, funcName, parameters);
            Tests.AssertEqual(result, 13232, unrealClass, funcName + " result");

            if (func > 1)
            {
                sbyte p1 = (sbyte)parameters[0];
                byte p2 = (byte)parameters[1];
                short p3 = (short)parameters[2];
                ushort p4 = (ushort)parameters[3];
                int p5 = (int)parameters[4];
                uint p6 = (uint)parameters[5];
                long p7 = (long)parameters[6];
                ulong p8 = (ulong)parameters[7];
                float p9 = (float)parameters[8];
                double p10 = (double)parameters[9];
                Test_SimpleDelegate p11 = (Test_SimpleDelegate)parameters[10];
                Test_SimpleMulticastDelegate p12 = (Test_SimpleMulticastDelegate)parameters[11];
                UObject p13 = (UObject)parameters[12];
                Test_SimpleEnum p14 = (Test_SimpleEnum)parameters[13];
                Test_FixedArrayInStruct p15 = (Test_FixedArrayInStruct)parameters[14];
                TSubclassOf<UObject> p16 = (TSubclassOf<UObject>)parameters[15];
                TLazyObject<UObject> p17 = (TLazyObject<UObject>)parameters[16];
                TWeakObject<UObject> p18 = (TWeakObject<UObject>)parameters[17];
                TSoftClass<UObject> p19 = (TSoftClass<UObject>)parameters[18];
                TSoftObject<UObject> p20 = (TSoftObject<UObject>)parameters[19];
                string p21 = (string)parameters[20];
                FName p22 = (FName)parameters[21];

                UClass actorClass = GetActorClass();

                Tests.AssertEqual(p1, 3, unrealClass, funcName + ".p1");
                Tests.AssertEqual(p2, 4, unrealClass, funcName + ".p2");
                Tests.AssertEqual(p3, 5, unrealClass, funcName + ".p3");
                Tests.AssertEqual(p4, 6, unrealClass, funcName + ".p4");
                Tests.AssertEqual(p5, 7, unrealClass, funcName + ".p5");
                Tests.AssertEqual(p6, 8u, unrealClass, funcName + ".p6");
                Tests.AssertEqual(p7, 9, unrealClass, funcName + ".p7");
                Tests.AssertEqual(p8, 10u, unrealClass, funcName + ".p8");
                Tests.AssertEqual(p9, 11.2f, unrealClass, funcName + ".p9");
                Tests.AssertEqual(p10, 12.5, unrealClass, funcName + ".p10");
                Tests.Assert(!p11.IsBound, unrealClass, funcName + ".p11");
                Tests.Assert(p12.IsBound, unrealClass, funcName + ".p12");
                Tests.AssertEqual(p12.Count, 3, unrealClass, funcName + ".p12");
                Tests.AssertEqual(p13, actorClass, unrealClass, funcName + ".p13");
                Tests.AssertEqual(p14, Test_SimpleEnum.Val2, unrealClass, funcName + ".p14");
                Tests.AssertEqual(p15.Array1[0], 2, unrealClass, funcName + ".p15");
                Tests.AssertEqual(p15.Array1[1], 5, unrealClass, funcName + ".p15");
                Tests.AssertEqual(p15.Array1[2], 100, unrealClass, funcName + ".p15");
                Tests.AssertEqual(p16.Value, actorClass, unrealClass, funcName + ".p16");
                Tests.AssertEqual(p17.Value, actorClass, unrealClass, funcName + ".p17");
                Tests.AssertEqual(p18.Value, actorClass, unrealClass, funcName + ".p18");
                Tests.AssertEqual(p19.Value, actorClass, unrealClass, funcName + ".p19");
                Tests.AssertEqual(p20.Value, actorClass, unrealClass, funcName + ".p20");
                Tests.AssertEqual(p21, "changed123", unrealClass, funcName + ".p21");
                Tests.AssertEqual(p22, new FName("321changed"), unrealClass, funcName + ".p22");
            }
        }
    }

    class Test_SimpleFuncs : UObject
    {
        [UFunction]
        public long Func1(
            sbyte p1, 
            byte p2,
            short p3,
            ushort p4,
            int p5,
            uint p6,
            long p7,
            ulong p8,
            float p9,
            double p10,
            Test_SimpleDelegate p11,
            Test_SimpleMulticastDelegate p12,
            UObject p13,
            Test_SimpleEnum p14,
            Test_FixedArrayInStruct p15,
            TSubclassOf<UObject> p16,
            TLazyObject<UObject> p17,
            TWeakObject<UObject> p18,
            TSoftClass<UObject> p19,
            TSoftObject<UObject> p20,
            string p21,
            FName p22)
        {
            UClass unrealClass = GetClass();

            Tests.AssertEqual(p1, 2, unrealClass, "Func1.p1");
            Tests.AssertEqual(p2, 3, unrealClass, "Func1.p2");
            Tests.AssertEqual(p3, 4, unrealClass, "Func1.p3");
            Tests.AssertEqual(p4, 5, unrealClass, "Func1.p4");
            Tests.AssertEqual(p5, 6, unrealClass, "Func1.p5");
            Tests.AssertEqual(p6, 7u, unrealClass, "Func1.p6");
            Tests.AssertEqual(p7, 8, unrealClass, "Func1.p7");
            Tests.AssertEqual(p8, 9u, unrealClass, "Func1.p8");
            Tests.AssertEqual(p9, 10.2f, unrealClass, "Func1.p9");
            Tests.AssertEqual(p10, 11.5, unrealClass, "Func1.p10");
            Tests.Assert(p11.IsBound, unrealClass, "Func1.p11");
            Tests.Assert(p12.IsBound, unrealClass, "Func1.p12");
            Tests.AssertEqual(p12.Count, 2, unrealClass, "Func1.p12");
            Tests.AssertEqual(p13, this, unrealClass, "Func1.p13");
            Tests.AssertEqual(p14, Test_SimpleEnum.Val3, unrealClass, "Func1.p14");
            Tests.AssertEqual(p15.Array1[0], 2, unrealClass, "Func1.p15");
            Tests.AssertEqual(p15.Array1[1], 5, unrealClass, "Func1.p15");
            Tests.AssertEqual(p16.Value, unrealClass, unrealClass, "Func1.p16");
            Tests.AssertEqual(p17.Value, this, unrealClass, "Func1.p17");
            Tests.AssertEqual(p18, TWeakObject<UObject>.Null, unrealClass, "Func1.p18");
            Tests.AssertEqual(p19.Value, unrealClass, unrealClass, "Func1.p19");
            Tests.AssertEqual(p20.Value, this, unrealClass, "Func1.p20");
            Tests.AssertEqual(p21, "Test123", unrealClass, "Func1.p21");
            Tests.AssertEqual(p22, new FName("321Test"), unrealClass, "Func1.p22");

            double param3 = 4.5;
            string param4;
            p11.Invoke(3, "param2", ref param3, out param4);
            Tests.AssertEqual(param3, 5, unrealClass, "BindMe.param3");
            Tests.AssertEqual(param4, "out", unrealClass, "BindMe.param4");

            int multiParam3 = 2;
            p12.Invoke(1, "two", ref multiParam3);
            Tests.AssertEqual(multiParam3, 7, unrealClass, "BindMeMulti.param3");

            return 13232;
        }

        [UFunction]
        public long Func2(
            ref sbyte p1,
            ref byte p2,
            ref short p3,
            ref ushort p4,
            ref int p5,
            ref uint p6,
            ref long p7,
            ref ulong p8,
            ref float p9,
            ref double p10,
            ref Test_SimpleDelegate p11,
            ref Test_SimpleMulticastDelegate p12,
            ref UObject p13,
            ref Test_SimpleEnum p14,
            ref Test_FixedArrayInStruct p15,
            ref TSubclassOf<UObject> p16,
            ref TLazyObject<UObject> p17,
            ref TWeakObject<UObject> p18,
            ref TSoftClass<UObject> p19,
            ref TSoftObject<UObject> p20,
            ref string p21,
            ref FName p22)
        {
            UClass unrealClass = GetClass();

            Tests.AssertEqual(p1, 2, unrealClass, "Func2.p1");
            Tests.AssertEqual(p2, 3, unrealClass, "Func2.p2");
            Tests.AssertEqual(p3, 4, unrealClass, "Func2.p3");
            Tests.AssertEqual(p4, 5, unrealClass, "Func2.p4");
            Tests.AssertEqual(p5, 6, unrealClass, "Func2.p5");
            Tests.AssertEqual(p6, 7u, unrealClass, "Func2.p6");
            Tests.AssertEqual(p7, 8, unrealClass, "Func2.p7");
            Tests.AssertEqual(p8, 9u, unrealClass, "Func2.p8");
            Tests.AssertEqual(p9, 10.2f, unrealClass, "Func2.p9");
            Tests.AssertEqual(p10, 11.5, unrealClass, "Func2.p10");
            Tests.Assert(p11.IsBound, unrealClass, "Func2.p11");
            Tests.Assert(p12.IsBound, unrealClass, "Func2.p12");
            Tests.AssertEqual(p12.Count, 2, unrealClass, "Func2.p12");
            Tests.AssertEqual(p13, this, unrealClass, "Func2.p13");
            Tests.AssertEqual(p14, Test_SimpleEnum.Val3, unrealClass, "Func2.p14");
            Tests.AssertEqual(p15.Array1[0], 2, unrealClass, "Func2.p15");
            Tests.AssertEqual(p15.Array1[1], 5, unrealClass, "Func2.p15");
            Tests.AssertEqual(p16.Value, unrealClass, unrealClass, "Func2.p16");
            Tests.AssertEqual(p17.Value, this, unrealClass, "Func2.p17");
            Tests.AssertEqual(p18, TWeakObject<UObject>.Null, unrealClass, "Func2.p18");
            Tests.AssertEqual(p19.Value, unrealClass, unrealClass, "Func2.p19");
            Tests.AssertEqual(p20.Value, this, unrealClass, "Func2.p20");
            Tests.AssertEqual(p21, "Test123", unrealClass, "Func2.p21");
            Tests.AssertEqual(p22, new FName("321Test"), unrealClass, "Func2.p22");

            double param3 = 4.5;
            string param4;
            p11.Invoke(3, "param2", ref param3, out param4);
            Tests.AssertEqual(param3, 5, unrealClass, "BindMe.param3");
            Tests.AssertEqual(param4, "out", unrealClass, "BindMe.param4");
            
            int multiParam3 = 2;
            p12.Invoke(1, "two", ref multiParam3);
            Tests.AssertEqual(multiParam3, 7, unrealClass, "BindMeMulti.param3");

            UClass actorClass = GetActorClass();

            p1++;
            p2++;
            p3++;
            p4++;
            p5++;
            p6++;
            p7++;
            p8++;
            p9++;
            p10++;
            p11.Clear();
            p12.Bind(BindMeMulti3);
            p13 = actorClass;
            p14 = Test_SimpleEnum.Val2;
            p15.Array1[2] = 100;
            p16.SetClass(actorClass);
            p17.Value = actorClass;
            p18.Value = actorClass;
            p19.Value = actorClass;
            p20.Value = actorClass;
            p21 = "changed123";
            p22 = new FName("321changed");
            return 13232;
        }

        [UFunction]
        private long Func3(
            out sbyte p1,
            out byte p2,
            out short p3,
            out ushort p4,
            out int p5,
            out uint p6,
            out long p7,
            out ulong p8,
            out float p9,
            out double p10,
            out Test_SimpleDelegate p11,
            out Test_SimpleMulticastDelegate p12,
            out UObject p13,
            out Test_SimpleEnum p14,
            out Test_FixedArrayInStruct p15,
            out TSubclassOf<UObject> p16,
            out TLazyObject<UObject> p17,
            out TWeakObject<UObject> p18,
            out TSoftClass<UObject> p19,
            out TSoftObject<UObject> p20,
            out string p21,
            out FName p22)
        {
            Test_SimpleDelegate simpleDelegate = new Test_SimpleDelegate();
            simpleDelegate.Bind(BindMe);

            Test_SimpleMulticastDelegate multicastDelegate = new Test_SimpleMulticastDelegate();
            multicastDelegate.Bind(BindMeMulti1);
            multicastDelegate.Bind(BindMeMulti2);
            multicastDelegate.Bind(BindMeMulti3);

            UClass actorClass = GetActorClass();

            p1 = 3;
            p2 = 4;
            p3 = 5;
            p4 = 6;
            p5 = 7;
            p6 = 8;
            p7 = 9;
            p8 = 10;
            p9 = 11.2f;
            p10 = 12.5f;
            p11 = null;
            p12 = multicastDelegate;
            p13 = actorClass;
            p14 = Test_SimpleEnum.Val2;
            p15 = new Test_FixedArrayInStruct()
            {
                Array1 = new sbyte[3] { 2, 5, 100 }
            };
            p16 = new TSubclassOf<UObject>(actorClass);
            p17 = new TLazyObject<UObject>(actorClass);
            p18 = new TWeakObject<UObject>(actorClass);
            p19 = new TSoftClass<UObject>(actorClass);
            p20 = new TSoftObject<UObject>(actorClass);
            p21 = "changed123";
            p22 = new FName("321changed");
            return 13232;
        }

        [UFunction]
        public TSoftClass<UObject> BindMe(int param1, string param2, ref double param3, out string param4)
        {
            Tests.AssertEqual(param1, 3, GetClass(), "BindMe.param1");
            Tests.AssertEqual(param2, "param2", GetClass(), "BindMe.param2");
            Tests.AssertEqual(param3, 4.5, GetClass(), "BindMe.param3");
            param3 += 0.5;
            param4 = "out";
            return new TSoftClass<UObject>("/Script/Engine.Actor");
        }

        [UFunction]
        public void BindMeMulti1(ulong param1, string param2, ref int param3)
        {
            Tests.Assert(param3 > 1, GetClass(), "BindMeMulti1.param3");
            param3 += 2;
        }

        [UFunction]
        public void BindMeMulti2(ulong param1, string param2, ref int param3)
        {
            Tests.Assert(param3 > 1, GetClass(), "BindMeMulti1.param3");
            param3 += 3;
        }

        [UFunction]
        public void BindMeMulti3(ulong param1, string param2, ref int param3)
        {
        }

        [UFunction]
        public sbyte ResultFunc1()
        {
            return 4;
        }

        [UFunction]
        public byte ResultFunc2()
        {
            return 5;
        }

        [UFunction]
        public short ResultFunc3()
        {
            return 6;
        }

        [UFunction]
        public ushort ResultFunc4()
        {
            return 7;
        }

        [UFunction]
        public int ResultFunc5()
        {
            return 8;
        }

        [UFunction]
        public uint ResultFunc6()
        {
            return 9;
        }

        [UFunction]
        public long ResultFunc7()
        {
            return 10;
        }

        [UFunction]
        public ulong ResultFunc8()
        {
            return 11;
        }

        [UFunction]
        public float ResultFunc9()
        {
            return 12.3f;
        }

        [UFunction]
        public double ResultFunc10()
        {
            return 13.4;
        }

        [UFunction]
        public Test_SimpleDelegate ResultFunc11()
        {
            Test_SimpleDelegate simpleDelegate = new Test_SimpleDelegate();
            simpleDelegate.Bind(BindMe);
            return simpleDelegate;
        }

        [UFunction]
        public Test_SimpleMulticastDelegate ResultFunc12()
        {
            Test_SimpleMulticastDelegate multicastDelegate = new Test_SimpleMulticastDelegate();
            multicastDelegate.Bind(BindMeMulti1);
            multicastDelegate.Bind(BindMeMulti2);
            multicastDelegate.Bind(BindMeMulti3);
            return multicastDelegate;
        }

        [UFunction]
        public UObject ResultFunc13()
        {
            return GetActorClass();
        }

        [UFunction]
        public UObject ResultFunc13_Null()
        {
            return null;
        }

        [UFunction]
        public Test_SimpleEnum ResultFunc14()
        {
            return Test_SimpleEnum.Val3;
        }

        [UFunction]
        public Test_FixedArrayInStruct ResultFunc15()
        {
            return new Test_FixedArrayInStruct()
            {
                Array1 = new sbyte[3] { 2, 5, 111 }
            };
        }

        [UFunction]
        public TSubclassOf<UObject> ResultFunc16()
        {
            return new TSubclassOf<UObject>(GetActorClass());
        }

        [UFunction]
        public TSubclassOf<UObject> ResultFunc16_Null()
        {
            return TSubclassOf<UObject>.Null;
        }

        [UFunction]
        public TLazyObject<UObject> ResultFunc17()
        {
            return new TLazyObject<UObject>(GetActorClass());
        }

        [UFunction]
        public TLazyObject<UObject> ResultFunc17_Null()
        {
            return TLazyObject<UObject>.Null;
        }

        [UFunction]
        public TWeakObject<UObject> ResultFunc18()
        {
            return new TWeakObject<UObject>(GetActorClass());
        }

        [UFunction]
        public TWeakObject<UObject> ResultFunc18_Null()
        {
            return TWeakObject<UObject>.Null;
        }

        [UFunction]
        public TSoftClass<UObject> ResultFunc19()
        {
            return new TSoftClass<UObject>(GetActorClass());
        }

        [UFunction]
        public TSoftClass<UObject> ResultFunc19_Null()
        {
            return TSoftClass<UObject>.Null;
        }

        [UFunction]
        public TSoftObject<UObject> ResultFunc20()
        {
            return new TSoftObject<UObject>(GetActorClass());
        }

        [UFunction]
        public TSoftObject<UObject> ResultFunc20_Null()
        {
            return TSoftObject<UObject>.Null;
        }

        [UFunction]
        public string ResultFunc21()
        {
            return "result";
        }

        [UFunction]
        public string ResultFunc21_Null()
        {
            return null;
        }

        [UFunction]
        public FName ResultFunc22()
        {
            return new FName("result");
        }

        [UFunctionIgnore]
        private static UClass GetActorClass()
        {
            return UClass.GetClass("/Script/Engine.Actor");
        }
    }
}
#endif
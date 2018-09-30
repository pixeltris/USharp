#if WITH_USHARP_TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime.Tests
{
    // Only testing a smaller subset of types here to reduce the amount of code.
    // - Only testing TSoftClass as a return result as TSoftClass/TSoftObject are one of the most likely to have 
    //   issues marshaling due to being an FString container

    // Using DynamicInvoke for a full marshal

    // Basic collections function param tests (no virtual / override)
    static class CollectionFunctionParamTests
    {
        public static void Run()
        {
            UClass unrealClass = UClass.GetClass<Test_CollectionsFuncs>();
            Tests.Assert(unrealClass != null, "Test_CollectionsFuncs");

            Test_CollectionsFuncs obj = UObject.NewObject<Test_CollectionsFuncs>();
            RunArrayTests(obj, unrealClass);
            RunSetTests(obj, unrealClass);
            RunMapTest(obj, unrealClass);
        }

        private static void RunArrayTests(Test_CollectionsFuncs obj, UClass unrealClass)
        {
            var p1 = new List<long>();
            var p2 = new List<UObject>();
            var p3 = new List<Test_SimpleEnum>();
            var p4 = new List<Test_SimpleBlittableStruct>();
            var p5 = new List<TSoftClass<UObject>>();
            var p6 = new List<string>();
            var p7 = new List<FName>();
            Test_CollectionsFuncs.MakeArrayTest(p1, p2, p3, p4, p5, p6, p7);

            // Normal
            {
                object[] parameters = { p1, p2, p3, p4, p5, p6, p7 };
                UObject.DynamicInvoke(obj, "ArrayFunc1", parameters);
            }

            // Ref
            {
                object[] parameters = { p1, p2, p3, p4, p5, p6, p7 };
                UObject.DynamicInvoke(obj, "ArrayFunc2", parameters);
            }

            // Ref (starting empty)
            {
                p1.Clear();
                p2.Clear();
                p3.Clear();
                p4.Clear();
                p5.Clear();
                p6.Clear();
                p7.Clear();
                object[] parameters = { p1, p2, p3, p4, p5, p6, p7 };
                UObject.DynamicInvoke(obj, "ArrayFunc3", parameters);
                p1 = (List<long>)parameters[0];
                p2 = (List<UObject>)parameters[1];
                p3 = (List<Test_SimpleEnum>)parameters[2];
                p4 = (List<Test_SimpleBlittableStruct>)parameters[3];
                p5 = (List<TSoftClass<UObject>>)parameters[4];
                p6 = (List<string>)parameters[5];
                p7 = (List<FName>)parameters[6];
                Test_CollectionsFuncs.AssertArrayTest(obj, p1, p2, p3, p4, p5, p6, p7);
            }

            // Out
            {
                object[] parameters = { null, null, null, null, null, null, null };
                UObject.DynamicInvoke(obj, "ArrayFunc4", parameters);
                p1 = (List<long>)parameters[0];
                p2 = (List<UObject>)parameters[1];
                p3 = (List<Test_SimpleEnum>)parameters[2];
                p4 = (List<Test_SimpleBlittableStruct>)parameters[3];
                p5 = (List<TSoftClass<UObject>>)parameters[4];
                p6 = (List<string>)parameters[5];
                p7 = (List<FName>)parameters[6];
                Test_CollectionsFuncs.AssertArrayTest(obj, p1, p2, p3, p4, p5, p6, p7);
            }

            // Result
            {
                object[] parameters = { "inStr" };
                var setResult = (List<TSoftClass<UObject>>)UObject.DynamicInvoke(obj, "ArrayFunc5", parameters);
                Tests.Assert(setResult.Contains(TSoftClass<UObject>.Null), unrealClass, "ArrayFunc5");
                Tests.Assert(setResult.Contains(new TSoftClass<UObject>(Tests.ActorClass)), unrealClass, "ArrayFunc5");
                Tests.AssertEqual((string)parameters[0], "outStr", unrealClass, "ArrayFunc5.param1");
            }
        }

        private static void RunSetTests(Test_CollectionsFuncs obj, UClass unrealClass)
        {
            var p1 = new HashSet<long>();
            var p2 = new HashSet<UObject>();
            var p3 = new HashSet<Test_SimpleEnum>();
            var p4 = new HashSet<Test_SimpleBlittableStruct>();
            var p5 = new HashSet<TSoftClass<UObject>>();
            var p6 = new HashSet<string>();
            var p7 = new HashSet<FName>();
            Test_CollectionsFuncs.MakeSetTest(p1, p2, p3, p4, p5, p6, p7);

            // Normal
            {
                object[] parameters = { p1, p2, p3, p4, p5, p6, p7 };
                UObject.DynamicInvoke(obj, "SetFunc1", parameters);
            }

            // Ref
            {
                object[] parameters = { p1, p2, p3, p4, p5, p6, p7 };
                UObject.DynamicInvoke(obj, "SetFunc2", parameters);
            }

            // Ref (starting empty)
            {
                p1.Clear();
                p2.Clear();
                p3.Clear();
                p4.Clear();
                p5.Clear();
                p6.Clear();
                p7.Clear();
                object[] parameters = { p1, p2, p3, p4, p5, p6, p7 };
                UObject.DynamicInvoke(obj, "SetFunc3", parameters);
                p1 = (HashSet<long>)parameters[0];
                p2 = (HashSet<UObject>)parameters[1];
                p3 = (HashSet<Test_SimpleEnum>)parameters[2];
                p4 = (HashSet<Test_SimpleBlittableStruct>)parameters[3];
                p5 = (HashSet<TSoftClass<UObject>>)parameters[4];
                p6 = (HashSet<string>)parameters[5];
                p7 = (HashSet<FName>)parameters[6];
                Test_CollectionsFuncs.AssertSetTest(obj, p1, p2, p3, p4, p5, p6, p7);
            }

            // Out
            {
                object[] parameters = { null, null, null, null, null, null, null };
                UObject.DynamicInvoke(obj, "SetFunc4", parameters);
                p1 = (HashSet<long>)parameters[0];
                p2 = (HashSet<UObject>)parameters[1];
                p3 = (HashSet<Test_SimpleEnum>)parameters[2];
                p4 = (HashSet<Test_SimpleBlittableStruct>)parameters[3];
                p5 = (HashSet<TSoftClass<UObject>>)parameters[4];
                p6 = (HashSet<string>)parameters[5];
                p7 = (HashSet<FName>)parameters[6];
                Test_CollectionsFuncs.AssertSetTest(obj, p1, p2, p3, p4, p5, p6, p7);
            }

            // Result
            {
                object[] parameters = { "inStr" };
                var setResult = (HashSet<TSoftClass<UObject>>)UObject.DynamicInvoke(obj, "SetFunc5", parameters);
                Tests.Assert(setResult.Contains(TSoftClass<UObject>.Null), unrealClass, "SetFunc5");
                Tests.Assert(setResult.Contains(new TSoftClass<UObject>(Tests.ActorClass)), unrealClass, "SetFunc5");
                Tests.AssertEqual((string)parameters[0], "outStr", unrealClass, "SetFunc5.param1");
            }
        }

        private static void RunMapTest(Test_CollectionsFuncs obj, UClass unrealClass)
        {
            var p1 = new Dictionary<long, string>();
            var p2 = new Dictionary<UObject, string>();
            var p3 = new Dictionary<Test_SimpleEnum, string>();
            var p4 = new Dictionary<Test_SimpleBlittableStruct, UObject>();
            var p5 = new Dictionary<TSoftClass<UObject>, TSoftClass<UObject>>();
            var p6 = new Dictionary<string, string>();
            var p7 = new Dictionary<FName, string>();
            Test_CollectionsFuncs.MakeMapTest(p1, p2, p3, p4, p5, p6, p7);

            // Normal
            {
                object[] parameters = { p1, p2, p3, p4, p5, p6, p7 };
                UObject.DynamicInvoke(obj, "MapFunc1", parameters);
            }

            // Ref
            {
                object[] parameters = { p1, p2, p3, p4, p5, p6, p7 };
                UObject.DynamicInvoke(obj, "MapFunc2", parameters);
            }

            // Ref (starting empty)
            {
                p1.Clear();
                p2.Clear();
                p3.Clear();
                p4.Clear();
                p5.Clear();
                p6.Clear();
                p7.Clear();
                object[] parameters = { p1, p2, p3, p4, p5, p6, p7 };
                UObject.DynamicInvoke(obj, "MapFunc3", parameters);
                p1 = (Dictionary<long, string>)parameters[0];
                p2 = (Dictionary<UObject, string>)parameters[1];
                p3 = (Dictionary<Test_SimpleEnum, string>)parameters[2];
                p4 = (Dictionary<Test_SimpleBlittableStruct, UObject>)parameters[3];
                p5 = (Dictionary<TSoftClass<UObject>, TSoftClass<UObject>>)parameters[4];
                p6 = (Dictionary<string, string>)parameters[5];
                p7 = (Dictionary<FName, string>)parameters[6];
                Test_CollectionsFuncs.AssertMapTest(obj, p1, p2, p3, p4, p5, p6, p7);
            }

            // Out
            {
                object[] parameters = { null, null, null, null, null, null, null };
                UObject.DynamicInvoke(obj, "MapFunc4", parameters);
                p1 = (Dictionary<long, string>)parameters[0];
                p2 = (Dictionary<UObject, string>)parameters[1];
                p3 = (Dictionary<Test_SimpleEnum, string>)parameters[2];
                p4 = (Dictionary<Test_SimpleBlittableStruct, UObject>)parameters[3];
                p5 = (Dictionary<TSoftClass<UObject>, TSoftClass<UObject>>)parameters[4];
                p6 = (Dictionary<string, string>)parameters[5];
                p7 = (Dictionary<FName, string>)parameters[6];
                Test_CollectionsFuncs.AssertMapTest(obj, p1, p2, p3, p4, p5, p6, p7);
            }

            // Result
            {
                object[] parameters = { "inStr" };
                var mapResult = (Dictionary<TSoftClass<UObject>, TSoftClass<UObject>>)UObject.DynamicInvoke(obj, "MapFunc5", parameters);
                Tests.AssertEqual(mapResult[TSoftClass<UObject>.Null], new TSoftClass<UObject>(Tests.ActorClass), unrealClass, "MapFunc5");
                Tests.AssertEqual(mapResult[new TSoftClass<UObject>(Tests.ActorClass)], TSoftClass<UObject>.Null, unrealClass, "MapFunc5");
                Tests.AssertEqual((string)parameters[0], "outStr", unrealClass, "MapFunc5.param1");
            }
        }
    }

    [UClass]
    class Test_CollectionsFuncs : UObject
    {
        ///////////////////////////////////////////////////////////////////
        // TArray
        ///////////////////////////////////////////////////////////////////

        [UFunctionIgnore]
        public static void MakeArrayTest(
            IList<long> p1,
            IList<UObject> p2,
            IList<Test_SimpleEnum> p3,
            IList<Test_SimpleBlittableStruct> p4,
            IList<TSoftClass<UObject>> p5,
            IList<string> p6,
            IList<FName> p7)
        {
            p1.Add(10);
            p2.Add(Tests.ActorClass);
            p3.Add(Test_SimpleEnum.Val3);
            p4.Add(new Test_SimpleBlittableStruct() { Val3 = 99 });
            p5.Add(TSoftClass<UObject>.Null);
            p5.Add(new TSoftClass<UObject>(Tests.ActorClass));
            p6.Add("test123");
            p7.Add(new FName("test1234"));
        }

        [UFunctionIgnore]
        public static void AssertArrayTest(UObject obj,
            IList<long> p1,
            IList<UObject> p2,
            IList<Test_SimpleEnum> p3,
            IList<Test_SimpleBlittableStruct> p4,
            IList<TSoftClass<UObject>> p5,
            IList<string> p6,
            IList<FName> p7)
        {
            UClass unrealClass = obj.GetClass();

            Tests.Assert(p1.Contains(10), unrealClass, "AssertSetTest.p1");
            Tests.Assert(p2.Contains(Tests.ActorClass), unrealClass, "AssertSetTest.p2");
            Tests.Assert(p3.Contains(Test_SimpleEnum.Val3), unrealClass, "AssertSetTest.p3");
            Tests.Assert(p4.Contains(new Test_SimpleBlittableStruct() { Val3 = 99 }), unrealClass, "AssertSetTest.p4");
            Tests.Assert(p5.Contains(TSoftClass<UObject>.Null), unrealClass, "AssertSetTest.p5");
            Tests.Assert(p5.Contains(new TSoftClass<UObject>(Tests.ActorClass)), unrealClass, "AssertSetTest.p5");
            Tests.Assert(p6.Contains("test123"), unrealClass, "AssertSetTest.p6");
            Tests.Assert(p7.Contains(new FName("test1234")), unrealClass, "AssertSetTest.p7");
        }

        [UFunction]
        public long ArrayFunc1(
            IList<long> p1,
            IList<UObject> p2,
            IList<Test_SimpleEnum> p3,
            IList<Test_SimpleBlittableStruct> p4,
            IList<TSoftClass<UObject>> p5,
            IList<string> p6,
            IList<FName> p7)
        {
            AssertArrayTest(this, p1, p2, p3, p4, p5, p6, p7);
            return 13232;
        }

        [UFunction]
        public long ArrayFunc2(
            ref IList<long> p1,
            ref IList<UObject> p2,
            ref IList<Test_SimpleEnum> p3,
            ref IList<Test_SimpleBlittableStruct> p4,
            ref IList<TSoftClass<UObject>> p5,
            ref IList<string> p6,
            ref IList<FName> p7)
        {
            AssertArrayTest(this, p1, p2, p3, p4, p5, p6, p7);
            return 13232;
        }

        [UFunction]
        public long ArrayFunc3(
            ref IList<long> p1,
            ref IList<UObject> p2,
            ref IList<Test_SimpleEnum> p3,
            ref IList<Test_SimpleBlittableStruct> p4,
            ref IList<TSoftClass<UObject>> p5,
            ref IList<string> p6,
            ref IList<FName> p7)
        {
            Tests.AssertEqual(p1.Count, 0, GetClass(), "SetFunc3 ref empty map");
            Tests.AssertEqual(p2.Count, 0, GetClass(), "SetFunc3 ref empty map");
            Tests.AssertEqual(p3.Count, 0, GetClass(), "SetFunc3 ref empty map");
            Tests.AssertEqual(p4.Count, 0, GetClass(), "SetFunc3 ref empty map");
            Tests.AssertEqual(p5.Count, 0, GetClass(), "SetFunc3 ref empty map");
            Tests.AssertEqual(p6.Count, 0, GetClass(), "SetFunc3 ref empty map");
            Tests.AssertEqual(p7.Count, 0, GetClass(), "SetFunc3 ref empty map");
            MakeArrayTest(p1, p2, p3, p4, p5, p6, p7);
            AssertArrayTest(this, p1, p2, p3, p4, p5, p6, p7);
            return 13232;
        }

        [UFunction]
        public long ArrayFunc4(
            out IList<long> p1,
            out IList<UObject> p2,
            out IList<Test_SimpleEnum> p3,
            out IList<Test_SimpleBlittableStruct> p4,
            out IList<TSoftClass<UObject>> p5,
            out IList<string> p6,
            out IList<FName> p7)
        {
            p1 = new List<long>();
            p2 = new List<UObject>();
            p3 = new List<Test_SimpleEnum>();
            p4 = new List<Test_SimpleBlittableStruct>();
            p5 = new List<TSoftClass<UObject>>();
            p6 = new List<string>();
            p7 = new List<FName>();
            MakeArrayTest(p1, p2, p3, p4, p5, p6, p7);
            return 13232;
        }

        [UFunction]
        public List<TSoftClass<UObject>> ArrayFunc5(ref string inStr)
        {
            Tests.AssertEqual(inStr, "inStr", GetClass(), "ArrayFunc5.inStr");
            inStr = "outStr";

            List<TSoftClass<UObject>> result = new List<TSoftClass<UObject>>();
            result.Add(TSoftClass<UObject>.Null);
            result.Add(new TSoftClass<UObject>(Tests.ActorClass));
            return result;
        }

        ///////////////////////////////////////////////////////////////////
        // TSet
        ///////////////////////////////////////////////////////////////////

        [UFunctionIgnore]
        public static void MakeSetTest(
            ISet<long> p1,
            ISet<UObject> p2,
            ISet<Test_SimpleEnum> p3,
            ISet<Test_SimpleBlittableStruct> p4,
            ISet<TSoftClass<UObject>> p5,
            ISet<string> p6,
            ISet<FName> p7)
        {
            p1.Add(10);
            p2.Add(Tests.ActorClass);
            p3.Add(Test_SimpleEnum.Val3);
            p4.Add(new Test_SimpleBlittableStruct() { Val3 = 99 });
            p5.Add(TSoftClass<UObject>.Null);
            p5.Add(new TSoftClass<UObject>(Tests.ActorClass));
            p6.Add("test123");
            p7.Add(new FName("test1234"));
        }

        [UFunctionIgnore]
        public static void AssertSetTest(UObject obj,
            ISet<long> p1,
            ISet<UObject> p2,
            ISet<Test_SimpleEnum> p3,
            ISet<Test_SimpleBlittableStruct> p4,
            ISet<TSoftClass<UObject>> p5,
            ISet<string> p6,
            ISet<FName> p7)
        {
            UClass unrealClass = obj.GetClass();

            Tests.Assert(p1.Contains(10), unrealClass, "AssertSetTest.p1");
            Tests.Assert(p2.Contains(Tests.ActorClass), unrealClass, "AssertSetTest.p2");
            Tests.Assert(p3.Contains(Test_SimpleEnum.Val3), unrealClass, "AssertSetTest.p3");
            Tests.Assert(p4.Contains(new Test_SimpleBlittableStruct() { Val3 = 99 }), unrealClass, "AssertSetTest.p4");
            Tests.Assert(p5.Contains(TSoftClass<UObject>.Null), unrealClass, "AssertSetTest.p5");
            Tests.Assert(p5.Contains(new TSoftClass<UObject>(Tests.ActorClass)), unrealClass, "AssertSetTest.p5");
            Tests.Assert(p6.Contains("test123"), unrealClass, "AssertSetTest.p6");
            Tests.Assert(p7.Contains(new FName("test1234")), unrealClass, "AssertSetTest.p7");
        }

        [UFunction]
        public long SetFunc1(
            ISet<long> p1,
            ISet<UObject> p2,
            ISet<Test_SimpleEnum> p3,
            ISet<Test_SimpleBlittableStruct> p4,
            ISet<TSoftClass<UObject>> p5,
            ISet<string> p6,
            ISet<FName> p7)
        {
            AssertSetTest(this, p1, p2, p3, p4, p5, p6, p7);
            return 13232;
        }

        [UFunction]
        public long SetFunc2(
            ref ISet<long> p1,
            ref ISet<UObject> p2,
            ref ISet<Test_SimpleEnum> p3,
            ref ISet<Test_SimpleBlittableStruct> p4,
            ref ISet<TSoftClass<UObject>> p5,
            ref ISet<string> p6,
            ref ISet<FName> p7)
        {
            AssertSetTest(this, p1, p2, p3, p4, p5, p6, p7);
            return 13232;
        }

        [UFunction]
        public long SetFunc3(
            ref ISet<long> p1,
            ref ISet<UObject> p2,
            ref ISet<Test_SimpleEnum> p3,
            ref ISet<Test_SimpleBlittableStruct> p4,
            ref ISet<TSoftClass<UObject>> p5,
            ref ISet<string> p6,
            ref ISet<FName> p7)
        {
            Tests.AssertEqual(p1.Count, 0, GetClass(), "SetFunc3 ref empty map");
            Tests.AssertEqual(p2.Count, 0, GetClass(), "SetFunc3 ref empty map");
            Tests.AssertEqual(p3.Count, 0, GetClass(), "SetFunc3 ref empty map");
            Tests.AssertEqual(p4.Count, 0, GetClass(), "SetFunc3 ref empty map");
            Tests.AssertEqual(p5.Count, 0, GetClass(), "SetFunc3 ref empty map");
            Tests.AssertEqual(p6.Count, 0, GetClass(), "SetFunc3 ref empty map");
            Tests.AssertEqual(p7.Count, 0, GetClass(), "SetFunc3 ref empty map");
            MakeSetTest(p1, p2, p3, p4, p5, p6, p7);
            AssertSetTest(this, p1, p2, p3, p4, p5, p6, p7);
            return 13232;
        }

        [UFunction]
        public long SetFunc4(
            out ISet<long> p1,
            out ISet<UObject> p2,
            out ISet<Test_SimpleEnum> p3,
            out ISet<Test_SimpleBlittableStruct> p4,
            out ISet<TSoftClass<UObject>> p5,
            out ISet<string> p6,
            out ISet<FName> p7)
        {
            p1 = new HashSet<long>();
            p2 = new HashSet<UObject>();
            p3 = new HashSet<Test_SimpleEnum>();
            p4 = new HashSet<Test_SimpleBlittableStruct>();
            p5 = new HashSet<TSoftClass<UObject>>();
            p6 = new HashSet<string>();
            p7 = new HashSet<FName>();
            MakeSetTest(p1, p2, p3, p4, p5, p6, p7);
            return 13232;
        }

        [UFunction]
        public HashSet<TSoftClass<UObject>> SetFunc5(ref string inStr)
        {
            Tests.AssertEqual(inStr, "inStr", GetClass(), "SetFunc5.inStr");
            inStr = "outStr";

            HashSet<TSoftClass<UObject>> result = new HashSet<TSoftClass<UObject>>();
            result.Add(TSoftClass<UObject>.Null);
            result.Add(new TSoftClass<UObject>(Tests.ActorClass));
            return result;
        }

        ///////////////////////////////////////////////////////////////////
        // TMap
        ///////////////////////////////////////////////////////////////////

        [UFunctionIgnore]
        public static void MakeMapTest(
            IDictionary<long, string> p1,
            IDictionary<UObject, string> p2,
            IDictionary<Test_SimpleEnum, string> p3,
            IDictionary<Test_SimpleBlittableStruct, UObject> p4,
            IDictionary<TSoftClass<UObject>, TSoftClass<UObject>> p5,
            IDictionary<string, string> p6,
            IDictionary<FName, string> p7)
        {
            p1[10] = "tp1";
            p2[Tests.ActorClass] = "tp2";
            p3[Test_SimpleEnum.Val3] = "tp3";
            p4[new Test_SimpleBlittableStruct() { Val3 = 99 }] = Tests.ActorClass;
            p5[TSoftClass<UObject>.Null] = new TSoftClass<UObject>(Tests.ActorClass);
            p5[new TSoftClass<UObject>(Tests.ActorClass)] = TSoftClass<UObject>.Null;
            p6["test123"] = "tp6";
            p7[new FName("test1234")] = "tp7";
        }

        [UFunctionIgnore]
        public static void AssertMapTest(UObject obj,
            IDictionary<long, string> p1,
            IDictionary<UObject, string> p2,
            IDictionary<Test_SimpleEnum, string> p3,
            IDictionary<Test_SimpleBlittableStruct, UObject> p4,
            IDictionary<TSoftClass<UObject>, TSoftClass<UObject>> p5,
            IDictionary<string, string> p6,
            IDictionary<FName, string> p7)
        {
            UClass unrealClass = obj.GetClass();

            Tests.AssertEqual(p1[10], "tp1", unrealClass, "AssertMapTest.p1");
            Tests.AssertEqual(p2[Tests.ActorClass], "tp2", unrealClass, "AssertMapTest.p2");
            Tests.AssertEqual(p3[Test_SimpleEnum.Val3], "tp3", unrealClass, "AssertMapTest.p3");
            Tests.AssertEqual(p4[new Test_SimpleBlittableStruct() { Val3 = 99 }], Tests.ActorClass, unrealClass, "AssertMapTest.p4");
            Tests.AssertEqual(p5[TSoftClass<UObject>.Null], new TSoftClass<UObject>(Tests.ActorClass), unrealClass, "AssertMapTest.p5");
            Tests.AssertEqual(p5[new TSoftClass<UObject>(Tests.ActorClass)], TSoftClass<UObject>.Null, unrealClass, "AssertMapTest.p5");
            Tests.AssertEqual(p6["test123"], "tp6", unrealClass, "AssertMapTest.p6");
            Tests.AssertEqual(p7[new FName("test1234")], "tp7", unrealClass, "AssertMapTest.p7");
        }

        [UFunction]
        public long MapFunc1(
            IDictionary<long, string> p1,
            IDictionary<UObject, string> p2,
            IDictionary<Test_SimpleEnum, string> p3,
            IDictionary<Test_SimpleBlittableStruct, UObject> p4,
            IDictionary<TSoftClass<UObject>, TSoftClass<UObject>> p5,
            IDictionary<string, string> p6,
            IDictionary<FName, string> p7)
        {
            AssertMapTest(this, p1, p2, p3, p4, p5, p6, p7);
            return 13232;
        }

        [UFunction]
        public long MapFunc2(
            ref IDictionary<long, string> p1,
            ref IDictionary<UObject, string> p2,
            ref IDictionary<Test_SimpleEnum, string> p3,
            ref IDictionary<Test_SimpleBlittableStruct, UObject> p4,
            ref IDictionary<TSoftClass<UObject>, TSoftClass<UObject>> p5,
            ref IDictionary<string, string> p6,
            ref IDictionary<FName, string> p7)
        {
            AssertMapTest(this, p1, p2, p3, p4, p5, p6, p7);
            return 13232;
        }

        [UFunction]
        public long MapFunc3(
            ref IDictionary<long, string> p1,
            ref IDictionary<UObject, string> p2,
            ref IDictionary<Test_SimpleEnum, string> p3,
            ref IDictionary<Test_SimpleBlittableStruct, UObject> p4,
            ref IDictionary<TSoftClass<UObject>, TSoftClass<UObject>> p5,
            ref IDictionary<string, string> p6,
            ref IDictionary<FName, string> p7)
        {
            Tests.AssertEqual(p1.Count, 0, GetClass(), "MapFunc3 ref empty map");
            Tests.AssertEqual(p2.Count, 0, GetClass(), "MapFunc3 ref empty map");
            Tests.AssertEqual(p3.Count, 0, GetClass(), "MapFunc3 ref empty map");
            Tests.AssertEqual(p4.Count, 0, GetClass(), "MapFunc3 ref empty map");
            Tests.AssertEqual(p5.Count, 0, GetClass(), "MapFunc3 ref empty map");
            Tests.AssertEqual(p6.Count, 0, GetClass(), "MapFunc3 ref empty map");
            Tests.AssertEqual(p7.Count, 0, GetClass(), "MapFunc3 ref empty map");
            MakeMapTest(p1, p2, p3, p4, p5, p6, p7);
            AssertMapTest(this, p1, p2, p3, p4, p5, p6, p7);
            return 13232;
        }

        [UFunction]
        public long MapFunc4(
            out IDictionary<long, string> p1,
            out IDictionary<UObject, string> p2,
            out IDictionary<Test_SimpleEnum, string> p3,
            out IDictionary<Test_SimpleBlittableStruct, UObject> p4,
            out IDictionary<TSoftClass<UObject>, TSoftClass<UObject>> p5,
            out IDictionary<string, string> p6,
            out IDictionary<FName, string> p7)
        {
            p1 = new Dictionary<long, string>();
            p2 = new Dictionary<UObject, string>();
            p3 = new Dictionary<Test_SimpleEnum, string>();
            p4 = new Dictionary<Test_SimpleBlittableStruct, UObject>();
            p5 = new Dictionary<TSoftClass<UObject>, TSoftClass<UObject>>();
            p6 = new Dictionary<string, string>();
            p7 = new Dictionary<FName, string>();
            MakeMapTest(p1, p2, p3, p4, p5, p6, p7);
            return 13232;
        }

        [UFunction]
        public Dictionary<TSoftClass<UObject>, TSoftClass<UObject>> MapFunc5(ref string inStr)
        {
            Tests.AssertEqual(inStr, "inStr", GetClass(), "MapFunc5.inStr");
            inStr = "outStr";

            Dictionary<TSoftClass<UObject>, TSoftClass<UObject>> result = new Dictionary<TSoftClass<UObject>, TSoftClass<UObject>>();
            result[TSoftClass<UObject>.Null] = new TSoftClass<UObject>(Tests.ActorClass);
            result[new TSoftClass<UObject>(Tests.ActorClass)] = TSoftClass<UObject>.Null;
            return result;
        }
    }
}
#endif
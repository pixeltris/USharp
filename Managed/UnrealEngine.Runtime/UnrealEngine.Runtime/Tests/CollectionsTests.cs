#if WITH_USHARP_TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Tests
{
    static class CollectionsTests
    {
        public static void Run()
        {
            Test_SimpleStructTArray.Run();
            Test_SimpleStructTSet.Run();
            Test_SimpleStructTMap.Run();

            Test_SimpleClassTArray.Run();
            Test_SimpleClassTSet.Run();
            Test_SimpleClassTMap.Run();
        }
    }

    [UStruct]
    struct Test_SimpleStructTArray
    {
        public List<sbyte> Val1;
        public List<byte> Val2;
        public List<short> Val3;
        public List<ushort> Val4;
        public List<int> Val5;
        public List<uint> Val6;
        public List<long> Val7;
        public List<ulong> Val8;
        public List<float> Val9;
        public List<double> Val10;
        public List<Test_SimpleDelegate> Val11;
        public List<Test_SimpleMulticastDelegate> Val12;
        public List<UObject> Val13;
        public List<Test_SimpleEnum> Val14;
        public List<Test_SimpleBlittableStruct> Val15;
        public List<TSubclassOf<UObject>> Val16;
        public List<TLazyObject<UObject>> Val17;
        public List<TWeakObject<UObject>> Val18;
        public List<TSoftClass<UObject>> Val19;
        public List<TSoftObject<UObject>> Val20;
        public List<string> Val21;
        public List<FName> Val22;
        //public List<FText> Val23;

        public static void Run()
        {
            UScriptStruct unrealStruct = UScriptStruct.GetStruct<Test_SimpleStructTArray>();
            Tests.Assert(unrealStruct != null, "Test_SimpleStructTArray");

            Tests.AssertTArrayProperty<UInt8Property>(unrealStruct, "Val1");
            Tests.AssertTArrayProperty<UByteProperty>(unrealStruct, "Val2");
            Tests.AssertTArrayProperty<UInt16Property>(unrealStruct, "Val3");
            Tests.AssertTArrayProperty<UUInt16Property>(unrealStruct, "Val4");
            Tests.AssertTArrayProperty<UIntProperty>(unrealStruct, "Val5");
            Tests.AssertTArrayProperty<UUInt32Property>(unrealStruct, "Val6");
            Tests.AssertTArrayProperty<UInt64Property>(unrealStruct, "Val7");
            Tests.AssertTArrayProperty<UUInt64Property>(unrealStruct, "Val8");
            Tests.AssertTArrayProperty<UFloatProperty>(unrealStruct, "Val9");
            Tests.AssertTArrayProperty<UDoubleProperty>(unrealStruct, "Val10");
            Tests.AssertTArrayProperty<UDelegateProperty>(unrealStruct, "Val11");
            Tests.AssertTArrayProperty<UMulticastDelegateProperty>(unrealStruct, "Val12");
            Tests.AssertTArrayProperty<UObjectProperty>(unrealStruct, "Val13");
            Tests.AssertTArrayProperty<UEnumProperty>(unrealStruct, "Val14");
            Tests.AssertTArrayProperty<UStructProperty>(unrealStruct, "Val15");
            Tests.AssertTArrayProperty<UClassProperty>(unrealStruct, "Val16");
            Tests.AssertTArrayProperty<ULazyObjectProperty>(unrealStruct, "Val17");
            Tests.AssertTArrayProperty<UWeakObjectProperty>(unrealStruct, "Val18");
            Tests.AssertTArrayProperty<USoftClassProperty>(unrealStruct, "Val19");
            Tests.AssertTArrayProperty<USoftObjectProperty>(unrealStruct, "Val20");
            Tests.AssertTArrayProperty<UStrProperty>(unrealStruct, "Val21");
            Tests.AssertTArrayProperty<UNameProperty>(unrealStruct, "Val22");
            //Tests.AssertTArrayProperty<UTextProperty>(unrealStruct, "Val23");
            
            Test_SimpleStructTArray defaultValue = StructDefault<Test_SimpleStructTArray>.Value;

            // Check all lists default to empty (they will be constructed by the marshaler)
            foreach (FieldInfo field in defaultValue.GetType().GetFields())
            {
                Tests.AssertEqual((field.GetValue(defaultValue) as System.Collections.IList).Count, 0,
                    unrealStruct, field.Name);
            }
        }
    }

    [UStruct]
    struct Test_SimpleStructTSet
    {
        public HashSet<sbyte> Val1;
        public HashSet<byte> Val2;
        public HashSet<short> Val3;
        public HashSet<ushort> Val4;
        public HashSet<int> Val5;
        public HashSet<uint> Val6;
        public HashSet<long> Val7;
        public HashSet<ulong> Val8;
        public HashSet<float> Val9;
        public HashSet<double> Val10;
        //public HashSet<Test_SimpleDelegate> Val11;// don't have GetTypeHash
        //public HashSet<Test_SimpleMulticastDelegate> Val12;// don't have GetTypeHash
        public HashSet<UObject> Val13;
        public HashSet<Test_SimpleEnum> Val14;
        public HashSet<Test_SimpleBlittableStruct> Val15;
        public HashSet<TSubclassOf<UObject>> Val16;
        public HashSet<TLazyObject<UObject>> Val17;
        //public HashSet<TWeakObject<UObject>> Val18;// don't have GetTypeHash
        public HashSet<TSoftClass<UObject>> Val19;
        public HashSet<TSoftObject<UObject>> Val20;
        public HashSet<string> Val21;
        public HashSet<FName> Val22;
        //public HashSet<FText> Val23;

        public static void Run()
        {
            UScriptStruct unrealStruct = UScriptStruct.GetStruct<Test_SimpleStructTSet>();
            Tests.Assert(unrealStruct != null, "Test_SimpleStructTSet");

            Tests.AssertTSetProperty<UInt8Property>(unrealStruct, "Val1");
            Tests.AssertTSetProperty<UByteProperty>(unrealStruct, "Val2");
            Tests.AssertTSetProperty<UInt16Property>(unrealStruct, "Val3");
            Tests.AssertTSetProperty<UUInt16Property>(unrealStruct, "Val4");
            Tests.AssertTSetProperty<UIntProperty>(unrealStruct, "Val5");
            Tests.AssertTSetProperty<UUInt32Property>(unrealStruct, "Val6");
            Tests.AssertTSetProperty<UInt64Property>(unrealStruct, "Val7");
            Tests.AssertTSetProperty<UUInt64Property>(unrealStruct, "Val8");
            Tests.AssertTSetProperty<UFloatProperty>(unrealStruct, "Val9");
            Tests.AssertTSetProperty<UDoubleProperty>(unrealStruct, "Val10");
            //Tests.AssertTSetProperty<UDelegateProperty>(unrealStruct, "Val11");
            //Tests.AssertTSetProperty<UMulticastDelegateProperty>(unrealStruct, "Val12");
            Tests.AssertTSetProperty<UObjectProperty>(unrealStruct, "Val13");
            Tests.AssertTSetProperty<UEnumProperty>(unrealStruct, "Val14");
            Tests.AssertTSetProperty<UStructProperty>(unrealStruct, "Val15");
            Tests.AssertTSetProperty<UClassProperty>(unrealStruct, "Val16");
            Tests.AssertTSetProperty<ULazyObjectProperty>(unrealStruct, "Val17");
            //Tests.AssertTSetProperty<UWeakObjectProperty>(unrealStruct, "Val18");
            Tests.AssertTSetProperty<USoftClassProperty>(unrealStruct, "Val19");
            Tests.AssertTSetProperty<USoftObjectProperty>(unrealStruct, "Val20");
            Tests.AssertTSetProperty<UStrProperty>(unrealStruct, "Val21");
            Tests.AssertTSetProperty<UNameProperty>(unrealStruct, "Val22");
            //Tests.AssertTSetProperty<UTextProperty>(unrealStruct, "Val23");

            Test_SimpleStructTSet defaultValue = StructDefault<Test_SimpleStructTSet>.Value;

            // Check all lists default to empty (they will be constructed by the marshaler)
            foreach (FieldInfo field in defaultValue.GetType().GetFields())
            {
                // HashSet doesn't have a Count we can easily get, obtain it using reflection
                object collection = field.GetValue(defaultValue);
                int count = (int)collection.GetType().GetProperty("Count").GetValue(collection);
                Tests.AssertEqual(count, 0, unrealStruct, field.Name);
            }
        }
    }

    [UStruct]
    struct Test_SimpleStructTMap
    {
        public Dictionary<sbyte, FName> Val1;
        public Dictionary<byte, string> Val2;
        public Dictionary<short, TSoftObject<UObject>> Val3;
        public Dictionary<ushort, TSoftClass<UObject>> Val4;
        public Dictionary<int, TSoftClass<UObject>> Val5;
        public Dictionary<uint, TWeakObject<UObject>> Val6;
        public Dictionary<long, TLazyObject<UObject>> Val7;
        public Dictionary<ulong, TSubclassOf<UObject>> Val8;
        public Dictionary<float, Test_SimpleBlittableStruct> Val9;
        public Dictionary<double, Test_SimpleEnum> Val10;
        //public Dictionary<Test_SimpleDelegate, UObject> Val11;// don't have GetTypeHash
        //public Dictionary<Test_SimpleMulticastDelegate, Test_SimpleMulticastDelegate> Val12;// don't have GetTypeHash
        public Dictionary<UObject, Test_SimpleDelegate> Val13;
        public Dictionary<Test_SimpleEnum, double> Val14;
        public Dictionary<Test_SimpleBlittableStruct, float> Val15;
        public Dictionary<TSubclassOf<UObject>, ulong> Val16;
        public Dictionary<TLazyObject<UObject>, long> Val17;
        ///public Dictionary<TWeakObject<UObject>, uint> Val18;// doesn't have GetTypeHash (C++ allows it but creates a runtime error)
        public Dictionary<TSoftClass<UObject>, int> Val19;
        public Dictionary<TSoftObject<UObject>, ushort> Val20;
        public Dictionary<string, short> Val21;
        public Dictionary<FName, byte> Val22;
        //public HashSet<FText> Val23;

        public static void Run()
        {
            UScriptStruct unrealStruct = UScriptStruct.GetStruct<Test_SimpleStructTMap>();
            Tests.Assert(unrealStruct != null, "Test_SimpleStructTMap");

            Tests.AssertTMapProperty<UInt8Property, UNameProperty>(unrealStruct, "Val1");
            Tests.AssertTMapProperty<UByteProperty, UStrProperty>(unrealStruct, "Val2");
            Tests.AssertTMapProperty<UInt16Property, USoftObjectProperty>(unrealStruct, "Val3");
            Tests.AssertTMapProperty<UUInt16Property, USoftClassProperty>(unrealStruct, "Val4");
            Tests.AssertTMapProperty<UIntProperty, USoftClassProperty>(unrealStruct, "Val5");
            Tests.AssertTMapProperty<UUInt32Property, UWeakObjectProperty>(unrealStruct, "Val6");
            Tests.AssertTMapProperty<UInt64Property, ULazyObjectProperty>(unrealStruct, "Val7");
            Tests.AssertTMapProperty<UUInt64Property, UClassProperty>(unrealStruct, "Val8");
            Tests.AssertTMapProperty<UFloatProperty, UStructProperty>(unrealStruct, "Val9");
            Tests.AssertTMapProperty<UDoubleProperty, UEnumProperty>(unrealStruct, "Val10");
            //Tests.AssertTMapProperty<UDelegateProperty, UObjectProperty>(unrealStruct, "Val11");
            //Tests.AssertTMapProperty<UMulticastDelegateProperty, UMulticastDelegateProperty>(unrealStruct, "Val12");
            Tests.AssertTMapProperty<UObjectProperty, UDelegateProperty>(unrealStruct, "Val13");
            Tests.AssertTMapProperty<UEnumProperty, UDoubleProperty>(unrealStruct, "Val14");
            Tests.AssertTMapProperty<UStructProperty, UFloatProperty>(unrealStruct, "Val15");
            Tests.AssertTMapProperty<UClassProperty, UUInt64Property>(unrealStruct, "Val16");
            Tests.AssertTMapProperty<ULazyObjectProperty, UInt64Property>(unrealStruct, "Val17");
            //Tests.AssertTMapProperty<UWeakObjectProperty, UUInt32Property>(unrealStruct, "Val18");
            Tests.AssertTMapProperty<USoftClassProperty, UIntProperty>(unrealStruct, "Val19");
            Tests.AssertTMapProperty<USoftObjectProperty, UUInt16Property>(unrealStruct, "Val20");
            Tests.AssertTMapProperty<UStrProperty, UInt16Property>(unrealStruct, "Val21");
            Tests.AssertTMapProperty<UNameProperty, UByteProperty>(unrealStruct, "Val22");
            //Tests.AssertTSetProperty<UTextProperty>(unrealStruct, "Val23");

            Test_SimpleStructTMap defaultValue = StructDefault<Test_SimpleStructTMap>.Value;

            // Check all lists default to empty (they will be constructed by the marshaler)
            foreach (FieldInfo field in defaultValue.GetType().GetFields())
            {
                Tests.AssertEqual((field.GetValue(defaultValue) as System.Collections.IDictionary).Count, 0,
                    unrealStruct, field.Name);
            }
        }
    }

    [UClass]
    class Test_SimpleClassTArray : UObject
    {
        public IList<sbyte> Val1 { get; set; }
        public IList<byte> Val2 { get; set; }
        public IList<short> Val3 { get; set; }
        public IList<ushort> Val4 { get; set; }
        public IList<int> Val5 { get; set; }
        public IList<uint> Val6 { get; set; }
        public IList<long> Val7 { get; set; }
        public IList<ulong> Val8 { get; set; }
        public IList<float> Val9 { get; set; }
        public IList<double> Val10 { get; set; }
        public IList<Test_SimpleDelegate> Val11 { get; set; }
        public IList<Test_SimpleMulticastDelegate> Val12 { get; set; }
        public IList<UObject> Val13 { get; set; }
        public IList<Test_SimpleEnum> Val14 { get; set; }
        public IList<Test_SimpleBlittableStruct> Val15 { get; set; }
        public IList<TSubclassOf<UObject>> Val16 { get; set; }
        public IList<TLazyObject<UObject>> Val17 { get; set; }
        public IList<TWeakObject<UObject>> Val18{ get; set; }
        public IList<TSoftClass<UObject>> Val19 { get; set; }
        public IList<TSoftObject<UObject>> Val20 { get; set; }
        public IList<string> Val21 { get; set; }
        public IList<FName> Val22 { get; set; }
        //public IList<FText> Val23 { get; set; }

        [UFunctionIgnore]
        public static void Run()
        {
            UClass unrealClass = UClass.GetClass<Test_SimpleClassTArray>();
            Tests.Assert(unrealClass != null, "Test_SimpleClassTArray");

            Tests.AssertTArrayProperty<UInt8Property>(unrealClass, "Val1");
            Tests.AssertTArrayProperty<UByteProperty>(unrealClass, "Val2");
            Tests.AssertTArrayProperty<UInt16Property>(unrealClass, "Val3");
            Tests.AssertTArrayProperty<UUInt16Property>(unrealClass, "Val4");
            Tests.AssertTArrayProperty<UIntProperty>(unrealClass, "Val5");
            Tests.AssertTArrayProperty<UUInt32Property>(unrealClass, "Val6");
            Tests.AssertTArrayProperty<UInt64Property>(unrealClass, "Val7");
            Tests.AssertTArrayProperty<UUInt64Property>(unrealClass, "Val8");
            Tests.AssertTArrayProperty<UFloatProperty>(unrealClass, "Val9");
            Tests.AssertTArrayProperty<UDoubleProperty>(unrealClass, "Val10");
            Tests.AssertTArrayProperty<UDelegateProperty>(unrealClass, "Val11");
            Tests.AssertTArrayProperty<UMulticastDelegateProperty>(unrealClass, "Val12");
            Tests.AssertTArrayProperty<UObjectProperty>(unrealClass, "Val13");
            Tests.AssertTArrayProperty<UEnumProperty>(unrealClass, "Val14");
            Tests.AssertTArrayProperty<UStructProperty>(unrealClass, "Val15");
            Tests.AssertTArrayProperty<UClassProperty>(unrealClass, "Val16");
            Tests.AssertTArrayProperty<ULazyObjectProperty>(unrealClass, "Val17");
            Tests.AssertTArrayProperty<UWeakObjectProperty>(unrealClass, "Val18");
            Tests.AssertTArrayProperty<USoftClassProperty>(unrealClass, "Val19");
            Tests.AssertTArrayProperty<USoftObjectProperty>(unrealClass, "Val20");
            Tests.AssertTArrayProperty<UStrProperty>(unrealClass, "Val21");
            Tests.AssertTArrayProperty<UNameProperty>(unrealClass, "Val22");
            //Tests.AssertTArrayProperty<UTextProperty>(unrealClass, "Val23");

            Test_SimpleClassTArray obj = NewObject<Test_SimpleClassTArray>();

            obj.Val1.Add(100);
            Tests.Assert(obj.Val1.Contains(100), unrealClass, "Val1");
            obj.Val1.Add(101);
            Tests.Assert(obj.Val1.Contains(101), unrealClass, "Val1");
            Tests.AssertEqual(obj.Val1.Count, 2, unrealClass, "Val1");
            obj.Val1.Remove(101);
            Tests.AssertEqual(obj.Val1.Count, 1, unrealClass, "Val1");
            obj.Val1.Clear();
            Tests.AssertEqual(obj.Val1.Count, 0, unrealClass, "Val1");

            obj.Val2.Add(102);
            obj.Val2.Add(103);
            Tests.Assert(obj.Val2.Contains(102), unrealClass, "Val2");
            Tests.Assert(obj.Val2.Contains(102), unrealClass, "Val2");
            Tests.AssertEqual(obj.Val2.Count, 2, unrealClass, "Val2");

            obj.Val3.Add(103);
            obj.Val3.Add(104);
            Tests.Assert(obj.Val3.Contains(103), unrealClass, "Val3");
            Tests.Assert(obj.Val3.Contains(104), unrealClass, "Val3");

            obj.Val4.Add(105);
            obj.Val4.Add(106);
            Tests.Assert(obj.Val4.Contains(105), unrealClass, "Val4");
            Tests.Assert(obj.Val4.Contains(106), unrealClass, "Val4");

            obj.Val5.Add(107);
            obj.Val5.Add(108);
            Tests.Assert(obj.Val5.Contains(107), unrealClass, "Val5");
            Tests.Assert(obj.Val5.Contains(108), unrealClass, "Val5");

            obj.Val6.Add(109);
            obj.Val6.Add(110);
            Tests.Assert(obj.Val6.Contains(109), unrealClass, "Val6");
            Tests.Assert(obj.Val6.Contains(110), unrealClass, "Val6");

            obj.Val7.Add(111);
            obj.Val7.Add(112);
            Tests.Assert(obj.Val7.Contains(111), unrealClass, "Val7");
            Tests.Assert(obj.Val7.Contains(112), unrealClass, "Val7");

            obj.Val8.Add(113);
            obj.Val8.Add(114);
            Tests.Assert(obj.Val8.Contains(113), unrealClass, "Val8");
            Tests.Assert(obj.Val8.Contains(114), unrealClass, "Val8");

            obj.Val9.Add(115.5f);
            obj.Val9.Add(116.5f);
            Tests.Assert(obj.Val9.Contains(115.5f), unrealClass, "Val9");
            Tests.Assert(obj.Val9.Contains(116.5f), unrealClass, "Val9");

            obj.Val10.Add(117.2);
            obj.Val10.Add(118.2);
            Tests.Assert(obj.Val10.Contains(117.2), unrealClass, "Val10");
            Tests.Assert(obj.Val10.Contains(118.2), unrealClass, "Val10");

            // TODO: Refactor delegates or implement IEquatable so that IndexOf works
            //Test_SimpleDelegate simpleDelegate = new Test_SimpleDelegate();
            //simpleDelegate.Bind(obj.BindSimpleDelegate);
            //obj.Val11.Add(simpleDelegate);
            //int indx = obj.Val11.IndexOf(simpleDelegate);
            //Tests.Assert(indx == 0 && obj.Val11[0].IsBound, unrealClass, "Val13");

            Tests.Assert(!obj.Val13.Contains(Tests.ActorClass), unrealClass, "Val13");
            Tests.Assert(!obj.Val13.Contains(null), unrealClass, "Val13");
            obj.Val13.Add(Tests.ActorClass);
            obj.Val13.Add(null);
            Tests.Assert(obj.Val13.Contains(Tests.ActorClass), unrealClass, "Val13");
            Tests.Assert(obj.Val13.Contains(null), unrealClass, "Val13");

            obj.Val14.Add(Test_SimpleEnum.Val3);
            obj.Val14.Add(Test_SimpleEnum.Val2);
            Tests.Assert(obj.Val14.Contains(Test_SimpleEnum.Val3), unrealClass, "Val14");
            Tests.Assert(obj.Val14.Contains(Test_SimpleEnum.Val2), unrealClass, "Val14");

            Test_SimpleBlittableStruct blitt1 = StructDefault<Test_SimpleBlittableStruct>.Value;
            blitt1.Val1 = 10;
            blitt1.Val2 = 11;
            obj.Val15.Add(blitt1);
            obj.Val15.Add(default(Test_SimpleBlittableStruct));
            Tests.Assert(obj.Val15.Contains(blitt1), unrealClass, "Val15");
            Tests.Assert(obj.Val15.Contains(default(Test_SimpleBlittableStruct)), unrealClass, "Val15");

            obj.Val16.Add(new TSubclassOf<UObject>(Tests.ActorClass));
            obj.Val16.Add(TSubclassOf<UObject>.Null);
            Tests.Assert(obj.Val16.Contains(new TSubclassOf<UObject>(Tests.ActorClass)), unrealClass, "Val16");
            Tests.Assert(obj.Val16.Contains(TSubclassOf<UObject>.Null), unrealClass, "Val16");

            obj.Val17.Add(new TLazyObject<UObject>(Tests.ActorClass));
            obj.Val17.Add(TLazyObject<UObject>.Null);
            Tests.Assert(obj.Val17.Contains(new TLazyObject<UObject>(Tests.ActorClass)), unrealClass, "Val17");
            Tests.Assert(obj.Val17.Contains(TLazyObject<UObject>.Null), unrealClass, "Val17");

            obj.Val18.Add(new TWeakObject<UObject>(Tests.ActorClass));
            obj.Val18.Add(TWeakObject<UObject>.Null);
            Tests.Assert(obj.Val18.Contains(new TWeakObject<UObject>(Tests.ActorClass)), unrealClass, "Val18");
            Tests.Assert(obj.Val18.Contains(TWeakObject<UObject>.Null), unrealClass, "Val18");

            obj.Val19.Add(new TSoftClass<UObject>(Tests.ActorClass));
            obj.Val19.Add(TSoftClass<UObject>.Null);
            Tests.Assert(obj.Val19.Contains(new TSoftClass<UObject>(Tests.ActorClass)), unrealClass, "Val19");
            Tests.Assert(obj.Val19.Contains(TSoftClass<UObject>.Null), unrealClass, "Val19");

            obj.Val20.Add(new TSoftObject<UObject>(Tests.ActorClass));
            obj.Val20.Add(TSoftObject<UObject>.Null);
            Tests.Assert(obj.Val20.Contains(new TSoftObject<UObject>(Tests.ActorClass)), unrealClass, "Val20");
            Tests.Assert(obj.Val20.Contains(TSoftObject<UObject>.Null), unrealClass, "Val20");

            obj.Val21.Add("test1");
            obj.Val21.Add("test2");
            Tests.Assert(obj.Val21.Contains("test1"), unrealClass, "Val21");
            Tests.Assert(obj.Val21.Contains("test2"), unrealClass, "Val21");

            obj.Val22.Add(new FName("test3"));
            obj.Val22.Add(new FName("test4"));
            Tests.Assert(obj.Val22.Contains(new FName("test3")), unrealClass, "Val22");
            Tests.Assert(obj.Val22.Contains(new FName("test4")), unrealClass, "Val22");
        }

        [UFunction]
        private TSoftClass<UObject> BindSimpleDelegate(int param1, string param2, ref double param3, out string param4)
        {
            param4 = "0";
            return TSoftClass<UObject>.Null;
        }
    }

    [UClass]
    class Test_SimpleClassTSet : UObject
    {
        public ISet<sbyte> Val1 { get; set; }
        public ISet<byte> Val2 { get; set; }
        public ISet<short> Val3 { get; set; }
        public ISet<ushort> Val4 { get; set; }
        public ISet<int> Val5 { get; set; }
        public ISet<uint> Val6 { get; set; }
        public ISet<long> Val7 { get; set; }
        public ISet<ulong> Val8 { get; set; }
        public ISet<float> Val9 { get; set; }
        public ISet<double> Val10 { get; set; }
        //public ISet<Test_SimpleDelegate> Val11 { get; set; }// don't have GetTypeHash
        //public ISet<Test_SimpleMulticastDelegate> Val12 { get; set; }// don't have GetTypeHash
        public ISet<UObject> Val13 { get; set; }
        public ISet<Test_SimpleEnum> Val14 { get; set; }
        public ISet<Test_SimpleBlittableStruct> Val15 { get; set; }
        public ISet<TSubclassOf<UObject>> Val16 { get; set; }
        public ISet<TLazyObject<UObject>> Val17 { get; set; }
        //public ISet<TWeakObject<UObject>> Val18{ get; set; }// doesn't have GetTypeHash (C++ allows it but creates a runtime error)
        public ISet<TSoftClass<UObject>> Val19 { get; set; }
        public ISet<TSoftObject<UObject>> Val20 { get; set; }
        public ISet<string> Val21 { get; set; }
        public ISet<FName> Val22 { get; set; }
        //public ISet<FText> Val23 { get; set; }

        [UFunctionIgnore]
        public static void Run()
        {
            UClass unrealClass = UClass.GetClass<Test_SimpleClassTSet>();
            Tests.Assert(unrealClass != null, "Test_SimpleClassTSet");

            Tests.AssertTSetProperty<UInt8Property>(unrealClass, "Val1");
            Tests.AssertTSetProperty<UByteProperty>(unrealClass, "Val2");
            Tests.AssertTSetProperty<UInt16Property>(unrealClass, "Val3");
            Tests.AssertTSetProperty<UUInt16Property>(unrealClass, "Val4");
            Tests.AssertTSetProperty<UIntProperty>(unrealClass, "Val5");
            Tests.AssertTSetProperty<UUInt32Property>(unrealClass, "Val6");
            Tests.AssertTSetProperty<UInt64Property>(unrealClass, "Val7");
            Tests.AssertTSetProperty<UUInt64Property>(unrealClass, "Val8");
            Tests.AssertTSetProperty<UFloatProperty>(unrealClass, "Val9");
            Tests.AssertTSetProperty<UDoubleProperty>(unrealClass, "Val10");
            //Tests.AssertTSetProperty<UDelegateProperty>(unrealClass, "Val11");
            //Tests.AssertTSetProperty<UMulticastDelegateProperty>(unrealClass, "Val12");
            Tests.AssertTSetProperty<UObjectProperty>(unrealClass, "Val13");
            Tests.AssertTSetProperty<UEnumProperty>(unrealClass, "Val14");
            Tests.AssertTSetProperty<UStructProperty>(unrealClass, "Val15");
            Tests.AssertTSetProperty<UClassProperty>(unrealClass, "Val16");
            Tests.AssertTSetProperty<ULazyObjectProperty>(unrealClass, "Val17");
            //Tests.AssertTSetProperty<UWeakObjectProperty>(unrealClass, "Val18");
            Tests.AssertTSetProperty<USoftClassProperty>(unrealClass, "Val19");
            Tests.AssertTSetProperty<USoftObjectProperty>(unrealClass, "Val20");
            Tests.AssertTSetProperty<UStrProperty>(unrealClass, "Val21");
            Tests.AssertTSetProperty<UNameProperty>(unrealClass, "Val22");
            //Tests.AssertTSetProperty<UTextProperty>(unrealClass, "Val23");

            Test_SimpleClassTSet obj = NewObject<Test_SimpleClassTSet>();

            obj.Val1.Add(100);
            Tests.Assert(obj.Val1.Contains(100), unrealClass, "Val1");
            obj.Val1.Add(101);
            Tests.Assert(obj.Val1.Contains(101), unrealClass, "Val1");
            Tests.Assert(!obj.Val1.Add(101), unrealClass, "Val1");
            Tests.AssertEqual(obj.Val1.Count, 2, unrealClass, "Val1");
            obj.Val1.Remove(101);
            Tests.AssertEqual(obj.Val1.Count, 1, unrealClass, "Val1");
            obj.Val1.Clear();
            Tests.AssertEqual(obj.Val1.Count, 0, unrealClass, "Val1");

            obj.Val2.Add(102);
            obj.Val2.Add(103);
            Tests.Assert(obj.Val2.Contains(102), unrealClass, "Val2");
            Tests.Assert(obj.Val2.Contains(102), unrealClass, "Val2");
            Tests.AssertEqual(obj.Val2.Count, 2, unrealClass, "Val2");

            obj.Val3.Add(103);
            obj.Val3.Add(104);
            Tests.Assert(obj.Val3.Contains(103), unrealClass, "Val3");
            Tests.Assert(obj.Val3.Contains(104), unrealClass, "Val3");

            obj.Val4.Add(105);
            obj.Val4.Add(106);
            Tests.Assert(obj.Val4.Contains(105), unrealClass, "Val4");
            Tests.Assert(obj.Val4.Contains(106), unrealClass, "Val4");

            obj.Val5.Add(107);
            obj.Val5.Add(108);
            Tests.Assert(obj.Val5.Contains(107), unrealClass, "Val5");
            Tests.Assert(obj.Val5.Contains(108), unrealClass, "Val5");

            obj.Val6.Add(109);
            obj.Val6.Add(110);
            Tests.Assert(obj.Val6.Contains(109), unrealClass, "Val6");
            Tests.Assert(obj.Val6.Contains(110), unrealClass, "Val6");

            obj.Val7.Add(111);
            obj.Val7.Add(112);
            Tests.Assert(obj.Val7.Contains(111), unrealClass, "Val7");
            Tests.Assert(obj.Val7.Contains(112), unrealClass, "Val7");

            obj.Val8.Add(113);
            obj.Val8.Add(114);
            Tests.Assert(obj.Val8.Contains(113), unrealClass, "Val8");
            Tests.Assert(obj.Val8.Contains(114), unrealClass, "Val8");
            
            obj.Val9.Add(115.5f);
            obj.Val9.Add(116.5f);
            Tests.Assert(obj.Val9.Contains(115.5f), unrealClass, "Val9");
            Tests.Assert(obj.Val9.Contains(116.5f), unrealClass, "Val9");

            obj.Val10.Add(117.2);
            obj.Val10.Add(118.2);
            Tests.Assert(obj.Val10.Contains(117.2), unrealClass, "Val10");
            Tests.Assert(obj.Val10.Contains(118.2), unrealClass, "Val10");

            Tests.Assert(!obj.Val13.Contains(Tests.ActorClass), unrealClass, "Val13");
            Tests.Assert(!obj.Val13.Contains(null), unrealClass, "Val13");
            obj.Val13.Add(Tests.ActorClass);
            obj.Val13.Add(null);
            Tests.Assert(obj.Val13.Contains(Tests.ActorClass), unrealClass, "Val13");
            Tests.Assert(obj.Val13.Contains(null), unrealClass, "Val13");

            obj.Val14.Add(Test_SimpleEnum.Val3);
            obj.Val14.Add(Test_SimpleEnum.Val2);
            Tests.Assert(obj.Val14.Contains(Test_SimpleEnum.Val3), unrealClass, "Val14");
            Tests.Assert(obj.Val14.Contains(Test_SimpleEnum.Val2), unrealClass, "Val14");

            Test_SimpleBlittableStruct blitt1 = StructDefault<Test_SimpleBlittableStruct>.Value;
            blitt1.Val1 = 10;
            blitt1.Val2 = 11;
            obj.Val15.Add(blitt1);
            obj.Val15.Add(default(Test_SimpleBlittableStruct));
            Tests.Assert(obj.Val15.Contains(blitt1), unrealClass, "Val15");
            Tests.Assert(obj.Val15.Contains(default(Test_SimpleBlittableStruct)), unrealClass, "Val15");

            obj.Val16.Add(new TSubclassOf<UObject>(Tests.ActorClass));
            obj.Val16.Add(TSubclassOf<UObject>.Null);
            Tests.Assert(obj.Val16.Contains(new TSubclassOf<UObject>(Tests.ActorClass)), unrealClass, "Val16");
            Tests.Assert(obj.Val16.Contains(TSubclassOf<UObject>.Null), unrealClass, "Val16");

            obj.Val17.Add(new TLazyObject<UObject>(Tests.ActorClass));
            obj.Val17.Add(TLazyObject<UObject>.Null);
            Tests.Assert(obj.Val17.Contains(new TLazyObject<UObject>(Tests.ActorClass)), unrealClass, "Val17");
            Tests.Assert(obj.Val17.Contains(TLazyObject<UObject>.Null), unrealClass, "Val17");

            obj.Val19.Add(new TSoftClass<UObject>(Tests.ActorClass));
            obj.Val19.Add(TSoftClass<UObject>.Null);
            Tests.Assert(obj.Val19.Contains(new TSoftClass<UObject>(Tests.ActorClass)), unrealClass, "Val19");
            Tests.Assert(obj.Val19.Contains(TSoftClass<UObject>.Null), unrealClass, "Val19");

            obj.Val20.Add(new TSoftObject<UObject>(Tests.ActorClass));
            obj.Val20.Add(TSoftObject<UObject>.Null);
            Tests.Assert(obj.Val20.Contains(new TSoftObject<UObject>(Tests.ActorClass)), unrealClass, "Val20");
            Tests.Assert(obj.Val20.Contains(TSoftObject<UObject>.Null), unrealClass, "Val20");

            obj.Val21.Add("test1");
            obj.Val21.Add("test2");
            Tests.Assert(obj.Val21.Contains("test1"), unrealClass, "Val21");
            Tests.Assert(obj.Val21.Contains("test2"), unrealClass, "Val21");

            obj.Val22.Add(new FName("test3"));
            obj.Val22.Add(new FName("test4"));
            Tests.Assert(obj.Val22.Contains(new FName("test3")), unrealClass, "Val22");
            Tests.Assert(obj.Val22.Contains(new FName("test4")), unrealClass, "Val22");
        }
    }

    [UClass]
    class Test_SimpleClassTMap : UObject
    {
        public IDictionary<sbyte, FName> Val1 { get; set; }
        public IDictionary<byte, string> Val2 { get; set; }
        public IDictionary<short, TSoftObject<UObject>> Val3 { get; set; }
        public IDictionary<ushort, TSoftClass<UObject>> Val4 { get; set; }
        public IDictionary<int, TSoftClass<UObject>> Val5 { get; set; }
        public IDictionary<uint, TWeakObject<UObject>> Val6 { get; set; }
        public IDictionary<long, TLazyObject<UObject>> Val7 { get; set; }
        public IDictionary<ulong, TSubclassOf<UObject>> Val8 { get; set; }
        public IDictionary<float, Test_SimpleBlittableStruct> Val9 { get; set; }
        public IDictionary<double, Test_SimpleEnum> Val10 { get; set; }
        //public IDictionary<Test_SimpleDelegate, UObject> Val11 { get; set; }// don't have GetTypeHash
        //public IDictionary<Test_SimpleMulticastDelegate, Test_SimpleMulticastDelegate> Val12 { get; set; }// don't have GetTypeHash
        public IDictionary<UObject, Test_SimpleDelegate> Val13 { get; set; }
        public IDictionary<Test_SimpleEnum, double> Val14 { get; set; }
        public IDictionary<Test_SimpleBlittableStruct, float> Val15 { get; set; }
        public IDictionary<TSubclassOf<UObject>, ulong> Val16 { get; set; }
        public IDictionary<TLazyObject<UObject>, long> Val17 { get; set; }
        //public IDictionary<TWeakObject<UObject>, uint> Val18{ get; set; }// doesn't have GetTypeHash (C++ allows it but creates a runtime error)
        public IDictionary<TSoftClass<UObject>, int> Val19 { get; set; }
        public IDictionary<TSoftObject<UObject>, ushort> Val20 { get; set; }
        public IDictionary<string, short> Val21 { get; set; }
        public IDictionary<FName, byte> Val22 { get; set; }
        //public IDictionary<FText, string> Val23 { get; set; }

        [UFunctionIgnore]
        public static void Run()
        {
            UClass unrealClass = UClass.GetClass<Test_SimpleClassTMap>();
            Tests.Assert(unrealClass != null, "Test_SimpleClassTMap");

            Tests.AssertTMapProperty<UInt8Property, UNameProperty>(unrealClass, "Val1");
            Tests.AssertTMapProperty<UByteProperty, UStrProperty>(unrealClass, "Val2");
            Tests.AssertTMapProperty<UInt16Property, USoftObjectProperty>(unrealClass, "Val3");
            Tests.AssertTMapProperty<UUInt16Property, USoftClassProperty>(unrealClass, "Val4");
            Tests.AssertTMapProperty<UIntProperty, USoftClassProperty>(unrealClass, "Val5");
            Tests.AssertTMapProperty<UUInt32Property, UWeakObjectProperty>(unrealClass, "Val6");
            Tests.AssertTMapProperty<UInt64Property, ULazyObjectProperty>(unrealClass, "Val7");
            Tests.AssertTMapProperty<UUInt64Property, UClassProperty>(unrealClass, "Val8");
            Tests.AssertTMapProperty<UFloatProperty, UStructProperty>(unrealClass, "Val9");
            Tests.AssertTMapProperty<UDoubleProperty, UEnumProperty>(unrealClass, "Val10");
            //Tests.AssertTMapProperty<UDelegateProperty, UObjectProperty>(unrealClass, "Val11");
            //Tests.AssertTMapProperty<UMulticastDelegateProperty, UMulticastDelegateProperty>(unrealClass, "Val12");
            Tests.AssertTMapProperty<UObjectProperty, UDelegateProperty>(unrealClass, "Val13");
            Tests.AssertTMapProperty<UEnumProperty, UDoubleProperty>(unrealClass, "Val14");
            Tests.AssertTMapProperty<UStructProperty, UFloatProperty>(unrealClass, "Val15");
            Tests.AssertTMapProperty<UClassProperty, UUInt64Property>(unrealClass, "Val16");
            Tests.AssertTMapProperty<ULazyObjectProperty, UInt64Property>(unrealClass, "Val17");
            //Tests.AssertTMapProperty<UWeakObjectProperty, UUInt32Property>(unrealClass, "Val18");
            Tests.AssertTMapProperty<USoftClassProperty, UIntProperty>(unrealClass, "Val19");
            Tests.AssertTMapProperty<USoftObjectProperty, UUInt16Property>(unrealClass, "Val20");
            Tests.AssertTMapProperty<UStrProperty, UInt16Property>(unrealClass, "Val21");
            Tests.AssertTMapProperty<UNameProperty, UByteProperty>(unrealClass, "Val22");
            //Tests.AssertTSetProperty<UTextProperty>(unrealClass, "Val23");

            Test_SimpleClassTMap obj = NewObject<Test_SimpleClassTMap>();

            obj.Val1.Add(100, new FName("test1"));
            Tests.AssertEqual(obj.Val1[100], new FName("test1"), unrealClass, "Val1");
            obj.Val1.Add(101, new FName("test2"));
            Tests.AssertEqual(obj.Val1[101], new FName("test2"), unrealClass, "Val1");
            obj.Val1.Add(101, new FName("test3"));
            IDictionary<sbyte, FName> test = obj.Val1;
            Tests.AssertEqual(obj.Val1[101], new FName("test3"), unrealClass, "Val1");
            Tests.AssertEqual(obj.Val1.Count, 2, unrealClass, "Val1");
            obj.Val1.Remove(101);
            Tests.AssertEqual(obj.Val1.Count, 1, unrealClass, "Val1");
            obj.Val1.Clear();
            Tests.AssertEqual(obj.Val1.Count, 0, unrealClass, "Val1");

            obj.Val2.Add(102, "test4");
            obj.Val2.Add(103, "test5");
            Tests.AssertEqual(obj.Val2[102], "test4", unrealClass, "Val2");
            Tests.AssertEqual(obj.Val2[103], "test5", unrealClass, "Val2");
            Tests.AssertEqual(obj.Val2.Count, 2, unrealClass, "Val2");

            obj.Val3.Add(103, new TSoftObject<UObject>(Tests.ActorClass));
            obj.Val3.Add(104, TSoftObject<UObject>.Null);
            Tests.AssertEqual(obj.Val3[103], new TSoftObject<UObject>(Tests.ActorClass), unrealClass, "Val3");
            Tests.AssertEqual(obj.Val3[104], TSoftObject<UObject>.Null, unrealClass, "Val3");
            obj.Val3[103] = TSoftObject<UObject>.Null;
            obj.Val3[104] = new TSoftObject<UObject>(Tests.ActorClass);
            Tests.AssertEqual(obj.Val3[103], TSoftObject<UObject>.Null, unrealClass, "Val3");
            Tests.AssertEqual(obj.Val3[104], new TSoftObject<UObject>(Tests.ActorClass), unrealClass, "Val3");

            obj.Val4.Add(105, new TSoftClass<UObject>(Tests.ActorClass));
            obj.Val4.Add(106, TSoftClass<UObject>.Null);
            Tests.AssertEqual(obj.Val4[105], new TSoftClass<UObject>(Tests.ActorClass), unrealClass, "Val4");
            Tests.AssertEqual(obj.Val4[106], TSoftClass<UObject>.Null, unrealClass, "Val4");

            obj.Val5.Add(107, new TSoftClass<UObject>(Tests.ActorClass));
            obj.Val5.Add(108, TSoftClass<UObject>.Null);
            Tests.AssertEqual(obj.Val5[107], new TSoftClass<UObject>(Tests.ActorClass), unrealClass, "Val5");
            Tests.AssertEqual(obj.Val5[108], TSoftClass<UObject>.Null, unrealClass, "Val5");

            obj.Val6.Add(109, new TWeakObject<UObject>(Tests.ActorClass));
            obj.Val6.Add(110, TWeakObject<UObject>.Null);
            Tests.AssertEqual(obj.Val6[109], new TWeakObject<UObject>(Tests.ActorClass), unrealClass, "Val6");
            Tests.AssertEqual(obj.Val6[110], TWeakObject<UObject>.Null, unrealClass, "Val6");

            obj.Val7.Add(111, new TLazyObject<UObject>(Tests.ActorClass));
            obj.Val7.Add(112, TLazyObject<UObject>.Null);
            Tests.AssertEqual(obj.Val7[111], new TLazyObject<UObject>(Tests.ActorClass), unrealClass, "Val7");
            Tests.AssertEqual(obj.Val7[112], TLazyObject<UObject>.Null, unrealClass, "Val7");

            obj.Val8.Add(113, new TSubclassOf<UObject>(Tests.ActorClass));
            obj.Val8.Add(114, TSubclassOf<UObject>.Null);
            Tests.AssertEqual(obj.Val8[113], new TSubclassOf<UObject>(Tests.ActorClass), unrealClass, "Val8");
            Tests.AssertEqual(obj.Val8[114], TSubclassOf<UObject>.Null, unrealClass, "Val8");

            Test_SimpleBlittableStruct blitt1 = StructDefault<Test_SimpleBlittableStruct>.Value;
            blitt1.Val1 = 10;
            blitt1.Val2 = 11;
            obj.Val9.Add(115.5f, blitt1);
            obj.Val9.Add(116.5f, default(Test_SimpleBlittableStruct));
            Tests.AssertEqual(obj.Val9[115.5f], blitt1, unrealClass, "Val9");
            Tests.AssertEqual(obj.Val9[116.5f], default(Test_SimpleBlittableStruct), unrealClass, "Val9");

            obj.Val10.Add(117.2, Test_SimpleEnum.Val2);
            obj.Val10.Add(118.2, Test_SimpleEnum.Val3);
            Tests.AssertEqual(obj.Val10[117.2], Test_SimpleEnum.Val2, unrealClass, "Val10");
            Tests.AssertEqual(obj.Val10[118.2], Test_SimpleEnum.Val3, unrealClass, "Val10");

            Test_SimpleDelegate simpleDelegate = new Test_SimpleDelegate();
            simpleDelegate.Bind(obj.BindSimpleDelegate);
            obj.Val13.Add(Tests.ActorClass, simpleDelegate);
            obj.Val13.Add(null, null);
            Tests.Assert(obj.Val13[Tests.ActorClass].IsTargetBound(obj, new FName("BindSimpleDelegate")), unrealClass, "Val13");
            Tests.Assert(!obj.Val13[null].IsBound, unrealClass, "Val13");

            obj.Val14.Add(Test_SimpleEnum.Val3, 0.3);
            obj.Val14.Add(Test_SimpleEnum.Val2, 0.6);
            Tests.AssertEqual(obj.Val14[Test_SimpleEnum.Val3], 0.3, unrealClass, "Val14");
            Tests.AssertEqual(obj.Val14[Test_SimpleEnum.Val2], 0.6, unrealClass, "Val14");

            obj.Val15.Add(blitt1, 0.4f);
            obj.Val15.Add(default(Test_SimpleBlittableStruct), 0.8f);
            Tests.AssertEqual(obj.Val15[blitt1], 0.4f, unrealClass, "Val15");
            Tests.AssertEqual(obj.Val15[default(Test_SimpleBlittableStruct)], 0.8f, unrealClass, "Val15");

            obj.Val16.Add(new TSubclassOf<UObject>(Tests.ActorClass), 22u);
            obj.Val16.Add(TSubclassOf<UObject>.Null, 24u);
            Tests.AssertEqual(obj.Val16[new TSubclassOf<UObject>(Tests.ActorClass)], 22u, unrealClass, "Val16");
            Tests.AssertEqual(obj.Val16[TSubclassOf<UObject>.Null], 24u, unrealClass, "Val16");

            obj.Val17.Add(new TLazyObject<UObject>(Tests.ActorClass), 25);
            obj.Val17.Add(TLazyObject<UObject>.Null, 26);
            Tests.AssertEqual(obj.Val17[new TLazyObject<UObject>(Tests.ActorClass)], 25, unrealClass, "Val17");
            Tests.AssertEqual(obj.Val17[TLazyObject<UObject>.Null], 26, unrealClass, "Val17");

            obj.Val19.Add(new TSoftClass<UObject>(Tests.ActorClass), 29);
            obj.Val19.Add(TSoftClass<UObject>.Null, 30);
            Tests.AssertEqual(obj.Val19[new TSoftClass<UObject>(Tests.ActorClass)], 29, unrealClass, "Val19");
            Tests.AssertEqual(obj.Val19[TSoftClass<UObject>.Null], 30, unrealClass, "Val19");

            obj.Val20.Add(new TSoftObject<UObject>(Tests.ActorClass), 31);
            obj.Val20.Add(TSoftObject<UObject>.Null, 32);
            Tests.AssertEqual(obj.Val20[new TSoftObject<UObject>(Tests.ActorClass)], 31, unrealClass, "Val20");
            Tests.AssertEqual(obj.Val20[TSoftObject<UObject>.Null], 32, unrealClass, "Val20");

            obj.Val21.Add("test1", 33);
            obj.Val21.Add("test2", 34);
            Tests.AssertEqual(obj.Val21["test1"], 33, unrealClass, "Val21");
            Tests.AssertEqual(obj.Val21["test2"], 34, unrealClass, "Val21");

            obj.Val22.Add(new FName("test3"), 35);
            obj.Val22.Add(new FName("test4"), 36);
            Tests.AssertEqual(obj.Val22[new FName("test3")], 35, unrealClass, "Val22");
            Tests.AssertEqual(obj.Val22[new FName("test4")], 36, unrealClass, "Val22");
        }

        [UFunction]
        private TSoftClass<UObject> BindSimpleDelegate(int param1, string param2, ref double param3, out string param4)
        {
            param4 = "0";
            return TSoftClass<UObject>.Null;
        }
    }
}
#endif
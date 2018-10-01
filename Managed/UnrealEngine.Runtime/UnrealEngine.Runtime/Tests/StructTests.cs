#if WITH_USHARP_TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Tests
{
    static class StructTests
    {
        public static void Run()
        {
            Test_SimpleBlittableStruct.Run();
            Test_SimpleStruct.Run();
            Test_SimpleStructOnClass.Run();
        }
    }

    [UStruct]
    struct Test_SimpleBlittableStruct
    {
        public sbyte Val1;
        public byte Val2;
        public short Val3;
        public ushort Val4;
        public int Val5;
        public uint Val6;
        public long Val7;
        public ulong Val8;
        public float Val9;
        public double Val10;
        public FName Val11;

        // Could add another POD struct nested in here but it seems to not be blittable when
        // adding a POD struct into another POD struct.

        public static void Run()
        {
            UScriptStruct unrealStruct = UScriptStruct.GetStruct<Test_SimpleBlittableStruct>();
            Tests.Assert(unrealStruct != null, "Test_SimpleBlittableStruct");

            Tests.AssertProperty<UInt8Property>(unrealStruct, "Val1");
            Tests.AssertProperty<UByteProperty>(unrealStruct, "Val2");
            Tests.AssertProperty<UInt16Property>(unrealStruct, "Val3");
            Tests.AssertProperty<UUInt16Property>(unrealStruct, "Val4");
            Tests.AssertProperty<UIntProperty>(unrealStruct, "Val5");
            Tests.AssertProperty<UUInt32Property>(unrealStruct, "Val6");
            Tests.AssertProperty<UInt64Property>(unrealStruct, "Val7");
            Tests.AssertProperty<UUInt64Property>(unrealStruct, "Val8");
            Tests.AssertProperty<UFloatProperty>(unrealStruct, "Val9");
            Tests.AssertProperty<UDoubleProperty>(unrealStruct, "Val10");
            Tests.AssertProperty<UNameProperty>(unrealStruct, "Val11");

            Tests.Assert(StructDefault<Test_SimpleBlittableStruct>.IsBlittableStruct, unrealStruct, "IsBlittableStruct");
            Tests.Assert(unrealStruct.StructFlags.HasFlag(EStructFlags.IsPlainOldData), unrealStruct, "IsPlainOldData");

            Test_SimpleBlittableStruct defaultValue = StructDefault<Test_SimpleBlittableStruct>.Value;
            Tests.AssertEqual(defaultValue, default(Test_SimpleBlittableStruct), unrealStruct, "default state");
        }
    }

    [UStruct]
    struct Test_SimpleStruct
    {
        public bool Val0;
        public sbyte Val1;
        public byte Val2;
        public short Val3;
        public ushort Val4;
        public int Val5;
        public uint Val6;
        public long Val7;
        public ulong Val8;
        public float Val9;
        public double Val10;
        public Test_SimpleDelegate Val11;
        public Test_SimpleMulticastDelegate Val12;
        public UObject Val13;
        public Test_SimpleEnum Val14;
        public Test_SimpleBlittableStruct Val15;
        public TSubclassOf<UObject> Val16;
        public TLazyObject<UObject> Val17;
        public TWeakObject<UObject> Val18;
        public TSoftClass<UObject> Val19;
        public TSoftObject<UObject> Val20;
        public string Val21;
        public FName Val22;
        //public FText Val23;

        public static void Run()
        {
            UScriptStruct unrealStruct = UScriptStruct.GetStruct<Test_SimpleStruct>();
            Tests.Assert(unrealStruct != null, "Test_SimpleStruct");

            Tests.AssertProperty<UBoolProperty>(unrealStruct, "Val0");
            Tests.AssertProperty<UInt8Property>(unrealStruct, "Val1");
            Tests.AssertProperty<UByteProperty>(unrealStruct, "Val2");
            Tests.AssertProperty<UInt16Property>(unrealStruct, "Val3");
            Tests.AssertProperty<UUInt16Property>(unrealStruct, "Val4");
            Tests.AssertProperty<UIntProperty>(unrealStruct, "Val5");
            Tests.AssertProperty<UUInt32Property>(unrealStruct, "Val6");
            Tests.AssertProperty<UInt64Property>(unrealStruct, "Val7");
            Tests.AssertProperty<UUInt64Property>(unrealStruct, "Val8");
            Tests.AssertProperty<UFloatProperty>(unrealStruct, "Val9");
            Tests.AssertProperty<UDoubleProperty>(unrealStruct, "Val10");
            Tests.AssertProperty<UDelegateProperty>(unrealStruct, "Val11");
            Tests.AssertProperty<UMulticastDelegateProperty>(unrealStruct, "Val12");
            Tests.AssertProperty<UObjectProperty>(unrealStruct, "Val13");
            Tests.AssertProperty<UEnumProperty>(unrealStruct, "Val14");
            Tests.AssertProperty<UStructProperty>(unrealStruct, "Val15");
            Tests.AssertProperty<UClassProperty>(unrealStruct, "Val16");
            Tests.AssertProperty<ULazyObjectProperty>(unrealStruct, "Val17");
            Tests.AssertProperty<UWeakObjectProperty>(unrealStruct, "Val18");
            Tests.AssertProperty<USoftClassProperty>(unrealStruct, "Val19");
            Tests.AssertProperty<USoftObjectProperty>(unrealStruct, "Val20");
            Tests.AssertProperty<UStrProperty>(unrealStruct, "Val21");
            Tests.AssertProperty<UNameProperty>(unrealStruct, "Val22");
            //Tests.AssertProperty<UTextProperty>(unrealStruct, "Val23");
        }
    }

    /// <summary>
    /// UObject for testing Test_SimpleStruct when used as a member of a UObject / used on function params
    /// </summary>
    class Test_SimpleStructOnClass : UObject
    {
        public Test_SimpleBlittableStruct BlittableStruct { get; set; }
        public Test_SimpleStruct Struct { get; set; }

        // Make sure this signature matches Test_SimpleDelegate
        private TSoftClass<UObject> BindToStructDelegate(int param1, string param2, ref double param3, out string param4)
        {
            Tests.AssertEqual(param1, 13, "BindToStructDelegate.param1");
            Tests.AssertEqual(param2, "inParam2", "BindToStructDelegate.param2");
            Tests.AssertEqual(param3, 44.3, "BindToStructDelegate.param3");

            param2 = "outParam2";
            param3 = 99.88;
            param4 = "outParam4";

            return new TSoftClass<UObject>(UClass.GetClass<Test_SimpleStructOnClass>().GetPathName());
        }

        // Make sure this signature matches Test_SimpleMulticastDelegate
        private void BindToStructMulticastDelegate1(ulong param1, string param2, ref int param3)
        {
            Tests.AssertEqual(param1, 3u, "BindToStructMulticastDelegate1");
            Tests.AssertEqual(param2, "param2", "BindToStructMulticastDelegate1");
            Tests.Assert(param3 >= 1 && param3 <= 4, "BindToStructMulticastDelegate1");
            param3++;
        }

        // Make sure this signature matches Test_SimpleMulticastDelegate
        private void BindToStructMulticastDelegate2(ulong param1, string param2, ref int param3)
        {
            Tests.AssertEqual(param1, 3u, "BindToStructMulticastDelegate2");
            Tests.AssertEqual(param2, "param2", "BindToStructMulticastDelegate2");
            Tests.Assert(param3 >= 1 && param3 <= 4, "BindToStructMulticastDelegate2");
            param3++;
        }

        // Make sure this signature matches Test_SimpleMulticastDelegate
        private void BindToStructMulticastDelegate3(ulong param1, string param2, ref int param3)
        {
            Tests.AssertEqual(param1, 3u, "BindToStructMulticastDelegate3");
            Tests.AssertEqual(param2, "param2", "BindToStructMulticastDelegate3");
            Tests.Assert(param3 >= 1 && param3 <= 4, "BindToStructMulticastDelegate3");
            param3++;
        }

        [UFunctionIgnore]
        public static void Run()
        {
            Test_SimpleStructOnClass obj = UObject.NewObject<Test_SimpleStructOnClass>();
            Tests.Assert(obj != null, "Test_SimpleStructOnClass NewObject");

            UClass uclass = obj.GetClass();
            Tests.AssertEqual(uclass, UClass.GetClass(obj.GetType()), "Test_SimpleStructOnClass GetClass/Type");

            // Create another obj of the same type for comparisons of 
            Test_SimpleStructOnClass other = UObject.NewObject<Test_SimpleStructOnClass>();

            // Test BlittableStruct default state            
            string msg = "BlittableStruct default state";
            Tests.AssertDefault(obj.BlittableStruct.Val1, uclass, msg);
            Tests.AssertDefault(obj.BlittableStruct.Val2, uclass, msg);
            Tests.AssertDefault(obj.BlittableStruct.Val3, uclass, msg);
            Tests.AssertDefault(obj.BlittableStruct.Val4, uclass, msg);
            Tests.AssertDefault(obj.BlittableStruct.Val5, uclass, msg);
            Tests.AssertDefault(obj.BlittableStruct.Val6, uclass, msg);
            Tests.AssertDefault(obj.BlittableStruct.Val7, uclass, msg);
            Tests.AssertDefault(obj.BlittableStruct.Val8, uclass, msg);
            Tests.AssertDefault(obj.BlittableStruct.Val9, uclass, msg);
            Tests.AssertDefault(obj.BlittableStruct.Val10, uclass, msg);
            Tests.AssertDefault(obj.BlittableStruct.Val11, uclass, msg);

            // Test Struct default state
            msg = "Struct default state";
            Tests.AssertDefault(obj.Struct.Val0, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val1, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val2, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val3, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val4, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val5, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val6, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val7, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val8, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val9, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val10, uclass, msg);
            //Tests.AssertDefault(obj.Struct.Val11, uclass, msg);// delegate will be non-null from struct marshaler
            //Tests.AssertDefault(obj.Struct.Val12, uclass, msg);// delegate will be non-null from struct marshaler
            Tests.AssertDefault(obj.Struct.Val13, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val14, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val15, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val16, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val17, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val18, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val19, uclass, msg);
            Tests.AssertDefault(obj.Struct.Val20, uclass, msg);
            //Tests.AssertDefault(obj.Struct.Val21, uclass, msg);// string will be non null from struct marshaler
            Tests.AssertDefault(obj.Struct.Val22, uclass, msg);

            Test_SimpleBlittableStruct blittableStructVal = obj.BlittableStruct;
            blittableStructVal.Val1 = 1;
            blittableStructVal.Val2 = 2;
            blittableStructVal.Val3 = 3;
            blittableStructVal.Val4 = 4;
            blittableStructVal.Val5 = 5;
            blittableStructVal.Val6 = 6;
            blittableStructVal.Val7 = 7;
            blittableStructVal.Val8 = 8;
            blittableStructVal.Val9 = 9;
            blittableStructVal.Val10 = 10;
            blittableStructVal.Val11 = new FName("Hello world");
            obj.BlittableStruct = blittableStructVal;
            Tests.AssertEqual(obj.BlittableStruct, blittableStructVal, uclass, "modify BlittableStruct");

            Test_SimpleStruct structVal = obj.Struct;
            structVal.Val0 = true;
            structVal.Val1 = 1;
            structVal.Val2 = 2;
            structVal.Val3 = 3;
            structVal.Val4 = 4;
            structVal.Val5 = 5;
            structVal.Val6 = 6;
            structVal.Val7 = 7;
            structVal.Val8 = 8;
            structVal.Val9 = 9;
            structVal.Val10 = 10;
            structVal.Val11.Bind(obj.BindToStructDelegate);
            structVal.Val12.Bind(obj.BindToStructMulticastDelegate1);
            structVal.Val12.Bind(obj.BindToStructMulticastDelegate2);
            structVal.Val12.Bind(obj.BindToStructMulticastDelegate3);
            structVal.Val13 = other;
            structVal.Val14 = Test_SimpleEnum.Val3;
            structVal.Val15 = new Test_SimpleBlittableStruct()
            {
                Val1 = 1,
                Val2 = 2,
                Val3 = 3,
                Val4 = 4,
                Val5 = 5,
                Val6 = 6,
                Val7 = 7,
                Val8 = 8,
                Val9 = 9,
                Val10 = 10,
                Val11 = new FName("Hello world")
            };
            structVal.Val16.SetClass<Test_SimpleStructOnClass>();
            structVal.Val17.Set(other);
            structVal.Val18.Set(obj);
            structVal.Val19.SetClass<Test_SimpleStructOnClass>();
            structVal.Val20.Value = UClass.GetClass<Test_SimpleStructOnClass>();
            structVal.Val21 = "Hello!";
            structVal.Val22 = new FName("Hello FName");

            Tests.Assert(structVal.Val11.IsBound, uclass, ".Val11.IsBound");
            Tests.Assert(structVal.Val12.IsBound, uclass, ".Val12.IsBound");
            obj.Struct = structVal;
            //Tests.AssertEqual(obj.Struct, structVal, uclass, "modify Struct");
            // We can't test equals of struct value due to new instances of delegates created on each struct marshal
            // TODO: Replace delegates with structs so they dont require allocation of new managed classes on marshaling

            Test_SimpleStruct newStructVal = obj.Struct;
            Tests.AssertEqual(newStructVal.Val0, true, uclass, ".Val0");
            Tests.AssertEqual(newStructVal.Val1, 1, uclass, ".Val1");
            Tests.AssertEqual(newStructVal.Val2, 2, uclass, ".Val2");
            Tests.AssertEqual(newStructVal.Val3, 3, uclass, ".Val3");
            Tests.AssertEqual(newStructVal.Val4, 4, uclass, ".Val4");
            Tests.AssertEqual(newStructVal.Val5, 5, uclass, ".Val5");
            Tests.AssertEqual(newStructVal.Val6, 6u, uclass, ".Val6");
            Tests.AssertEqual(newStructVal.Val7, 7, uclass, ".Val7");
            Tests.AssertEqual(newStructVal.Val8, 8u, uclass, ".Val8");
            Tests.AssertEqual(newStructVal.Val9, 9.0f, uclass, ".Val9");
            Tests.AssertEqual(newStructVal.Val10, 10.0, uclass, ".Val10");
            //Tests.AssertEqual(newStructVal.Val11, , uclass, ".Val11");
            //Tests.AssertEqual(newStructVal.Val12, , uclass, ".Val12");
            Tests.AssertEqual(newStructVal.Val13, other, uclass, ".Val13");
            Tests.AssertEqual(newStructVal.Val14, Test_SimpleEnum.Val3, uclass, ".Val14");
            Tests.AssertEqual(newStructVal.Val15, structVal.Val15, uclass, ".Val15");
            Tests.AssertEqual(obj.Struct.Val16.Value, UClass.GetClass<Test_SimpleStructOnClass>(), uclass, ".Val16");
            Tests.AssertEqual(obj.Struct.Val17.Get(), other, uclass, ".Val17");
            Tests.AssertEqual(obj.Struct.Val18.Get(), obj, uclass, ".Val18");
            Tests.AssertEqual(obj.Struct.Val19.Value, UClass.GetClass<Test_SimpleStructOnClass>(), uclass, ".Val19");
            Tests.AssertEqual(obj.Struct.Val20.Value, UClass.GetClass<Test_SimpleStructOnClass>(), uclass, ".Val20");

            // Test the delegates
            double param3 = 44.3;
            string param4;
            TSoftClass<UObject> someClass = structVal.Val11.Invoke(13, "inParam2", ref param3, out param4);
            Tests.AssertEqual(param3, 99.88, uclass, ".BindToStructDelegate.param3");
            Tests.AssertEqual(param4, "outParam4", uclass, ".BindToStructDelegate.param4");
            Tests.AssertEqual(someClass.Value, UClass.GetClass<Test_SimpleStructOnClass>(), uclass, ".BindToStructDelegate." + UFunction.ReturnValuePropName);

            // Test the multicast delegate
            int param3Multicast = 1;
            structVal.Val12.Invoke(3u, "param2", ref param3Multicast);
            Tests.AssertEqual(param3Multicast, 4, ".BindToStructMulticastDelegate.param3");
        }
    }
}
#endif
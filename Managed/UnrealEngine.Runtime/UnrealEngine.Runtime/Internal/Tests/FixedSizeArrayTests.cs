#if WITH_USHARP_TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Tests
{
    static class FixedSizeArrayTests
    {
        public static void Run()
        {
            Test_FixedArrayInStruct.Run();
            Test_FixedArrayInClass.Run();
        }
    }

    [UStruct]
    struct Test_FixedArrayInStruct
    {
        [UProperty(FixedSizeArrayDim = 3)]
        public sbyte[] Array1;
        [UProperty(FixedSizeArrayDim = 4)]
        public byte[] Array2;
        [UProperty(FixedSizeArrayDim = 5)]
        public short[] Array3;
        [UProperty(FixedSizeArrayDim = 6)]
        public ushort[] Array4;
        [UProperty(FixedSizeArrayDim = 7)]
        public int[] Array5;
        [UProperty(FixedSizeArrayDim = 8)]
        public uint[] Array6;
        [UProperty(FixedSizeArrayDim = 9)]
        public long[] Array7;
        [UProperty(FixedSizeArrayDim = 10)]
        public ulong[] Array8;
        [UProperty(FixedSizeArrayDim = 11)]
        public float[] Array9;
        [UProperty(FixedSizeArrayDim = 12)]
        public double[] Array10;
        [UProperty(FixedSizeArrayDim = 13)]
        public Test_SimpleDelegate[] Array11;
        [UProperty(FixedSizeArrayDim = 14)]
        public Test_SimpleMulticastDelegate[] Array12;
        [UProperty(FixedSizeArrayDim = 15)]
        public UObject[] Array13;
        [UProperty(FixedSizeArrayDim = 16)]
        public Test_SimpleEnum[] Array14;
        [UProperty(FixedSizeArrayDim = 17)]
        public Test_SimpleStruct[] Array15;
        [UProperty(FixedSizeArrayDim = 18)]
        public TSubclassOf<UObject>[] Array16;
        [UProperty(FixedSizeArrayDim = 19)]
        public TLazyObject<UObject>[] Array17;
        [UProperty(FixedSizeArrayDim = 20)]
        public TWeakObject<UObject>[] Array18;
        [UProperty(FixedSizeArrayDim = 21)]
        public TSoftClass<UObject>[] Array19;
        [UProperty(FixedSizeArrayDim = 22)]
        public TSoftObject<UObject>[] Array20;
        [UProperty(FixedSizeArrayDim = 23)]
        public string[] Array21;
        [UProperty(FixedSizeArrayDim = 24)]
        public FName[] Array22;
        //[UProperty(FixedSizeArrayDim = 25)]
        //public FText[] Array23;

        [UFunctionIgnore]
        public static void Run()
        {
            UScriptStruct unrealStruct = UScriptStruct.GetStruct<Test_FixedArrayInStruct>();
            Tests.Assert(unrealStruct != null, "Test_FixedArrayInStruct");

            Tests.AssertFixedArrayProperty<UInt8Property>(unrealStruct, "Array1", 3);
            Tests.AssertFixedArrayProperty<UByteProperty>(unrealStruct, "Array2", 4);
            Tests.AssertFixedArrayProperty<UInt16Property>(unrealStruct, "Array3", 5);
            Tests.AssertFixedArrayProperty<UUInt16Property>(unrealStruct, "Array4", 6);
            Tests.AssertFixedArrayProperty<UIntProperty>(unrealStruct, "Array5", 7);
            Tests.AssertFixedArrayProperty<UUInt32Property>(unrealStruct, "Array6", 8);
            Tests.AssertFixedArrayProperty<UInt64Property>(unrealStruct, "Array7", 9);
            Tests.AssertFixedArrayProperty<UUInt64Property>(unrealStruct, "Array8", 10);
            Tests.AssertFixedArrayProperty<UFloatProperty>(unrealStruct, "Array9", 11);
            Tests.AssertFixedArrayProperty<UDoubleProperty>(unrealStruct, "Array10", 12);
            Tests.AssertFixedArrayProperty<UDelegateProperty>(unrealStruct, "Array11", 13);
            Tests.AssertFixedArrayProperty<UMulticastDelegateProperty>(unrealStruct, "Array12", 14);
            Tests.AssertFixedArrayProperty<UObjectProperty>(unrealStruct, "Array13", 15);
            Tests.AssertFixedArrayProperty<UEnumProperty>(unrealStruct, "Array14", 16);
            Tests.AssertFixedArrayProperty<UStructProperty>(unrealStruct, "Array15", 17);
            Tests.AssertFixedArrayProperty<UClassProperty>(unrealStruct, "Array16", 18);
            Tests.AssertFixedArrayProperty<ULazyObjectProperty>(unrealStruct, "Array17", 19);
            Tests.AssertFixedArrayProperty<UWeakObjectProperty>(unrealStruct, "Array18", 20);
            Tests.AssertFixedArrayProperty<USoftClassProperty>(unrealStruct, "Array19", 21);
            Tests.AssertFixedArrayProperty<USoftObjectProperty>(unrealStruct, "Array20", 22);
            Tests.AssertFixedArrayProperty<UStrProperty>(unrealStruct, "Array21", 23);
            Tests.AssertFixedArrayProperty<UNameProperty>(unrealStruct, "Array22", 24);
            //Tests.AssertFixedArrayProperty<UTextProperty>(unrealStruct, "Array23", 25);

            Test_FixedArrayInStruct defaultValue = StructDefault<Test_FixedArrayInStruct>.Value;

            Tests.AssertEqual(defaultValue.Array1.Length, 3, unrealStruct, ".Array1");
            Tests.AssertEqual(defaultValue.Array2.Length, 4, unrealStruct, ".Array2");
            Tests.AssertEqual(defaultValue.Array3.Length, 5, unrealStruct, ".Array3");
            Tests.AssertEqual(defaultValue.Array4.Length, 6, unrealStruct, ".Array4");
            Tests.AssertEqual(defaultValue.Array5.Length, 7, unrealStruct, ".Array5");
            Tests.AssertEqual(defaultValue.Array6.Length, 8, unrealStruct, ".Array6");
            Tests.AssertEqual(defaultValue.Array7.Length, 9, unrealStruct, ".Array7");
            Tests.AssertEqual(defaultValue.Array8.Length, 10, unrealStruct, ".Array8");
            Tests.AssertEqual(defaultValue.Array9.Length, 11, unrealStruct, ".Array9");
            Tests.AssertEqual(defaultValue.Array10.Length, 12, unrealStruct, ".Array10");
            Tests.AssertEqual(defaultValue.Array11.Length, 13, unrealStruct, ".Array11");
            Tests.AssertEqual(defaultValue.Array12.Length, 14, unrealStruct, ".Array12");
            Tests.AssertEqual(defaultValue.Array13.Length, 15, unrealStruct, ".Array13");
            Tests.AssertEqual(defaultValue.Array14.Length, 16, unrealStruct, ".Array14");
            Tests.AssertEqual(defaultValue.Array15.Length, 17, unrealStruct, ".Array15");
            Tests.AssertEqual(defaultValue.Array16.Length, 18, unrealStruct, ".Array16");
            Tests.AssertEqual(defaultValue.Array17.Length, 19, unrealStruct, ".Array17");
            Tests.AssertEqual(defaultValue.Array18.Length, 20, unrealStruct, ".Array18");
            Tests.AssertEqual(defaultValue.Array19.Length, 21, unrealStruct, ".Array19");
            Tests.AssertEqual(defaultValue.Array20.Length, 22, unrealStruct, ".Array20");
            Tests.AssertEqual(defaultValue.Array21.Length, 23, unrealStruct, ".Array21");
            Tests.AssertEqual(defaultValue.Array22.Length, 24, unrealStruct, ".Array22");
        }
    }

    class Test_FixedArrayInClass : UObject
    {
        [UProperty(FixedSizeArrayDim = 3)]
        public TFixedSizeArray<sbyte> Array1 { get; set; }
        [UProperty(FixedSizeArrayDim = 4)]
        public TFixedSizeArray<byte> Array2 { get; set; }
        [UProperty(FixedSizeArrayDim = 5)]
        public TFixedSizeArray<short> Array3 { get; set; }
        [UProperty(FixedSizeArrayDim = 6)]
        public TFixedSizeArray<ushort> Array4 { get; set; }
        [UProperty(FixedSizeArrayDim = 7)]
        public TFixedSizeArray<int> Array5 { get; set; }
        [UProperty(FixedSizeArrayDim = 8)]
        public TFixedSizeArray<uint> Array6 { get; set; }
        [UProperty(FixedSizeArrayDim = 9)]
        public TFixedSizeArray<long> Array7 { get; set; }
        [UProperty(FixedSizeArrayDim = 10)]
        public TFixedSizeArray<ulong> Array8 { get; set; }
        [UProperty(FixedSizeArrayDim = 11)]
        public TFixedSizeArray<float> Array9 { get; set; }
        [UProperty(FixedSizeArrayDim = 12)]
        public TFixedSizeArray<double> Array10 { get; set; }
        [UProperty(FixedSizeArrayDim = 13)]
        public TFixedSizeArray<Test_SimpleDelegateForArrayTests> Array11 { get; set; }
        [UProperty(FixedSizeArrayDim = 14)]
        public TFixedSizeArray<Test_SimpleMulticastForArrayTests> Array12 { get; set; }
        [UProperty(FixedSizeArrayDim = 15)]
        public TFixedSizeArray<UObject> Array13 { get; set; }
        [UProperty(FixedSizeArrayDim = 16)]
        public TFixedSizeArray<Test_SimpleEnum> Array14 { get; set; }
        [UProperty(FixedSizeArrayDim = 17)]
        public TFixedSizeArray<Test_FixedArrayInStruct> Array15 { get; set; }
        [UProperty(FixedSizeArrayDim = 18)]
        public TFixedSizeArray<TSubclassOf<UObject>> Array16 { get; set; }
        [UProperty(FixedSizeArrayDim = 19)]
        public TFixedSizeArray<TLazyObject<UObject>> Array17 { get; set; }
        [UProperty(FixedSizeArrayDim = 20)]
        public TFixedSizeArray<TWeakObject<UObject>> Array18 { get; set; }
        [UProperty(FixedSizeArrayDim = 21)]
        public TFixedSizeArray<TSoftClass<UObject>> Array19 { get; set; }
        [UProperty(FixedSizeArrayDim = 22)]
        public TFixedSizeArray<TSoftObject<UObject>> Array20 { get; set; }
        [UProperty(FixedSizeArrayDim = 23)]
        public TFixedSizeArray<string> Array21 { get; set; }
        [UProperty(FixedSizeArrayDim = 24)]
        public TFixedSizeArray<FName> Array22 { get; set; }
        //[UProperty(FixedSizeArrayDim = 25)]
        //public TFixedSizeArray<FText> Array23 { get; set; }

        public int GetMe { get; set; }

        [UFunctionIgnore]
        public static void Run()
        {
            // Another object for testing
            Test_SimpleClass1 otherObj = UObject.NewObject<Test_SimpleClass1>();

            UClass unrealClass = UClass.GetClass<Test_FixedArrayInClass>();
            Tests.Assert(unrealClass != null, "Test_FixedArrayInClass");

            Tests.AssertFixedArrayProperty<UInt8Property>(unrealClass, "Array1", 3);
            Tests.AssertFixedArrayProperty<UByteProperty>(unrealClass, "Array2", 4);
            Tests.AssertFixedArrayProperty<UInt16Property>(unrealClass, "Array3", 5);
            Tests.AssertFixedArrayProperty<UUInt16Property>(unrealClass, "Array4", 6);
            Tests.AssertFixedArrayProperty<UIntProperty>(unrealClass, "Array5", 7);
            Tests.AssertFixedArrayProperty<UUInt32Property>(unrealClass, "Array6", 8);
            Tests.AssertFixedArrayProperty<UInt64Property>(unrealClass, "Array7", 9);
            Tests.AssertFixedArrayProperty<UUInt64Property>(unrealClass, "Array8", 10);
            Tests.AssertFixedArrayProperty<UFloatProperty>(unrealClass, "Array9", 11);
            Tests.AssertFixedArrayProperty<UDoubleProperty>(unrealClass, "Array10", 12);
            Tests.AssertFixedArrayProperty<UDelegateProperty>(unrealClass, "Array11", 13);
            Tests.AssertFixedArrayProperty<UMulticastDelegateProperty>(unrealClass, "Array12", 14);
            Tests.AssertFixedArrayProperty<UObjectProperty>(unrealClass, "Array13", 15);
            Tests.AssertFixedArrayProperty<UEnumProperty>(unrealClass, "Array14", 16);
            Tests.AssertFixedArrayProperty<UStructProperty>(unrealClass, "Array15", 17);
            Tests.AssertFixedArrayProperty<UClassProperty>(unrealClass, "Array16", 18);
            Tests.AssertFixedArrayProperty<ULazyObjectProperty>(unrealClass, "Array17", 19);
            Tests.AssertFixedArrayProperty<UWeakObjectProperty>(unrealClass, "Array18", 20);
            Tests.AssertFixedArrayProperty<USoftClassProperty>(unrealClass, "Array19", 21);
            Tests.AssertFixedArrayProperty<USoftObjectProperty>(unrealClass, "Array20", 22);
            Tests.AssertFixedArrayProperty<UStrProperty>(unrealClass, "Array21", 23);
            Tests.AssertFixedArrayProperty<UNameProperty>(unrealClass, "Array22", 24);
            //Tests.AssertFixedArrayProperty<UTextProperty>(unrealClass, "Array23", 25);            

            Test_FixedArrayInClass obj = UObject.NewObject<Test_FixedArrayInClass>();
            Tests.AssertEqual(obj.Array1.Length, 3, unrealClass, ".Array1");
            Tests.AssertEqual(obj.Array2.Length, 4, unrealClass, ".Array2");
            Tests.AssertEqual(obj.Array3.Length, 5, unrealClass, ".Array3");
            Tests.AssertEqual(obj.Array4.Length, 6, unrealClass, ".Array4");
            Tests.AssertEqual(obj.Array5.Length, 7, unrealClass, ".Array5");
            Tests.AssertEqual(obj.Array6.Length, 8, unrealClass, ".Array6");
            Tests.AssertEqual(obj.Array7.Length, 9, unrealClass, ".Array7");
            Tests.AssertEqual(obj.Array8.Length, 10, unrealClass, ".Array8");
            Tests.AssertEqual(obj.Array9.Length, 11, unrealClass, ".Array9");
            Tests.AssertEqual(obj.Array10.Length, 12, unrealClass, ".Array10");
            Tests.AssertEqual(obj.Array11.Length, 13, unrealClass, ".Array11");
            Tests.AssertEqual(obj.Array12.Length, 14, unrealClass, ".Array12");
            Tests.AssertEqual(obj.Array13.Length, 15, unrealClass, ".Array13");
            Tests.AssertEqual(obj.Array14.Length, 16, unrealClass, ".Array14");
            Tests.AssertEqual(obj.Array15.Length, 17, unrealClass, ".Array15");
            Tests.AssertEqual(obj.Array16.Length, 18, unrealClass, ".Array16");
            Tests.AssertEqual(obj.Array17.Length, 19, unrealClass, ".Array17");
            Tests.AssertEqual(obj.Array18.Length, 20, unrealClass, ".Array18");
            Tests.AssertEqual(obj.Array19.Length, 21, unrealClass, ".Array19");
            Tests.AssertEqual(obj.Array20.Length, 22, unrealClass, ".Array20");
            Tests.AssertEqual(obj.Array21.Length, 23, unrealClass, ".Array21");
            Tests.AssertEqual(obj.Array22.Length, 24, unrealClass, ".Array22");

            BeginMember("Array1");
            for (int i = 0; i < obj.Array1.Length; i++)
            {
                Tests.AssertEqual(obj.Array1[i], 0, unrealClass, currentMemberName);
                obj.Array1[i] += 5;
                Tests.AssertEqual(obj.Array1[i], 5, unrealClass, currentMemberName);
                obj.Array1[i] += 10;
                Tests.AssertEqual(obj.Array1[i], 15, unrealClass, currentMemberName);
            }

            BeginMember("Array2");
            for (int i = 0; i < obj.Array2.Length; i++)
            {
                Tests.AssertEqual(obj.Array2[i], 0, unrealClass, currentMemberName);
                obj.Array2[i] += 8;
                Tests.AssertEqual(obj.Array2[i], 8, unrealClass, currentMemberName);
                obj.Array2[i] += 3;
                Tests.AssertEqual(obj.Array2[i], 11, unrealClass, currentMemberName);
            }

            BeginMember("Array3");
            for (int i = 0; i < obj.Array2.Length; i++)
            {
                Tests.AssertEqual(obj.Array3[i], 0, unrealClass, currentMemberName);
                obj.Array3[i] += 4;
                Tests.AssertEqual(obj.Array3[i], 4, unrealClass, currentMemberName);
                obj.Array3[i] += 5;
                Tests.AssertEqual(obj.Array3[i], 9, unrealClass, currentMemberName);
            }

            BeginMember("Array4");
            for (int i = 0; i < obj.Array4.Length; i++)
            {
                Tests.AssertEqual(obj.Array4[i], 0, unrealClass, currentMemberName);
                obj.Array4[i] += 7;
                Tests.AssertEqual(obj.Array4[i], 7, unrealClass, currentMemberName);
                obj.Array4[i] += 3;
                Tests.AssertEqual(obj.Array4[i], 10, unrealClass, currentMemberName);
            }

            BeginMember("Array5");
            for (int i = 0; i < obj.Array5.Length; i++)
            {
                Tests.AssertEqual(obj.Array5[i], 0, unrealClass, currentMemberName);
                obj.Array5[i] += 2;
                Tests.AssertEqual(obj.Array5[i], 2, unrealClass, currentMemberName);
                obj.Array5[i] += 6;
                Tests.AssertEqual(obj.Array5[i], 8, unrealClass, currentMemberName);
            }

            BeginMember("Array6");
            for (int i = 0; i < obj.Array6.Length; i++)
            {
                Tests.AssertEqual(obj.Array6[i], 0u, unrealClass, currentMemberName);
                obj.Array6[i] += 3;
                Tests.AssertEqual(obj.Array6[i], 3u, unrealClass, currentMemberName);
                obj.Array6[i] += 7;
                Tests.AssertEqual(obj.Array6[i], 10u, unrealClass, currentMemberName);
            }

            BeginMember("Array7");
            for (int i = 0; i < obj.Array7.Length; i++)
            {
                Tests.AssertEqual(obj.Array7[i], 0, unrealClass, currentMemberName);
                obj.Array7[i] += 1;
                Tests.AssertEqual(obj.Array7[i], 1, unrealClass, currentMemberName);
                obj.Array7[i] += 1;
                Tests.AssertEqual(obj.Array7[i], 2, unrealClass, currentMemberName);
            }

            BeginMember("Array8");
            for (int i = 0; i < obj.Array8.Length; i++)
            {
                Tests.AssertEqual(obj.Array8[i], 0u, unrealClass, currentMemberName);
                obj.Array8[i] += 3;
                Tests.AssertEqual(obj.Array8[i], 3u, unrealClass, currentMemberName);
                obj.Array8[i] += 6;
                Tests.AssertEqual(obj.Array8[i], 9u, unrealClass, currentMemberName);
            }

            BeginMember("Array9");
            for (int i = 0; i < obj.Array9.Length; i++)
            {
                Tests.AssertEqual(obj.Array9[i], 0, unrealClass, currentMemberName);
                obj.Array9[i] += 1.5f;
                Tests.AssertEqual(obj.Array9[i], 1.5f, unrealClass, currentMemberName);
                obj.Array9[i] += 2.3f;
                Tests.AssertEqual(obj.Array9[i], 3.8f, unrealClass, currentMemberName);
            }

            BeginMember("Array10");
            for (int i = 0; i < obj.Array10.Length; i++)
            {
                Tests.AssertEqual(obj.Array10[i], 0, unrealClass, currentMemberName);
                obj.Array10[i] += 3.1;
                Tests.AssertEqual(obj.Array10[i], 3.1, unrealClass, currentMemberName);
                obj.Array10[i] += 1.1;
                Tests.AssertEqual(obj.Array10[i], 4.2, unrealClass, currentMemberName);
            }

            BeginMember("Array11");
            for (int i = 0; i < obj.Array11.Length; i++)
            {
                // Need to read the delegate, modify the state and write it back (can't modify directly
                // since each access to the array index will return a new delegate) - (this is because if the
                // delegate held onto the collection index address there is no guarantee it still sits at that address
                // as the collection changes) - (for fixed arrays this is valid but fixed arrays are going to be
                // rarely used anyway so it isn't really worth working in any improvement for it)
                var del = obj.Array11[i];
                Tests.Assert(!del.IsBound, unrealClass, currentMemberName);
                del.Bind(obj.SimpleFunc);
                Tests.Assert(del.IsBound, unrealClass, currentMemberName);
                obj.Array11[i] = del;

                int arg = 3;
                obj.Array11[i].Invoke(ref arg);
                Tests.AssertEqual(arg, 4, unrealClass, currentMemberName);
            }

            BeginMember("Array12");
            for (int i = 0; i < obj.Array12.Length; i++)
            {
                var del = obj.Array12[i];
                Tests.Assert(!del.IsBound, unrealClass, currentMemberName);
                del.Bind(obj.SimpleFunc1);
                del.Bind(obj.SimpleFunc2);
                Tests.Assert(del.IsBound, unrealClass, currentMemberName);
                Tests.Assert(del.Count == 2, unrealClass, currentMemberName);
                obj.Array12[i] = del;

                double arg = 0.5;
                obj.Array12[i].Invoke(ref arg);
                Tests.AssertEqual(arg, 2.6, unrealClass, currentMemberName);
            }

            BeginMember("Array13");
            for (int i = 0; i < obj.Array13.Length; i++)
            {
                Tests.AssertEqual(obj.Array13[i], null, unrealClass, currentMemberName);
            }
            obj.Array13[0] = obj;
            obj.Array13[3] = otherObj;
            Tests.AssertEqual(obj.Array13[0], obj, unrealClass, currentMemberName);
            Tests.AssertEqual(obj.Array13[3], otherObj, unrealClass, currentMemberName);
            for (int i = 0; i < obj.Array13.Length; i++)
            {
                if (i != 0 && i != 3)
                {
                    Tests.AssertEqual(obj.Array13[i], null, unrealClass, currentMemberName);
                }
            }
            obj.Array13[0] = null;
            obj.Array13[3] = null;
            for (int i = 0; i < obj.Array13.Length; i++)
            {
                Tests.AssertEqual(obj.Array13[i], null, unrealClass, currentMemberName);
            }

            BeginMember("Array14");
            for (int i = 0; i < obj.Array14.Length; i++)
            {
                Tests.AssertEqual(obj.Array14[i], default(Test_SimpleEnum), unrealClass, currentMemberName);
            }
            obj.Array14[1] = Test_SimpleEnum.Val3;
            obj.Array14[4] = Test_SimpleEnum.Val2;
            Tests.AssertEqual(obj.Array14[1], Test_SimpleEnum.Val3, unrealClass, currentMemberName);
            Tests.AssertEqual(obj.Array14[4], Test_SimpleEnum.Val2, unrealClass, currentMemberName);
            for (int i = 0; i < obj.Array13.Length; i++)
            {
                if (i != 1 && i != 4)
                {
                    Tests.AssertEqual(obj.Array14[i], default(Test_SimpleEnum), unrealClass, currentMemberName);
                }
            }

            // This is going to be slow, it has do an entire copy of the struct (which has many arrays which
            // need to be fully copied)
            BeginMember("Array15");
            // Read the struct at index 0, modify state, write it back, then validate
            var v1 = obj.Array15[0];
            v1.Array5[3] = 1;
            v1.Array5[4] = 2;
            v1.Array7[5] = 3;
            v1.Array7[6] = 4;
            obj.Array15[0] = v1;
            v1 = obj.Array15[0];
            Tests.AssertEqual(v1.Array5[1], 0, unrealClass, currentMemberName);
            Tests.AssertEqual(v1.Array5[3], 1, unrealClass, currentMemberName);
            Tests.AssertEqual(v1.Array5[4], 2, unrealClass, currentMemberName);
            Tests.AssertEqual(v1.Array7[1], 0, unrealClass, currentMemberName);
            Tests.AssertEqual(v1.Array7[5], 3, unrealClass, currentMemberName);
            Tests.AssertEqual(v1.Array7[6], 4, unrealClass, currentMemberName);

            BeginMember("Array16");
            for (int i = 0; i < obj.Array16.Length; i++)
            {
                Tests.AssertEqual(obj.Array16[i].Value, null, unrealClass, currentMemberName);
            }
            // same test but two different ways of doing the assignment
            var subclassOf = obj.Array16[1];
            subclassOf.Value = UClass.GetClass<Test_FixedArrayInClass>();
            obj.Array16[1] = subclassOf;
            Tests.AssertEqual(obj.Array16[1].Value, UClass.GetClass<Test_FixedArrayInClass>(), unrealClass, currentMemberName);
            obj.Array16[2] = new TSubclassOf<UObject>(UClass.GetClass<Test_SimpleClass1>());
            Tests.AssertEqual(obj.Array16[2].Value, UClass.GetClass<Test_SimpleClass1>(), unrealClass, currentMemberName);
            obj.Array16[1] = TSubclassOf<UObject>.Null;
            obj.Array16[2] = TSubclassOf<UObject>.Null;
            Tests.AssertEqual(obj.Array16[1].Value, null, unrealClass, currentMemberName);
            Tests.AssertEqual(obj.Array16[2].Value, null, unrealClass, currentMemberName);

            BeginMember("Array17");
            for (int i = 0; i < obj.Array17.Length; i++)
            {
                Tests.AssertEqual(obj.Array17[i].Value, null, unrealClass, currentMemberName);
            }
            // same test but two different ways of doing the assignment
            var lazyObject = obj.Array17[3];
            lazyObject.Value = obj;
            obj.Array17[3] = lazyObject;
            Tests.AssertEqual(obj.Array17[3].Value, obj, unrealClass, currentMemberName);
            obj.Array17[4] = new TLazyObject<UObject>(otherObj);
            Tests.AssertEqual(obj.Array17[4].Value, otherObj, unrealClass, currentMemberName);
            obj.Array17[3] = TLazyObject<UObject>.Null;
            obj.Array17[4] = TLazyObject<UObject>.Null;
            Tests.AssertEqual(obj.Array17[3].Value, null, unrealClass, currentMemberName);
            Tests.AssertEqual(obj.Array17[4].Value, null, unrealClass, currentMemberName);

            BeginMember("Array18");
            for (int i = 0; i < obj.Array18.Length; i++)
            {
                Tests.AssertEqual(obj.Array18[i].Value, null, unrealClass, currentMemberName);
            }
            var weakObject = obj.Array18[5];
            weakObject.Value = otherObj;
            obj.Array18[5] = weakObject;
            Tests.AssertEqual(obj.Array18[5].Value, otherObj, unrealClass, currentMemberName);
            obj.Array18[6] = new TWeakObject<UObject>(obj);
            Tests.AssertEqual(obj.Array18[6].Value, obj, unrealClass, currentMemberName);
            obj.Array18[5] = TWeakObject<UObject>.Null;
            obj.Array18[6] = TWeakObject<UObject>.Null;
            Tests.AssertEqual(obj.Array18[5].Value, null, unrealClass, currentMemberName);
            Tests.AssertEqual(obj.Array18[6].Value, null, unrealClass, currentMemberName);

            BeginMember("Array19");
            for (int i = 0; i < obj.Array19.Length; i++)
            {
                Tests.AssertEqual(obj.Array19[i].Value, null, unrealClass, currentMemberName);
            }
            var softclassOf = obj.Array19[7];
            softclassOf.Value = UClass.GetClass<Test_FixedArrayInClass>();
            obj.Array19[7] = softclassOf;
            Tests.AssertEqual(obj.Array19[7].Value, UClass.GetClass<Test_FixedArrayInClass>(), unrealClass, currentMemberName);
            obj.Array19[8] = new TSoftClass<UObject>(UClass.GetClass<Test_SimpleClass1>());
            Tests.AssertEqual(obj.Array19[8].Value, UClass.GetClass<Test_SimpleClass1>(), unrealClass, currentMemberName);
            obj.Array19[7] = TSoftClass<UObject>.Null;
            obj.Array19[8] = TSoftClass<UObject>.Null;
            Tests.AssertEqual(obj.Array19[7].Value, null, unrealClass, currentMemberName);
            Tests.AssertEqual(obj.Array19[8].Value, null, unrealClass, currentMemberName);

            BeginMember("Array20");
            for (int i = 0; i < obj.Array20.Length; i++)
            {
                Tests.AssertEqual(obj.Array20[i].Value, null, unrealClass, currentMemberName);
            }
            var softObject = obj.Array20[9];
            softObject.Value = UClass.GetClass<Test_FixedArrayInClass>();
            obj.Array20[9] = softObject;
            Tests.AssertEqual(obj.Array20[9].Value, UClass.GetClass<Test_FixedArrayInClass>(), unrealClass, currentMemberName);
            obj.Array20[10] = new TSoftObject<UObject>(UClass.GetClass<Test_SimpleClass1>());
            Tests.AssertEqual(obj.Array20[10].Value, UClass.GetClass<Test_SimpleClass1>(), unrealClass, currentMemberName);
            obj.Array20[9] = TSoftObject<UObject>.Null;
            obj.Array20[10] = TSoftObject<UObject>.Null;
            Tests.AssertEqual(obj.Array20[9].Value, null, unrealClass, currentMemberName);
            Tests.AssertEqual(obj.Array20[10].Value, null, unrealClass, currentMemberName);

            BeginMember("Array21");
            for (int i = 0; i < obj.Array21.Length; i++)
            {
                Tests.AssertEqual(obj.Array21[i], string.Empty, unrealClass, currentMemberName);
            }
            obj.Array21[2] = "Str1";
            Tests.AssertEqual(obj.Array21[2], "Str1", unrealClass, currentMemberName);
            obj.Array21[2] = null;// null will be "" when read back
            Tests.AssertEqual(obj.Array21[2], string.Empty, unrealClass, currentMemberName);
            obj.Array21[3] = "Str2";
            Tests.AssertEqual(obj.Array21[3], "Str2", unrealClass, currentMemberName);
            obj.Array21[3] = "";
            Tests.AssertEqual(obj.Array21[3], "", unrealClass, currentMemberName);

            BeginMember("Array22");
            for (int i = 0; i < obj.Array22.Length; i++)
            {
                Tests.AssertEqual(obj.Array22[i], FName.None, unrealClass, currentMemberName);
            }
            obj.Array22[4] = new FName("Name1");
            Tests.AssertEqual(obj.Array22[4], new FName("Name1"), unrealClass, currentMemberName);
            obj.Array22[4] = default(FName);// Same as FName.None
            Tests.AssertEqual(obj.Array22[4], FName.None, unrealClass, currentMemberName);
            obj.Array22[5] = new FName("Name2");
            Tests.AssertEqual(obj.Array22[5], new FName("Name2"), unrealClass, currentMemberName);
            obj.Array22[5] = new FName("Name2Changed");
            Tests.AssertEqual(obj.Array22[5], new FName("Name2Changed"), unrealClass, currentMemberName);

            RunParamTests(obj);
        }

        [UFunctionIgnore]
        private static void RunParamTests(Test_FixedArrayInClass obj)
        {
            UClass unrealClass = obj.GetClass();

            // Using DynamicInvoke to simulate what would happen if called from C++. If we called these functions directly
            // it wouldn't give meaningful results as it would just exhibit pure C# behaviour of the struct

            Test_FixedArrayInStruct resultStruct = (Test_FixedArrayInStruct)UObject.DynamicInvoke(obj, "StructArrayFuncTestResult");
            AssertTestArrayFuncParam(resultStruct, unrealClass, 0);

            // Pass the struct over to a simple param function which will do the same check
            UObject.DynamicInvoke(obj, "StructArrayFuncTestParam", resultStruct);
            AssertTestArrayFuncParam(resultStruct, unrealClass, 2);

            object[] parameters = { resultStruct };
            UObject.DynamicInvoke(obj, "StructArrayFuncTestRefParam", parameters);
            resultStruct = (Test_FixedArrayInStruct)parameters[0];
            AssertTestArrayFuncParam(resultStruct, unrealClass, 4);

            // the 'out' call will set it back to the state similar to the first call
            parameters[0] = null;// null should be fine here? it will be overwritten since its an out param
            UObject.DynamicInvoke(obj, "StructArrayFuncTestOutParam", parameters);
            resultStruct = (Test_FixedArrayInStruct)parameters[0];
            AssertTestArrayFuncParam(resultStruct, unrealClass, 5);
        }

        private Test_FixedArrayInStruct StructArrayFuncTestResult()
        {
            // Setting these to larger than they can actually be, the marshaler will trim these!
            return new Test_FixedArrayInStruct()
            {
                Array1 = new sbyte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },//3 actual items
                Array6 = new uint[10] // 8 actual items
                {
                    uint.MaxValue - 1,
                    uint.MaxValue - 2,
                    uint.MaxValue - 3,
                    uint.MaxValue - 4,
                    uint.MaxValue - 5,
                    uint.MaxValue - 6,
                    uint.MaxValue - 7,
                    uint.MaxValue - 8,
                    uint.MaxValue - 9,
                    uint.MaxValue - 10,
                }
            };
        }

        private void StructArrayFuncTestParam(Test_FixedArrayInStruct param)
        {
            AssertTestArrayFuncParam(param, GetClass(), 1);
        }
        
        private void StructArrayFuncTestRefParam(ref Test_FixedArrayInStruct param)
        {
            AssertTestArrayFuncParam(param, GetClass(), 3);

            // Set some values
            param.Array2 = new byte[10] { 2, 3, 4, 5, 6, 7, 8, 10, 11, 12 };//4 actual items
        }

        private void StructArrayFuncTestOutParam(out Test_FixedArrayInStruct param)
        {
            param = new Test_FixedArrayInStruct()
            {
                Array1 = new sbyte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },//3 actual items
                Array6 = new uint[10] // 8 actual items
                {
                    uint.MaxValue - 1,
                    uint.MaxValue - 2,
                    uint.MaxValue - 3,
                    uint.MaxValue - 4,
                    uint.MaxValue - 5,
                    uint.MaxValue - 6,
                    uint.MaxValue - 7,
                    uint.MaxValue - 8,
                    uint.MaxValue - 9,
                    uint.MaxValue - 10,
                }
            };
        }

        [UFunctionIgnore]
        private static void AssertTestArrayFuncParam(Test_FixedArrayInStruct value, UClass unrealClass, int step)
        {
            for (int i = 0; i < value.Array1.Length; i++)
            {
                Tests.AssertEqual(i + 1, value.Array1[i], unrealClass, "StructArrayFuncTestResult-" + step);
            }
            for (int i = 0; i < value.Array6.Length; i++)
            {
                Tests.AssertEqual(uint.MaxValue - (i + 1), value.Array6[i], unrealClass, "StructArrayFuncTestResult-" + step);
            }
            if (step == 4)
            {
                // check the ref func changed this, otherwise it should be default
                for (int i = 0; i < value.Array1.Length; i++)
                {
                    Tests.AssertEqual(i + 2, value.Array2[i], unrealClass, "StructArrayFuncTestResult-" + step);
                }
            }
            else
            {
                for (int i = 0; i < value.Array1.Length; i++)
                {
                    Tests.AssertEqual(0, value.Array2[i], unrealClass, "StructArrayFuncTestResult-" + step);
                }
            }
        }

        private void SimpleFunc(ref int val)
        {
            val++;
        }

        private void SimpleFunc1(ref double val)
        {
            val += 0.1;
        }

        private void SimpleFunc2(ref double val)
        {
            val += 2;
        }

        private static string currentMemberName;
        private static void BeginMember(string memberName)
        {
            currentMemberName = "." + memberName;
        }
    }
}
#endif
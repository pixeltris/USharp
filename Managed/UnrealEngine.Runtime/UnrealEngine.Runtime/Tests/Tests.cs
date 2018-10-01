#if WITH_USHARP_TESTS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime.Tests
{
    internal static class Tests
    {
        public static UClass ActorClass;

        private static void RunMiscTests()
        {
            // A couple of misc tests

            // Test FName finding an existing FName which doesn't exist returns FName.None
            Tests.AssertEqual(new FName("I don't exist! (C# test 123!)", FName.EFindName.Find), FName.None, "find non existing FName");
        }

        internal static void OnNativeFunctionsRegistered()
        {
            if (Debugger.IsAttached && ManagedUnrealModuleInfo.AssemblyHasSerializedModuleInfo(
                System.Reflection.Assembly.GetExecutingAssembly()))
            {
                ActorClass = UClass.GetClass("/Script/Engine.Actor");
            
                // Run the tests a few times. If there is corrupt memory it might not show itself on the first run.
                for (int i = 0; i < 5; i++)
                {
                    DelegateTests.Run();
                    EnumTests.Run();
                    FixedSizeArrayTests.Run();
                    StructTests.Run();
                    ClassTests.Run();
                    CollectionsTests.Run();
                    SimpleFunctionParamTests.Run();
                    CollectionFunctionParamTests.Run();
                }
            }
        }

        public static void AssertProperty<T>(UStruct unrealStruct, string name, EPropertyFlags flags) where T : UProperty
        {
            UProperty prop = unrealStruct.FindPropertyByName(new FName(name));
            Assert(prop as T != null, unrealStruct.GetName() + "." + name);
            Assert(prop.HasAllPropertyFlags(flags), unrealStruct.GetName() + "." + name + " (flags)");
        }

        public static void AssertProperty<T>(UStruct unrealStruct, string name) where T : UProperty
        {
            Assert(unrealStruct.FindPropertyByName(new FName(name)) as T != null, unrealStruct.GetName() + "." + name);
        }

        public static void AssertTArrayProperty<T>(UStruct unrealStruct, string name) where T : UProperty
        {
            UProperty prop = unrealStruct.FindPropertyByName(new FName(name));
            Assert(prop != null, unrealStruct.GetName() + "." + name);

            UArrayProperty arrayProp = prop as UArrayProperty;
            Assert(arrayProp != null, unrealStruct.GetName() + "." + name);
            
            Assert(arrayProp.Inner as T != null, unrealStruct.GetName() + "." + name + " TArray Inner property");
        }

        public static void AssertTSetProperty<T>(UStruct unrealStruct, string name) where T : UProperty
        {
            UProperty prop = unrealStruct.FindPropertyByName(new FName(name));
            Assert(prop != null, unrealStruct.GetName() + "." + name);

            USetProperty setProp = prop as USetProperty;
            Assert(setProp != null, unrealStruct.GetName() + "." + name);

            Assert(setProp.ElementProp as T != null, unrealStruct.GetName() + "." + name + " TSet ElementProp");
        }

        public static void AssertTMapProperty<TKey, TValue>(UStruct unrealStruct, string name) 
            where TKey : UProperty
            where TValue : UProperty
        {
            UProperty prop = unrealStruct.FindPropertyByName(new FName(name));
            Assert(prop != null, unrealStruct.GetName() + "." + name);

            UMapProperty mapProp = prop as UMapProperty;
            Assert(mapProp != null, unrealStruct.GetName() + "." + name);

            Assert(mapProp.KeyProp as TKey != null, unrealStruct.GetName() + "." + name + " TMap KeyProp");
            Assert(mapProp.ValueProp as TValue != null, unrealStruct.GetName() + "." + name + " TMap ValueProp");
        }

        public static void AssertFixedArrayProperty<T>(UStruct unrealStruct, string name, int length) where T : UProperty
        {
            T prop = unrealStruct.FindPropertyByName(new FName(name)) as T;
            Assert(prop != null, unrealStruct.GetName() + "." + name);
            AssertEqual(prop.ArrayDim, length, unrealStruct.GetName() + "." + name + ".FixedSizeArrayDim");
        }

        public static void Assert(bool condition, UStruct unrealStruct, string message)
        {
            Debug.Assert(condition, unrealStruct.GetName() + GetFullMessage(message));
        }

        public static void AssertDefault<T>(T value, UStruct unrealStruct, string message)
        {
            Debug.Assert(Object.Equals(value, default(T)), unrealStruct + GetFullMessage(message));
        }

        public static void AssertEqual<T>(T value, T expected, UStruct unrealStruct, string message)
        {
            Debug.Assert(Object.Equals(value, expected), unrealStruct + GetFullMessage(message));
        }

        public static void AssertNotNull<T>(T value, UStruct unrealStruct, string message) where T : class
        {
            Debug.Assert(value != null, unrealStruct + GetFullMessage(message));
        }

        private static string GetFullMessage(string message)
        {
            return string.IsNullOrEmpty(message) ? string.Empty :
                message.StartsWith(".") ? message : " - " + message;
        }

        public static void AssertEnumValue(UEnum unrealEnum, string name, long value)
        {
            Tests.AssertEqual(unrealEnum.GetValueByNameString(name), value, unrealEnum.GetName() + "." + name);
        }

        private static void PrintEnumValues(UEnum unrealEnum)
        {
            Dictionary<string, long> values = new Dictionary<string, long>();
            int numValues= unrealEnum.NumEnums() - 1;// skip Max value
            for (int i = 0; i < numValues; i++)
            {
                values[unrealEnum.GetNameStringByIndex(i)] = unrealEnum.GetValueByIndex(i);
            }

            foreach (KeyValuePair<string, long> value in values)
            {
                Debug.WriteLine(value.Key + ":" + value.Value);
            }
        }

        public static void Assert(bool condition, string message)
        {
            Debug.Assert(condition, message);
        }

        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
        }

        public static void AssertEqual<T>(T value, T expected)
        {
            Debug.Assert(Object.Equals(value, expected));
        }

        public static void AssertEqual<T>(T value, T expected, string message)
        {
            Debug.Assert(Object.Equals(value, expected), message);
        }
    }
}
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnrealEngine.Runtime
{
    static class Extensions
    {
        public static bool IsSameOrSubclassOfGeneric(this Type type, Type c)
        {
            while (type != null && type != typeof(object))
            {
                Type cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (cur == c)
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }

        public static bool IsSameOrSubclassOf(this Type type, Type c)
        {
            return type == c || type.IsSubclassOf(c);
        }

        /// <summary>
        /// Returns the first method with the given name without throwing an exception if there is a duplicate
        /// </summary>
        public static MethodInfo GetFirstMethod(this Type type, string methodName)
        {
            return type.GetFirstMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
        }

        /// <summary>
        /// Returns the first method with the given name without throwing an exception if there is a duplicate
        /// </summary>
        public static MethodInfo GetFirstMethod(this Type type, string methodName, BindingFlags bindingFlags)
        {
            foreach (MethodInfo method in type.GetMethods(bindingFlags))
            {
                if (method.Name == methodName)
                {
                    return method;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the method with the given name. If there are two or more methods with the same name this will return null.
        /// </summary>
        public static MethodInfo GetMethodUnique(this Type type, string methodName)
        {
            return type.GetMethodUnique(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
        }

        /// <summary>
        /// Returns the method with the given name. If there are two or more methods with the same name this will return null.
        /// </summary>
        public static MethodInfo GetMethodUnique(this Type type, string methodName, BindingFlags bindingFlags)
        {
            MethodInfo result = null;
            foreach (MethodInfo method in type.GetMethods(bindingFlags))
            {
                if (method.Name == methodName)
                {
                    if (result == null)
                    {
                        result = method;
                    }
                    else
                    {
                        // Failed to find a unique method with the given name
                        return null;
                    }
                    return method;
                }
            }
            return result;
        }

        public static string RemoveFromStart(this string str, string prefix)
        {
            return prefix != null && str.StartsWith(prefix) ? str.Substring(prefix.Length) : str;
        }

        public static string RemoveFromEnd(this string str, string suffix)
        {
            return suffix != null && str.EndsWith(suffix) ? str.Substring(0, str.Length - suffix.Length) : str;
        }

        /// <summary>
        /// Swaps the given index with the end of the collection and then removes it from the end
        /// </summary>
        public static void RemoveAtSwap<T>(this List<T> collection, int index)
        {
            collection[index] = collection[collection.Count - 1];
            collection.RemoveAt(collection.Count - 1);
        }

        /// <summary>
        /// Does a RemoveAtSwap but also updates an indexField to index and index to -1
        /// (indexField should be an index memeber inside the last element in the array)
        /// </summary>
        public static void RemoveAtSwapEx<T>(this List<T> collection, ref int index, ref int indexField)
        {
            collection.RemoveAtSwap(index);
            indexField = index;
            index = -1;
        }

        /// <summary>
        /// Swaps the given item with the end of the collection and then removes it from the end
        /// </summary>
        public static void RemoveSwap<T>(this List<T> collection, T item)
        {
            collection[collection.IndexOf(item)] = collection[collection.Count - 1];
            collection.RemoveAt(collection.Count - 1);
        }

        /// <summary>
        /// Gets names and values for the given type.
        /// </summary>
        /// <param name="type">The type of the enum</param>
        /// <returns>The enum names and values</returns>
        public static Dictionary<string, ulong> GetEnumNamesValues(this Type type)
        {
            byte enumByteSize;
            return type.GetEnumNamesValues(out enumByteSize);
        }
        
        /// <summary>
        /// Gets names and values for the given type. Also calculates the size of the enum in bytes (based on the values
        /// in the enum. To get the actual underlying type size use Type.GetTypeCode(type.GetEnumUnderlyingType()))
        /// </summary>
        /// <param name="type">The type of the enum</param>
        /// <param name="calculatedEnumByteSize">The calculated size of the enum in bytes</param>
        /// <returns>The enum names and values</returns>
        public static Dictionary<string, ulong> GetEnumNamesValues(this Type type, out byte calculatedEnumByteSize)
        {
            Type enumType = type;
            if (type.IsByRef && type.HasElementType)
            {
                enumType = type.GetElementType();
            }

            if (!enumType.IsEnum)
            {
                throw new Exception("Attempting to get the enum byte size from a non-enum type");
            }

            Dictionary<string, ulong> values = new Dictionary<string, ulong>();

            // Is there a better way to do this without the duplicate code? (and without crashing
            // due to type mismatch when converting the enum value type)
            calculatedEnumByteSize = 1;
            switch (Type.GetTypeCode(type.GetEnumUnderlyingType()))
            {
                case TypeCode.SByte:
                    foreach (object enumValue in Enum.GetValues(enumType))
                    {
                        byte value = (byte)(sbyte)enumValue;
                        values[enumValue.ToString()] = value;
                    }
                    break;
                case TypeCode.Int16:
                    foreach (object enumValue in Enum.GetValues(enumType))
                    {
                        ushort value = (ushort)(short)enumValue;
                        values[enumValue.ToString()] = value;
                        if (value > byte.MaxValue)
                        {
                            calculatedEnumByteSize = 2;
                        }
                    }
                    break;
                case TypeCode.Int32:
                    foreach (object enumValue in Enum.GetValues(enumType))
                    {
                        uint value = (uint)(int)enumValue;
                        values[enumValue.ToString()] = value;
                        if (value > ushort.MaxValue)
                        {
                            calculatedEnumByteSize = 4;
                        }
                        else if (value > byte.MaxValue)
                        {
                            calculatedEnumByteSize = 2;
                        }
                    }
                    break;
                case TypeCode.Int64:
                    foreach (object enumValue in Enum.GetValues(enumType))
                    {
                        ulong value = (ulong)(long)enumValue;
                        values[enumValue.ToString()] = value;
                        if (value > uint.MaxValue)
                        {
                            calculatedEnumByteSize = 8;
                        }
                        else if (value > ushort.MaxValue)
                        {
                            calculatedEnumByteSize = 4;
                        }
                        else if (value > byte.MaxValue)
                        {
                            calculatedEnumByteSize = 2;
                        }
                    }
                    break;
                case TypeCode.Byte:
                    foreach (object enumValue in Enum.GetValues(enumType))
                    {
                        byte value = (byte)enumValue;
                        values[enumValue.ToString()] = value;
                    }
                    break;
                case TypeCode.UInt16:
                    foreach (object enumValue in Enum.GetValues(enumType))
                    {
                        ushort value = (ushort)enumValue;
                        values[enumValue.ToString()] = value;
                        if (value > byte.MaxValue)
                        {
                            calculatedEnumByteSize = 2;
                        }
                    }
                    break;
                case TypeCode.UInt32:
                    foreach (object enumValue in Enum.GetValues(enumType))
                    {
                        uint value = (uint)enumValue;
                        values[enumValue.ToString()] = value;
                        if (value > ushort.MaxValue)
                        {
                            calculatedEnumByteSize = 4;
                        }
                        else if (value > byte.MaxValue)
                        {
                            calculatedEnumByteSize = 2;
                        }
                    }
                    break;
                case TypeCode.UInt64:
                    foreach (object enumValue in Enum.GetValues(enumType))
                    {
                        ulong value = (ulong)enumValue;
                        values[enumValue.ToString()] = value;
                        if (value > uint.MaxValue)
                        {
                            calculatedEnumByteSize = 8;
                        }
                        else if (value > ushort.MaxValue)
                        {
                            calculatedEnumByteSize = 4;
                        }
                        else if (value > byte.MaxValue)
                        {
                            calculatedEnumByteSize = 2;
                        }
                    }
                    break;
            }

            return values;

        }

        /// <summary>
        /// Returns the calculated size of the enum in bytes (based on the values in the enum. To get the actual
        /// underlying type size use Type.GetTypeCode(type.GetEnumUnderlyingType()))
        /// </summary>
        /// <param name="type">The type of the enum</param>
        /// <returns>The calculated size of the enum in bytes</returns>
        public static byte GetEnumByteSize(this Type type)
        {
            Type enumType = type;
            if (type.IsByRef && type.HasElementType)
            {
                enumType = type.GetElementType();
            }

            if (!enumType.IsEnum)
            {
                throw new Exception("Attempting to get the enum byte size from a non-enum type");
            }

            // Is there a better way to do this without the duplicate code? (and without crashing
            // due to type mismatch when converting the enum value type)
            byte enumSize = 1;
            switch (Type.GetTypeCode(enumType.GetEnumUnderlyingType()))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                    enumSize = 1;
                    break;

                case TypeCode.Int16:
                    foreach (short signedValue in Enum.GetValues(enumType))
                    {
                        ushort value = (ushort)signedValue;
                        if (value > byte.MaxValue)
                        {
                            enumSize = 2;
                            break;
                        }
                    }
                    break;
                case TypeCode.Int32:
                    foreach (int signedValue in Enum.GetValues(enumType))
                    {
                        uint value = (uint)signedValue;
                        if (value > ushort.MaxValue)
                        {
                            enumSize = 4;
                            break;
                        }
                        else if (value > byte.MaxValue)
                        {
                            enumSize = 2;
                        }
                    }
                    break;
                case TypeCode.Int64:
                    foreach (long signedValue in Enum.GetValues(enumType))
                    {
                        ulong value = (ulong)signedValue;
                        if (value > uint.MaxValue)
                        {
                            enumSize = 8;
                            break;
                        }
                        else if (value > ushort.MaxValue)
                        {
                            enumSize = 4;
                        }
                        else if (value > byte.MaxValue)
                        {
                            enumSize = 2;
                        }
                    }
                    break;
                case TypeCode.UInt16:
                    foreach (ushort value in Enum.GetValues(enumType))
                    {
                        if (value > byte.MaxValue)
                        {
                            enumSize = 2;
                            break;
                        }
                    }
                    break;
                case TypeCode.UInt32:
                    foreach (uint value in Enum.GetValues(enumType))
                    {
                        if (value > ushort.MaxValue)
                        {
                            enumSize = 4;
                            break;
                        }
                        else if (value > byte.MaxValue)
                        {
                            enumSize = 2;
                        }
                    }
                    break;
                case TypeCode.UInt64:
                    foreach (ulong value in Enum.GetValues(enumType))
                    {
                        if (value > uint.MaxValue)
                        {
                            enumSize = 8;
                            break;
                        }
                        else if (value > ushort.MaxValue)
                        {
                            enumSize = 4;
                        }
                        else if (value > byte.MaxValue)
                        {
                            enumSize = 2;
                        }
                    }
                    break;
            }
            return enumSize;
        }
    }
}

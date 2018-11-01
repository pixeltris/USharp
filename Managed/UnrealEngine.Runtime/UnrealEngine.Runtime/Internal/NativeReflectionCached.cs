using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Cached version of NativeReflection for accessing the reflection system without creating managed objects.
    /// This uses simple caching to avoid multiple lookups of FindField which would otherwise use linear linked list lookups.
    /// </summary>
    public class NativeReflectionCached
    {
        // TODO: Merge GetPropertyRef / GetPropertyOffset / validate
        // GetPropertyRefOffset, GetPropertyRefOffsetAndValidate, GetPropertyOffsetAndValidate, GetPropertyRefAndValidate

        struct CachedFieldInfo
        {
            public IntPtr Address;
            public int Offset;
        }

        private static Dictionary<string, CachedFieldInfo> lastUnrealStructChildren = new Dictionary<string, CachedFieldInfo>();
        private static IntPtr lastUnrealStruct;

        private static Dictionary<string, CachedFieldInfo> lastUnrealFunctionChildren = new Dictionary<string, CachedFieldInfo>();
        private static IntPtr lastUnrealFunction;

        // Cache a single FString for getting names
        private static FStringUnsafe nameUnsafe = new FStringUnsafe();

        public static void Clear()
        {
            lastUnrealStructChildren.Clear();
            lastUnrealFunctionChildren.Clear();
            lastUnrealStruct = IntPtr.Zero;
            lastUnrealFunction = IntPtr.Zero;
        }

        public static IntPtr GetFunction(IntPtr unrealClass, string functionName)
        {
            return FindField(Classes.UFunction, unrealClass, functionName);
        }

        public static IntPtr GetProperty(IntPtr unrealStruct, string propertyName)
        {
            if (unrealStruct == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            return FindField(unrealStruct, propertyName);
        }

        public static bool GetPropertyRef(ref UFieldAddress property, IntPtr unrealStruct, string propertyName)
        {
            if (property == null)
            {
                property = new UFieldAddress();
            }
            return property.Update(GetProperty(unrealStruct, propertyName));
        }

        public static int GetPropertyOffset(IntPtr unrealStruct, string propertyName)
        {
            CachedFieldInfo fieldInfo;
            if (FindFieldInfo(Classes.UProperty, unrealStruct, propertyName, out fieldInfo))
            {
                return fieldInfo.Offset;
            }
            return 0;
        }

        public static int GetPropertyArrayElementSize(IntPtr unrealStruct, string propertyName)
        {
            if (unrealStruct == IntPtr.Zero)
            {
                return 0;
            }
            IntPtr arrayProperty = FindField(Classes.UArrayProperty, unrealStruct, propertyName);
            if (arrayProperty == IntPtr.Zero)
            {
                return 0;
            }
            IntPtr innerProperty = Native_UArrayProperty.Get_Inner(arrayProperty);
            return Native_UProperty.GetSize(innerProperty);
        }

        public static ushort GetPropertyRepIndex(IntPtr unrealStruct, string propertyName)
        {
            IntPtr property = FindField(unrealStruct, propertyName);
            return property == IntPtr.Zero ? (ushort)0 : Native_UProperty.Get_RepIndex(property);
        }

        public static IntPtr FindField(IntPtr unrealStruct, FName fieldName)
        {
            if (fieldName == FName.None)
            {
                return IntPtr.Zero;
            }
            return FindField(unrealStruct, fieldName.ToString());
        }

        public static IntPtr FindField(IntPtr typeClass, IntPtr unrealStruct, FName fieldName)
        {
            if (fieldName == FName.None)
            {
                return IntPtr.Zero;
            }
            return FindField(typeClass, unrealStruct, fieldName.ToString());
        }

        public static IntPtr FindField(IntPtr unrealStruct, string fieldName)
        {
            return FindField(Classes.UProperty, unrealStruct, fieldName);
        }

        public static IntPtr FindField(IntPtr typeClass, IntPtr unrealStruct, string fieldName)
        {
            CachedFieldInfo fieldInfo;
            if (FindFieldInfo(typeClass, unrealStruct, fieldName, out fieldInfo))
            {
                return fieldInfo.Address;
            }
            return IntPtr.Zero;
        }

        private static bool FindFieldInfo(IntPtr typeClass, IntPtr unrealStruct, string fieldName, out CachedFieldInfo fieldInfo)
        {
            if (typeClass == IntPtr.Zero || unrealStruct == IntPtr.Zero || string.IsNullOrEmpty(fieldName))
            {
                fieldInfo = default(CachedFieldInfo);
                return false;
            }

            if (unrealStruct == lastUnrealStruct)
            {
                return lastUnrealStructChildren.TryGetValue(fieldName, out fieldInfo);
            }
            else if (unrealStruct == lastUnrealFunction)
            {
                return lastUnrealFunctionChildren.TryGetValue(fieldName, out fieldInfo);
            }
            else
            {
                Dictionary<string, CachedFieldInfo> fields = null;

                if (Native_UObjectBaseUtility.IsA(unrealStruct, Classes.UFunction))
                {
                    fields = lastUnrealFunctionChildren;
                    lastUnrealFunction = unrealStruct;
                }
                else
                {
                    fields = lastUnrealStructChildren;
                    lastUnrealStruct = unrealStruct;
                }

                fields.Clear();

                foreach (IntPtr field in new NativeReflection.NativeFieldIterator(
                    EClassCastFlags.UFunction | EClassCastFlags.UProperty, unrealStruct, false, false))
                {
                    Native_UObjectBaseUtility.GetNameOut(field, ref nameUnsafe.Array);
                    string name = nameUnsafe.Value;

                    // Temporary check for debugging purposes. There shouldn't be any duplicate fields as we aren't looking in the hierarchy.
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debug.Assert(!fields.ContainsKey(name));
                    }

                    fields[name] = new CachedFieldInfo()
                    {
                        Address = field,
                        Offset = Native_UProperty.GetOffset_ForInternal(field)
                    };
                }

                return fields.TryGetValue(fieldName, out fieldInfo);
            }
        }

        /// <summary>
        /// Validates that the given property exists and matches the given UProperty class (e.g. UBoolProperty::StaticClass())
        /// </summary>
        /// <param name="unrealStruct">The address of the structure which owns the property</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="propertyClass">The expected UProperty class of the property</param>
        /// <returns></returns>
        public static bool ValidatePropertyClass(IntPtr unrealStruct, string propertyName, IntPtr propertyClass)
        {
            IntPtr field = FindField(unrealStruct, propertyName);
            if (field == IntPtr.Zero || !Native_UObjectBaseUtility.IsA(field, Classes.UProperty))
            {
                return false;
            }
            IntPtr actualClass = Native_UObjectBase.GetClass(field);
            if (actualClass == propertyClass)
            {
                return true;
            }
            if (actualClass != IntPtr.Zero && propertyClass == Classes.UEnumProperty &&
                Native_UStruct.IsChildOf(actualClass, Classes.UNumericProperty))
            {
                return Native_UNumericProperty.IsEnum(field);
            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Helper class for getting the default value for the given unreal UScriptStruct type. This is required for structs
    /// which have a custom default constructor (i.e. where EPropertyFlags.ZeroConstructor is false).
    /// </summary>
    public static class StructDefault<T>
    {
        private static IntPtr unrealStruct;
        private static int structSize;
        private static bool isLoaded;
        private static bool useDefaultT;// use default(T) instead of constructing a native struct
        private static readonly string structPath;
        private static readonly MarshalingDelegates<T>.FromNative fromNative;

        /// <summary>
        /// True if the type T is a UScriptStruct type
        /// </summary>
        public static readonly bool IsStruct;

        /// <summary>
        /// True if the type T is a UScriptStruct type but is treated as a class in managed code
        /// </summary>
        public static readonly bool IsStructAsClass;

        /// <summary>
        /// True if the type T is a blittable UScriptStruct type (determined by if type T has the FromNative/ToNative methods)
        /// </summary>
        public static readonly bool IsBlittableStruct;

        public static int Size
        {
            get
            {
                if (!isLoaded)
                {
                    Load();
                }
                return structSize;
            }
        }

        public static T Value
        {
            get
            {
                if (useDefaultT)
                {
                    return default(T);
                }

                if (!isLoaded)
                {
                    Load();
                    if (!isLoaded || useDefaultT)
                    {
                        return default(T);
                    }
                }

                if (IsStructAsClass)
                {
                    // Make sure this allocation stays in sync with the allocation method used in StructAsClassBase.Initialize
                    IntPtr buffer = FMemory.Malloc(structSize);
                    Native_UStruct.InitializeStruct(unrealStruct, buffer, 1);
                    T result = fromNative(buffer, 0, IntPtr.Zero);
                    return result;
                }
                else
                {
                    unsafe
                    {
                        byte* bufferPtr = stackalloc byte[structSize];
                        IntPtr buffer = (IntPtr)bufferPtr;

                        Native_UStruct.InitializeStruct(unrealStruct, buffer, 1);
                        T result = fromNative(buffer, 0, IntPtr.Zero);
                        Native_UStruct.DestroyStruct(unrealStruct, buffer, 1);

                        return result;
                    }
                }
            }
        }

        static void Load()
        {
            unrealStruct = UScriptStruct.ResolveStructAddress(structPath);
            if (unrealStruct != IntPtr.Zero)
            {
                isLoaded = true;
                structSize = NativeReflection.GetStructSize(unrealStruct);

                if (!IsStructAsClass)
                {
                    // NOTE:  UScriptStruct::CopyScriptStruct uses the following logic which we may want to use instead
                    //
                    // if (StructFlags & STRUCT_CopyNative) { GetCppStructOps()->Copy(); }
                    // else if (StructFlags & STRUCT_IsPlainOldData) { FMemory::Memcpy(); }
                    // else { foreach(prop) CopyCompleteValue_InContainer(prop, ...); }

                    if (UScriptStruct.IsPODZeroInit(unrealStruct))
                    {
                        useDefaultT = true;
                    }
                }
            }
        }

        static StructDefault()
        {
            Type type = typeof(T);

            IsStructAsClass = type.IsSubclassOf(typeof(StructAsClass));

            if (!type.IsValueType && !IsStructAsClass)
            {
                useDefaultT = true;
                return;
            }

            UUnrealTypePathAttribute pathAttribute = UnrealTypes.GetPathAttribute(type);
            if (UnrealTypes.All.TryGetValue(type, out pathAttribute) && !string.IsNullOrEmpty(pathAttribute.Path))
            {
                structPath = pathAttribute.Path;
            }

            IsStruct = !string.IsNullOrEmpty(structPath);

            if (IsStructAsClass)
            {
                // All structs as classes must be allocated on the heap and cannot use default(T)
                useDefaultT = false;
                fromNative = MarshalingDelegateResolver<T>.FromNative;
                return;
            }

            if (!IsStruct)
            {
                useDefaultT = true;
                return;
            }

            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                if (method.Name == "FromNative" && method.GetParameters().Length == 3)
                {
                    fromNative = (MarshalingDelegates<T>.FromNative)Delegate.CreateDelegate(typeof(MarshalingDelegates<T>.FromNative), method);
                    break;
                }
            }

            if (fromNative != null)
            {
                IsBlittableStruct = false;
            }
            else
            {
                IsBlittableStruct = true;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // Lots of duplicate code here as these marshalers are going to be called often
    // - If there is little impact on performance remove all of the duplicate code and just use
    //   the signature (IntPtr nativeBuffer, int arrayIndex, IntPtr prop, UObject owner, T value)

    // Collection marshalers can be found in TArray/TSet/TMap files.
    // All other marshalers should be here.

    public static class MarshalingDelegates<T>
    {
        public delegate T FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop);
        public delegate void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, T value);
        public delegate void Destroy(IntPtr nativeBuffer, int arrayIndex, IntPtr prop);
    }

    /// <summary>
    /// Used to cache delegates for the given marshaler type
    /// </summary>
    public static class CachedMarshalingDelegates<T, TMarshaler>
    {
        // Using MarshalingDelegateResolver for now. It isn't likely to work on platforms such as iOS where AOT is required.
        // Move back to the old code if MarshalingDelegateResolver isn't usable everywhere.
        // (MarshalingDelegateResolver is a little nicer as we don't need to provide the marshaler, but it doesn't matter
        //  too much since it's just used by generated code anyway)
        public static readonly MarshalingDelegates<T>.FromNative FromNative = MarshalingDelegateResolver<T>.FromNative;
        public static readonly MarshalingDelegates<T>.ToNative ToNative = MarshalingDelegateResolver<T>.ToNative;
        public static readonly MarshalingDelegates<T>.Destroy Destroy = MarshalingDelegateResolver<T>.Destroy;

        //static CachedMarshalingDelegates()
        //{
        //    Type type = typeof(TMarshaler);
        //
        //    MethodInfo fromNativeMethod = type.GetMethod("FromNative",
        //        new Type[] { typeof(IntPtr), typeof(int), typeof(IntPtr) });
        //
        //    MethodInfo toNativeMethod = type.GetMethod("ToNative",
        //        new Type[] { typeof(IntPtr), typeof(int), typeof(IntPtr), typeof(T) });
        //
        //    MethodInfo destroyMethod = type.GetMethod("Destroy",
        //        new Type[] { typeof(IntPtr), typeof(int), typeof(IntPtr) });
        //
        //    Debug.Assert(fromNativeMethod != null);
        //    Debug.Assert(toNativeMethod != null);
        //
        //    FromNative = (MarshalingDelegates<T>.FromNative)Delegate.CreateDelegate(typeof(MarshalingDelegates<T>.FromNative), fromNativeMethod);
        //    Debug.Assert(FromNative != null);
        //
        //    ToNative = (MarshalingDelegates<T>.ToNative)Delegate.CreateDelegate(typeof(MarshalingDelegates<T>.ToNative), toNativeMethod);
        //    Debug.Assert(ToNative != null);
        //
        //    if (destroyMethod != null)
        //    {
        //        Destroy = (MarshalingDelegates<T>.Destroy)Delegate.CreateDelegate(typeof(MarshalingDelegates<T>.Destroy), destroyMethod);
        //        Debug.Assert(Destroy != null);
        //    }
        //}
    }

    /// <summary>
    /// Marshaler type map for types which don't require IsSubclassOf to resolve
    /// </summary>
    static class SimpleMarshalerTypeMap
    {
        private static Dictionary<Type, Type> marshalerTypeMap = new Dictionary<Type, Type>();

        public static Type GetMarshalerType(Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                type = type.GetGenericTypeDefinition();
            }

            Type marshalerType;
            marshalerTypeMap.TryGetValue(type, out marshalerType);
            return marshalerType;
        }

        static SimpleMarshalerTypeMap()
        {
            marshalerTypeMap.Add(typeof(TSubclassOf<>), typeof(TSubclassOfMarshaler<>));
            marshalerTypeMap.Add(typeof(TSubclassOfInterface<>), typeof(TSubclassOfInterfaceMarshaler<>));
            marshalerTypeMap.Add(typeof(TSoftClass<>), typeof(TSoftClassMarshaler<>));
            marshalerTypeMap.Add(typeof(TSoftObject<>), typeof(TSoftObjectMarshaler<>));
            marshalerTypeMap.Add(typeof(TWeakObject<>), typeof(TWeakObjectMarshaler<>));
            marshalerTypeMap.Add(typeof(TLazyObject<>), typeof(TLazyObjectMarshaler<>));
            marshalerTypeMap.Add(typeof(FSoftObjectPath), typeof(FSoftObjectPathMarshaler));
            marshalerTypeMap.Add(typeof(bool), typeof(BoolMarshaler));
            marshalerTypeMap.Add(typeof(string), typeof(FStringMarshaler));
            marshalerTypeMap.Add(typeof(FText), typeof(FTextMarshaler));
        }
    }

    public static class MarshalingDelegateResolverSlow
    {
        private static Dictionary<Type, Delegate> FromNative = new Dictionary<Type, Delegate>();
        private static Dictionary<Type, Delegate> ToNative = new Dictionary<Type, Delegate>();
        private static Dictionary<Type, Delegate> Destroy = new Dictionary<Type, Delegate>();

        public static Delegate GetFromNative(Type type)
        {
            return GetDelegate(type, FromNative);
        }

        public static Delegate GetToNative(Type type)
        {
            return GetDelegate(type, ToNative);
        }

        public static Delegate GetDestroy(Type type)
        {
            return GetDelegate(type, Destroy);
        }

        private static Delegate GetDelegate(Type type, Dictionary<Type, Delegate> collection)
        {
            Delegate method;
            if (!collection.TryGetValue(type, out method))
            {
                ProcessType(type);
                collection.TryGetValue(type, out method);
            }
            return method;
        }

        private static void ProcessType(Type type)
        {
            Type resolver = typeof(MarshalingDelegateResolver<>).MakeGenericType(type);
            FromNative.Add(type, resolver.GetField("FromNative").GetValue(null) as Delegate);
            ToNative.Add(type, resolver.GetField("ToNative").GetValue(null) as Delegate);
            Destroy.Add(type, resolver.GetField("Destroy").GetValue(null) as Delegate);
        }
    }

    public static class MarshalingDelegateResolver<T>
    {
        public static readonly MarshalingDelegates<T>.FromNative FromNative;
        public static readonly MarshalingDelegates<T>.ToNative ToNative;
        public static readonly MarshalingDelegates<T>.Destroy Destroy;

        static MarshalingDelegateResolver()
        {
            Type type = typeof(T);
            Type marshalerType = null;

            if (type.IsEnum)
            {
                marshalerType = typeof(EnumMarshaler<>).MakeGenericType(typeof(T));
            }
            else if (type.IsSameOrSubclassOf(typeof(UObject)))
            {
                marshalerType = typeof(UObjectMarshaler<>).MakeGenericType(typeof(T));
            }
            else if (type.IsSameOrSubclassOfGeneric(typeof(FDelegate<>)))
            {
                // This assumes delegates are never defined as generic
                marshalerType = typeof(FDelegateMarshaler<>).MakeGenericType(typeof(T));
            }
            else if (type.IsSameOrSubclassOfGeneric(typeof(FMulticastDelegate<>)))
            {
                // This assumes delegates are never defined as generic
                marshalerType = typeof(FMulticastDelegateMarshaler<>).MakeGenericType(typeof(T));
            }
            else if (type.IsSubclassOf(typeof(StructAsClass)))
            {
                marshalerType = typeof(StructAsClassMarshaler<>).MakeGenericType(typeof(T));
            }
            else if (type.IsInterface && typeof(IInterface).IsAssignableFrom(type) && type != typeof(IInterface))
            {
                marshalerType = typeof(InterfaceMarshaler<>).MakeGenericType(type);
            }
            else
            {
                marshalerType = SimpleMarshalerTypeMap.GetMarshalerType(type);
                if (marshalerType != null)
                {
                    if (marshalerType.IsGenericTypeDefinition)
                    {
                        // Only handling 1 generic argument
                        Debug.Assert(type.GetGenericArguments().Length == 1);

                        marshalerType = marshalerType.MakeGenericType(type.GetGenericArguments()[0]);
                    }
                }
                else if (type.IsValueType)
                {
                    if (StructDefault<T>.IsStruct && !StructDefault<T>.IsBlittableStruct)
                    {
                        // Non-blittable structs will contain their own marshaling methods
                        marshalerType = typeof(T);
                    }
                    else
                    {
                        marshalerType = typeof(BlittableTypeMarshaler<>).MakeGenericType(typeof(T));
                    }
                }
                else if (type.IsGenericType)
                {
                    // This is mostly for DynamicInvoker support (collection copy marshaler only)
                    Type genericType = type.GetGenericTypeDefinition();
                    Type[] interfaces = genericType.GetInterfaces();
                    for (int i = -1; i < interfaces.Length; i++)
                    {
                        Type interfaceType = null;
                        if (i == -1)
                        {
                            interfaceType = type.IsInterface ? type : null;
                        }
                        else
                        {
                            interfaceType = interfaces[i];
                        }

                        if (interfaceType != null && interfaceType.IsGenericType)
                        {
                            Type genericInterfaceType = interfaceType.GetGenericTypeDefinition();

                            if (genericInterfaceType.IsSameOrSubclassOf(typeof(ISet<>)))
                            {
                                marshalerType = typeof(TSetStaticCopyMarshaler<>).MakeGenericType(
                                    typeof(T).GetGenericArguments());
                                break;
                            }
                            if (genericInterfaceType.IsSameOrSubclassOf(typeof(IDictionary<,>)) ||
                                genericInterfaceType.IsSameOrSubclassOf(typeof(IReadOnlyDictionary<,>)))
                            {
                                marshalerType = typeof(TMapStaticCopyMarshaler<,>).MakeGenericType(
                                    typeof(T).GetGenericArguments());
                                break;
                            }
                            if (genericInterfaceType.IsSameOrSubclassOf(typeof(IList<>)) ||
                                genericInterfaceType.IsSameOrSubclassOf(typeof(IReadOnlyList<>)))
                            {
                                marshalerType = typeof(TArrayStaticCopyMarshaler<>).MakeGenericType(
                                    typeof(T).GetGenericArguments());
                                break;
                            }
                        }
                    }
                }
            }

            Debug.Assert(marshalerType != null, "Failed to find the marshaler for the given type " + type.FullName);

            MethodInfo fromNativeMethod = marshalerType.GetMethod("FromNative",
                new Type[] { typeof(IntPtr), typeof(int), typeof(IntPtr) });

            MethodInfo toNativeMethod = marshalerType.GetMethod("ToNative",
                new Type[] { typeof(IntPtr), typeof(int), typeof(IntPtr), typeof(T) });

            MethodInfo destroyMethod = marshalerType.GetMethod("Destroy",
                new Type[] { typeof(IntPtr), typeof(int), typeof(IntPtr) });

            Debug.Assert(fromNativeMethod != null);
            Debug.Assert(toNativeMethod != null);

            FromNative = (MarshalingDelegates<T>.FromNative)Delegate.CreateDelegate(typeof(MarshalingDelegates<T>.FromNative), fromNativeMethod);
            Debug.Assert(FromNative != null);

            ToNative = (MarshalingDelegates<T>.ToNative)Delegate.CreateDelegate(typeof(MarshalingDelegates<T>.ToNative), toNativeMethod);
            Debug.Assert(ToNative != null);

            if (destroyMethod != null)
            {
                Destroy = (MarshalingDelegates<T>.Destroy)Delegate.CreateDelegate(typeof(MarshalingDelegates<T>.Destroy), destroyMethod);
                Debug.Assert(Destroy != null);
            }
        }
    }

    /// <summary>
    /// Handles marshaling of fixed sized arrays.
    /// Note that this doesn't work with TArray, TSet, TMap (which is fine as unreal doesn't support arrays of collections)
    /// FHeaderParser::GetVarNameAndDim - "Static arrays of containers are not allowed"
    /// </summary>
    public class TFixedSizeArrayMarshaler<T>
    {
        static readonly MarshalingDelegates<T>.FromNative fromNative = MarshalingDelegateResolver<T>.FromNative;
        static readonly MarshalingDelegates<T>.ToNative toNative = MarshalingDelegateResolver<T>.ToNative;

        //public class TFixedSizeArrayMarshaler<T, TMarshaler>
        //    static readonly MarshalingDelegates<T>.FromNative fromNative = CachedMarshalingDelegates<T, TMarshaler>.FromNative;
        //    static readonly MarshalingDelegates<T>.ToNative toNative = CachedMarshalingDelegates<T, TMarshaler>.ToNative;

        public static T[] FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            int arrayDim = Native_UProperty.Get_ArrayDim(prop);
            T[] result = new T[arrayDim];
            for (int i = 0; i < arrayDim; i++)
            {
                result[i] = fromNative(nativeBuffer, i, prop);
            }
            return result;
        }
        
        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, T[] value)
        {
            int arrayDim = Native_UProperty.Get_ArrayDim(prop);
            for (int i = 0; i < arrayDim; i++)
            {
                toNative(nativeBuffer, i, prop, value == null || i >= value.Length ? StructDefault<T>.Value : value[i]);
            }
        }
    }

    public class InterfaceMarshaler<T> where T : class, IInterface
    {
        private static IntPtr interfaceClassAddress;
        internal static void UpdateInterfaceClassAddress()
        {
            interfaceClassAddress = UClass.GetInterfaceClassAddress<T>();
        }

        public static T FromNative(IntPtr nativeBuffer)
        {
            return FromNative(nativeBuffer, 0, IntPtr.Zero);
        }

        public static void ToNative(IntPtr nativeBuffer, T value)
        {
            ToNative(nativeBuffer, 0, IntPtr.Zero, value);
        }

        public static T FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            FScriptInterface scriptInterface = BlittableTypeMarshaler<FScriptInterface>.FromNative(nativeBuffer, arrayIndex, prop);
            UObject obj = GCHelper.Find(scriptInterface.ObjectPointer);
            if (obj != null)
            {
                return obj.GetInterface<T>();
            }
            return null;
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, T value)
        {
            IntPtr obj = value.GetAddress();
            IntPtr interfacePtr = IntPtr.Zero;
            if (obj != IntPtr.Zero)
            {
                // TODO: Cache this instead of updating it every call. We need to take into account
                //       hotreload too (call it from UClass.Load() / UClass.RegisterManagedClass() / 
                //       other method when we do proper hotrloading of native classes)
                UpdateInterfaceClassAddress();
                if (interfaceClassAddress != IntPtr.Zero)
                {
                    interfacePtr = Native_UObjectBaseUtility.GetInterfaceAddress(obj, interfaceClassAddress);
                }
            }

            if (obj != IntPtr.Zero && interfacePtr != IntPtr.Zero)
            {
                BlittableTypeMarshaler<FScriptInterface>.ToNative(nativeBuffer, arrayIndex, new FScriptInterface(obj, interfacePtr));
            }
            else
            {
                BlittableTypeMarshaler<FScriptInterface>.ToNative(nativeBuffer, arrayIndex, default(FScriptInterface));
            }
        }
    }

    public class UObjectMarshaler<T> where T : UObject
    {
        public static T FromNative(IntPtr nativeBuffer)
        {
            return GCHelper.Find<T>(Marshal.ReadIntPtr(nativeBuffer));
        }

        public static void ToNative(IntPtr nativeBuffer, T value)
        {
            Marshal.WriteIntPtr(nativeBuffer, value == null ? IntPtr.Zero : value.Address);
        }

        public static T FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            IntPtr elementPtr = nativeBuffer + (arrayIndex * IntPtr.Size);
            return GCHelper.Find<T>(Marshal.ReadIntPtr(elementPtr));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, T value)
        {
            IntPtr elementPtr = nativeBuffer + (arrayIndex * IntPtr.Size);
            Marshal.WriteIntPtr(elementPtr, value == null ? IntPtr.Zero : value.Address);
        }
    }

    public class TSubclassOfInterfaceMarshaler<T> where T : class, IInterface
    {
        public static TSubclassOfInterface<T> FromNative(IntPtr nativeBuffer)
        {
            FSubclassOf subclassOf = BlittableTypeMarshaler<FSubclassOf>.FromNative(nativeBuffer);
            return new TSubclassOfInterface<T>(subclassOf.Class);
        }

        public static void ToNative(IntPtr nativeBuffer, TSubclassOfInterface<T> value)
        {
            BlittableTypeMarshaler<FSubclassOf>.ToNative(nativeBuffer, value.subclassOf);
        }

        public static TSubclassOfInterface<T> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            FSubclassOf subclassOf = BlittableTypeMarshaler<FSubclassOf>.FromNative(nativeBuffer, arrayIndex, prop);
            return new TSubclassOfInterface<T>(subclassOf.Class);
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, TSubclassOfInterface<T> value)
        {
            BlittableTypeMarshaler<FSubclassOf>.ToNative(nativeBuffer, arrayIndex, prop, value.subclassOf);
        }
    }

    public class TSubclassOfMarshaler<T> where T : UObject
    {
        public static TSubclassOf<T> FromNative(IntPtr nativeBuffer)
        {
            FSubclassOf subclassOf = BlittableTypeMarshaler<FSubclassOf>.FromNative(nativeBuffer);
            return new TSubclassOf<T>(subclassOf.Class);
        }

        public static void ToNative(IntPtr nativeBuffer,  TSubclassOf<T> value)
        {
            BlittableTypeMarshaler<FSubclassOf>.ToNative(nativeBuffer, value.subclassOf);
        }

        public static TSubclassOf<T> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            FSubclassOf subclassOf = BlittableTypeMarshaler<FSubclassOf>.FromNative(nativeBuffer, arrayIndex, prop);
            return new TSubclassOf<T>(subclassOf.Class);
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, TSubclassOf<T> value)
        {
            BlittableTypeMarshaler<FSubclassOf>.ToNative(nativeBuffer, arrayIndex, prop, value.subclassOf);
        }
    }

    public class TSoftClassMarshaler<T> where T : UObject
    {
        public static TSoftClass<T> FromNative(IntPtr nativeBuffer)
        {
            FSoftObjectPtrUnsafe objectPtr = BlittableTypeMarshaler<FSoftObjectPtrUnsafe>.FromNative(nativeBuffer);
            return new TSoftClass<T>(objectPtr.ObjectPath);
        }

        public static void ToNative(IntPtr nativeBuffer, TSoftClass<T> value)
        {
            // Destroy existing memory
            Destroy(nativeBuffer, 0, IntPtr.Zero);

            // This assumes the given address is responsible for destroying the memory (this allocates an FString)
            BlittableTypeMarshaler<FSoftObjectPtrUnsafe>.ToNative(nativeBuffer, new FSoftObjectPtrUnsafe(value.ObjectPath));
        }

        public static TSoftClass<T> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            FSoftObjectPtrUnsafe objectPtr = BlittableTypeMarshaler<FSoftObjectPtrUnsafe>.FromNative(nativeBuffer, arrayIndex, prop);
            return new TSoftClass<T>(objectPtr.ObjectPath);
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, TSoftClass<T> value)
        {
            // Destroy existing memory
            Destroy(nativeBuffer, arrayIndex, prop);

            // This assumes the given address is responsible for destroying the memory (this allocates an FString)
            BlittableTypeMarshaler<FSoftObjectPtrUnsafe>.ToNative(nativeBuffer, arrayIndex, prop, new FSoftObjectPtrUnsafe(value.ObjectPath));
        }

        public static void Destroy(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            int size = Marshal.SizeOf(typeof(FSoftObjectPtrUnsafe));
            IntPtr elementPtr = nativeBuffer + (arrayIndex * size);
            Marshal.PtrToStructure<FSoftObjectPtrUnsafe>(elementPtr).Dispose();

            // Zero the memory in case this address is used again
            FMemory.Memset(elementPtr, 0, size);
        }
    }

    public class TSoftObjectMarshaler<T> where T : UObject
    {
        public static TSoftObject<T> FromNative(IntPtr nativeBuffer)
        {
            FSoftObjectPtrUnsafe objectPtr = BlittableTypeMarshaler<FSoftObjectPtrUnsafe>.FromNative(nativeBuffer);
            return new TSoftObject<T>(objectPtr.ObjectPath);
        }

        public static void ToNative(IntPtr nativeBuffer, TSoftObject<T> value)
        {
            // Destroy existing memory
            Destroy(nativeBuffer, 0, IntPtr.Zero);

            // This assumes the given address is responsible for destroying the memory (this allocates an FString)
            BlittableTypeMarshaler<FSoftObjectPtrUnsafe>.ToNative(nativeBuffer, new FSoftObjectPtrUnsafe(value.ObjectPath));
        }

        public static TSoftObject<T> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            FSoftObjectPtrUnsafe objectPtr = BlittableTypeMarshaler<FSoftObjectPtrUnsafe>.FromNative(nativeBuffer, arrayIndex, prop);
            return new TSoftObject<T>(objectPtr.ObjectPath);
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, TSoftObject<T> value)
        {
            // Destroy existing memory
            Destroy(nativeBuffer, arrayIndex, prop);

            // This assumes the given address is responsible for destroying the memory (this allocates an FString)
            BlittableTypeMarshaler<FSoftObjectPtrUnsafe>.ToNative(nativeBuffer, arrayIndex, prop, new FSoftObjectPtrUnsafe(value.ObjectPath));
        }

        public static void Destroy(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            int size = Marshal.SizeOf(typeof(FSoftObjectPtrUnsafe));
            IntPtr elementPtr = nativeBuffer + (arrayIndex * size);
            Marshal.PtrToStructure<FSoftObjectPtrUnsafe>(elementPtr).Dispose();

            // Zero the memory in case this address is used again
            FMemory.Memset(elementPtr, 0, size);
        }
    }

    public class TWeakObjectMarshaler<T> where T : UObject
    {
        public static TWeakObject<T> FromNative(IntPtr nativeBuffer)
        {
            FWeakObjectPtr weakObjectPtr = BlittableTypeMarshaler<FWeakObjectPtr>.FromNative(nativeBuffer);
            return new TWeakObject<T>(weakObjectPtr);
        }

        public static void ToNative(IntPtr nativeBuffer, TWeakObject<T> value)
        {
            BlittableTypeMarshaler<FWeakObjectPtr>.ToNative(nativeBuffer, value.weakObjectPtr);
        }

        public static TWeakObject<T> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            FWeakObjectPtr weakObjectPtr = BlittableTypeMarshaler<FWeakObjectPtr>.FromNative(nativeBuffer, arrayIndex, prop);
            return new TWeakObject<T>(weakObjectPtr);
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, TWeakObject<T> value)
        {
            BlittableTypeMarshaler<FWeakObjectPtr>.ToNative(nativeBuffer, arrayIndex, prop, value.weakObjectPtr);
        }
    }

    public class TLazyObjectMarshaler<T> where T : UObject
    {
        public static TLazyObject<T> FromNative(IntPtr nativeBuffer)
        {
            FLazyObjectPtr lazyObjectPtr = BlittableTypeMarshaler<FLazyObjectPtr>.FromNative(nativeBuffer);
            return new TLazyObject<T>(lazyObjectPtr);
        }

        public static void ToNative(IntPtr nativeBuffer, TLazyObject<T> value)
        {
            BlittableTypeMarshaler<FLazyObjectPtr>.ToNative(nativeBuffer, value.lazyObjectPtr);
        }

        public static TLazyObject<T> FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            FLazyObjectPtr lazyObjectPtr = BlittableTypeMarshaler<FLazyObjectPtr>.FromNative(nativeBuffer, arrayIndex, prop);
            return new TLazyObject<T>(lazyObjectPtr);
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, TLazyObject<T> value)
        {
            BlittableTypeMarshaler<FLazyObjectPtr>.ToNative(nativeBuffer, arrayIndex, prop, value.lazyObjectPtr);
        }
    }

    public class FSoftObjectPathMarshaler
    {
        public static FSoftObjectPath FromNative(IntPtr nativeBuffer)
        {
            return new FSoftObjectPath(nativeBuffer);
        }

        public static void ToNative(IntPtr nativeBuffer, FSoftObjectPath value)
        {
            // Destroy existing memory
            Destroy(nativeBuffer, 0, IntPtr.Zero);

            // This assumes the given address is responsible for destroying the memory (this allocates an FString)
            BlittableTypeMarshaler<FSoftObjectPathUnsafe>.ToNative(nativeBuffer, new FSoftObjectPathUnsafe(value));
        }

        public static FSoftObjectPath FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            FSoftObjectPathUnsafe softObjectPath = BlittableTypeMarshaler<FSoftObjectPathUnsafe>.FromNative(nativeBuffer, arrayIndex, prop);
            return new FSoftObjectPath(softObjectPath);
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, FSoftObjectPath value)
        {
            // Destroy existing memory
            Destroy(nativeBuffer, arrayIndex, prop);

            // This assumes the given address is responsible for destroying the memory (this allocates an FString)
            BlittableTypeMarshaler<FSoftObjectPathUnsafe>.ToNative(nativeBuffer, arrayIndex, prop, new FSoftObjectPathUnsafe(value));
        }

        public static void Destroy(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            int size = Marshal.SizeOf(typeof(FSoftObjectPathUnsafe));
            IntPtr elementPtr = nativeBuffer + (arrayIndex * size);
            Marshal.PtrToStructure<FSoftObjectPathUnsafe>(elementPtr).Dispose();

            // Zero the memory in case this address is used again
            FMemory.Memset(elementPtr, 0, size);
        }
    }

    public class BlittableTypeMarshaler<T> where T : struct
    {
        private static readonly int TSize = Marshal.SizeOf<T>();

#if STRUCT_SAFEGUARDS
        // The struct layout of an unreal struct could potentially differ from the layout defined in
        // managed code (e.g. changes to editor defined structs). For non-blittable structs there are
        // generated safeguards to ensure invalid memory isn't accessed. For blittable structs this
        // marshaler is accessed directly. Add checks to ensure the struct size matches up.
        private static readonly bool isUnrealStruct = false;
        private static readonly string unrealStructPath = null;
        private static int nativeSize;

        static BlittableTypeMarshaler()
        {
            unrealStructPath = null;

            UMetaPathAttribute pathAttribute = typeof(T).GetCustomAttribute<UMetaPathAttribute>();
            if (pathAttribute != null && !string.IsNullOrEmpty(pathAttribute.Path))
            {
                unrealStructPath = pathAttribute.Path;
            }

            USharpPathAttribute sharpPathAttribute = typeof(T).GetCustomAttribute<USharpPathAttribute>();
            if (sharpPathAttribute != null && !string.IsNullOrEmpty(sharpPathAttribute.Path))
            {
                unrealStructPath = sharpPathAttribute.Path;
            }

            // TODO: Create a global delegate for type changes (struct,enum,class,delegate) and add a handler here
            LoadNativeStructSize();
        }

        private static void LoadNativeStructSize()
        {
            IntPtr unrealStruct = NativeReflection.GetStruct(unrealStructPath);
            nativeSize = NativeReflection.GetStructSize(unrealStruct);
        }

        public static T FromNative(IntPtr nativeBuffer)
        {
            if (nativeSize != TSize)
            {
                NativeReflection.LogInvalidStructAccessed(unrealStructPath);
                return default(T);
            }
            return Marshal.PtrToStructure<T>(nativeBuffer);
        }

        public static void ToNative(IntPtr nativeBuffer, T value)
        {
            if (nativeSize != TSize)
            {
                NativeReflection.LogInvalidStructAccessed(unrealStructPath);
                return;
            }
            Marshal.StructureToPtr<T>(value, nativeBuffer, false);
        }

        public static T FromNative(IntPtr nativeBuffer, int arrayIndex)
        {
            if (nativeSize != TSize)
            {
                NativeReflection.LogInvalidStructAccessed(unrealStructPath);
                return default(T);
            }
            return Marshal.PtrToStructure<T>(nativeBuffer + (arrayIndex * TSize));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, T value)
        {
            if (nativeSize != TSize)
            {
                NativeReflection.LogInvalidStructAccessed(unrealStructPath);
                return;
            }
            Marshal.StructureToPtr<T>(value, nativeBuffer + (arrayIndex * TSize), false);
        }

        public static T FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            if (nativeSize != TSize)
            {
                NativeReflection.LogInvalidStructAccessed(unrealStructPath);
                return default(T);
            }
            return Marshal.PtrToStructure<T>(nativeBuffer + (arrayIndex * TSize));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, T value)
        {
            if (nativeSize != TSize)
            {
                NativeReflection.LogInvalidStructAccessed(unrealStructPath);
                return;
            }
            Marshal.StructureToPtr<T>(value, nativeBuffer + (arrayIndex * TSize), false);
        }
#else
        public static T FromNative(IntPtr nativeBuffer)
        {
            return Marshal.PtrToStructure<T>(nativeBuffer);
        }

        public static void ToNative(IntPtr nativeBuffer, T value)
        {
            Marshal.StructureToPtr<T>(value, nativeBuffer, false);
        }

        public static T FromNative(IntPtr nativeBuffer, int arrayIndex)
        {
            return Marshal.PtrToStructure<T>(nativeBuffer + (arrayIndex * TSize));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, T value)
        {
            Marshal.StructureToPtr<T>(value, nativeBuffer + (arrayIndex * TSize), false);
        }

        public static T FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            return Marshal.PtrToStructure<T>(nativeBuffer + (arrayIndex * TSize));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, T value)
        {
            Marshal.StructureToPtr<T>(value, nativeBuffer + (arrayIndex * TSize), false);
        }
#endif
    }

    public class EnumMarshaler<T> where T : struct, IConvertible
    {
        // The reflection system supports non-byte enums but Blueprint visible enums can only be a byte.
        const int defaultSize = 1;

        static readonly TypeCode typeCode;// The type code of the underlying type of the enum
        static readonly int enumSize;// The size of the underlying type of the enum

        // Is __makeref safe to use on all platforms? If not then use the boxed version.

        // Note that it might be possible for an enum to be represented as different sizes depending on
        // how the UProperty is set up for that target enum.

        static EnumMarshaler()
        {
            typeCode = Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T)));
            switch (typeCode)
            {
                default:
                case TypeCode.Byte:
                case TypeCode.SByte:
                    enumSize = 1;
                    break;
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    enumSize = 2;
                    break;
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    enumSize = 4;
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    enumSize = 8;
                    break;
            }
        }

        public static T FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            int size = prop == IntPtr.Zero ? enumSize : Native_UProperty.Get_ElementSize(prop);
            return FromNativeConvert(nativeBuffer, arrayIndex, size);
            //return FromNativeUnsafe(nativeBuffer, arrayIndex, size);//doesn't work with mono
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, T value)
        {
            int size = prop == IntPtr.Zero ? enumSize : Native_UProperty.Get_ElementSize(prop);
            ToNativeConvert(nativeBuffer, arrayIndex, value, size);
            //ToNativeUnsafe(nativeBuffer, arrayIndex, value, size);//doesn't work with mono
        }

        private static T FromNativeConvert(IntPtr nativeBuffer, int arrayIndex, int size)
        {
            IntPtr address = nativeBuffer + (arrayIndex * size);

            switch (typeCode)
            {
                case TypeCode.SByte: return (T)(object)(sbyte)ReadValue(address, size);
                case TypeCode.Byte: return (T)(object)(byte)ReadValue(address, size);
                case TypeCode.Int16: return (T)(object)(short)ReadValue(address, size);
                case TypeCode.UInt16: return (T)(object)(ushort)ReadValue(address, size);
                case TypeCode.Int32: return (T)(object)(int)ReadValue(address, size);
                case TypeCode.UInt32: return (T)(object)(uint)ReadValue(address, size);
                case TypeCode.Int64: return (T)(object)(long)ReadValue(address, size);
                case TypeCode.UInt64: return (T)(object)(ulong)ReadValue(address, size);
                default: return default(T);
            }
        }

        public static void ToNativeConvert(IntPtr nativeBuffer, int arrayIndex, T value, int size)
        {            
            IntPtr address = nativeBuffer + (arrayIndex * size);

            switch (typeCode)
            {
                case TypeCode.SByte:
                    WriteValue(address, size, value.ToSByte(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.Byte:
                    WriteValue(address, size, value.ToByte(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.Int16:
                    WriteValue(address, size, value.ToInt16(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.UInt16:
                    WriteValue(address, size, value.ToUInt16(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.Int32:
                    WriteValue(address, size, value.ToUInt32(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.UInt32:
                    WriteValue(address, size, value.ToInt32(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.Int64:
                    WriteValue(address, size, value.ToInt64(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.UInt64:
                    WriteValue(address, size, (long)value.ToUInt64(CultureInfo.InvariantCulture));
                    break;
            }
        }

        public static T FromNativeUnsafe(IntPtr nativeBuffer, int arrayIndex, int size)
        {
            IntPtr address = nativeBuffer + (arrayIndex * size);

            unsafe
            {
                T val = default(T);
                TypedReference valRef = __makeref(val);
                switch (size)
                {
                    case 1:
                        *(byte*)(*((IntPtr*)&valRef)) = (byte)ReadValue(address, size);
                        break;
                    case 2:
                        *(short*)(*((IntPtr*)&valRef)) = (short)ReadValue(address, size);
                        break;
                    case 4:
                        *(int*)(*((IntPtr*)&valRef)) = (int)ReadValue(address, size);
                        break;
                    case 8:
                        *(long*)(*((IntPtr*)&valRef)) = (long)ReadValue(address, size);
                        break;
                }
                return val;
            }
        }

        private static void ToNativeUnsafe(IntPtr nativeBuffer, int arrayIndex, T value, int size)
        {
            IntPtr address = nativeBuffer + (arrayIndex * size);

            unsafe
            {                
                switch (size)
                {
                    case 1:
                        {
                            T val = value;
                            TypedReference valRef = __makeref(val);
                            WriteValue(address, size, *(byte*)(*((IntPtr*)&valRef)));
                        }
                        break;
                    case 2:
                        {
                            T val = value;
                            TypedReference valRef = __makeref(val);
                            WriteValue(address, size, *(short*)(*((IntPtr*)&valRef)));
                        }
                        break;
                    case 4:
                        {
                            T val = value;
                            TypedReference valRef = __makeref(val);
                            WriteValue(address, size, *(int*)(*((IntPtr*)&valRef)));
                        }
                        break;
                    case 8:
                        {
                            T val = value;
                            TypedReference valRef = __makeref(val);
                            WriteValue(address, size, *(long*)(*((IntPtr*)&valRef)));
                        }
                        break;
                }
            }
        }

        private static long ReadValue(IntPtr address, int size)
        {
            switch (size)
            {
                default:
                case 1: return Marshal.ReadByte(address);
                case 2: return Marshal.ReadInt16(address);
                case 4: return Marshal.ReadInt32(address);
                case 8: return Marshal.ReadInt64(address);
            }
        }

        private static void WriteValue(IntPtr address, int size, long value)
        {
            switch (size)
            {
                default:
                case 1:
                    Marshal.WriteByte(address, (byte)value);
                    break;
                case 2:
                    Marshal.WriteInt16(address, (short)value);
                    break;
                case 4:
                    Marshal.WriteInt32(address, (int)value);
                    break;
                case 8:
                    Marshal.WriteInt64(address, value);
                    break;
            }
        }
    }

    public class FDelegateMarshaler<T> where T : IDelegateBase, new()
    {
        public static T FromNative(IntPtr nativeBuffer)
        {
            T result = new T();
            result.FromNative(nativeBuffer);
            return result;
        }

        public static void ToNative(IntPtr nativeBuffer, T value)
        {
            if (value == null)
            {
                BlittableTypeMarshaler<FScriptDelegate>.ToNative(nativeBuffer, default(FScriptDelegate));
            }
            else
            {
                value.ToNative(nativeBuffer);
            }
        }

        public static T FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            T result = new T();
            result.FromNative(nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FScriptDelegate))));
            return result;
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, T value)
        {
            if (value == null)
            {
                BlittableTypeMarshaler<FScriptDelegate>.ToNative(nativeBuffer, arrayIndex, prop, default(FScriptDelegate));
            }
            else
            {
                value.ToNative(nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FScriptDelegate))));
            }
        }
    }

    public class FMulticastDelegateMarshaler<T> where T : IDelegateBase, new()
    {
        public static T FromNative(IntPtr nativeBuffer)
        {
            T result = new T();
            result.FromNative(nativeBuffer);
            return result;
        }

        public static void ToNative(IntPtr nativeBuffer, T value)
        {
            if (value == null)
            {
                unsafe
                {
                    ((FMulticastScriptDelegate*)nativeBuffer)->InvocationList.Destroy();
                }
                //BlittableTypeMarshaler<FMulticastScriptDelegate>.ToNative(nativeBuffer, default(FMulticastScriptDelegate));
            }
            else
            {
                value.ToNative(nativeBuffer);
            }
        }

        public static T FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            T result = new T();
            result.FromNative(nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FMulticastScriptDelegate))));
            return result;
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, T value)
        {
            if (value == null)
            {
                unsafe
                {
                    ((FMulticastScriptDelegate*)(nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FMulticastScriptDelegate)))))->InvocationList.Destroy();
                }
                //BlittableTypeMarshaler<FMulticastScriptDelegate>.ToNative(nativeBuffer, arrayIndex, prop, default(FMulticastScriptDelegate));
            }
            else
            {
                value.ToNative(nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FMulticastScriptDelegate))));
            }
        }
    }    
    
    public class BoolMarshaler
    {
        private static int boolSize = 4;
        public static int BoolSize
        {
            get { return boolSize; }
            private set { boolSize = value; }
        }

        // Does ElementSize work correctly with bit masked bool properties?
        // Get/SetPropertyValue should handle bitfield bools?

        public static bool FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            if (prop == IntPtr.Zero)
            {
                return FromNative(nativeBuffer, arrayIndex, BoolSize);
            }
            else
            {                
                return Native_UBoolProperty.GetPropertyValue(prop, nativeBuffer + (arrayIndex * Native_UProperty.Get_ElementSize(prop)));
            }
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, bool value)
        {
            if (prop == IntPtr.Zero)
            {
                ToNative(nativeBuffer, arrayIndex, value, BoolSize);
            }
            else
            {
                Native_UBoolProperty.SetPropertyValue(prop, nativeBuffer + (arrayIndex * Native_UProperty.Get_ElementSize(prop)), value);
            }
        }
        
        private static bool FromNative(IntPtr nativeBuffer, int arrayIndex, int size)
        {
            switch (size)
            {
                case 1: return BlittableTypeMarshaler<byte>.FromNative(nativeBuffer, arrayIndex, IntPtr.Zero) != 0;
                case 2: return BlittableTypeMarshaler<ushort>.FromNative(nativeBuffer, arrayIndex, IntPtr.Zero) != 0;
                case 4: return BlittableTypeMarshaler<uint>.FromNative(nativeBuffer, arrayIndex, IntPtr.Zero) != 0;
                case 8: return BlittableTypeMarshaler<ulong>.FromNative(nativeBuffer, arrayIndex, IntPtr.Zero) != 0;
                default: throw new NotImplementedException("Unexpected bool size " + size);
            }
        }
        
        private static void ToNative(IntPtr nativeBuffer, int arrayIndex, bool value, int size)
        {
            switch (size)
            {
                case 1: BlittableTypeMarshaler<byte>.ToNative(nativeBuffer, arrayIndex, IntPtr.Zero, value ? byte.MaxValue : (byte)0); break;
                case 2: BlittableTypeMarshaler<ushort>.ToNative(nativeBuffer, arrayIndex, IntPtr.Zero, value ? ushort.MaxValue : (ushort)0); break;
                case 4: BlittableTypeMarshaler<uint>.ToNative(nativeBuffer, arrayIndex, IntPtr.Zero, value ? uint.MaxValue : (uint)0); break;
                case 8: BlittableTypeMarshaler<ulong>.ToNative(nativeBuffer, arrayIndex, IntPtr.Zero, value ? ulong.MaxValue : (ulong)0); break;
                default: throw new NotImplementedException("Unexpected bool size " + size);
            }
        }

        internal static void OnNativeFunctionsRegistered()
        {
            // Move GetBoolSize somewhere more appropriate?
            BoolSize = Native_UBoolProperty.GetBoolSize();
        }
    }

    public class FStringMarshaler
    {
        public static Encoding Encoding
        {
            get
            {
                switch (CharSize)
                {
                    case 4: return Encoding.UTF32;
                    case 1: return Encoding.ASCII;
                    default: return Encoding.Unicode;
                }
            }
        }

        private static int charSize = 2;
        public static int CharSize
        {
            get { return charSize; }
            private set { charSize = value; }
        }

        public static readonly string DefaultString = string.Empty;

        public static string FromCharPtr(IntPtr address)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FString.FromCharPtr(address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        public static string FromPtr(IntPtr address, bool destory = false)
        {
            unsafe
            {
                if (address == IntPtr.Zero)
                {
                    return DefaultString;
                }
                FScriptArray* array = (FScriptArray*)address;
                return FromArray(ref *array, destory);
            }
        }

        public static string FromArray(FScriptArray array, bool destroy)
        {
            return FromArray(ref array, destroy);
        }

        public static string FromArray(ref FScriptArray array, bool destroy = false)
        {
            if (array.Data == IntPtr.Zero)
            {
                return DefaultString;
            }
            byte[] str = new byte[array.Count * charSize];
            Marshal.Copy(array.Data, str, 0, str.Length);
            string result = Encoding.GetString(str).TrimEnd('\0');
            if (destroy)
            {
                Native_FScriptArray.Destroy(ref array);
            }
            return result;
        }

        public static FScriptArray ToArray(ref FScriptArray array, string value)
        {
            if (value == null)
            {
                array.Destroy();
                return array;
            }

            byte[] buffer = Encoding.GetBytes(value);
            if (buffer.Length != value.Length * CharSize)
            {
                System.Diagnostics.Debugger.Break();
            }
            byte[] padding = new byte[CharSize];
            int paddedLen = value.Length + 1;
            array.Empty(paddedLen, CharSize);
            array.Add(CharSize, paddedLen);
            Marshal.Copy(padding, 0, array.Data + buffer.Length, padding.Length);
            if (array.Data != IntPtr.Zero && buffer.Length > 0)
            {
                Marshal.Copy(buffer, 0, array.Data, buffer.Length);
            }
            return array;
        }

        public static unsafe FScriptArray ToArray(IntPtr arrayAddress, string value)
        {
            FScriptArray array = *(FScriptArray*)arrayAddress;
            ToArray(ref array, value);
            *(FScriptArray*)arrayAddress = array;
            return array;
        }

        // Marshaling methods used by generated code

        public static string FromNative(IntPtr nativeBuffer)
        {
            return FromNative(nativeBuffer, 0, IntPtr.Zero);
        }

        public static string FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            unsafe
            {
                return FromPtr(nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FScriptArray))), false);
            }
        }

        public static void ToNative(IntPtr nativeBuffer, string value)
        {
            ToNative(nativeBuffer, 0, IntPtr.Zero, value);
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, string value)
        {
            // Note: This will free the existing memory if there is any. Don't call this on garbage memory.
            ToArray(nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FScriptArray))), value);
        }

        public static void Destroy(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            IntPtr elementPtr = nativeBuffer + (arrayIndex * Marshal.SizeOf(typeof(FScriptArray)));
            unsafe
            {
                FScriptArray* array = (FScriptArray*)elementPtr;
                array->Destroy();
            }
        }

        internal static void OnNativeFunctionsRegistered()
        {
            CharSize = Native_FString.GetCharSize();
        }
    }

    public class FTextMarshaler
    {
        public static FText FromNative(IntPtr nativeBuffer)
        {
            return FromNative(nativeBuffer, 0, IntPtr.Zero);
        }

        public static FText FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            FText result = new FText(nativeBuffer + (arrayIndex * FText.FTextNative.StructSize), false);
            return result;
        }

        public static void ToNative(IntPtr nativeBuffer, FText value)
        {
            ToNative(nativeBuffer, 0, IntPtr.Zero, value);
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, FText value)
        {
            unsafe
            {
                FText.FTextNative* from = (FText.FTextNative*)value.Address;
                FText.FTextNative* to = (FText.FTextNative*)(nativeBuffer + (arrayIndex * FText.FTextNative.StructSize));

                to->TextData.ReleaseSharedReference(ESPMode.ThreadSafe);
                *to = *from;
                to->TextData.AddSharedReference(ESPMode.ThreadSafe);
            }
        }
    }

    /*class FTextCachedMarshaler : CachedMarshaler<FText>
    {
        private FText[] items = null;

        public FTextCachedMarshaler(IntPtr address, UFieldAddress property, UObject owner)
            : base(address, property, owner)
        {
            items = new FText[FixedArrayLength];
        }

        protected override FText Create(int index)
        {
            return new FText(Address + (index * Property.GenericArg1Size), Owner);
        }

        public override FText Get(int index)
        {
            ValidateIndex(index, items);
            return items[index];
        }

        public override void Set(int index, FText value)
        {
            ValidateIndex(index, items);
            items[index].CopyFrom(value);
        }
    }

    // Really is a pseudo copy of TFixedSizeArray<>. Modify TFixedSizeArray<> to hold onto a nullable CachedMarshaler<T>?
    // and then use these custom get/set methods if there is an imeplementation for the given T?
    abstract class CachedMarshaler<T>
    {
        public int FixedArrayLength
        {
            get { return Property.ArrayDim; }
        }
        public readonly IntPtr Address;
        public readonly UObject Owner;
        public readonly UFieldAddress Property;

        public CachedMarshaler(IntPtr address, UFieldAddress property, UObject owner)
        {
            Address = address;            
            Property = property;
            Owner = owner;
        }

        public T Get()
        {
            return Get(0);
        }

        public void Set(T value)
        {
            Set(0, value);
        }

        protected abstract T Create(int index);
        public abstract T Get(int index);
        public abstract void Set(int index, T value);

        protected void ValidateIndex(int index)
        {
            if (index < 0 || index >= FixedArrayLength)
            {
                throw new IndexOutOfRangeException(string.Format("Index {0} out of bounds. Array is size {1}.", index, FixedArrayLength));
            }
        }

        protected void ValidateIndex(int index, T[] items)
        {
            ValidateIndex(index);
            if (items[index] == null)
            {
                items[index] = Create(index);
            }
        }
    }*/

    /// <summary>
    /// Marshaler for structs treated as a class in managed code due to large struct layout not suitable for
    /// marshaling at regular intervals
    /// </summary>
    public class StructAsClassMarshaler<T> where T : StructAsClass, new()
    {
        public static T FromNative(IntPtr nativeBuffer)
        {
            T result = new T();
            result.Address = nativeBuffer;
            result.Initialize();
            return result;
        }

        public static void ToNative(IntPtr nativeBuffer, T value)
        {
            if (value == null)
            {
                using (T defaultValue = StructDefault<T>.Value)
                {
                    defaultValue.InternalCopyTo(nativeBuffer);
                }
            }
            else
            {
                value.InternalCopyTo(nativeBuffer);
            }
        }

        public static T FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            return FromNative(nativeBuffer);
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, T value)
        {
            ToNative(nativeBuffer, value);
        }

        public static void CopyFromNative(IntPtr nativeBuffer, ref T value)
        {
            if (value == null)
            {
                value = new T();
                value.Initialize();
            }
            value.InternalCopyFrom(nativeBuffer);
        }

        public static void CopyFromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, ref T value)
        {
            value.InternalCopyFrom(nativeBuffer + (StructDefault<T>.Size * arrayIndex));
        }
    }
}

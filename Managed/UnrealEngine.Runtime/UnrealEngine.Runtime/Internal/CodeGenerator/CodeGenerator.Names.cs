using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public partial class CodeGenerator
    {
        // Should probably be just be using nameof directly but for now this will do (also partly due to wanting
        // support for an old version of VS/C# which don't support nameof)
        public static class Names
        {
            public static string FMemory
            {
                get { return NameOf(typeof(FMemory)); }
            }

            public static string FMemory_Memzero
            {
                get { return FMemory + ".Memzero"; }
            }

            public static string UObject_Address
            {
                get { return "Address"; }
            }

            public static string UObject_CheckDestroyed
            {
                get { return "CheckDestroyed"; }
            }

            public static string GCHelper
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.GCHelper)); }
            }

			public static string GCHelper_Find
            {
				get { return GCHelper + ".Find"; }
            }

            public static string NativeReflection
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.NativeReflection)); }
            }

            public static string NativeReflection_GetStruct
            {
                get { return NativeReflection + ".GetStruct"; }
            }

            public static string NativeReflection_GetClass
            {
                get { return NativeReflection + ".GetClass"; }
            }

            public static string NativeReflection_GetStructSize
            {
                get { return NativeReflection + ".GetStructSize"; }
            }

            //public static string NativeReflection_GetPropertyRef
            //{
            //    get { return NativeReflection + ".GetPropertyRef"; }
            //}

            //public static string NativeReflection_GetPropertyOffset
            //{
            //    get { return NativeReflection + ".GetPropertyOffset"; }
            //}

            public static string NativeReflection_GetFunctionFromInstance
            {
                get { return NativeReflection + ".GetFunctionFromInstance"; }
            }

            public static string NativeReflection_GetFunction
            {
                get { return NativeReflection + ".GetFunction"; }
            }

            public static string NativeReflection_GetFunctionParamsSize
            {
                get { return NativeReflection + ".GetFunctionParamsSize"; }
            }

            public static string NativeReflection_InvokeStaticFunction
            {
                get { return NativeReflection + ".InvokeStaticFunction"; }
            }

            public static string NativeReflection_InvokeFunction
            {
                get { return NativeReflection + ".InvokeFunction"; }
            }

            public static string NativeReflection_InvokeFunction_InitAll
            {
                get { return NativeReflection + ".InvokeFunction_InitAll"; }
            }

            public static string NativeReflection_InvokeFunction_DestroyAll
            {
                get { return NativeReflection + ".InvokeFunction_DestroyAll"; }
            }

            public static string NativeReflection_InitializeValue_InContainer
            {
                get { return NativeReflection + ".InitializeValue_InContainer"; }
            }

            public static string NativeReflection_DestroyValue_InContainer
            {
                get { return NativeReflection + ".DestroyValue_InContainer"; }
            }

            public static string NativeReflection_ValidateBlittableStructSize
            {
                get { return NativeReflection + ".ValidateBlittableStructSize"; }
            }

            public static string NativeReflection_ValidatePropertyClass
            {
                get { return NativeReflection + ".ValidatePropertyClass"; }
            }

            public static string NativeReflection_LogFunctionIsValid
            {
                get { return NativeReflection + ".LogFunctionIsValid"; }
            }

            public static string NativeReflection_LogStructIsValid
            {
                get { return NativeReflection + ".LogStructIsValid"; }
            }

            public static string NativeReflection_LogInvalidPropertyAccessed
            {
                get { return NativeReflection + ".LogInvalidPropertyAccessed"; }
            }

            public static string NativeReflection_LogInvalidFunctionAccessed
            {
                get { return NativeReflection + ".LogInvalidFunctionAccessed"; }
            }

            public static string NativeReflection_LogInvalidStructAccessed
            {
                get { return NativeReflection + ".LogInvalidStructAccessed"; }
            }

            public static string NativeReflectionCached
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.NativeReflectionCached)); }
            }

            public static string NativeReflectionCached_GetPropertyRef
            {
                get { return NativeReflectionCached + ".GetPropertyRef"; }
            }

            public static string NativeReflectionCached_GetPropertyOffset
            {
                get { return NativeReflectionCached + ".GetPropertyOffset"; }
            }

            public static string NativeReflectionCached_GetFunction
            {
                get { return NativeReflectionCached + ".GetFunction"; }
            }

            public static string NativeReflectionCached_ValidatePropertyClass
            {
                get { return NativeReflectionCached + ".ValidatePropertyClass"; }
            }

            public static string UnrealTypes
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.UnrealTypes)); }
            }

            public static string UnrealTypes_CanLazyLoadNativeType
            {
                get { return UnrealTypes + ".CanLazyLoadNativeType"; }
            }

            public static string UnrealTypes_OnCCtorCalled
            {
                get { return UnrealTypes + ".OnCCtorCalled"; }
            }

            public static string Classes
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.Classes)); }
            }

            public static string UFieldAddress
            {
                get { return NameOf<UnrealEngine.Runtime.UFieldAddress>(); }
            }

            public static string UFieldAddress_Address
            {
                get { return "Address"; }
            }

            public static string EPropertyType
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.EPropertyFlags)); }
            }

            public static string FName
            {
                get { return NameOf<UnrealEngine.Runtime.FName>(); }
            }

            public static string FText
            {
                get
                {
                    return "FText";
                    //return NameOf<UnrealEngine.Runtime.FText>(); }
                }
            }

            public static string FObjectInitializer
            {
                get { return NameOf<UnrealEngine.Runtime.FObjectInitializer>(); }
            }

            public static string FDelegate
            {
                get { return NameOf<UnrealEngine.Runtime.FDelegate<object>>(); }
            }

            public static string FMulticastDelegate
            {
                get { return NameOf<UnrealEngine.Runtime.FMulticastDelegate<object>>(); }
            }

            public static string FDelegateBase_GetInvoker
            {
                get { return "GetInvoker"; }
            }

            public static string FDelegateBase_ProcessDelegate
            {
                get { return "ProcessDelegate"; }
            }

            public static string FDelegateBase_SetAddress
			{
                get { return "SetAddress"; }
            }

            public static string TSoftClass
            {
                get { return NameOf<UnrealEngine.Runtime.TSoftClass<UObject>>(); }
            }

            public static string TSoftObject
            {
                get { return NameOf<UnrealEngine.Runtime.TSoftObject<UObject>>(); }
            }

            public static string TWeakObject
            {
                get { return NameOf<UnrealEngine.Runtime.TWeakObject<UObject>>(); }
            }

            public static string TLazyObject
            {
                get { return NameOf<UnrealEngine.Runtime.TLazyObject<UObject>>(); }
            }

            public static string TSubclassOf
            {
                get { return NameOf<UnrealEngine.Runtime.TSubclassOf<UObject>>(); }
            }

            public static string TSubclassOfInterface
            {
                get { return NameOf<UnrealEngine.Runtime.TSubclassOfInterface<IInterface>>(); }
            }            

            public static string TArrayReadOnly
            {
                get { return NameOf<UnrealEngine.Runtime.TArrayReadOnly<object>>(); }
            }

            public static string TArrayReadWrite
            {
                get { return NameOf<UnrealEngine.Runtime.TArrayReadWrite<object>>(); }
            }

            public static string TSetReadOnly
            {
                get { return NameOf<UnrealEngine.Runtime.TSetReadOnly<object>>(); }
            }

            public static string TSetReadWrite
            {
                get { return NameOf<UnrealEngine.Runtime.TSetReadWrite<object>>(); }
            }

            public static string TMapReadOnly
            {
                get { return NameOf<UnrealEngine.Runtime.TMapReadOnly<object, object>>(); }
            }

            public static string TMapReadWrite
            {
                get { return NameOf<UnrealEngine.Runtime.TMapReadWrite<object, object>>(); }
            }

            public static string EnumMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.EnumMarshaler<>)); }
            }

            public static string BlittableTypeMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.BlittableTypeMarshaler<>)); }
            }

            public static string BoolMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.BoolMarshaler)); }
            }

            public static string FStringMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.FStringMarshaler)); }
            }

            public static string FStringMarshaler_DefaultString
            {
				get { return FStringMarshaler + ".DefaultString"; }
            }

            public static string TArrayReadWriteMarshaler
            {
                get { return NameOf<UnrealEngine.Runtime.TArrayReadWriteMarshaler<object>>(); }
            }

            public static string TArrayCopyMarshaler
            {
                get { return NameOf<UnrealEngine.Runtime.TArrayCopyMarshaler<object>>(); }
            }

            public static string TArrayReadOnlyMarshaler
            {
                get { return NameOf<UnrealEngine.Runtime.TArrayReadOnlyMarshaler<object>>(); }
            }

            public static string TSetReadWriteMarshaler
            {
                get { return NameOf<UnrealEngine.Runtime.TSetReadWriteMarshaler<object>>(); }
            }

            public static string TSetCopyMarshaler
            {
                get { return NameOf<UnrealEngine.Runtime.TSetCopyMarshaler<object>>(); }
            }

            public static string TSetReadOnlyMarshaler
            {
                get { return NameOf<UnrealEngine.Runtime.TSetReadOnlyMarshaler<object>>(); }
            }

            public static string TMapReadWriteMarshaler
            {
                get { return NameOf<UnrealEngine.Runtime.TMapReadWriteMarshaler<object, object>>(); }
            }

            public static string TMapCopyMarshaler
            {
                get { return NameOf<UnrealEngine.Runtime.TMapCopyMarshaler<object, object>>(); }
            }

            public static string TMapReadOnlyMarshaler
            {
                get { return NameOf<UnrealEngine.Runtime.TMapReadOnlyMarshaler<object, object>>(); }
            }

            public static string InterfaceMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.InterfaceMarshaler<>)); }
            }

            public static string UObjectMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.UObjectMarshaler<>)); }
            }

            public static string TSubclassOfInterfaceMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.TSubclassOfInterfaceMarshaler<>)); }
            }

            public static string TSubclassOfMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.TSubclassOfMarshaler<>)); }
            }

            public static string TSoftClassMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.TSoftClassMarshaler<>)); }
            }

            public static string TSoftObjectMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.TSoftObjectMarshaler<>)); }
            }

            public static string TWeakObjectMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.TWeakObjectMarshaler<>)); }
            }

            public static string TLazyObjectMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.TLazyObjectMarshaler<>)); }
            }

            public static string TFixedSizeArrayMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.TFixedSizeArrayMarshaler<>)); }
            }

            public static string StructAsClassMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.StructAsClassMarshaler<>)); }
            }

            public static string FDelegateMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.FDelegateMarshaler<>)); }
            }

            public static string FMulticastDelegateMarshaler
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.FMulticastDelegateMarshaler<>)); }
            }

            public static string FSoftObjectPathMarshaler
            {
                get { return NameOf<FSoftObjectPathMarshaler>(); }
            }

            public static string CachedMarshalingDelegates
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.CachedMarshalingDelegates<,>)); }
            }

            public static string TFixedSizeArray
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.TFixedSizeArray<>)); }
            }

            public static string TFixedSizeArrayReadOnly
            {
                get { return NameOf(typeof(UnrealEngine.Runtime.TFixedSizeArrayReadOnly<>)); }
            }

            public static string IInterface
            {
				get { return NameOf<IInterface>(); }
            }

            public static string IInterfaceImpl
            {
                get { return NameOf<IInterfaceImpl>(); }
            }

            public static string UFunctionAttributeShort
            {
                get { return NameOfAttribute<UFunctionAttribute>(); }
            }

            public static string StructAsClass
            {
                get { return NameOf<StructAsClass>(); }
            }

            public static string StructAsClass_Initialize
            {
                get { return "Initialize"; }
            }

            public static string StructAsClass_CopyFrom
            {
                get { return "CopyFrom"; }
            }

            public static string EFunctionFlags
            {
                get { return NameOf<EFunctionFlags>(); }
            }

            private static string NameOf(Type type)
            {
                string name;
                if (!cachedNameOf.TryGetValue(type, out name))
                {
                    name = type.Name;
                    int genericsIndex = name.IndexOf('`');
                    if (genericsIndex >= 0)
                    {
                        name = name.Substring(0, genericsIndex);
                    }
                    cachedNameOf.Add(type, name);
                }
                return name;
            }

            private static string NameOfAttribute(Type type)
            {
                System.Diagnostics.Debug.Assert(type.IsSubclassOf(typeof(Attribute)));

                string name;
                if (!cachedNameOf.TryGetValue(type, out name))
                {
                    name = type.Name;
                    int genericsIndex = name.IndexOf('`');
                    if (genericsIndex >= 0)
                    {
                        name = name.Substring(0, genericsIndex);
                    }
                    name = name.RemoveFromEnd("Attribute");
                    cachedNameOf.Add(type, name);
                }
                return name;
            }

            private static string NameOf<T>()
            {
                return CachedNameOf<T>.Name;
            }

            private static string NameOfAttribute<T>()
            {
                return CachedNameOfAttribute<T>.Name;
            }

            private static Dictionary<Type, string> cachedNameOf = new Dictionary<Type, string>();
            private static Dictionary<Type, string> cachedNameOfAttribute = new Dictionary<Type, string>();

            private static class CachedNameOf<T>
            {
                public static readonly string Name = NameOf(typeof(T));
            }

            private static class CachedNameOfAttribute<T>
            {
                public static readonly string Name = NameOfAttribute(typeof(T));
            }
        }
    }
}

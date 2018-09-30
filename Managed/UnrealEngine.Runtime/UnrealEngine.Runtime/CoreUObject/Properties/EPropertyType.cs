using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // This doesn't match a native enum but it's somewhat similar to a couple of other property enums
    // EPropertyClass (used by code gen) - Engine\Source\Runtime\CoreUObject\Public\UObject\UObjectGlobals.h
    // EPropertyType (used for VM / UHT) - Engine\Source\Runtime\CoreUObject\Public\UObject\Stack.h

    public enum EPropertyType
    {
        Unknown,

        Bool,

        Int8,
        Int16,
        Int,
        Int64,

        Byte,
        UInt16,
        UInt32,
        UInt64,

        Double,
        Float,

        Enum,

        Interface,
        Struct,
        Class,// TSubclassOf<>

        Object,
        LazyObject,
        WeakObject,

        SoftClass,
        SoftObject,

        Delegate,
        MulticastDelegate,

        Array,
        Map,
        Set,
        
        Str,
        Name,
        Text,

        //Numeric,//Abstract
        //ObjectBase,//Abstract

        /// <summary>
        /// Represents an array defined by TFixedSizeArray
        /// 
        /// NOTE: This isn't a real property type (this is for internally keeping track of fixed size array types)
        /// </summary>
        InternalNativeFixedSizeArray,
        /// <summary>
        /// Represents an array defined by T[].
        /// 
        /// NOTE: This isn't a real property type (this is for internally keeping track of fixed size array types)
        /// </summary>
        InternalManagedFixedSizeArray
    }
}

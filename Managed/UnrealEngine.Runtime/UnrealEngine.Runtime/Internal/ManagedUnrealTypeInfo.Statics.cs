using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnrealEngine.Runtime.ManagedUnrealTypeInfoExceptions;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public partial class ManagedUnrealTypeInfo
    {
        /// <summary>
        /// Checks if blueprint supports the given property
        /// </summary>
        /// <param name="propertyInfo">The property to check</param>
        /// <param name="memberVariable">States that the given property is defined inside a struct / class 
        /// (should be false for inner types of a collection / function parameters)</param>
        /// <returns>True if blueprint supports the given property type</returns>
        public static bool DoesBlueprintSupportType(ManagedUnrealPropertyInfo propertyInfo, bool memberVariable)
        {
            if (propertyInfo.IsCollection)
            {
                foreach (ManagedUnrealTypeInfoReference typeRef in propertyInfo.GenericArgs)
                {
                    if (!DoesBlueprintSupportType(typeRef.TypeCode, false, true))
                    {
                        return false;
                    }
                }

                // Additional checks in FHeaderParser::GetVarType
                // else if ( VarType.Matches(TEXT("TMap")) )
                // else if ( VarType.Matches(TEXT("TSet")) )
                switch (propertyInfo.Type.TypeCode)
                {
                    case EPropertyType.Set:
                    case EPropertyType.Map:
                        switch (propertyInfo.GenericArgs[0].TypeCode)
                        {
                            // "UINTERFACEs are not currently supported as element types."
                            // "FText is not currently supported as an element type."
                            case EPropertyType.Interface:
                            case EPropertyType.Text:
                                return false;
                        }
                        break;
                }

                return true;
            }
            else
            {
                return DoesBlueprintSupportType(propertyInfo.Type.TypeCode, memberVariable, false);
            }
        }

        /// <summary>
        /// Checks if blueprint supports the given type
        /// </summary>
        /// <param name="typeCode">The type to check</param>
        /// <param name="memberVariable">States that the given property is defined inside a struct / class 
        /// (should be false for inner types of a collection / function parameters)</param>
        /// <returns>True if blueprint supports the given type</returns>
        /// <param name="collectionArg">The given property is an argument to a collection</param>
        public static bool DoesBlueprintSupportType(EPropertyType typeCode, bool memberVariable, bool collectionArg)
        {
            // See IsPropertySupportedByBlueprint() (Engine\Source\Programs\UnrealHeaderTool\Private\HeaderParser.cpp)
            switch (typeCode)
            {
                case EPropertyType.Int8:
                case EPropertyType.Int16:
                case EPropertyType.UInt16:
                case EPropertyType.UInt32:
                case EPropertyType.Int64:
                case EPropertyType.UInt64:
                case EPropertyType.Double:
                case EPropertyType.LazyObject:
                case EPropertyType.InternalManagedFixedSizeArray:
                case EPropertyType.InternalNativeFixedSizeArray:
                    return false;
                case EPropertyType.WeakObject:
                case EPropertyType.MulticastDelegate:
                    // Only supported as members of a struct / class (not supported as collection inners / function params)
                    return memberVariable;
                case EPropertyType.Array:
                case EPropertyType.Set:
                case EPropertyType.Map:
                    // "Nested containers are not supported."
                    return !collectionArg;
                default:
                    return true;
            }
        }

        public static bool IsExportableType(Type type)
        {            
            if (type.IsGenericType)
            {
                return false;
            }

            // NOTE: We aren't doing any with handling types which are exposed to unreal which reference
            // types which aren't exposed to unreal. We would need some error logging for that scenario.

            EPropertyType typeCode = GetTypeCode(type);
            switch (typeCode)
            {
                case EPropertyType.Object:
                    return IsExportableType(type, ManagedUnrealVisibility.ClassRequirement, typeof(UClassAttribute));

                case EPropertyType.Enum:
                    return IsExportableType(type, ManagedUnrealVisibility.EnumRequirement, typeof(UEnumAttribute));

                case EPropertyType.Interface:
                    return IsExportableType(type, ManagedUnrealVisibility.InterfaceRequirement, typeof(UInterfaceAttribute));

                case EPropertyType.Struct:
                    {
                        if (type.IsSubclassOf(typeof(StructAsClass)))
                        {
                            throw new NotImplementedException("StructAsClass support is currently disabled due to being incomplete. " +
                                "'" + type.FullName + "'");
                            //return IsExportableType(type, ManagedUnrealVisibility.StructRequirement, typeof(UStructAttribute));
                        }
                        else
                        {
                            return IsExportableType(type, ManagedUnrealVisibility.StructRequirement, typeof(UStructAttribute));
                        }
                    }
                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    return IsExportableType(type, ManagedUnrealVisibility.DelegateRequirement, typeof(UDelegateAttribute));

            }
            return false;
        }

        private static bool IsExportableType(Type type, ManagedUnrealVisibility.Requirement requirement, Type attribute)
        {
            switch (requirement)
            {
                case ManagedUnrealVisibility.Requirement.MainAttribute:
                    if (type.GetCustomAttribute(attribute, false) == null)
                    {
                        return false;
                    }
                    break;
                case ManagedUnrealVisibility.Requirement.AnyAttribute:
                    if (!type.HasCustomAttribute<ManagedUnrealAttributeBase>(false) &&
                        !type.HasCustomAttribute<UMetaAttribute>(false))
                    {
                        return false;
                    }
                    break;
            }

            if (UnrealTypes.IsNativeUnrealType(type))
            {
                // Skip types defined by generated code
                return false;
            }

            if (type == typeof(FDelegate<>) || type == typeof(FMulticastDelegate<>))
            {
                return false;
            }

            return true;
        }

        public static EPropertyType GetTypeCode(Type type)
        {
            if (type.IsArray)
            {
                return EPropertyType.InternalManagedFixedSizeArray;
            }

            if (type.IsByRef && type.HasElementType)
            {
                type = type.GetElementType();
            }

            if (type.IsEnum)
            {
                return EPropertyType.Enum;
            }

            switch (Type.GetTypeCode(type))
            {
                case System.TypeCode.Boolean: return EPropertyType.Bool;

                case System.TypeCode.SByte: return EPropertyType.Int8;
                case System.TypeCode.Int16: return EPropertyType.Int16;
                case System.TypeCode.Int32: return EPropertyType.Int;
                case System.TypeCode.Int64: return EPropertyType.Int64;

                case System.TypeCode.Byte: return EPropertyType.Byte;
                case System.TypeCode.UInt16: return EPropertyType.UInt16;
                case System.TypeCode.UInt32: return EPropertyType.UInt32;
                case System.TypeCode.UInt64: return EPropertyType.UInt64;

                case System.TypeCode.Double: return EPropertyType.Double;
                case System.TypeCode.Single: return EPropertyType.Float;

                case System.TypeCode.String: return EPropertyType.Str;
            }
            if (type.IsSameOrSubclassOf(typeof(UObject)))
            {
                return EPropertyType.Object;
            }
            else if (type.IsInterface)
            {
                if (type.IsSameOrSubclassOfGeneric(typeof(ISet<>)))
                {
                    return EPropertyType.Set;
                }
                if (type.IsSameOrSubclassOfGeneric(typeof(IDictionary<,>)) ||
                    type.IsSameOrSubclassOfGeneric(typeof(IReadOnlyDictionary<,>)))
                {
                    return EPropertyType.Map;
                }
                if (type.IsSameOrSubclassOfGeneric(typeof(IList<>)) ||
                    type.IsSameOrSubclassOfGeneric(typeof(IReadOnlyList<>)))
                {
                    return EPropertyType.Array;
                }

                // Interfaces exposed to Unreal must inherit from IInterface
                if (typeof(IInterface).IsAssignableFrom(type) && type != typeof(IInterface))
                {                    
                    return EPropertyType.Interface;
                }

                return EPropertyType.Unknown;
            }
            else if (type == typeof(FName))
            {
                return EPropertyType.Name;
            }
            else if (type == typeof(FText))
            {
                return EPropertyType.Text;
            }

            if (type.IsSubclassOf(typeof(StructAsClass)) && !type.IsAbstract)
            {
                return EPropertyType.Struct;
            }

            if (type.IsGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();

                if (genericType.IsSameOrSubclassOf(typeof(TLazyObject<>)))
                {
                    return EPropertyType.LazyObject;
                }
                if (genericType.IsSameOrSubclassOf(typeof(TWeakObject<>)))
                {
                    return EPropertyType.WeakObject;
                }

                if (genericType.IsSameOrSubclassOf(typeof(TSubclassOf<>)) ||
                    genericType.IsSameOrSubclassOf(typeof(TSubclassOfInterface<>)))
                {
                    return EPropertyType.Class;
                }

                if (genericType.IsSameOrSubclassOf(typeof(TSoftClass<>)))
                {
                    return EPropertyType.SoftClass;
                }
                if (genericType.IsSameOrSubclassOf(typeof(TSoftObject<>)))
                {
                    return EPropertyType.SoftObject;
                }

                if (type.IsSameOrSubclassOfGeneric(typeof(TFixedSizeArrayBase<>)))
                {
                    return EPropertyType.InternalNativeFixedSizeArray;
                }

                Type[] interfaces = genericType.GetInterfaces();
                for (int i = 0; i < interfaces.Length; i++)
                {
                    if (interfaces[i].IsGenericType)
                    {
                        Type genericInterfaceType = interfaces[i].GetGenericTypeDefinition();

                        if (genericInterfaceType.IsSameOrSubclassOf(typeof(ISet<>)))
                        {
                            return EPropertyType.Set;
                        }
                        if (genericInterfaceType.IsSameOrSubclassOf(typeof(IDictionary<,>)) ||
                            genericInterfaceType.IsSameOrSubclassOf(typeof(IReadOnlyDictionary<,>)))
                        {
                            return EPropertyType.Map;
                        }
                        if (genericInterfaceType.IsSameOrSubclassOf(typeof(IList<>)) ||
                            genericInterfaceType.IsSameOrSubclassOf(typeof(IReadOnlyList<>)))
                        {
                            return EPropertyType.Array;
                        }
                    }
                }
            }

            if (type.IsSameOrSubclassOfGeneric(typeof(FDelegate<>)))
            {
                return EPropertyType.Delegate;
            }
            if (type.IsSameOrSubclassOfGeneric(typeof(FMulticastDelegate<>)))
            {
                return EPropertyType.MulticastDelegate;
            }

            if (type.IsValueType)
            {
                return EPropertyType.Struct;
            }

            return EPropertyType.Unknown;
        }

        public static bool HasGetTypeHash(EPropertyType typeCode)
        {
            switch (typeCode)
            {
                // C++ allows WeakObjectPtr but creates it creates a runtime error due to not having
                // an implementation of UProperty::GetValueTypeHashInternal
                case EPropertyType.WeakObject:
                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    return false;
                default:
                    // NOTE: Structs may or may not have a struct hash, let them all through for later checks
                    return true;
            }
        }

        public static string GetTypeNameWithoutPrefix(Type type, EPropertyType typeCode)
        {
            string typeName = type.Name;

            char[] prefixChars = null;
            switch (typeCode)
            {
                case EPropertyType.Object:
                    prefixChars = new char[] { 'A', 'U' };
                    break;
                case EPropertyType.Struct:
                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    prefixChars = new char[] { 'F' };
                    break;
                case EPropertyType.Enum:
                    prefixChars = new char[] { 'E' };
                    break;
                case EPropertyType.Interface:
                    prefixChars = new char[] { 'I' };
                    break;
            }

            if (prefixChars != null)
            {
                for (int i = 0; i < prefixChars.Length; i++)
                {
                    if (typeName.Length > 2 && typeName[0] == prefixChars[i] && char.IsUpper(typeName[1]) && char.IsLower(typeName[2]))
                    {
                        typeName = typeName.Substring(1);
                        break;
                    }
                }
            }

            return typeName;
        }

        public static bool RequiresNativePropertyField(ManagedUnrealPropertyInfo propertyInfo, bool lazyFunctionParamInit)
        {
            switch (propertyInfo.Type.TypeCode)
            {
                case EPropertyType.InternalNativeFixedSizeArray:
                case EPropertyType.InternalManagedFixedSizeArray:
                case EPropertyType.Bool:
                case EPropertyType.Enum:
                case EPropertyType.Array:
                case EPropertyType.Set:
                case EPropertyType.Map:
                    return true;
            }

            if (!lazyFunctionParamInit &&
                (propertyInfo.IsFunctionParam || propertyInfo.IsFunctionReturnValue))
            {
                // We need the property address to call InitializeValue / DestroyValue
                if (PropertyRequiresInit(propertyInfo) || PropertyRequiresDestroy(propertyInfo))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the target type marshaler requires a UProperty in the FromNative / ToNative marshaling methods
        /// </summary>
        public static bool MarshalerRequiresNativePropertyField(EPropertyType typeCode)
        {
            switch (typeCode)
            {
                case EPropertyType.InternalManagedFixedSizeArray:
                case EPropertyType.InternalNativeFixedSizeArray:
                case EPropertyType.Bool:
                case EPropertyType.Enum:
                    return true;
            }
            return false;
        }

        public static Type MakeGenericTypeWithPropertyArgs(Type type, ManagedUnrealPropertyInfo propertyInfo)
        {
            List<Type> genericArgs = new List<Type>();
            foreach (ManagedUnrealTypeInfoReference arg in propertyInfo.GenericArgs)
            {
                genericArgs.Add(GetTypeFromTypeInfo(arg));
            }
            return type.MakeGenericType(genericArgs.ToArray());
        }

        public static Type GetTypeFromPropertyInfo(ManagedUnrealPropertyInfo propertyInfo)
        {
            Type type = null;
            if (propertyInfo.IsFixedSizeArray)
            {
                // This will return the underlying type of the array rather than an array type
                type = GetTypeFromTypeInfo(propertyInfo.GenericArgs[0]);
            }
            else
            {
                type = GetTypeFromTypeInfo(propertyInfo.Type);
            }
            if (propertyInfo.IsCollection)
            {
                return MakeGenericTypeWithPropertyArgs(type, propertyInfo);
            }
            return type;
        }

        public static Type GetTypeFromTypeInfo(ManagedUnrealTypeInfo typeInfo)
        {
            return GetTypeFromTypeInfo(typeInfo.TypeCode, typeInfo.Path);
        }

        public static Type GetTypeFromTypeInfo(ManagedUnrealTypeInfoReference typeInfo)
        {
            return GetTypeFromTypeInfo(typeInfo.TypeCode, typeInfo.Path);
        }

        public static Type GetTypeFromTypeInfo(EPropertyType typeCode, string typePath)
        {
            switch (typeCode)
            {
                case EPropertyType.Bool: return typeof(bool);
                case EPropertyType.Int8: return typeof(sbyte);
                case EPropertyType.Int16: return typeof(short);
                case EPropertyType.Int: return typeof(int);
                case EPropertyType.Int64: return typeof(long);
                case EPropertyType.Byte: return typeof(byte);
                case EPropertyType.UInt16: return typeof(ushort);
                case EPropertyType.UInt32: return typeof(uint);
                case EPropertyType.UInt64: return typeof(ulong);
                case EPropertyType.Float: return typeof(float);
                case EPropertyType.Double: return typeof(double);
                case EPropertyType.Name: return typeof(FName);
                case EPropertyType.Text: return typeof(FText);
                case EPropertyType.Str: return typeof(string);

                case EPropertyType.Array: return typeof(IList<>);
                case EPropertyType.Map: return typeof(IDictionary<,>);
                case EPropertyType.Set: return typeof(ISet<>);

                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    {
                        Type delegateType = FindTypeByPath(typePath);
                        if (delegateType != null)
                        {
                            return delegateType;
                        }
                        else
                        {
                            throw new UnrealTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.Enum:
                    {
                        Type enumType = FindTypeByPath(typePath);
                        if (enumType != null)
                        {
                            return enumType;
                        }
                        else
                        {
                            throw new UnrealTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.Interface:
                    {
                        Type interfaceType = FindTypeByPath(typePath);
                        if (interfaceType != null)
                        {
                            return interfaceType;
                        }
                        else
                        {
                            throw new UnrealTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.Struct:
                    {
                        Type structType = FindTypeByPath(typePath);
                        if (structType != null)
                        {
                            return structType;
                        }
                        else
                        {
                            throw new UnrealTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.Class:
                    {                        
                        Type objectType = FindTypeByPath(typePath);
                        if (objectType != null)
                        {
                            Type subclassOfType = null;
                            if (objectType.IsInterface)
                            {
                                subclassOfType = typeof(TSubclassOfInterface<>);
                            }
                            else
                            {
                                subclassOfType = typeof(TSubclassOf<>);
                            }

                            return subclassOfType.MakeGenericType(objectType);
                        }
                        else
                        {
                            throw new UnrealTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.Object:
                    {
                        Type objectType = FindTypeByPath(typePath);
                        if (objectType != null)
                        {
                            return objectType;
                        }
                        else
                        {
                            throw new UnrealTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.LazyObject:
                    {
                        Type lazyObjectType = typeof(TLazyObject<>);
                        Type objectType = FindTypeByPath(typePath);
                        if (objectType != null)
                        {
                            return lazyObjectType.MakeGenericType(objectType);
                        }
                        else
                        {
                            throw new UnrealTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.WeakObject:
                    {
                        Type weakObjectType = typeof(TWeakObject<>);
                        Type objectType = FindTypeByPath(typePath);
                        if (objectType != null)
                        {
                            return weakObjectType.MakeGenericType(objectType);
                        }
                        else
                        {
                            throw new UnrealTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.SoftClass:
                    {
                        Type softClassType = typeof(TSoftClass<>);
                        Type objectType = FindTypeByPath(typePath);
                        if (objectType != null)
                        {
                            return softClassType.MakeGenericType(objectType);
                        }
                        else
                        {
                            throw new UnrealTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.SoftObject:
                    {
                        Type softObjectType = typeof(TSoftObject<>);
                        Type objectType = FindTypeByPath(typePath);
                        if (objectType != null)
                        {
                            return softObjectType.MakeGenericType(objectType);
                        }
                        else
                        {
                            throw new UnrealTypeNotFoundException(typeCode, typePath);
                        }
                    }

                default:
                    throw new NotImplementedException("Unhandled type " + typeCode);
            }
        }

        public static Type FindTypeByPath(string typePath)
        {
            Type type;
            ManagedUnrealModuleInfo.AllTypesByPath.TryGetValue(typePath, out type);
            return type;
        }

        public static ManagedUnrealTypeInfo FindTypeInfoByPath(string typePath)
        {
            ManagedUnrealTypeInfo typeInfo;
            ManagedUnrealModuleInfo.AllTypeInfosByPath.TryGetValue(typePath, out typeInfo);
            return typeInfo;
        }

        public static ManagedUnrealTypeInfo FindTypeInfoFromType(Type type)
        {
            ManagedUnrealModuleInfo module;
            if (ManagedUnrealModuleInfo.ModulesByType.TryGetValue(type, out module))
            {
                ManagedUnrealTypeInfo typeInfo;
                module.TypeInfosByType.TryGetValue(type, out typeInfo);
                return typeInfo;
            }
            return null;
        }

        public static Type GetDowngradedCollectionType(ManagedUnrealPropertyInfo propertyInfo, ManagedUnrealMarshalerType marshalerType)
        {
            switch (propertyInfo.Type.TypeCode)
            {
                case EPropertyType.Array:
                    return MakeGenericTypeWithPropertyArgs(typeof(IList<>), propertyInfo);

                case EPropertyType.Map:
                    return MakeGenericTypeWithPropertyArgs(typeof(IDictionary<,>), propertyInfo);

                case EPropertyType.Set:
                    return MakeGenericTypeWithPropertyArgs(typeof(ISet<>), propertyInfo);
            }
            return null;
        }

        public static bool IsCollectionType(EPropertyType typeCode)
        {
            switch (typeCode)
            {
                case EPropertyType.Array:
                case EPropertyType.Map:
                case EPropertyType.Set:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsDelegateType(EPropertyType typeCode)
        {
            switch (typeCode)
            {
                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    return true;
                default:
                    return false;
            }
        }

        public static Type GetCollectionType(ManagedUnrealPropertyInfo propertyInfo, ManagedUnrealMarshalerType marshalerType)
        {
            return GetTypeFromMarshalerInfo(new ManagedUnrealMarshalerInfo(propertyInfo, marshalerType));
        }

        public static Type GetCollectionType(EPropertyType typeCode, ManagedUnrealMarshalerType marshalerType)
        {
            switch (typeCode)
            {
                case EPropertyType.Array:
                    switch (marshalerType)
                    {
                        case ManagedUnrealMarshalerType.Default: return typeof(TArrayReadWrite<>);
                        case ManagedUnrealMarshalerType.ReadOnly: return typeof(TArrayReadOnly<>);
                        case ManagedUnrealMarshalerType.Copy: return typeof(IList<>);
                    }
                    break;

                case EPropertyType.Map:
                    switch (marshalerType)
                    {
                        case ManagedUnrealMarshalerType.Default: return typeof(TMapReadWrite<,>);
                        case ManagedUnrealMarshalerType.ReadOnly: return typeof(TMapReadOnly<,>);
                        case ManagedUnrealMarshalerType.Copy: return typeof(IDictionary<,>);
                    }
                    break;

                case EPropertyType.Set:
                    switch (marshalerType)
                    {
                        case ManagedUnrealMarshalerType.Default: return typeof(TSetReadWrite<>);
                        case ManagedUnrealMarshalerType.ReadOnly: return typeof(TSetReadOnly<>);
                        case ManagedUnrealMarshalerType.Copy: return typeof(ISet<>);
                    }
                    break;
            }
            return null;
        }

        public static Type GetMarshalerType(ManagedUnrealMarshalerType marshalerType, ManagedUnrealTypeInfo typeInfo)
        {
            return GetMarshalerType(marshalerType, typeInfo.TypeCode, typeInfo.Path);
        }

        public static Type GetMarshalerType(ManagedUnrealMarshalerType marshalerType, ManagedUnrealTypeInfoReference typeInfo)
        {
            return GetMarshalerType(marshalerType, typeInfo.TypeCode, typeInfo.Path);
        }

        public static Type GetMarshalerType(ManagedUnrealMarshalerType marshalerType, ManagedUnrealPropertyInfo propertyInfo)
        {
            ManagedUnrealTypeInfoReference arg1 = propertyInfo.GenericArgs.Count >= 1 ? propertyInfo.GenericArgs[0] : null;
            ManagedUnrealTypeInfoReference arg2 = propertyInfo.GenericArgs.Count >= 2 ? propertyInfo.GenericArgs[1] : null;

            return GetMarshalerType(marshalerType,
                propertyInfo.Type.TypeCode, propertyInfo.Type.Path,
                arg1 != null ? arg1.TypeCode : EPropertyType.Unknown, arg1 != null ? arg1.Path : null,
                arg2 != null ? arg2.TypeCode : EPropertyType.Unknown, arg2 != null ? arg2.Path : null);
        }

        public static Type GetMarshalerType(ManagedUnrealMarshalerType marshalerType, 
            EPropertyType typeCode, string typePath,
            EPropertyType arg1TypeCode = EPropertyType.Unknown, string arg1TypePath = null,
            EPropertyType arg2TypeCode = EPropertyType.Unknown, string arg2TypePath = null)
        {
            ManagedUnrealMarshalerInfo marshalerInfo = new ManagedUnrealMarshalerInfo(
                typeCode, typePath, arg1TypeCode, arg1TypePath, arg2TypeCode, arg2TypePath, marshalerType);
            return GetTypeFromMarshalerInfo(marshalerInfo);
        }

        public static Type GetTypeFromMarshalerInfo(ManagedUnrealMarshalerInfo marshalerInfo)
        {
            switch (marshalerInfo.Type)
            {
                case EPropertyType.InternalNativeFixedSizeArray:
                    {
                        Type arrayMarshalerType = null;
                        switch (marshalerInfo.MarshalerType)
                        {
                            case ManagedUnrealMarshalerType.ReadOnly:
                                arrayMarshalerType = typeof(TFixedSizeArrayReadOnly<>);
                                break;
                            default:
                                arrayMarshalerType = typeof(TFixedSizeArray<>);
                                break;
                        }
                        Type elementType = GetTypeFromTypeInfo(marshalerInfo.Arg1Type, marshalerInfo.Arg1Path);
                        return arrayMarshalerType.MakeGenericType(elementType);
                    }
                case EPropertyType.InternalManagedFixedSizeArray:
                    {
                        Type elementType = GetTypeFromTypeInfo(marshalerInfo.Arg1Type, marshalerInfo.Arg1Path);
                        return typeof(TFixedSizeArrayMarshaler<>).MakeGenericType(elementType);
                    }

                case EPropertyType.Array:
                    {
                        Type arrayMarshalerType = null;
                        switch (marshalerInfo.MarshalerType)
                        {
                            case ManagedUnrealMarshalerType.Default:
                                arrayMarshalerType = typeof(TArrayReadWriteMarshaler<>);
                                break;
                            case ManagedUnrealMarshalerType.ReadOnly:
                                arrayMarshalerType = typeof(TArrayReadOnlyMarshaler<>);
                                break;
                            case ManagedUnrealMarshalerType.Copy:
                                arrayMarshalerType = typeof(TArrayCopyMarshaler<>);
                                break;
                        }
                        Type elementType = GetTypeFromTypeInfo(marshalerInfo.Arg1Type, marshalerInfo.Arg1Path);
                        return arrayMarshalerType.MakeGenericType(elementType);
                    }

                case EPropertyType.Map:
                    {
                        Type mapMarshalerType = null;
                        switch (marshalerInfo.MarshalerType)
                        {
                            case ManagedUnrealMarshalerType.Default:
                                mapMarshalerType = typeof(TMapReadWriteMarshaler<,>);
                                break;
                            case ManagedUnrealMarshalerType.ReadOnly:
                                mapMarshalerType = typeof(TMapReadOnlyMarshaler<,>);
                                break;
                            case ManagedUnrealMarshalerType.Copy:
                                mapMarshalerType = typeof(TMapCopyMarshaler<,>);
                                break;
                        }
                        Type keyType = GetTypeFromTypeInfo(marshalerInfo.Arg1Type, marshalerInfo.Arg1Path);
                        Type valueType = GetTypeFromTypeInfo(marshalerInfo.Arg2Type, marshalerInfo.Arg2Path);
                        return mapMarshalerType.MakeGenericType(keyType, valueType);
                    }

                case EPropertyType.Set:
                    {
                        Type setMarshalerType = null;
                        switch (marshalerInfo.MarshalerType)
                        {
                            case ManagedUnrealMarshalerType.Default:
                                setMarshalerType = typeof(TSetReadWriteMarshaler<>);
                                break;
                            case ManagedUnrealMarshalerType.ReadOnly:
                                setMarshalerType = typeof(TSetReadOnlyMarshaler<>);
                                break;
                            case ManagedUnrealMarshalerType.Copy:
                                setMarshalerType = typeof(TSetCopyMarshaler<>);
                                break;
                        }
                        Type elementType = GetTypeFromTypeInfo(marshalerInfo.Arg1Type, marshalerInfo.Arg1Path);
                        return setMarshalerType.MakeGenericType(elementType);
                    }

                default: return GetTypeFromMarshalerInfo(marshalerInfo.Type, marshalerInfo.Path);
            }
        }

        public static Type GetTypeFromMarshalerInfo(EPropertyType typeCode, string typePath)
        {
            switch (typeCode)
            {
                case EPropertyType.Bool: return typeof(BoolMarshaler);
                case EPropertyType.Int8: return typeof(BlittableTypeMarshaler<sbyte>);
                case EPropertyType.Int16: return typeof(BlittableTypeMarshaler<short>);
                case EPropertyType.Int: return typeof(BlittableTypeMarshaler<int>);
                case EPropertyType.Int64: return typeof(BlittableTypeMarshaler<long>);
                case EPropertyType.Byte: return typeof(BlittableTypeMarshaler<byte>);
                case EPropertyType.UInt16: return typeof(BlittableTypeMarshaler<ushort>);
                case EPropertyType.UInt32: return typeof(BlittableTypeMarshaler<uint>);
                case EPropertyType.UInt64: return typeof(BlittableTypeMarshaler<ulong>);
                case EPropertyType.Float: return typeof(BlittableTypeMarshaler<float>);
                case EPropertyType.Double: return typeof(BlittableTypeMarshaler<double>);
                case EPropertyType.Name: return typeof(BlittableTypeMarshaler<FName>);
                case EPropertyType.Text: return typeof(FTextMarshaler);
                case EPropertyType.Str: return typeof(FStringMarshaler);

                case EPropertyType.Enum:
                    {
                        Type enumMarshalerType = typeof(EnumMarshaler<>);
                        Type enumType = FindTypeByPath(typePath);
                        if (enumType != null)
                        {
                            return enumMarshalerType.MakeGenericType(enumType);
                        }
                        else
                        {
                            throw new UnrealMarshalerTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.Interface:
                    {
                        Type interfaceMarshalerType = typeof(InterfaceMarshaler<>);
                        Type interfaceType = FindTypeByPath(typePath);
                        if (interfaceType != null)
                        {
                            return interfaceMarshalerType.MakeGenericType(interfaceType);
                        }
                        else
                        {
                            throw new UnrealMarshalerTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.Struct:
                    {
                        Type structType = FindTypeByPath(typePath);
                        if (structType != null)
                        {
                            if (structType.IsSubclassOf(typeof(StructAsClass)))
                            {
                                return typeof(StructAsClassMarshaler<>).MakeGenericType(structType);
                            }
                            else
                            {
                                ManagedUnrealTypeInfo structTypeInfo = FindTypeInfoFromType(structType);
                                if (structTypeInfo != null)
                                {
                                    if (structTypeInfo.IsBlittable)
                                    {
                                        return typeof(BlittableTypeMarshaler<>).MakeGenericType(structType);
                                    }
                                    else
                                    {
                                        return structType;
                                    }
                                }
                                else
                                {
                                    if (ManagedUnrealModuleInfo.AllKnownBlittableTypes.ContainsKey(typePath))
                                    {
                                        return typeof(BlittableTypeMarshaler<>).MakeGenericType(structType);
                                    }
                                    else if (ManagedUnrealModuleInfo.AllKnownNonBlittableTypes.ContainsKey(typePath))
                                    {
                                        return structType;
                                    }
                                    throw new UnrealMarshalerTypeNotFoundException(typeCode, typePath);
                                }
                            }
                        }
                        else
                        {
                            throw new UnrealMarshalerTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.Class:
                    {                        
                        Type objectType = FindTypeByPath(typePath);
                        if (objectType != null)
                        {
                            Type classMarshalerType = null;
                            if (objectType.IsInterface)
                            {
                                classMarshalerType = typeof(TSubclassOfInterfaceMarshaler<>);
                            }
                            else
                            {
                                classMarshalerType = typeof(TSubclassOfMarshaler<>);
                            }

                            return classMarshalerType.MakeGenericType(objectType);
                        }
                        else
                        {
                            throw new UnrealMarshalerTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.Object:
                    {
                        Type objectMarshalerType = typeof(UObjectMarshaler<>);
                        Type objectType = FindTypeByPath(typePath);
                        if (objectType != null)
                        {
                            return objectMarshalerType.MakeGenericType(objectType);
                        }
                        else
                        {
                            throw new UnrealMarshalerTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.LazyObject:
                    {
                        Type lazyObjectMarshalerType = typeof(TLazyObjectMarshaler<>);
                        Type objectType = FindTypeByPath(typePath);
                        if (objectType != null)
                        {
                            return lazyObjectMarshalerType.MakeGenericType(objectType);
                        }
                        else
                        {
                            throw new UnrealMarshalerTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.WeakObject:
                    {
                        Type weakObjectMarshalerType = typeof(TWeakObjectMarshaler<>);
                        Type objectType = FindTypeByPath(typePath);
                        if (objectType != null)
                        {
                            return weakObjectMarshalerType.MakeGenericType(objectType);
                        }
                        else
                        {
                            throw new UnrealMarshalerTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.SoftClass:
                    {
                        Type softClassMarshalerType = typeof(TSoftClassMarshaler<>);
                        Type objectType = FindTypeByPath(typePath);
                        if (objectType != null)
                        {
                            return softClassMarshalerType.MakeGenericType(objectType);
                        }
                        else
                        {
                            throw new UnrealMarshalerTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.SoftObject:
                    {
                        Type softObjectMarshalerType = typeof(TSoftObjectMarshaler<>);
                        Type objectType = FindTypeByPath(typePath);
                        if (objectType != null)
                        {
                            return softObjectMarshalerType.MakeGenericType(objectType);
                        }
                        else
                        {
                            throw new UnrealMarshalerTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.Delegate:
                    {
                        Type delegateMarshalerType = typeof(FDelegateMarshaler<>);
                        Type delegateType = FindTypeByPath(typePath);
                        if (delegateType != null)
                        {
                            return delegateMarshalerType.MakeGenericType(delegateType);
                        }
                        else
                        {
                            throw new UnrealMarshalerTypeNotFoundException(typeCode, typePath);
                        }
                    }

                case EPropertyType.MulticastDelegate:
                    {
                        Type delegateMarshalerType = typeof(FMulticastDelegateMarshaler<>);
                        Type delegateType = FindTypeByPath(typePath);
                        if (delegateType != null)
                        {
                            return delegateMarshalerType.MakeGenericType(delegateType);
                        }
                        else
                        {
                            throw new UnrealMarshalerTypeNotFoundException(typeCode, typePath);
                        }
                    }

                default:
                    throw new NotImplementedException("Unhandled marshaler type " + typeCode);
            }
        }

        public static Type GetMarshalingDelegatesDelegate(ManagedUnrealTypeInfo typeInfo, bool fromNative)
        {
            return GetMarshalingDelegatesDelegate(typeInfo.TypeCode, typeInfo.Path, fromNative);
        }

        public static Type GetMarshalingDelegatesDelegate(ManagedUnrealTypeInfoReference typeInfo, bool fromNative)
        {
            return GetMarshalingDelegatesDelegate(typeInfo.TypeCode, typeInfo.Path, fromNative);
        }

        public static Type GetMarshalingDelegatesDelegate(EPropertyType typeCode, string typePath, bool fromNative)
        {
            if (fromNative)
            {
                return typeof(MarshalingDelegates<>.FromNative).MakeGenericType(GetTypeFromTypeInfo(typeCode, typePath));
            }
            else
            {
                return typeof(MarshalingDelegates<>.ToNative).MakeGenericType(GetTypeFromTypeInfo(typeCode, typePath));
            }
        }

        public static Type GetCachedMarshalingDelegatesType(ManagedUnrealMarshalerType marshalerType, ManagedUnrealTypeInfoReference typeInfo)
        {
            // Remove the marshalerType param? It is only used for collections and as marshaling delegates are only for collections
            // and the args of a collection can't be another collection, the marshalerType isn't used.

            // <T, TMarshaler>
            return typeof(CachedMarshalingDelegates<,>).MakeGenericType(
                GetTypeFromTypeInfo(typeInfo), GetMarshalerType(marshalerType, typeInfo));
        }

        public static FieldInfo GetCachedMarshalingDelegatesDelegate(Type cachedMarshalingDelegatesType, bool fromNative)
        {
            FieldInfo field = null;
            if (fromNative)
            {
                field = cachedMarshalingDelegatesType.GetField("FromNative");
            }
            else
            {
                field = cachedMarshalingDelegatesType.GetField("ToNative");
            }
            Debug.Assert(field != null);
            return field;
        }

        public static bool PropertyRequiresMarshalerInstance(ManagedUnrealPropertyInfo propertyInfo)
        {
            return propertyInfo.IsCollection;
        }

        public static bool IsPropertyMarshalerStruct(ManagedUnrealPropertyInfo propertyInfo)
        {
            if (propertyInfo.IsCollection)
            {
                // Collection copy marshalers are structs
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the given property requires initialization (UProperty::InitializeValue) before being used. If false
        /// zerod memory should be fine.
        /// </summary>
        public static bool PropertyRequiresInit(ManagedUnrealPropertyInfo propertyInfo)
        {
            switch (propertyInfo.Type.TypeCode)
            {
                case EPropertyType.Bool:
                case EPropertyType.Byte:
                case EPropertyType.Int8:
                case EPropertyType.Int16:
                case EPropertyType.UInt16:
                case EPropertyType.Int:
                case EPropertyType.UInt32:
                case EPropertyType.Int64:
                case EPropertyType.UInt64:
                case EPropertyType.Float:
                case EPropertyType.Double:
                case EPropertyType.Name:
                case EPropertyType.Enum:
                case EPropertyType.Object:
                case EPropertyType.Class:
                case EPropertyType.Interface:
                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                case EPropertyType.LazyObject:
                case EPropertyType.WeakObject:
                    return false;

                case EPropertyType.Array:
                case EPropertyType.Set:
                case EPropertyType.Map:
                case EPropertyType.Str:
                case EPropertyType.Text:
                case EPropertyType.SoftClass:
                case EPropertyType.SoftObject:
                    // These need destroying but should be fine without being initialized
                    return false;

                case EPropertyType.InternalNativeFixedSizeArray:
                case EPropertyType.InternalManagedFixedSizeArray:
                    return true;

                case EPropertyType.Struct:
                    // Structs are the main reason for the requirement of init / destroy
                    EStructFlags structFlags = EStructFlags.NoFlags;
                    Type structType = FindTypeByPath(propertyInfo.Type.Path);
                    if (structType != null)
                    {
                        structFlags = ResolveStructCtorDtorFlags(structType);
                    }
                    return !structFlags.HasFlag(EStructFlags.ZeroConstructor);

                default:
                    throw new NotImplementedException("Unhandled type " + propertyInfo.Type.TypeCode);
            }
        }

        /// <summary>
        /// Returns true if the given property requires destruction (UProperty::DestroyValue) after being used.
        /// </summary>
        public static bool PropertyRequiresDestroy(ManagedUnrealPropertyInfo propertyInfo)
        {
            switch (propertyInfo.Type.TypeCode)
            {
                case EPropertyType.Bool:
                case EPropertyType.Byte:
                case EPropertyType.Int8:
                case EPropertyType.Int16:
                case EPropertyType.UInt16:
                case EPropertyType.Int:
                case EPropertyType.UInt32:
                case EPropertyType.Int64:
                case EPropertyType.UInt64:
                case EPropertyType.Float:
                case EPropertyType.Double:
                case EPropertyType.Name:
                case EPropertyType.Enum:
                case EPropertyType.Object:
                case EPropertyType.Class:
                case EPropertyType.Interface:
                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                case EPropertyType.LazyObject:
                case EPropertyType.WeakObject:
                    return false;

                case EPropertyType.Array:
                case EPropertyType.Set:
                case EPropertyType.Map:
                case EPropertyType.Str:
                case EPropertyType.Text:
                case EPropertyType.SoftClass:
                case EPropertyType.SoftObject:
                    return true;

                case EPropertyType.InternalNativeFixedSizeArray:
                case EPropertyType.InternalManagedFixedSizeArray:
                    return true;

                case EPropertyType.Struct:
                    // Structs are the main reason for the requirement of init / destroy
                    EStructFlags structFlags = EStructFlags.NoFlags;
                    Type structType = FindTypeByPath(propertyInfo.Type.Path);
                    if (structType != null)
                    {
                        structFlags = ResolveStructCtorDtorFlags(structType);
                    }
                    return !structFlags.HasFlag(EStructFlags.NoDestructor);

                default:
                    throw new NotImplementedException("Unhandled type " + propertyInfo.Type.TypeCode);
            }
        }
        
        private static EStructFlags ResolveStructCtorDtorFlags(Type structType)
        {
            EStructFlags structFlags;
            if (!ManagedUnrealModuleInfo.resolvedStructCtorDtorFlags.TryGetValue(structType, out structFlags))
            {
                structFlags = EStructFlags.ZeroConstructor | EStructFlags.NoDestructor;
                
                // This needs to be a resolve type rather than find type            
                ManagedUnrealTypeInfo typeInfo = FindTypeInfoFromType(structType);
                if (typeInfo == null)
                {
                    return EStructFlags.NoFlags;
                }

                foreach (ManagedUnrealPropertyInfo propInfo in typeInfo.Properties)
                {
                    if (propInfo.Type.TypeCode == EPropertyType.Struct)
                    {
                        // It seems when you Bind/StaticLink a POD struct which contains another POD struct it doesn't
                        // get marked as POD. We should do the same to be consistent. If this ever changes update the
                        // checks based on the requirements for a POD struct to contain another POD struct.
                        structFlags &= ~EStructFlags.ZeroConstructor;
                        structFlags &= ~EStructFlags.NoDestructor;
                        break;
                    }

                    // Hope there can't be circular dependencies
                    if (PropertyRequiresInit(propInfo))
                    {
                        structFlags &= ~EStructFlags.ZeroConstructor;
                    }
                    if (PropertyRequiresDestroy(propInfo))
                    {
                        structFlags &= ~EStructFlags.NoDestructor;
                    }
                }
                ManagedUnrealModuleInfo.resolvedStructCtorDtorFlags.Add(structType, structFlags);
            }
            return structFlags;
        }

        public static bool IsSamePropertyType(ManagedUnrealPropertyInfo prop, ManagedUnrealPropertyInfo other)
        {
            if (prop.Type.TypeCode != other.Type.TypeCode ||
                prop.Type.Path != other.Type.Path)
            {
                return false;
            }

            if (prop.GenericArgs.Count != other.GenericArgs.Count)
            {
                return false;
            }

            for (int i = 0; i < prop.GenericArgs.Count; i++)
            {
                if (prop.GenericArgs[i].TypeCode != other.GenericArgs[i].TypeCode ||
                    prop.GenericArgs[i].TypeCode != other.GenericArgs[i].TypeCode)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

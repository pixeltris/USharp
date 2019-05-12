using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// An UnrealScript variable.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.Property", "CoreUObject", UnrealModuleType.Engine)]
    public class UProperty : UField
    {
        /// <summary>
        /// Gets the UProperty address for the given path (e.g. "/Script/Engine.Actor:bReplicates")
        /// </summary>
        /// <param name="path">The path of the UProperty</param>
        /// <returns>The address of the UProperty for the given path</returns>
        public static IntPtr GetPropertyAddress(string path)
        {
            return NativeReflection.FindObject(Classes.UProperty, IntPtr.Zero, path, false);
        }

        /// <summary>
        /// Gets the UProperty for the given path (e.g. "/Script/Engine.Actor:bReplicates")
        /// </summary>
        /// <param name="path">The path of the UProperty</param>
        /// <returns>The UProperty for the given path</returns>
        public static UProperty GetProperty(string path)
        {
            IntPtr address = GetPropertyAddress(path);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UProperty>(address);
            }
            return null;
        }

        public virtual EPropertyType PropertyType
        {
            get { return EPropertyType.Unknown; }
        }

        /// <summary>
        /// Returns true if the type is blittable
        /// </summary>
        public virtual bool IsBlittableType
        {
            get { return false; }
        }

        /// <summary>
        /// Returns true if the type is blittable and it isn't an array
        /// </summary>
        public bool IsBlittable
        {
            get { return IsBlittableType && !IsFixedSizeArray; }
        }

        /// <summary>
        /// Returns true if this is a fixed size array (ArrayDim > 1)
        /// </summary>
        public bool IsFixedSizeArray
        {
            get { return ArrayDim > 1; }
        }

        public int Offset
        {
            get { return Native_UProperty.GetOffset_ForInternal(Address); }
        }

        public int ArrayDim
        {
            get { return Native_UProperty.Get_ArrayDim(Address); }
            set { Native_UProperty.Set_ArrayDim(Address, value); }
        }

        public int ElementSize
        {
            get { return Native_UProperty.Get_ElementSize(Address); }
            set { Native_UProperty.Set_ElementSize(Address, value); }
        }

        public EPropertyFlags PropertyFlags
        {
            get { return Native_UProperty.Get_PropertyFlags(Address); }
            set { Native_UProperty.Set_PropertyFlags(Address, value); }
        }

        public ushort RepIndex
        {
            get { return Native_UProperty.Get_RepIndex(Address); }
            set { Native_UProperty.Set_RepIndex(Address, value); }
        }

        public FName RepNotifyFunc
        {
            get
            {
                FName result;
                Native_UProperty.Get_RepNotifyFunc(Address, out result);
                return result;
            }
            set { Native_UProperty.Set_RepNotifyFunc(Address, ref value); }
        }

        private CachedUObject<UProperty> propertyLinkNext;
        /// <summary>
        /// In memory only: Linked list of properties from most-derived to base
        /// </summary>
        public UProperty PropertyLinkNext
        {
            get { return propertyLinkNext.Update(Native_UProperty.Get_PropertyLinkNext(Address)); }
            set { Native_UProperty.Set_PropertyLinkNext(Address, propertyLinkNext.Set(value)); }
        }

        private CachedUObject<UProperty> nextRef;
        /// <summary>
        /// In memory only: Linked list of object reference properties from most-derived to base
        /// </summary>
        public UProperty NextRef
        {
            get { return nextRef.Update(Native_UProperty.Get_NextRef(Address)); }
            set { Native_UProperty.Set_NextRef(Address, nextRef.Set(value)); }
        }

        private CachedUObject<UProperty> destructorLinkNext;
        /// <summary>
        /// In memory only: Linked list of properties requiring destruction. Note this does not include things that will be destroyed byt he native destructor
        /// </summary>
        public UProperty DestructorLinkNext
        {
            get { return destructorLinkNext.Update(Native_UProperty.Get_DestructorLinkNext(Address)); }
            set { Native_UProperty.Set_DestructorLinkNext(Address, destructorLinkNext.Set(value)); }
        }

        /// <summary>
        /// In memory only: Linked list of properties requiring post constructor initialization.
        /// </summary>
        private CachedUObject<UProperty> postConstructLinkNext;
        public UProperty PostConstructLinkNext
        {
            get { return postConstructLinkNext.Update(Native_UProperty.Get_PostConstructLinkNext(Address)); }
            set { Native_UProperty.Set_PostConstructLinkNext(Address, postConstructLinkNext.Set(value)); }
        }

        public string GetCPPMacroType(string extendedTypeText)
        {
            using (FStringUnsafe extendedTypeTextUnsafe = new FStringUnsafe(extendedTypeText))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UProperty.GetCPPMacroType(Address, ref extendedTypeTextUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        public bool PassCPPArgsByRef()
        {
            return Native_UProperty.PassCPPArgsByRef(Address);
        }

        /// <summary>
        /// Returns the C++ name of the property, including the _DEPRECATED suffix if the 
        /// property is deprecated.
        /// </summary>
        /// <returns>C++ name of property</returns>
        public string GetNameCPP()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UProperty.GetNameCPP(Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Returns the text to use for exporting this property to header file.
        /// </summary>
        /// <param name="extendedTypeText">for property types which use templates, will be filled in with the type</param>
        /// <param name="cppExportFlags">flags for modifying the behavior of the export</param>
        /// <returns></returns>
        public string GetCPPType(string extendedTypeText = null, uint cppExportFlags = 0)
        {
            if (extendedTypeText == null)
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_UProperty.GetCPPType(Address, IntPtr.Zero, cppExportFlags, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
            else
            {
                unsafe
                {
                    using (FStringUnsafe extendedTypeTextUnsafe = new FStringUnsafe(extendedTypeText))
                    using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                    {
                        fixed (FScriptArray* extendedTypeTextUnsafePtr = &extendedTypeTextUnsafe.Array)
                        {
                            Native_UProperty.GetCPPType(Address, (IntPtr)extendedTypeTextUnsafePtr, cppExportFlags, ref resultUnsafe.Array);
                            return resultUnsafe.Value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Return offset of property from container base.
        /// </summary>
        /// <returns></returns>
        public int GetOffset_ForDebug()
        {
            return Native_UProperty.GetOffset_ForDebug(Address);
        }

        /// <summary>
        /// Return offset of property from container base.
        /// </summary>
        /// <returns></returns>
        public int GetOffset_ForUFunction()
        {
            return Native_UProperty.GetOffset_ForUFunction(Address);
        }

        /// <summary>
        /// Return offset of property from container base.
        /// </summary>
        /// <returns></returns>
        public int GetOffset_ForGC()
        {
            return Native_UProperty.GetOffset_ForGC(Address);
        }

        /// <summary>
        /// Return offset of property from container base.
        /// </summary>
        public int GetOffset_ForInternal()
        {
            return Native_UProperty.GetOffset_ForInternal(Address);
        }

        /// <summary>
        /// Return offset of property from container base.
        /// </summary>
        /// <returns></returns>
        public int GetOffset_ReplaceWith_ContainerPtrToValuePtr()
        {
            return Native_UProperty.GetOffset_ReplaceWith_ContainerPtrToValuePtr(Address);
        }

        /// <summary>
        /// Get the pointer to property value in a supplied 'container'. 
        /// You can _only_ call this function on a UObject* or a uint8*. If the property you want is a 'top level' UObject property, you _must_
        /// call the function passing in a UObject* and not a uint8*. There are checks inside the function to vertify this.
        /// </summary>
        /// <param name="container">UObject* or uint8* to container of property value</param>
        /// <param name="arrayIndex">In array case, index of array element we want</param>
        /// <returns></returns>
        public IntPtr ContainerUObjectPtrToValuePtr(IntPtr container, int arrayIndex = 0)
        {
            return Native_UProperty.ContainerUObjectPtrToValuePtr(Address, container, arrayIndex);
        }

        /// <summary>
        /// Get the pointer to property value in a supplied 'container'. 
        /// You can _only_ call this function on a UObject* or a uint8*. If the property you want is a 'top level' UObject property, you _must_
        /// call the function passing in a UObject* and not a uint8*. There are checks inside the function to vertify this.
        /// </summary>
        /// <param name="container">UObject* or uint8* to container of property value</param>
        /// <param name="arrayIndex">In array case, index of array element we want</param>
        /// <returns></returns>
        public IntPtr ContainerPtrToValuePtr(UObject container, int arrayIndex = 0)
        {
            return ContainerUObjectPtrToValuePtr(container == null ? IntPtr.Zero : container.Address, arrayIndex);
        }

        /// <summary>
        /// Get the pointer to property value in a supplied 'container'. 
        /// You can _only_ call this function on a UObject* or a uint8*. If the property you want is a 'top level' UObject property, you _must_
        /// call the function passing in a UObject* and not a uint8*. There are checks inside the function to vertify this.
        /// </summary>
        /// <param name="container">UObject* or uint8* to container of property value</param>
        /// <param name="arrayIndex">In array case, index of array element we want</param>
        /// <returns></returns>
        public IntPtr ContainerPtrToValuePtr(IntPtr container, int arrayIndex = 0)
        {
            return Native_UProperty.ContainerVoidPtrToValuePtr(Address, container, arrayIndex);
        }

        public uint GetValueTypeHash(IntPtr src)
        {
            return Native_UProperty.GetValueTypeHash(Address, src);
        }

        public bool ShouldPort(uint portFlags)
        {
            return Native_UProperty.ShouldPort(Address, portFlags);
        }

        public FName GetID()
        {
            FName result;
            Native_UProperty.GetID(Address, out result);
            return result;
        }

        public int GetMinAlignment()
        {
            return Native_UProperty.GetMinAlignment(Address);
        }

        /// <summary>
        /// Returns true if this property, or in the case of e.g. array or struct properties any sub- property, contains a
        /// UObject reference.
        /// </summary>
        /// <returns>true if property (or sub- properties) contain a UObject reference, false otherwise</returns>
        public bool ContainsObjectReference(List<UStructProperty> encounteredStructProps)
        {
            using (TArrayUnsafe<UStructProperty> encounteredStructPropsUnsafe = new TArrayUnsafe<UStructProperty>())
            {
                bool result = Native_UProperty.ContainsObjectReference(Address, encounteredStructPropsUnsafe.Address);
                if (encounteredStructProps != null)
                {
                    encounteredStructProps.Clear();
                    encounteredStructProps.AddRange(encounteredStructPropsUnsafe);
                }
                return result;
            }
        }

        /// <summary>
        /// Returns true if this property, or in the case of e.g. array or struct properties any sub- property, contains a
        /// weak UObject reference.
        /// </summary>
        /// <returns>true if property (or sub- properties) contain a weak UObject reference, false otherwise</returns>
        public bool ContainsWeakObjectReference()
        {
            return Native_UProperty.ContainsWeakObjectReference(Address);
        }

        /// <summary>
        /// Returns true if this property, or in the case of e.g. array or struct properties any sub- property, contains a
        /// UObject reference that is marked CPF_NeedCtorLink (i.e. instanced keyword).
        /// </summary>
        /// <returns>true if property (or sub- properties) contain a UObjectProperty that is marked CPF_NeedCtorLink, false otherwise</returns>
        public bool ContainsInstancedObjectProperty()
        {
            return Native_UProperty.ContainsInstancedObjectProperty(Address);
        }

        /// <summary>
        /// Emits tokens used by realtime garbage collection code to passed in ReferenceTokenStream. The offset emitted is relative
        /// to the passed in BaseOffset which is used by e.g. arrays of structs.
        /// </summary>
        /// <param name="unrealClass"></param>
        /// <param name="baseOffset"></param>
        /// <param name="encounteredStructProps"></param>
        public void EmitReferenceInfo(UClass unrealClass, int baseOffset, List<UStructProperty> encounteredStructProps)
        {
            using (TArrayUnsafe<UStructProperty> encounteredStructPropsUnsafe = new TArrayUnsafe<UStructProperty>())
            {
                Native_UProperty.EmitReferenceInfo(Address, unrealClass == null ? IntPtr.Zero : unrealClass.Address, baseOffset,
                    encounteredStructPropsUnsafe.Address);
                if (encounteredStructProps != null)
                {
                    encounteredStructProps.Clear();
                    encounteredStructProps.AddRange(encounteredStructPropsUnsafe);
                }
            }
        }

        public int GetSize()
        {
            return Native_UProperty.GetSize(Address);
        }

        /// <summary>
        /// Determines whether this property value is eligible for copying when duplicating an object
        /// </summary>
        /// <returns>true if this property value should be copied into the duplicate object</returns>
        public bool ShouldDuplicateValue()
        {
            return Native_UProperty.ShouldDuplicateValue(Address);
        }

        /// <summary>
        /// Returns the first UProperty in this property's Outer chain that does not have a UProperty for an Outer
        /// </summary>
        /// <returns></returns>
        public UProperty GetOwnerProperty()
        {
            return GCHelper.Find<UProperty>(Native_UProperty.GetOwnerProperty(Address));
        }

        /// <summary>
        /// Returns this property's propertyflags
        /// </summary>
        /// <returns></returns>
        public EPropertyFlags GetPropertyFlags()
        {
            return Native_UProperty.GetPropertyFlags(Address);
        }

        public void SetPropertyFlags(EPropertyFlags newFlags)
        {
            Native_UProperty.SetPropertyFlags(Address, newFlags);
        }

        public void ClearPropertyFlags(EPropertyFlags newFlags)
        {
            Native_UProperty.ClearPropertyFlags(Address, newFlags);
        }

        /// <summary>
        /// Used to safely check whether any of the passed in flags are set. This is required
        /// as PropertyFlags currently is a 64 bit data type and bool is a 32 bit data type so
        /// simply using PropertyFlags&amp;CPF_MyFlagBiggerThanMaxInt won't work correctly when
        /// assigned directly to an bool.
        /// </summary>
        /// <param name="flagsToCheck">Object flags to check for.</param>
        /// <returns>true if any of the passed in flags are set, false otherwise  (including no flags passed in).</returns>
        public bool HasAnyPropertyFlags(EPropertyFlags flagsToCheck)
        {
            return Native_UProperty.HasAnyPropertyFlags(Address, flagsToCheck);
        }

        /// <summary>
        /// Used to safely check whether all of the passed in flags are set. This is required
        /// as PropertyFlags currently is a 64 bit data type and bool is a 32 bit data type so
        /// simply using PropertyFlags&amp;CPF_MyFlagBiggerThanMaxInt won't work correctly when
        /// assigned directly to an bool.
        /// </summary>
        /// <param name="flagsToCheck">Object flags to check for</param>
        /// <returns>true if all of the passed in flags are set (including no flags passed in), false otherwise</returns>
        public bool HasAllPropertyFlags(EPropertyFlags flagsToCheck)
        {
            return Native_UProperty.HasAllPropertyFlags(Address, flagsToCheck);
        }

        /// <summary>
        /// Returns the replication owner, which is the property itself, or NULL if this isn't important for replication.
        /// It is relevant if the property is a net relevant and not being run in the editor
        /// </summary>
        /// <returns></returns>
        public UProperty GetRepOwner()
        {
            return GCHelper.Find<UProperty>(Native_UProperty.GetRepOwner(Address));
        }

        /// <summary>
        /// Editor-only properties are those that only are used with the editor is present or cannot be removed from serialisation.
        /// Editor-only properties include: EditorOnly properties
        /// Properties that cannot be removed from serialisation are:
        ///     Boolean properties (may affect GCC_BITFIELD_MAGIC computation)
        ///     Native properties (native serialisation)
        /// </summary>
        /// <returns></returns>
        public bool IsEditorOnlyProperty()
        {
            return Native_UProperty.IsEditorOnlyProperty(Address);
        }

        /// <summary>
        /// returns true, if Other is property of exactly the same type
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool SameType(UProperty other)
        {
            return Native_UProperty.SameType(Address, other == null ? IntPtr.Zero : other.Address);
        }

        // Move this somewhere else? Where would this be more appropriate?
        public static Type GetTypeFromProperty(UProperty prop)
        {
            if (prop == null)
            {
                return null;
            }

            switch (prop.PropertyType)
            {
                case EPropertyType.Bool:
                    return typeof(bool);
                case EPropertyType.Int8:
                    return typeof(sbyte);
                case EPropertyType.Byte:
                    return typeof(byte);
                case EPropertyType.Int16:
                    return typeof(short);
                case EPropertyType.UInt16:
                    return typeof(ushort);
                case EPropertyType.Int:
                    return typeof(int);
                case EPropertyType.UInt32:
                    return typeof(uint);
                case EPropertyType.Int64:
                    return typeof(long);
                case EPropertyType.UInt64:
                    return typeof(ulong);
                case EPropertyType.Float:
                    return typeof(float);
                case EPropertyType.Double:
                    return typeof(double);
                case EPropertyType.Enum:
                    {
                        UEnum unrealEnum = (prop as UEnumProperty).GetEnum();
                        if (unrealEnum == null)
                        {
                            return null;
                        }

                        Type enumType;
                        ManagedUnrealModuleInfo.AllKnownUnrealTypes.TryGetValue(unrealEnum.GetPathName(), out enumType);
                        return enumType;
                    }
                case EPropertyType.Str:
                    return typeof(string);
                case EPropertyType.Name:
                    return typeof(FName);
                case EPropertyType.Text:
                    return typeof(FText);
                case EPropertyType.Interface:
                    {
                        UClass unrealClassInterface = (prop as UInterfaceProperty).InterfaceClass;
                        if (unrealClassInterface == null)
                        {
                            return null;
                        }

                        Type interfaceType;
                        ManagedUnrealModuleInfo.AllKnownUnrealTypes.TryGetValue(unrealClassInterface.GetPathName(), out interfaceType);
                        return interfaceType;

                    }
                case EPropertyType.Struct:
                    {
                        UScriptStruct unrealStruct = (prop as UStructProperty).Struct;
                        if (unrealStruct == null)
                        {
                            return null;
                        }

                        Type structType;
                        ManagedUnrealModuleInfo.AllKnownUnrealTypes.TryGetValue(unrealStruct.GetPathName(), out structType);
                        return structType;
                    }
                case EPropertyType.Class:
                case EPropertyType.Object:
                case EPropertyType.LazyObject:
                case EPropertyType.WeakObject:
                case EPropertyType.SoftClass:
                case EPropertyType.SoftObject:
                    {
                        UClass objectClass = (prop as UObjectPropertyBase).PropertyClass;
                        switch (prop.PropertyType)
                        {
                            case EPropertyType.Class:
                                objectClass = (prop as UClassProperty).MetaClass;
                                break;
                            case EPropertyType.SoftClass:
                                objectClass = (prop as USoftClassProperty).MetaClass;
                                break;
                        }

                        Type type = null;
                        if (objectClass != null)
                        {
                            // Could use UClass.GetType but using AllKnownUnrealTypes for slightly more coverage
                            // UClass.GetType(objectClass)
                            ManagedUnrealModuleInfo.AllKnownUnrealTypes.TryGetValue(objectClass.GetPathName(), out type);
                        }

                        if (type == null)
                        {
                            //classType = typeof(UObject);// Fall back to UObject? Return null?
                            return null;
                        }
                        switch (prop.PropertyType)
                        {
                            case EPropertyType.Class: return typeof(TSubclassOf<>).MakeGenericType(type);
                            case EPropertyType.LazyObject: return typeof(TLazyObject<>).MakeGenericType(type);
                            case EPropertyType.WeakObject: return typeof(TWeakObject<>).MakeGenericType(type);
                            case EPropertyType.SoftClass: return typeof(TSoftClass<>).MakeGenericType(type);
                            case EPropertyType.SoftObject: return typeof(TSoftObject<>).MakeGenericType(type);
                            case EPropertyType.Object: return type;
                        }
                        return type;
                    }

                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    Type delegateType = null;
                    UFunction signatureFunc = null;
                    if (prop.PropertyType == EPropertyType.Delegate)
                    {
                        signatureFunc = (prop as UDelegateProperty).SignatureFunction;
                    }
                    else if (prop.PropertyType == EPropertyType.MulticastDelegate)
                    {
                        signatureFunc = (prop as UMulticastDelegateProperty).SignatureFunction;
                    }
                    if (signatureFunc != null)
                    {
                        if (ManagedUnrealModuleInfo.AllKnownUnrealTypes.TryGetValue(signatureFunc.GetPathName(), out delegateType))
                        {
                            if (prop.PropertyType == EPropertyType.Delegate)
                            {
                                if (!delegateType.IsSameOrSubclassOfGeneric(typeof(FDelegate<>)))
                                {
                                    delegateType = null;
                                }
                            }
                            else if (prop.PropertyType == EPropertyType.MulticastDelegate)
                            {
                                if (!delegateType.IsSameOrSubclassOfGeneric(typeof(FMulticastDelegate<>)))
                                {
                                    delegateType = null;
                                }
                            }
                        }
                    }
                    return delegateType;

                case EPropertyType.Array:
                    {
                        UArrayProperty arrayProp = prop as UArrayProperty;
                        Type innerType = GetTypeFromProperty(arrayProp.Inner);
                        if (innerType != null)
                        {
                            // Possibly handle IReadOnlyList?
                            return typeof(IList<>).MakeGenericType(innerType);
                        }
                        return null;
                    }
                case EPropertyType.Set:
                    {
                        USetProperty setProp = prop as USetProperty;
                        Type innerType = GetTypeFromProperty(setProp.ElementProp);
                        if (innerType != null)
                        {
                            return typeof(ISet<>).MakeGenericType(innerType);
                        }
                        return null;
                    }
                case EPropertyType.Map:
                    {
                        UMapProperty mapProp = prop as UMapProperty;
                        Type keyType = GetTypeFromProperty(mapProp.KeyProp);
                        Type valueType = GetTypeFromProperty(mapProp.ValueProp);
                        if (keyType != null && valueType != null)
                        {
                            // Possibly handle IReadOnlyDictionary?
                            return typeof(IDictionary<,>).MakeGenericType(keyType, valueType);
                        }
                        return null;
                    }
            }

            return null;
        }

        /// <summary>
        /// Copies the value from a property into the string OutForm. ContainerMem points to the Struct or Class containing Property
        /// 
        /// <para/>This is a copy of FBlueprintEditorUtils::PropertyValueToString()
        /// </summary>
        public bool ValueToString(IntPtr container, out string result)
        {
            return ValueToString_Direct(ContainerPtrToValuePtr(container), out result);
        }

        /// <summary>
        /// Copies the value from a property into the string OutForm. DirectValue is the raw memory address of the property value
        /// 
        /// <para/>This is a copy of FBlueprintEditorUtils::PropertyValueToString_Direct()
        /// </summary>
        public bool ValueToString_Direct(IntPtr directValue, out string result)
        {
            // TODO: Handle the special case math structs like in FBlueprintEditorUtils::PropertyValueToString_Direct

            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                IntPtr defaultValue = directValue;
                bool succeeded = Native_UProperty.ExportText_Direct(Address, ref resultUnsafe.Array,
                    directValue, defaultValue, IntPtr.Zero, (int)EPropertyPortFlags.SerializedAsImportText, IntPtr.Zero);
                result = resultUnsafe.Value;
                return succeeded;
            }
        }
    }
}

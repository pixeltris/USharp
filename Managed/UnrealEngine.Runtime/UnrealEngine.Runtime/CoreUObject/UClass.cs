using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnrealEngine.Engine;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// An object class.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.Class", "CoreUObject", UnrealModuleType.Engine)]
    public class UClass : UStruct
    {
        private static Dictionary<Type, UClass> classes = new Dictionary<Type, UClass>();
        private static Dictionary<IntPtr, Type> classesByAddress = new Dictionary<IntPtr, Type>();

        // Hold onto seen class types for possible classes which are loaded after our Load() function is called
        private static HashSet<Type> seenClasses = new HashSet<Type>();
        private static int lastModuleCount = -1;

        // Delegates for handling some callback functions used in UClass
        public delegate void ClassConstructorType(IntPtr objectInitializer);
        public delegate IntPtr ClassVTableHelperCtorCallerType(IntPtr vtableHelper);
        public delegate void ClassAddReferencedObjectsType(IntPtr inThis, IntPtr referenceCollector);

        internal static void Load()
        {
            classes.Clear();
            classesByAddress.Clear();
            seenClasses.Clear();

            // Use two passes so that classesByAddress has all available types which are needed when
            // calling GCHelper.Find() in the second pass

            // First pass to fill up classesByAddress collection
            foreach (KeyValuePair<Type, UMetaPathAttribute> nativeType in UnrealTypes.Native)
            {
                Type type = nativeType.Key;
                UMetaPathAttribute attribute = nativeType.Value;

                if (type.IsSameOrSubclassOf(typeof(UObject)) || type.IsInterface)
                {
                    IntPtr classAddress = GetClassAddress(attribute.Path);
                    if (classAddress != IntPtr.Zero)
                    {
                        classesByAddress[classAddress] = type;
                    }
                }
            }

            // It should now be safe to access GCHelper (which is required by the second pass to fill
            // in the classes collection with the managed UClass objects)
            GCHelper.Available = true;

            int unknownCount = 0;
            // Second pass fill up classes collection
            foreach (KeyValuePair<IntPtr, Type> classByAddress in classesByAddress)
            {
                IntPtr classAddress = classByAddress.Key;
                Type type = classByAddress.Value;

                UClass unrealClass = GCHelper.Find<UClass>(classAddress);
                if (unrealClass != null)
                {
                    classes[type] = unrealClass;
                }
                else
                {
                    unknownCount++;
                }
            }

            if (unknownCount > 0)
            {
                // sync classes/classByAddress if some failed to add to the classes collection
                foreach (KeyValuePair<IntPtr, Type> classByAddress in new Dictionary<IntPtr, Type>(classesByAddress))
                {
                    if (!classes.ContainsKey(classByAddress.Value))
                    {
                        classesByAddress.Remove(classByAddress.Key);
                    }
                }
            }
        }

        internal static void Load(Assembly assembly)
        {
            Dictionary<Type, UMetaPathAttribute> nativeTypes;
            if (UnrealTypes.AssembliesNativeTypes.TryGetValue(assembly, out nativeTypes))
            {
                foreach (KeyValuePair<Type, UMetaPathAttribute> nativeType in nativeTypes)
                {
                    Type type = nativeType.Key;
                    UMetaPathAttribute attribute = nativeType.Value;

                    if (type.IsSameOrSubclassOf(typeof(UObject)) || type.IsInterface)
                    {
                        IntPtr classAddress = GetClassAddress(attribute.Path);
                        if (classAddress != IntPtr.Zero)
                        {
                            classesByAddress[classAddress] = type;
                        }
                    }
                }

                int unknownCount = 0;
                foreach (KeyValuePair<IntPtr, Type> classByAddress in classesByAddress)
                {
                    IntPtr classAddress = classByAddress.Key;
                    Type type = classByAddress.Value;

                    UClass unrealClass = GCHelper.Find<UClass>(classAddress);
                    if (unrealClass != null)
                    {
                        classes[type] = unrealClass;
                    }
                    else
                    {
                        unknownCount++;
                    }
                }

                if (unknownCount > 0)
                {
                    // sync classes/classByAddress if some failed to add to the classes collection
                    foreach (KeyValuePair<IntPtr, Type> classByAddress in new Dictionary<IntPtr, Type>(classesByAddress))
                    {
                        if (!classes.ContainsKey(classByAddress.Value))
                        {
                            classesByAddress.Remove(classByAddress.Key);
                        }
                    }
                }
            }
        }

        internal static void RegisterManagedClass(IntPtr classAddress, Type type)
        {
            UClass existingClass;
            if (classes.TryGetValue(type, out existingClass))
            {
                classes.Remove(type);
                classesByAddress.Remove(existingClass.Address);
            }
            seenClasses.Remove(type);

            classesByAddress[classAddress] = type;
            UClass unrealClass = GCHelper.Find<UClass>(classAddress);
            if (unrealClass != null)
            {
                classes[type] = unrealClass;
            }
            else
            {
                classesByAddress.Remove(classAddress);
            }

            // If this is an interface add it to UnrealInterfacePool so that we can create instances of this
            // interface which are implemented in Blueprint
            if (type.IsInterface)
            {
                UnrealInterfacePool.LoadType(type);
            }
        }

        /// <summary>
        /// Gets the UClass address holding the interface information for the given path (e.g. "/Script/MovieScene.MovieSceneEasingFunction")
        /// </summary>
        /// <param name="path">The path of the interface</param>
        /// <returns>The address of the UClass interface information for the given path</returns>
        public static IntPtr GetInterfaceClassAddress(string path)
        {
            IntPtr address = GetClassAddress(path);

            // Restrict this to just interfaces
            if (address != IntPtr.Zero && Native_UClass.HasAnyClassFlags(address, EClassFlags.Interface))
            {
                return address;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets the UClass address holding the interface information for the given IInterface derived type
        /// </summary>
        /// <typeparam name="T">The IInterface derived type</typeparam>
        /// <returns>The address of the UClass interface information for the given type</returns>
        public static IntPtr GetInterfaceClassAddress<T>() where T : IInterface
        {
            return GetInterfaceClassAddress(typeof(T));
        }

        /// <summary>
        /// Gets the UClass address holding the interface information for the given IInterface derived type
        /// </summary>
        /// <param name="type">The IInterface derived type</param>
        /// <returns>The address of the UClass interface information for the given type</returns>
        public static IntPtr GetInterfaceClassAddress(Type type)
        {
            UUnrealTypePathAttribute pathAttribute = UnrealTypes.GetPathAttribute(type);
            if (pathAttribute != null)
            {
                if (pathAttribute.IsManagedType)
                {
                    return ManagedUnrealTypes.GetInterfaceAddress(type);
                }
                else
                {
                    UClass unrealClass = GetClass(type);
                    if (unrealClass != null && unrealClass.HasAnyClassFlags(EClassFlags.Interface))
                    {
                        return unrealClass.Address;
                    }
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets the UClass holding the interface information the given IInterface derived type
        /// </summary>
        /// <typeparam name="T">The IInterface derived type</typeparam>
        /// <returns>The UClass holding the interface information the given IInterface derived type</returns>
        public static UClass GetInterfaceClass<T>() where T : IInterface
        {
            return GetInterfaceClass(typeof(T));
        }

        /// <summary>
        /// Gets the UClass holding the interface information the given IInterface derived type
        /// </summary>
        /// <param name="type">The IInterface derived type</param>
        /// <returns>The UClass holding the interface information the given IInterface derived type</returns>
        public static UClass GetInterfaceClass(Type type)
        {
            UClass interfaceClass = GetClass(type);
            if (interfaceClass != null && interfaceClass.HasAnyClassFlags(EClassFlags.Interface))
            {
                return interfaceClass;
            }
            return null;
        }

        /// <summary>
        /// Gets the UClass address for the given path (e.g. "/Script/Engine.Actor")
        /// </summary>
        /// <param name="path">The path of the object/class</param>
        /// <returns>The UClass address</returns>
        public static IntPtr GetClassAddress(string path)
        {
            IntPtr address = NativeReflection.FindObject(Classes.UClass, IntPtr.Zero, path, false);
            if (address == IntPtr.Zero)
            {
                FName newPath = FLinkerLoad.FindNewNameForClass(new FName(path), false);
                if (newPath != FName.None)
                {
                    address = NativeReflection.FindObject(Classes.UClass, IntPtr.Zero, newPath.ToString(), false);
                }
            }
            return address;
        }

        /// <summary>
        /// Loads the UClass address for the given path (e.g. "/Script/Engine.Actor")
        /// </summary>
        /// <param name="path">The path of the object/class</param>
        /// <returns>The UClass address</returns>
        public static IntPtr LoadClassAddress(string path)
        {
            return NativeReflection.LoadObject(Classes.UClass, IntPtr.Zero, path);
        }

        /// <summary>
        /// Finds or loads the UClass address for the given path (e.g. "/Script/Engine.Actor")
        /// </summary>
        /// <param name="path">The path of the object/class</param>
        /// <returns>The UClass address</returns>
        public static IntPtr ResolveClassAddress(string path)
        {
            IntPtr address = GetClassAddress(path);
            if (address == IntPtr.Zero)
            {
                address = LoadClassAddress(path);
            }
            return address;
        }

        /// <summary>
        /// Gets the UClass address for the given UObject derived type
        /// </summary>
        /// <typeparam name="T">The UObject derived type</typeparam>
        /// <returns>The address of the UClass for the given type</returns>
        public static IntPtr GetClassAddress<T>() where T : UObject
        {
            return GetClassAddress(typeof(T));
        }

        /// <summary>
        /// Gets the UClass address for the given UObject derived type
        /// </summary>
        /// <param name="type">The UObject derived type</param>
        /// <returns>The address of the UClass for the given type</returns>
        public static IntPtr GetClassAddress(Type type)
        {
            UUnrealTypePathAttribute pathAttribute = UnrealTypes.GetPathAttribute(type);
            if (pathAttribute != null)
            {
                if (pathAttribute.IsManagedType)
                {
                    return ManagedUnrealTypes.GetClassAddress(type);
                }
                else
                {
                    UClass unrealClass = GetClass(type);
                    if (unrealClass != null)
                    {
                        return unrealClass.Address;
                    }
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets the UClass for the given type
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>The UClass for the given type</returns>
        public static UClass GetClass<T>()
        {
            return GetClass(typeof(T));
        }

        /// <summary>
        /// Gets the UClass for the given type
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The UClass for the given type</returns>
        public static UClass GetClass(Type type)
        {
            UClass result = null;
            if (classes.TryGetValue(type, out result))
            {
                return result;
            }
            
            if (type.IsEnum || type.IsValueType || typeof(IDelegateBase).IsAssignableFrom(type))
            {
                // Find the top-most UClass (UUserDefinedEnum, UEnum, UUserDefinedStruct, UUserStruct, etc)
                // NOTE: This wont contain any useful information about the actual type itself
                
                IntPtr address = IntPtr.Zero;
                if (type.IsEnum)
                {
                    address = UEnum.GetEnumAddress(type);
                }
                else if (type.IsValueType)
                {
                    address = UScriptStruct.GetStructAddress(type);
                }
                else
                {
                    address = UFunction.GetDelegateSignatureAddress(type);
                }
                if (address != IntPtr.Zero)
                {
                    return GetClass(address);
                }
                return null;
            }

            if (!type.IsSameOrSubclassOf(typeof(UObject)) &&
                (!type.IsInterface || !typeof(IInterface).IsAssignableFrom(type)) || type == typeof(IInterface))
            {
                return null;
            }

            if (seenClasses.Contains(type))
            {
                // Note: GetModuleCount uses a lock
                // TODO: Find some multicast delegate which is called when a module is loaded or a new class type is created.
                // - FModuleManager::Get().OnProcessLoadedObjectsCallback
                if (FModuleManager.Get().GetModuleCount() != lastModuleCount)
                {
                    seenClasses.Clear();
                }
                else
                {
                    return null;
                }
            }

            if (!seenClasses.Contains(type))
            {
                seenClasses.Add(type);

                UMetaPathAttribute pathAttribute;
                if (UnrealTypes.Native.TryGetValue(type, out pathAttribute))
                {
                    IntPtr classAddress = GetClassAddress(pathAttribute.Path);
                    if (classAddress == IntPtr.Zero)
                    {
                        // Fallback if this class isn't loaded yet. TODO: Check if this is the correct method to call.
                        classAddress = NativeReflection.LoadObject(Classes.UClass, IntPtr.Zero, pathAttribute.Path);
                    }
                    if (classAddress != IntPtr.Zero)
                    {
                        UClass unrealClass = GCHelper.Find<UClass>(classAddress);
                        if (unrealClass != null)
                        {
                            classesByAddress[classAddress] = type;
                            classes[type] = unrealClass;
                            return unrealClass;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the UClass for the given UObject address
        /// </summary>
        /// <param name="objectAddress">The address of the UObject</param>
        /// <returns>The UClass for the given UObject address</returns>
        public static UClass GetClass(IntPtr objectAddress)
        {
            if (objectAddress == IntPtr.Zero)
            {
                return null;
            }
            return GCHelper.Find<UClass>(Native_UObjectBase.GetClass(objectAddress));
        }

        /// <summary>
        /// Gets the UClass for the given path (e.g. "/Script/Engine.Actor")
        /// </summary>
        /// <param name="path">The path of the object/class</param>
        /// <returns>The UClass for the given path</returns>
        public static UClass GetClass(string path)
        {
            UClass foundClass = UObject.FindObject<UClass>(UObject.AnyPackage, path);

            if (foundClass == null)
            {
                // Look for class redirectors
                FName newPath = FLinkerLoad.FindNewNameForClass(new FName(path), false);
                
                if (newPath != FName.None)
                {
                    foundClass = UObject.FindObject<UClass>(UObject.AnyPackage, newPath.ToString());
                }
            }
            return foundClass;
        }

        /// <summary>
        /// Gets the managed type for a given UClass
        /// </summary>
        /// <param name="unrealClass">The UClass to get the managed type for</param>
        /// <returns>The managed type</returns>
        public static Type GetType(UClass unrealClass)
        {
            Type type;
            classesByAddress.TryGetValue(unrealClass.Address, out type);
            return type;
        }

        /// <summary>
        /// Gets the managed type for a given UClass address
        /// </summary>
        /// <param name="unrealClass">The UClass address to get the managed type for</param>
        /// <returns>The managed type</returns>
        public static Type GetTypeFromClassAddress(IntPtr unrealClass)
        {
            Type type;
            classesByAddress.TryGetValue(unrealClass, out type);
            return type;
        }

        /// <summary>
        /// Gets the first known managed type for a given UClass
        /// </summary>
        /// <param name="unrealClass">The UClass to get the managed type from</param>
        /// <param name="includeAbstract">Include abstract types (if true a wrapper cannot be instantiated from this type)</param>
        /// <returns>The managed type</returns>
        public static Type GetFirstKnownType(UClass unrealClass, bool includeAbstract = true)
        {
            Type type = null;
            IntPtr unrealClassAddress = unrealClass.Address;
            while (type == null && unrealClassAddress != IntPtr.Zero)
            {
                classesByAddress.TryGetValue(unrealClassAddress, out type);
                if (type != null && !includeAbstract && type.IsAbstract)
                {
                    type = null;
                }
                unrealClassAddress = Native_UStruct.GetSuperStruct(unrealClassAddress);
            }
            return type;
        }

        /// <summary>
        /// Gets the managed type for a given UObject address
        /// </summary>
        /// <param name="objectAddress">The UObject address</param>
        /// <returns>The managed type</returns>
        public static Type GetType(IntPtr objectAddress)
        {
            if (objectAddress == IntPtr.Zero)
            {
                return null;
            }
            
            IntPtr unrealClassAddress = Native_UObjectBase.GetClass(objectAddress);
            if (unrealClassAddress == Classes.UClass)
            {
                // The objectAddress is already a class, if the type is also a UClass (UDynamicClass, UBlueprintGeneratedClass
                // then get the correct class type, otherwise return typeof(UClass))
                Type classType = null;
                if (classesByAddress.TryGetValue(objectAddress, out classType) && classType.IsSubclassOf(typeof(UClass)))
                {
                    return classType;
                }
                return typeof(UClass);
            }
            if (unrealClassAddress != IntPtr.Zero)
            {
                Type result = null;
                classesByAddress.TryGetValue(unrealClassAddress, out result);
                return result;
            }
            return null;
        }

        /// <summary>
        /// Gets the first known type from a given UObject address (classes may be defined at runtime which we don't have a type mapping for.
        /// Look up the inheritance chain until we have a known type)
        /// </summary>
        /// <param name="objectAddress">The UObject address</param>
        /// <param name="isKnownType">True if the result is the exact type of the given UObject address</param>
        /// <param name="includeAbstract">Include abstract types (if true a wrapper cannot be instantiated from this type)</param>
        /// <returns>The first known managed type</returns>
        public static Type GetFirstKnownType(IntPtr objectAddress, out bool isKnownType, bool includeAbstract = true)
        {
            isKnownType = true;
            if (objectAddress == IntPtr.Zero)
            {
                return null;
            }

            Type type = GetType(objectAddress);
            if (type != null && (!type.IsAbstract || includeAbstract))
            {
                return type;
            }
            else
            {
                type = null;
            }

            IntPtr parentClassAddress = IntPtr.Zero;

            IntPtr unrealClassAddress = Native_UObjectBase.GetClass(objectAddress);
            if (unrealClassAddress == Classes.UClass)
            {
                // The objectAddress is already a class
                unrealClassAddress = objectAddress;
            }

            if (unrealClassAddress != IntPtr.Zero)
            {
                parentClassAddress = Native_UStruct.GetSuperStruct(unrealClassAddress);
            }

            while (type == null && parentClassAddress != IntPtr.Zero)
            {
                isKnownType = false;
                classesByAddress.TryGetValue(parentClassAddress, out type);
                if (type != null && type.IsAbstract && !includeAbstract)
                {
                    type = null;
                }
                parentClassAddress = Native_UStruct.GetSuperStruct(parentClassAddress);
            }

            if (type == null && unrealClassAddress != IntPtr.Zero)
            {
                // Some fallbacks
                if (Native_UStruct.IsChildOf(unrealClassAddress, Classes.UStruct))
                {
                    if (Native_UStruct.IsChildOf(unrealClassAddress, Classes.UClass))
                    {
                        if (Native_UStruct.IsChildOf(unrealClassAddress, Classes.UBlueprintGeneratedClass))
                        {
                            return typeof(UBlueprintGeneratedClass);
                        }

                        return typeof(UClass);
                    }

                    if (Native_UStruct.IsChildOf(unrealClassAddress, Classes.UScriptStruct))
                    {
                        if (Native_UStruct.IsChildOf(unrealClassAddress, Classes.UUserDefinedStruct))
                        {
                            return typeof(UUserDefinedStruct);
                        }

                        return typeof(UScriptStruct);
                    }

                    if (Native_UStruct.IsChildOf(unrealClassAddress, Classes.UEnum))
                    {
                        if (Native_UStruct.IsChildOf(unrealClassAddress, Classes.UUserDefinedEnum))
                        {
                            return typeof(UUserDefinedEnum);
                        }

                        return typeof(UEnum);
                    }

                    if (Native_UStruct.IsChildOf(unrealClassAddress, Classes.UFunction))
                    {
                        return typeof(UFunction);
                    }

                    return typeof(UStruct);
                }

                // UObject fallback (would expect this to always be true)
                if (Native_UStruct.IsChildOf(unrealClassAddress, Classes.UObject))
                {                    
                    return typeof(UObject);
                }
            }
            
            return type;
        }

        /// <summary>
        /// Class flags; See EClassFlags for more information
        /// </summary>
        public EClassFlags ClassFlags
        {
            get { return Native_UClass.Get_ClassFlags(Address); }
            set { Native_UClass.Set_ClassFlags(Address, value); }
        }

        /// <summary>
        /// Cast flags used to accelerate dynamic_cast<T*> on objects of this type for common T
        /// </summary>
        public EClassCastFlags ClassCastFlags
        {
            get { return Native_UClass.Get_ClassCastFlags(Address); }
            set { Native_UClass.Set_ClassCastFlags(Address, value); }
        }

        /// <summary>
        /// Class pseudo-unique counter; used to accelerate unique instance name generation
        /// </summary>
        public int ClassUnique
        {
            get { return Native_UClass.Get_ClassUnique(Address); }
            set { Native_UClass.Set_ClassUnique(Address, value); }
        }

        private CachedUObject<UClass> classWithin;
        /// <summary>
        /// The required type for the outer of instances of this class.
        /// </summary>
        public UClass ClassWithin
        {
            get { return classWithin.Update(Native_UClass.Get_ClassWithin(Address)); }
            set { Native_UClass.Set_ClassWithin(Address, classWithin.Set(value)); }
        }

        private CachedUObject<UClass> classGeneratedBy;
        /// <summary>
        /// This is the blueprint that caused the generation of this class, or NULL if it is a native compiled-in class
        /// </summary>
        public UClass ClassGeneratedBy
        {
            get { return classGeneratedBy.Update(Native_UClass.Get_ClassGeneratedBy(Address)); }
            set { Native_UClass.Set_ClassGeneratedBy(Address, classGeneratedBy.Set(value)); }
        }

        /// <summary>
        /// Which Name.ini file to load Config variables out of
        /// </summary>
        public FName ClassConfigName
        {
            get
            {
                FName result;
                Native_UClass.Get_ClassConfigName(Address, out result);
                return result;
            }
            set { Native_UClass.Set_ClassConfigName(Address, ref value); }
        }

        /// <summary>
        /// List of network relevant fields (properties and functions)
        /// </summary>
        public UField[] NetFields
        {
            get
            {
                IntPtr netFields = Native_UClass.Get_NetFields(Address);
                if (netFields != IntPtr.Zero)
                {
                    return new TArrayUnsafeRef<UField>(netFields).ToArray();
                }
                return null;
            }
            set
            {
                using (TArrayUnsafe<UField> netFieldsUnsafe = new TArrayUnsafe<UField>())
                {
                    netFieldsUnsafe.AddRange(value);
                    Native_UClass.Set_NetFields(Address, netFieldsUnsafe.Address);
                }
            }
        }

        private CachedUObject<UObject> classDefaultObject;
        /// <summary>
        /// The class default object; used for delta serialization and object initialization
        /// </summary>
        public UObject ClassDefaultObject
        {
            get { return classDefaultObject.Update(Native_UClass.Get_ClassDefaultObject(Address)); }
            set { Native_UClass.Set_ClassDefaultObject(Address, classDefaultObject.Set(value)); }
        }

        /// <summary>
        /// Used to check if the class was cooked or not.
        /// </summary>
        public bool Cooked
        {
            get { return Native_UClass.Get_bCooked(Address); }
            set { Native_UClass.Set_bCooked(Address, value); }
        }

        /// <summary>
        /// The list of interfaces which this class implements, along with the pointer property that is located at the offset of the interface's vtable.
        /// If the interface class isn't native, the property will be NULL.
        /// </summary>
        public FImplementedInterface[] Interfaces
        {
            get
            {
                using (TArrayUnsafe<FImplementedInterface> interfaces = new TArrayUnsafe<FImplementedInterface>())
                {
                    Native_UClass.Get_Interfaces(Address, interfaces.Address);
                    return interfaces.ToArray();
                }
            }
            set
            {
                using (TArrayUnsafe<FImplementedInterface> interfacesUnsafe = new TArrayUnsafe<FImplementedInterface>())
                {
                    interfacesUnsafe.AddRange(value);
                    Native_UClass.Set_Interfaces(Address, interfacesUnsafe.Address);
                }
            }
        }

        /// <summary>
        /// This class's native functions.
        /// </summary>
        public FNativeFunctionLookup[] NativeFunctionLookupTable
        {
            get
            {
                IntPtr nativeFunctionLookupTable = Native_UClass.Get_NativeFunctionLookupTable(Address);
                if (nativeFunctionLookupTable != IntPtr.Zero)
                {
                    return new TArrayUnsafeRef<FNativeFunctionLookup>(nativeFunctionLookupTable).ToArray();
                }
                return null;
            }
            set
            {
                using (TArrayUnsafe<FNativeFunctionLookup> nativeFunctionLookupTableUnsafe = new TArrayUnsafe<FNativeFunctionLookup>())
                {
                    nativeFunctionLookupTableUnsafe.AddRange(value);
                    Native_UClass.Set_NativeFunctionLookupTable(Address, nativeFunctionLookupTableUnsafe.Address);
                }
            }
        }

        public bool IsFunctionHidden(string inFunction)
        {
            // WITH_EDITOR || HACK_HEADER_GENERATOR
            if (Native_UClass.IsFunctionHidden == null)
            {
                return false;
            }

            using (FStringUnsafe inFunctionUnsafe = new FStringUnsafe(inFunction))
            {
                return Native_UClass.IsFunctionHidden(Address, ref inFunctionUnsafe.Array);
            }
        }

        public bool IsAutoExpandCategory(string inCategory)
        {
            // WITH_EDITOR || HACK_HEADER_GENERATOR
            if (Native_UClass.IsAutoExpandCategory == null)
            {
                return false;
            }

            using (FStringUnsafe inCategoryUnsafe = new FStringUnsafe(inCategory))
            {
                return Native_UClass.IsAutoExpandCategory(Address, ref inCategoryUnsafe.Array);
            }
        }

        public bool IsAutoCollapseCategory(string inCategory)
        {
            // WITH_EDITOR || HACK_HEADER_GENERATOR
            if (Native_UClass.IsAutoCollapseCategory == null)
            {
                return false;
            }

            using (FStringUnsafe inCategoryUnsafe = new FStringUnsafe(inCategory))
            {
                return Native_UClass.IsAutoCollapseCategory(Address, ref inCategoryUnsafe.Array);
            }
        }

        public bool IsClassGroupName(string inGroupName)
        {
            // WITH_EDITOR || HACK_HEADER_GENERATOR
            if (Native_UClass.IsClassGroupName == null)
            {
                return false;
            }

            using (FStringUnsafe inGroupNameUnsafe = new FStringUnsafe(inGroupName))
            {
                return Native_UClass.IsClassGroupName(Address, ref inGroupNameUnsafe.Array);
            }
        }

        /// <summary>
        /// Prepends reference token stream with super class's stream.
        /// </summary>
        /// <param name="superClass">Super class to prepend stream with.</param>
        public void PrependStreamWithSuperClass(UClass superClass)
        {
            Native_UClass.PrependStreamWithSuperClass(Address, superClass == null ? IntPtr.Zero : superClass.Address);
        }

        /// <summary>
        /// Replace a native function in the  internal native function table
        /// </summary>
        /// <param name="inName">name of the function</param>
        /// <param name="inPointer">pointer to the function</param>
        /// <param name="addToFunctionRemapTable">For C++ hot-reloading, UFunctions are patched in a deferred manner and this should be true
        /// For script hot-reloading, script integrations may have a many to 1 mapping of UFunction to native pointer
        /// because dispatch is shared, so the C++ remap table does not work in this case, and this should be false</param>
        /// <returns>true if the function was found and replaced, false if it was not</returns>
        public bool ReplaceNativeFunction(FName inName, IntPtr inPointer, bool addToFunctionRemapTable)
        {
            // WITH_HOT_RELOAD
            if (Native_UClass.ReplaceNativeFunction == null)
            {
                return false;
            }

            return Native_UClass.ReplaceNativeFunction(Address, ref inName, inPointer, addToFunctionRemapTable);
        }

        /// <summary>
        /// If there are potentially multiple versions of this class (e.g. blueprint generated classes), this function will return the authoritative version, which should be used for references
        /// </summary>
        /// <returns>The version of this class that references should be stored to</returns>
        public UClass GetAuthoritativeClass()
        {
            return GCHelper.Find<UClass>(Native_UClass.GetAuthoritativeClass(Address));
        }

        /// <summary>
        /// Add a native function to the internal native function table
        /// </summary>
        /// <param name="name">name of the function</param>
        /// <param name="pointer">pointer to the function</param>
        public void AddNativeFunction(string name, IntPtr pointer)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            {
                Native_UClass.AddNativeFunction(Address, ref nameUnsafe.Array, pointer);
            }
        }

        /// <summary>
        /// Add a function to the function map
        /// </summary>
        /// <param name="function"></param>
        public void AddFunctionToFunctionMap(UFunction function, FName funcName)
        {
            Native_UClass.AddFunctionToFunctionMap(Address, function == null ? IntPtr.Zero : function.Address, ref funcName);
        }

        public UFunction FindFunctionByName(FName name, bool includeSuper = true)
        {
            return GCHelper.Find<UFunction>(Native_UClass.FindFunctionByName(Address, ref name, includeSuper));
        }

        /// <summary>
        /// Translates the hardcoded script config names (engine, editor, input and 
        /// game) to their global pendants and otherwise uses config(myini) name to
        /// look for a game specific implementation and creates one based on the
        /// default if it doesn't exist yet.
        /// </summary>
        /// <returns>name of the class specific ini file</returns>
        public string GetConfigName()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UClass.GetConfigName(Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        public UClass GetSuperClass()
        {
            return GCHelper.Find<UClass>(Native_UClass.GetSuperClass(Address));
        }

        public int GetDefaultsCount()
        {
            return Native_UClass.GetDefaultsCount(Address);
        }

        /// <summary>
        /// Get the default object from the class
        /// </summary>
        /// <param name="createIfNeeded">if true (default) then the CDO is created if it is NULL.</param>
        /// <returns>the CDO for this class</returns>
        public UObject GetDefaultObject(bool createIfNeeded = true)
        {
            return GCHelper.Find(Native_UClass.GetDefaultObject(Address, createIfNeeded));
        }

        /// <summary>
        /// Get the name of the CDO for the this class
        /// </summary>
        /// <returns>The name of the CDO</returns>
        public FName GetDefaultObjectName()
        {
            FName result;
            Native_UClass.GetDefaultObjectName(Address, out result);
            return result;
        }

        /// <summary>
        /// Searches for the default instanced object (often a component) by name
        /// </summary>
        /// <param name="toFind"></param>
        /// <returns></returns>
        public UObject GetDefaultSubobjectByName(FName toFind)
        {
            return GCHelper.Find(Native_UClass.GetDefaultSubobjectByName(Address, ref toFind));
        }

        /// <summary>
        /// Adds a new default instance map item
        /// </summary>
        /// <param name="newSubobject"></param>
        /// <param name="baseClass"></param>
        public void AddDefaultSubobject(UObject newSubobject, UClass baseClass)
        {
            Native_UClass.AddDefaultSubobject(Address, newSubobject == null ? IntPtr.Zero : newSubobject.Address, baseClass == null ? IntPtr.Zero : baseClass.Address);
        }

        /// <summary>
        /// Gets all default instanced objects (often components).
        /// </summary>
        /// <param name="outDefaultSubobjects">An array to be filled with default subobjects.</param>
        public void GetDefaultObjectSubobjects(out UObject[] outDefaultSubobjects)
        {
            using (TArrayUnsafe<UObject> defaultSubobjectsUnsafe = new TArrayUnsafe<UObject>())
            {
                Native_UClass.GetDefaultObjectSubobjects(Address, defaultSubobjectsUnsafe.Address);
                outDefaultSubobjects = defaultSubobjectsUnsafe.ToArray();
            }
        }

        /// <summary>
        /// Used to safely check whether the passed in flag is set.
        /// </summary>
        /// <param name="flagsToCheck">Class flag to check for</param>
        /// <returns>true if the passed in flag is set, false otherwise
        /// (including no flag passed in, unless the FlagsToCheck is CLASS_AllFlags)</returns>
        public bool HasAnyClassFlags(EClassFlags flagsToCheck)
        {
            return Native_UClass.HasAnyClassFlags(Address, flagsToCheck);
        }

        /// <summary>
        /// Used to safely check whether all of the passed in flags are set.
        /// </summary>
        /// <param name="flagsToCheck">Class flags to check for</param>
        /// <returns>true if all of the passed in flags are set (including no flags passed in), false otherwise</returns>
        public bool HasAllClassFlags(EClassFlags flagsToCheck)
        {
            return Native_UClass.HasAllClassFlags(Address, flagsToCheck);
        }

        /// <summary>
        /// Gets the class flags.
        /// </summary>
        /// <returns>The class flags.</returns>
        public EClassFlags GetClassFlags()
        {
            return Native_UClass.GetClassFlags(Address);
        }

        /// <summary>
        /// Used to safely check whether the passed in flag is set.
        /// </summary>
        /// <param name="flagToCheck">the cast flag to check for (value should be one of the EClassCastFlags enums)</param>
        /// <returns>true if the passed in flag is set, false otherwise
        /// (including no flag passed in)</returns>
        public bool HasAnyCastFlag(EClassCastFlags flagToCheck)
        {
            return Native_UClass.HasAnyCastFlag(Address, flagToCheck);
        }

        public bool HasAllCastFlags(EClassCastFlags flagToCheck)
        {
            return Native_UClass.HasAllCastFlags(Address, flagToCheck);
        }

        public string GetDescription()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UClass.GetDescription(Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Assembles the token stream for realtime garbage collection by combining the per class only
        /// token stream for each class in the class hierarchy. This is only done once and duplicate
        /// work is avoided by using an object flag.
        /// </summary>
        public void AssembleReferenceTokenStream(bool force = false)
        {
            Native_UClass.AssembleReferenceTokenStream(Address, force);
        }

        /// <summary>
        /// This will return whether or not this class implements the passed in class / interface
        /// </summary>
        /// <typeparam name="T">The IInterface derived type</typeparam>
        /// <returns></returns>
        public bool ImplementsInterface<T>() where T : IInterface
        {
            return ImplementsInterface(typeof(T));
        }

        /// <summary>
        /// This will return whether or not this class implements the passed in class / interface
        /// </summary>
        /// <param name="type">The IInterface derived type</param>
        /// <returns></returns>
        public bool ImplementsInterface(Type type)
        {
            return ImplementsInterface(UClass.GetInterfaceClass(type));
        }

        /// <summary>
        /// This will return whether or not this class implements the passed in class / interface 
        /// </summary>
        /// <param name="someInterface">the interface to check and see if this class implements it</param>
        /// <returns></returns>
        public bool ImplementsInterface(UClass someInterface)
        {
            if (someInterface != null && someInterface.HasAnyClassFlags(EClassFlags.Interface))
            {
                return Native_UClass.ImplementsInterface(Address, someInterface.Address);
            }
            return false;
        }

        /// <summary>
        /// Purges out the properties of this class in preparation for it to be regenerated
        /// </summary>
        /// <param name="recompilingOnLoad">true if we are recompiling on load</param>
        public void PurgeClass(bool recompilingOnLoad)
        {
            Native_UClass.PurgeClass(Address, recompilingOnLoad);
        }

        /// <summary>
        /// Finds the common base class that parents the two classes passed in.
        /// </summary>
        /// <param name="inClassA">the first class to find the common base for</param>
        /// <param name="inClassB">the second class to find the common base for</param>
        /// <returns>the common base class or NULL</returns>
        public static UClass FindCommonBase(UClass inClassA, UClass inClassB)
        {
            return GCHelper.Find<UClass>(Native_UClass.FindCommonBase(inClassA == null ? IntPtr.Zero : inClassA.Address, inClassB == null ? IntPtr.Zero : inClassB.Address));
        }

        /// <summary>
        /// Finds the common base class that parents the array of classes passed in.
        /// </summary>
        /// <param name="inClasses">the array of classes to find the common base for</param>
        /// <returns>the common base class or NULL</returns>
        public static UClass FindCommonBase(UClass[] inClasses)
        {
            using (TArrayUnsafe<UClass> inClassesUnsafe = new TArrayUnsafe<UClass>())
            {
                inClassesUnsafe.AddRange(inClasses);
                return GCHelper.Find<UClass>(Native_UClass.FindCommonBaseMany(inClassesUnsafe.Address));
            }
        }

        /// <summary>
        /// Determines if the specified function has been implemented in a Blueprint
        /// </summary>
        /// <param name="inFunctionName">The name of the function to test</param>
        /// <returns>True if the specified function exists and is implemented in a blueprint generated class</returns>
        public bool IsFunctionImplementedInBlueprint(FName inFunctionName)
        {
            return Native_UClass.IsFunctionImplementedInBlueprint(Address, ref inFunctionName);
        }

        /// <summary>
        /// Checks if the property exists on this class or a parent class.
        /// </summary>
        /// <param name="inProperty">The property to check if it is contained in this or a parent class.</param>
        /// <returns>True if the property exists on this or a parent class.</returns>
        public bool HasProperty(UProperty inProperty)
        {
            return Native_UClass.HasProperty(Address, inProperty == null ? IntPtr.Zero : inProperty.Address);
        }
    }
}

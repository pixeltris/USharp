using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // Using custom hotreloading instead of HotReload.cpp as we have some slightly different requirements.
    // - We update the editor for changes to UScriptStruct/UEnum in a similar way to UUserDefinedStruct/UUserDefinedEnum. 
    //   (HotReload.cpp doesn't currently update the editor for changes to enums/structs).
    //   - Nodes need updating for enums/structs and UDataTable instances need to be updated for structs.
    // - We use constant addresses where possible (enums / delegates). C++ hotreload creates new ones on each reload. Blueprint
    //   always uses constant addresses for all blueprint types (UBlueprintGeneratedClass,UUserDefinedStruct,etc).
    //   It would be ideal if we could extend constant addresses to structs / classes but this complicates recompiling blueprints 
    //   which inherit from our classes due to memory layout changes.

    // Due to some minor quirks / limitations in UUserDefinedStruct/UUserDefinedEnum/UBlueprintGeneratedClass we are using
    // UScriptStruct/UEnum/UClass instead. This means we can't have things like C# types which depend on BP types.
    // - For enums UUserDefinedEnum is restricted to ECppForm::Namespaced (checked in GenerateFullEnumName), ECppForm::EnumClass 
    //   is required to utilize UEnumProperty which can use non-byte enums which can be useful to have large enums.
    // - UClass is generally better instead of UBlueprintGenerated to avoid issues where our classes are recompiled as regular 
    //   blueprints / trashed unexpectedly. One solution would be to add a custom blueprint compiler but our classes would still be given 
    //   special blueprint treatment in many places which often isn't desirable. A drawback of not using UBlueprintGeneratedClass 
    //   is that UClass currently doesn't have support for custom input / custom net replication props. See mhutch's pull requests 
    //   for monoue. (UDynamicClass supports custom input so we could hack into using that temporarily).

    // TODO: Use ResolveClassAddress and other ResolveXXX functions instead of GetClassAddress?

    // NOTE: We are reversing our collections when creating properties/functions to get the correct order.
    //       This is required due to the setup of the Children linked list.

    public static partial class ManagedUnrealTypes
    {
        private static List<ManagedClass> classesToReinstance = new List<ManagedClass>();
        private static Dictionary<IntPtr, IntPtr> hotReloadedClasses = new Dictionary<IntPtr, IntPtr>();

        // When recreating types ctors shouldn't be called. It is better to check for an invalid ctor
        // call rather than having obscure errors due to the ctor being called too early.
        private static bool ctorsAvailable = false;

        private static int numChangedTypes = 0;
        public static bool SkipReinstance { get; private set; }
        public static bool SkipBroadcastHotReload { get; private set; }

        public static void Load()
        {
            // Clear NativeReflectionCached just in case it is holding onto old values.
            NativeReflectionCached.Clear();

            numChangedTypes = 0;
            SkipReinstance = false;
            SkipBroadcastHotReload = false;

            foreach (KeyValuePair<Type, USharpPathAttribute> type in UnrealTypes.Managed)
            {
                InitType(type.Key, type.Value);
            }
            BuildTypes();
        }

        public static void OnUnload()
        {
            // Redirect function pointers which will be destroyed when unloading the AppDomain
            foreach (ManagedClass managedClass in Classes.Values)
            {
                if (managedClass.Address != IntPtr.Zero)
                {
                    Native_USharpClass.Set_ManagedConstructor(managedClass.Address, IntPtr.Zero);
                    managedClass.SetFallbackInvokers();
                }
            }
        }

        private static bool GetCachedType(string path, out string hash, out IntPtr obj)
        {
            using (FStringUnsafe pathUnsafe = new FStringUnsafe(path))
            using (FStringUnsafe hashUnsafe = new FStringUnsafe())
            {
                obj = IntPtr.Zero;
                bool result = Native_ManagedUnrealType.GetType(ref pathUnsafe.Array, ref hashUnsafe.Array, ref obj);
                hash = hashUnsafe.Value;
                return result;
            }
        }

        private static void AddCachedType(string path, string hash, IntPtr obj)
        {
            using (FStringUnsafe pathUnsafe = new FStringUnsafe(path))
            using (FStringUnsafe hashUnsafe = new FStringUnsafe(hash))
            {
                Native_ManagedUnrealType.AddType(ref pathUnsafe.Array, ref hashUnsafe.Array, obj);
            }
        }

        private static void RemoveCachedType(string path)
        {
            using (FStringUnsafe pathUnsafe = new FStringUnsafe(path))
            {
                Native_ManagedUnrealType.RemoveType(ref pathUnsafe.Array);
            }
        }

        private static void BuildEnums()
        {
            foreach (ManagedEnum managedEnum in Enums.Values)
            {
                if (managedEnum.Changed)
                {
                    ManagedUnrealEnumInfo enumInfo = managedEnum.TypeInfo as ManagedUnrealEnumInfo;
                    IntPtr sharpEnum = managedEnum.Address;

                    Dictionary<FName, long> oldValues = new Dictionary<FName, long>();
                    int numOldValues = Native_UEnum.NumEnums(sharpEnum) - 1;// skip Max value
                    for (int i = 0; i < numOldValues; i++)
                    {
                        FName name;
                        Native_UEnum.GetNameByIndex(sharpEnum, i, out name);
                        oldValues[name] = Native_UEnum.GetValueByIndex(sharpEnum, i);
                    }

                    Dictionary<FName, long> values = new Dictionary<FName, long>();
                    foreach (ManagedUnrealEnumValueInfo enumValue in enumInfo.EnumValues)
                    {
                        // Get the full namespaced required by UUserDefinedEnum (EnumPath::EnumValueName)
                        using (FStringUnsafe nameUnsafe = new FStringUnsafe(enumValue.Name))
                        using (FStringUnsafe fullEnumNameUnsafe = new FStringUnsafe())
                        {
                            Native_UEnum.GenerateFullEnumName(sharpEnum, ref nameUnsafe.Array, ref fullEnumNameUnsafe.Array);
                            values[new FName(fullEnumNameUnsafe.Value)] = (long)enumValue.Value;
                        }
                    }

                    // UUserDefinedEnum uses ECppForm.Namespaced
                    using (TArrayUnsafe<FName> namesUnsafe = new TArrayUnsafe<FName>())
                    using (TArrayUnsafe<long> valuesUnsafe = new TArrayUnsafe<long>())
                    {
                        namesUnsafe.AddRange(values.Keys.ToArray());
                        valuesUnsafe.AddRange(values.Values.ToArray());
                        Native_UEnum.SetEnums(sharpEnum, namesUnsafe.Address, valuesUnsafe.Address, 
                            UEnum.ECppForm.EnumClass, true);
                    }
                    
                    if (FBuild.WithEditor)
                    {
                        SetAllMetaData(sharpEnum, enumInfo, UMeta.Target.Enum);

                        // Update the editor with the new enum values
                        using (TArrayUnsafe<FName> oldNamesUnsafe = new TArrayUnsafe<FName>())
                        using (TArrayUnsafe<long> oldValuesUnsafe = new TArrayUnsafe<long>())
                        {
                            oldNamesUnsafe.AddRange(oldValues.Keys.ToArray());
                            oldValuesUnsafe.AddRange(oldValues.Values.ToArray());
                            Native_SharpHotReloadUtils.UpdateEnum(sharpEnum, oldNamesUnsafe.Address, oldValuesUnsafe.Address, true);
                        }
                    }
                }
            }
        }

        private static void BuildStruct(ManagedStruct managedStruct)
        {
            IntPtr sharpStruct = managedStruct.Address;
            if (sharpStruct == IntPtr.Zero || !managedStruct.HasChanged)
            {
                return;
            }

            // Only useful flag should be EStructFlags.Atomic (otherwise just set flags back to 0)
            EStructFlags structFlags = managedStruct.TypeInfo.StructFlags;
            Native_UScriptStruct.Set_StructFlags(sharpStruct, structFlags);

            // InnerCompileStruct calls Struct->SetSuperStruct(EditorData->NativeBase) but NativeBase is never set

            // Create properties
            foreach (ManagedUnrealPropertyInfo propertyInfo in managedStruct.TypeInfo.Properties.Reverse<ManagedUnrealPropertyInfo>())
            {
                CreateProperty(sharpStruct, propertyInfo, true);
            }

            SetAllMetaData(sharpStruct, managedStruct.TypeInfo, UMeta.Target.Struct);

            Native_UField.Bind(sharpStruct);
            Native_UStruct.StaticLink(sharpStruct, true);
            managedStruct.Linked = true;

            if (managedStruct.TypeInfo.IsBlittable)
            {
                structFlags = Native_UScriptStruct.Get_StructFlags(sharpStruct);
                if (!structFlags.HasFlag(EStructFlags.IsPlainOldData))
                {
                    // If a struct contains another blittable struct it doesn't get flagged with POD.
                    // Either update our IsBlittable logic to do what the engine does or just validate
                    // the struct size is the same so that our BlittableTypeMarshaler still works.
                    // - Add the POD flag manually? Would also need to add NoDtor too which may be unexpected.
                    FMessage.Log("Unexpected Non-POD '" + NativeReflection.GetPathName(sharpStruct) + "'");
                    Debug.Assert(Marshal.SizeOf(managedStruct.Type) == NativeReflection.GetStructSize(sharpStruct),
                        "C# is treating a type as blittable but it isn't POD and the size doesn't match the native size '" +
                        NativeReflection.GetPathName(sharpStruct) + "'");
                }
            }
        }

        private static void BuildStructAndDependencies(ManagedStruct managedStruct,
            Dictionary<ManagedStruct, HashSet<ManagedStruct>> depends,
            HashSet<ManagedStruct> compiledStructs)
        {
            while (depends[managedStruct].Count > 0)
            {
                ManagedStruct dependency = depends[managedStruct].First();
                if (!compiledStructs.Contains(dependency))
                {
                    BuildStructAndDependencies(dependency, depends, compiledStructs);
                }
                depends[managedStruct].Remove(dependency);
            }
            BuildStruct(managedStruct);
            compiledStructs.Add(managedStruct);
        }

        private static void BuildStructs()
        {
            // Get all changed structs and build them in-order based on dependencies.
            // This assumes there can't be circular dependencies (if there are this will result in a stack overflow)

            Dictionary<string, ManagedStruct> structsByPath = new Dictionary<string, ManagedStruct>();
            foreach (ManagedStruct managedStruct in Structs.Values)
            {
                if (managedStruct.HasChanged)
                {
                    structsByPath[managedStruct.Path] = managedStruct;
                }
            }

            // Hold onto 3 type refs for struct dependency lookups (the type + up to 2 args)
            ManagedUnrealTypeInfoReference[] typeRefs = new ManagedUnrealTypeInfoReference[3];

            Dictionary<ManagedStruct, HashSet<ManagedStruct>> depends = new Dictionary<ManagedStruct, HashSet<ManagedStruct>>();
            foreach (ManagedStruct managedStruct in structsByPath.Values)
            {
                HashSet<ManagedStruct> dependsList = new HashSet<ManagedStruct>();
                depends.Add(managedStruct, dependsList);

                foreach (ManagedUnrealPropertyInfo propertyInfo in managedStruct.TypeInfo.Properties)
                {
                    // Look for a struct ref by either a direct direct reference or a generic arg (collections, fixed arrays)
                    typeRefs[0] = propertyInfo.Type;
                    typeRefs[1] = propertyInfo.GenericArgs.Count > 0 ? propertyInfo.GenericArgs[0] : null;
                    typeRefs[2] = propertyInfo.GenericArgs.Count > 1 ? propertyInfo.GenericArgs[1] : null;
                    foreach (ManagedUnrealTypeInfoReference typeRef in typeRefs)
                    {
                        ManagedStruct structRef;
                        if (typeRef != null && typeRef.TypeCode == EPropertyType.Struct &&
                            !string.IsNullOrEmpty(typeRef.Path) &&
                            structsByPath.TryGetValue(typeRef.Path, out structRef))
                        {
                            dependsList.Add(structRef);
                        }
                    }
                }
            }

            IntPtr blueprintsToRecompile = IntPtr.Zero;
            IntPtr bpStructsToRecompile = IntPtr.Zero;// UUserDefinedStructs to recompile
            if (FBuild.WithEditor)
            {
                // - FStructureEditorUtils::BroadcastPreChange() (mostly DataTable related)
                // - FUserDefinedStructureCompilerInner::ReplaceStructWithTempDuplicate
                using (TArrayUnsafe<IntPtr> oldSharpStructs = new TArrayUnsafe<IntPtr>())
                {
                    foreach(ManagedStruct managedStruct in depends.Keys)
                    {
                        if (managedStruct.OldAddress != IntPtr.Zero)
                        {
                            oldSharpStructs.Add(managedStruct.OldAddress);
                        }
                    }

                    using (var timing = HotReload.Timing.Create(HotReload.Timing.SharpHotReloadUtils_PreUpdateStructs))
                    {
                        Native_SharpHotReloadUtils.PreUpdateStructs(oldSharpStructs.Address, ref blueprintsToRecompile, ref bpStructsToRecompile);
                    }
                }
                Debug.Assert(blueprintsToRecompile != IntPtr.Zero && bpStructsToRecompile != IntPtr.Zero);                
            }

            HashSet<ManagedStruct> compiledStructs = new HashSet<ManagedStruct>();
            foreach (ManagedStruct managedStruct in depends.Keys)
            {
                if (!compiledStructs.Contains(managedStruct))
                {
                    BuildStructAndDependencies(managedStruct, depends, compiledStructs);
                }
            }

            if (FBuild.WithEditor)
            {
                // - Compile all UUserDefinedStructs which depend on our compiled structs
                // - Update blueprint pins etc
                // - FStructureEditorUtils::BroadcastPostChange()
                using (TArrayUnsafe<IntPtr> sharpChangedStructsOld = new TArrayUnsafe<IntPtr>())
                using (TArrayUnsafe<IntPtr> sharpChangedStructsNew = new TArrayUnsafe<IntPtr>())
                {
                    foreach (ManagedStruct compiledStruct in compiledStructs)
                    {
                        if (compiledStruct.OldAddress != IntPtr.Zero)
                        {
                            sharpChangedStructsOld.Add(compiledStruct.OldAddress);
                            sharpChangedStructsNew.Add(compiledStruct.Address);
                        }
                    }

                    using (var timing = HotReload.Timing.Create(HotReload.Timing.SharpHotReloadUtils_PostUpdateStructs))
                    {
                        Native_SharpHotReloadUtils.PostUpdateStructs(
                            sharpChangedStructsOld.Address, sharpChangedStructsNew.Address, blueprintsToRecompile, bpStructsToRecompile);
                    }
                }
            }
        }

        private static void ClassConstructor(ManagedClass managedClass, IntPtr objectInitializerPtr)
        {
            IntPtr sharpClass = managedClass.Address;
            if (sharpClass != IntPtr.Zero)
            {
                Debug.Assert(ctorsAvailable, "Ctor called before all C# classes have been created");

                FObjectInitializer objectInitializer = new FObjectInitializer(objectInitializerPtr);

                // Call the initializer if this isn't an interface and the initializer is overridden somewhere in the class hierarchy
                UObject obj = null;
                bool callInitializer = !managedClass.IsInterface && managedClass.TypeInfo.OverridesObjectInitializerHierarchical;
                if (callInitializer)
                {
                    GCHelper.ManagedObjectBeingInitialized = objectInitializer.ObjectAddress;
                    obj = objectInitializer.GetObj();
                    GCHelper.ManagedObjectBeingInitialized = IntPtr.Zero;
                }

                // Initialize C# members which can't be zero initialized (e.g. FText)
                // - Does this impact how the FObjectInitializer destructor is called (it will call FObjectInitializer::InitProperties). This could
                //   potentially conflict with our use of InitializeValue_InContainer and leak memory? Check how blueprint does this initialization
                IntPtr objAddress = objectInitializer.ObjectAddress;
                foreach (IntPtr prop in new NativeReflection.NativeFieldIterator(Runtime.Classes.UProperty, sharpClass, true))
                {
                    // Only initialize members defined in C#, the other properties will be initialized by the base native ctor
                    if (Native_UObjectBase.GetClass(Native_UField.GetOwnerClass(prop)) != Runtime.Classes.USharpClass)
                    {
                        break;
                    }
                    
                    // Only initialize the value if the property can't be zero initialized
                    if (!Native_UProperty.HasAnyPropertyFlags(prop, EPropertyFlags.ZeroConstructor))
                    {
                        Native_UProperty.InitializeValue_InContainer(prop, objAddress);
                    }
                }

                //EClassFlags oldClassFlags = Native_UClass.Get_ClassFlags(managedClass.Address);
                //Native_UClass.Set_ClassFlags(managedClass.Address, oldClassFlags & (~(EClassFlags.Intrinsic | EClassFlags.Native)));

                // Call the native parent class constructor. This needs to be called on the first known native class constructor rather
                // than the parent class constructor as the parent might be a managed class. If the parent is a managed class and we 
                // called the parent directly our Initialize(objectInitializer) would get called multiple times.
                Debug.Assert(managedClass.NativeParentClassConstructor != IntPtr.Zero);
                Native_UClass.Call_ClassConstructorDirectly(managedClass.NativeParentClassConstructor, objectInitializerPtr);

                //Native_UClass.Set_ClassFlags(managedClass.Address, oldClassFlags);

                if (callInitializer)
                {
                    // Call the managed object initializer
                    obj.Initialize(objectInitializer);
                }
            }
        }

        private static void BuildInterfaces()
        {
            BuildClassesInterfaces(Interfaces);
        }

        private static void BuildClasses()
        {
            BuildClassesInterfaces(Classes);
        }

        private static void BuildClassAndBaseHierarchical<T>(ManagedClass managedClass, Dictionary<Type, T> collection, 
            HashSet<ManagedClass> compiledClasses, Dictionary<string, ManagedClass> classesByPath) where T : ManagedClass
        {
            // If the class has been structurally modified create a reinstancer and reinstance.
            // If the class hasn't been changed create a reinstancer which will reinstance only if the CDO has changed.
            // If it is an interface and it hasn't been structurally modified the CDO shouldn't have changed so we can skip the reinstancer CDO check.

            if (compiledClasses.Contains(managedClass))
            {
                return;
            }

            ManagedUnrealTypeInfo classInfo = managedClass.TypeInfo;
            IntPtr sharpClass = managedClass.Address;

            if (!managedClass.IsInterface)
            {
                // Always update the class constructor as the C# constructor is trashed on hotreload
                Native_USharpClass.SetSharpClassConstructor(sharpClass, Marshal.GetFunctionPointerForDelegate(managedClass.ClassConstructor));
            }

            if (!managedClass.HasChanged)
            {
                // Classes which haven't changed structurally may still require reinstancing if the CDO code has changed.
                // The only real way of checking that is creating a new CDO and comparing it to the old. This might be a little
                // bit expensive so possibly think of a way to filter this in the future to speed up reload times.
                if (managedClass.TypeInfo.IsClass && managedClass.TypeInfo.OverridesObjectInitializerHierarchical)
                {
                    Debug.Assert(managedClass.OldAddress == IntPtr.Zero);
                    classesToReinstance.Add(managedClass);
                }
                return;
            }

            // Build the base type (if not built already)
            foreach (ManagedUnrealTypeInfoReference typeRef in managedClass.TypeInfo.BaseTypes)
            {
                ManagedClass baseType;
                if (typeRef.TypeCode == managedClass.TypeCode &&
                    classesByPath.TryGetValue(typeRef.Path, out baseType) &&
                    !compiledClasses.Contains(baseType))
                {
                    BuildClassAndBaseHierarchical(baseType, collection, compiledClasses, classesByPath);
                }
            }

            bool isInterface = false;
            UMeta.Target metadataTarget = UMeta.Target.Class;
            if (managedClass.TypeCode == EPropertyType.Interface)
            {
                isInterface = true;
                metadataTarget = UMeta.Target.Interface;
            }

            {
                // If this class / interface implements interfaces create an interfaces array and link them up
                TArrayUnsafe<FImplementedInterface> implementedInterfaces = null;
                if (classInfo.AdditionalFlags.HasFlag(ManagedUnrealTypeInfoFlags.ImplementsInterface))
                {
                    implementedInterfaces = new TArrayUnsafe<FImplementedInterface>();
                }

                IntPtr parentClass = IntPtr.Zero;
                foreach (ManagedUnrealTypeInfoReference typeRef in classInfo.BaseTypes)
                {
                    if (typeRef.TypeCode == EPropertyType.Interface)
                    {
                        // TODO: Add UInterface.GetInterfaceAddress()?
                        IntPtr interfaceAddress = UClass.GetClassAddress(typeRef.Path);

                        // Is 0 the correct pointer offset to use for the vtable?
                        implementedInterfaces.Add(new FImplementedInterface(interfaceAddress, 0, true));
                    }
                    else
                    {
                        parentClass = UClass.GetClassAddress(typeRef.Path);
                    }
                }
                if (parentClass == IntPtr.Zero)
                {
                    if (isInterface)
                    {
                        parentClass = Runtime.Classes.UInterface;
                    }
                    else
                    {
                        parentClass = Runtime.Classes.UObject;
                    }
                }
                SetClassParent(sharpClass, parentClass);

                // Resolve the native parent class now that the parent class is set up
                managedClass.ResolveNativeParentClass();
                if (!isInterface)
                {
                    Native_USharpClass.UpdateNativeParentConstructor(sharpClass);
                }

                // Clean up the implemented interfaces array
                if (implementedInterfaces != null)
                {
                    Native_UClass.Set_Interfaces(sharpClass, implementedInterfaces.Address);

                    implementedInterfaces.Dispose();
                    implementedInterfaces = null;
                }

                // Inherit the parent class flags if they aren't already set from TypeInfo.ClassFlags
                EClassFlags classFlags = (Native_UClass.GetClassFlags(parentClass) & EClassFlags.ScriptInherit);
                classFlags |= managedClass.TypeInfo.ClassFlags;

                // If using UClass EClassFlags.Native is required (otherwise functions are broken on interfaces and runtime errors on classes)
                // If using UBlueprintGeneratedClass no special EClassFlags are required.
                classFlags |= EClassFlags.Native;

                Native_UClass.Set_ClassFlags(sharpClass, classFlags);

                // Create functions
                foreach (ManagedUnrealFunctionInfo functionInfo in managedClass.TypeInfo.Functions.Reverse<ManagedUnrealFunctionInfo>())
                {
                    IntPtr function = CreateFunction(sharpClass, parentClass, functionInfo, managedClass.FunctionInvoker);
                    if (function != IntPtr.Zero)
                    {
                        managedClass.AddInvoker(functionInfo, function);
                        SetAllMetaData(function, functionInfo, UMeta.Target.Function);
                    }
                }

                if (!isInterface)
                {
                    // Create properties
                    foreach (ManagedUnrealPropertyInfo propertyInfo in managedClass.TypeInfo.Properties.Reverse<ManagedUnrealPropertyInfo>())
                    {
                        CreateProperty(sharpClass, propertyInfo, true);
                    }
                }

                SetAllMetaData(sharpClass, managedClass.TypeInfo, metadataTarget);

                Native_UField.Bind(sharpClass);
                Native_UStruct.StaticLink(sharpClass, true);
                Native_UClass.AssembleReferenceTokenStream(sharpClass, true);
                managedClass.Linked = true;
            }
            
            if (managedClass.OldAddress != IntPtr.Zero)
            {
                classesToReinstance.Add(managedClass);
            }

            compiledClasses.Add(managedClass);
        }

        private static void BuildClassesInterfaces<T>(Dictionary<Type, T> collection) where T : ManagedClass
        {
            HashSet<ManagedClass> compiledClasses = new HashSet<ManagedClass>();

            // Get all of the types by path for lookup when needing to build a base managed type
            Dictionary<string, ManagedClass> classesByPath = new Dictionary<string, ManagedClass>();
            List<ManagedClass> unchangedClasses = new List<ManagedClass>();
            foreach (ManagedClass managedClass in collection.Values)
            {
                if (managedClass.HasChanged)
                {
                    classesByPath[managedClass.Path] = managedClass;
                }
                else
                {
                    unchangedClasses.Add(managedClass);
                }
            }

            foreach (ManagedClass managedClass in collection.Values)
            {
                BuildClassAndBaseHierarchical(managedClass, collection, compiledClasses, classesByPath);
            }

            // For unchanged classes we need to restore the old managed functions
            foreach (ManagedClass managedClass in unchangedClasses)
            {
                if (managedClass.NativeParentClass == IntPtr.Zero)
                {
                    managedClass.ResolveNativeParentClass();
                }

                if (managedClass.TypeInfo.Functions.Count > 0)
                {
                    IntPtr invokerAddress = Marshal.GetFunctionPointerForDelegate(managedClass.FunctionInvoker);
                    foreach (ManagedUnrealFunctionInfo functionInfo in managedClass.TypeInfo.Functions)
                    {
                        using (FStringUnsafe functionNameUnsafe = new FStringUnsafe(functionInfo.GetName()))
                        {
                            IntPtr function = Native_USharpClass.SetFunctionInvoker(managedClass.Address, ref functionNameUnsafe.Array, invokerAddress);
                            if (function != IntPtr.Zero)
                            {
                                managedClass.AddInvoker(functionInfo, function);
                            }
                        }
                    }
                }
            }
        }

        private static void BuildDelegates()
        {
            using (TArrayUnsafe<IntPtr> changedDelegatesUnsafe = new TArrayUnsafe<IntPtr>())
            {
                foreach (ManagedDelegateSignature managedDelegate in DelegateSignatures.Values)
                {
                    IntPtr function = managedDelegate.Address;
                    if (function == IntPtr.Zero)
                    {
                        continue;
                    }

                    if (managedDelegate.HasChanged)
                    {
                        changedDelegatesUnsafe.Add(function);
                    }

                    if (managedDelegate.Changed)
                    {
                        if (managedDelegate.Linked)
                        {
                            CleanAndSanitizeDelegate(managedDelegate.Address);
                        }

                        // Create the properties for the delegate
                        {
                            ManagedUnrealFunctionInfo functionInfo = managedDelegate.TypeInfo.Functions[0];
                            EFunctionFlags functionFlags = functionInfo.Flags;

                            // Add ReturnProp first so that it is visually last (Prop->Next = Children; Children = Prop)
                            if (functionInfo.ReturnProp != null)
                            {
                                CreateFunctionParam(functionInfo, function, ref functionFlags, functionInfo.ReturnProp);
                            }
                            foreach (ManagedUnrealPropertyInfo propertyInfo in functionInfo.Params.Reverse<ManagedUnrealPropertyInfo>())
                            {
                                CreateFunctionParam(functionInfo, function, ref functionFlags, propertyInfo);
                            }

                            Native_UFunction.Set_FunctionFlags(function, functionFlags);
                        }

                        SetAllMetaData(function, managedDelegate.TypeInfo, UMeta.Target.Delegate);

                        Native_UField.Bind(function);
                        Native_UStruct.StaticLink(function, true);
                        managedDelegate.Linked = true;
                    }
                    else if (managedDelegate.ChangedByRef)
                    {
                        Native_UField.Bind(function);
                        Native_UStruct.StaticLink(function, true);
                        managedDelegate.Linked = true;
                    }
                }

                if (FBuild.WithEditor)
                {
                    Native_SharpHotReloadUtils.UpdateDelegates(changedDelegatesUnsafe.Address);
                }
            }
        }

        private static void CleanAndSanitizeDelegate(IntPtr sharpFunction)
        {
            string transientDelegateString = "TRASHDELEGATE_" + NativeReflection.GetFName(sharpFunction).ToString();
            FName transientDelegateName = NativeReflection.MakeUniqueObjectName(NativeReflection.GetTransientPackage(),
                Runtime.Classes.UDelegateFunction, new FName(transientDelegateString));
            IntPtr transientDelegate = NativeReflection.NewObject(NativeReflection.GetTransientPackage(),
                Runtime.Classes.UDelegateFunction, transientDelegateName, EObjectFlags.Public | EObjectFlags.Transient);

            // Purge all subobjects (properties, functions, params) of the struct, as they will be regenerated
            IntPtr[] delegateSubObjects = NativeReflection.GetObjectsWithOuter(sharpFunction, false);

            // Also possibly want NonTransactional / DoNotDirty? (at least DoNotDirty?)
            ERenameFlags renFlags = ERenameFlags.DontCreateRedirectors;

            foreach (IntPtr subObj in delegateSubObjects)
            {
                using (FStringUnsafe subObjNameUnsafe = new FStringUnsafe(NativeReflection.GetFName(subObj).ToString()))
                {
                    Native_UObject.Rename(subObj, ref subObjNameUnsafe.Array, transientDelegate, renFlags);
                }
            }

            // Purge the delegate to get it back to a "base" state
            Native_UStruct.SetSuperStruct(sharpFunction, IntPtr.Zero);
            Native_UStruct.Set_Children(sharpFunction, IntPtr.Zero);
            new TArrayUnsafeRef<byte>(Native_UStruct.Get_Script(sharpFunction)).Clear();// Should already be empty
            Native_UStruct.Set_MinAlignment(sharpFunction, 0);
            Native_UStruct.Set_RefLink(sharpFunction, IntPtr.Zero);
            Native_UStruct.Set_PropertyLink(sharpFunction, IntPtr.Zero);
            Native_UStruct.Set_DestructorLink(sharpFunction, IntPtr.Zero);
            new TArrayUnsafeRef<UObject>(Native_UStruct.Get_ScriptObjectReferences(sharpFunction)).Clear();
            Native_UFunction.Set_NumParms(sharpFunction, 0);
            Native_UFunction.Set_ParmsSize(sharpFunction, 0);
            Native_UFunction.Set_ReturnValueOffset(sharpFunction, ushort.MaxValue);// State no return param
            Native_UFunction.Set_RPCId(sharpFunction, 0);
            Native_UFunction.Set_RPCResponseId(sharpFunction, 0);
            Native_UFunction.Set_FirstPropertyToInit(sharpFunction, IntPtr.Zero);
            Native_UFunction.Set_FunctionFlags(sharpFunction, (EFunctionFlags)0);
            Native_UFunction.SetNativeFunc(sharpFunction, IntPtr.Zero);
        }

        private static void BuildTypes()
        {
            hotReloadedClasses.Clear();
            classesToReinstance.Clear();
            ctorsAvailable = false;

            Dictionary<Type, ManagedTypeBase> allTypes = new Dictionary<Type, ManagedTypeBase>();
            Dictionary<Type, ManagedTypeBase> changedTypes = new Dictionary<Type, ManagedTypeBase>();
            Dictionary<Type, ManagedTypeBase> unchangedTypes = new Dictionary<Type, ManagedTypeBase>();
            
            CollectTypes(Classes, allTypes, changedTypes, unchangedTypes);
            CollectTypes(Interfaces, allTypes, changedTypes, unchangedTypes);
            CollectTypes(Structs, allTypes, changedTypes, unchangedTypes);
            CollectTypes(Enums, allTypes, changedTypes, unchangedTypes);
            CollectTypes(DelegateSignatures, allTypes, changedTypes, unchangedTypes);

            Dictionary<string, ManagedTypeBase> allTypesByPath = new Dictionary<string, ManagedTypeBase>();
            foreach (ManagedTypeBase managedType in allTypes.Values)
            {
                allTypesByPath[managedType.Path] = managedType;
            }

            ChainChangedDependencies(allTypesByPath, allTypes, changedTypes, unchangedTypes);

            if ((numChangedTypes == 0 && HotReload.MinimalReload) || !FBuild.WithEditor)
            {
                SkipReinstance = true;
                SkipBroadcastHotReload = true;
            }

            // Recreate classes/structs for hotreload rename the old versions to HOTRELOADED_XXX. If we don't do this then
            // blueprints which inherit from our classes will become corrupted due to the changed memory layout.
            foreach (ManagedTypeBase managedType in allTypes.Values)
            {
                IntPtr unrealClass = IntPtr.Zero;

                if (managedType.HasChanged && managedType.Linked &&
                    managedType.OldAddress == IntPtr.Zero && managedType.Address != IntPtr.Zero)
                {
                    switch (managedType.TypeInfo.TypeCode)
                    {
                        case EPropertyType.Struct:
                            unrealClass = Runtime.Classes.USharpStruct;
                            break;
                        case EPropertyType.Interface:
                            unrealClass = Runtime.Classes.UInterface;
                            break;
                        case EPropertyType.Object:
                            unrealClass = Runtime.Classes.USharpClass;
                            break;
                    }
                }

                if (unrealClass != IntPtr.Zero)
                {
                    IntPtr oldAddress = managedType.Address;

                    // Move the old type to the transient package
                    if (managedType.TypeInfo.IsStruct)
                    {
                        // Copy of UObjectBase.cpp FindExistingStructOrEnumIfHotReload

                        // This will hide the type and allow it to be GCed
                        Native_UObjectBaseUtility.ClearFlags(oldAddress, EObjectFlags.Standalone | EObjectFlags.Public);
                        Native_UObjectBaseUtility.RemoveFromRoot(oldAddress);

                        FName oldRename = NativeReflection.MakeUniqueObjectName(NativeReflection.GetTransientPackage(),
                            Native_UObjectBase.GetClass(oldAddress), new FName("USharpHotReloaded_" + NativeReflection.GetFName(oldAddress)));
                        using (FStringUnsafe oldRenameUnsafe = new FStringUnsafe(oldRename.ToString()))
                        {
                            Native_UObject.Rename(oldAddress, ref oldRenameUnsafe.Array, NativeReflection.GetTransientPackage(), ERenameFlags.None);
                        }

                        // Remove BlueprintType to hide the old struct from blueprint
                        UMeta.RemoveMetaData(oldAddress, MDStruct.BlueprintType);
                    }
                    else
                    {
                        // Copy of UObjectBase.cpp UClassCompiledInDefer

                        Native_UObjectBaseUtility.RemoveFromRoot(oldAddress);

                        // This will hide the type and allow it to be GCed
                        Native_UObjectBaseUtility.ClearFlags(oldAddress, EObjectFlags.Standalone | EObjectFlags.Public);

                        IntPtr defaultObject = Native_UClass.GetDefaultObject(oldAddress, false);
                        if (defaultObject != IntPtr.Zero)
                        {
                            Native_UObjectBaseUtility.RemoveFromRoot(defaultObject);
                            Native_UObjectBaseUtility.ClearFlags(defaultObject, EObjectFlags.Standalone | EObjectFlags.Public);
                        }

                        FName oldClassRename = NativeReflection.MakeUniqueObjectName(NativeReflection.GetTransientPackage(),
                            Native_UObjectBase.GetClass(oldAddress), new FName("USharpHotReloaded_" + NativeReflection.GetFName(oldAddress)));
                        using (FStringUnsafe oldClassRenameUnsafe = new FStringUnsafe(oldClassRename.ToString()))
                        {
                            Native_UObject.Rename(oldAddress, ref oldClassRenameUnsafe.Array, NativeReflection.GetTransientPackage(), ERenameFlags.None);
                        }

                        Native_UObjectBaseUtility.SetFlags(oldAddress, EObjectFlags.Transient);

                        // Root will be removed once it has been reinstanced (FHotReloadClassReinstancer::SetupNewClassReinstancing)
                        Native_UObjectBaseUtility.AddToRoot(oldAddress);
                    }

                    Debug.Assert(managedType.OldAddress == IntPtr.Zero);
                    managedType.OldAddress = oldAddress;

                    EObjectFlags flags = EObjectFlags.Public | EObjectFlags.Standalone;
                    managedType.Address = NativeReflection.NewObject(managedType.Package, unrealClass,
                        new FName(managedType.Name), flags);

                    if (managedType.TypeInfo.IsStruct)
                    {
                        Native_USharpStruct.CreateGuid(managedType.Address);
                    }
                    else
                    {
                        hotReloadedClasses[oldAddress] = managedType.Address;
                    }

                    AddCachedType(managedType.TypeInfo.Path, managedType.TypeInfo.Hash, managedType.Address);
                }

                if (managedType.Address != IntPtr.Zero &&
                    (managedType.TypeInfo.IsClass || managedType.TypeInfo.IsInterface))
                {
                    // Register classes with UClass so that GCHelper.Find resolves properly when creating the CDO.
                    UClass.RegisterManagedClass(managedType.Address, managedType.Type);
                }
            }
            
            if (hotReloadedClasses.Count > 0)
            {
                UpdateClassReferences(allTypes);
                hotReloadedClasses.Clear();
            }

            // 1) Update enums first as they have no dependencies
            // 2) Update structs (in-order)
            // 3) Update interfaces/classes (in-order (based on inheritance))
            // 4) Update delegates (any order should be fine?)

            BuildEnums();
            BuildStructs();
            BuildInterfaces();
            BuildClasses();
            BuildDelegates();

            // Call LoadNativeTypeMethod on all types (unless lazy loading is enabled and the cctor hasn't been 
            // called in which case we will lazy load that info when the type is first accessed).
            foreach (Type type in allTypes.Keys)
            {
                OnTypeRegistered(type);

                bool lazyLoad = UnrealTypes.LazyLoadingEnabled && !UnrealTypes.HasCCtorBeenCalled(type);
                if (!lazyLoad)
                {
                    Type targetType = type;

                    // If this is an interface get the default implementation type which will hold the loader method
                    if (type.IsInterface)
                    {
                        USharpPathAttribute attribute = type.GetCustomAttribute<USharpPathAttribute>(false);
                        if (attribute != null && attribute.InterfaceImpl != null)
                        {
                            targetType = attribute.InterfaceImpl;
                        }
                    }

                    MethodInfo method = targetType.GetMethod(CodeGeneratorSettings.LoadNativeTypeMethodName,
                        BindingFlags.Static | BindingFlags.NonPublic);

                    if (method != null)
                    {
                        method.Invoke(null, null);
                    }
                }
            }

            ctorsAvailable = true;

            // Create the CDO for classes after all classes have been initialized as classes may reference other classes.
            foreach (ManagedClass managedClass in Classes.Values)
            {
                CreateCDO(managedClass);
            }
            // Probably not needed
            foreach (ManagedClass managedClass in Interfaces.Values)
            {
                CreateCDO(managedClass);
            }

            if (!SkipReinstance)
            {
                using (var timing = HotReload.Timing.Create(HotReload.Timing.ManagedUnrealTypes_ReinstanceClasses))
                {
                    // Reinstance any classes which require reinstancing
                    foreach (ManagedClass managedClass in classesToReinstance)
                    {
                        IntPtr classReinstancer = IntPtr.Zero;

                        classReinstancer = Native_SharpHotReloadUtils.CreateClassReinstancer(
                            managedClass.Address, managedClass.OldAddress != IntPtr.Zero ? managedClass.OldAddress : managedClass.Address);
                        Debug.Assert(classReinstancer != IntPtr.Zero);

                        // This will reinstance objects of this class (and delete the class reinstancer).
                        Native_SharpHotReloadUtils.ReinstanceClass(classReinstancer);
                    }
                }

                using (var timing = HotReload.Timing.Create(HotReload.Timing.SharpHotReloadUtils_FinalizeClasses))
                {
                    Native_SharpHotReloadUtils.FinalizeClasses();
                }
            }
            classesToReinstance.Clear();

            // Clear the "Changed" state
            foreach (ManagedTypeBase managedType in allTypes.Values)
            {
                managedType.Changed = false;
                managedType.ChangedByRef = false;
            }

            // Clear any temporary metadata which was created which isn't needed anymore
            ClearTypeMetaData();
        }

        private static void CreateCDO(ManagedClass managedClass)
        {
            // HACK: We aren't using defered type building like native code, therefore we don't make calls out to
            //       NotifyRegistrationEvent to build the type when ready. CompiledIn expects NotifyRegistrationEvent
            //       to be called an asserts everyting is set up in order. Temporarily remove CompiledIn to skip these checks.
            // TODO: Use the proper defering way of loading types. This is important anyway as our classes may depend on other
            //       classes which haven't loaded yet.
            IntPtr package = Native_UObjectBaseUtility.GetOutermost(managedClass.Address);
            bool hasCompiledIn = Native_UPackage.HasAnyPackageFlags(package, EPackageFlags.CompiledIn);
            if (hasCompiledIn)
            {
                Native_UPackage.ClearPackageFlags(package, EPackageFlags.CompiledIn);
            }

            Native_UClass.GetDefaultObject(managedClass.Address, true);

            if (hasCompiledIn)
            {
                Native_UPackage.SetPackageFlags(package, EPackageFlags.CompiledIn);
            }
        }

        private static void UpdateClassReferences(Dictionary<Type, ManagedTypeBase> allTypes)
        {
            foreach (ManagedTypeBase managedType in allTypes.Values)
            {
                if (managedType.Address != IntPtr.Zero)
                {
                    UpdateClassReferences(managedType.Address);
                }
            }
        }

        private static void UpdateClassReferences(IntPtr field)
        {
            if (Native_UObjectBaseUtility.IsA(field, Runtime.Classes.UClass) ||
                Native_UObjectBaseUtility.IsA(field, Runtime.Classes.UScriptStruct) ||
                Native_UObjectBaseUtility.IsA(field, Runtime.Classes.UFunction))
            {
                IntPtr child = Native_UStruct.Get_Children(field);
                while (child != IntPtr.Zero)
                {
                    UpdateClassReferences(child);
                    child = Native_UField.Get_Next(child);
                }
            }
            else if (Native_UObjectBaseUtility.IsA(field, Runtime.Classes.UProperty))
            {
                EPropertyType propertyType = NativeReflection.GetPropertyType(field);
                switch (propertyType)
                {
                    case EPropertyType.SoftClass:
                        {
                            IntPtr newClass;
                            if (hotReloadedClasses.TryGetValue(Native_USoftClassProperty.Get_MetaClass(field), out newClass))
                            {
                                Native_USoftClassProperty.Set_MetaClass(field, newClass);
                            }
                        }
                        break;

                    case EPropertyType.Class:
                        {
                            IntPtr newClass;
                            if (hotReloadedClasses.TryGetValue(Native_UClassProperty.Get_MetaClass(field), out newClass))
                            {
                                Native_UClassProperty.Set_MetaClass(field, newClass);
                            }
                        }
                        break;

                    case EPropertyType.Interface:
                        {
                            IntPtr newClass;
                            if (hotReloadedClasses.TryGetValue(Native_UInterfaceProperty.Get_InterfaceClass(field), out newClass))
                            {
                                Native_UInterfaceProperty.Set_InterfaceClass(field, newClass);
                            }
                        }
                        break;

                    case EPropertyType.SoftObject:
                    case EPropertyType.LazyObject:
                    case EPropertyType.WeakObject:
                    case EPropertyType.Object:
                        {
                            IntPtr newClass;
                            if (hotReloadedClasses.TryGetValue(Native_UObjectPropertyBase.Get_PropertyClass(field), out newClass))
                            {
                                Native_UObjectPropertyBase.Set_PropertyClass(field, newClass);
                            }
                        }
                        break;
                }
            }
        }

        private static void SetClassParent(IntPtr sharpClass, IntPtr parentClass)
        {
            IntPtr classWithin = Native_UClass.Get_ClassWithin(parentClass);
            if (classWithin == IntPtr.Zero)
            {
                classWithin = Runtime.Classes.UObject;
            }

            FName classConfigName;
            if (Native_UObjectBaseUtility.IsNative(sharpClass))
            {
                // C++ uses ClassToClean->StaticConfigName() which due to being a static method should
                // return UObject::StaticConfigName() which doesn't seem too useful
                Native_UClass.Get_ClassConfigName(sharpClass, out classConfigName);
            }
            else
            {
                Native_UClass.Get_ClassConfigName(parentClass, out classConfigName);
            }

            // Set properties we need to regenerate the class with
            Native_UStruct.Set_PropertyLink(sharpClass, Native_UStruct.Get_PropertyLink(parentClass));
            Native_UStruct.SetSuperStruct(sharpClass, parentClass);
            Native_UClass.Set_ClassWithin(sharpClass, classWithin);
            Native_UClass.Set_ClassConfigName(sharpClass, ref classConfigName);
        }

        private static IntPtr CreateFunction(IntPtr outer, IntPtr parentClass, ManagedUnrealFunctionInfo functionInfo,
            UFunction.FuncInvokerNative funcInvoker)
        {
            // NOTE: We need EObjectFlags.MarkAsNative to allow packages to include this as an import. Imports are an important
            //       part of package saving to ensure our referenced fields are serialized properly. Import gathering uses a 
            //       Obj->IsNative() check for import inclusion (see FArchiveSaveTagImports::operator<<)
            // NOTE: EObjectFlags.MarkAsNative will likely stop our classes from being GCed on hotreload. Unmark them on hotreload?
            EObjectFlags objectFlags = EObjectFlags.Public | EObjectFlags.Transient | EObjectFlags.MarkAsNative;
            IntPtr function = NativeReflection.NewObject(outer, Runtime.Classes.UFunction, new FName(functionInfo.GetName()), objectFlags);
            
            IntPtr parentFunction = IntPtr.Zero;

            // We want EFunctionFlags.Native so that UObject::CallFunction calls our invoker func with the desired params.
            EFunctionFlags functionFlags = EFunctionFlags.Native;

            functionFlags |= functionInfo.Flags;
            functionFlags |= EFunctionFlags.Public;

            if (functionInfo.IsVirtual)
            {
                Debug.Assert(!functionInfo.IsOverride);
            }
            else if (functionInfo.IsOverride)
            {
                Debug.Assert(!functionInfo.IsVirtual);
            }

            // Flags to inherit from the parent function. (Also see what BP copies from the parent on compile):
            // - Engine\Source\Editor\Kismet\Private\BlueprintCompilationManager.cpp (FBlueprintCompilationManagerImpl::FastGenerateSkeletonClass)
            // - Engine\Source\Editor\KismetCompiler\Private\KismetCompiler.cpp (FKismetCompilerContext::PrecompileFunction)
            EFunctionFlags funcInherit = 
                EFunctionFlags.NetFuncFlags | EFunctionFlags.FuncInherit | EFunctionFlags.AccessSpecifiers;

            if (functionInfo.AdditionalFlags.HasFlag(ManagedUnrealFunctionFlags.InterfaceImplementation))
            {
                // Const seems to be required here otherwise two versions of the same event appear in the
                // function graph with different targets (one is the interface, one is the class itself). When
                // we provide const it just shows the interface event (with the correct interface icon).
                // - Does adding const mess up anything? Const states "function can be called from blueprint code, 
                //   and only reads state (never writes)".
                functionFlags |= EFunctionFlags.Const;

                // Setting the super function here doesn't seem to be required.
                // - C++ NEVER sets the super function for implemented interfaces.
                // - Blueprint ALWAYS sets the super func for implemented interfaces.
                // - It seems to have no impact regardless of whether or not we set it.
                //
                /*FName functionName = new FName(functionInfo.GetName());
                IntPtr interfaceFunction = IntPtr.Zero;
                var implementedInterfaces = new TArrayUnsafeRef<FImplementedInterface>(
                    Native_UClass.Get_InterfacesRef(outer));
                foreach (FImplementedInterface implementedInterface in implementedInterfaces)
                {
                    interfaceFunction = Native_UClass.FindFunctionByName(implementedInterface.InterfaceClassAddress, ref functionName, false);
                    if (interfaceFunction != IntPtr.Zero)
                    {
                        break;
                    }
                }
                
                Debug.Assert(interfaceFunction != IntPtr.Zero);
                
                Native_UStruct.SetSuperStruct(function, interfaceFunction);
                EFunctionFlags parentFunctionFlags = Native_UFunction.Get_FunctionFlags(interfaceFunction);
                functionFlags |= (parentFunctionFlags & funcInherit);*/
            }
            else if (functionInfo.IsOverride)
            {
                FName functionName = new FName(functionInfo.GetName());
                parentFunction = Native_UClass.FindFunctionByName(parentClass, ref functionName, true);
                Debug.Assert(parentFunction != IntPtr.Zero, "Couldn't find parent function for override '" + functionInfo.Path + "'");

                Native_UStruct.SetSuperStruct(function, parentFunction);
                EFunctionFlags parentFunctionFlags = Native_UFunction.Get_FunctionFlags(parentFunction);                
                functionFlags |= (parentFunctionFlags & funcInherit);
            }

            // Add ReturnProp first so that it is visually last (Prop->Next = Children; Children = Prop)
            if (functionInfo.ReturnProp != null)
            {
                CreateFunctionParam(functionInfo, function, ref functionFlags, functionInfo.ReturnProp);
            }
            foreach (ManagedUnrealPropertyInfo propertyInfo in functionInfo.Params.Reverse<ManagedUnrealPropertyInfo>())
            {
                CreateFunctionParam(functionInfo, function, ref functionFlags, propertyInfo);
            }

            Native_UFunction.Set_FunctionFlags(function, functionFlags);

            // Setting up the function address must come before Bind/StaticLink.
            // UFunction::Bind will look up the function address and use it to assign UFunction::Func
            {
                IntPtr funcInvokerAddress = Marshal.GetFunctionPointerForDelegate(funcInvoker);
                using (FStringUnsafe nameUnsafe = new FStringUnsafe(functionInfo.GetName()))
                {
                    // This is the same as FNativeFunctionRegistrar::RegisterFunction()
                    Native_UClass.AddNativeFunction(outer, ref nameUnsafe.Array, funcInvokerAddress);
                }
                FName functionFName = NativeReflection.GetFName(function);
                Native_UClass.AddFunctionToFunctionMap(outer, function, ref functionFName);
            }

            Native_UField.Bind(function);
            Native_UStruct.StaticLink(function, true);

            Native_UField.Set_Next(function, Native_UStruct.Get_Children(outer));
            Native_UStruct.Set_Children(outer, function);

            if (functionInfo.IsOverride && parentFunction != IntPtr.Zero)
            {
                FixupFunctionOverrideSignature(function, parentFunction);
            }

            return function;
        }

        /// <summary>
        /// HACK: We are currently treating params with ConstParam as regular params (non ref). However this means we lose
        /// constness information is required to produce a compatible function signature with the native function.
        /// To rectify this look for all native params with ConstParm|ReferenceParm and copy the ref/out/const info if so.
        /// </summary>
        private static void FixupFunctionOverrideSignature(IntPtr function, IntPtr parentFunction)
        {
            // This is a somewhat copy of UFunction::IsSignatureCompatibleWith but we are fixing up the flags instead

            // Run thru the parameter property chains to compare each property
            var iteratorA = new NativeReflection.NativeFieldIterator(Runtime.Classes.UProperty, function).GetEnumerator();
            var iteratorB = new NativeReflection.NativeFieldIterator(Runtime.Classes.UProperty, parentFunction).GetEnumerator();

            EPropertyFlags constRef = EPropertyFlags.ConstParm | EPropertyFlags.ReferenceParm;
            EPropertyFlags constRefOut = EPropertyFlags.ConstParm | EPropertyFlags.ReferenceParm | EPropertyFlags.OutParm;

            while (iteratorA.Current != IntPtr.Zero && (Native_UProperty.Get_PropertyFlags(iteratorA.Current).HasFlag(EPropertyFlags.Parm)))
            {
                if (iteratorB.Current != IntPtr.Zero && (Native_UProperty.Get_PropertyFlags(iteratorB.Current).HasFlag(EPropertyFlags.Parm)))
                {
                    // Compare the two properties to make sure their types are identical
                    // Note: currently this requires both to be strictly identical and wouldn't allow functions that differ only by how derived a class is,
                    // which might be desirable when binding delegates, assuming there is directionality in the SignatureIsCompatibleWith call
                    IntPtr propA = iteratorA.Current;
                    IntPtr propB = iteratorB.Current;

                    // Skip ArePropertiesTheSame() call, assume they are the same as this isn't important here

                    //EPropertyFlags flagsA = Native_UProperty.Get_PropertyFlags(propA);
                    EPropertyFlags flagsB = Native_UProperty.Get_PropertyFlags(propB);

                    // Const ref param! Copy copy/ref/out
                    if ((flagsB & constRef) == constRef)
                    {
                        Native_UProperty.SetPropertyFlags(propA, (flagsB & constRefOut));
                    }
                }
                else
                {
                    break;
                }

                iteratorA.MoveNext();
                iteratorB.MoveNext();
            }
        }

        private static IntPtr CreateFunctionParam(ManagedUnrealFunctionInfo functionInfo, IntPtr function, ref EFunctionFlags functionFlags,
            ManagedUnrealPropertyInfo paramInfo)
        {
            IntPtr property = CreateProperty(function, paramInfo, true);
            if (property == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            ValidateFunctionParamFlags(paramInfo, property, functionFlags);
            return property;
        }

        /// <summary>
        /// Some basic validation of flags which should have been set when creating the module info
        /// </summary>
        [Conditional("DEBUG")]
        private static void ValidateFunctionParamFlags(ManagedUnrealPropertyInfo paramInfo, IntPtr property, EFunctionFlags functionFlags)
        {
            EPropertyFlags propertyFlags = Native_UProperty.Get_PropertyFlags(property);
            Debug.Assert(propertyFlags.HasFlag(EPropertyFlags.Parm));
            if (paramInfo.IsOut)
            {
                Debug.Assert(propertyFlags.HasFlag(EPropertyFlags.OutParm));
            }
            else if (paramInfo.IsByRef)
            {
                Debug.Assert(propertyFlags.HasFlag(EPropertyFlags.OutParm | EPropertyFlags.ReferenceParm));
            }
            else if (paramInfo.IsFunctionReturnValue)
            {
                Debug.Assert(propertyFlags.HasFlag(EPropertyFlags.OutParm | EPropertyFlags.ReturnParm));
            }
            if (propertyFlags.HasFlag(EPropertyFlags.OutParm))
            {
                Debug.Assert(functionFlags.HasFlag(EFunctionFlags.HasOutParms));
            }
        }

        private static IntPtr CreateProperty(IntPtr outer, ManagedUnrealPropertyInfo propertyInfo, bool addToOuter)
        {
            Type type = ManagedUnrealTypeInfo.GetTypeFromPropertyInfo(propertyInfo);
            IntPtr property = CreateProperty(outer, type, propertyInfo.Type.TypeCode, propertyInfo.Name, propertyInfo.GenericArgs, 
                propertyInfo.FixedSizeArrayDim, addToOuter);

            if (property != IntPtr.Zero)
            {
                EPropertyFlags propertyFlags = Native_UProperty.Get_PropertyFlags(property);
                propertyFlags |= propertyInfo.Flags;

                // TODO: Propagate flags to inner properties which need it (collections)

                // Somewhat emulate a check in Engine/Source/Programs/UnrealHeaderTool/Private/HeaderParser.cpp
                // DoesAnythingInHierarchyHaveDefaultToInstanced()
                // This should be for EObjectFlags.ObjectReference only?
                EPropertyFlags inlinePropFlags = EPropertyFlags.InstancedReference | EPropertyFlags.ExportObject;
                if ((propertyFlags & inlinePropFlags) == inlinePropFlags)
                {
                    LateAddMetaData(propertyInfo.Path, (FName)MDProp.EditInline.ToString(), "true", false);
                }

                Native_UProperty.Set_PropertyFlags(property, propertyFlags);
            }

            SetAllMetaData(property, propertyInfo, UMeta.Target.Property);

            return property;
        }

        private static IntPtr CreateProperty(IntPtr outer, Type type, EPropertyType propertyType, string propertyName,
            List<ManagedUnrealTypeInfoReference> args, int fixedSizeArrayDim, bool addToOuter)
        {
            // NOTE: HeaderParser.cpp and UObjectGlobals.cpp use "new" instead of NewObject for creating properties.
            // - "new" sets the offset and calls GetOuterUField()->AddCppProperty(this);
            // - "new" will add flags by calling TTypeFundamentals::GetComputedFlagsPropertyFlags()
            //   These flags should be added when Link is called but means we can't call AddCppProperty on sub-properties 
            //   as some properties require those flags. We must emulate AddCppProperty on:
            //   - UEnumProperty, UArrayProperty - CAN call AddCppProperty
            //   - USetProperty, UMapProperty - CANNOT call AddCppProperty

            bool isFixedSizeArray = 
                propertyType == EPropertyType.InternalNativeFixedSizeArray || 
                propertyType == EPropertyType.InternalManagedFixedSizeArray;
            if (isFixedSizeArray)
            {
                // Get the actual type of the array
                propertyType = args[0].TypeCode;
            }

            IntPtr propertyClass = NativeReflection.GetPropertyClass(propertyType);
            if (propertyClass == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            // NOTE: We need EObjectFlags.MarkAsNative to allow packages to include this as an import. Imports are an important
            //       part of package saving to ensure our referenced fields are serialized properly. Import gathering uses a 
            //       Obj->IsNative() check for import inclusion (see FArchiveSaveTagImports::operator<<)
            // NOTE: EObjectFlags.MarkAsNative will likely stop our classes from being GCed on hotreload. Unmark them on hotreload?
            EObjectFlags objectFlags = EObjectFlags.Public | EObjectFlags.Transient | EObjectFlags.MarkAsNative;
            IntPtr property = NativeReflection.NewObject(outer, propertyClass, new FName(propertyName), objectFlags);

            if (isFixedSizeArray && fixedSizeArrayDim > 1)
            {
                Native_UProperty.Set_ArrayDim(property, fixedSizeArrayDim);
            }

            // Set type specific information
            switch (propertyType)
            {
                case EPropertyType.Array:
                    CreateProperty(property, type.GenericTypeArguments[0], args[0].TypeCode, propertyName, null, 0, true);
                    break;

                case EPropertyType.Set:
                    // TODO: Ensure the element property has a hash method
                    Native_USetProperty.Set_ElementProp(property,
                        CreateProperty(property, type.GenericTypeArguments[0], args[0].TypeCode, propertyName, null, 0, false));
                    break;
                    
                case EPropertyType.Map:
                    // TODO: Ensure the key property has a hash method
                    // C++ adds a "_Key" suffix to the key prop e.g.:
                    // "/Script/USharp.StructMapTest:Val1" - main name of the prop
                    // "/Script/USharp.StructMapTest:Val1.Val1_Key" - the key
                    // "/Script/USharp.StructMapTest:Val1.Val1" - the value
                    Native_UMapProperty.Set_KeyProp(property,
                        CreateProperty(property, type.GenericTypeArguments[0], args[0].TypeCode, propertyName + "_Key", null, 0, false));
                    Native_UMapProperty.Set_ValueProp(property,
                        CreateProperty(property, type.GenericTypeArguments[1], args[1].TypeCode, propertyName, null, 0, false));
                    break;

                case EPropertyType.Interface:
                    // TODO: Add UInterface.GetInterfaceAddress()?
                    IntPtr interfaceAddress = UClass.GetClassAddress(type);
                    Debug.Assert(interfaceAddress != IntPtr.Zero);// Not a lot we can do if an interface doesn't exist...
                    Debug.Assert(Native_UClass.GetClassFlags(interfaceAddress).HasFlag(EClassFlags.Interface));
                    Native_UInterfaceProperty.Set_InterfaceClass(property, interfaceAddress);
                    break;

                case EPropertyType.Class:
                    Native_UClassProperty.SetMetaClass(property, UClass.GetClassAddress(type.GenericTypeArguments[0]));
                    break;

                case EPropertyType.Object:
                    IntPtr objClass = UClass.GetClassAddress(type);
                    if (objClass == IntPtr.Zero)
                    {
                        // Use UObject if the desired object type isn't available
                        objClass = UClass.GetClassAddress(typeof(UObject));
                    }
                    Native_UObjectPropertyBase.SetPropertyClass(property, objClass);
                    break;

                case EPropertyType.LazyObject:
                case EPropertyType.WeakObject:
                case EPropertyType.SoftObject:
                    Native_UObjectPropertyBase.SetPropertyClass(property, UClass.GetClassAddress(type.GenericTypeArguments[0]));
                    break;

                case EPropertyType.SoftClass:
                    Native_USoftClassProperty.SetMetaClass(property, UClass.GetClassAddress(type.GenericTypeArguments[0]));
                    break;

                case EPropertyType.Enum:
                    Native_UEnumProperty.SetEnum(property, UEnum.GetEnumAddress(type));

                    // This assumes the type has been constrained to a byte type if marked as BlueprintType
                    // (enums exposed to Blueprint must be a byte)
                    Type enumUnderlyingType = type.GetEnumUnderlyingType();
                    EPropertyType enumUnderlyingTypeCode = EPropertyType.Int;
                    switch (Type.GetTypeCode(enumUnderlyingType))
                    {
                        case TypeCode.SByte:
                            enumUnderlyingTypeCode = EPropertyType.Int8;
                            break;
                        case TypeCode.Byte:
                            enumUnderlyingTypeCode = EPropertyType.Byte;
                            break;
                        case TypeCode.Int16:
                            enumUnderlyingTypeCode = EPropertyType.Int16;
                            break;
                        case TypeCode.UInt16:
                            enumUnderlyingTypeCode = EPropertyType.UInt16;
                            break;
                        case TypeCode.Int32:
                            enumUnderlyingTypeCode = EPropertyType.Int;
                            break;
                        case TypeCode.UInt32:
                            enumUnderlyingTypeCode = EPropertyType.UInt32;
                            break;
                        case TypeCode.Int64:
                            enumUnderlyingTypeCode = EPropertyType.Int64;
                            break;
                        case TypeCode.UInt64:
                            enumUnderlyingTypeCode = EPropertyType.UInt64;
                            break;
                    }
                    CreateProperty(property, enumUnderlyingType, enumUnderlyingTypeCode, propertyName, null, 0, true);
                    Debug.Assert(Native_UEnumProperty.GetUnderlyingProperty(property) != IntPtr.Zero);
                    break;

                case EPropertyType.Struct:
                    Native_UStructProperty.Set_Struct(property, UScriptStruct.ResolveStructAddress(type));
                    break;

                case EPropertyType.Delegate:
                    Native_UDelegateProperty.Set_SignatureFunction(property, UFunction.GetDelegateSignatureAddress(type));
                    break;

                case EPropertyType.MulticastDelegate:
                    Native_UMulticastDelegateProperty.Set_SignatureFunction(property, UFunction.GetDelegateSignatureAddress(type));
                    break;
            }

            if (addToOuter)
            {
                Native_UField.AddCppProperty(outer, property);
            }

            return property;
        }

        /// <summary>
        /// Copy of Engine/Source/Programs/UnrealHeaderTool/Private/HeaderParser.cpp
        /// TODO: Really this should be pre-determined in the ManagedUnrealTypeInfo
        /// </summary>
        private static void InheritDefaultToInstance(ManagedUnrealPropertyInfo propertyInfo, IntPtr property, IntPtr unrealClass)
        {
            bool defaultToInstanced = false;

            IntPtr search = unrealClass;
            while (!defaultToInstanced && (unrealClass != IntPtr.Zero))
            {
                defaultToInstanced = Native_UClass.HasAnyClassFlags(search, EClassFlags.DefaultToInstanced);
                unrealClass = Native_UClass.GetSuperClass(unrealClass);
            }

            if (defaultToInstanced)
            {
                Native_UProperty.SetPropertyFlags(property, EPropertyFlags.InstancedReference | EPropertyFlags.ExportObject);
                LateAddMetaData(propertyInfo.Path, (FName)MDProp.EditInline.ToString(), "true", false);
            }
        }

        /// <summary>
        /// Creates a Guid for the given name
        /// </summary>
        private static Guid CreateNameGuid(string name, HashSet<Guid> existingGuids = null)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.Unicode.GetBytes(name));

                // Truncate the hash
                byte[] buffer = new byte[16];
                Buffer.BlockCopy(hash, 0, buffer, 0, buffer.Length);

                if (existingGuids != null)
                {
                    // Use the last byte to avoid conflicts
                    buffer[buffer.Length - 1] = 0;
                    while (existingGuids.Contains(new Guid(buffer)))
                    {
                        buffer[buffer.Length - 1]++;
                    }
                }

                return new Guid(buffer);
            }
        }

        private static void CollectTypes<T>(Dictionary<Type, T> types,
            Dictionary<Type, ManagedTypeBase> allTypes,
            Dictionary<Type, ManagedTypeBase> changedTypes,
            Dictionary<Type, ManagedTypeBase> unchangedTypes) where T : ManagedTypeBase
        {
            foreach (KeyValuePair<Type, T> type in types)
            {
                allTypes.Add(type.Key, type.Value);
                if (type.Value.Changed)
                {
                    changedTypes.Add(type.Key, type.Value);
                }
                else
                {
                    unchangedTypes.Add(type.Key, type.Value);
                }
            }
        }

        private static void ChainChangedDependencies(
            Dictionary<string, ManagedTypeBase> allTypesByPath,
            Dictionary<Type, ManagedTypeBase> allTypes,
            Dictionary<Type, ManagedTypeBase> changedTypes,
            Dictionary<Type, ManagedTypeBase> unchangedTypes)
        {
            // Types which should create structural changes to other types (does an enum size ever change?)
            EPropertyType[] memberTypeCodes = { EPropertyType.Struct, EPropertyType.Enum };
            EPropertyType[] funcParamTypeCodes = { EPropertyType.Struct, EPropertyType.Enum };
            EPropertyType[] baseTypeCodes = { EPropertyType.Object, EPropertyType.Interface };

            int numNewChangedTypes = 1;
            while (numNewChangedTypes > 0)
            {
                numNewChangedTypes = 0;

                foreach (ManagedTypeBase managedType in new List<ManagedTypeBase>(unchangedTypes.Values))
                {
                    if (managedType.TypeCode == EPropertyType.Enum)
                    {
                        // Skip enums as they don't contain other types which would change the structure of the enum
                        continue;
                    }

                    foreach (ManagedUnrealPropertyInfo propertyInfo in managedType.TypeInfo.Properties)
                    {
                        if (TypeRefIsChangedType(managedType, propertyInfo.Type, changedTypes, allTypesByPath, memberTypeCodes))
                        {
                            managedType.ChangedByRef = true;
                            break;
                        }
                    }

                    if (!managedType.ChangedByRef)
                    {
                        foreach (ManagedUnrealFunctionInfo functionInfo in managedType.TypeInfo.Functions)
                        {
                            if (functionInfo.ReturnProp != null)
                            {
                                if (TypeRefIsChangedType(managedType, functionInfo.ReturnProp.Type, changedTypes, allTypesByPath, funcParamTypeCodes))
                                {
                                    managedType.ChangedByRef = true;
                                    break;
                                }
                            }
                            foreach (ManagedUnrealPropertyInfo propertyInfo in managedType.TypeInfo.Properties)
                            {
                                if (TypeRefIsChangedType(managedType, propertyInfo.Type, changedTypes, allTypesByPath, funcParamTypeCodes))
                                {
                                    managedType.ChangedByRef = true;
                                    break;
                                }
                            }
                            if (managedType.ChangedByRef)
                            {
                                break;
                            }
                        }
                    }

                    if (!managedType.ChangedByRef && managedType.TypeCode == EPropertyType.Object)
                    {
                        foreach (ManagedUnrealTypeInfoReference baseTypeInfo in managedType.TypeInfo.BaseTypes)
                        {
                            if (TypeRefIsChangedType(managedType, baseTypeInfo, changedTypes, allTypesByPath, baseTypeCodes))
                            {
                                managedType.ChangedByRef = true;
                                break;
                            }
                        }
                    }

                    if (managedType.ChangedByRef)
                    {
                        changedTypes.Add(managedType.Type, managedType);
                        unchangedTypes.Remove(managedType.Type);
                        numNewChangedTypes++;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the given type ref has changed during a hot reload (always true on first load)
        /// </summary>
        private static bool TypeRefIsChangedType(ManagedTypeBase managedType, ManagedUnrealTypeInfoReference typeRef,
            Dictionary<Type, ManagedTypeBase> changedTypes, Dictionary<string, ManagedTypeBase> allTypesByPath,
            params EPropertyType[] validTargetTypes)
        {
            if (typeRef == null || string.IsNullOrEmpty(typeRef.Path))
            {
                return false;
            }

            ManagedTypeBase otherManagedType;
            if (allTypesByPath.TryGetValue(typeRef.Path, out otherManagedType) && otherManagedType.HasChanged &&
                otherManagedType != managedType && validTargetTypes.Contains(otherManagedType.TypeCode))
            {
                return true;
            }

            return false;
        }

        private static void InitType(Type type, USharpPathAttribute pathAttribute)
        {
            if (type.IsSubclassOf(typeof(UObject)))
            {
                InitType<ManagedClass, USharpClass>(type, Classes, pathAttribute);
            }
            else if (type.IsEnum)
            {
                InitType<ManagedEnum, UEnum>(type, Enums, pathAttribute);
            }
            else if (typeof(IDelegateBase).IsAssignableFrom(type))
            {
                InitType<ManagedDelegateSignature, UDelegateFunction>(type, DelegateSignatures, pathAttribute);
            }
            else if (type.IsValueType)
            {
                InitType<ManagedStruct, USharpStruct>(type, Structs, pathAttribute);
            }
            else if (type.IsInterface)
            {
                InitType<ManagedInterface, UClass>(type, Interfaces, pathAttribute);
            }

            InitTypeMetaData(type);
        }

        private static void InitType<TManagedType, TNativeType>(Type type, Dictionary<Type, TManagedType> types, USharpPathAttribute pathAttribute)
            where TManagedType : ManagedTypeBase, new()
            where TNativeType : UObject
        {
            string root, directory, moduleName, objectName, memberName;
            FPackageName.GetPathInfo(pathAttribute.Path, out root, out directory, out moduleName, out objectName, out memberName);

            string packageName = "/" + root + "/" + directory;

            if (string.IsNullOrEmpty(moduleName) || string.IsNullOrEmpty(objectName))
            {
                return;
            }

            IntPtr package = NativeReflection.FindObject(Runtime.Classes.UPackage, IntPtr.Zero, packageName, true);
            if (package == IntPtr.Zero)
            {
                package = NativeReflection.CreatePackage(IntPtr.Zero, packageName);
                if (package == IntPtr.Zero)
                {
                    return;
                }
                Native_UPackage.SetPackageFlags(package, EPackageFlags.CompiledIn);

                // TODO: Find how to create a proper guid for a package (UHT CodeGenerator.cpp seems to use a crc of generated code)
                // NOTE: SHA256 seems to crash under CoreCLR (.NET Core) on Linux (access violation reading at address 0).
                //using (SHA256 sha256 = SHA256.Create())
                {
                    //byte[] hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(packageName));
                    byte[] hash = BitConverter.GetBytes(packageName.GetHashCode());

                    // Truncate the hash
                    byte[] buffer = new byte[16];
                    Buffer.BlockCopy(hash, 0, buffer, 0, Math.Min(buffer.Length, hash.Length));

                    Guid guid = new Guid(buffer);
                    Native_UPackage.SetGuid(package, ref guid);
                }
            }

            if (package == IntPtr.Zero || string.IsNullOrEmpty(packageName) || string.IsNullOrEmpty(objectName))
            {
                return;
            }
            
            ManagedUnrealModuleInfo moduleInfo = ManagedUnrealModuleInfo.FindModule(type);
            if (moduleInfo == null)
            {
                moduleInfo = ManagedUnrealModuleInfo.LoadModuleFromAssembly(type.Assembly);
                if (moduleInfo == null)
                {
                    return;
                }
            }

            ManagedUnrealTypeInfo typeInfo = moduleInfo.FindType(type);
            if (typeInfo == null)
            {
                return;
            }

            TManagedType managedType;
            types.TryGetValue(type, out managedType);

            if (managedType != null && !FBuild.WithHotReload)
            {
                // The type has previously been loaded and hot reload isn't available to enable reloading
                return;
            }

            string cachedTypeInfoHash;
            IntPtr cachedObjAddress;
            bool hasCachedType = GetCachedType(pathAttribute.Path, out cachedTypeInfoHash, out cachedObjAddress);

            if (managedType == null)
            {
                managedType = new TManagedType();
                types[type] = managedType;
            }

            managedType.ModuleInfo = moduleInfo;
            managedType.TypeInfo = typeInfo;
            managedType.Type = type;
            managedType.PackageName = packageName;
            managedType.Package = package;
            managedType.Name = objectName;
            if (hasCachedType && cachedTypeInfoHash == managedType.TypeInfo.Hash)
            {
                managedType.Address = cachedObjAddress;
                managedType.Changed = false;
            }
            else
            {
                managedType.Address = IntPtr.Zero;
                managedType.Changed = true;
            }

            if (managedType.Address == IntPtr.Zero)
            {
                // Attempt to find the existing object by the path if the cache failed
                managedType.Address = NativeReflection.FindObject(Runtime.Classes.UObject, IntPtr.Zero, typeInfo.Path, false);
            }

            if (managedType.Address != IntPtr.Zero)
            {
                if (!Native_UObjectBaseUtility.IsA(managedType.Address, UClass.GetClassAddress<TNativeType>()))
                {
                    // TODO:
                    // The object already exists but isn't of the desired type. It is likely a type was changed (e.g. struct->UObject)
                    // Move the old object to the transient package and create a new object?
                    // - See UObjectBase.cpp UClassCompiledInDefer / FindExistingObjectIfHotReload
                    System.Diagnostics.Debugger.Break();
                    //MoveObjectToTransientPackage();
                    //managedType.Address = IntPtr.Zero;
                }
            }

            if (managedType.Address == IntPtr.Zero)
            {
                EObjectFlags flags = EObjectFlags.Public | EObjectFlags.Standalone;
                managedType.Address = NativeReflection.NewObject(package, UClass.GetClassAddress<TNativeType>(),
                        new FName(objectName), flags);

                switch (typeInfo.TypeCode)
                {
                    case EPropertyType.Enum:
                        // UUserDefinedEnum requires ECppForm.Namespaced - set it by calling SetEnums
                        using (TArrayUnsafe<FName> namesUnsafe = new TArrayUnsafe<FName>())
                        using (TArrayUnsafe<long> valuesUnsafe = new TArrayUnsafe<long>())
                        {
                            Native_UEnum.SetEnums(managedType.Address, namesUnsafe.Address, valuesUnsafe.Address,
                                UEnum.ECppForm.EnumClass, true);
                        }
                        break;
                    case EPropertyType.Struct:
                        Native_USharpStruct.CreateGuid(managedType.Address);
                        break;
                }
            }
            else
            {
                // The object already exists, it has likely already been linked.
                managedType.Linked = true;
            }

            if (managedType.HasChanged && !managedType.Linked)
            {
                numChangedTypes++;
            }

            if (managedType.Address != IntPtr.Zero && 
                (!hasCachedType || cachedObjAddress != managedType.Address || cachedTypeInfoHash != managedType.TypeInfo.Hash))
            {
                AddCachedType(typeInfo.Path, typeInfo.Hash, managedType.Address);
            }
        }
    }
}

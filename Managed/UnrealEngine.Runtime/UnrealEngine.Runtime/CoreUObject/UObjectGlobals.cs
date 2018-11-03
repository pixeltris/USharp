using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    //public static class UObjectGlobals
    public partial class UObject
    {
        public static ObjectOuter AnyPackage
        {
            get { return ObjectOuter.AnyPackage; }
        }

        /// <summary>
        /// set while in SavePackage() to detect certain operations that are illegal while saving
        /// </summary>
        public static bool IsSavingPackage()
        {
            return Native_UObjectGlobals.Get_GIsSavingPackage();
        }

        /// <summary>
        /// Whether we are inside garbage collection
        /// </summary>
        /// <returns></returns>
        public static bool IsGarbageCollecting()
        {
            return Native_UObjectGlobals.IsGarbageCollecting();
        }

        /// <summary>
        /// Deletes all unreferenced objects, keeping objects that have any of the passed in KeepFlags set. Will wait for other threads to unlock GC.
        /// </summary>
        public static void CollectGarbage()
        {
            Native_UObjectGlobals.CollectGarbageDefault();
        }

        /// <summary>
        /// Deletes all unreferenced objects, keeping objects that have any of the passed in KeepFlags set. Will wait for other threads to unlock GC.
        /// </summary>
        /// <param name="keepFlags">objects with those flags will be kept regardless of being referenced or not</param>
        /// <param name="performFullPurge">if true, perform a full purge after the mark pass</param>
        public static void CollectGarbage(EObjectFlags keepFlags, bool performFullPurge = true)
        {
            Native_UObjectGlobals.CollectGarbage(keepFlags, performFullPurge);
        }

        /// <summary>
        /// Performs garbage collection only if no other thread holds a lock on GC
        /// </summary>
        /// <returns></returns>
        public static bool TryCollectGarbage()
        {
            return Native_UObjectGlobals.TryCollectGarbageDefault();
        }

        /// <summary>
        /// Performs garbage collection only if no other thread holds a lock on GC
        /// </summary>
        /// <param name="keepFlags">objects with those flags will be kept regardless of being referenced or not</param>
        /// <param name="performFullPurge">if true, perform a full purge after the mark pass</param>
        /// <returns></returns>
        public static bool TryCollectGarbage(EObjectFlags keepFlags, bool performFullPurge = true)
        {
            return Native_UObjectGlobals.TryCollectGarbage(keepFlags, performFullPurge);
        }

        /// <summary>
        /// Returns whether an incremental purge is still pending/ in progress.
        /// </summary>
        /// <returns>true if incremental purge needs to be kicked off or is currently in progress, false othwerise.</returns>
        public static bool IsIncrementalPurgePending()
        {
            return Native_UObjectGlobals.IsIncrementalPurgePending();
        }

        /// <summary>
        /// Incrementally purge garbage by deleting all unreferenced objects after routing Destroy.
        /// 
        /// Calling code needs to be EXTREMELY careful when and how to call this function as 
        /// RF_Unreachable cannot change on any objects unless any pending purge has completed!
        /// </summary>
        /// <param name="useTimeLimit">whether the time limit parameter should be used</param>
        /// <param name="timeLimit">soft time limit for this function call</param>
        public static void IncrementalPurgeGarbage(bool useTimeLimit, float timeLimit = 0.002f)
        {
            Native_UObjectGlobals.IncrementalPurgeGarbage(useTimeLimit, timeLimit);
        }

        /// <summary>
        /// Create a unique name by combining a base name and an arbitrary number string.
        /// The object name returned is guaranteed not to exist.
        /// </summary>
        /// <param name="outer">the outer for the object that needs to be named</param>
        /// <param name="unrealClass">the class for the object</param>
        /// <param name="baseName">optional base name to use when generating the unique object name; if not specified, the class's name is used</param>
        /// <returns>name is the form BaseName_##, where ## is the number of objects of this
        /// type that have been created since the last time the class was garbage collected.</returns>
        public static FName MakeUniqueObjectName(ObjectOuter outer, UClass unrealClass, FName baseName = default(FName))
        {
            FName result;
            Native_UObjectGlobals.MakeUniqueObjectName(outer.Address, 
                unrealClass == null ? IntPtr.Zero : unrealClass.Address,
                ref baseName, out result);
            return result;
        }

        /// <summary>
        /// Given a display label string, generates an FName slug that is a valid FName for that label.
        /// If the object's current name is already satisfactory, then that name will be returned.
        /// For example, "[MyObject]: Object Label" becomes "MyObjectObjectLabel" FName slug.
        /// 
        /// Note: The generated name isn't guaranteed to be unique.
        /// </summary>
        /// <param name="displayLabel">The label string to convert to an FName</param>
        /// <param name="currentObjectName">The object's current name, or NAME_None if it has no name yet</param>
        /// <returns>The generated object name</returns>
        public static FName MakeObjectNameFromDisplayLabel(string displayLabel, FName currentObjectName)
        {
            using (FStringUnsafe displayLabelUnsafe = new FStringUnsafe(displayLabel))
            {
                FName result;
                Native_UObjectGlobals.MakeObjectNameFromDisplayLabel(ref displayLabelUnsafe.Array, ref currentObjectName, out result);
                return result;
            }
        }

        /// <summary>
        /// Returns whether an object is referenced, not counting the one
        /// reference at Obj.
        /// </summary>
        /// <param name="res">Object to check</param>
        /// <param name="keepFlags">Objects with these flags will be considered as being referenced</param>
        /// <param name="internalKeepFlags">Objects with these internal flags will be considered as being referenced</param>
        /// <param name="checkSubObjects">Treat subobjects as if they are the same as passed in object</param>
        /// <returns>true if object is referenced, false otherwise</returns>
        public static bool IsReferenced(UObject res, EObjectFlags keepFlags, EInternalObjectFlags internalKeepFlags, bool checkSubObjects = false)
        {
            // TODO: Support FReferencerInformationList
            return Native_UObjectGlobals.IsReferenced(res == null ? IntPtr.Zero : res.Address, keepFlags, internalKeepFlags, checkSubObjects, IntPtr.Zero);
        }

        /// <summary>
        /// Returns whether we are currently loading a package (sync or async)
        /// </summary>
        /// <returns>true if we are loading a package, false otherwise</returns>
        public static bool IsLoading()
        {
            return Native_UObjectGlobals.IsLoading();
        }

        public static UPackage GetTransientPackage()
        {
            return GCHelper.Find<UPackage>(Native_UObjectGlobals.GetTransientPackage());
        }

        /// <summary>
        /// Constructs a gameplay object
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="outer">the outer for the new object.  If not specified, object will be created in the transient package.</param>
        /// <param name="unrealClass">the class of object to construct</param>
        /// <param name="name">the name for the new object.  If not specified, the object will be given a transient name via MakeUniqueObjectName</param>
        /// <param name="flags">the object flags to apply to the new object</param>
        /// <param name="template">the object to use for initializing the new object.  If not specified, the class's default object will be used</param>
        /// <param name="copyTransientsFromClassDefaults">if true, copy transient from the class defaults instead of the pass in archetype ptr (often these are the same)</param>
        /// <param name="instanceGraph">contains the mappings of instanced objects and components to their templates</param>
        /// <returns>a pointer of type T to a new object of the specified class</returns>
        public static T NewObject<T>(
            ObjectOuter outer,
            UClass unrealClass,
            FName name = default(FName),
            EObjectFlags flags = EObjectFlags.NoFlags,
            UObject template = null,
            bool copyTransientsFromClassDefaults = false,
            IntPtr instanceGraph = default(IntPtr)) where T : UObject
        {
            return NewObject<T>(true, outer, unrealClass, name, flags, template, copyTransientsFromClassDefaults, instanceGraph);
        }

        /// <summary>
        /// Constructs a gameplay object
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="outer">the outer for the new object.  If not specified, object will be created in the transient package.</param>
        /// <returns>a pointer of type T to a new object of the specified class</returns>
        public static T NewObject<T>(ObjectOuter outer = default(ObjectOuter)) where T : UObject
        {
            return NewObject<T>(false, outer, UClass.GetClass<T>());
        }

        /// <summary>
        /// Constructs a gameplay object
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="outer">the outer for the new object.  If not specified, object will be created in the transient package.</param>
        /// <param name="name">the name for the new object.  If not specified, the object will be given a transient name via MakeUniqueObjectName</param>
        /// <param name="flags">the object flags to apply to the new object</param>
        /// <param name="template">the object to use for initializing the new object.  If not specified, the class's default object will be used</param>
        /// <param name="copyTransientsFromClassDefaults">if true, copy transient from the class defaults instead of the pass in archetype ptr (often these are the same)</param>
        /// <param name="instanceGraph">contains the mappings of instanced objects and components to their templates</param>
        /// <returns>a pointer of type T to a new object of the specified class</returns>
        public static T NewObject<T>(
            ObjectOuter outer,
            FName name = default(FName),
            EObjectFlags flags = EObjectFlags.NoFlags,
            UObject template = null,
            bool copyTransientsFromClassDefaults = false,
            IntPtr instanceGraph = default(IntPtr)) where T : UObject
        {
            return NewObject<T>(false, outer, UClass.GetClass<T>(), name, flags, template, copyTransientsFromClassDefaults, instanceGraph);
        }        

        private static T NewObject<T>(
            bool checkClass,
            ObjectOuter outer,
            UClass unrealClass,
            FName name = default(FName),
            EObjectFlags flags = EObjectFlags.NoFlags,
            UObject template = null,
            bool copyTransientsFromClassDefaults = false,
            IntPtr instanceGraph = default(IntPtr)) where T : UObject
        {
            if (unrealClass == null)
            {
                return null;
            }

            if (!outer.IsAnyPackage && outer.Object == null)
            {
                outer.Object = GetTransientPackage();
            }

            if (name == FName.None)
            {
                FObjectInitializer.AssertIfInConstructor(outer.Object);
            }

            if (checkClass)
            {
                // DO_CHECK
                // Class was specified explicitly, so needs to be validated
                if (Native_UObjectGlobals.CheckIsClassChildOf_Internal != null)
                {
                    UClass parentClass = UClass.GetClass<T>();
                    Native_UObjectGlobals.CheckIsClassChildOf_Internal(
                        parentClass == null ? IntPtr.Zero : parentClass.Address,
                        unrealClass == null ? IntPtr.Zero : unrealClass.Address);
                }
            }

            return GCHelper.Find<T>(Native_UObjectGlobals.StaticConstructObject_Internal(
                unrealClass == null ? IntPtr.Zero : unrealClass.Address,
                outer.Address, ref name, flags, EInternalObjectFlags.None,
                template == null ? IntPtr.Zero : template.Address,
                copyTransientsFromClassDefaults,
                instanceGraph));
        }

        /// <summary>
        /// Creates a copy of SourceObject using the Outer and Name specified, as well as copies of all objects contained by SourceObject.
        /// Any objects referenced by SourceOuter or RootObject and contained by SourceOuter are also copied, maintaining their name relative to SourceOuter.  Any
        /// references to objects that are duplicated are automatically replaced with the copy of the object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="sourceObject">the object to duplicate</param>
        /// <param name="outer">the object to use as the Outer for the copy of SourceObject</param>
        /// <param name="name">the name to use for the copy of SourceObject</param>
        /// <param name="flagMask">a bitmask of EObjectFlags that should be propagated to the object copies.  The resulting object copies will only have the object flags
        /// specified copied from their source object.</param>
        /// <param name="destClass">optional class to specify for the destination object. MUST BE SERIALIZATION COMPATIBLE WITH SOURCE OBJECT!!!</param>
        /// <param name="duplicateMode"></param>
        /// <param name="internalFlagsMask">bitmask of EInternalObjectFlags that should be propagated to the object copies.</param>
        /// <returns></returns>
        public static T DuplicateObject<T>(
            T sourceObject,
            ObjectOuter outer,
            FName name = default(FName),
            EObjectFlags flagMask = EObjectFlags.AllFlags,
            UClass destClass = null,
            EDuplicateMode duplicateMode = EDuplicateMode.Normal,
            EInternalObjectFlags internalFlagsMask = EInternalObjectFlags.AllFlags) where T : UObject
        {
            if (sourceObject != null)
            {
                if (!outer.IsAnyPackage && outer.Object == null)
                {
                    outer.Object = GetTransientPackage();
                }
                return GCHelper.Find<T>(Native_UObjectGlobals.StaticDuplicateObject(
                    sourceObject == null ? IntPtr.Zero : sourceObject.Address,
                    outer.Address, ref name, flagMask,
                    destClass == null ? IntPtr.Zero : destClass.Address,
                    duplicateMode, internalFlagsMask));
            }
            return null;
        }

        /// <summary>
        /// Find an optional object, relies on the name being unqualified
        /// </summary>
        public static T FindObjectFast<T>(
            ObjectOuter outer,
            FName name,
            bool exactClass = false,
            bool anyPackage = false,
            EObjectFlags exclusiveFlags = EObjectFlags.NoFlags,
            EInternalObjectFlags exclusiveInternalFlags = EInternalObjectFlags.None) where T : UObject
        {
            UClass unrealClass = UClass.GetClass<T>();
            return GCHelper.Find<T>(Native_UObjectGlobals.StaticFindObjectFast(
                unrealClass == null ? IntPtr.Zero : unrealClass.Address,
                outer.Address, ref name, exactClass, anyPackage, exclusiveFlags, exclusiveInternalFlags));
        }

        /// <summary>
        /// Find an optional object.
        /// </summary>
        public static T FindObject<T>(ObjectOuter outer, string name, bool exactClass = false) where T : UObject
        {
            UClass unrealClass = UClass.GetClass<T>();
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            {
                return GCHelper.Find<T>(Native_UObjectGlobals.StaticFindObject(
                    unrealClass == null ? IntPtr.Zero : unrealClass.Address,
                    outer.Address, ref nameUnsafe.Array, exactClass));
            }
        }

        /// <summary>
        /// Find an object, no failure allowed.
        /// </summary>
        public static T FindObjectChecked<T>(ObjectOuter outer, string name, bool exactClass = false) where T : UObject
        {
            UClass unrealClass = UClass.GetClass<T>();
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            {
                return GCHelper.Find<T>(Native_UObjectGlobals.StaticFindObjectChecked(
                    unrealClass == null ? IntPtr.Zero : unrealClass.Address,
                    outer.Address, ref nameUnsafe.Array, exactClass));
            }
        }

        /// <summary>
        /// Find an object without asserting on GIsSavingPackage or IsGarbageCollecting()
        /// </summary>
        public static T FindObjectSafe<T>(ObjectOuter outer, string name, bool exactClass = false) where T : UObject
        {
            UClass unrealClass = UClass.GetClass<T>();
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            {
                return GCHelper.Find<T>(Native_UObjectGlobals.StaticFindObjectSafe(
                    unrealClass == null ? IntPtr.Zero : unrealClass.Address,
                    outer.Address, ref nameUnsafe.Array, exactClass));
            }
        }

        /// <summary>
        /// Load an object.
        /// </summary>
        public static T LoadObject<T>(ObjectOuter outer, string name, string filename = null, ELoadFlags loadFlags = ELoadFlags.None) where T : UObject
        {
            UClass unrealClass = UClass.GetClass<T>();
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            using (FStringUnsafe filenameUnsafe = new FStringUnsafe(filename))
            {
                return GCHelper.Find<T>(Native_UObjectGlobals.StaticLoadObject(
                    unrealClass == null ? IntPtr.Zero : unrealClass.Address,
                    outer.Address, ref nameUnsafe.Array, ref filenameUnsafe.Array, loadFlags, IntPtr.Zero, true));
            }
        }

        /// <summary>
        /// Load a class object.
        /// </summary>
        public static UClass LoadClass<T>(ObjectOuter outer, string name, string filename = null, ELoadFlags loadFlags = ELoadFlags.None)
        {
            UClass unrealClass = UClass.GetClass<T>();
            if (unrealClass == null)
            {
                return null;
            }
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            using (FStringUnsafe filenameUnsafe = new FStringUnsafe(filename))
            {
                return GCHelper.Find<UClass>(Native_UObjectGlobals.StaticLoadClass(
                    unrealClass.Address, outer.Address, ref nameUnsafe.Array, ref filenameUnsafe.Array, loadFlags, IntPtr.Zero));
            }
        }

        /// <summary>
        /// Load a class object.
        /// </summary>
        public static UClass LoadClass(UClass baseClass, ObjectOuter outer, string name, string filename = null, ELoadFlags loadFlags = ELoadFlags.None)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            using (FStringUnsafe filenameUnsafe = new FStringUnsafe(filename))
            {
                return GCHelper.Find<UClass>(Native_UObjectGlobals.StaticLoadClass(
                    baseClass.Address, outer.Address, ref nameUnsafe.Array, ref filenameUnsafe.Array, loadFlags, IntPtr.Zero));
            }
        }

        /// <summary>
        /// Get default object of a class.
        /// </summary>
        public static T GetDefault<T>() where T : UObject
        {
            UClass unrealClass = UClass.GetClass<T>();
            if (unrealClass != null)
            {
                return unrealClass.GetDefaultObject() as T;
            }
            return null;
        }

        /// <summary>
        /// Gets the default object of a class.
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="unrealClass">The class to get the CDO for.</param>
        /// <returns>Class default object (CDO).</returns>
        public static T GetDefault<T>(UClass unrealClass) where T : UObject
        {
            if (unrealClass != null && unrealClass.IsA<T>())
            {
                UObject defaultObject = unrealClass.GetDefaultObject();
                if (defaultObject.IsA<T>())
                {
                    return defaultObject as T;
                }
            }
            return null;
        }

        /// <summary>
        /// Loads a package and all contained objects that match context flags.
        /// </summary>
        /// <param name="outer">Package to load new package into (usually NULL or ULevel->GetOuter())</param>
        /// <param name="longPackageName">Long package name to load</param>
        /// <param name="loadFlags">Flags controlling loading behavior</param>
        /// <returns>Loaded package if successful, NULL otherwise</returns>
        public static UPackage LoadPackage(UPackage outer, string longPackageName, ELoadFlags loadFlags)
        {
            using (FStringUnsafe longPackageNameUnsafe = new FStringUnsafe(longPackageName))
            {
                return GCHelper.Find<UPackage>(Native_UObjectGlobals.LoadPackage(
                    outer == null ? IntPtr.Zero : outer.Address, ref longPackageNameUnsafe.Array, loadFlags));
            }
        }

        /// <summary>
        /// Find an existing package by name
        /// </summary>
        /// <param name="outer">The Outer object to search inside</param>
        /// <param name="packageName">The name of the package to find</param>
        /// <returns>The package if it exists</returns>
        public static UPackage FindPackage(ObjectOuter outer, string packageName)
        {
            using (FStringUnsafe packageNameUnsafe = new FStringUnsafe(packageName))
            {
                return GCHelper.Find<UPackage>(Native_UObjectGlobals.FindPackage(
                    outer.Address, ref packageNameUnsafe.Array));
            }
        }

        /// <summary>
        /// Find an existing package by name or create it if it doesn't exist
        /// </summary>
        /// <param name="outer">The Outer object to search inside</param>
        /// <param name="packageName"></param>
        /// <returns>The existing package or a newly created one</returns>
        public static UPackage CreatePackage(ObjectOuter outer, string packageName)
        {
            using (FStringUnsafe packageNameUnsafe = new FStringUnsafe(packageName))
            {
                return GCHelper.Find<UPackage>(Native_UObjectGlobals.CreatePackage(outer.Address, ref packageNameUnsafe.Array));
            }
        }

        /// <summary>
        /// Create a new instance of an object or replace an existing object.  If both an Outer and Name are specified, and there is an object already in memory with the same Class, Outer, and Name, the
        /// existing object will be destructed, and the new object will be created in its place.
        /// </summary>
        /// <param name="unrealClass">the class of the object to create</param>
        /// <param name="outer">the object to create this object within (the Outer property for the new object will be set to the value specified here).</param>
        /// <param name="name">the name to give the new object. If no value (NAME_None) is specified, the object will be given a unique name in the form of ClassName_#.</param>
        /// <param name="outReusedSubobject">flag indicating if the object is a subobject that has already been created (in which case further initialization is not necessary).</param>
        /// <param name="setFlags">the ObjectFlags to assign to the new object. some flags can affect the behavior of constructing the object.</param>
        /// <param name="internalSetFlags">the InternalObjectFlags to assign to the new object. some flags can affect the behavior of constructing the object.</param>
        /// <param name="canReuseSubobjects">if set to true, SAO will not attempt to destroy a subobject if it already exists in memory.</param>
        /// <returns>a pointer to a fully initialized object of the specified class.</returns>
        public static UObject StaticAllocateObject(UClass unrealClass, ObjectOuter outer, FName name, out bool outReusedSubobject,
            EObjectFlags setFlags, EInternalObjectFlags internalSetFlags, bool canReuseSubobjects = false)
        {
            csbool outReusedSubobjectTemp;
            UObject result = GCHelper.Find(Native_UObjectGlobals.StaticAllocateObject(
                unrealClass == null ? IntPtr.Zero : unrealClass.Address,
                outer.Address, ref name, setFlags, internalSetFlags, canReuseSubobjects, out outReusedSubobjectTemp));
            outReusedSubobject = outReusedSubobjectTemp;
            return result;
        }
    }
}

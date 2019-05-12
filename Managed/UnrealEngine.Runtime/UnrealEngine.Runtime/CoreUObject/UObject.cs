using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// The base class of all objects.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.Object", "CoreUObject", UnrealModuleType.Engine)]
    public partial class UObject : IEquatable<UObject>, IInterface
    {
        /// <summary>
        /// Used to consume event methods (BlueprintEvent / RPC) which will be rewritten by AssemblyRewriter.
        /// They are rewritten to call the most derived function implementation (BlueprintEvent) or the correct network endpoint (RPC).
        /// </summary>
        public static Exception EventDef
        {
            get { return new EventNotRewrittenException(); }
        }

        // Our UObject overhead is objRefId(uint) + objRef (UObjectRef - pooled) + Address(IntPtr) +
        // injectedInterfaces(dictionary - null until accessed on types without a concrete C# implementation)

        /// <summary>
        /// Used for UObject types where we don't have a concrete C# type which implements the interfaces.
        /// If the UObject is defined in C# this should always be null as we can access the interface functions directly.
        /// This collection should hold pooled IInterface objects which are reused when this object is destroyed.
        /// </summary>
        private Dictionary<Type, IInterface> injectedInterfaces;

        internal uint objRefId;
        internal UObjectRef objRef;// This will be set to null when the native object is GCed

        /// <summary>
        /// The address of the object.
        /// This will be set to IntPtr.Zero when the native object is GCed.
        /// </summary>
        public IntPtr Address { get; internal set; }

        /// <summary>
        /// Has this object been fully destroyed. If this is true it is unsafe to use any members which access the
        /// underlying native memory of the object as this will read / write invalid memory.
        /// </summary>
        public bool IsDestroyed
        {
            get { return objRef == null; }
        }

        /// <summary>
        /// Is this a known type with an equivalent C# class
        /// </summary>
        public bool IsKnownType
        {
            get { return objRef.IsKnownType; }
        }

        /// <summary>
        /// Returns the address of the object (this used by IInterface and can otherwise be ignored)
        /// </summary>
        public IntPtr GetAddress()
        {
            return Address;
        }

        /// <summary>
        /// Returns itself (this used by IInterface and can otherwise be ignored)
        /// </summary>
        public UObject GetObject()
        {
            return this;
        }

        /// <summary>
        /// Used to see if the object is dead but the memory is still valid
        /// </summary>
        public bool IsPendingKill
        {
            get { return Native_UObjectBaseUtility.IsPendingKill(Address); }
            set
            {
                if (value)
                {
                    Native_UObjectBaseUtility.MarkPendingKill(Address);
                }
                else
                {
                    Native_UObjectBaseUtility.ClearPendingKill(Address);
                }
            }
        }

        /// <summary>
        /// The fully qualified pathname for this object
        /// </summary>
        public string PathName
        {
            get { return GetPathName(); }
        }

        /// <summary>
        /// Is this object is explicitly rooted (this prevents the object and all
        /// its descendants from being deleted during garbage collection)
        /// </summary>
        public bool IsRooted
        {
            get { return Native_UObjectBaseUtility.IsRooted(Address); }
            set
            {
                if (value)
                {
                    Native_UObjectBaseUtility.AddToRoot(Address);
                }
                else
                {
                    Native_UObjectBaseUtility.RemoveFromRoot(Address);
                }
            }
        }

        /// <summary>
        /// Checks if the object is native.
        /// </summary>
        public bool IsNative
        {
            get { return Native_UObjectBaseUtility.IsNative(Address); }
        }

        /// <summary>
        /// Checks to see if the object appears to be valid
        /// </summary>
        /// <returns>true if this appears to be a valid object</returns>
        public bool IsValidLowLevel()
        {
            return Native_UObjectBase.IsValidLowLevel(Address);
        }

        /// <summary>
        /// Faster version of IsValidLowLevel.
        /// Checks to see if the object appears to be valid by checking pointers and their alignment.
        /// Name and InternalIndex checks are less accurate than IsValidLowLevel.
        /// </summary>
        /// <param name="recursive">true if the Class pointer should be checked with IsValidLowLevelFast</param>
        /// <returns>true if this appears to be a valid object</returns>
        public bool IsValidLowLevelFast(bool recursive = true)
        {
            return Native_UObjectBase.IsValidLowLevelFast(Address, recursive);
        }

        /// <summary>
        /// Returns the name of this object (with no path information)
        /// </summary>
        /// <returns>Name of the object.</returns>
        public string GetName()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UObjectBaseUtility.GetName(Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        public FName GetFName()
        {
            FName result;
            Native_UObjectBase.GetFName(Address, out result);
            return result;
        }

        /// <summary>
        /// Returns the stat ID of the object...
        /// </summary>
        public TStatId GetStatId()
        {
            TStatId result;
            Native_UObjectBase.GetStatID(Address, out result);
            return result;
        }

        /// <summary>
        /// Walks up the chain of packages until it reaches the top level, which it ignores.
        /// </summary>
        /// <param name="startWithOuter"></param>
        /// <returns></returns>
        public string GetFullGroupName(bool startWithOuter)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UObjectBaseUtility.GetFullGroupName(Address, startWithOuter, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Returns the fully qualified pathname for this object as well as the name of the class, in the format:
        /// 'ClassName Outermost[.Outer].Name'.
        /// </summary>
        /// <param name="stopOuter">if specified, indicates that the output string should be relative to this object.  if StopOuter
        /// does not exist in this object's Outer chain, the result would be the same as passing NULL.</param>
        /// <returns></returns>
        public string GetFullName(UObject stopOuter = null)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UObjectBaseUtility.GetFullName(Address, stopOuter == null ? IntPtr.Zero : stopOuter.Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Returns the fully qualified pathname for this object, in the format:
        /// 'Outermost[.Outer].Name'
        /// </summary>
        /// <param name="stopOuter">if specified, indicates that the output string should be relative to this object.  if StopOuter
        /// does not exist in this object's Outer chain, the result would be the same as passing NULL.</param>
        /// <returns></returns>
        public string GetPathName(UObject stopOuter = null)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UObjectBaseUtility.GetPathName(Address, stopOuter == null ? IntPtr.Zero : stopOuter.Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Returns the unique ID of the object...these are reused so it is only unique while the object is alive.
        /// Useful as a tag.
        /// </summary>
        /// <returns></returns>
        public uint GetUniqueID()
        {
            return Native_UObjectBase.GetUniqueID(Address);
        }        

        private CachedUObject<UClass> cachedClass;
        public UClass GetClass()
        {
            return cachedClass.Update(Native_UObjectBase.GetClass(Address));
        }

        private CachedUObject<UObject> cachedOuter;
        public UObject GetOuter()
        {
            return cachedOuter.Update(Native_UObjectBase.GetOuter(Address));
        }

        /// <summary>
        /// Retrieve the object flags directly
        /// </summary>
        /// <returns>Flags for this object</returns>
        public EObjectFlags GetFlags()
        {
            return Native_UObjectBase.GetFlags(Address);
        }

        /// <summary>
        /// Set object flags.
        /// </summary>
        /// <param name="newFlags">Object flags to set.</param>
        public void SetFlags(EObjectFlags newFlags)
        {
            Native_UObjectBaseUtility.SetFlags(Address, newFlags);
        }

        /// <summary>
        /// Clears passed in object flags.
        /// </summary>
        /// <param name="newFlags">Object flags to clear.</param>
        public void ClearFlags(EObjectFlags newFlags)
        {
            Native_UObjectBaseUtility.ClearFlags(Address, newFlags);
        }

        /// <summary>
        /// Used to safely check whether any of the passed in flags are set. 
        /// </summary>
        /// <param name="flagsToCheck">Object flags to check for.</param>
        /// <returns>true if any of the passed in flags are set, false otherwise  (including no flags passed in).</returns>
        public bool HasAnyFlags(EObjectFlags flagsToCheck)
        {
            return Native_UObjectBaseUtility.HasAnyFlags(Address, flagsToCheck);
        }

        /// <summary>
        /// Used to safely check whether all of the passed in flags are set. 
        /// </summary>
        /// <param name="flagsToCheck">Object flags to check for</param>
        /// <returns>true if all of the passed in flags are set (including no flags passed in), false otherwise</returns>
        public bool HasAllFlags(EObjectFlags flagsToCheck)
        {
            return Native_UObjectBaseUtility.HasAllFlags(Address, flagsToCheck);
        }

        /// <summary>
        /// Atomically adds the specified flags.
        /// Do not use unless you know what you are doing.
        /// Designed to be used only by parallel GC and UObject loading thread.
        /// </summary>
        /// <param name="flagsToAdd"></param>
        public void AtomicallySetFlags(EObjectFlags flagsToAdd)
        {
            Native_UObjectBase.AtomicallySetFlags(Address, flagsToAdd);
        }

        /// <summary>
        /// Atomically clears the specified flags.
        /// Do not use unless you know what you are doing.
        /// Designed to be used only by parallel GC and UObject loading thread.
        /// </summary>
        /// <param name="flagsToClear"></param>
        public void AtomicallyClearFlags(EObjectFlags flagsToClear)
        {
            Native_UObjectBase.AtomicallyClearFlags(Address, flagsToClear);
        }

        /// <summary>
        /// Returns object flags that are both in the mask and set on the object.
        /// </summary>
        /// <param name="mask">Mask to mask object flags with</param>
        /// <returns>flags that are set in both the object and the mask</returns>
        public EObjectFlags GetMaskedFlags(EObjectFlags mask)
        {
            return Native_UObjectBaseUtility.GetMaskedFlags(Address, mask);
        }

        /// <summary>
        /// Adds marks to an object
        /// </summary>
        /// <param name="marks">Logical OR of OBJECTMARK_'s to apply</param>
        public void Mark(EObjectMark marks)
        {
            Native_UObjectBaseUtility.Mark(Address, marks);
        }

        /// <summary>
        /// Removes marks from and object
        /// </summary>
        /// <param name="marks">Logical OR of OBJECTMARK_'s to remove</param>
        public void UnMark(EObjectMark marks)
        {
            Native_UObjectBaseUtility.UnMark(Address, marks);
        }

        /// <summary>
        /// Tests an object for having ANY of a set of marks
        /// </summary>
        /// <param name="marks">Logical OR of OBJECTMARK_'s to test</param>
        /// <returns>true if the object has any of the given marks.</returns>
        public bool HasAnyMarks(EObjectMark marks)
        {
            return Native_UObjectBaseUtility.HasAnyMarks(Address, marks);
        }

        /// <summary>
        /// Tests an object for having ALL of a set of marks
        /// </summary>
        /// <param name="marks">Logical OR of OBJECTMARK_'s to test</param>
        /// <returns>true if the object has any of the given marks.</returns>
        public bool HasAllMarks(EObjectMark marks)
        {            
            return Native_UObjectBaseUtility.HasAnyMarks(Address, marks);
        }

        /// <summary>
        /// Marks this object as <see cref="EInternalObjectFlags.PendingKill"/>.
        /// </summary>
        public void MarkPendingKill()
        {
            Native_UObjectBaseUtility.MarkPendingKill(Address);
        }

        /// <summary>
        /// Unmarks this object as <see cref="EInternalObjectFlags.PendingKill"/>.
        /// </summary>
        public void ClearPendingKill()
        {
            Native_UObjectBaseUtility.ClearPendingKill(Address);
        }

        /// <summary>
        /// Add an object to the root set. This prevents the object and all
        /// its descendants from being deleted during garbage collection.
        /// </summary>
        public void AddToRoot()
        {
            Native_UObjectBaseUtility.AddToRoot(Address);
        }

        /// <summary>
        /// Remove an object from the root set.
        /// </summary>
        public void RemoveFromRoot()
        {
            Native_UObjectBaseUtility.RemoveFromRoot(Address);
        }

        /// <summary>
        /// Sets passed in internal object flags.
        /// </summary>
        /// <param name="flagsToSet">Internal object flags to set.</param>
        public void SetInternalFlags(EInternalObjectFlags flagsToSet)
        {
            Native_UObjectBaseUtility.SetInternalFlags(Address, flagsToSet);
        }

        /// <summary>
        /// Gets internal object flags.
        /// </summary>
        /// <returns>the internal object flags.</returns>
        public EInternalObjectFlags GetInternalFlags()
        {
            return Native_UObjectBaseUtility.GetInternalFlags(Address);
        }

        /// <summary>
        /// Used to safely check whether any of the passed in internal flags are set.
        /// </summary>
        /// <param name="flagsToCheck">Object flags to check for.</param>
        /// <returns>true if any of the passed in flags are set, false otherwise  (including no flags passed in).</returns>
        public bool HasAnyInternalFlags(EInternalObjectFlags flagsToCheck)
        {
            return Native_UObjectBaseUtility.HasAnyInternalFlags(Address, flagsToCheck);
        }

        private CachedUObject<UPackage> cachedOutermost;
        /// <summary>
        /// Walks up the list of outers until it finds the highest one.
        /// </summary>
        /// <returns>outermost non NULL Outer.</returns>        
        public UPackage GetOutermost()
        {
            return cachedOutermost.Update(Native_UObjectBaseUtility.GetOutermost(Address));
        }

        /// <summary>
        /// Finds the outermost package and marks it dirty.
        /// The editor suppresses this behavior during load as it is against policy to dirty packages simply by loading them.
        /// </summary>
        /// <returns>false if the request to mark the package dirty was suppressed by the editor and true otherwise.</returns>
        public bool MarkPackageDirty()
        {
            return Native_UObjectBaseUtility.MarkPackageDirty(Address);
        }

        /// <summary>
        /// Determines whether this object is a template object
        /// </summary>
        /// <param name="templateTypes"></param>
        /// <returns>true if this object is a template object (owned by a UClass)</returns>
        public bool IsTemplate(EObjectFlags templateTypes = EObjectFlags.ArchetypeObject | EObjectFlags.ClassDefaultObject)
        {
            return Native_UObjectBaseUtility.IsTemplate(Address, templateTypes);
        }

        /// <summary>
        /// Traverses the outer chain searching for the next object of a certain type.  (T must be derived from UObject)
        /// </summary>
        /// <param name="target">class to search for</param>
        /// <returns>a pointer to the first object in this object's Outer chain which is of the correct type.</returns>
        public UObject GetTypedOuter(UClass target)
        {
            return GCHelper.Find(Native_UObjectBaseUtility.GetTypedOuter(Address, target == null ? IntPtr.Zero : target.Address));
        }

        /// <summary>
        /// Traverses the outer chain searching for the next object of a certain type.  (T must be derived from UObject)
        /// </summary>
        /// <typeparam name="T">class to search for</typeparam>
        /// <returns>a pointer to the first object in this object's Outer chain which is of the correct type.</returns>
        public UObject GetTypedOuter<T>() where T : UObject
        {
            UClass unrealClass = UClass.GetClass<T>();
            if (unrealClass != null)
            {
                return GetTypedOuter(unrealClass);
            }
            return null;
        }

        /// <summary>
        /// Returns true if the specified object appears somewhere in this object's outer chain.
        /// </summary>
        /// <param name="someOuter"></param>
        /// <returns></returns>
        public bool IsIn(UObject someOuter)
        {
            return Native_UObjectBaseUtility.IsIn(Address, someOuter == null ? IntPtr.Zero : someOuter.Address);
        }

        /// <summary>
        /// Find out if this object is inside (has an outer) that is of the specified class
        /// </summary>
        /// <param name="someBaseClass">The base class to compare against</param>
        /// <returns>True if this object is in an object of the given type.</returns>
        public bool IsInA(UClass someBaseClass)
        {
            return Native_UObjectBaseUtility.IsInA(Address, someBaseClass == null ? IntPtr.Zero : someBaseClass.Address);
        }

        /// <summary>
        /// Checks whether this object's top-most package has any of the specified flags
        /// </summary>
        /// <param name="checkFlagMask">a bitmask of EPackageFlags values to check for</param>
        /// <returns></returns>
        public bool RootPackageHasAnyFlags(EPackageFlags checkFlagMask)
        {
            return Native_UObjectBaseUtility.RootPackageHasAnyFlags(Address, checkFlagMask);
        }

        /// <summary>
        /// Returns true if this object is of the template type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>true if this object is of the template type.</returns>
        public bool IsA<T>() where T : UObject
        {
            // NOTE: Removing this check for now as it hides logic where IsChildOf should be used instead of IsA and as such
            //       the native could would return false here.
            // First do a managed check rather than resolving the UClass and doing a native IsA call
            //if (this is T)
            //{
            //    return true;
            //}

            UClass unrealClass = UClass.GetClass<T>();
            if (unrealClass == null)
            {
                return false;
            }
            return Native_UObjectBaseUtility.IsA(Address, unrealClass.Address);
        }

        /// <summary>
        /// Returns true if this object is of the given type.
        /// </summary>
        /// <param name="someBaseClass"></param>
        /// <returns>true if this object is of the given type.</returns>
        public bool IsA(UClass someBaseClass)
        {
            return Native_UObjectBaseUtility.IsA(Address, someBaseClass == null ? IntPtr.Zero : someBaseClass.Address);
        }

        public bool IsA(Type type)
        {
            UClass unrealClass = UClass.GetClass(type);
            if (unrealClass == null)
            {
                return false;
            }
            return Native_UObjectBaseUtility.IsA(Address, unrealClass.Address);
        }

        public bool TryCast<T>(out UObject obj) where T : UObject
        {
            T result = this as T;
            if (result != null)
            {
                obj = result;
                return true;
            }
            if (IsA<T>())
            {                
                obj = this;
                return true;
            }
            obj = null;
            return false;
        }

        public UObject TryCast<T>(out bool knownType) where T : UObject
        {
            T result = this as T;
            if (result != null)
            {
                knownType = true;
                return result;
            }
            if (IsA<T>())
            {
                knownType = false;
                return this;
            }
            knownType = false;
            return null;
        }

        public UObject TryCast<T>() where T : UObject
        {
            UObject obj;
            TryCast<T>(out obj);
            return obj;
        }

        public T Cast<T>() where T : UObject
        {
            return this as T;
        }

        /// <summary>
        /// Finds the most-derived class which is a parent of both TestClass and this object's class.
        /// </summary>
        /// <param name="testClass"></param>
        /// <returns>the class to find the common base for</returns>
        public UClass FindNearestCommonBaseClass(UClass testClass)
        {
            return GCHelper.Find<UClass>(Native_UObjectBaseUtility.FindNearestCommonBaseClass(Address, testClass == null ? IntPtr.Zero : testClass.Address));
        }

        /// <summary>
        /// Returns a pointer to this object safely converted to a pointer of the specified interface class.
        /// </summary>
        /// <param name="interfaceClass">the interface class to use for the returned type</param>
        /// <returns>a pointer that can be assigned to a variable of the interface type specified, or NULL if this object's
        /// class doesn't implement the interface indicated.  Will be the same value as 'this' if the interface class
        /// isn't native.</returns>
        public IntPtr GetInterfaceAddress(UClass interfaceClass)
        {
            return Native_UObjectBaseUtility.GetInterfaceAddress(Address, interfaceClass == null ? IntPtr.Zero : interfaceClass.Address);
        }

        /// <summary>
        /// Returns a pointer to the I* native interface object that this object implements.
        /// Returns NULL if this object does not implement InterfaceClass, or does not do so natively.
        /// </summary>
        /// <param name="interfaceClass"></param>
        /// <returns></returns>
        public IntPtr GetNativeInterfaceAddress(UClass interfaceClass)
        {
            return Native_UObjectBaseUtility.GetNativeInterfaceAddress(Address, interfaceClass == null ? IntPtr.Zero : interfaceClass.Address);
        }

        /// <summary>
        /// Returns whether this component was instanced from a component/subobject template, or if it is a component/subobject template.
        /// This is based on a name comparison with the outer class instance lookup table
        /// </summary>
        /// <returns>true if this component was instanced from a template.  false if this component was created manually at runtime.</returns>
        public bool IsDefaultSubobject()
        {
            return Native_UObjectBaseUtility.IsDefaultSubobject(Address);
        }

        /// <summary>
        /// Returns properties that are replicated for the lifetime of the actor channel
        /// </summary>
        public virtual void GetLifetimeReplicatedProps(FLifetimePropertyCollection lifetimeProps)
        {
        }

        internal virtual void SetupPlayerInputComponent(IntPtr playerInputComponent)
        {
            // This is eventually implemented in APawn
        }

        internal virtual void BeginPlayInternal()
        {
            // This is eventually implemented in injected classes
        }

        internal virtual void EndPlayInternal(byte endPlayReason)
        {
            // This is eventually implemented in injected classes
        }

        /// <summary>
        /// Looks for a given function name
        /// </summary>
        public UFunction FindFunction(FName name)
        {
            return GCHelper.Find<UFunction>(Native_UObject.FindFunction(Address, ref name));
        }

        /// <summary>
        /// Looks for a given function name
        /// </summary>
        public UFunction FindFunctionChecked(FName name)
        {
            return GCHelper.Find<UFunction>(Native_UObject.FindFunctionChecked(Address, ref name));
        }

        /// <summary>
        /// Uses the TArchiveObjectReferenceCollector to build a list of all components referenced by this object which have this object as the outer
        /// </summary>
        /// <param name="includeNestedSubobjects">controls whether subobjects which are contained by this object, but do not have this object
        /// as its direct Outer should be included</param>
        /// <returns>the array that should be populated with the default subobjects "owned" by this object</returns>
        public UObject[] CollectDefaultSubobjects(bool includeNestedSubobjects = false)
        {
            using (TArrayUnsafe<UObject> result = new TArrayUnsafe<UObject>())
            {
                Native_UObject.CollectDefaultSubobjects(Address, result.Address, includeNestedSubobjects);
                return result.ToArray();
            }
        }

        /// <summary>
        /// Gets all objects which inherit from a specified base class. 
        /// Does not include any class default objects
        /// </summary>
        public static TObjectIterator<T> GetObjects<T>(
            EObjectFlags additionalExclusionFlags = EObjectFlags.ClassDefaultObject,
            bool includeDerivedClasses = true,
            EInternalObjectFlags internalExclusionFlags = EInternalObjectFlags.None) where T : UObject
        {
            return new TObjectIterator<T>(additionalExclusionFlags, includeDerivedClasses, internalExclusionFlags);
        }

        /// <summary>
        /// Gets all objects, including class default objects.
        /// </summary>
        public static FObjectIterator GetObjectsEx<T>(
            bool onlyGCedObjects = false,
            EObjectFlags additionalExclusionFlags = EObjectFlags.NoFlags,
            EInternalObjectFlags internalExclusionFlags = EInternalObjectFlags.None) where T : UObject
        {
            return GetObjectsEx(UClass.GetClass<T>(), onlyGCedObjects, additionalExclusionFlags, internalExclusionFlags);
        }

        /// <summary>
        /// Gets all objects, including class default objects.
        /// </summary>
        public static FObjectIterator GetObjectsEx(
            UClass unrealClass,
            bool onlyGCedObjects = false,
            EObjectFlags additionalExclusionFlags = EObjectFlags.NoFlags,
            EInternalObjectFlags internalExclusionFlags = EInternalObjectFlags.None)
        {
            return new FObjectIterator(unrealClass, onlyGCedObjects, additionalExclusionFlags, internalExclusionFlags);
        }

        /// <summary>
        /// Gets all objects, including class default objects.
        /// </summary>
        public static FObjectIterator GetObjectsEx(
            bool onlyGCedObjects = false,
            EObjectFlags additionalExclusionFlags = EObjectFlags.NoFlags,
            EInternalObjectFlags internalExclusionFlags = EInternalObjectFlags.None)
        {
            return new FObjectIterator(null, onlyGCedObjects, additionalExclusionFlags, internalExclusionFlags);
        }

        public bool CallFunctionByNameWithArguments(string cmd, bool forceCallWithNonExec = false)
        {
            return CallFunctionByNameWithArguments(cmd, this, forceCallWithNonExec);
        }

        public bool CallFunctionByNameWithArguments(string cmd, UObject executor, bool forceCallWithNonExec = false)
        {
            using (FStringUnsafe cmdUnsafe = new FStringUnsafe(cmd))
            {
                return Native_UObject.CallFunctionByNameWithArguments(Address, ref cmdUnsafe.Array, IntPtr.Zero, executor.Address, forceCallWithNonExec);
            }
        }

        /// <summary>
        /// Returns the name of this object (with no path information)
        /// </summary>
        /// <param name="obj">object to retrieve the name for; NULL gives "None"</param>
        /// <returns>Name of the object.</returns>
        public static string GetNameSafe(UObject obj)
        {
            if (obj == null)
            {
                return "None";
            }
            return obj.GetName();
        }

        /// <summary>
        /// Returns the path name of this object
        /// </summary>
        /// <param name="obj">object to retrieve the path name for; NULL gives "None"</param>
        /// <returns>path name of the object.</returns>
        public static string GetPathNameSafe(UObject obj)
        {
            if (obj == null)
            {
                return "None";
            }
            return obj.GetPathName();
        }

        /// <summary>
        /// Returns the full name of this object
        /// </summary>
        /// <param name="obj">object to retrieve the full name for; NULL (or a null class!) gives "None"</param>
        /// <returns>full name of the object.</returns>
        public static string GetFullNameSafe(UObject obj)
        {
            if (obj == null || Native_UObjectBase.GetClass(obj.Address) == IntPtr.Zero)
            {
                return "None";
            }
            return obj.GetFullName();
        }

        /// <summary>
        /// Invokes a UFunction of the given name using the CDO (class default object)
        /// </summary>
        public static object DynamicInvokeStatic<T>(string functionName, object[] parameters) where T : UObject
        {
            UClass unrealClass = UClass.GetClass<T>();
            if (unrealClass != null)
            {
                return DynamicInvokeStatic(unrealClass, functionName, parameters);
            }
            return null;
        }

        /// <summary>
        /// Invokes a UFunction of the given name using the CDO (class default object)
        /// </summary>
        public static object DynamicInvokeStatic(UClass unrealClass, string functionName, params object[] parameters)
        {
            return DynamicInvokeInternal(unrealClass, unrealClass.ClassDefaultObject, functionName, parameters);
        }

        /// <summary>
        /// Invokes a UFunction of the given name
        /// </summary>
        public static object DynamicInvoke(UObject obj, string functionName, params object[] parameters)
        {
            return DynamicInvokeInternal(obj.GetClass(), obj, functionName, parameters);
        }

        private static object DynamicInvokeInternal(UClass unrealClass, UObject obj, string functionName, params object[] parameters)
        {
            UFunction func = obj.GetClass().FindFunctionByName(new FName(functionName));
            if (func == null)
            {
                return null;
            }

            if (parameters == null)
            {
                parameters = new object[0];
            }

            bool validParams = true;

            Dictionary<UProperty, Delegate> fromNativeParams = new Dictionary<UProperty, Delegate>();
            Dictionary<UProperty, Delegate> toNativeParams = new Dictionary<UProperty, Delegate>();

            UProperty returnValueProp = null;
            List<UProperty> paramProps = new List<UProperty>();

            foreach (UProperty prop in func.GetProperties<UProperty>())
            {
                if (prop.HasAnyPropertyFlags(EPropertyFlags.Parm))
                {
                    if (prop.HasAnyPropertyFlags(EPropertyFlags.ReturnParm))
                    {
                        returnValueProp = prop;
                    }
                    else
                    {
                        paramProps.Add(prop);
                    }

                    Type paramType = UProperty.GetTypeFromProperty(prop);
                    if (paramType == null)
                    {
                        validParams = false;
                        break;
                    }

                    Delegate fromNative = MarshalingDelegateResolverSlow.GetFromNative(paramType);
                    Delegate toNative = MarshalingDelegateResolverSlow.GetToNative(paramType);
                    if (fromNative == null || toNative == null)
                    {
                        validParams = false;
                        break;
                    }

                    fromNativeParams.Add(prop, fromNative);
                    toNativeParams.Add(prop, toNative);
                }
            }

            if (parameters.Length != paramProps.Count)
            {
                validParams = false;
            }

            if (!validParams)
            {
                return null;
            }

            // Sort the parameters by offset, this is assumingly the correct thing to do?
            // - Otherwise we need to take the param names into this function. Or just not sort at all?
            paramProps.Sort((x, y) => x.GetOffset_ForUFunction().CompareTo(y.GetOffset_ForUFunction()));

            object result = null;

            unsafe
            {
                int paramsSize = func.ParmsSize;
                byte* paramsBufferAllocation = stackalloc byte[func.ParmsSize];
                IntPtr paramsBuffer = new IntPtr(paramsBufferAllocation);
                FMemory.Memzero(paramsBuffer, paramsSize);

                // Initialize default values for all parameters
                foreach (UProperty prop in func.GetProperties<UProperty>())
                {
                    if (prop.HasAnyPropertyFlags(EPropertyFlags.Parm))
                    {
                        Native.Native_UProperty.InitializeValue_InContainer(prop.Address, paramsBuffer);
                    }
                }

                // Copy the managed parameters to the buffer
                for (int i = 0; i < parameters.Length; i++)
                {
                    UProperty paramProp = paramProps[i];
                    object paramValue = parameters[i];
                    if (paramValue != null && (!paramProp.HasAnyPropertyFlags(EPropertyFlags.OutParm) ||
                        paramProp.HasAnyPropertyFlags(EPropertyFlags.ReferenceParm)))
                    {
                        toNativeParams[paramProp].DynamicInvoke(
                            paramsBuffer + paramProp.GetOffset_ForUFunction(), (int)0, paramProp.Address, paramValue);
                    }
                }

                // Invoke the function
                NativeReflection.InvokeFunction(obj.Address, func.Address, paramsBuffer, paramsSize);

                // Copy parameters / return value from the buffer
                for (int i = 0; i < parameters.Length; i++)
                {
                    UProperty paramProp = paramProps[i];
                    if (paramProp.HasAnyPropertyFlags(EPropertyFlags.OutParm))
                    {
                        parameters[i] = fromNativeParams[paramProp].DynamicInvoke(
                            paramsBuffer + paramProp.GetOffset_ForUFunction(), (int)0, paramProp.Address);
                    }
                }
                if (returnValueProp != null)
                {
                    result = fromNativeParams[returnValueProp].DynamicInvoke(
                        paramsBuffer + returnValueProp.GetOffset_ForUFunction(), (int)0, returnValueProp.Address);
                }

                // Destroy the memory for all of the parameters
                foreach (UProperty prop in func.GetProperties<UProperty>())
                {
                    if (prop.HasAnyPropertyFlags(EPropertyFlags.Parm))
                    {
                        Native.Native_UProperty.DestroyValue_InContainer(prop.Address, paramsBuffer);
                    }
                }
            }

            return result;
        }

        public virtual void Initialize()
        {
        }

        public virtual void Initialize(FObjectInitializer initializer)
        {
        }

        public void CheckDestroyed()
        {
            if (IsDestroyed)
            {
                throw new Exception("Attempting to access a destroyed unreal object of type " + GetType().ToString());
            }
        }

        /// <summary>
        /// Handle unloading of the object on managed assembly hotreload
        /// </summary>
        public virtual void OnAssemblyUnload()
        {
        }

        /// <summary>
        /// Handle reloading of the object on managed assembly hotreload
        /// </summary>
        public virtual void OnAssemblyReload()
        {
        }

        /// <summary>
        /// Used by generated code to handle underlying native type info changes (hotreload / blueprint compile)
        /// </summary>
        protected virtual void OnNativeTypeChanged()
        {
        }

        internal void ReleaseInjectedInterfaces()
        {
            if (injectedInterfaces != null)
            {
                foreach (IInterface instance in injectedInterfaces.Values)
                {
                    UnrealInterfacePool.ReturnObject(instance);
                }
                injectedInterfaces = null;
            }
        }

        public unsafe T GetInterface<T>() where T : class, IInterface
        {
            T result = this as T;
            if (result != null)
            {
                return result;
            }
            if (injectedInterfaces == null)
            {
                // If the injected interfaces haven't been set up set them up now

                if (objRef == null)
                {
                    return null;
                }

                UClass unrealClass = GetClass();
                if (unrealClass as USharpClass != null)
                {
                    // This is a C# defined type. We know if it implements the target interface or not due to the
                    // above "this as T". There isn't any need to inject interfaces into the UObject.
                    return null;
                }

                FScriptArray* interfacesPtr = (FScriptArray*)Native_UClass.Get_InterfacesRef(unrealClass.Address);
                if (interfacesPtr->ArrayNum != 0)
                {
                    injectedInterfaces = new Dictionary<Type, IInterface>();
                    foreach (FImplementedInterface implementedInterface in unrealClass.Interfaces)
                    {
                        if (implementedInterface.InterfaceClassAddress != IntPtr.Zero)
                        {
                            Type type = UClass.GetTypeFromClassAddress(implementedInterface.InterfaceClassAddress);
                            if (type != null)
                            {
                                IInterface instance = UnrealInterfacePool.New(type, objRef);
                                if (instance != null)
                                {
                                    injectedInterfaces[type] = instance;
                                    if (type == typeof(T))
                                    {
                                        result = instance as T;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Try and get the interface from the injected interfaces.
                IInterface instance;
                injectedInterfaces.TryGetValue(typeof(T), out instance);
                result = instance as T;
            }
            return result;
        }

        /// <summary>
        /// Returns true if this objects class implements the given IInterface derived type
        /// (call this on UObject instances; if you are working with a UClass call ImplementsInterface() instead).<para/>
        /// This is the equivalent of UKismetSystemLibrary::DoesImplementInterface().
        /// This is also the same as obj.GetClass().ImplementsInterface().
        /// </summary>
        /// <typeparam name="T">The IInterface derived type</typeparam>
        /// <returns>True if  this objects class implements the given IInterface derived type</returns>
        public bool DoesImplementInterface<T>() where T : IInterface
        {
            IntPtr interfaceClass = UClass.GetInterfaceClassAddress<T>();
            if (interfaceClass != IntPtr.Zero &&  Native_UClass.GetClassFlags(interfaceClass).HasFlag(EClassFlags.Interface))
            {
                return Native_UClass.ImplementsInterface(Native_UObjectBase.GetClass(Address), interfaceClass);
            }
            return false;
        }

        /// <summary>
        /// Returns true if this objects class implements the given IInterface derived type
        /// (call this on UObject instances; if you are working with a UClass call ImplementsInterface() instead).<para/>
        /// This is the equivalent of UKismetSystemLibrary::DoesImplementInterface().
        /// This is also the same as obj.GetClass().ImplementsInterface().
        /// </summary>
        /// <param name="type">The IInterface derived type</param>
        /// <returns>True if  this objects class implements the given IInterface derived type</returns>
        public bool DoesImplementInterface(Type type)
        {
            UClass interfaceClass = UClass.GetClass(type);
            if (interfaceClass != null && interfaceClass.ClassFlags.HasFlag(EClassFlags.Interface))
            {
                return Native_UClass.ImplementsInterface(Native_UObjectBase.GetClass(Address), interfaceClass.Address);
            }
            return false;
        }

        public Coroutine StartCoroutine(object obj, IEnumerator coroutine, bool pool = Coroutine.PoolByDefault)
        {
            return Coroutine.StartCoroutine(this, coroutine, pool);
        }

        public Coroutine StartCoroutine(IEnumerator coroutine, CoroutineGroup group = CoroutineGroup.Tick, bool pool = Coroutine.PoolByDefault)
        {
            return Coroutine.StartCoroutine(this, coroutine, group, pool);
        }

        public Coroutine StartCoroutine(IEnumerator coroutine, CoroutineGroup group = CoroutineGroup.Tick, string tag = null, bool pool = Coroutine.PoolByDefault)
        {
            return Coroutine.StartCoroutine(this, coroutine, group, tag, pool);
        }

        public void StopCoroutine(Coroutine coroutine)
        {
            Coroutine.StopCoroutine(coroutine);
        }

        public void StopCoroutine(IEnumerator coroutine)
        {
            Coroutine.StopCoroutine(this, coroutine);
        }

        public void StopAllCoroutines()
        {
            Coroutine.StopAllCoroutines(this);
        }

        public void StopCoroutines(string tag)
        {
            Coroutine.StopCoroutines(tag);
        }

        public List<Coroutine> FindCoroutines()
        {
            return Coroutine.FindCoroutines(this);
        }

        public List<Coroutine> FindCoroutines(string tag)
        {
            return Coroutine.FindCoroutines(this, tag);
        }

        public bool ContainsCoroutine(Coroutine coroutine)
        {
            return (coroutine.Owner as UObject) == this;
        }

        // Start invoker time
        
        public void StartInvoker(InvokerHandler handler, TimeSpan time, TimeSpan repeatedTime = default(TimeSpan), CoroutineGroup group = CoroutineGroup.Tick, bool pool = Invoker.PoolByDefault)
        {
            Invoker.StartInvoker(this, handler, time, repeatedTime, group, pool);
        }

        public void StartInvoker(InvokerHandlerWithInvoker handler, TimeSpan time, TimeSpan repeatedTime = default(TimeSpan), CoroutineGroup group = CoroutineGroup.Tick, bool pool = Invoker.PoolByDefault)
        {
            Invoker.StartInvoker(this, handler, time, repeatedTime, group, pool);
        }

        public void StartInvoker(InvokerHandlerWithObject handler, TimeSpan time, TimeSpan repeatedTime = default(TimeSpan), CoroutineGroup group = CoroutineGroup.Tick, bool pool = Invoker.PoolByDefault)
        {
            Invoker.StartInvoker(this, handler, time, repeatedTime, group, pool);
        }

        public void StartInvoker(InvokerHandlerWithObjectInvoker handler, TimeSpan time, TimeSpan repeatedTime = default(TimeSpan), CoroutineGroup group = CoroutineGroup.Tick, bool pool = Invoker.PoolByDefault)
        {
            Invoker.StartInvoker(this, handler, time, repeatedTime, group, pool);
        }

        // Start invoker ticks

        public void StartInvokerTicks(InvokerHandler handler, ulong ticks, ulong repeatedTicks = default(ulong), CoroutineGroup group = CoroutineGroup.Tick, bool pool = Invoker.PoolByDefault)
        {
            Invoker.StartInvokerTicks(this, handler, ticks, repeatedTicks, group, pool);
        }

        public void StartInvokerTicks(InvokerHandlerWithInvoker handler, ulong ticks, ulong repeatedTicks = default(ulong), CoroutineGroup group = CoroutineGroup.Tick, bool pool = Invoker.PoolByDefault)
        {
            Invoker.StartInvokerTicks(this, handler, ticks, repeatedTicks, group, pool);
        }

        public void StartInvokerTicks(InvokerHandlerWithObject handler, ulong ticks, ulong repeatedTicks = default(ulong), CoroutineGroup group = CoroutineGroup.Tick, bool pool = Invoker.PoolByDefault)
        {
            Invoker.StartInvokerTicks(this, handler, ticks, repeatedTicks, group, pool);
        }

        public void StartInvokerTicks(InvokerHandlerWithObjectInvoker handler, ulong ticks, ulong repeatedTicks = default(ulong), CoroutineGroup group = CoroutineGroup.Tick, bool pool = Invoker.PoolByDefault)
        {
            Invoker.StartInvokerTicks(this, handler, ticks, repeatedTicks, group, pool);
        }

        // Start invoker frames

        public void StartInvokerFrames(InvokerHandler handler, ulong frames, ulong repeatedFrames = default(ulong), CoroutineGroup group = CoroutineGroup.Tick, bool pool = Invoker.PoolByDefault)
        {
            Invoker.StartInvokerFrames(this, handler, frames, repeatedFrames, group, pool);
        }

        public void StartInvokerFrames(InvokerHandlerWithInvoker handler, ulong frames, ulong repeatedFrames = default(ulong), CoroutineGroup group = CoroutineGroup.Tick, bool pool = Invoker.PoolByDefault)
        {
            Invoker.StartInvokerFrames(this, handler, frames, repeatedFrames, group, pool);
        }

        public void StartInvokerFrames(InvokerHandlerWithObject handler, ulong frames, ulong repeatedFrames = default(ulong), CoroutineGroup group = CoroutineGroup.Tick, bool pool = Invoker.PoolByDefault)
        {
            Invoker.StartInvokerFrames(this, handler, frames, repeatedFrames, group, pool);
        }

        public void StartInvokerFrames(InvokerHandlerWithObjectInvoker handler, ulong frames, ulong repeatedFrames = default(ulong), CoroutineGroup group = CoroutineGroup.Tick, bool pool = Invoker.PoolByDefault)
        {
            Invoker.StartInvokerFrames(this, handler, frames, repeatedFrames, group, pool);
        }

        public void StopInvoker(Invoker invoker)
        {
            invoker.Stop();
        }

        public void StopInvokerByMethod(Delegate method)
        {
            Invoker.StopInvokerByMethod(this, method);
        }

        public void StopAllInvokers()
        {
            Invoker.StopAllInvokers(this);
        }

        public List<Invoker> FindInvokers()
        {
            return Invoker.FindInvokers(this);
        }

        public List<Invoker> FindInvokers(string tag)
        {
            return Invoker.FindInvokers(this, tag);
        }

        public List<Invoker> FindInvokers(int tagId)
        {
            return Invoker.FindInvokers(this, tagId);
        }

        public bool ContainsInvoker(Invoker invoker)
        {
            return (invoker.Owner as UObject) == this;
        }

        // It could be possible that two C# objects point to the same address so override == and compare Address
        // rather than doing the default object reference check
        public static bool operator ==(UObject a, UObject b)
        {
            if (Object.ReferenceEquals(a, null))
            {
                if (Object.ReferenceEquals(b, null))
                {
                    return true;
                }
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(UObject a, UObject b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as UObject);
        }

        public bool Equals(UObject other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }
            return objRefId == other.objRefId;
        }

        public override int GetHashCode()
        {
            return objRefId.GetHashCode();
        }
    }
}

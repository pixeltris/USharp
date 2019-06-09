// ARRAY_GC means that an array will be used instead of a dictionary for lookups. An array may be faster
// when there are more objects known to GCHelper.
// TODO: Use UObjectCreateListeners / UObjectDeleteListeners and more closely map the internal ids? The current
//       setup may result in two UObject instances having the same internal id
//#define ARRAY_GC

// The fastest way to implement GCHelper would be to fork UE4 and add a member to UObjectBase which is a 
// void* / GCHandle pointing to the C# instance

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public static class GCHelper
    {
#if ARRAY_GC
        private static List<UObjectRef> References = new List<UObjectRef>();
#else
        // <nativeAddress, UObjectRef>
        private static Dictionary<IntPtr, UObjectRef> References = new Dictionary<IntPtr, UObjectRef>();
#endif

        /// <summary>
        /// UObjectBase::InternalIndex offset
        /// </summary>
        private static int objectInternalIndexOffset;

        /// <summary>
        /// Used internally to avoid duplicate Initialize() calls
        /// </summary>
        internal static IntPtr ManagedObjectBeingInitialized;

        public static EObjectFlags GarbageCollectionKeepFlags
        {
            get { return FGlobals.IsEditor ? EObjectFlags.Standalone : EObjectFlags.NoFlags; }
        }

        // Pool UObjectRef instances to be reused (no reason not to)
        // TODO: Extend this to UObject instances as well. This is slightly more complicated with C# classes
        //       as there might be unexpected C# state that was set up which would have to be manually cleared which
        //       wouldn't be super obvious.
        //       - Make it an optional feature which is always on for non-C# defined classes?
        private static UObjectRefPool objRefPool = new UObjectRefPool();

        internal static bool Available { get; set; }

        public static int ReferenceCount
        {
            get { return References.Count; }
        }

        [Conditional("DEBUG")]
        private static void CheckAvailable()
        {
            if (!Available)
            {
                throw new Exception("GCHelper accessed before UObject classes loaded.");
            }
        }

        public static T FindInterface<T>(IntPtr native) where T : class, IInterface
        {
            UObject obj = Find(native);
            if (obj != null)
            {
                return obj.GetInterface<T>();
            }
            return null;
        }

        public static T Find<T>(IntPtr native) where T : UObject
        {
            return Find(native) as T;
        }

        public static UObject Find(IntPtr native)
        {
            UObjectRef objRef = FindRef(native);
            return objRef == null ? null : objRef.Managed;
        }

        public static unsafe UObjectRef FindRef(IntPtr native)
        {
            CheckAvailable();

            if (native == IntPtr.Zero)
            {
                return null;
            }

#if ARRAY_GC            
            int objectInternalIndex = *(int*)(native + objectInternalIndexOffset);
            UObjectRef objRef = References.Count > objectInternalIndex ? References[objectInternalIndex] : null;
            if (objRef == null)
            {
                IntPtr gcHandlePtr = OnAdd(native);
                if (gcHandlePtr != IntPtr.Zero)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(gcHandlePtr);
                    objRef = (UObjectRef)gcHandle.Target;
                }
            }
            return objRef;
#else
            UObjectRef objRef;
            if (!References.TryGetValue(native, out objRef))
            {
                IntPtr gcHandlePtr = Add(native);
                if (gcHandlePtr != IntPtr.Zero)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(gcHandlePtr);
                    objRef = (UObjectRef)gcHandle.Target;
                }
            }
            return objRef;
#endif
        }

        public static UObjectRef FindExistingRef(UObject obj)
        {
            return obj.objRef;
        }

        private static IntPtr Add(IntPtr native)
        {
            return Native_GCHelper.Add(native);
        }

        public static void Remove(IntPtr native)
        {
            Native_GCHelper.Remove(native);
        }

        private static unsafe IntPtr OnAdd(IntPtr native)
        {
            UObjectRef objRef = null;
            int objectInternalIndex = *(int*)(native + objectInternalIndexOffset);
#if ARRAY_GC
            while (References.Count <= objectInternalIndex)
            {
                References.Add(null);
            }
            if (References[objectInternalIndex] == null)
#else
            if (!References.TryGetValue(native, out objRef))
#endif
            {
                bool isKnownType;
                Type type = UClass.GetFirstKnownType(native, out isKnownType, false);
                if (type == null)
                {
                    // This probably means the given address is not valid (check IsValid/IsValidLowLevel/IsValidLowLevelFast ?)
                    string className = string.Empty;
                    string fullName = string.Empty;
                    try
                    {
                        using (FStringUnsafe classNameUnsafe = new FStringUnsafe())
                        {
                            Native_UObjectBaseUtility.GetName(Native_UObjectBase.GetClass(native), ref classNameUnsafe.Array);
                            className = classNameUnsafe.Value;
                        }
                    }
                    catch
                    {
                    }
                    try
                    {
                        using (FStringUnsafe fullNameUnsafe = new FStringUnsafe())
                        {
                            Native_UObjectBaseUtility.GetFullName(native, IntPtr.Zero, ref fullNameUnsafe.Array);
                            fullName = fullNameUnsafe.Value;
                        }
                    }
                    catch
                    {
                    }
                    // Get a smaller stack snippet
                    StackTrace stack = null;
                    try
                    {
                        stack = new StackTrace(4);
                    }
                    catch
                    {
                    }
                    string error = string.Format("[GCHelper-Error] Couldn't find type for requested UObject. Address: {0} (0x{1}) Name: \"{2}\" FullName: \"{3}\" Stack:\r\n{4}",
                        native.ToInt32(), native.ToInt32().ToString("X8"), className, fullName, stack);
                    FMessage.Log(ELogVerbosity.Error, error);
                    Debug.Assert(false, error);
                    return IntPtr.Zero;
                }
                if (type.IsInterface)
                {
                    // Validate that we are getting a UInterface and we aren't doing something very wrong.
                    Debug.Assert(Native_UObjectBaseUtility.IsA(native, UClass.GetClassAddress(type)));

                    // This should be a UInterface instance. We might want to do something more complex here
                    // where interfaces inherit from other interfaces.
                    type = typeof(UInterface);
                }
                objRef = objRefPool.New(native, type, isKnownType, objectInternalIndex);
#if ARRAY_GC
                References[objectInternalIndex] = objRef;
#else
                References.Add(native, objRef);
#endif
                return GCHandle.ToIntPtr(objRef.ManagedHandle);
            }
            return GCHandle.ToIntPtr(objRef.ManagedHandle);
        }

        private static void OnRemove(IntPtr gcHandlePtr)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr(gcHandlePtr);
            UObjectRef objRef = (UObjectRef)gcHandle.Target;
            //FMessage.Log("GC " + (objRef.Managed == null ? "null" : objRef.Managed.GetType().ToString()) + " (GCHandle: " + gcHandlePtr.ToString("X16") + " ptr: " + objRef.Native.ToString("X16") + ")");
            objRef.Managed.OnDestroyedInternal();
            Coroutine.RemoveObjectByGC(objRef.Managed);
            Invoker.RemoveObjectByGC(objRef.Managed);
            objRef.Managed.objRef = null;// This will make UObject.IsDestroyed true
            objRef.Managed.Address = IntPtr.Zero;// Reset the address
#if ARRAY_GC
            References[objRef.InternalIndex] = null;
#else
            References.Remove(objRef.Native);
#endif
            gcHandle.Free();

            // Return the objRef to the pool (this will also reset the objRef state back to empty)
            objRefPool.ReturnObject(objRef);
        }

        private static void OnPostGarbageCollect()
        {
            FMessage.Log("OnPostGarbageCollectBegin");
            Native_GCHelper.CollectGarbage();
            FMessage.Log("OnPostGarbageCollectEnd");
        }

        public static void CollectGarbage(bool managedOnly)
        {
            if (managedOnly)
            {
                Native_GCHelper.CollectGarbage();
            }
            else
            {
                UObject.CollectGarbage();
            }
        }

        // Hold onto the delegates to avoid them from being garbage collected
        private static Native_GCHelper.Del_Add onAdd;
        private static Native_GCHelper.Del_Remove onRemove;

        internal static void OnNativeFunctionsRegistered()
        {
            onAdd = new Native_GCHelper.Del_Add(OnAdd);
            onRemove = new Native_GCHelper.Del_Remove(OnRemove);

            Native_GCHelper.Set_OnAdd(onAdd);
            Native_GCHelper.Set_OnRemove(onRemove);

            objectInternalIndexOffset = Native_GCHelper.GetInternalIndexOffset();

            FCoreUObjectDelegates.PostGarbageCollect.Bind(OnPostGarbageCollect);
        }

        internal static void OnUnload()
        {
            // Call unload on all managed objects until there are no objects left to process

            // Find which types implement OnAssemblyUnload or OnAssemblyReload
            Dictionary<Type, bool> typesRequiringUnloadOrReload = new Dictionary<Type, bool>();

            List<IntPtr> allUnloadOrReloadReferences = new List<IntPtr>();
#if ARRAY_GC
            Dictionary<IntPtr, UObjectRef> references = new Dictionary<IntPtr, UObjectRef>();
            foreach (UObjectRef objRef in References)
            {
                if (objRef != null)
                {
                    references.Add(objRef.Native, objRef);
                }
            }
#else
            Dictionary<IntPtr, UObjectRef> references = new Dictionary<IntPtr, UObjectRef>(References);
#endif
            Dictionary<IntPtr, UObjectRef> newReferences = new Dictionary<IntPtr, UObjectRef>(references);
            while (newReferences.Count > 0)
            {
                foreach (KeyValuePair<IntPtr, UObjectRef> reference in newReferences)
                {
                    UObject obj = reference.Value.Managed;
                    if (obj != null && !obj.IsDestroyed)
                    {
                        bool requiresUnloadOrReload;
                        Type type = obj.GetType();
                        if (!typesRequiringUnloadOrReload.TryGetValue(type, out requiresUnloadOrReload))
                        {
                            MethodInfo unloadMethod = type.GetMethod("OnAssemblyUnload");
                            if (unloadMethod.DeclaringType != typeof(UObject))
                            {
                                requiresUnloadOrReload = true;
                            }
                            else
                            {
                                MethodInfo reloadMethod = type.GetMethod("OnAssemblyReload");
                                if (reloadMethod.DeclaringType != typeof(UObject))
                                {
                                    requiresUnloadOrReload = true;
                                }
                            }
                            typesRequiringUnloadOrReload.Add(type, requiresUnloadOrReload);
                        }

                        if (requiresUnloadOrReload)
                        {
                            reference.Value.Managed.OnAssemblyUnload();
                            allUnloadOrReloadReferences.Add(reference.Key);
                        }
                    }
                }
                newReferences.Clear();
#if ARRAY_GC
                foreach (UObjectRef objRef in References)
                {
                    if (objRef != null && !references.ContainsKey(objRef.Native))
                    {
                        references.Add(objRef.Native, objRef);
                        newReferences.Add(objRef.Native, objRef);
                    }
                }
#else
                foreach (KeyValuePair<IntPtr, UObjectRef> reference in References)
                {
                    if (!references.ContainsKey(reference.Key))
                    {
                        references.Add(reference.Key, reference.Value);
                        newReferences.Add(reference.Key, reference.Value);
                    }
                }
#endif
            }

            // Save all of the objects which we called OnAssemblyUnload on so that we can call
            // OnAssemblyReload when hotreload reloads.
            GCHelperHotReloadData hotReloadData = HotReload.Data.Create<GCHelperHotReloadData>();
            foreach (IntPtr address in allUnloadOrReloadReferences)
            {
                hotReloadData.Objects.Add(new FWeakObjectPtr(address));
            }

            Native_GCHelper.Clear();
        }

        internal static void OnReload()
        {
            GCHelperHotReloadData hotReloadData = HotReload.Data.Get<GCHelperHotReloadData>();
            if (hotReloadData != null)
            {
                foreach (FWeakObjectPtr weakObj in hotReloadData.Objects)
                {
                    if (weakObj.IsValid())
                    {
                        UObject obj = GCHelper.Find(weakObj.GetPtr());
                        if (obj != null)
                        {
                            obj.OnAssemblyReload();
                        }
                    }
                }
            }
        }

        class GCHelperHotReloadData : HotReload.DataItem
        {
            public List<FWeakObjectPtr> Objects = new List<FWeakObjectPtr>();

            public override void Load()
            {
                int count = ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    Objects.Add(new FWeakObjectPtr()
                    {
                        ObjectIndex = ReadInt32(),
                        ObjectSerialNumber = ReadInt32()
                    });
                }
            }

            public override void Save()
            {
                WriteInt32(Objects.Count);
                foreach (FWeakObjectPtr weakObject in Objects)
                {
                    WriteInt32(weakObject.ObjectIndex);
                    WriteInt32(weakObject.ObjectSerialNumber);
                }
            }
        }

        class UObjectRefPool
        {
            private Stack<UObjectRef> pool = new Stack<UObjectRef>();

            public UObjectRef New(IntPtr native, Type type, bool isKnownType, int internalIndex)
            {
                UObjectRef result = null;
                if (pool.Count > 0)
                {
                    result = pool.Pop();
                }
                else
                {
                    result = new UObjectRef();
                }
                result.Initialize(native, type, isKnownType, internalIndex);
                return result;
            }

            public void ReturnObject(UObjectRef obj)
            {
                obj.Reset();

                // TODO: Limit the pool size?
                pool.Push(obj);
            }
        }
    }

    public class UObjectRef
    {
        public IntPtr Native;
        public GCHandle ManagedHandle;
        public UObject Managed;
        public bool IsKnownType;
        public uint Id;
        /// <summary>
        /// UObjectBase::InternalIndex
        /// </summary>
        public int InternalIndex;
        public event UObjectRefDestroyedHandler OnDestroyed;

        private static uint sid = 1;

        public void Initialize(IntPtr native, Type type, bool isKnownType, int internalIndex)
        {
            // Something is still holding on this obj ref
            Debug.Assert(OnDestroyed == null);

            InternalIndex = internalIndex;

            // If this loops around things could potentially break down if some objects have
            // extremely long lifetime and others have a short lifetime (this id is mostly just
            // used for equality checks)
            unchecked
            {
                Id = sid++;
            }

            Native = native;
            IsKnownType = isKnownType;

            // TODO: Replace this with lambda compiled constructor as it is is faster
            Managed = (UObject)Activator.CreateInstance(type);
            Managed.objRef = this;
            Managed.objRefId = Id;
            Managed.Address = native;

            // Call the regular Initialize() method unless this object is a managed object currently being initialized
            // (in which case Initialize(objectInitializer) will be called after the managed object has been created)
            if (GCHelper.ManagedObjectBeingInitialized != native)
            {
                Managed.Initialize();
            }

            // A handle to itself for GCHelper C# <-> GCHelper C++
            // Do we need to use Pinned here? Probably.
            ManagedHandle = GCHandle.Alloc(this, GCHandleType.Weak);
        }

        public void Reset()
        {
            if (OnDestroyed != null)
            {
                OnDestroyed(this);
            }

            // Something is still holding on this obj ref
            Debug.Assert(OnDestroyed == null);

            Native = IntPtr.Zero;
            ManagedHandle = default(GCHandle);// Is there any reason to clear this? If we change this change gcHandle.Free();
            Managed = null;
            IsKnownType = false;
            Id = 0;
            OnDestroyed = null;
        }
    }

    public delegate void UObjectRefDestroyedHandler(UObjectRef objRef);
}

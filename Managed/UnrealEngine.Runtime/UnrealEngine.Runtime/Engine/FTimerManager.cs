using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    // This wrapper is somewhat similar to UKismetSystemLibrary's access to FTimerManager (UFunction only)

    // Note: This shouldn't require any special unloading on hot-reload as we bind to UFunction instances which
    // should stay valid between appdomains.

    /// <summary>
    /// Class to globally manage timers.<para/>
    /// USharp only supports UFunction delegates which are bound to UObject instances. This is because arbitary 
    /// delegates would be invalidated on HotReload. Additional support would be required to maintain and remove 
    /// arbitary delegates.
    /// </summary>
    public class FTimerManager
    {
        /// <summary>
        /// The address of the native FTimerManager
        /// </summary>
        public IntPtr Address { get; private set; }

        /// <summary>
        /// The UObject which owns the native FTimerManager (only tracks UWorld, UGameInstance, UEditorEngine)
        /// </summary>
        public UObject Owner { get; private set; }

        /// <summary>
        /// GEditor->GetTimerManager()
        /// </summary>
        public static FTimerManager EngineEditor
        {
            get
            {
                if (FBuild.WithEditor)
                {
                    IntPtr editor = FGlobals.GEditor;
                    if (editor != IntPtr.Zero)
                    {
                        return FTimerManagerCache.GetManager(Native_UEditorEngine.GetTimerManager(editor));
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// GEditor->GetPIEWorldContext().World()->GetTimerManager()
        /// </summary>
        public static FTimerManager PIEWorld
        {
            get
            {
                if (FBuild.WithEditor)
                {
                    IntPtr editor = FGlobals.GEditor;
                    if (editor != IntPtr.Zero)
                    {
                        FWorldContextPtr worldContextPtr = new FWorldContextPtr(
                                Native_UEditorEngine.GetPIEWorldContext(editor));
                        if (worldContextPtr.Address != IntPtr.Zero)
                        {
                            IntPtr world = worldContextPtr.World();
                            if (world != IntPtr.Zero)
                            {
                                return FTimerManagerCache.GetManager(Native_UWorld.GetTimerManager(world));
                            }
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// GEngine->GetWorld()->GetTimerManager()
        /// </summary>
        public static FTimerManager EngineWorld
        {
            get
            {
                IntPtr engine = FGlobals.GEngine;
                if (engine != IntPtr.Zero)
                {
                    IntPtr world = Native_UObject.GetWorld(engine);
                    if (world != IntPtr.Zero)
                    {
                        return FTimerManagerCache.GetManager(Native_UWorld.GetTimerManager(world));
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// ((UGameEngine*)GEngine)->GameInstance->GetTimerManager()
        /// </summary>
        public static FTimerManager GameInstance
        {
            get
            {
                IntPtr engine = FGlobals.GEngine;
                if (engine != IntPtr.Zero && Classes.UGameEngine != IntPtr.Zero &&
                    Native_UObjectBaseUtility.IsA(engine, Classes.UGameEngine))
                {
                    IntPtr gameInstance = Native_UGameEngine.Get_GameInstance(engine);
                    if (gameInstance != IntPtr.Zero)
                    {
                        return FTimerManagerCache.GetManager(Native_UGameInstance.GetTimerManager(gameInstance));
                    }
                }
                return null;
            }
        }

        internal FTimerManager(IntPtr address)
        {
            Address = address;
        }

        public static FTimerManager GetManager(IntPtr address)
        {
            return FTimerManagerCache.GetManager(address);
        }

        public void Tick(float deltaTime)
        {
            Native_FTimerManager.Tick(Address, deltaTime);
        }

        /// <summary>
        /// Sets a timer to call the given native function at a set interval. If a timer is already set
        /// for this delegate, it will update the current timer to the new parameters and reset its
        /// elapsed time to 0.
        /// </summary>
        /// <param name="obj">Object to call the timer function on.</param>
        /// <param name="functionName">Method to call when timer fires (must be a UFunction method).</param>
        /// <param name="time">The amount of time between set and firing. If &lt;= 0.f, clears existing timers.</param>
        /// <param name="looping">true to keep firing at Rate intervals, false to fire only once.</param>
        /// <param name="firstDelay">The time for the first iteration of a looping timer. If &lt; 0.f inRate will be used.</param>
        /// <returns>The handle for the timer.</returns>
        public FTimerHandle SetTimer(UObject obj, string functionName, float time, bool looping = false, float firstDelay = -1.0f)
        {
            FTimerHandle handle = default(FTimerHandle);
            SetTimer(ref handle, obj, new FName(functionName), time, looping, firstDelay);
            return handle;
        }

        /// <summary>
        /// Sets a timer to call the given native function at a set interval. If a timer is already set
        /// for this delegate, it will update the current timer to the new parameters and reset its
        /// elapsed time to 0.
        /// </summary>
        /// <param name="obj">Object to call the timer function on.</param>
        /// <param name="function">Method to call when timer fires (must be a UFunction method).</param>
        /// <param name="time">The amount of time between set and firing. If &lt;= 0.f, clears existing timers.</param>
        /// <param name="looping">true to keep firing at Rate intervals, false to fire only once.</param>
        /// <param name="firstDelay">The time for the first iteration of a looping timer. If &lt; 0.f inRate will be used.</param>
        /// <returns>The handle for the timer.</returns>
        public FTimerHandle SetTimer(UObject obj, FSimpleDelegate function, float time, bool looping = false, float firstDelay = -1.0f)
        {
            FTimerHandle handle = default(FTimerHandle);
            SetTimer(ref handle, obj, GetFunctionName(obj, function), time, looping, firstDelay);
            return handle;
        }

        /// <summary>
        /// Sets a timer to call the given native function at a set interval. If a timer is already set
        /// for this delegate, it will update the current timer to the new parameters and reset its
        /// elapsed time to 0.
        /// </summary>
        /// <param name="obj">Object to call the timer function on.</param>
        /// <param name="functionName">Method to call when timer fires (must be a UFunction method).</param>
        /// <param name="time">The amount of time between set and firing. If &lt;= 0.f, clears existing timers.</param>
        /// <param name="looping">true to keep firing at Rate intervals, false to fire only once.</param>
        /// <param name="firstDelay">The time for the first iteration of a looping timer. If &lt; 0.f inRate will be used.</param>
        /// /// <returns>The handle for the timer.</returns>
        public FTimerHandle SetTimer(UObject obj, FName functionName, float time, bool looping, float firstDelay)
        {
            FTimerHandle handle = default(FTimerHandle);
            SetTimer(ref handle, obj, functionName, time, looping, firstDelay);
            return handle;
        }

        /// <summary>
        /// Sets a timer to call the given native function at a set interval. If a timer is already set
        /// for this delegate, it will update the current timer to the new parameters and reset its
        /// elapsed time to 0.
        /// </summary>
        /// <param name="inOutHandle">Handle to identify this timer. If it is invalid when passed in it will be made into a valid handle.</param>
        /// <param name="obj">Object to call the timer function on.</param>
        /// <param name="functionName">Method to call when timer fires (must be a UFunction method).</param>
        /// <param name="time">The amount of time between set and firing. If &lt;= 0.f, clears existing timers.</param>
        /// <param name="looping">true to keep firing at Rate intervals, false to fire only once.</param>
        /// <param name="firstDelay">The time for the first iteration of a looping timer. If &lt; 0.f inRate will be used.</param>
        public void SetTimer(ref FTimerHandle inOutHandle, UObject obj, string functionName, float time, bool looping = false, float firstDelay = -1.0f)
        {
            SetTimer(ref inOutHandle, obj, new FName(functionName), time, looping, firstDelay);
        }

        /// <summary>
        /// Sets a timer to call the given native function at a set interval. If a timer is already set
        /// for this delegate, it will update the current timer to the new parameters and reset its
        /// elapsed time to 0.
        /// </summary>
        /// <param name="inOutHandle">Handle to identify this timer. If it is invalid when passed in it will be made into a valid handle.</param>
        /// <param name="obj">Object to call the timer function on.</param>
        /// <param name="function">Method to call when timer fires (must be a UFunction method).</param>
        /// <param name="time">The amount of time between set and firing. If &lt;= 0.f, clears existing timers.</param>
        /// <param name="looping">true to keep firing at Rate intervals, false to fire only once.</param>
        /// <param name="firstDelay">The time for the first iteration of a looping timer. If &lt; 0.f inRate will be used.</param>
        public void SetTimer(ref FTimerHandle inOutHandle, UObject obj, FSimpleDelegate function, float time, bool looping = false, float firstDelay = -1.0f)
        {
            SetTimer(ref inOutHandle, obj, GetFunctionName(obj, function), time, looping, firstDelay);
        }

        /// <summary>
        /// Sets a timer to call the given native function at a set interval. If a timer is already set
        /// for this delegate, it will update the current timer to the new parameters and reset its
        /// elapsed time to 0.
        /// </summary>
        /// <param name="inOutHandle">Handle to identify this timer. If it is invalid when passed in it will be made into a valid handle.</param>
        /// <param name="obj">Object to call the timer function on.</param>
        /// <param name="functionName">Method to call when timer fires (must be a UFunction method).</param>
        /// <param name="time">The amount of time between set and firing. If &lt;= 0.f, clears existing timers.</param>
        /// <param name="looping">true to keep firing at Rate intervals, false to fire only once.</param>
        /// <param name="firstDelay">The time for the first iteration of a looping timer. If &lt; 0.f inRate will be used.</param>
        public void SetTimer(ref FTimerHandle inOutHandle, UObject obj, FName functionName, float time, bool looping, float firstDelay)
        {
            if (ValidateFunction(obj, functionName))
            {
                FScriptDelegate del = new FScriptDelegate(obj, functionName);
                Native_FTimerManager.SetTimer(Address, ref inOutHandle, ref del, time, looping, firstDelay);
            }
        }

        /// <summary>
        /// Sets a timer to call the given native function on the next tick
        /// (this could be called RunFunctionOnNextTick as it isn't really timer related (aside from being done by FTimerManager)).
        /// </summary>
        /// <param name="obj">Object to call the timer function on.</param>
        /// <param name="function">Method to call when timer fires.</param>
        public void SetTimerForNextTick(UObject obj, FSimpleDelegate function)
        {
            SetTimerForNextTick(obj, GetFunctionName(obj, function));
        }

        /// <summary>
        /// Sets a timer to call the given native function on the next tick
        /// (this could be called RunFunctionOnNextTick as it isn't really timer related (aside from being done by FTimerManager)).
        /// </summary>
        /// <param name="obj">Object to call the timer function on.</param>
        /// <param name="functionName">Method to call when timer fires.</param>
        public void SetTimerForNextTick(UObject obj, string functionName)
        {
            SetTimerForNextTick(obj, new FName(functionName));
        }

        /// <summary>
        /// Sets a timer to call the given native function on the next tick
        /// (this could be called RunFunctionOnNextTick as it isn't really timer related (aside from being done by FTimerManager)).
        /// </summary>
        /// <param name="obj">Object to call the timer function on.</param>
        /// <param name="functionName">Method to call when timer fires.</param>
        public void SetTimerForNextTick(UObject obj, FName functionName)
        {
            if (ValidateFunction(obj, functionName))
            {
                FScriptDelegate del = new FScriptDelegate(obj, functionName);
                Native_FTimerManager.SetTimerForNextTick(Address, ref del);
            }
        }

        /// <summary>
        /// Clears a previously set timer, identical to calling SetTimer() with a &lt;= 0.f rate.
        /// Invalidates the timer handle as it should no longer be used.
        /// </summary>
        /// <param name="handle">The handle of the timer to clear.</param>
        public void ClearTimer(ref FTimerHandle handle)
        {
            Native_FTimerManager.ClearTimer(Address, ref handle);
        }

        /// <summary>
        /// Clears all timers that are bound to functions on the given object.
        /// </summary>
        public void ClearAllTimersForObject(UObject obj)
        {
            Native_FTimerManager.ClearAllTimersForObject(Address, obj.Address);
        }

        /// <summary>
        /// Pauses a previously set timer.
        /// </summary>
        /// <param name="handle">The handle of the timer to pause.</param>
        public void PauseTimer(FTimerHandle handle)
        {
            Native_FTimerManager.PauseTimer(Address, ref handle);
        }

        /// <summary>
        /// Unpauses a previously set timer.
        /// </summary>
        /// <param name="handle">The handle of the timer to unpause.</param>
        public void UnPauseTimer(FTimerHandle handle)
        {
            Native_FTimerManager.UnPauseTimer(Address, ref handle);
        }

        /// <summary>
        /// Gets the current rate (time between activations) for the specified timer.
        /// </summary>
        /// <param name="handle">The handle of the timer to return the rate of.</param>
        /// <returns>The current rate or -1.f if timer does not exist.</returns>
        public float GetTimerRate(FTimerHandle handle)
        {
            return Native_FTimerManager.GetTimerRate(Address, ref handle);
        }

        /// <summary>
        /// Returns true if the specified timer exists and is not paused
        /// </summary>
        /// <param name="handle">The handle of the timer to check for being active.</param>
        /// <returns>true if the timer exists and is active, false otherwise.</returns>
        public bool IsTimerActive(FTimerHandle handle)
        {
            return Native_FTimerManager.IsTimerActive(Address, ref handle);
        }

        /// <summary>
        /// Returns true if the specified timer exists and is paused
        /// </summary>
        /// <param name="handle">The handle of the timer to check for being paused.</param>
        /// <returns>true if the timer exists and is paused, false otherwise.</returns>
        public bool IsTimerPaused(FTimerHandle handle)
        {
            return Native_FTimerManager.IsTimerPaused(Address, ref handle);
        }

        /// <summary>
        /// Returns true if the specified timer exists and is pending
        /// </summary>
        /// <param name="handle">The handle of the timer to check for being pending.</param>
        /// <returns>true if the timer exists and is pending, false otherwise.</returns>
        public bool IsTimerPending(FTimerHandle handle)
        {
            return Native_FTimerManager.IsTimerPending(Address, ref handle);
        }

        /// <summary>
        /// Returns true if the specified timer exists
        /// </summary>
        /// <param name="handle">The handle of the timer to check for existence.</param>
        /// <returns>true if the timer exists, false otherwise.</returns>
        public bool TimerExists(FTimerHandle handle)
        {
            return Native_FTimerManager.TimerExists(Address, ref handle);
        }

        /// <summary>
        /// Gets the current elapsed time for the specified timer.
        /// </summary>
        /// <param name="handle">The handle of the timer to check the elapsed time of.</param>
        /// <returns>The current time elapsed or -1.f if the timer does not exist.</returns>
        public float GetTimerElapsed(FTimerHandle handle)
        {
            return Native_FTimerManager.GetTimerElapsed(Address, ref handle);
        }

        /// <summary>
        /// Gets the time remaining before the specified timer is called
        /// </summary>
        /// <param name="handle">The handle of the timer to check the remaining time of.</param>
        /// <returns>The current time remaining, or -1.f if timer does not exist</returns>
        public float GetTimerRemaining(FTimerHandle handle)
        {
            return Native_FTimerManager.GetTimerRemaining(Address, ref handle);
        }

        public bool HasBeenTickedThisFrame()
        {
            return Native_FTimerManager.HasBeenTickedThisFrame(Address);
        }

        public FTimerHandle FindTimerHandle(UObject obj, string functionName)
        {
            return FindTimerHandle(obj, new FName(functionName));
        }

        public FTimerHandle FindTimerHandle(UObject obj, FSimpleDelegate function)
        {
            return FindTimerHandle(obj, GetFunctionName(obj, function));
        }

        public FTimerHandle FindTimerHandle(UObject obj, FName functionName)
        {
            FTimerHandle result = default(FTimerHandle);
            if (obj != null && functionName != FName.None)
            {
                FScriptDelegate del = new FScriptDelegate(obj, functionName);
                Native_FTimerManager.K2_FindDynamicTimerHandle(Address, ref del, ref result);
            }
            return result;
        }

        /// <summary>
        /// Debug command to output info on all timers currently set to the log.
        /// </summary>
        public void ListTimers()
        {
            Native_FTimerManager.ListTimers(Address);
        }

        /// <summary>
        /// Get the current last assigned handle
        /// </summary>
        public static void ValidateHandle(ref FTimerHandle inOutHandle)
        {
            Native_FTimerManager.ValidateHandle(ref inOutHandle);
        }

        // TODO: Change the parameter to use a concrete type UGameInstance
        /// <summary>
        /// Used by the UGameInstance constructor to set this manager's owning game instance.
        /// </summary>
        public void SetGameInstance(UObject gameInstance)
        {
            if (gameInstance != null && gameInstance.GetClass().Address == Classes.UGameInstance)
            {
                Native_FTimerManager.SetGameInstance(Address, gameInstance.Address);
            }
        }

        private static bool ValidateFunction(UObject obj, FName functionName)
        {
            // Same validation that is in UKismetSystemLibrary::K2_SetTimer
            if (obj != null && functionName != FName.None)
            {
                IntPtr functionAddress = Native_UObject.FindFunction(obj.Address, ref functionName);
                if (functionAddress != IntPtr.Zero && Native_UFunction.Get_ParmsSize(functionAddress) > 0)
                {
                    // User passed in a valid function, but one that takes parameters
                    // FTimerDynamicDelegate expects zero parameters and will choke on execution if it tries
                    // to execute a mismatched function
                    FMessage.Log(ELogVerbosity.Warning, "SetTimer passed a function (" +
                        NativeReflection.GetPathName(functionAddress) + ") that expects parameters.");
                    return false;
                }
            }
            return true;
        }

        private static FName GetFunctionName(UObject obj, FSimpleDelegate function)
        {
            Delegate del = function as Delegate;
            if (del != null)
            {
                UObject target = del.Target as UObject;
                if (target != null)// Also check against obj?
                {
                    IntPtr functionAddress = NativeReflection.LookupTable.FindFunction(target, del.Method);
                    FName name;
                    Native_UObjectBase.GetFName(functionAddress, out name);
                    return name;
                }
            }
            return FName.None;
        }
    }

    /// <summary>
    /// Helper for caching FTimerManager instances based on the UObject owner (if any)
    /// </summary>
    internal static class FTimerManagerCache
    {
        // There appears to be 3 locations where FTimerManager instances are held.
        // We need to cache FTimerManager for the lifetime of those objects.
        // UWorld (holds one when UGameInstance is null)
        // UGameInstance
        // UEditorEngine (holds one to manage "all timer delegates")

        struct TimerManagerInfo
        {
            public FWeakObjectPtr Owner;
            public FTimerManager TimeManager;
        }

        // <FWeakObjectPtr, FTimerManager*>
        private static Dictionary<FWeakObjectPtr, IntPtr> timerManagerOwners = new Dictionary<FWeakObjectPtr, IntPtr>();
        // <FTimerManager*, TimerManagerInfo>
        private static Dictionary<IntPtr, TimerManagerInfo> timerManagers = new Dictionary<IntPtr, TimerManagerInfo>();

        // The classes which can own a FTimerManager
        private static bool hasClasses = false;
        private static IntPtr worldClass;
        private static IntPtr gameInstanceClass;
        private static IntPtr editorEngineClass;

        public static FTimerManager GetManager(IntPtr address)
        {
            if (address == IntPtr.Zero)
            {
                return null;
            }

            TimerManagerInfo timerManagerInfo;
            if (timerManagers.TryGetValue(address, out timerManagerInfo))
            {
                return timerManagerInfo.TimeManager;
            }

            UpdateOwnerClasses();

            IntPtr owner = IntPtr.Zero;
            if (owner == IntPtr.Zero && worldClass != IntPtr.Zero)
            {
                foreach (IntPtr world in new NativeReflection.NativeObjectIterator(worldClass))
                {
                    IntPtr timerManager = Native_UWorld.GetTimerManager(world);
                    if (timerManager == address)
                    {
                        IntPtr gameInstance = Native_UWorld.GetGameInstance(world);
                        if (gameInstance != IntPtr.Zero && Native_UGameInstance.GetTimerManager(gameInstance) == address)
                        {
                            owner = gameInstance;
                            break;
                        }
                        else
                        {
                            owner = world;
                            break;
                        }
                    }
                }
            }

            if (owner == IntPtr.Zero && gameInstanceClass != IntPtr.Zero)
            {
                foreach (IntPtr gameInstance in new NativeReflection.NativeObjectIterator(gameInstanceClass))
                {
                    if (Native_UGameInstance.GetTimerManager(gameInstance) == address)
                    {
                        owner = gameInstance;
                        break;
                    }
                }
            }

            if (owner == IntPtr.Zero && editorEngineClass != IntPtr.Zero)
            {
                foreach (IntPtr editorEngine in new NativeReflection.NativeObjectIterator(editorEngineClass))
                {
                    if (Native_UEditorEngine.GetTimerManager(editorEngine) == address)
                    {
                        owner = editorEngine;
                        break;
                    }
                }
            }

            if (owner != IntPtr.Zero)
            {
                FWeakObjectPtr ownerWeakObjPtr = new FWeakObjectPtr();
                ownerWeakObjPtr.Set(owner);
                timerManagerInfo = new TimerManagerInfo();
                timerManagerInfo.Owner = ownerWeakObjPtr;
                timerManagerInfo.TimeManager = new FTimerManager(address);

                timerManagers.Add(address, timerManagerInfo);
                timerManagerOwners[ownerWeakObjPtr] = address;

                return timerManagerInfo.TimeManager;
            }
            else
            {
                return new FTimerManager(address);
            }
        }

        private static void UpdateOwnerClasses()
        {
            if (!hasClasses)
            {
                if (worldClass == IntPtr.Zero)
                {
                    worldClass = UClass.GetClassAddress("/Script/Engine.World");
                }
                if (gameInstanceClass == IntPtr.Zero)
                {
                    gameInstanceClass = UClass.GetClassAddress("/Script/Engine.GameInstance");
                }
                hasClasses = worldClass != IntPtr.Zero && gameInstanceClass != IntPtr.Zero;

                if (FBuild.WithEditor)
                {
                    if (editorEngineClass == IntPtr.Zero)
                    {
                        editorEngineClass = UClass.GetClassAddress("/Script/UnrealEd.EditorEngine");
                    }
                    hasClasses = hasClasses && editorEngineClass != IntPtr.Zero;
                }
            }
        }

        private static void OnPostGarbageCollect()
        {
            List<FWeakObjectPtr> destroyedOwners = null;

            foreach (FWeakObjectPtr obj in timerManagerOwners.Keys)
            {
                if (!obj.IsValid())
                {
                    if (destroyedOwners == null)
                    {
                        destroyedOwners = new List<FWeakObjectPtr>();
                    }
                    destroyedOwners.Add(obj);
                }
            }

            if (destroyedOwners != null)
            {
                foreach (FWeakObjectPtr obj in destroyedOwners)
                {
                    foreach (KeyValuePair<IntPtr, TimerManagerInfo> timerManager in 
                        new Dictionary<IntPtr, TimerManagerInfo>(timerManagers))
                    {
                        if (timerManager.Value.Owner == obj)
                        {
                            timerManagers.Remove(timerManager.Key);
                        }
                    }
                    timerManagerOwners.Remove(obj);
                }
            }
        }
        
        internal static void OnNativeFunctionsRegistered()
        {
            FCoreUObjectDelegates.PostGarbageCollect.Bind(OnPostGarbageCollect);
        }
    }
}

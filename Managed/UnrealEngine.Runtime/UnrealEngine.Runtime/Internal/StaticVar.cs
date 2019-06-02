using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Engine;
using UnrealEngine.Runtime.Native;
using UnrealEngine.UnrealEd;

namespace UnrealEngine.Runtime
{
    public abstract class StaticVar
    {
        public bool IsRegistered { get; private set; }

        public StaticVar()
        {
            Register();
        }

        public virtual void Register()
        {
            if (!IsRegistered)
            {
                StaticVarManager.Vars.Add(this);
                IsRegistered = true;
            }
        }

        public virtual void Unregister()
        {
            if (IsRegistered)
            {
                StaticVarManager.Vars.Remove(this);
                IsRegistered = false;
            }
        }

        /// <summary>
        /// Called when HotReload occurs
        /// </summary>
        public virtual void OnUnload()
        {
        }

        public virtual void OnPIEBegin(bool simulating)
        {
        }

        public virtual void OnPIEEnd(bool simulating)
        {
        }

        public virtual void OnWorldAdded(IntPtr world)
        {
        }

        public virtual void OnWorldDestroyed(IntPtr world)
        {
        }

        public virtual void OnGameInstanceShutdown(IntPtr gameInstance)
        {
        }
    }

    /// <summary>
    /// A static variable which can have a unique value for each UWorld<para/>
    /// NOTE: The value will be reset on hotreload
    /// </summary>
    /// <typeparam name="T">The type of the static variable</typeparam>
    public class WorldStaticVar<T> : StaticVar
    {
        // TODO: Avoid using dictionaries
        protected Dictionary<IntPtr, T> values = new Dictionary<IntPtr, T>();
        protected int worldTypeFlags;

        public GetDefaultValueHandler GetDefaultValue;
        public delegate T GetDefaultValueHandler(UObject world);

        public WorldStaticVar()
        {
        }

        public WorldStaticVar(GetDefaultValueHandler getDefaultValue, params EWorldType[] worldTypes)
        {
            GetDefaultValue = getDefaultValue;
            foreach (EWorldType worldType in worldTypes)
            {
                worldTypeFlags |= (1 << (int)worldType);
            }
        }

        public WorldStaticVar(params EWorldType[] worldTypes)
            : this (null, worldTypes)
        {
        }

        public virtual bool HasValue(UObject worldContextObject)
        {
            IntPtr world = Native_UObject.GetWorld(worldContextObject.Address);
            if (worldTypeFlags == 0)
            {
                return values.ContainsKey(world);
            }
            else if (world != IntPtr.Zero)
            {
                int worldType = (1 << (int)Native_UWorld.Get_WorldType(world));
                if ((worldTypeFlags & worldType) == worldType)
                {
                    return values.ContainsKey(world);
                }
            }
            return false;
        }

        public virtual bool TryGetValue(UObject worldContextObject, out T value)
        {
            IntPtr world = Native_UObject.GetWorld(worldContextObject.Address);
            if (worldTypeFlags == 0)
            {
                if (values.TryGetValue(world, out value))
                {
                    return true;
                }
            }
            else if (world != IntPtr.Zero)
            {
                int worldType = (1 << (int)Native_UWorld.Get_WorldType(world));
                if ((worldTypeFlags & worldType) == worldType)
                {
                    if (values.TryGetValue(world, out value))
                    {
                        return true;
                    }
                }
            }
            value = default(T);
            return false;
        }

        public T Get(UObject worldContextObject)
        {
            T value;
            TryGetValue(worldContextObject, out value);
            return value;
        }

        public virtual bool Set(UObject worldContextObject, T value)
        {
            IntPtr world = Native_UObject.GetWorld(worldContextObject.Address);
            if (worldTypeFlags == 0)
            {
                values[world] = value;
                return true;
            }
            else if (world != IntPtr.Zero)
            {
                int worldType = (1 << (int)Native_UWorld.Get_WorldType(world));
                if ((worldTypeFlags & worldType) == worldType)
                {
                    values[world] = value;
                    return true;
                }
            }
            return false;
        }

        public override void OnWorldAdded(IntPtr world)
        {
            if (GetDefaultValue != null)
            {
                values[world] = GetDefaultValue(GCHelper.Find(world));
            }
        }

        public override void OnWorldDestroyed(IntPtr world)
        {
            values.Remove(world);
        }
    }

    /// <summary>
    /// A static variable which will be reset when PIE (Play In Editor) begins/ends<para/>
    /// NOTE: The value will be reset on hotreload<para/>
    /// </summary>
    /// <typeparam name="T">The type of the static variable</typeparam>
    public class GameStaticVar<T> : StaticVar
    {
        public T Value;

        public GetDefaultValueHandler GetDefaultValue;
        public delegate T GetDefaultValueHandler();

        public GameStaticVar()
        {
        }

        public GameStaticVar(GetDefaultValueHandler getDefaultValue)
        {
            GetDefaultValue = getDefaultValue;
            if (GetDefaultValue != null)
            {
                Value = GetDefaultValue();
            }
        }

        public override void OnPIEBegin(bool simulating)
        {
            if (GetDefaultValue != null)
            {
                Value = GetDefaultValue();
            }
            else
            {
                Value = default(T);
            }
        }

        public override void OnPIEEnd(bool simulating)
        {
            Value = default(T);
        }

        public static implicit operator T(GameStaticVar<T> value)
        {
            return value.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    /// A static variable which can have a unique value for each UGameInstance<para/>
    /// NOTE: The value will be reset on hotreload<para/>
    /// NOTE: As each UGameInstance typically works with a single UWorld this is likely no different than using <see cref="WorldStaticVar{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of the static variable</typeparam>
    public class GameInstanceStaticVar<T> : StaticVar
    {
        // TODO: Avoid using dictionaries
        protected Dictionary<IntPtr, T> values = new Dictionary<IntPtr, T>();

        private IntPtr GetDefaultWorld()
        {
#if WITH_EDITORONLY_DATA
            FWorldContext worldContext = new FWorldContext(Native_UEditorEngine.GetPIEWorldContext(FGlobals.GEditor));
            return worldContext.IsNull ? IntPtr.Zero : worldContext.CurrentWorld;
#else
            return Native_UObject.GetWorld(FGlobals.GEngine);
#endif
        }

        public virtual bool Clear()
        {
            IntPtr world = GetDefaultWorld();
            if (world != IntPtr.Zero)
            {
                IntPtr gameInstance = Native_UWorld.GetGameInstance(world);
                if (gameInstance != IntPtr.Zero)
                {
                    return values.Remove(gameInstance);
                }
            }
            return false;
        }

        public virtual bool HasValue()
        {
            IntPtr world = GetDefaultWorld();
            if (world != IntPtr.Zero)
            {
                IntPtr gameInstance = Native_UWorld.GetGameInstance(world);
                return gameInstance != IntPtr.Zero && values.ContainsKey(gameInstance);
            }
            return false;
        }

        public virtual bool TryGetValue(out T value)
        {
            IntPtr world = GetDefaultWorld();
            if (world != IntPtr.Zero)
            {
                IntPtr gameInstance = Native_UWorld.GetGameInstance(world);
                if (gameInstance != IntPtr.Zero)
                {
                    if (values.TryGetValue(gameInstance, out value))
                    {
                        return true;
                    }
                }
            }
            value = default(T);
            return false;
        }

        public T Get()
        {
            T result;
            TryGetValue(out result);
            return result;
        }

        public T Set(T value)
        {
            IntPtr world = GetDefaultWorld();
            if (world != IntPtr.Zero)
            {
                IntPtr gameInstance = Native_UWorld.GetGameInstance(world);
                if (gameInstance != IntPtr.Zero)
                {
                    values[gameInstance] = value;
                }
            }
            return value;
        }

        public virtual bool Clear(UObject worldContextObject)
        {
            IntPtr gameInstance = Native_UWorld.GetGameInstance(worldContextObject.Address);
            if (gameInstance != IntPtr.Zero)
            {
                return values.Remove(gameInstance);
            }
            return false;
        }

        public virtual bool HasValue(UObject worldContextObject)
        {
            IntPtr gameInstance = Native_UWorld.GetGameInstance(worldContextObject.Address);
            return gameInstance != IntPtr.Zero && values.ContainsKey(gameInstance);
        }

        public virtual bool TryGetValue(UObject worldContextObject, out T value)
        {
            IntPtr gameInstance = Native_UWorld.GetGameInstance(worldContextObject.Address);
            if (gameInstance != IntPtr.Zero)
            {
                if (values.TryGetValue(gameInstance, out value))
                {
                    return true;
                }
            }
            value = default(T);
            return false;
        }

        public T Get(UObject worldContextObject)
        {
            T result;
            TryGetValue(worldContextObject, out result);
            return result;
        }

        public T Set(UObject worldContextObject, T value)
        {
            IntPtr gameInstance = Native_UWorld.GetGameInstance(worldContextObject.Address);
            if (gameInstance != IntPtr.Zero)
            {
                values[gameInstance] = value;
            }
            return value;
        }

        public override void OnGameInstanceShutdown(IntPtr gameInstance)
        {
            values.Remove(gameInstance);
        }
    }

    public static class StaticVarManager
    {
        internal static List<StaticVar> Vars = new List<StaticVar>();

        internal static void OnNativeFunctionsRegistered()
        {
            FEditorDelegates.PreBeginPIE.Bind(OnPreBeginPIE);
            FEditorDelegates.EndPIE.Bind(OnEndPIE);

            // NOTE: UEngine::OnWorldAdded / UEngine::OnWorldDestroyed aren't great for tracking PIE worlds as they
            //       are created by duplication. Use FWorldDelegates events instead.

            FWorldDelegates.OnPostWorldCreation.Bind(OnPostWorldCreation);
            FWorldDelegates.OnPostWorldCleanup.Bind(OnPostWorldCleanup);
        }

        internal static void OnUnload()
        {
            // TODO: Only call this on types which override this function
            foreach (StaticVar staticVar in Vars)
            {
                staticVar.OnUnload();
            }
        }

        private static void OnPreBeginPIE(bool simulating)
        {
            // TODO: Only call this on types which override this function
            foreach (StaticVar staticVar in Vars)
            {
                staticVar.OnPIEBegin(simulating);
            }
        }

        private static void OnEndPIE(bool simulating)
        {
            // TODO: Only call this on types which override this function
            foreach (StaticVar staticVar in Vars)
            {
                staticVar.OnPIEEnd(simulating);
            }
        }

        private static void OnPostWorldCreation(IntPtr world)
        {
            // TODO: Only call this on types which override this function
            foreach (StaticVar staticVar in Vars)
            {
                staticVar.OnWorldAdded(world);
            }
        }

        private static void OnPostWorldCleanup(IntPtr world, bool sessionEnded, bool cleanupResources)
        {
            // At this point UGameInstance::WorldContext should have been set to nullptr. While WorldContext being nullptr
            // doesn't mean the UGameInstance is nessesarily "destroyed" but there isn't any code which reinitializes 
            // UGameInstance by calling UGameInstance::InitializeStandalone / UGameInstance::InitializeForMinimalNetRPC
            // 
            // UEditorEngine::TeardownPlaySession()
            // {
            //     UGameInstance::Shutdown(); - sets this->WorldContext = nullptr
            //     PlayWorld->CleanupWorld(); - called shortly after Shutdown()
            //     {
            //         // These delegates are the first available delegates we can use after UGameInstance state is invalidated
            //         FWorldDelegates::OnWorldCleanup
            //         FWorldDelegates::OnPostWorldCleanup
            //     }
            // }

            // TODO: Only call this on types which override this function
            IntPtr gameInstance = Native_UWorld.GetGameInstance(world);
            foreach (StaticVar staticVar in Vars)
            {
                if (gameInstance != IntPtr.Zero)
                {
                    staticVar.OnGameInstanceShutdown(gameInstance);
                }
                staticVar.OnWorldDestroyed(world);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    /// <summary>
    /// FWorldContext
    /// A context for dealing with UWorlds at the engine level. As the engine brings up and destroys world, we need a way to keep straight
    /// what world belongs to what.
    /// 
    /// WorldContexts can be thought of as a track. By default we have 1 track that we load and unload levels on. Adding a second context is adding
    /// a second track; another track of progression for worlds to live on. 
    /// 
    /// For the GameEngine, there will be one WorldContext until we decide to support multiple simultaneous worlds.
    /// For the EditorEngine, there may be one WorldContext for the EditorWorld and one for the PIE World.
    /// 
    /// FWorldContext provides both a way to manage 'the current PIE UWorld*' as well as state that goes along with connecting/travelling to 
    /// new worlds.
    /// 
    /// FWorldContext should remain internal to the UEngine classes. Outside code should not keep pointers or try to manage FWorldContexts directly.
    /// Outside code can steal deal with UWorld*, and pass UWorld*s into Engine level functions. The Engine code can look up the relevant context 
    /// for a given UWorld*.
    /// 
    /// For convenience, FWorldContext can maintain outside pointers to UWorld*s. For example, PIE can tie UWorld* UEditorEngine::PlayWorld to the PIE
    /// world context. If the PIE UWorld changes, the UEditorEngine::PlayWorld pointer will be automatically updated. This is done with AddRef() and
    /// SetCurrentWorld().
    /// </summary>
    public struct FWorldContext
    {
        public IntPtr Address;

        public bool IsNull
        {
            get { return Address == IntPtr.Zero; }
        }

        public EWorldType WorldType
        {
            get { return (EWorldType)Native_FWorldContext.Get_WorldType(Address); }
        }

        public FName ContextHandle
        {
            get
            {
                FName result;
                Native_FWorldContext.Get_ContextHandle(Address, out result);
                return result;
            }
        }

        /// <summary>
        /// URL to travel to for pending client connect
        /// </summary>
        public string TravelURL
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FWorldContext.Get_TravelURL(Address, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// TravelType for pending client connects
        /// </summary>
        public ETravelType TravelType
        {
            get { return (ETravelType)Native_FWorldContext.Get_TravelType(Address); }
        }

        /// <summary>
        /// UGameViewportClient*
        /// </summary>
        public IntPtr GameViewport
        {
            get { return Native_FWorldContext.Get_GameViewport(Address); }
        }

        /// <summary>
        /// UGameInstance*
        /// </summary>
        public IntPtr OwningGameInstance
        {
            get { return Native_FWorldContext.Get_OwningGameInstance(Address); }
        }

        /// <summary>
        /// The PIE instance of this world, -1 is default
        /// </summary>
        public int PIEInstance
        {
            get { return Native_FWorldContext.Get_PIEInstance(Address); }
        }

        /// <summary>
        /// The Prefix in front of PIE level names, empty is defaults
        /// </summary>
        public string PIEPrefix
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FWorldContext.Get_PIEPrefix(Address, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Is this running as a dedicated server
        /// </summary>
        public bool RunAsDedicated
        {
            get { return Native_FWorldContext.Get_RunAsDedicated(Address); }
        }

        /// <summary>
        /// Is this world context waiting for an online login to complete (for PIE)
        /// </summary>
        public bool WaitingOnOnlineSubsystem
        {
            get { return Native_FWorldContext.Get_bWaitingOnOnlineSubsystem(Address); }
        }

        /// <summary>
        /// Handle to this world context's audio device.
        /// </summary>
        public uint AudioDeviceHandle
        {
            get { return Native_FWorldContext.Get_AudioDeviceHandle(Address); }
        }

        /// <summary>
        /// Set CurrentWorld and update external reference pointers to reflect this
        /// </summary>
        public void SetCurrentWorld(IntPtr world)
        {
            Native_FWorldContext.SetCurrentWorld(Address, world);
        }
        
        public IntPtr CurrentWorld
        {
            get { return Native_FWorldContext.World(Address); }
        }
        
        public FWorldContext(IntPtr address)
        {
            Address = address;
        }

        public static FWorldContext[] GetWorldContexts()
        {
            using (TArrayUnsafe<IntPtr> resultUnsafe = new TArrayUnsafe<IntPtr>())
            {
                Native_UEngine.GetWorldContexts(resultUnsafe.Address);

                int count = resultUnsafe.Count;
                FWorldContext[] result = new FWorldContext[count];
                for (int i = 0; i < count; i++)
                {
                    IntPtr worldContextPtr = resultUnsafe[i];
                    Debug.Assert(worldContextPtr != IntPtr.Zero);
                    result[i] = new FWorldContext(worldContextPtr);
                }
                return result;
            }
        }
    }
}

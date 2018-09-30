using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public struct FWorldContextPtr
    {
        public IntPtr Address;

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
        
        public IntPtr World()
        {
            return Native_FWorldContext.World(Address);
        }
        
        public FWorldContextPtr(IntPtr address)
        {
            Address = address;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    // Engine\Source\Runtime\Engine\Classes\Engine\World.h

    /// <summary>
    /// World delegates
    /// </summary>
    public static class FWorldDelegates
    {
        static FWorldDelegates()
        {
            HotReload.RegisterNativeDelegateManager(typeof(FWorldDelegates));
        }

        public static OnWorldCleanupHandler OnWorldCleanup = new OnWorldCleanupHandler();
        public class OnWorldCleanupHandler : NativeMulticastDelegate<Native_FWorldDelegates.Del_WorldCleanupEvent, Native_FWorldDelegates.Del_Reg_OnWorldCleanup, OnWorldCleanupHandler.Signature>
        {
            public delegate void Signature(IntPtr world, bool sessionEnded, bool cleanupResources);
            private void NativeCallback(IntPtr world, csbool sessionEnded, csbool cleanupResources)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(world, sessionEnded, cleanupResources);
                }
            }
        }

        public static OnPostWorldCleanupHandler OnPostWorldCleanup = new OnPostWorldCleanupHandler();
        public class OnPostWorldCleanupHandler : NativeMulticastDelegate<Native_FWorldDelegates.Del_WorldCleanupEvent, Native_FWorldDelegates.Del_Reg_OnPostWorldCleanup, OnPostWorldCleanupHandler.Signature>
        {
            public delegate void Signature(IntPtr world, bool sessionEnded, bool cleanupResources);
            private void NativeCallback(IntPtr world, csbool sessionEnded, csbool cleanupResources)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(world, sessionEnded, cleanupResources);
                }
            }
        }
    }
}

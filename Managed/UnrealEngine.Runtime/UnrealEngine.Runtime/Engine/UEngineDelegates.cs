using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    // Engine\Source\Runtime\Engine\Classes\Engine\Engine.h

    // This isn't actually a class/file in UE4, this exports delegate functions from UEngine out here to contain
    // these messy delegates in one place

    /// <summary>
    /// Delegates used by UEngine. This assumes GEngine/GEditor has a static address for the duration of the application.
    /// </summary>
    public static class UEngineDelegates
    {
        static UEngineDelegates()
        {
            HotReload.RegisterNativeDelegateManager(typeof(UEngineDelegates));
        }

        /// <summary>
        /// Triggered when a world is added.
        /// </summary>
        public static OnWorldAddedHandler OnWorldAdded = new OnWorldAddedHandler();
        public class OnWorldAddedHandler : NativeMulticastDelegate<Native_UEngineDelegates.Del_OnWorldAdded, Native_UEngineDelegates.Del_Reg_OnWorldAdded, OnWorldAddedHandler.Signature>
        {
            public delegate void Signature(IntPtr world);
            private void NativeCallback(IntPtr world)
            {
                try
                {
                    var evnt = managed.Delegate;
                    if (evnt != null)
                    {
                        evnt(world);
                    }
                }
                catch (Exception e)
                {
                    FMessage.LogDelegateException(e);
                }
            }
        }

        /// <summary>
        /// Triggered when a world is destroyed.
        /// </summary>
        public static OnWorldDestroyedHandler OnWorldDestroyed = new OnWorldDestroyedHandler();
        public class OnWorldDestroyedHandler : NativeMulticastDelegate<Native_UEngineDelegates.Del_OnWorldDestroyed, Native_UEngineDelegates.Del_Reg_OnWorldDestroyed, OnWorldDestroyedHandler.Signature>
        {
            public delegate void Signature(IntPtr world);
            private void NativeCallback(IntPtr world)
            {
                try
                {
                    var evnt = managed.Delegate;
                    if (evnt != null)
                    {
                        evnt(world);
                    }
                }
                catch (Exception e)
                {
                    FMessage.LogDelegateException(e);
                }
            }
        }
    }
}

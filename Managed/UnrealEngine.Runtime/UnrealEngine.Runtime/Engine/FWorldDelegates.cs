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

        /// <summary>
        /// Callback for world creation. (called by UWorld::UWorld constructor)
        /// </summary>
        public static OnPostWorldCreationHandler OnPostWorldCreation = new OnPostWorldCreationHandler();
        public class OnPostWorldCreationHandler : NativeMulticastDelegate<Native_FWorldDelegates.Del_WorldEvent, Native_FWorldDelegates.Del_Reg_OnPostWorldCreation, OnPostWorldCreationHandler.Signature>
        {
            public delegate void Signature(IntPtr world);
            private void NativeCallback(IntPtr world)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(world);
                }
            }
        }

        /// <summary>
        /// Callback for world initialization (pre) (called by UWorld::InitWorld)
        /// </summary>
        public static OnPreWorldInitializationHandler OnPreWorldInitialization = new OnPreWorldInitializationHandler();
        public class OnPreWorldInitializationHandler : NativeMulticastDelegate<Native_FWorldDelegates.Del_WorldInitializationEvent, Native_FWorldDelegates.Del_Reg_OnPreWorldInitialization, OnPreWorldInitializationHandler.Signature>
        {
            public delegate void Signature(IntPtr world, IntPtr ivs);
            private void NativeCallback(IntPtr world, IntPtr ivs)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(world, ivs);
                }
            }
        }

        /// <summary>
        /// Callback for world initialization (post) (called by UWorld::InitWorld)
        /// </summary>
        public static OnPostWorldInitializationHandler OnPostWorldInitialization = new OnPostWorldInitializationHandler();
        public class OnPostWorldInitializationHandler : NativeMulticastDelegate<Native_FWorldDelegates.Del_WorldInitializationEvent, Native_FWorldDelegates.Del_Reg_OnPostWorldInitialization, OnPostWorldInitializationHandler.Signature>
        {
            public delegate void Signature(IntPtr world, IntPtr ivs);
            private void NativeCallback(IntPtr world, IntPtr ivs)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(world, ivs);
                }
            }
        }

        /// <summary>
        /// Post duplication event. (called by UWorld::PostDuplicate)
        /// </summary>
        public static OnPostDuplicateHandler OnPostDuplicate = new OnPostDuplicateHandler();
        public class OnPostDuplicateHandler : NativeMulticastDelegate<Native_FWorldDelegates.Del_WorldPostDuplicateEvent, Native_FWorldDelegates.Del_Reg_OnPostDuplicate, OnPostDuplicateHandler.Signature>
        {
            public delegate void Signature(IntPtr world, bool duplicateForPIE, IntPtr replacementMap, IntPtr objectsToFixReferences);
            private void NativeCallback(IntPtr world, csbool duplicateForPIE, IntPtr replacementMap, IntPtr objectsToFixReferences)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(world, duplicateForPIE, replacementMap, objectsToFixReferences);
                }
            }
        }

        /// <summary>
        /// Callback for world cleanup start (called by UWorld::CleanupWorld)
        /// </summary>
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

        /// <summary>
        /// Callback for world cleanup end (called by UWorld::CleanupWorld)
        /// </summary>
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

        /// <summary>
        /// Callback for world destruction (only called for initialized worlds) (called by UWorld::FinishDestroy)<para/>
        /// Non initialized worlds include the CDO and worlds which get loaded due to level streaming.
        /// </summary>
        public static OnPreWorldFinishDestroyHandler OnPreWorldFinishDestroy = new OnPreWorldFinishDestroyHandler();
        public class OnPreWorldFinishDestroyHandler : NativeMulticastDelegate<Native_FWorldDelegates.Del_WorldEvent, Native_FWorldDelegates.Del_Reg_OnPreWorldFinishDestroy, OnPreWorldFinishDestroyHandler.Signature>
        {
            public delegate void Signature(IntPtr world);
            private void NativeCallback(IntPtr world)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(world);
                }
            }
        }
    }
}

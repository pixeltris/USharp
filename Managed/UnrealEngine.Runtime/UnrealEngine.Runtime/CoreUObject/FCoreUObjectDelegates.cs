using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Global CoreUObject delegates
    /// </summary>
    public static class FCoreUObjectDelegates
    {
        static FCoreUObjectDelegates()
        {
            HotReload.RegisterNativeDelegateManager(typeof(FCoreUObjectDelegates));
        }

        /// <summary>
        /// Called when any object is modified at all
        /// </summary>
        //WITH_EDITOR
        public static OnObjectModifiedHandler OnObjectModified = new OnObjectModifiedHandler();
        public class OnObjectModifiedHandler : NativeMulticastDelegate<Native_FCoreUObjectDelegates.Del_OnObjectModified, Native_FCoreUObjectDelegates.Del_Reg_OnObjectModified, OnObjectModifiedHandler.Signature>
        {
            public delegate void Signature(UObject objectBeingModified);
            private void NativeCallback(IntPtr objectBeingModified)
            {
                try
                {
                    var evnt = managed.Delegate;
                    if (evnt != null)
                    {
                        evnt(GCHelper.Find(objectBeingModified));
                    }
                }
                catch (Exception e)
                {
                    FMessage.LogDelegateException(e);
                }
            }
        }

        /// <summary>
        /// Callback for when an asset is loaded (Editor)
        /// </summary>
        //WITH_EDITOR
        public static OnAssetLoadedHandler OnAssetLoaded = new OnAssetLoadedHandler();
        public class OnAssetLoadedHandler : NativeMulticastDelegate<Native_FCoreUObjectDelegates.Del_OnAssetLoaded, Native_FCoreUObjectDelegates.Del_Reg_OnAssetLoaded, OnAssetLoadedHandler.Signature>
        {
            public delegate void Signature(UObject asset);
            private void NativeCallback(IntPtr asset)
            {
                try
                {
                    var evnt = managed.Delegate;
                    if (evnt != null)
                    {
                        evnt(GCHelper.Find(asset));
                    }
                }
                catch (Exception e)
                {
                    FMessage.LogDelegateException(e);
                }
            }
        }

        /// <summary>
        /// Called when an asset is saved
        /// </summary>
        //WITH_EDITOR
        public static OnObjectSavedHandler OnObjectSaved = new OnObjectSavedHandler();
        public class OnObjectSavedHandler : NativeMulticastDelegate<Native_FCoreUObjectDelegates.Del_OnObjectSaved, Native_FCoreUObjectDelegates.Del_Reg_OnObjectSaved, OnObjectSavedHandler.Signature>
        {
            public delegate void Signature(UObject savedObject);
            private void NativeCallback(IntPtr savedObject)
            {
                try
                {
                    var evnt = managed.Delegate;
                    if (evnt != null)
                    {
                        evnt(GCHelper.Find(savedObject));
                    }
                }
                catch (Exception e)
                {
                    FMessage.LogDelegateException(e);
                }
            }
        }

        /// <summary>
        /// Sent at the very beginning of LoadMap
        /// </summary>
        public static PreLoadMapHandler PreLoadMap = new PreLoadMapHandler();
        public class PreLoadMapHandler : NativeMulticastDelegate<Native_FCoreUObjectDelegates.Del_PreLoadMap, Native_FCoreUObjectDelegates.Del_Reg_PreLoadMap, PreLoadMapHandler.Signature>
        {
            public delegate void Signature(string mapName);
            private void NativeCallback(ref FScriptArray mapName)
            {
                try
                {
                    string mapNameStr = FStringMarshaler.FromArray(mapName, false);
                    var evnt = managed.Delegate;
                    if (evnt != null)
                    {
                        evnt(mapNameStr);
                    }
                }
                catch (Exception e)
                {
                    FMessage.LogDelegateException(e);
                }
            }
        }

        /// <summary>
        /// Sent at the _successful_ end of LoadMap
        /// </summary>
        public static PostLoadMapWithWorldHandler PostLoadMapWithWorld = new PostLoadMapWithWorldHandler();
        public class PostLoadMapWithWorldHandler : NativeMulticastDelegate<Native_FCoreUObjectDelegates.Del_PostLoadMapWithWorld, Native_FCoreUObjectDelegates.Del_Reg_PostLoadMapWithWorld, PostLoadMapWithWorldHandler.Signature>
        {
            public delegate void Signature(UObject loadedWorld);
            private void NativeCallback(IntPtr loadedWorld)
            {
                try
                {
                    var evnt = managed.Delegate;
                    if (evnt != null)
                    {
                        evnt(GCHelper.Find(loadedWorld));
                    }
                }
                catch (Exception e)
                {
                    FMessage.LogDelegateException(e);
                }
            }
        }

        /// <summary>
        /// Sent at the _successful_ end of LoadMap
        /// </summary>
        public static PostDemoPlayHandler PostDemoPlay = new PostDemoPlayHandler();
        public class PostDemoPlayHandler : NativeSimpleMulticastDelegate<Native_FCoreUObjectDelegates.Del_Reg_PostDemoPlay> { }

        /// <summary>
        /// Called before garbage collection
        /// </summary>
        public static PreGarbageCollectHandler PreGarbageCollect = new PreGarbageCollectHandler();
        public class PreGarbageCollectHandler : NativeSimpleMulticastDelegate<Native_FCoreUObjectDelegates.Del_Reg_PreGarbageCollect> { }

        /// <summary>
        /// Called after garbage collection
        /// </summary>
        public static PostGarbageCollectHandler PostGarbageCollect = new PostGarbageCollectHandler();
        public class PostGarbageCollectHandler : NativeSimpleMulticastDelegate<Native_FCoreUObjectDelegates.Del_Reg_PostGarbageCollect> { }
    }
}

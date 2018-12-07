using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_FCoreUObjectDelegates
    {
        public delegate void Del_OnObjectModified(IntPtr objectBeingModified);
        public delegate void Del_OnAssetLoaded(IntPtr asset);
        public delegate void Del_OnObjectSaved(IntPtr savedObject);
        public delegate void Del_PreLoadMap(ref FScriptArray mapName);
        public delegate void Del_PostLoadMapWithWorld(IntPtr loadedWorld);

        public delegate void Del_Reg_OnObjectModified(IntPtr instance, Del_OnObjectModified handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnAssetLoaded(IntPtr instance, Del_OnAssetLoaded handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnObjectSaved(IntPtr instance, Del_OnObjectSaved handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_PreLoadMap(IntPtr instance, Del_PreLoadMap handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_PostLoadMapWithWorld(IntPtr instance, Del_PostLoadMapWithWorld handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_PostDemoPlay(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_PreGarbageCollect(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_PostGarbageCollect(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);

        public static Del_Reg_OnObjectModified Reg_OnObjectModified;
        public static Del_Reg_OnAssetLoaded Reg_OnAssetLoaded;
        public static Del_Reg_OnObjectSaved Reg_OnObjectSaved;
        public static Del_Reg_PreLoadMap Reg_PreLoadMap;
        public static Del_Reg_PostLoadMapWithWorld Reg_PostLoadMapWithWorld;
        public static Del_Reg_PostDemoPlay Reg_PostDemoPlay;
        public static Del_Reg_PreGarbageCollect Reg_PreGarbageCollect;
        public static Del_Reg_PostGarbageCollect Reg_PostGarbageCollect;
    }
}

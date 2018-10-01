using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_SharpHotReloadUtils
    {
        public delegate void Del_UpdateDelegates(IntPtr delegates);
        public delegate void Del_UpdateEnum(IntPtr unrealEnum, IntPtr oldNames, IntPtr oldValues, csbool resolveData);
        public delegate void Del_PreUpdateStructs(IntPtr sharpStructs, ref IntPtr outBlueprintsToRecompile, ref IntPtr outChangedStructsBP);
        public delegate void Del_PostUpdateStructs(IntPtr sharpChangedStructsOld, IntPtr sharpChangedStructsNew, IntPtr blueprintsToRecompile, IntPtr changedStructsBP);
        public delegate IntPtr Del_CreateClassReinstancer(IntPtr newClass, IntPtr oldClass);
        public delegate void Del_SetTempClass(IntPtr reinstancer, IntPtr trashedClass);
        public delegate void Del_ReinstanceClass(IntPtr reinstancer);
        public delegate void Del_FinalizeClasses();
        public delegate void Del_BroadcastOnHotReload(csbool wasTriggeredAutomatically);
        public delegate csbool Del_Get_MinimalHotReload();
        public delegate void Del_Set_MinimalHotReload(csbool value);

        public static Del_UpdateDelegates UpdateDelegates;
        public static Del_UpdateEnum UpdateEnum;
        public static Del_PreUpdateStructs PreUpdateStructs;
        public static Del_PostUpdateStructs PostUpdateStructs;
        public static Del_CreateClassReinstancer CreateClassReinstancer;
        public static Del_SetTempClass SetTempClass;
        public static Del_ReinstanceClass ReinstanceClass;
        public static Del_FinalizeClasses FinalizeClasses;
        public static Del_BroadcastOnHotReload BroadcastOnHotReload;
        public static Del_Get_MinimalHotReload Get_MinimalHotReload;
        public static Del_Set_MinimalHotReload Set_MinimalHotReload;
    }
}

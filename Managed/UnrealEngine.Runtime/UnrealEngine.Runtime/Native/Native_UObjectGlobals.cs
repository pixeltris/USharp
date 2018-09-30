using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UObjectGlobals
    {
        public delegate csbool Del_Get_GIsSavingPackage();
        public delegate csbool Del_IsGarbageCollecting();
        public delegate void Del_CollectGarbage(EObjectFlags keepFlags, csbool performFullPurge);
        public delegate void Del_CollectGarbageDefault();
        public delegate csbool Del_TryCollectGarbage(EObjectFlags keepFlags, csbool performFullPurge);
        public delegate csbool Del_TryCollectGarbageDefault();
        public delegate csbool Del_IsIncrementalPurgePending();
        public delegate void Del_IncrementalPurgeGarbage(csbool useTimeLimit, float timeLimit);
        public delegate void Del_MakeUniqueObjectName(IntPtr outer, IntPtr unrealClass, ref FName baseName, out FName result);
        public delegate void Del_MakeObjectNameFromDisplayLabel(ref FScriptArray displayLabel, ref FName currentObjectName, out FName result);
        public delegate csbool Del_IsReferenced(IntPtr res, EObjectFlags keepFlags, EInternalObjectFlags internalKeepFlags, csbool checkSubObjects, IntPtr foundReferences);
        public delegate csbool Del_IsLoading();
        public delegate IntPtr Del_GetTransientPackage();
        public delegate void Del_CheckIsClassChildOf_Internal(IntPtr parent, IntPtr child);
        public delegate IntPtr Del_StaticConstructObject_Internal(IntPtr unrealClass, IntPtr inOuter, ref FName name, EObjectFlags setFlags, EInternalObjectFlags internalSetFlags, IntPtr template, csbool copyTransientsFromClassDefaults, IntPtr instanceGraph);
        public delegate IntPtr Del_StaticDuplicateObject(IntPtr sourceObject, IntPtr destOuter, ref FName destName, EObjectFlags flagMask, IntPtr destClass, EDuplicateMode duplicateMode, EInternalObjectFlags internalFlagsMask);
        public delegate IntPtr Del_StaticFindObjectFast(IntPtr unrealClass, IntPtr inOuter, ref FName inName, csbool exactClass, csbool anyPackage, EObjectFlags exclusiveFlags, EInternalObjectFlags exclusiveInternalFlags);
        public delegate IntPtr Del_StaticFindObject(IntPtr unrealClass, IntPtr inOuter, ref FScriptArray name, csbool exactClass);
        public delegate IntPtr Del_StaticFindObjectChecked(IntPtr unrealClass, IntPtr inOuter, ref FScriptArray name, csbool exactClass);
        public delegate IntPtr Del_StaticFindObjectSafe(IntPtr unrealClass, IntPtr inOuter, ref FScriptArray name, csbool exactClass);
        public delegate IntPtr Del_StaticLoadObject(IntPtr unrealClass, IntPtr inOuter, ref FScriptArray name, ref FScriptArray filename, ELoadFlags loadFlags, IntPtr sandbox, csbool allowObjectReconciliation);
        public delegate IntPtr Del_StaticLoadClass(IntPtr baseClass, IntPtr inOuter, ref FScriptArray name, ref FScriptArray filename, ELoadFlags loadFlags, IntPtr sandbox);
        public delegate IntPtr Del_LoadPackage(IntPtr inOuter, ref FScriptArray inLongPackageName, ELoadFlags loadFlags);
        public delegate IntPtr Del_FindPackage(IntPtr inOuter, ref FScriptArray packageName);
        public delegate IntPtr Del_CreatePackage(IntPtr inOuter, ref FScriptArray packageName);
        public delegate IntPtr Del_StaticAllocateObject(IntPtr unrealClass, IntPtr inOuter, ref FName name, EObjectFlags setFlags, EInternalObjectFlags internalSetFlags, csbool canReuseSubobjects, out csbool outReusedSubobject);

        public static Del_Get_GIsSavingPackage Get_GIsSavingPackage;
        public static Del_IsGarbageCollecting IsGarbageCollecting;
        public static Del_CollectGarbage CollectGarbage;
        public static Del_CollectGarbageDefault CollectGarbageDefault;
        public static Del_TryCollectGarbage TryCollectGarbage;
        public static Del_TryCollectGarbageDefault TryCollectGarbageDefault;
        public static Del_IsIncrementalPurgePending IsIncrementalPurgePending;
        public static Del_IncrementalPurgeGarbage IncrementalPurgeGarbage;
        public static Del_MakeUniqueObjectName MakeUniqueObjectName;
        public static Del_MakeObjectNameFromDisplayLabel MakeObjectNameFromDisplayLabel;
        public static Del_IsReferenced IsReferenced;
        public static Del_IsLoading IsLoading;
        public static Del_GetTransientPackage GetTransientPackage;
        public static Del_CheckIsClassChildOf_Internal CheckIsClassChildOf_Internal;
        public static Del_StaticConstructObject_Internal StaticConstructObject_Internal;
        public static Del_StaticDuplicateObject StaticDuplicateObject;
        public static Del_StaticFindObjectFast StaticFindObjectFast;
        public static Del_StaticFindObject StaticFindObject;
        public static Del_StaticFindObjectChecked StaticFindObjectChecked;
        public static Del_StaticFindObjectSafe StaticFindObjectSafe;
        public static Del_StaticLoadObject StaticLoadObject;
        public static Del_StaticLoadClass StaticLoadClass;
        public static Del_LoadPackage LoadPackage;
        public static Del_FindPackage FindPackage;
        public static Del_CreatePackage CreatePackage;
        public static Del_StaticAllocateObject StaticAllocateObject;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UObjectBase
    {        
        public delegate csbool Del_IsValidLowLevel(IntPtr instance);
        public delegate csbool Del_IsValidLowLevelFast(IntPtr instance, csbool bRecursive);
        public delegate uint Del_GetUniqueID(IntPtr instance);
        public delegate IntPtr Del_GetClass(IntPtr instance);
        public delegate IntPtr Del_GetOuter(IntPtr instance);
        public delegate void Del_GetFName(IntPtr instance, out FName result);
        public delegate void Del_GetStatID(IntPtr instance, out TStatId result);
        public delegate EObjectFlags Del_GetFlags(IntPtr instance);
        public delegate void Del_AtomicallySetFlags(IntPtr instance, EObjectFlags flagsToAdd);
        public delegate void Del_AtomicallyClearFlags(IntPtr instance, EObjectFlags flagsToClear);
        public delegate void Del_UObjectForceRegistration(IntPtr obj);
        public delegate void Del_ProcessNewlyLoadedUObjects();

        public static Del_IsValidLowLevel IsValidLowLevel;
        public static Del_IsValidLowLevelFast IsValidLowLevelFast;
        public static Del_GetUniqueID GetUniqueID;
        public static Del_GetClass GetClass;
        public static Del_GetOuter GetOuter;
        public static Del_GetFName GetFName;
        public static Del_GetStatID GetStatID;
        public static Del_GetFlags GetFlags;
        public static Del_AtomicallySetFlags AtomicallySetFlags;
        public static Del_AtomicallyClearFlags AtomicallyClearFlags;
        public static Del_UObjectForceRegistration UObjectForceRegistration;
        public static Del_ProcessNewlyLoadedUObjects ProcessNewlyLoadedUObjects;
    }
}

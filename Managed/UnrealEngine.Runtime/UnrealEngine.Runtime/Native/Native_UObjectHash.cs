using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UObjectHash
    {
        public delegate void Del_GetObjectsWithOuter(IntPtr outer, IntPtr results, csbool includeNestedObjects, EObjectFlags exclusionFlags, EInternalObjectFlags exclusionInternalFlags);
        public delegate IntPtr Del_FindObjectWithOuter(IntPtr outer, IntPtr classToLookFor, ref FName nameToLookFor);
        public delegate void Del_GetObjectsOfClass(IntPtr classToLookFor, IntPtr results, csbool includeDerivedClasses, EObjectFlags additionalExcludeFlags, EInternalObjectFlags exclusionInternalFlags);
        public delegate void Del_GetDerivedClasses(IntPtr classToLookFor, IntPtr results, csbool recursive);
        
        public static Del_GetObjectsWithOuter GetObjectsWithOuter;
        public static Del_FindObjectWithOuter FindObjectWithOuter;
        public static Del_GetObjectsOfClass GetObjectsOfClass;
        public static Del_GetDerivedClasses GetDerivedClasses;
    }
}

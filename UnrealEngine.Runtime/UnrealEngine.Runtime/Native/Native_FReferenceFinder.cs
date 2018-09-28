using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FReferenceFinder
    {
        public delegate IntPtr Del_New(IntPtr objectArray, IntPtr outer, csbool requireDirectOuter, csbool shouldIgnoreArchetype, csbool serializeRecursively, csbool shouldIgnoreTransient);
        public delegate void Del_Delete(IntPtr instance);
        public delegate void Del_FindReferences(IntPtr instance, IntPtr obj, IntPtr referencingObject, IntPtr referencingProperty);

        public static Del_New New;
        public static Del_Delete Delete;
        public static Del_FindReferences FindReferences;
    }
}

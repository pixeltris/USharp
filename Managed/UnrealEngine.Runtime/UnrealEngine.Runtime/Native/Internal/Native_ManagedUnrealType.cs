using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned
#pragma warning disable 108 // Hides inherited member (GetType)

namespace UnrealEngine.Runtime.Native
{
    static class Native_ManagedUnrealType
    {
        public delegate csbool Del_GetType(ref FScriptArray path, ref FScriptArray hash, ref IntPtr obj);
        public delegate void Del_AddType(ref FScriptArray path, ref FScriptArray hash, IntPtr obj);
        public delegate void Del_RemoveType(ref FScriptArray path);

        public static Del_GetType GetType;
        public static Del_AddType AddType;
        public static Del_RemoveType RemoveType;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FCoreRedirects
    {
        public delegate FCoreRedirectObjectName Del_GetRedirectedName(ECoreRedirectFlags type, ref FCoreRedirectObjectName oldObjectName);
        public delegate csbool Del_IsKnownMissing(ECoreRedirectFlags type, ref FCoreRedirectObjectName objectName);
        public delegate csbool Del_AddKnownMissing(ECoreRedirectFlags type, ref FCoreRedirectObjectName objectName);
        public delegate csbool Del_RemoveKnownMissing(ECoreRedirectFlags type, ref FCoreRedirectObjectName objectName);
        public delegate csbool Del_FindPreviousNames(ECoreRedirectFlags type, ref FCoreRedirectObjectName newObjectName, IntPtr previousNames);
        public delegate csbool Del_ReadRedirectsFromIni(ref FScriptArray iniName);
        public delegate csbool Del_IsInitialized();
        public delegate ECoreRedirectFlags Del_GetFlagsForTypeName(ref FName packageName, ref FName typeName);
        public delegate ECoreRedirectFlags Del_GetFlagsForTypeClass(IntPtr typeClass);

        public static Del_GetRedirectedName GetRedirectedName;
        public static Del_IsKnownMissing IsKnownMissing;
        public static Del_AddKnownMissing AddKnownMissing;
        public static Del_RemoveKnownMissing RemoveKnownMissing;
        public static Del_FindPreviousNames FindPreviousNames;
        public static Del_ReadRedirectsFromIni ReadRedirectsFromIni;
        public static Del_IsInitialized IsInitialized;
        public static Del_GetFlagsForTypeName GetFlagsForTypeName;
        public static Del_GetFlagsForTypeClass GetFlagsForTypeClass;
    }
}

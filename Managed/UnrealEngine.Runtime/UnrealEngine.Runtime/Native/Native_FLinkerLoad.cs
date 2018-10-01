using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FLinkerLoad
    {
        public delegate void Del_FindPreviousNamesForClass(ref FScriptArray currentClassPath, csbool isInstance, IntPtr result);
        public delegate void Del_FindNewNameForClass(ref FName oldClassName, csbool isInstance, out FName result);
        public delegate void Del_FindNewNameForEnum(ref FName oldEnumName, out FName result);
        public delegate void Del_FindNewNameForStruct(ref FName oldStructName, out FName result);
        public delegate void Del_InvalidateExport(IntPtr oldObject);

        public static Del_FindPreviousNamesForClass FindPreviousNamesForClass;
        public static Del_FindNewNameForClass FindNewNameForClass;
        public static Del_FindNewNameForEnum FindNewNameForEnum;
        public static Del_FindNewNameForStruct FindNewNameForStruct;
        public static Del_InvalidateExport InvalidateExport;
    }
}

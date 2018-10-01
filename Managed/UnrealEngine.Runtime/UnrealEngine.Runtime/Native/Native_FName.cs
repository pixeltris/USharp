using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable 649 // Field is never assigned
#pragma warning disable 108 // Hiding inherited member (ToString)

namespace UnrealEngine.Runtime.Native
{
    static class Native_FName
    {
        public delegate void Del_FromEName(out FName outName, int n);
        public delegate void Del_FromENameNumber(out FName outName, int n, int inNumber);
        public delegate void Del_FromString(out FName outName, ref FScriptArray str, FName.EFindName findType);
        public delegate void Del_FromStringNumber(out FName outName, ref FScriptArray str, int number, FName.EFindName findType);
        public delegate void Del_ToString(ref FName name, ref FScriptArray result);
        public delegate void Del_GetPlainNameString(ref FName name, ref FScriptArray result);
        public delegate csbool Del_IsEqual(ref FName name, ref FName other, FName.ENameCase compareMethod, csbool compareNumber);
        public delegate int Del_Compare(ref FName name, ref FName other);

        public static Del_FromEName FromEName;
        public static Del_FromENameNumber FromENameNumber;
        public static Del_FromString FromString;
        public static Del_FromStringNumber FromStringNumber;
        public static Del_ToString ToString;
        public static Del_GetPlainNameString GetPlainNameString;
        public static Del_IsEqual IsEqual;
        public static Del_Compare Compare;
    }
}

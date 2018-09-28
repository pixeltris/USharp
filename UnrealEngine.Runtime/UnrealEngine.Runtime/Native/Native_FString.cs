using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FString
    {
        public delegate int Del_GetCharSize();
        public delegate void Del_FromCharPtr(IntPtr charArray, ref FScriptArray result);

        public static Del_GetCharSize GetCharSize;
        public static Del_FromCharPtr FromCharPtr;
    }
}

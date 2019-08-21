using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned
#pragma warning disable 108 // ToString hiding base virtual function

namespace UnrealEngine.Runtime.Native
{
    static class Native_FVariantData
    {
        public delegate void Del_FromStringEx(IntPtr instance, ref FScriptArray str);
        public delegate void Del_ToStringEx(IntPtr instance, ref FScriptArray str);

        public static Del_FromStringEx FromStringEx;
        public static Del_ToStringEx ToStringEx;
    }
}

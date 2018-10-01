using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UEnumProperty
    {
        public delegate void Del_SetEnum(IntPtr instance, IntPtr inEnum);
        public delegate IntPtr Del_GetEnum(IntPtr instance);
        public delegate IntPtr Del_GetUnderlyingProperty(IntPtr instance);
        
        public static Del_SetEnum SetEnum;
        public static Del_GetEnum GetEnum;
        public static Del_GetUnderlyingProperty GetUnderlyingProperty;
    }
}

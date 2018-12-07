using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UBoolProperty
    {
        public delegate int Del_GetBoolSize();
        public delegate csbool Del_GetPropertyValue(IntPtr instance, IntPtr address);
        public delegate void Del_SetPropertyValue(IntPtr instance, IntPtr address, csbool value);
        
        public static Del_GetBoolSize GetBoolSize;
        public static Del_GetPropertyValue GetPropertyValue;
        public static Del_SetPropertyValue SetPropertyValue;
    }
}

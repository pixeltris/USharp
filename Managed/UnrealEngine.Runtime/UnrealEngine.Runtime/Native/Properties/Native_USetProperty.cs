using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_USetProperty
    {
        public delegate IntPtr Del_Get_ElementProp(IntPtr instance);
        public delegate void Del_Set_ElementProp(IntPtr instance, IntPtr value);
        public delegate FScriptSetLayout Del_Get_SetLayout(IntPtr instance);
        public delegate void Del_Set_SetLayout(IntPtr instance, FScriptSetLayout value);
        
        public static Del_Get_ElementProp Get_ElementProp;
        public static Del_Set_ElementProp Set_ElementProp;
        public static Del_Get_SetLayout Get_SetLayout;
        public static Del_Set_SetLayout Set_SetLayout;
    }
}

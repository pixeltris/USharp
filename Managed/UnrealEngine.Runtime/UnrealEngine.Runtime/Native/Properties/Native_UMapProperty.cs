using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UMapProperty
    {
        public delegate IntPtr Del_Get_KeyProp(IntPtr instance);
        public delegate void Del_Set_KeyProp(IntPtr instance, IntPtr value);
        public delegate IntPtr Del_Get_ValueProp(IntPtr instance);
        public delegate void Del_Set_ValueProp(IntPtr instance, IntPtr value);
        public delegate FScriptMapLayout Del_Get_MapLayout(IntPtr instance);
        public delegate void Del_Set_MapLayout(IntPtr instance, FScriptMapLayout value);
        
        public static Del_Get_KeyProp Get_KeyProp;
        public static Del_Set_KeyProp Set_KeyProp;
        public static Del_Get_ValueProp Get_ValueProp;
        public static Del_Set_ValueProp Set_ValueProp;
        public static Del_Get_MapLayout Get_MapLayout;
        public static Del_Set_MapLayout Set_MapLayout;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UStructProperty
    {
        public delegate IntPtr Del_Get_Struct(IntPtr instance);
        public delegate void Del_Set_Struct(IntPtr instance, IntPtr value);
        
        public static Del_Get_Struct Get_Struct;
        public static Del_Set_Struct Set_Struct;
    }
}

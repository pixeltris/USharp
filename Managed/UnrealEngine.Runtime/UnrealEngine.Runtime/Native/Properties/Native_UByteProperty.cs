using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UByteProperty
    {
        public delegate IntPtr Del_Get_Enum(IntPtr instance);
        public delegate void Del_Set_Enum(IntPtr instance, IntPtr value);
        
        public static Del_Get_Enum Get_Enum;
        public static Del_Set_Enum Set_Enum;
    }
}

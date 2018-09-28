using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UArrayProperty
    {
        public delegate IntPtr Del_Get_Inner(IntPtr instance);
        public delegate void Del_Set_Inner(IntPtr instance, IntPtr value);
        
        public static Del_Get_Inner Get_Inner;
        public static Del_Set_Inner Set_Inner;
    }
}

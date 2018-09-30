using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UInterfaceProperty
    {
        public delegate IntPtr Del_Get_InterfaceClass(IntPtr instance);
        public delegate void Del_Set_InterfaceClass(IntPtr instance, IntPtr value);
        public delegate void Del_SetInterfaceClass(IntPtr instance, IntPtr newMetaClass);
        
        public static Del_Get_InterfaceClass Get_InterfaceClass;
        public static Del_Set_InterfaceClass Set_InterfaceClass;
        public static Del_SetInterfaceClass SetInterfaceClass;
    }
}

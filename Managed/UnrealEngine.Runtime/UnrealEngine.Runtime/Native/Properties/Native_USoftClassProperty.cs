using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_USoftClassProperty
    {
        public delegate IntPtr Del_Get_MetaClass(IntPtr instance);
        public delegate void Del_Set_MetaClass(IntPtr instance, IntPtr value);
        public delegate void Del_SetMetaClass(IntPtr instance, IntPtr newMetaClass);
        
        public static Del_Get_MetaClass Get_MetaClass;
        public static Del_Set_MetaClass Set_MetaClass;
        public static Del_SetMetaClass SetMetaClass;
    }
}

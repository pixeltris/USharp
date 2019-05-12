using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_UMaterialInstanceDynamic
    {
        public delegate IntPtr Del_Create(IntPtr parentMaterial, IntPtr outer);
        public delegate IntPtr Del_Create_Named(IntPtr parentMaterial, IntPtr outer, ref FName name);

        public static Del_Create Create;
        public static Del_Create_Named Create_Named;
    }
}

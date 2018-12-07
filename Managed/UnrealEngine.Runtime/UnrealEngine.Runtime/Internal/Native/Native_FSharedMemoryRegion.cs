using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FSharedMemoryRegion
    {
        public delegate void Del_GetName(IntPtr instance, ref FScriptArray result);
        public delegate IntPtr Del_GetAddress(IntPtr instance);
        public delegate IntPtr Del_GetSize(IntPtr instance);

        public static Del_GetName GetName;
        public static Del_GetAddress GetAddress;
        public static Del_GetSize GetSize;
    }
}

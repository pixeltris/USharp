using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_USharpStruct
    {
        public delegate IntPtr Del_CreateGuid(IntPtr instance);

        public static Del_CreateGuid CreateGuid;
    }
}

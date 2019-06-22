using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UOnlineBlueprintCallProxyBase
    {
        public delegate void Del_Activate(IntPtr instance);

        public static Del_Activate Activate;
    }
}

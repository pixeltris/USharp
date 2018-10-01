using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UEditorEngine
    {
        public delegate IntPtr Del_GetTimerManager(IntPtr instance);
        public delegate IntPtr Del_GetPIEWorldContext(IntPtr instance);

        public static Del_GetTimerManager GetTimerManager;
        public static Del_GetPIEWorldContext GetPIEWorldContext;
    }
}

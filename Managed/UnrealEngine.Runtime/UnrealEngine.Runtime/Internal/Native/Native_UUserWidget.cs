using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UUserWidget
    {
        public delegate IntPtr Del_GetWidgetFromName(IntPtr instance, ref FName name);

        public static Del_GetWidgetFromName GetWidgetFromName;
    }
}

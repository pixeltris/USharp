using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_UBlueprintAsyncActionBase
    {
        public delegate void Del_Activate(IntPtr instance);
        public delegate void Del_RegisterWithGameInstanceWorldContext(IntPtr instance, IntPtr worldContextObject);
        public delegate void Del_RegisterWithGameInstance(IntPtr instance, IntPtr worldContextObject);
        public delegate void Del_SetReadyToDestroy(IntPtr instance);

        public static Del_Activate Activate;
        public static Del_RegisterWithGameInstanceWorldContext RegisterWithGameInstanceWorldContext;
        public static Del_RegisterWithGameInstance RegisterWithGameInstance;
        public static Del_SetReadyToDestroy SetReadyToDestroy;
    }
}

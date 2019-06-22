using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Engine;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_UUSharpAsyncActionBase
    {
        public delegate IntPtr Del_GetRegisteredWithGameInstance(IntPtr instance);
        public delegate void Del_Base_RegisterWithGameInstanceWorldContext(IntPtr instance, IntPtr worldContextObject);
        public delegate void Del_Base_RegisterWithGameInstance(IntPtr instance, IntPtr gameInstance);
        public delegate void Del_Base_SetReadyToDestroy(IntPtr instance);

        internal static ManagedLatentCallbackRegisterDel Set_Callback;
        public static Del_GetRegisteredWithGameInstance GetRegisteredWithGameInstance;
        public static Del_Base_RegisterWithGameInstanceWorldContext Base_RegisterWithGameInstanceWorldContext;
        public static Del_Base_RegisterWithGameInstance Base_RegisterWithGameInstance;
        public static Del_Base_SetReadyToDestroy Base_SetReadyToDestroy;
    }
}

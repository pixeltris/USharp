using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FWorldContext
    {
        public delegate byte Del_Get_WorldType(IntPtr instance);
        public delegate void Del_Get_ContextHandle(IntPtr instance, out FName result);
        public delegate void Del_Get_TravelURL(IntPtr instance, ref FScriptArray result);
        public delegate byte Del_Get_TravelType(IntPtr instance);
        public delegate IntPtr Del_Get_GameViewport(IntPtr instance);
        public delegate IntPtr Del_Get_OwningGameInstance(IntPtr instance);
        public delegate int Del_Get_PIEInstance(IntPtr instance);
        public delegate void Del_Get_PIEPrefix(IntPtr instance, ref FScriptArray result);
        public delegate csbool Del_Get_RunAsDedicated(IntPtr instance);
        public delegate csbool Del_Get_bWaitingOnOnlineSubsystem(IntPtr instance);
        public delegate uint Del_Get_AudioDeviceHandle(IntPtr instance);
        public delegate void Del_SetCurrentWorld(IntPtr instance, IntPtr world);
        public delegate IntPtr Del_World(IntPtr instance);

        public static Del_Get_WorldType Get_WorldType;
        public static Del_Get_ContextHandle Get_ContextHandle;
        public static Del_Get_TravelURL Get_TravelURL;
        public static Del_Get_TravelType Get_TravelType;
        public static Del_Get_GameViewport Get_GameViewport;
        public static Del_Get_OwningGameInstance Get_OwningGameInstance;
        public static Del_Get_PIEInstance Get_PIEInstance;
        public static Del_Get_PIEPrefix Get_PIEPrefix;
        public static Del_Get_RunAsDedicated Get_RunAsDedicated;
        public static Del_Get_bWaitingOnOnlineSubsystem Get_bWaitingOnOnlineSubsystem;
        public static Del_Get_AudioDeviceHandle Get_AudioDeviceHandle;
        public static Del_SetCurrentWorld SetCurrentWorld;
        public static Del_World World;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UWorld
    {
        public delegate IntPtr Del_Get_GWorld();
        public delegate IntPtr Del_GetLevels(IntPtr instance);        
        public delegate IntPtr Del_GetGameInstance(IntPtr instance);
        public delegate IntPtr Del_GetTimerManager(IntPtr instance);

        public static Del_Get_GWorld Get_GWorld;
        public static Del_GetLevels GetLevels;
        public static Del_GetGameInstance GetGameInstance;
        public static Del_GetTimerManager GetTimerManager;
    }
}

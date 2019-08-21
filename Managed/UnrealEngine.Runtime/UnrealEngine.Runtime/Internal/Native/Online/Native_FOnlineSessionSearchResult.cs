using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FOnlineSessionSearchResult
    {
        public delegate IntPtr Del_Get_Session(IntPtr instance);
        public delegate int Del_Get_PingInMs(IntPtr instance);
        public delegate void Del_Set_PingInMs(IntPtr instance, int value);

        public static Del_Get_Session Get_Session;
        public static Del_Get_PingInMs Get_PingInMs;
        public static Del_Set_PingInMs Set_PingInMs;
    }
}

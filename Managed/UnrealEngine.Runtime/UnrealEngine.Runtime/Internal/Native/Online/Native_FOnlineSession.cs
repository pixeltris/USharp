using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FOnlineSession
    {
        public delegate void Del_Get_OwningUserId(IntPtr instance, ref FSharedPtr result);
        public delegate void Del_Set_OwningUserId(IntPtr instance, ref FSharedPtr value);
        public delegate void Del_Get_OwningUserName(IntPtr instance, ref FScriptArray result);
        public delegate void Del_Set_OwningUserName(IntPtr instance, ref FScriptArray value);
        public delegate IntPtr Del_Get_SessionSettings(IntPtr instance);
        public delegate void Del_Get_SessionInfo(IntPtr instance, ref FSharedPtr result);
        public delegate void Del_Set_SessionInfo(IntPtr instance, ref FSharedPtr value);
        public delegate int Del_Get_NumOpenPrivateConnections(IntPtr instance);
        public delegate void Del_Set_NumOpenPrivateConnections(IntPtr instance, int value);
        public delegate int Del_Get_NumOpenPublicConnections(IntPtr instance);
        public delegate void Del_Set_NumOpenPublicConnections(IntPtr instance, int value);

        public static Del_Get_OwningUserId Get_OwningUserId;
        public static Del_Set_OwningUserId Set_OwningUserId;
        public static Del_Get_OwningUserName Get_OwningUserName;
        public static Del_Set_OwningUserName Set_OwningUserName;
        public static Del_Get_SessionSettings Get_SessionSettings;
        public static Del_Get_SessionInfo Get_SessionInfo;
        public static Del_Set_SessionInfo Set_SessionInfo;
        public static Del_Get_NumOpenPrivateConnections Get_NumOpenPrivateConnections;
        public static Del_Set_NumOpenPrivateConnections Set_NumOpenPrivateConnections;
        public static Del_Get_NumOpenPublicConnections Get_NumOpenPublicConnections;
        public static Del_Set_NumOpenPublicConnections Set_NumOpenPublicConnections;
    }
}

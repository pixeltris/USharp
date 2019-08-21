using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FOnlineSessionSettings
    {
        public delegate int Del_Get_NumPublicConnections(IntPtr instance);
        public delegate void Del_Set_NumPublicConnections(IntPtr instance, int value);
        public delegate int Del_Get_NumPrivateConnections(IntPtr instance);
        public delegate void Del_Set_NumPrivateConnections(IntPtr instance, int value);
        public delegate csbool Del_Get_bShouldAdvertise(IntPtr instance);
        public delegate void Del_Set_bShouldAdvertise(IntPtr instance, csbool value);
        public delegate csbool Del_Get_bAllowJoinInProgress(IntPtr instance);
        public delegate void Del_Set_bAllowJoinInProgress(IntPtr instance, csbool value);
        public delegate csbool Del_Get_bIsLANMatch(IntPtr instance);
        public delegate void Del_Set_bIsLANMatch(IntPtr instance, csbool value);
        public delegate csbool Del_Get_bIsDedicated(IntPtr instance);
        public delegate void Del_Set_bIsDedicated(IntPtr instance, csbool value);
        public delegate csbool Del_Get_bUsesStats(IntPtr instance);
        public delegate void Del_Set_bUsesStats(IntPtr instance, csbool value);
        public delegate csbool Del_Get_bAllowInvites(IntPtr instance);
        public delegate void Del_Set_bAllowInvites(IntPtr instance, csbool value);
        public delegate csbool Del_Get_bUsesPresence(IntPtr instance);
        public delegate void Del_Set_bUsesPresence(IntPtr instance, csbool value);
        public delegate csbool Del_Get_bAllowJoinViaPresence(IntPtr instance);
        public delegate void Del_Set_bAllowJoinViaPresence(IntPtr instance, csbool value);
        public delegate csbool Del_Get_bAllowJoinViaPresenceFriendsOnly(IntPtr instance);
        public delegate void Del_Set_bAllowJoinViaPresenceFriendsOnly(IntPtr instance, csbool value);
        public delegate csbool Del_Get_bAntiCheatProtected(IntPtr instance);
        public delegate void Del_Set_bAntiCheatProtected(IntPtr instance, csbool value);
        public delegate int Del_Get_BuildUniqueId(IntPtr instance);
        public delegate void Del_Set_BuildUniqueId(IntPtr instance, int value);
        public delegate void Del_Get_Settings(IntPtr instance, IntPtr keys, IntPtr values);
        public delegate void Del_Set_Settings(IntPtr instance, IntPtr keys, IntPtr values);

        public static Del_Get_NumPublicConnections Get_NumPublicConnections;
        public static Del_Set_NumPublicConnections Set_NumPublicConnections;
        public static Del_Get_NumPrivateConnections Get_NumPrivateConnections;
        public static Del_Set_NumPrivateConnections Set_NumPrivateConnections;
        public static Del_Get_bShouldAdvertise Get_bShouldAdvertise;
        public static Del_Set_bShouldAdvertise Set_bShouldAdvertise;
        public static Del_Get_bAllowJoinInProgress Get_bAllowJoinInProgress;
        public static Del_Set_bAllowJoinInProgress Set_bAllowJoinInProgress;
        public static Del_Get_bIsLANMatch Get_bIsLANMatch;
        public static Del_Set_bIsLANMatch Set_bIsLANMatch;
        public static Del_Get_bIsDedicated Get_bIsDedicated;
        public static Del_Set_bIsDedicated Set_bIsDedicated;
        public static Del_Get_bUsesStats Get_bUsesStats;
        public static Del_Set_bUsesStats Set_bUsesStats;
        public static Del_Get_bAllowInvites Get_bAllowInvites;
        public static Del_Set_bAllowInvites Set_bAllowInvites;
        public static Del_Get_bUsesPresence Get_bUsesPresence;
        public static Del_Set_bUsesPresence Set_bUsesPresence;
        public static Del_Get_bAllowJoinViaPresence Get_bAllowJoinViaPresence;
        public static Del_Set_bAllowJoinViaPresence Set_bAllowJoinViaPresence;
        public static Del_Get_bAllowJoinViaPresenceFriendsOnly Get_bAllowJoinViaPresenceFriendsOnly;
        public static Del_Set_bAllowJoinViaPresenceFriendsOnly Set_bAllowJoinViaPresenceFriendsOnly;
        public static Del_Get_bAntiCheatProtected Get_bAntiCheatProtected;
        public static Del_Set_bAntiCheatProtected Set_bAntiCheatProtected;
        public static Del_Get_BuildUniqueId Get_BuildUniqueId;
        public static Del_Set_BuildUniqueId Set_BuildUniqueId;
        public static Del_Get_Settings Get_Settings;
        public static Del_Set_Settings Set_Settings;
    }
}

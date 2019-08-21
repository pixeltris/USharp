using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Plugins.OnlineSubsystem
{
    public static class FSessionUtils
    {
        public static readonly FName NAME_GameSession = (FName)"GameSession";

        public static EOnlineSessionState GetSessionState()
        {
            return GetSessionState(NAME_GameSession);
        }

        public static EOnlineSessionState GetSessionState(FName sessionName)
        {
            return (EOnlineSessionState)Native_FSessionUtils.GetSessionState(ref sessionName);
        }

        public static bool CancelFindSessions()
        {
            return Native_FSessionUtils.CancelFindSessions();
        }

        public static int GetNumSessions()
        {
            return Native_FSessionUtils.GetNumSessions();
        }
    }
}

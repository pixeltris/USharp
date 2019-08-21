using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FSessionUtils
    {
        public delegate int Del_GetSessionState(ref FName sessionName);
        public delegate csbool Del_CancelFindSessions();
        public delegate int Del_GetNumSessions();

        public static Del_GetSessionState GetSessionState;
        public static Del_CancelFindSessions CancelFindSessions;
        public static Del_GetNumSessions GetNumSessions;
    }
}

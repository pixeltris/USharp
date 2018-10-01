using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FCoreNet
    {
        public delegate void Del_RPC_ResetLastFailedReason();
        public delegate void Del_RPC_ValidateFailed(ref FScriptArray reason);
        public delegate void Del_RPC_GetLastFailedReason(ref FScriptArray result);

        public static Del_RPC_ResetLastFailedReason RPC_ResetLastFailedReason;
        public static Del_RPC_ValidateFailed RPC_ValidateFailed;
        public static Del_RPC_GetLastFailedReason RPC_GetLastFailedReason;
    }
}

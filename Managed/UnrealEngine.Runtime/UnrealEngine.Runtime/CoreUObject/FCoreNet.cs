using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // This isn't a native class (rename this? merge with FCoreGlobals?). This wraps functions in CoreNet.h

    public static class FCoreNet
    {
        public static void RPC_ResetLastFailedReason()
        {
            Native_FCoreNet.RPC_ResetLastFailedReason();
        }

        public static void RPC_ValidateFailed(string reason)
        {
            using (FStringUnsafe reasonUnsafe = new FStringUnsafe(reason))
            {
                Native_FCoreNet.RPC_ValidateFailed(ref reasonUnsafe.Array);
            }
        }

        public static string RPC_GetLastFailedReason()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FCoreNet.RPC_GetLastFailedReason(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }
    }
}

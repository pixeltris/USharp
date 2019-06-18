using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FLatentResponse
    {
        public delegate IntPtr Del_DoneIf(IntPtr instance, csbool condition);
        public delegate IntPtr Del_TriggerLink(IntPtr instance, ref FName executionFunction, int linkID, ref FWeakObjectPtr callbackTarget);
        public delegate IntPtr Del_FinishAndTriggerIf(IntPtr instance, csbool condition, ref FName executionFunction, int linkID, ref FWeakObjectPtr callbackTarget);
        public delegate float Del_ElapsedTime(IntPtr instance);

        public static Del_DoneIf DoneIf;
        public static Del_TriggerLink TriggerLink;
        public static Del_FinishAndTriggerIf FinishAndTriggerIf;
        public static Del_ElapsedTime ElapsedTime;
    }
}

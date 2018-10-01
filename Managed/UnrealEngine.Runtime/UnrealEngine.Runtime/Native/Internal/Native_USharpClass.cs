using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_USharpClass
    {
        public delegate void Del_ClearFuncMapEx(IntPtr instance);
        public delegate void Del_SetFallbackFunctionInvoker(IntPtr instance, IntPtr function);
        public delegate IntPtr Del_SetFunctionInvoker(IntPtr instance, ref FScriptArray functionName, IntPtr invoker);

        public static Del_ClearFuncMapEx ClearFuncMapEx;
        public static Del_SetFallbackFunctionInvoker SetFallbackFunctionInvoker;
        public static Del_SetFunctionInvoker SetFunctionInvoker;
    }
}

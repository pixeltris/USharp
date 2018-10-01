using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_IConsoleVariable
    {
        public delegate void FConsoleVariableDelegate(IntPtr consoleVariable);

        public delegate int Del_GetInt(IntPtr instance);
        public delegate float Del_GetFloat(IntPtr instance);
        public delegate void Del_GetString(IntPtr instance, ref FScriptArray result);
        public delegate void Del_SetOnChangedCallback(IntPtr instance, FConsoleVariableDelegate callback);
        public delegate void Del_ClearOnChangedCallback(IntPtr instance);
        public delegate void Del_SetInt(IntPtr instance, int value, EConsoleVariableFlags setBy);
        public delegate void Del_SetFloat(IntPtr instance, float value, EConsoleVariableFlags setBy);
        public delegate void Del_SetString(IntPtr instance, ref FScriptArray value, EConsoleVariableFlags setBy);

        public static Del_GetInt GetInt;
        public static Del_GetFloat GetFloat;
        public static Del_GetString GetString;
        public static Del_SetOnChangedCallback SetOnChangedCallback;
        public static Del_ClearOnChangedCallback ClearOnChangedCallback;
        public static Del_SetInt SetInt;
        public static Del_SetFloat SetFloat;
        public static Del_SetString SetString;
    }
}

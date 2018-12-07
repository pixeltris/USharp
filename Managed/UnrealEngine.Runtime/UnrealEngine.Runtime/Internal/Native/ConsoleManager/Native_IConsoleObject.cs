using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_IConsoleObject
    {
        public delegate void Del_GetHelp(IntPtr instance, ref FScriptArray result);
        public delegate void Del_SetHelp(IntPtr instance, ref FScriptArray value);
        public delegate EConsoleVariableFlags Del_GetFlags(IntPtr instance);
        public delegate void Del_SetFlags(IntPtr instance, EConsoleVariableFlags value);
        public delegate void Del_ClearFlags(IntPtr instance, EConsoleVariableFlags value);
        public delegate csbool Del_TestFlags(IntPtr instance, EConsoleVariableFlags value);
        public delegate IntPtr Del_AsVariable(IntPtr instance);
        public delegate IntPtr Del_AsCommand(IntPtr instance);

        public static Del_GetHelp GetHelp;
        public static Del_SetHelp SetHelp;
        public static Del_GetFlags GetFlags;
        public static Del_SetFlags SetFlags;
        public static Del_ClearFlags ClearFlags;
        public static Del_TestFlags TestFlags;
        public static Del_AsVariable AsVariable;
        public static Del_AsCommand AsCommand;
    }
}

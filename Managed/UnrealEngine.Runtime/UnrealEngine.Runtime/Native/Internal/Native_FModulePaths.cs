using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FModulePaths
    {
        public delegate void Del_FindModulePaths(ref FScriptArray namePattern, csbool canUseCache, IntPtr keys, IntPtr values);

        public static Del_FindModulePaths FindModulePaths;
    }
}

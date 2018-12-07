using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FPlatformMisc
    {
        public delegate void Del_RequestExit(csbool force);

        public static Del_RequestExit RequestExit;
    }
}

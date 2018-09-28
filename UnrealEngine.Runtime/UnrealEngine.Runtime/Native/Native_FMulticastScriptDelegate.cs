using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FMulticastScriptDelegate
    {
        public delegate void Del_ProcessMulticastDelegate(ref FMulticastScriptDelegate instance, IntPtr parameters);

        public static Del_ProcessMulticastDelegate ProcessMulticastDelegate;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_IConsoleCommand
    {
        public delegate void Del_Execute(IntPtr instance, IntPtr args, IntPtr world, IntPtr outputDevice);

        public static Del_Execute Execute;
    }
}

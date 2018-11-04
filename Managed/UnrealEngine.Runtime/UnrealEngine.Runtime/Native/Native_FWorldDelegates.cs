using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_FWorldDelegates
    {
        public delegate void Del_WorldCleanupEvent(IntPtr world, csbool sessionEnded, csbool cleanupResources);

        public delegate void Del_Reg_OnWorldCleanup(IntPtr instance, Del_WorldCleanupEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnPostWorldCleanup(IntPtr instance, Del_WorldCleanupEvent handler, ref FDelegateHandle handle, csbool enable);

        public static Del_Reg_OnWorldCleanup Reg_OnWorldCleanup;
        public static Del_Reg_OnPostWorldCleanup Reg_OnPostWorldCleanup;
    }
}

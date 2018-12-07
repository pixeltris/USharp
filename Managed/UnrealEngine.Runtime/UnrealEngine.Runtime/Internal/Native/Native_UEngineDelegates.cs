using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_UEngineDelegates
    {
        public delegate void Del_OnWorldAdded(IntPtr world);
        public delegate void Del_OnWorldDestroyed(IntPtr world);

        public delegate void Del_Reg_OnWorldAdded(IntPtr instance, Del_OnWorldAdded handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnWorldDestroyed(IntPtr instance, Del_OnWorldDestroyed handler, ref FDelegateHandle handle, csbool enable);

        public static Del_Reg_OnWorldAdded Reg_OnWorldAdded;
        public static Del_Reg_OnWorldDestroyed Reg_OnWorldDestroyed;
    }
}

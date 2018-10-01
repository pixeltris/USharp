using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_FTicker
    {
        public delegate csbool Del_RegisterTicker(float deltaTime);

        public delegate void Del_Reg_CoreTicker(Del_RegisterTicker handler, ref FDelegateHandle handle, csbool enable, float delay);
        public delegate void Del_Tick(float deltaTime);

        public static Del_Reg_CoreTicker Reg_CoreTicker;
        public static Del_Tick Tick;
    }
}

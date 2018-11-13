using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_FInputTouchBinding
    {
        public delegate byte Del_Get_KeyEvent(IntPtr instance);
        public delegate void Del_Set_KeyEvent(IntPtr instance, byte value);
        public delegate IntPtr Del_Get_TouchDelegate(IntPtr instance);

        public static Del_Get_KeyEvent Get_KeyEvent;
        public static Del_Set_KeyEvent Set_KeyEvent;
        public static Del_Get_TouchDelegate Get_TouchDelegate;
    }
}

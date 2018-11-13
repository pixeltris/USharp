using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.InputCore;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_FInputKeyBinding
    {
        public delegate byte Del_Get_KeyEvent(IntPtr instance);
        public delegate void Del_Set_KeyEvent(IntPtr instance, byte value);
        public delegate void Del_Get_ChordEx(IntPtr instance, out FKey key, out csbool shift, out csbool ctrl, out csbool alt, out csbool cmd);
        public delegate void Del_Set_ChordEx(IntPtr instance, ref FKey key, csbool shift, csbool ctrl, csbool alt, csbool cmd);
        public delegate IntPtr Del_Get_KeyDelegate(IntPtr instance);

        public static Del_Get_KeyEvent Get_KeyEvent;
        public static Del_Set_KeyEvent Set_KeyEvent;
        public static Del_Get_ChordEx Get_ChordEx;
        public static Del_Set_ChordEx Set_ChordEx;
        public static Del_Get_KeyDelegate Get_KeyDelegate;
    }
}

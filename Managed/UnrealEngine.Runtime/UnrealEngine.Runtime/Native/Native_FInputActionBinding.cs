using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_FInputActionBinding
    {
        public delegate byte Del_Get_KeyEvent(IntPtr instance);
        public delegate void Del_Set_KeyEvent(IntPtr instance, byte value);
        public delegate IntPtr Del_Get_ActionDelegate(IntPtr instance);
        public delegate void Del_GetActionName(IntPtr instance, out FName result);
        public delegate csbool Del_IsPaired(IntPtr instance);

        public static Del_Get_KeyEvent Get_KeyEvent;
        public static Del_Set_KeyEvent Set_KeyEvent;
        public static Del_Get_ActionDelegate Get_ActionDelegate;
        public static Del_GetActionName GetActionName;
        public static Del_IsPaired IsPaired;
    }
}

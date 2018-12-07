using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.InputCore;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_FInputGestureBinding
    {
        public delegate float Del_Get_GestureValue(IntPtr instance);
        public delegate void Del_Set_GestureValue(IntPtr instance, float value);
        public delegate void Del_Get_GestureKey(IntPtr instance, out FKey result);
        public delegate void Del_Set_GestureKey(IntPtr instance, ref FKey value);
        public delegate IntPtr Del_Get_GestureDelegate(IntPtr instance);

        public static Del_Get_GestureValue Get_GestureValue;
        public static Del_Set_GestureValue Set_GestureValue;
        public static Del_Get_GestureKey Get_GestureKey;
        public static Del_Set_GestureKey Set_GestureKey;
        public static Del_Get_GestureDelegate Get_GestureDelegate;
    }
}

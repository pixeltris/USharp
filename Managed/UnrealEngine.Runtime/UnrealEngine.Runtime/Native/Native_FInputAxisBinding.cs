using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_FInputAxisBinding
    {
        public delegate void Del_Get_AxisName(IntPtr instance, out FName result);
        public delegate void Del_Set_AxisName(IntPtr instance, ref FName value);
        public delegate IntPtr Del_Get_AxisDelegate(IntPtr instance);
        public delegate float Del_Get_AxisValue(IntPtr instance);
        public delegate void Del_Set_AxisValue(IntPtr instance, float value);

        public static Del_Get_AxisName Get_AxisName;
        public static Del_Set_AxisName Set_AxisName;
        public static Del_Get_AxisDelegate Get_AxisDelegate;
        public static Del_Get_AxisValue Get_AxisValue;
        public static Del_Set_AxisValue Set_AxisValue;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.InputCore;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_FInputVectorAxisBinding
    {
        public delegate void Del_Get_AxisValue(IntPtr instance, out FVector result);
        public delegate void Del_Set_AxisValue(IntPtr instance, ref FVector value);
        public delegate void Del_Get_AxisKey(IntPtr instance, out FKey result);
        public delegate void Del_Set_AxisKey(IntPtr instance, ref FKey value);
        public delegate IntPtr Del_Get_AxisDelegate(IntPtr instance);

        public static Del_Get_AxisValue Get_AxisValue;
        public static Del_Set_AxisValue Set_AxisValue;
        public static Del_Get_AxisKey Get_AxisKey;
        public static Del_Set_AxisKey Set_AxisKey;
        public static Del_Get_AxisDelegate Get_AxisDelegate;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_APlayerController
    {
        public delegate FRotator Del_Get_RotationInput(IntPtr instance);
        public delegate void Del_Set_RotationInput(IntPtr instance, ref FRotator RotationInput);

        public static Del_Get_RotationInput Get_RotationInput;
        public static Del_Set_RotationInput Set_RotationInput;
    }
}

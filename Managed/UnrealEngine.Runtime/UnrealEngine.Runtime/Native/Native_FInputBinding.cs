using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_FInputBinding
    {
        public delegate csbool Del_Get_bConsumeInput(IntPtr instance);
        public delegate void Del_Set_bConsumeInput(IntPtr instance, csbool value);
        public delegate csbool Del_Get_bExecuteWhenPaused(IntPtr instance);
        public delegate void Del_Set_bExecuteWhenPaused(IntPtr instance, csbool value);

        public static Del_Get_bConsumeInput Get_bConsumeInput;
        public static Del_Set_bConsumeInput Set_bConsumeInput;
        public static Del_Get_bExecuteWhenPaused Get_bExecuteWhenPaused;
        public static Del_Set_bExecuteWhenPaused Set_bExecuteWhenPaused;
    }
}

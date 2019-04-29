using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_USceneComponent
    {
        public delegate void Del_SetupAttachment(IntPtr instance, IntPtr parent, ref FName socketName);

        public static Del_SetupAttachment SetupAttachment;
    }
}

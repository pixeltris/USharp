using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_AActor
    {
        public delegate float Del_GetActorTimeDilationOrDefault(IntPtr worldContextObject);
        public delegate IntPtr Del_GetWorld(IntPtr instance);

        public static Del_GetActorTimeDilationOrDefault GetActorTimeDilationOrDefault;
        public static Del_GetWorld GetWorld;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_AActor
    {
        public delegate float Del_GetActorTimeDilationOrDefault(IntPtr worldContextObject);

        public static Del_GetActorTimeDilationOrDefault GetActorTimeDilationOrDefault;
    }
}

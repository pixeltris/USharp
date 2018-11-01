using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Engine;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public class Native_UGameplayStatics
    {
        public delegate void Del_GetAllActorsOfClass(IntPtr WorldContextObject, IntPtr ActorClass, out TArrayUnsafe<IntPtr> OutActors);

        public static Del_GetAllActorsOfClass GetAllActorsOfClass;
    }
}

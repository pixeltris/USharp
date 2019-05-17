using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_UActorComponent
    {
        public delegate void Del_RegisterComponent(IntPtr instance);
        public delegate void Del_ReregisterComponent(IntPtr instance);
        public delegate void Del_UnregisterComponent(IntPtr instance);

        public static Del_RegisterComponent RegisterComponent;
        public static Del_ReregisterComponent ReregisterComponent;
        public static Del_UnregisterComponent UnregisterComponent;

    }
}

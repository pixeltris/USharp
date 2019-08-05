using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_FSubsystemCollection
    {
        public delegate void Del_Initialize(IntPtr instance);
        public delegate void Del_Deinitialize(IntPtr instance);
        public delegate csbool Del_InitializeDependency(IntPtr instance, IntPtr subsystemClass);
        public delegate IntPtr Del_GetSubsystem(IntPtr instance, IntPtr subsystemClass);

        public static Del_Initialize Initialize;
        public static Del_Deinitialize Deinitialize;
        public static Del_InitializeDependency InitializeDependency;
        public static Del_GetSubsystem GetSubsystem;
    }
}

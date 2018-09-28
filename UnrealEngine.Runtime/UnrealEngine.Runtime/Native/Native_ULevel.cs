using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_ULevel
    {
        public delegate void Del_GetLevelBlueprints(IntPtr instance, IntPtr outLevelBlueprints);

        public static Del_GetLevelBlueprints GetLevelBlueprints;
    }
}

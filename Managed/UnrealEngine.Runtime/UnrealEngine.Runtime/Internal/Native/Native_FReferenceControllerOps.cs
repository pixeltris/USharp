using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FReferenceControllerOps
    {
        public delegate int Del_GetSharedReferenceCount(IntPtr referenceController, ESPMode mode);
        public delegate void Del_AddSharedReference(IntPtr referenceController, ESPMode mode);
        public delegate void Del_ConditionallyAddSharedReference(IntPtr referenceController, ESPMode mode);
        public delegate void Del_ReleaseSharedReference(IntPtr referenceController, ESPMode mode);
        public delegate void Del_AddWeakReference(IntPtr referenceController, ESPMode mode);
        public delegate void Del_ReleaseWeakReference(IntPtr referenceController, ESPMode mode);

        public static Del_GetSharedReferenceCount GetSharedReferenceCount;
        public static Del_AddSharedReference AddSharedReference;
        public static Del_ConditionallyAddSharedReference ConditionallyAddSharedReference;
        public static Del_ReleaseSharedReference ReleaseSharedReference;
        public static Del_AddWeakReference AddWeakReference;
        public static Del_ReleaseWeakReference ReleaseWeakReference;
    }
}

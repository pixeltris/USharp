using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // This one doesn't actually exist in native code (only TSharedPtr)
    [StructLayout(LayoutKind.Sequential)]
    public struct FSharedPtr
    {
        public IntPtr ObjectPtr;
        public IntPtr ReferenceController;
    }

    // This one doesn't actually exist in native code (only TSharedRef)
    [StructLayout(LayoutKind.Sequential)]
    public struct FSharedRef
    {
        public IntPtr ObjectPtr;
        public IntPtr ReferenceController;
    }
}

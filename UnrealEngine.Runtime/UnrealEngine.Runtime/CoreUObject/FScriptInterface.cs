using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// This utility class stores the UProperty data for a native interface property.
    /// ObjectPointer and InterfacePointer point to different locations in the same UObject.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FScriptInterface
    {
        /// <summary>
        /// A pointer to a UObject that implements a native interface.
        /// </summary>
        public IntPtr ObjectPointer;

        /// <summary>
        /// Pointer to the location of the interface object within the UObject referenced by ObjectPointer.
        /// </summary>
        public IntPtr InterfacePointer;

        public FScriptInterface(IntPtr objectPointer, IntPtr interfacePointer)
        {
            ObjectPointer = objectPointer;
            InterfacePointer = interfacePointer;
        }
    }
}

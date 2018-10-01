using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // NOTE: Wrapper due to bool (see NativeFunctions "BoolInteropNotes")

    /// <summary>
    /// information about an interface a class implements
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FImplementedInterface
    {
        /// <summary>
        /// the interface class
        /// </summary>
        public IntPtr InterfaceClassAddress;

        /// <summary>
        /// the pointer offset of the interface's vtable
        /// </summary>
        public int PointerOffset;
        
        /// <summary>
        /// whether or not this interface has been implemented via K2
        /// </summary>
        public csbool ImplementedByK2;

        public UClass InterfaceClass
        {
            get { return GCHelper.Find<UClass>(InterfaceClassAddress); }
            set { InterfaceClassAddress = value == null ? IntPtr.Zero : value.Address; }
        }

        public FImplementedInterface(IntPtr interfaceClass, int pointerOffset, bool implementedByK2)
        {
            InterfaceClassAddress = interfaceClass;
            PointerOffset = pointerOffset;
            ImplementedByK2 = implementedByK2;
        }
    }
}

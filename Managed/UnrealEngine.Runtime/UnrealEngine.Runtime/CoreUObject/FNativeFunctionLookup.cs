using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\CoreUObject\Public\UObject\Class.h

    /// <summary>
    /// A struct that maps a string name to a native function
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FNativeFunctionLookup
    {
        public FName Name;
        public IntPtr Pointer;
    }
}

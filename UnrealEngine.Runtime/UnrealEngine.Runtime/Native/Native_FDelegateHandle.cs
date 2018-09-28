using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FDelegateHandle
    {
        public delegate void Del_GenerateNewHandle(ref FDelegateHandle result);

        public static Del_GenerateNewHandle GenerateNewHandle;
    }
}

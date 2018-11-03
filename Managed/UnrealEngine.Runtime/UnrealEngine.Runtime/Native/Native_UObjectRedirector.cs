using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UObjectRedirector
    {
        public delegate IntPtr Del_Get_DestinationObject(IntPtr instance);

        public static Del_Get_DestinationObject Get_DestinationObject;
    }
}

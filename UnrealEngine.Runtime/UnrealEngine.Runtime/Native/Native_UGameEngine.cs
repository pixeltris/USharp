using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UGameEngine
    {
        public delegate IntPtr Del_Get_GameInstance(IntPtr instance);

        public static Del_Get_GameInstance Get_GameInstance;
    }
}

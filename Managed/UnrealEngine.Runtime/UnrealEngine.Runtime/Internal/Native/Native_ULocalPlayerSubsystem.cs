using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_ULocalPlayerSubsystem
    {
        public delegate IntPtr Del_GetLocalPlayer(IntPtr instance);

        public static Del_GetLocalPlayer GetLocalPlayer;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_USharpSettings
    {
        public delegate csbool Del_Get_bDisableExceptionNotifier();

        public static Del_Get_bDisableExceptionNotifier Get_bDisableExceptionNotifier;
    }
}

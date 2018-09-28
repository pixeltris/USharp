using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FMessageDialog
    {
        public delegate EAppReturnType Del_Open(EAppMsgType messageType, ref FScriptArray message, ref FScriptArray optTitle);
        public delegate void Del_Log(ref FScriptArray message, ref FScriptArray categoryName, ELogVerbosity verbosity);

        public static Del_Open Open;
        public static Del_Log Log;
    }
}

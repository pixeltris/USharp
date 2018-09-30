using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FAsync
    {
        public delegate void Del_AsyncTask(FSimpleDelegate func, EAsyncThreadType threadType);

        public static Del_AsyncTask AsyncTask;
    }
}

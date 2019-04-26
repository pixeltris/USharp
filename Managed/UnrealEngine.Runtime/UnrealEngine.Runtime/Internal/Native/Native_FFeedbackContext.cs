using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FFeedbackContext
    {
        public delegate IntPtr Del_GetDesktopFeedbackContext();
        public delegate void Del_BeginSlowTask(IntPtr instance, ref FScriptArray task, csbool showProgressDialog, csbool showCancelButton);
        public delegate void Del_UpdateProgress(IntPtr instance, int numerator, int denominator);
        public delegate void Del_StatusUpdate(IntPtr instance, int numerator, int denominator, ref FScriptArray statusText);
        public delegate void Del_StatusForceUpdate(IntPtr instance, int numerator, int denominator, ref FScriptArray statusText);
        public delegate void Del_EndSlowTask(IntPtr instance);

        public static Del_GetDesktopFeedbackContext GetDesktopFeedbackContext;
        public static Del_BeginSlowTask BeginSlowTask;
        public static Del_UpdateProgress UpdateProgress;
        public static Del_StatusUpdate StatusUpdate;
        public static Del_StatusForceUpdate StatusForceUpdate;
        public static Del_EndSlowTask EndSlowTask;
    }
}

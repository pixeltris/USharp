using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// A context for displaying modal warning messages.
    /// </summary>
    public class FFeedbackContext
    {
        public IntPtr Address { get; private set; }

        public FFeedbackContext(IntPtr address)
        {
            Address = address;
        }

        public static FFeedbackContext GetGetDesktopFeedbackContext()
        {
            return new FFeedbackContext(Native_FFeedbackContext.GetDesktopFeedbackContext());
        }

        public void BeginSlowTask(string task, bool showProgressDialog, bool showCancelButton = false)
        {
            using (FStringUnsafe taskUnsafe = new FStringUnsafe(task))
            {
                Native_FFeedbackContext.BeginSlowTask(Address, ref taskUnsafe.Array, showProgressDialog, showCancelButton);
            }
        }

        public void UpdateProgress(int numerator, int denominator)
        {
            Native_FFeedbackContext.UpdateProgress(Address, numerator, denominator);
        }

        public void StatusUpdate(int numerator, int denominator, string statusText)
        {
            using (FStringUnsafe statusTextUnsafe = new FStringUnsafe(statusText))
            {
                Native_FFeedbackContext.StatusUpdate(Address, numerator, denominator, ref statusTextUnsafe.Array);
            }
        }

        public void StatusForceUpdate(int numerator, int denominator, string statusText)
        {
            using (FStringUnsafe statusTextUnsafe = new FStringUnsafe(statusText))
            {
                Native_FFeedbackContext.StatusForceUpdate(Address, numerator, denominator, ref statusTextUnsafe.Array);
            }
        }

        public void EndSlowTask()
        {
            Native_FFeedbackContext.EndSlowTask(Address);
        }
    }
}

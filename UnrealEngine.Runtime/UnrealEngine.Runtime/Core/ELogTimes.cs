using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Enum that defines how the log times are to be displayed.
    /// </summary>
    public enum ELogTimes
    {
        /// <summary>
        /// Do not display log timestamps
        /// </summary>
        None,

        /// <summary>
        /// Display log timestamps in UTC
        /// </summary>
        UTC,

        /// <summary>
        /// Display log timestamps in seconds elapsed since GStartTime
        /// </summary>
        SinceGStartTime
    }
}

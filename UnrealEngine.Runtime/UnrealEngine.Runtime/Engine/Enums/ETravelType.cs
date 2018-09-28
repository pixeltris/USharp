using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Engine
{
    /// <summary>
    /// Traveling from server to server.
    /// </summary>
    public enum ETravelType
    {
        /// <summary>
        /// Absolute URL.
        /// </summary>
        Absolute,

        /// <summary>
        /// Partial (carry name, reset server).
        /// </summary>
        Partial,

        /// <summary>
        /// Relative URL.
        /// </summary>
        Relative
    }
}

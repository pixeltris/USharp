using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public enum EDuplicateMode
    {
        /// <summary>
        /// No specific information about the reason for duplication
        /// </summary>
        Normal,

        /// <summary>
        /// Object is being duplicated as part of a world duplication
        /// </summary>
        World,

        /// <summary>
        /// Object is being duplicated as part of the process for entering Play In Editor
        /// </summary>
        PIE
    }
}

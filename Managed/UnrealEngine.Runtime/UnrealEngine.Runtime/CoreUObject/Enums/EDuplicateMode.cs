using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public enum EDuplicateMode
    {
        Normal,

        /// <summary>
        /// Object is being duplicated as part of world duplication
        /// </summary>
        World,

        /// <summary>
        /// Object is being duplicated as part of world duplication for PIE
        /// </summary>
        PIE
    }
}

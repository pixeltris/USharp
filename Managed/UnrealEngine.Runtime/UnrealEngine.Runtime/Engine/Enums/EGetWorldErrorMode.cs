using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Engine
{
    // Engine/Source/Runtime/Engine/Classes/Engine/Engine.h

    /// <summary>
    /// The kind of failure handling that GetWorldFromContextObject uses 
    /// </summary>
    public enum EGetWorldErrorMode : int
    {
        /// <summary>
        /// Silently returns nullptr, the calling code is expected to handle this gracefully
        /// </summary>
        ReturnNull,

        /// <summary>
        /// Raises a runtime error but still returns nullptr, the calling code is expected to handle this gracefully
        /// </summary>
        LogAndReturnNull,

        /// <summary>
        /// Asserts, the calling code is not expecting to handle a failure gracefully
        /// </summary>
        Assert
    }
}

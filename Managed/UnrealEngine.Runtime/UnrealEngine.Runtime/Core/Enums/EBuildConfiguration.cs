using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Enumerates build configurations.
    /// </summary>
    public enum EBuildConfiguration : int
    {
        /// <summary>
        /// Unknown build configuration.
        /// </summary>
        Unknown,

        /// <summary>
        /// Debug build.
        /// </summary>
        Debug,

        /// <summary>
        /// DebugGame build.
        /// </summary>
        DebugGame,

        /// <summary>
        /// Development build.
        /// </summary>
        Development,

        /// <summary>
        /// Shipping build.
        /// </summary>
        Shipping,

        /// <summary>
        /// Test build.
        /// </summary>
        Test
    }
}

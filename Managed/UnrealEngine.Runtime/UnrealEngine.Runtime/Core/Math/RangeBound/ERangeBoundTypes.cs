using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\RangeBound.h

    /// <summary>Enumerates the valid types of range bounds.</summary>
    [UEnum, BlueprintType, UMetaPath("/Script/CoreUObject.ERangeBoundTypes", "CoreUObject", UnrealModuleType.Engine)]
    public enum ERangeBoundTypes : byte
    {
        /// <summary>The range excludes the bound.</summary>
        Exclusive = 0,
        /// <summary>The range includes the bound.</summary>
        Inclusive = 1,
        /// <summary>The bound is open.</summary>
        Open = 2
    }
}

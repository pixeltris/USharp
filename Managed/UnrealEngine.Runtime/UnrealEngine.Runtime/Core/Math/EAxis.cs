using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\Axis.h

    /// <summary>Generic axis enum (mirrored for property use in Object.h)</summary>
    [UEnum, UMetaPath("/Script/CoreUObject.EAxis", "CoreUObject", UnrealModuleType.Engine)]
    public enum EAxis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 3
    }
}

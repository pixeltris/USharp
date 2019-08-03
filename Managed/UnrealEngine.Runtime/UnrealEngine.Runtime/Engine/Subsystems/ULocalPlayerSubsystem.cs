using System;
using UnrealEngine.Runtime;

namespace UnrealEngine.Engine
{
    /// <summary>
    /// Base class for auto instanced and initialized subsystem that share the lifetime of local players.
    /// </summary>
    [UClass, UMetaPath("/Script/Engine.LocalPlayerSubsystem")]
    public partial class ULocalPlayerSubsystem : USubsystem
    {
    }
}

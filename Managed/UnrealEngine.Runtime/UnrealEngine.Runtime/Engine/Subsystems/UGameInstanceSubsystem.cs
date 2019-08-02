using System;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    /// <summary>
    /// Base class for auto instanced and initialized systems that share the lifetime of the game instance.
    /// </summary>
    [UClass, UMetaPath("/Script/Engine.GameInstanceSubsystem")]
    public partial class UGameInstanceSubsystem : USubsystem
    {
    }
}

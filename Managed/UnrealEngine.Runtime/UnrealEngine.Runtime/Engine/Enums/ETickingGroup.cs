using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;

namespace UnrealEngine.Engine
{
    // Engine\Source\Runtime\Engine\Classes\Engine\EngineBaseTypes.h

    /// <summary>
    /// Determines which ticking group a tick function belongs to.
    /// </summary>
    [UEnum, BlueprintType, UMetaPath("/Script/Engine.ETickingGroup", "Engine", UnrealModuleType.Engine)]
    public enum ETickingGroup : byte
    {
        /// <summary>Any item that needs to be executed before physics simulation starts.</summary>
        PrePhysics = 0,
        /// <summary>Special tick group that starts physics simulation.</summary>
        StartPhysics = 1,
        /// <summary>Any item that can be run in parallel with our physics simulation work.</summary>
        DuringPhysics = 2,
        /// <summary>Special tick group that ends physics simulation.</summary>
        EndPhysics = 3,
        /// <summary>Any item that needs rigid body and cloth simulation to be complete before being executed.</summary>
        PostPhysics = 4,
        /// <summary>Any item that needs the update work to be done before being ticked.</summary>
        PostUpdateWork = 5,
        /// <summary>Catchall for anything demoted to the end.</summary>
        LastDemotable = 6,
        /// <summary>Special tick group that is not actually a tick group. After every tick group this is repeatedly re-run until there are no more newly spawned items to run.</summary>
        NewlySpawned = 7
    }
}

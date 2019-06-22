using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;

namespace UnrealEngine.GameplayTasks
{
    // Making sure these are exported and constrained to byte

    [UEnum, UMetaPath("/Script/GameplayTasks.EGameplayTaskState")]
    public enum EGameplayTaskState : byte
    {
        Uninitialized = 0,
        AwaitingActivation = 1,
        Paused = 2,
        Active = 3,
        Finished = 4
    }

    [UEnum, UMetaPath("/Script/GameplayTasks.EGameplayTaskState")]
    public enum ETaskResourceOverlapPolicy : byte
    {
        /// <summary>Pause overlapping same-priority tasks.</summary>
        StartOnTop = 0,
        /// <summary>Wait for other same-priority tasks to finish.</summary>
        StartAtEnd = 1
    }
}

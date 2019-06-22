using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;
using UnrealEngine.Engine;

namespace UnrealEngine.GameplayTasks
{
    public static class IGameplayTaskOwnerInterfaceExtensions
    {
        /// <summary>
        /// Finds tasks component for given GameplayTask, Task.GetGameplayTasksComponent() may not be initialized at this point!
        /// </summary>
        public static UGameplayTasksComponent GetGameplayTasksComponent(this IGameplayTaskOwnerInterface obj, UGameplayTask task)
        {
            return GCHelper.Find<UGameplayTasksComponent>(
                Native_IGameplayTaskOwnerInterface.Internal_GetGameplayTasksComponent(obj.GetAddress(), task.Address));
        }

        /// <summary>
        /// Get "body" of task's owner / default, having location in world (e.g. Owner = AIController, Avatar = Pawn)
        /// </summary>
        public static AActor GetGameplayTaskOwner(this IGameplayTaskOwnerInterface obj, UGameplayTask task)
        {
            return GCHelper.Find<AActor>(
                Native_IGameplayTaskOwnerInterface.Internal_GetGameplayTaskOwner(obj.GetAddress(), task.Address));
        }

        /// <summary>
        /// Get "body" of task's owner / default, having location in world (e.g. Owner = AIController, Avatar = Pawn)
        /// </summary>
        public static AActor GetGameplayTaskAvatar(this IGameplayTaskOwnerInterface obj, UGameplayTask task)
        {
            return GCHelper.Find<AActor>(
                Native_IGameplayTaskOwnerInterface.Internal_GetGameplayTaskAvatar(obj.GetAddress(), task.Address));
        }

        /// <summary>
        /// Get default priority for running a task
        /// </summary>
        public static byte GetGameplayTaskDefaultPriority(this IGameplayTaskOwnerInterface obj)
        {
            return Native_IGameplayTaskOwnerInterface.Internal_GetGameplayTaskDefaultPriority(obj.GetAddress());
        }

        /// <summary>
        /// Notify called after GameplayTask finishes initialization (not active yet)
        /// </summary>
        public static void OnGameplayTaskInitialized(this IGameplayTaskOwnerInterface obj, UGameplayTask task)
        {
            Native_IGameplayTaskOwnerInterface.Internal_OnGameplayTaskInitialized(obj.GetAddress(), task.Address);
        }

        /// <summary>
        /// Notify called after GameplayTask changes state to Active (initial activation or resuming)
        /// </summary>
        public static void OnGameplayTaskActivated(this IGameplayTaskOwnerInterface obj, UGameplayTask task)
        {
            Native_IGameplayTaskOwnerInterface.Internal_OnGameplayTaskActivated(obj.GetAddress(), task.Address);
        }

        /// <summary>
        /// Notify called after GameplayTask changes state from Active (finishing or pausing)
        /// </summary>
        public static void OnGameplayTaskDeactivated(this IGameplayTaskOwnerInterface obj, UGameplayTask task)
        {
            Native_IGameplayTaskOwnerInterface.Internal_OnGameplayTaskDeactivated(obj.GetAddress(), task.Address);
        }
    }
}

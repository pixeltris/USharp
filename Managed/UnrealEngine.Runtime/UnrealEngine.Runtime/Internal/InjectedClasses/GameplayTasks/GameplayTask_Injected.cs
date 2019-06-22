using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;
using UnrealEngine.Engine;

namespace UnrealEngine.GameplayTasks
{
    public partial class UGameplayTask : UnrealEngine.Runtime.UObject
    {
        /// <summary>
        /// This name allows us to find the task later so that we can end it.
        /// </summary>
        protected FName InstanceName
        {
            get
            {
                FName result;
                Native_UUSharpGameplayTask.Internal_Get_InstanceName(Address, out result);
                return result;
            }
            set
            {
                Native_UUSharpGameplayTask.Internal_Set_InstanceName(Address, ref value);
            }
        }

        /// <summary>
        /// This controls how this task will be treaded in relation to other, already running tasks, 
        /// provided GameplayTasksComponent is configured to care about priorities (the default behavior)
        /// </summary>
        protected byte Priority
        {
            get { return Native_UUSharpGameplayTask.Internal_Get_Priority(Address); }
            set { Native_UUSharpGameplayTask.Internal_Set_Priority(Address, value); }
        }

        protected ETaskResourceOverlapPolicy ResourceOverlapPolicy
        {
            get { return (ETaskResourceOverlapPolicy)Native_UUSharpGameplayTask.Internal_Get_ResourceOverlapPolicy(Address); }
            set { Native_UUSharpGameplayTask.Internal_Set_ResourceOverlapPolicy(Address, (byte)value); }
        }

        /// <summary>
        /// If true, this task will receive TickTask calls from TasksComponent
        /// </summary>
        protected bool bTickingTask
        {
            get { return Native_UUSharpGameplayTask.Internal_Get_bTickingTask(Address); }
            set { Native_UUSharpGameplayTask.Internal_Set_bTickingTask(Address, value); }
        }

        /// <summary>
        /// Should this task run on simulated clients? This should only be used in rare cases, such as movement tasks. Simulated Tasks do not broadcast their end delegates.
        /// </summary>
        protected bool bSimulatedTask
        {
            get { return Native_UUSharpGameplayTask.Internal_Get_bSimulatedTask(Address); }
            set { Native_UUSharpGameplayTask.Internal_Set_bSimulatedTask(Address, value); }
        }

        /// <summary>
        /// Am I actually running this as a simulated task. (This will be true on clients that simulating. This will be false on the server and the owning client)
        /// </summary>
        protected bool bIsSimulating
        {
            get { return Native_UUSharpGameplayTask.Internal_Get_bIsSimulating(Address); }
            set { Native_UUSharpGameplayTask.Internal_Set_bIsSimulating(Address, value); }
        }

        protected bool bIsPausable
        {
            get { return Native_UUSharpGameplayTask.Internal_Get_bIsPausable(Address); }
            set { Native_UUSharpGameplayTask.Internal_Set_bIsPausable(Address, value); }
        }

        protected bool bCaresAboutPriority
        {
            get { return Native_UUSharpGameplayTask.Internal_Get_bCaresAboutPriority(Address); }
            set { Native_UUSharpGameplayTask.Internal_Set_bCaresAboutPriority(Address, value); }
        }

        /// <summary>
        /// this is set to avoid duplicate calls to task's owner and TasksComponent when both are the same object
        /// </summary>
        protected bool bOwnedByTasksComponent
        {
            get { return Native_UUSharpGameplayTask.Internal_Get_bOwnedByTasksComponent(Address); }
            set { Native_UUSharpGameplayTask.Internal_Set_bOwnedByTasksComponent(Address, value); }
        }

        protected bool bClaimRequiredResources
        {
            get { return Native_UUSharpGameplayTask.Internal_Get_bClaimRequiredResources(Address); }
            set { Native_UUSharpGameplayTask.Internal_Set_bClaimRequiredResources(Address, value); }
        }

        protected bool bOwnerFinished
        {
            get { return Native_UUSharpGameplayTask.Internal_Get_bOwnerFinished(Address); }
            set { Native_UUSharpGameplayTask.Internal_Set_bOwnerFinished(Address, value); }
        }

        /// <summary>
        /// Abstract "resource" IDs this task needs available to be able to get activated.
        /// </summary>
        protected FGameplayResourceSet RequiredResources
        {
            get
            {
                FGameplayResourceSet result;
                Native_UUSharpGameplayTask.Internal_Get_RequiredResources(Address, out result);
                return result;
            }
            set
            {
                Native_UUSharpGameplayTask.Internal_Set_RequiredResources(Address, ref value);
            }
        }

        /// <summary>
        /// Resources that are going to be locked when this task gets activated, but are not required to get this task started
        /// </summary>
        protected FGameplayResourceSet ClaimedResources
        {
            get
            {
                FGameplayResourceSet result;
                Native_UUSharpGameplayTask.Internal_Get_ClaimedResources(Address, out result);
                return result;
            }
            set
            {
                Native_UUSharpGameplayTask.Internal_Set_ClaimedResources(Address, ref value);
            }
        }

        /// <summary>
        /// Task Owner that created us
        /// </summary>
        protected IGameplayTaskOwnerInterface TaskOwner
        {
            get
            {
                UObject obj = GCHelper.Find(Native_UUSharpGameplayTask.Internal_Get_TaskOwner(Address));
                return obj.GetInterface<IGameplayTaskOwnerInterface>();
            }
        }

        protected UGameplayTasksComponent TasksComponent
        {
            get { return GCHelper.Find<UGameplayTasksComponent>(Native_UUSharpGameplayTask.Internal_Get_TasksComponent(Address)); }
        }

        /// <summary>
        /// child task instance
        /// </summary>
        protected UGameplayTask ChildTask
        {
            get { return GCHelper.Find<UGameplayTask>(Native_UUSharpGameplayTask.Internal_Get_ChildTask(Address)); }
        }

        /// <summary>
        /// Initailizes the task with the task owner interface instance but does not actviate until Activate() is called
        /// </summary>
        protected void InitTask(IGameplayTaskOwnerInterface taskOwner, byte priority)
        {
            Native_UUSharpGameplayTask.Internal_InitTask(Address, taskOwner.GetAddress(), priority);
        }

        protected static IGameplayTaskOwnerInterface ConvertToTaskOwner(UObject ownerObject)
        {
            UObject obj = GCHelper.Find(Native_UUSharpGameplayTask.Internal_ConvertToTaskOwner(ownerObject.Address));
            if (obj != null)
            {
                return obj.GetInterface<IGameplayTaskOwnerInterface>();
            }
            return null;
        }

        protected static IGameplayTaskOwnerInterface ConvertToTaskOwner(AActor ownerActor)
        {
            UObject obj = GCHelper.Find(Native_UUSharpGameplayTask.Internal_ConvertToTaskOwnerActor(ownerActor.Address));
            if (obj != null)
            {
                return obj.GetInterface<IGameplayTaskOwnerInterface>();
            }
            return null;
        }

        /////////////////////////////////////////////////////////
        // Everything above this line belongs to UUSharpGameplayTask. Putting them here as they allow us to access these
        // protected members from C# when inheriting from something other than UUSharpGameplayTask
        /////////////////////////////////////////////////////////

        /// <summary>
        /// Called to trigger the actual task once the delegates have been set up
        /// </summary>
        public void ReadyForActivation()
        {
            Native_UGameplayTask.ReadyForActivation(Address);
        }

        public void InitSimulatedTask(UGameplayTasksComponent gameplayTasksComponent)
        {
            Native_UGameplayTask.InitSimulatedTask(Address,
                gameplayTasksComponent == null ? IntPtr.Zero : gameplayTasksComponent.Address);
        }

        /// <summary>
        /// Called when the task is asked to confirm from an outside node. What this means depends on the individual task. By default, this does nothing other than ending if bEndTask is true.
        /// </summary>
        public void ExternalConfirm(bool endTask)
        {
            Native_UGameplayTask.ExternalConfirm(Address, endTask);
        }

        public void ExternalCancel()
        {
            Native_UGameplayTask.ExternalCancel(Address);
        }

        /// <summary>
        /// Return debug string describing task
        /// </summary>
        public string GetDebugString()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UGameplayTask.GetDebugString(Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Proper way to get the owning actor of task owner. This can be the owner itself since the owner is given as a interface
        /// </summary>
        public AActor GetOwnerActor()
        {
            return GCHelper.Find<AActor>(Native_UGameplayTask.GetOwnerActor(Address));
        }

        /// <summary>
        /// Proper way to get the avatar actor associated with the task owner (usually a pawn, tower, etc)
        /// </summary>
        public AActor GetAvatarActor()
        {
            return GCHelper.Find<AActor>(Native_UGameplayTask.GetAvatarActor(Address));
        }

        /// <summary>
        /// Called explicitly to end the task (usually by the task itself). Calls OnDestroy. 
        /// @NOTE: you need to call EndTask before sending out any "on completed" delegates. 
        /// If you don't the task will still be in an "active" state while the event receivers may
        /// assume it's already "finished" */
        /// </summary>
        public void EndTask()
        {
            Native_UGameplayTask.EndTask(Address);
        }

        public FName GetInstanceName()
        {
            FName result;
            Native_UGameplayTask.GetInstanceName(Address, out result);
            return result;
        }

        public bool IsTickingTask()
        {
            return Native_UGameplayTask.IsTickingTask(Address);
        }

        public bool IsSimulatedTask()
        {
            return Native_UGameplayTask.IsSimulatedTask(Address);
        }

        public bool IsSimulating()
        {
            return Native_UGameplayTask.IsSimulating(Address);
        }

        public bool IsPausable()
        {
            return Native_UGameplayTask.IsPausable(Address);
        }

        public bool HasOwnerFinished()
        {
            return Native_UGameplayTask.HasOwnerFinished(Address);
        }

        public byte GetPriority()
        {
            return Native_UGameplayTask.GetPriority(Address);
        }

        public bool RequiresPriorityOrResourceManagement()
        {
            return Native_UGameplayTask.RequiresPriorityOrResourceManagement(Address);
        }

        public FGameplayResourceSet GetRequiredResources()
        {
            FGameplayResourceSet result;
            Native_UGameplayTask.GetRequiredResources(Address, out result);
            return result;
        }

        public FGameplayResourceSet GetClaimedResources()
        {
            FGameplayResourceSet result;
            Native_UGameplayTask.GetClaimedResources(Address, out result);
            return result;
        }

        public EGameplayTaskState GetState()
        {
            return (EGameplayTaskState)Native_UGameplayTask.GetState(Address);
        }

        public bool IsActive()
        {
            return Native_UGameplayTask.IsActive(Address);
        }

        public bool IsPaused()
        {
            return Native_UGameplayTask.IsPaused(Address);
        }

        public bool IsFinished()
        {
            return Native_UGameplayTask.IsFinished(Address);
        }

        public UGameplayTask GetChildTask()
        {
            return GCHelper.Find<UGameplayTask>(Native_UGameplayTask.GetChildTask(Address));
        }

        public IGameplayTaskOwnerInterface GetTaskOwner()
        {
            UObject obj = GCHelper.Find(Native_UGameplayTask.GetTaskOwner(Address));
            if (obj != null)
            {
                return obj.GetInterface<IGameplayTaskOwnerInterface>();
            }
            return null;
        }

        public UGameplayTasksComponent GetGameplayTasksComponent()
        {
            return GCHelper.Find<UGameplayTasksComponent>(Native_UGameplayTask.GetGameplayTasksComponent(Address));
        }

        public bool IsOwnedByTasksComponent()
        {
            return Native_UGameplayTask.IsOwnedByTasksComponent(Address);
        }

        /// <summary>
        /// Marks this task as requiring specified resource which has a number of consequences,
        /// like task not being able to run if the resource is already taken.
        /// 
        /// @note: Calling this function makes sense only until the task is being passed over to the GameplayTasksComponent.
        /// Once that's that resources data is consumed and further changes won't get applied 
        /// </summary>
        public void AddRequiredResource<T>() where T : UGameplayTaskResource
        {
            AddRequiredResource(TSubclassOf<UGameplayTaskResource>.From<T>());
        }

        public void AddRequiredResource(TSubclassOf<UGameplayTaskResource> requiredResource)
        {
            Native_UGameplayTask.AddRequiredResource(Address, ref requiredResource.subclassOf);
        }

        public void AddClaimedResource<T>() where T : UGameplayTaskResource
        {
            AddClaimedResource(TSubclassOf<UGameplayTaskResource>.From<T>());
        }

        public void AddClaimedResource(TSubclassOf<UGameplayTaskResource> claimedResource)
        {
            Native_UGameplayTask.AddClaimedResource(Address, ref claimedResource.subclassOf);
        }

        public ETaskResourceOverlapPolicy GetResourceOverlapPolicy()
        {
            return (ETaskResourceOverlapPolicy)Native_UGameplayTask.GetResourceOverlapPolicy(Address);
        }

        public bool IsWaitingOnRemotePlayerdata()
        {
            return Native_UGameplayTask.IsWaitingOnRemotePlayerdata(Address);
        }

        public bool IsWaitingOnAvatar()
        {
            return Native_UGameplayTask.IsWaitingOnAvatar(Address);
        }
            
        public string GetDebugDescription()
        {
            // TODO: Use #if WITH_EDITOR
            if (!FBuild.WithEditor)
            {
                return null;
            }

            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UGameplayTask.GetDebugDescription(Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }
    }
}

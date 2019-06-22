using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Engine;
using UnrealEngine.GameplayTasks;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_UUSharpGameplayTask
    {
        public delegate void Del_Base_Activate(IntPtr instance);
        public delegate void Del_Base_InitSimulatedTask(IntPtr instance, IntPtr gameplayTasksComponent);
        public delegate void Del_Base_ExternalConfirm(IntPtr instance, csbool endTask);
        public delegate void Del_Base_ExternalCancel(IntPtr instance);
        public delegate void Del_Base_GetDebugString(IntPtr instance, ref FScriptArray result);
        public delegate void Del_Base_OnDestroy(IntPtr instance, csbool ownerFinished);
        public delegate void Del_Base_Pause(IntPtr instance);
        public delegate void Del_Base_Resume(IntPtr instance);
        public delegate void Del_Base_GenerateDebugDescription(IntPtr instance, ref FScriptArray result);
        public delegate void Del_Internal_Get_InstanceName(IntPtr instance, out FName result);
        public delegate void Del_Internal_Set_InstanceName(IntPtr instance, ref FName value);
        public delegate byte Del_Internal_Get_Priority(IntPtr instance);
        public delegate void Del_Internal_Set_Priority(IntPtr instance, byte value);
        public delegate byte Del_Internal_Get_ResourceOverlapPolicy(IntPtr instance);
        public delegate void Del_Internal_Set_ResourceOverlapPolicy(IntPtr instance, byte value);
        public delegate csbool Del_Internal_Get_bTickingTask(IntPtr instance);
        public delegate void Del_Internal_Set_bTickingTask(IntPtr instance, csbool value);
        public delegate csbool Del_Internal_Get_bSimulatedTask(IntPtr instance);
        public delegate void Del_Internal_Set_bSimulatedTask(IntPtr instance, csbool value);
        public delegate csbool Del_Internal_Get_bIsSimulating(IntPtr instance);
        public delegate void Del_Internal_Set_bIsSimulating(IntPtr instance, csbool value);
        public delegate csbool Del_Internal_Get_bIsPausable(IntPtr instance);
        public delegate void Del_Internal_Set_bIsPausable(IntPtr instance, csbool value);
        public delegate csbool Del_Internal_Get_bCaresAboutPriority(IntPtr instance);
        public delegate void Del_Internal_Set_bCaresAboutPriority(IntPtr instance, csbool value);
        public delegate csbool Del_Internal_Get_bOwnedByTasksComponent(IntPtr instance);
        public delegate void Del_Internal_Set_bOwnedByTasksComponent(IntPtr instance, csbool value);
        public delegate csbool Del_Internal_Get_bClaimRequiredResources(IntPtr instance);
        public delegate void Del_Internal_Set_bClaimRequiredResources(IntPtr instance, csbool value);
        public delegate csbool Del_Internal_Get_bOwnerFinished(IntPtr instance);
        public delegate void Del_Internal_Set_bOwnerFinished(IntPtr instance, csbool value);
        public delegate void Del_Internal_Get_RequiredResources(IntPtr instance, out FGameplayResourceSet result);
        public delegate void Del_Internal_Set_RequiredResources(IntPtr instance, ref FGameplayResourceSet value);
        public delegate void Del_Internal_Get_ClaimedResources(IntPtr instance, out FGameplayResourceSet result);
        public delegate void Del_Internal_Set_ClaimedResources(IntPtr instance, ref FGameplayResourceSet value);
        public delegate IntPtr Del_Internal_Get_TaskOwner(IntPtr instance);
        public delegate IntPtr Del_Internal_Get_TasksComponent(IntPtr instance);
        public delegate IntPtr Del_Internal_Get_ChildTask(IntPtr instance);
        public delegate void Del_Internal_InitTask(IntPtr instance, IntPtr taskOwner, byte priority);
        public delegate IntPtr Del_Internal_ConvertToTaskOwner(IntPtr ownerObject);
        public delegate IntPtr Del_Internal_ConvertToTaskOwnerActor(IntPtr ownerActor);
        
        internal static ManagedLatentCallbackRegisterDel Set_Callback;
        public static Del_Base_Activate Base_Activate;
        public static Del_Base_InitSimulatedTask Base_InitSimulatedTask;
        public static Del_Base_ExternalConfirm Base_ExternalConfirm;
        public static Del_Base_ExternalCancel Base_ExternalCancel;
        public static Del_Base_GetDebugString Base_GetDebugString;
        public static Del_Base_OnDestroy Base_OnDestroy;
        public static Del_Base_Pause Base_Pause;
        public static Del_Base_Resume Base_Resume;
        public static Del_Base_GenerateDebugDescription Base_GenerateDebugDescription;
        public static Del_Internal_Get_InstanceName Internal_Get_InstanceName;
        public static Del_Internal_Set_InstanceName Internal_Set_InstanceName;
        public static Del_Internal_Get_Priority Internal_Get_Priority;
        public static Del_Internal_Set_Priority Internal_Set_Priority;
        public static Del_Internal_Get_ResourceOverlapPolicy Internal_Get_ResourceOverlapPolicy;
        public static Del_Internal_Set_ResourceOverlapPolicy Internal_Set_ResourceOverlapPolicy;
        public static Del_Internal_Get_bTickingTask Internal_Get_bTickingTask;
        public static Del_Internal_Set_bTickingTask Internal_Set_bTickingTask;
        public static Del_Internal_Get_bSimulatedTask Internal_Get_bSimulatedTask;
        public static Del_Internal_Set_bSimulatedTask Internal_Set_bSimulatedTask;
        public static Del_Internal_Get_bIsSimulating Internal_Get_bIsSimulating;
        public static Del_Internal_Set_bIsSimulating Internal_Set_bIsSimulating;
        public static Del_Internal_Get_bIsPausable Internal_Get_bIsPausable;
        public static Del_Internal_Set_bIsPausable Internal_Set_bIsPausable;
        public static Del_Internal_Get_bCaresAboutPriority Internal_Get_bCaresAboutPriority;
        public static Del_Internal_Set_bCaresAboutPriority Internal_Set_bCaresAboutPriority;
        public static Del_Internal_Get_bOwnedByTasksComponent Internal_Get_bOwnedByTasksComponent;
        public static Del_Internal_Set_bOwnedByTasksComponent Internal_Set_bOwnedByTasksComponent;
        public static Del_Internal_Get_bClaimRequiredResources Internal_Get_bClaimRequiredResources;
        public static Del_Internal_Set_bClaimRequiredResources Internal_Set_bClaimRequiredResources;
        public static Del_Internal_Get_bOwnerFinished Internal_Get_bOwnerFinished;
        public static Del_Internal_Set_bOwnerFinished Internal_Set_bOwnerFinished;
        public static Del_Internal_Get_RequiredResources Internal_Get_RequiredResources;
        public static Del_Internal_Set_RequiredResources Internal_Set_RequiredResources;
        public static Del_Internal_Get_ClaimedResources Internal_Get_ClaimedResources;
        public static Del_Internal_Set_ClaimedResources Internal_Set_ClaimedResources;
        public static Del_Internal_Get_TaskOwner Internal_Get_TaskOwner;
        public static Del_Internal_Get_TasksComponent Internal_Get_TasksComponent;
        public static Del_Internal_Get_ChildTask Internal_Get_ChildTask;
        public static Del_Internal_InitTask Internal_InitTask;
        public static Del_Internal_ConvertToTaskOwner Internal_ConvertToTaskOwner;
        public static Del_Internal_ConvertToTaskOwnerActor Internal_ConvertToTaskOwnerActor;
    }
}

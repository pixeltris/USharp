using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.GameplayTasks;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_UGameplayTask
    {
        public delegate void Del_ReadyForActivation(IntPtr instance);
        public delegate void Del_InitSimulatedTask(IntPtr instance, IntPtr gameplayTasksComponent);
        public delegate void Del_ExternalConfirm(IntPtr instance, csbool endTask);
        public delegate void Del_ExternalCancel(IntPtr instance);
        public delegate void Del_GetDebugString(IntPtr instance, ref FScriptArray result);
        public delegate IntPtr Del_GetOwnerActor(IntPtr instance);
        public delegate IntPtr Del_GetAvatarActor(IntPtr instance);
        public delegate void Del_EndTask(IntPtr instance);
        public delegate void Del_GetInstanceName(IntPtr instance, out FName result);
        public delegate csbool Del_IsTickingTask(IntPtr instance);
        public delegate csbool Del_IsSimulatedTask(IntPtr instance);
        public delegate csbool Del_IsSimulating(IntPtr instance);
        public delegate csbool Del_IsPausable(IntPtr instance);
        public delegate csbool Del_HasOwnerFinished(IntPtr instance);
        public delegate byte Del_GetPriority(IntPtr instance);
        public delegate csbool Del_RequiresPriorityOrResourceManagement(IntPtr instance);
        public delegate void Del_GetRequiredResources(IntPtr instance, out FGameplayResourceSet result);
        public delegate void Del_GetClaimedResources(IntPtr instance, out FGameplayResourceSet result);
        public delegate byte Del_GetState(IntPtr instance);
        public delegate csbool Del_IsActive(IntPtr instance);
        public delegate csbool Del_IsPaused(IntPtr instance);
        public delegate csbool Del_IsFinished(IntPtr instance);
        public delegate IntPtr Del_GetChildTask(IntPtr instance);
        public delegate IntPtr Del_GetTaskOwner(IntPtr instance);
        public delegate IntPtr Del_GetGameplayTasksComponent(IntPtr instance);
        public delegate csbool Del_IsOwnedByTasksComponent(IntPtr instance);
        public delegate void Del_AddRequiredResource(IntPtr instance, ref FSubclassOf requiredResource);
        public delegate void Del_AddClaimedResource(IntPtr instance, ref FSubclassOf claimedResource);
        public delegate byte Del_GetResourceOverlapPolicy(IntPtr instance);
        public delegate csbool Del_IsWaitingOnRemotePlayerdata(IntPtr instance);
        public delegate csbool Del_IsWaitingOnAvatar(IntPtr instance);
        public delegate void Del_GetDebugDescription(IntPtr instance, ref FScriptArray result);

        public static Del_ReadyForActivation ReadyForActivation;
        public static Del_InitSimulatedTask InitSimulatedTask;
        public static Del_ExternalConfirm ExternalConfirm;
        public static Del_ExternalCancel ExternalCancel;
        public static Del_GetDebugString GetDebugString;
        public static Del_GetOwnerActor GetOwnerActor;
        public static Del_GetAvatarActor GetAvatarActor;
        public static Del_EndTask EndTask;
        public static Del_GetInstanceName GetInstanceName;
        public static Del_IsTickingTask IsTickingTask;
        public static Del_IsSimulatedTask IsSimulatedTask;
        public static Del_IsSimulating IsSimulating;
        public static Del_IsPausable IsPausable;
        public static Del_HasOwnerFinished HasOwnerFinished;
        public static Del_GetPriority GetPriority;
        public static Del_RequiresPriorityOrResourceManagement RequiresPriorityOrResourceManagement;
        public static Del_GetRequiredResources GetRequiredResources;
        public static Del_GetClaimedResources GetClaimedResources;
        public static Del_GetState GetState;
        public static Del_IsActive IsActive;
        public static Del_IsPaused IsPaused;
        public static Del_IsFinished IsFinished;
        public static Del_GetChildTask GetChildTask;
        public static Del_GetTaskOwner GetTaskOwner;
        public static Del_GetGameplayTasksComponent GetGameplayTasksComponent;
        public static Del_IsOwnedByTasksComponent IsOwnedByTasksComponent;
        public static Del_AddRequiredResource AddRequiredResource;
        public static Del_AddClaimedResource AddClaimedResource;
        public static Del_GetResourceOverlapPolicy GetResourceOverlapPolicy;
        public static Del_IsWaitingOnRemotePlayerdata IsWaitingOnRemotePlayerdata;
        public static Del_IsWaitingOnAvatar IsWaitingOnAvatar;
        public static Del_GetDebugDescription GetDebugDescription;
    }
}

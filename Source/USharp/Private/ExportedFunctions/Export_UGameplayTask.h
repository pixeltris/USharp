CSEXPORT void CSCONV Export_UGameplayTask_ReadyForActivation(UGameplayTask* instance)
{
	instance->ReadyForActivation();
}

CSEXPORT void CSCONV Export_UGameplayTask_InitSimulatedTask(UGameplayTask* instance, UGameplayTasksComponent& InGameplayTasksComponent)
{
	instance->InitSimulatedTask(InGameplayTasksComponent);
}

CSEXPORT void CSCONV Export_UGameplayTask_ExternalConfirm(UGameplayTask* instance, csbool bEndTask)
{
	instance->ExternalConfirm((bool)bEndTask);
}

CSEXPORT void CSCONV Export_UGameplayTask_ExternalCancel(UGameplayTask* instance)
{
	instance->ExternalCancel();
}

CSEXPORT void CSCONV Export_UGameplayTask_GetDebugString(UGameplayTask* instance, FString& result)
{
	result = instance->GetDebugString();
}

CSEXPORT AActor* CSCONV Export_UGameplayTask_GetOwnerActor(UGameplayTask* instance)
{
	return instance->GetOwnerActor();
}

CSEXPORT AActor* CSCONV Export_UGameplayTask_GetAvatarActor(UGameplayTask* instance)
{
	return instance->GetAvatarActor();
}

CSEXPORT void CSCONV Export_UGameplayTask_EndTask(UGameplayTask* instance)
{
	instance->EndTask();
}

CSEXPORT void CSCONV Export_UGameplayTask_GetInstanceName(UGameplayTask* instance, FName& result)
{
	result = instance->GetInstanceName();
}

CSEXPORT csbool CSCONV Export_UGameplayTask_IsTickingTask(UGameplayTask* instance)
{
	return instance->IsTickingTask();
}

CSEXPORT csbool CSCONV Export_UGameplayTask_IsSimulatedTask(UGameplayTask* instance)
{
	return instance->IsSimulatedTask();
}

CSEXPORT csbool CSCONV Export_UGameplayTask_IsSimulating(UGameplayTask* instance)
{
	return instance->IsSimulating();
}

CSEXPORT csbool CSCONV Export_UGameplayTask_IsPausable(UGameplayTask* instance)
{
	return instance->IsPausable();
}

CSEXPORT csbool CSCONV Export_UGameplayTask_HasOwnerFinished(UGameplayTask* instance)
{
	return instance->HasOwnerFinished();
}

CSEXPORT uint8 CSCONV Export_UGameplayTask_GetPriority(UGameplayTask* instance)
{
	return instance->GetPriority();
}

CSEXPORT csbool CSCONV Export_UGameplayTask_RequiresPriorityOrResourceManagement(UGameplayTask* instance)
{
	return instance->RequiresPriorityOrResourceManagement();
}

CSEXPORT void CSCONV Export_UGameplayTask_GetRequiredResources(UGameplayTask* instance, FGameplayResourceSet& result)
{
	result = instance->GetRequiredResources();
}

CSEXPORT void CSCONV Export_UGameplayTask_GetClaimedResources(UGameplayTask* instance, FGameplayResourceSet& result)
{
	result = instance->GetClaimedResources();
}

CSEXPORT uint8 CSCONV Export_UGameplayTask_GetState(UGameplayTask* instance)
{
	return (uint8)instance->GetState();
}

CSEXPORT csbool CSCONV Export_UGameplayTask_IsActive(UGameplayTask* instance)
{
	return instance->IsActive();
}

CSEXPORT csbool CSCONV Export_UGameplayTask_IsPaused(UGameplayTask* instance)
{
	return instance->IsPaused();
}

CSEXPORT csbool CSCONV Export_UGameplayTask_IsFinished(UGameplayTask* instance)
{
	return instance->IsFinished();
}

CSEXPORT UGameplayTask* CSCONV Export_UGameplayTask_GetChildTask(UGameplayTask* instance)
{
	return instance->GetChildTask();
}

CSEXPORT UObject* CSCONV Export_UGameplayTask_GetTaskOwner(UGameplayTask* instance)
{
	return Cast<UObject>(instance);
}

CSEXPORT UGameplayTasksComponent* CSCONV Export_UGameplayTask_GetGameplayTasksComponent(UGameplayTask* instance)
{
	return instance->GetGameplayTasksComponent();
}

CSEXPORT csbool CSCONV Export_UGameplayTask_IsOwnedByTasksComponent(UGameplayTask* instance)
{
	return instance->IsOwnedByTasksComponent();
}

CSEXPORT void CSCONV Export_UGameplayTask_AddRequiredResource(UGameplayTask* instance, const TSubclassOf<UGameplayTaskResource>& RequiredResource)
{
	instance->AddRequiredResource(RequiredResource);
}

CSEXPORT void CSCONV Export_UGameplayTask_AddClaimedResource(UGameplayTask* instance, const TSubclassOf<UGameplayTaskResource>& ClaimedResource)
{
	instance->AddClaimedResource(ClaimedResource);
}

CSEXPORT uint8 CSCONV Export_UGameplayTask_GetResourceOverlapPolicy(UGameplayTask* instance)
{
	return (uint8)instance->GetResourceOverlapPolicy();
}

CSEXPORT csbool CSCONV Export_UGameplayTask_IsWaitingOnRemotePlayerdata(UGameplayTask* instance)
{
	return instance->IsWaitingOnRemotePlayerdata();
}

CSEXPORT csbool CSCONV Export_UGameplayTask_IsWaitingOnAvatar(UGameplayTask* instance)
{
	return instance->IsWaitingOnAvatar();
}

#if !(UE_BUILD_SHIPPING || UE_BUILD_TEST)
CSEXPORT void CSCONV Export_UGameplayTask_GetDebugDescription(UGameplayTask* instance, FString& result)
{
	result = instance->GetDebugDescription();
}
#endif

CSEXPORT void CSCONV Export_UGameplayTask(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UGameplayTask_ReadyForActivation);
	REGISTER_FUNC(Export_UGameplayTask_InitSimulatedTask);
	REGISTER_FUNC(Export_UGameplayTask_ExternalConfirm);
	REGISTER_FUNC(Export_UGameplayTask_ExternalCancel);
	REGISTER_FUNC(Export_UGameplayTask_GetDebugString);
	REGISTER_FUNC(Export_UGameplayTask_GetOwnerActor);
	REGISTER_FUNC(Export_UGameplayTask_GetAvatarActor);
	REGISTER_FUNC(Export_UGameplayTask_EndTask);
	REGISTER_FUNC(Export_UGameplayTask_GetInstanceName);
	REGISTER_FUNC(Export_UGameplayTask_IsTickingTask);
	REGISTER_FUNC(Export_UGameplayTask_IsSimulatedTask);
	REGISTER_FUNC(Export_UGameplayTask_IsSimulating);
	REGISTER_FUNC(Export_UGameplayTask_IsPausable);
	REGISTER_FUNC(Export_UGameplayTask_HasOwnerFinished);
	REGISTER_FUNC(Export_UGameplayTask_GetPriority);
	REGISTER_FUNC(Export_UGameplayTask_RequiresPriorityOrResourceManagement);
	REGISTER_FUNC(Export_UGameplayTask_GetRequiredResources);
	REGISTER_FUNC(Export_UGameplayTask_GetClaimedResources);
	REGISTER_FUNC(Export_UGameplayTask_GetState);
	REGISTER_FUNC(Export_UGameplayTask_IsActive);
	REGISTER_FUNC(Export_UGameplayTask_IsPaused);
	REGISTER_FUNC(Export_UGameplayTask_IsFinished);
	REGISTER_FUNC(Export_UGameplayTask_GetChildTask);
	REGISTER_FUNC(Export_UGameplayTask_GetTaskOwner);
	REGISTER_FUNC(Export_UGameplayTask_GetGameplayTasksComponent);
	REGISTER_FUNC(Export_UGameplayTask_IsOwnedByTasksComponent);
	REGISTER_FUNC(Export_UGameplayTask_AddRequiredResource);
	REGISTER_FUNC(Export_UGameplayTask_AddClaimedResource);
	REGISTER_FUNC(Export_UGameplayTask_GetResourceOverlapPolicy);
	REGISTER_FUNC(Export_UGameplayTask_IsWaitingOnRemotePlayerdata);
	REGISTER_FUNC(Export_UGameplayTask_IsWaitingOnAvatar);
#if !(UE_BUILD_SHIPPING || UE_BUILD_TEST)
	REGISTER_FUNC(Export_UGameplayTask_GetDebugDescription);
#endif
}
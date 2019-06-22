CSEXPORT void CSCONV Export_UUSharpGameplayTask_Set_Callback(ManagedLatentCallbackDel Callback)
{
	UUSharpGameplayTaskCallback = Callback;
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Base_Activate(UUSharpGameplayTask* instance)
{
	instance->Internal_Base_Activate();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Base_InitSimulatedTask(UUSharpGameplayTask* instance, UGameplayTasksComponent& InGameplayTasksComponent)
{
	instance->UGameplayTask::InitSimulatedTask(InGameplayTasksComponent);
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Base_ExternalConfirm(UUSharpGameplayTask* instance, csbool bEndTask)
{
	instance->UGameplayTask::ExternalConfirm((bool)bEndTask);
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Base_ExternalCancel(UUSharpGameplayTask* instance)
{
	instance->UGameplayTask::ExternalCancel();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Base_GetDebugString(UUSharpGameplayTask* instance, FString& result)
{
	result = instance->UGameplayTask::GetDebugString();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Base_OnDestroy(UUSharpGameplayTask* instance, csbool bInOwnerFinished)
{
	instance->Internal_Base_OnDestroy((bool)bInOwnerFinished);
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Base_Pause(UUSharpGameplayTask* instance)
{
	instance->Internal_Base_Pause();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Base_Resume(UUSharpGameplayTask* instance)
{
	instance->Internal_Base_Resume();
}

#if !(UE_BUILD_SHIPPING || UE_BUILD_TEST)
CSEXPORT void CSCONV Export_UUSharpGameplayTask_Base_GenerateDebugDescription(UUSharpGameplayTask* instance, FString& result)
{
	result = instance->UGameplayTask::GenerateDebugDescription();
}
#endif

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Get_InstanceName(UUSharpGameplayTask* instance, FName& result)
{
	result = instance->Internal_Get_InstanceName();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Set_InstanceName(UUSharpGameplayTask* instance, const FName& value)
{
	instance->Internal_Set_InstanceName(value);
}

CSEXPORT uint8 CSCONV Export_UUSharpGameplayTask_Internal_Get_Priority(UUSharpGameplayTask* instance)
{
	return instance->Internal_Get_Priority();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Set_Priority(UUSharpGameplayTask* instance, uint8 value)
{
	instance->Internal_Set_Priority(value);
}

CSEXPORT uint8 CSCONV Export_UUSharpGameplayTask_Internal_Get_ResourceOverlapPolicy(UUSharpGameplayTask* instance)
{
	return (uint8)instance->Internal_Get_Priority();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Set_ResourceOverlapPolicy(UUSharpGameplayTask* instance, uint8 value)
{
	instance->Internal_Set_ResourceOverlapPolicy((ETaskResourceOverlapPolicy)value);
}

CSEXPORT csbool CSCONV Export_UUSharpGameplayTask_Internal_Get_bTickingTask(UUSharpGameplayTask* instance)
{
	return instance->Internal_Get_bTickingTask();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Set_bTickingTask(UUSharpGameplayTask* instance, csbool value)
{
	instance->Internal_Set_bTickingTask(value);
}

CSEXPORT csbool CSCONV Export_UUSharpGameplayTask_Internal_Get_bSimulatedTask(UUSharpGameplayTask* instance)
{
	return instance->Internal_Get_bSimulatedTask();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Set_bSimulatedTask(UUSharpGameplayTask* instance, csbool value)
{
	instance->Internal_Set_bSimulatedTask(value);
}

CSEXPORT csbool CSCONV Export_UUSharpGameplayTask_Internal_Get_bIsSimulating(UUSharpGameplayTask* instance)
{
	return instance->Internal_Get_bIsSimulating();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Set_bIsSimulating(UUSharpGameplayTask* instance, csbool value)
{
	instance->Internal_Set_bIsSimulating(value);
}

CSEXPORT csbool CSCONV Export_UUSharpGameplayTask_Internal_Get_bIsPausable(UUSharpGameplayTask* instance)
{
	return instance->Internal_Get_bIsPausable();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Set_bIsPausable(UUSharpGameplayTask* instance, csbool value)
{
	instance->Internal_Set_bIsPausable(value);
}

CSEXPORT csbool CSCONV Export_UUSharpGameplayTask_Internal_Get_bCaresAboutPriority(UUSharpGameplayTask* instance)
{
	return instance->Internal_Get_bCaresAboutPriority();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Set_bCaresAboutPriority(UUSharpGameplayTask* instance, csbool value)
{
	instance->Internal_Set_bCaresAboutPriority(value);
}

CSEXPORT csbool CSCONV Export_UUSharpGameplayTask_Internal_Get_bOwnedByTasksComponent(UUSharpGameplayTask* instance)
{
	return instance->Internal_Get_bOwnedByTasksComponent();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Set_bOwnedByTasksComponent(UUSharpGameplayTask* instance, csbool value)
{
	instance->Internal_Set_bOwnedByTasksComponent(value);
}

CSEXPORT csbool CSCONV Export_UUSharpGameplayTask_Internal_Get_bClaimRequiredResources(UUSharpGameplayTask* instance)
{
	return instance->Internal_Get_bClaimRequiredResources();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Set_bClaimRequiredResources(UUSharpGameplayTask* instance, csbool value)
{
	instance->Internal_Set_bClaimRequiredResources(value);
}

CSEXPORT csbool CSCONV Export_UUSharpGameplayTask_Internal_Get_bOwnerFinished(UUSharpGameplayTask* instance)
{
	return instance->Internal_Get_bOwnerFinished();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Set_bOwnerFinished(UUSharpGameplayTask* instance, csbool value)
{
	instance->Internal_Set_bOwnerFinished(value);
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Get_RequiredResources(UUSharpGameplayTask* instance, FGameplayResourceSet& result)
{
	result = instance->Internal_Get_RequiredResources();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Set_RequiredResources(UUSharpGameplayTask* instance, const FGameplayResourceSet& value)
{
	instance->Internal_Set_RequiredResources(value);
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Get_ClaimedResources(UUSharpGameplayTask* instance, FGameplayResourceSet& result)
{
	result = instance->Internal_Get_ClaimedResources();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_Set_ClaimedResources(UUSharpGameplayTask* instance, const FGameplayResourceSet& value)
{
	instance->Internal_Set_ClaimedResources(value);
}

CSEXPORT UObject* CSCONV Export_UUSharpGameplayTask_Internal_Get_TaskOwner(UUSharpGameplayTask* instance)
{
	return Cast<UObject>(instance->Internal_Get_TaskOwner());
}

CSEXPORT UGameplayTasksComponent* CSCONV Export_UUSharpGameplayTask_Internal_Get_TasksComponent(UUSharpGameplayTask* instance)
{
	return instance->Internal_Get_TasksComponent();
}

CSEXPORT UGameplayTask* CSCONV Export_UUSharpGameplayTask_Internal_Get_ChildTask(UUSharpGameplayTask* instance)
{
	return instance->Internal_Get_ChildTask();
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask_Internal_InitTask(UUSharpGameplayTask* instance, UObject* InTaskOwner, uint8 InPriority)
{
	instance->Internal_InitTask(*Cast<IGameplayTaskOwnerInterface>(InTaskOwner), InPriority);
}

CSEXPORT UObject* CSCONV Export_UUSharpGameplayTask_Internal_ConvertToTaskOwner(UObject& OwnerObject)
{
	return Cast<UObject>(UUSharpGameplayTask::Internal_ConvertToTaskOwner(OwnerObject));
}

CSEXPORT UObject* CSCONV Export_UUSharpGameplayTask_Internal_ConvertToTaskOwnerActor(AActor& OwnerActor)
{
	return Cast<UObject>(UUSharpGameplayTask::Internal_ConvertToTaskOwnerActor(OwnerActor));
}

CSEXPORT void CSCONV Export_UUSharpGameplayTask(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UUSharpGameplayTask_Set_Callback);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Base_Activate);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Base_InitSimulatedTask);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Base_ExternalConfirm);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Base_ExternalCancel);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Base_GetDebugString);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Base_OnDestroy);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Base_Pause);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Base_Resume);
#if !(UE_BUILD_SHIPPING || UE_BUILD_TEST)
	REGISTER_FUNC(Export_UUSharpGameplayTask_Base_GenerateDebugDescription);
#endif
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_InstanceName);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Set_InstanceName);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_Priority);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Set_Priority);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_ResourceOverlapPolicy);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Set_ResourceOverlapPolicy);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_bTickingTask);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Set_bTickingTask);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_bSimulatedTask);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Set_bSimulatedTask);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_bIsSimulating);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Set_bIsSimulating);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_bIsPausable);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Set_bIsPausable);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_bCaresAboutPriority);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Set_bCaresAboutPriority);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_bOwnedByTasksComponent);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Set_bOwnedByTasksComponent);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_bClaimRequiredResources);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Set_bClaimRequiredResources);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_bOwnerFinished);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Set_bOwnerFinished);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_RequiredResources);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Set_RequiredResources);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_ClaimedResources);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Set_ClaimedResources);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_TaskOwner);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_TasksComponent);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_Get_ChildTask);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_InitTask);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_ConvertToTaskOwner);
	REGISTER_FUNC(Export_UUSharpGameplayTask_Internal_ConvertToTaskOwnerActor);
}
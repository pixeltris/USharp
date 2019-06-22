CSEXPORT UGameplayTasksComponent* CSCONV Export_IGameplayTaskOwnerInterface_GetGameplayTasksComponent(UObject* instance, const UGameplayTask& Task)
{
	return Cast<IGameplayTaskOwnerInterface>(instance)->GetGameplayTasksComponent(Task);
}

CSEXPORT AActor* CSCONV Export_IGameplayTaskOwnerInterface_GetGameplayTaskOwner(UObject* instance, const UGameplayTask* Task)
{
	return Cast<IGameplayTaskOwnerInterface>(instance)->GetGameplayTaskOwner(Task);
}

CSEXPORT AActor* CSCONV Export_IGameplayTaskOwnerInterface_GetGameplayTaskAvatar(UObject* instance, const UGameplayTask* Task)
{
	return Cast<IGameplayTaskOwnerInterface>(instance)->GetGameplayTaskAvatar(Task);
}

CSEXPORT uint8 CSCONV Export_IGameplayTaskOwnerInterface_GetGameplayTaskDefaultPriority(UObject* instance)
{
	return Cast<IGameplayTaskOwnerInterface>(instance)->GetGameplayTaskDefaultPriority();
}

CSEXPORT void CSCONV Export_IGameplayTaskOwnerInterface_OnGameplayTaskInitialized(UObject* instance, UGameplayTask& Task)
{
	Cast<IGameplayTaskOwnerInterface>(instance)->OnGameplayTaskInitialized(Task);
}

CSEXPORT void CSCONV Export_IGameplayTaskOwnerInterface_OnGameplayTaskDeactivated(UObject* instance, UGameplayTask& Task)
{
	Cast<IGameplayTaskOwnerInterface>(instance)->OnGameplayTaskDeactivated(Task);
}

CSEXPORT void CSCONV Export_IGameplayTaskOwnerInterface(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_IGameplayTaskOwnerInterface_GetGameplayTasksComponent);
	REGISTER_FUNC(Export_IGameplayTaskOwnerInterface_GetGameplayTaskOwner);
	REGISTER_FUNC(Export_IGameplayTaskOwnerInterface_GetGameplayTaskAvatar);
	REGISTER_FUNC(Export_IGameplayTaskOwnerInterface_GetGameplayTaskDefaultPriority);
	REGISTER_FUNC(Export_IGameplayTaskOwnerInterface_OnGameplayTaskInitialized);
	REGISTER_FUNC(Export_IGameplayTaskOwnerInterface_OnGameplayTaskDeactivated);
}
CSEXPORT UWorld* CSCONV Export_UWorld_Get_GWorld()
{
	return GWorld;
}

CSEXPORT EWorldType::Type CSCONV Export_UWorld_Get_WorldType(UWorld* instance)
{
	return instance->WorldType;
}

CSEXPORT const TArray<ULevel*>& CSCONV Export_UWorld_GetLevels(UWorld* instance)
{
	return instance->GetLevels();
}

CSEXPORT UGameInstance* CSCONV Export_UWorld_GetGameInstance(UWorld* instance)
{
	return instance->GetGameInstance();
}

CSEXPORT FTimerManager& CSCONV Export_UWorld_GetTimerManager(UWorld* instance)
{
	return instance->GetTimerManager();
}

CSEXPORT AActor* CSCONV Export_UWorld_SpawnActor(UWorld* instance, UClass* Class, FVector const& Location, FRotator const& Rotation, const FActorSpawnParameters& Params)
{
	return instance->SpawnActor(Class, &Location, &Rotation, Params);
}

CSEXPORT void CSCONV Export_UWorld(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UWorld_Get_GWorld);
	REGISTER_FUNC(Export_UWorld_Get_WorldType);
	REGISTER_FUNC(Export_UWorld_GetLevels);
	REGISTER_FUNC(Export_UWorld_GetGameInstance);
	REGISTER_FUNC(Export_UWorld_GetTimerManager);
	REGISTER_FUNC(Export_UWorld_SpawnActor);
}
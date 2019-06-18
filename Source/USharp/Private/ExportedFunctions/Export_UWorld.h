CSEXPORT int32 CSCONV Export_UWorld_Offset_TimeSeconds()
{
	return OffsetOf(&UWorld::TimeSeconds);
}

CSEXPORT int32 CSCONV Export_UWorld_Offset_UnpausedTimeSeconds()
{
	return OffsetOf(&UWorld::UnpausedTimeSeconds);
}

CSEXPORT int32 CSCONV Export_UWorld_Offset_RealTimeSeconds()
{
	return OffsetOf(&UWorld::RealTimeSeconds);
}

CSEXPORT int32 CSCONV Export_UWorld_Offset_DeltaTimeSeconds()
{
	return OffsetOf(&UWorld::DeltaTimeSeconds);
}

CSEXPORT int32 CSCONV Export_UWorld_Offset_PauseDelay()
{
	return OffsetOf(&UWorld::PauseDelay);
}

CSEXPORT csbool CSCONV Export_UWorld_Get_bDebugPauseExecution(UWorld* instance)
{
	return instance->bDebugPauseExecution;
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

CSEXPORT csbool CSCONV Export_UWorld_IsPaused(UWorld* instance)
{
	return instance->IsPaused();
}

CSEXPORT AActor* CSCONV Export_UWorld_SpawnActor(UWorld* instance, UClass* Class, FVector const& Location, FRotator const& Rotation, const FActorSpawnParameters& Params)
{
	return instance->SpawnActor(Class, &Location, &Rotation, Params);
}

CSEXPORT APlayerController* CSCONV Export_UWorld_GetFirstPlayerController(UWorld* instance)
{
	return instance->GetFirstPlayerController();
}

CSEXPORT FLatentActionManager& CSCONV Export_UWorld_GetLatentActionManager(UWorld* instance)
{
	return instance->GetLatentActionManager();
}

CSEXPORT void CSCONV Export_UWorld(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UWorld_Offset_TimeSeconds);
	REGISTER_FUNC(Export_UWorld_Offset_UnpausedTimeSeconds);
	REGISTER_FUNC(Export_UWorld_Offset_RealTimeSeconds);
	REGISTER_FUNC(Export_UWorld_Offset_DeltaTimeSeconds);
	REGISTER_FUNC(Export_UWorld_Offset_PauseDelay);
	REGISTER_FUNC(Export_UWorld_Get_bDebugPauseExecution);
	REGISTER_FUNC(Export_UWorld_Get_WorldType);
	REGISTER_FUNC(Export_UWorld_GetLevels);
	REGISTER_FUNC(Export_UWorld_GetGameInstance);
	REGISTER_FUNC(Export_UWorld_GetTimerManager);
	REGISTER_FUNC(Export_UWorld_IsPaused);
	REGISTER_FUNC(Export_UWorld_SpawnActor);
	REGISTER_FUNC(Export_UWorld_GetFirstPlayerController);
	REGISTER_FUNC(Export_UWorld_GetLatentActionManager);
}
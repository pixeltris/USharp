CSEXPORT void CSCONV Export_FTimerManager_Tick(FTimerManager* instance, float DeltaTime)
{
	instance->Tick(DeltaTime);
}

CSEXPORT void CSCONV Export_FTimerManager_SetTimer(FTimerManager* instance, FTimerHandle& InOutHandle, FTimerDynamicDelegate const& InDynDelegate, float InRate, bool InbLoop, float InFirstDelay)
{
	instance->SetTimer(InOutHandle, InDynDelegate, InRate, InbLoop, InFirstDelay);
}

CSEXPORT void CSCONV Export_FTimerManager_SetTimerForNextTick(FTimerManager* instance, FTimerDynamicDelegate const& InDynDelegate)
{
	instance->SetTimerForNextTick(InDynDelegate);
}

CSEXPORT void CSCONV Export_FTimerManager_ClearTimer(FTimerManager* instance, FTimerHandle& InHandle)
{
	instance->ClearTimer(InHandle);
}

CSEXPORT void CSCONV Export_FTimerManager_ClearAllTimersForObject(FTimerManager* instance, void const* Object)
{
	instance->ClearAllTimersForObject(Object);
}

CSEXPORT void CSCONV Export_FTimerManager_PauseTimer(FTimerManager* instance, const FTimerHandle& InHandle)
{
	instance->PauseTimer(InHandle);
}

CSEXPORT void CSCONV Export_FTimerManager_UnPauseTimer(FTimerManager* instance, const FTimerHandle& InHandle)
{
	instance->UnPauseTimer(InHandle);
}

CSEXPORT float CSCONV Export_FTimerManager_GetTimerRate(FTimerManager* instance, const FTimerHandle& InHandle)
{
	return instance->GetTimerRate(InHandle);
}

CSEXPORT csbool CSCONV Export_FTimerManager_IsTimerActive(FTimerManager* instance, const FTimerHandle& InHandle)
{
	return instance->IsTimerActive(InHandle);
}

CSEXPORT csbool CSCONV Export_FTimerManager_IsTimerPaused(FTimerManager* instance, const FTimerHandle& InHandle)
{
	return instance->IsTimerPaused(InHandle);
}

CSEXPORT csbool CSCONV Export_FTimerManager_IsTimerPending(FTimerManager* instance, const FTimerHandle& InHandle)
{
	return instance->IsTimerPending(InHandle);
}

CSEXPORT csbool CSCONV Export_FTimerManager_TimerExists(FTimerManager* instance, const FTimerHandle& InHandle)
{
	return instance->TimerExists(InHandle);
}

CSEXPORT float CSCONV Export_FTimerManager_GetTimerElapsed(FTimerManager* instance, const FTimerHandle& InHandle)
{
	return instance->GetTimerElapsed(InHandle);
}

CSEXPORT float CSCONV Export_FTimerManager_GetTimerRemaining(FTimerManager* instance, const FTimerHandle& InHandle)
{
	return instance->GetTimerRemaining(InHandle);
}

CSEXPORT csbool CSCONV Export_FTimerManager_HasBeenTickedThisFrame(FTimerManager* instance)
{
	return instance->HasBeenTickedThisFrame();
}

CSEXPORT void CSCONV Export_FTimerManager_K2_FindDynamicTimerHandle(FTimerManager* instance, const FTimerDynamicDelegate& InDynamicDelegate, FTimerHandle& result)
{
	result = instance->K2_FindDynamicTimerHandle(InDynamicDelegate);
}

CSEXPORT void CSCONV Export_FTimerManager_ListTimers(FTimerManager* instance)
{
	instance->ListTimers();
}

CSEXPORT void CSCONV Export_FTimerManager_SetGameInstance(FTimerManager* instance, UGameInstance* InGameInstance)
{
	instance->SetGameInstance(InGameInstance);
}

CSEXPORT void CSCONV Export_FTimerManager(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FTimerManager_Tick);
	REGISTER_FUNC(Export_FTimerManager_SetTimer);
	REGISTER_FUNC(Export_FTimerManager_SetTimerForNextTick);
	REGISTER_FUNC(Export_FTimerManager_ClearTimer);
	REGISTER_FUNC(Export_FTimerManager_ClearAllTimersForObject);
	REGISTER_FUNC(Export_FTimerManager_PauseTimer);
	REGISTER_FUNC(Export_FTimerManager_UnPauseTimer);
	REGISTER_FUNC(Export_FTimerManager_GetTimerRate);
	REGISTER_FUNC(Export_FTimerManager_IsTimerActive);
	REGISTER_FUNC(Export_FTimerManager_IsTimerPaused);
	REGISTER_FUNC(Export_FTimerManager_IsTimerPending);
	REGISTER_FUNC(Export_FTimerManager_TimerExists);
	REGISTER_FUNC(Export_FTimerManager_GetTimerElapsed);
	REGISTER_FUNC(Export_FTimerManager_GetTimerRemaining);
	REGISTER_FUNC(Export_FTimerManager_HasBeenTickedThisFrame);
	REGISTER_FUNC(Export_FTimerManager_K2_FindDynamicTimerHandle);
	REGISTER_FUNC(Export_FTimerManager_ListTimers);
	REGISTER_FUNC(Export_FTimerManager_SetGameInstance);
}
CSEXPORT uint8 CSCONV Export_FWorldContext_Get_WorldType(FWorldContext* instance)
{
	return instance->WorldType;
}

CSEXPORT void CSCONV Export_FWorldContext_Get_ContextHandle(FWorldContext* instance, FName& result)
{
	result = instance->ContextHandle;
}

CSEXPORT void CSCONV Export_FWorldContext_Get_TravelURL(FWorldContext* instance, FString& result)
{
	result = instance->TravelURL;
}

CSEXPORT uint8 CSCONV Export_FWorldContext_Get_TravelType(FWorldContext* instance)
{
	return instance->TravelType;
}

CSEXPORT class UGameViewportClient* CSCONV Export_FWorldContext_Get_GameViewport(FWorldContext* instance)
{
	return instance->GameViewport;
}

CSEXPORT class UGameInstance* CSCONV Export_FWorldContext_Get_OwningGameInstance(FWorldContext* instance)
{
	return instance->OwningGameInstance;
}

CSEXPORT int32 CSCONV Export_FWorldContext_Get_PIEInstance(FWorldContext* instance)
{
	return instance->PIEInstance;
}

CSEXPORT void CSCONV Export_FWorldContext_Get_PIEPrefix(FWorldContext* instance, FString& result)
{
	result = instance->PIEPrefix;
}

CSEXPORT csbool CSCONV Export_FWorldContext_Get_RunAsDedicated(FWorldContext* instance)
{
	return instance->RunAsDedicated;
}

CSEXPORT csbool CSCONV Export_FWorldContext_Get_bWaitingOnOnlineSubsystem(FWorldContext* instance)
{
	return instance->bWaitingOnOnlineSubsystem;
}

CSEXPORT uint32 CSCONV Export_FWorldContext_Get_AudioDeviceHandle(FWorldContext* instance)
{
	return instance->AudioDeviceHandle;
}

CSEXPORT void CSCONV Export_FWorldContext_SetCurrentWorld(FWorldContext* instance, UWorld *World)
{
	instance->SetCurrentWorld(World);
}

CSEXPORT UWorld* CSCONV Export_FWorldContext_World(FWorldContext* instance)
{
	return instance->World();
}

CSEXPORT void CSCONV Export_FWorldContext(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FWorldContext_Get_WorldType);
	REGISTER_FUNC(Export_FWorldContext_Get_ContextHandle);
	REGISTER_FUNC(Export_FWorldContext_Get_TravelURL);
	REGISTER_FUNC(Export_FWorldContext_Get_TravelType);
	REGISTER_FUNC(Export_FWorldContext_Get_GameViewport);
	REGISTER_FUNC(Export_FWorldContext_Get_OwningGameInstance);
	REGISTER_FUNC(Export_FWorldContext_Get_PIEInstance);
	REGISTER_FUNC(Export_FWorldContext_Get_PIEPrefix);
	REGISTER_FUNC(Export_FWorldContext_Get_RunAsDedicated);
	REGISTER_FUNC(Export_FWorldContext_Get_bWaitingOnOnlineSubsystem);
	REGISTER_FUNC(Export_FWorldContext_Get_AudioDeviceHandle);
	REGISTER_FUNC(Export_FWorldContext_SetCurrentWorld);
	REGISTER_FUNC(Export_FWorldContext_World);
}
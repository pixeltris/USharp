CSEXPORT void CSCONV Export_UEngine_CopyPropertiesForUnrelatedObjects(UObject* OldObject, UObject* NewObject, const FCopyPropertiesForUnrelatedObjectsParamsInterop& Params)
{
	UEngine::CopyPropertiesForUnrelatedObjects(OldObject, NewObject, FCopyPropertiesForUnrelatedObjectsParamsInterop::ToNative(Params));
}

CSEXPORT UWorld* CSCONV Export_UEngine_GetWorldFromContextObject(UObject* Object, EGetWorldErrorMode ErrorMode)
{
	return GEngine->GetWorldFromContextObject(Object, ErrorMode);
}

CSEXPORT FWorldContext* CSCONV Export_UEngine_GetWorldContextFromWorld(const UWorld* InWorld)
{
	return GEngine->GetWorldContextFromWorld(InWorld);
}

CSEXPORT FWorldContext* CSCONV Export_UEngine_GetWorldContextFromGameViewport(const UGameViewportClient *InViewport)
{
	return GEngine->GetWorldContextFromGameViewport(InViewport);
}

CSEXPORT FWorldContext* CSCONV Export_UEngine_GetWorldContextFromPendingNetGame(const UPendingNetGame *InPendingNetGame)
{
	return GEngine->GetWorldContextFromPendingNetGame(InPendingNetGame);
}

CSEXPORT FWorldContext* CSCONV Export_UEngine_GetWorldContextFromPendingNetGameNetDriver(const UNetDriver *InPendingNetGame)
{
	return GEngine->GetWorldContextFromPendingNetGameNetDriver(InPendingNetGame);
}

CSEXPORT FWorldContext* CSCONV Export_UEngine_GetWorldContextFromHandle(const FName& WorldContextHandle)
{
	return GEngine->GetWorldContextFromHandle(WorldContextHandle);
}

CSEXPORT FWorldContext* CSCONV Export_UEngine_GetWorldContextFromPIEInstance(const int32 PIEInstance)
{
	return GEngine->GetWorldContextFromPIEInstance(PIEInstance);
}

CSEXPORT FWorldContext& CSCONV Export_UEngine_GetWorldContextFromWorldChecked(const UWorld* InWorld)
{
	return GEngine->GetWorldContextFromWorldChecked(InWorld);
}

CSEXPORT FWorldContext& CSCONV Export_UEngine_GetWorldContextFromGameViewportChecked(const UGameViewportClient *InViewport)
{
	return GEngine->GetWorldContextFromGameViewportChecked(InViewport);
}

CSEXPORT FWorldContext& CSCONV Export_UEngine_GetWorldContextFromPendingNetGameChecked(const UPendingNetGame *InPendingNetGame)
{
	return GEngine->GetWorldContextFromPendingNetGameChecked(InPendingNetGame);
}

CSEXPORT FWorldContext& CSCONV Export_UEngine_GetWorldContextFromPendingNetGameNetDriverChecked(const UNetDriver *InPendingNetGame)
{
	return GEngine->GetWorldContextFromPendingNetGameNetDriverChecked(InPendingNetGame);
}

CSEXPORT FWorldContext& CSCONV Export_UEngine_GetWorldContextFromHandleChecked(const FName& WorldContextHandle)
{
	return GEngine->GetWorldContextFromHandleChecked(WorldContextHandle);
}

CSEXPORT FWorldContext& CSCONV Export_UEngine_GetWorldContextFromPIEInstanceChecked(const int32 PIEInstance)
{
	return GEngine->GetWorldContextFromPIEInstanceChecked(PIEInstance);
}

CSEXPORT void CSCONV Export_UEngine_GetWorldContexts(TArray<const FWorldContext*>& result)
{
	// Unsure if TIndirectArray<> maps directly to TArray<> so recreating it as a TArray<> for now
	for (const FWorldContext& Context : GEngine->GetWorldContexts())
	{
		result.Add(&Context);
	}
}

CSEXPORT void CSCONV Export_UEngine(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UEngine_CopyPropertiesForUnrelatedObjects);
	REGISTER_FUNC(Export_UEngine_GetWorldFromContextObject);
	REGISTER_FUNC(Export_UEngine_GetWorldContextFromWorld);
	REGISTER_FUNC(Export_UEngine_GetWorldContextFromGameViewport);
	REGISTER_FUNC(Export_UEngine_GetWorldContextFromPendingNetGame);
	REGISTER_FUNC(Export_UEngine_GetWorldContextFromPendingNetGameNetDriver);
	REGISTER_FUNC(Export_UEngine_GetWorldContextFromHandle);
	REGISTER_FUNC(Export_UEngine_GetWorldContextFromPIEInstance);
	REGISTER_FUNC(Export_UEngine_GetWorldContextFromWorldChecked);
	REGISTER_FUNC(Export_UEngine_GetWorldContextFromGameViewportChecked);
	REGISTER_FUNC(Export_UEngine_GetWorldContextFromPendingNetGameChecked);
	REGISTER_FUNC(Export_UEngine_GetWorldContextFromPendingNetGameNetDriverChecked);
	REGISTER_FUNC(Export_UEngine_GetWorldContextFromHandleChecked);
	REGISTER_FUNC(Export_UEngine_GetWorldContextFromPIEInstanceChecked);
	REGISTER_FUNC(Export_UEngine_GetWorldContexts);
}
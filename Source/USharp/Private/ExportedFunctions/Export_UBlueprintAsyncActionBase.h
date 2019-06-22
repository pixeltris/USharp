CSEXPORT void CSCONV Export_UBlueprintAsyncActionBase_RegisterWithGameInstanceWorldContext(UBlueprintAsyncActionBase* instance, UObject* WorldContextObject)
{
	instance->RegisterWithGameInstance(WorldContextObject);
}

CSEXPORT void CSCONV Export_UBlueprintAsyncActionBase_RegisterWithGameInstance(UBlueprintAsyncActionBase* instance, UGameInstance* GameInstance)
{
	instance->RegisterWithGameInstance(GameInstance);
}

CSEXPORT void CSCONV Export_UBlueprintAsyncActionBase_SetReadyToDestroy(UBlueprintAsyncActionBase* instance)
{
	instance->SetReadyToDestroy();
}

CSEXPORT void CSCONV Export_UBlueprintAsyncActionBase(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UBlueprintAsyncActionBase_RegisterWithGameInstanceWorldContext);
	REGISTER_FUNC(Export_UBlueprintAsyncActionBase_RegisterWithGameInstance);
	REGISTER_FUNC(Export_UBlueprintAsyncActionBase_SetReadyToDestroy);
}
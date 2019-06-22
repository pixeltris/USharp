CSEXPORT void CSCONV Export_UUSharpAsyncActionBase_Set_Callback(ManagedLatentCallbackDel Callback)
{
	UUSharpAsyncActionBaseCallback = Callback;
}

CSEXPORT UGameInstance* CSCONV Export_UUSharpAsyncActionBase_GetRegisteredWithGameInstance(UUSharpAsyncActionBase* instance)
{
	return instance->GetRegisteredWithGameInstance();
}

CSEXPORT void CSCONV Export_UUSharpAsyncActionBase_Base_Activate(UUSharpAsyncActionBase* instance)
{
	instance->UBlueprintAsyncActionBase::Activate();
}

CSEXPORT void CSCONV Export_UUSharpAsyncActionBase_Base_RegisterWithGameInstanceWorldContext(UUSharpAsyncActionBase* instance, UWorld* WorldContextObject)
{
	instance->UBlueprintAsyncActionBase::RegisterWithGameInstance(WorldContextObject);
}

CSEXPORT void CSCONV Export_UUSharpAsyncActionBase_Base_RegisterWithGameInstance(UUSharpAsyncActionBase* instance, UGameInstance* GameInstance)
{
	instance->UBlueprintAsyncActionBase::RegisterWithGameInstance(GameInstance);
}

CSEXPORT void CSCONV Export_UUSharpAsyncActionBase_Base_SetReadyToDestroy(UUSharpAsyncActionBase* instance)
{
	instance->UBlueprintAsyncActionBase::SetReadyToDestroy();
}

CSEXPORT void CSCONV Export_UUSharpAsyncActionBase(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UUSharpAsyncActionBase_Set_Callback);
	REGISTER_FUNC(Export_UUSharpAsyncActionBase_GetRegisteredWithGameInstance);
	REGISTER_FUNC(Export_UUSharpAsyncActionBase_Base_Activate);
	REGISTER_FUNC(Export_UUSharpAsyncActionBase_Base_RegisterWithGameInstanceWorldContext);
	REGISTER_FUNC(Export_UUSharpAsyncActionBase_Base_RegisterWithGameInstance);
	REGISTER_FUNC(Export_UUSharpAsyncActionBase_Base_SetReadyToDestroy);
}
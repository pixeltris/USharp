CSEXPORT FTimerManager& CSCONV Export_UGameInstance_GetTimerManager(UGameInstance* instance)
{
	return instance->GetTimerManager();
}

CSEXPORT void CSCONV Export_UGameInstance(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UGameInstance_GetTimerManager);
}
CSEXPORT UGameInstance* CSCONV Export_UGameInstanceSubsystem_GetGameInstance(UGameInstanceSubsystem* instance)
{
	return instance->GetGameInstance();
}

CSEXPORT void CSCONV Export_UGameInstanceSubsystem(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UGameInstanceSubsystem_GetGameInstance);
}
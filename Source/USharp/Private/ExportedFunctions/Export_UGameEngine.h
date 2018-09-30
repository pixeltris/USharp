CSEXPORT UGameInstance* CSCONV Export_UGameEngine_Get_GameInstance(UGameEngine* instance)
{
	return instance->GameInstance;
}

CSEXPORT void CSCONV Export_UGameEngine(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UGameEngine_Get_GameInstance);
}
CSEXPORT ULocalPlayer* CSCONV Export_ULocalPlayerSubsystem_GetLocalPlayer(ULocalPlayerSubsystem* instance)
{
	return instance->GetLocalPlayer();
}

CSEXPORT void CSCONV Export_ULocalPlayerSubsystem(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_ULocalPlayerSubsystem_GetLocalPlayer);
}
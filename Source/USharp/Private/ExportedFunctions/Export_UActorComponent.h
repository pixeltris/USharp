CSEXPORT void CSCONV Export_UActorComponent_RegisterComponent(UActorComponent* instance)
{
	instance->RegisterComponent();
}

CSEXPORT void CSCONV Export_UActorComponent(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UActorComponent_RegisterComponent);
}
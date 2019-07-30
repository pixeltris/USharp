CSEXPORT void CSCONV Export_UActorComponent_RegisterComponent(UActorComponent* instance)
{
	instance->RegisterComponent();
}

CSEXPORT void CSCONV Export_UActorComponent_ReregisterComponent(UActorComponent* instance)
{
	instance->ReregisterComponent();
}

CSEXPORT void CSCONV Export_UActorComponent_UnregisterComponent(UActorComponent* instance)
{
	instance->UnregisterComponent();
}

CSEXPORT UWorld* CSCONV Export_UActorComponent_GetWorld(UActorComponent* instance)
{
	return instance->GetWorld();
}

CSEXPORT void CSCONV Export_UActorComponent(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UActorComponent_RegisterComponent);
	REGISTER_FUNC(Export_UActorComponent_ReregisterComponent);
	REGISTER_FUNC(Export_UActorComponent_UnregisterComponent);
	REGISTER_FUNC(Export_UActorComponent_GetWorld);
}
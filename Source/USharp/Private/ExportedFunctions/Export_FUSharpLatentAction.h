CSEXPORT void CSCONV Export_FUSharpLatentAction_Set_bManagedObjectDestroyed(FUSharpLatentAction* instance, csbool value)
{
	instance->bManagedObjectDestroyed = (bool)value;
}

CSEXPORT void CSCONV Export_FUSharpLatentAction(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FUSharpLatentAction_Set_bManagedObjectDestroyed);
}
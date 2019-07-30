CSEXPORT FRotator CSCONV Export_APlayerController_Get_RotationInput(APlayerController* instance)
{
	return instance->RotationInput;
}

CSEXPORT void CSCONV Export_APlayerController_Set_RotationInput(APlayerController* instance, const FRotator& RotationInput)
{
	instance->RotationInput = RotationInput;
}

CSEXPORT void CSCONV Export_APlayerController(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_APlayerController_Get_RotationInput);
	REGISTER_FUNC(Export_APlayerController_Set_RotationInput);
}
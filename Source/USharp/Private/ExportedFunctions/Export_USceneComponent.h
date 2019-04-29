CSEXPORT void CSCONV Export_USceneComponent_SetupAttachment(USceneComponent* instance, USceneComponent* InParent, FName& InSocketName)
{
	instance->SetupAttachment(InParent, InSocketName);
}

CSEXPORT void CSCONV Export_USceneComponent(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_USceneComponent_SetupAttachment);
}
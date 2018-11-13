CSEXPORT csbool CSCONV Export_FInputBinding_Get_bConsumeInput(FInputBinding* instance)
{
	return instance->bConsumeInput;
}

CSEXPORT void CSCONV Export_FInputBinding_Set_bConsumeInput(FInputBinding* instance, csbool value)
{
	instance->bConsumeInput = value;
}

CSEXPORT csbool CSCONV Export_FInputBinding_Get_bExecuteWhenPaused(FInputBinding* instance)
{
	return instance->bExecuteWhenPaused;
}

CSEXPORT void CSCONV Export_FInputBinding_Set_bExecuteWhenPaused(FInputBinding* instance, csbool value)
{
	instance->bExecuteWhenPaused = value;
}

CSEXPORT void CSCONV Export_FInputBinding(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FInputBinding_Get_bConsumeInput);
	REGISTER_FUNC(Export_FInputBinding_Set_bConsumeInput);
	REGISTER_FUNC(Export_FInputBinding_Get_bExecuteWhenPaused);
	REGISTER_FUNC(Export_FInputBinding_Set_bExecuteWhenPaused);
}
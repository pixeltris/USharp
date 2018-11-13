CSEXPORT uint8 CSCONV Export_FInputTouchBinding_Get_KeyEvent(FInputTouchBinding* instance)
{
	return (uint8)instance->KeyEvent;
}

CSEXPORT void CSCONV Export_FInputTouchBinding_Set_KeyEvent(FInputTouchBinding* instance, uint8 value)
{
	instance->KeyEvent = (TEnumAsByte<EInputEvent>)value;
}

CSEXPORT FInputTouchUnifiedDelegate& CSCONV Export_FInputTouchBinding_Get_TouchDelegate(FInputTouchBinding* instance)
{
	return instance->TouchDelegate;
}

CSEXPORT void CSCONV Export_FInputTouchBinding(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FInputTouchBinding_Get_KeyEvent);
	REGISTER_FUNC(Export_FInputTouchBinding_Set_KeyEvent);
	REGISTER_FUNC(Export_FInputTouchBinding_Get_TouchDelegate);
}
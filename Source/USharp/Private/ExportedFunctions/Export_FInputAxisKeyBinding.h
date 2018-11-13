CSEXPORT float CSCONV Export_FInputAxisKeyBinding_Get_AxisValue(FInputAxisKeyBinding* instance)
{
	return instance->AxisValue;
}

CSEXPORT void CSCONV Export_FInputAxisKeyBinding_Set_AxisValue(FInputAxisKeyBinding* instance, float value)
{
	instance->AxisValue = value;
}

CSEXPORT void CSCONV Export_FInputAxisKeyBinding_Get_AxisKey(FInputAxisKeyBinding* instance, FKey& result)
{
	result = instance->AxisKey;
}

CSEXPORT void CSCONV Export_FInputAxisKeyBinding_Set_AxisKey(FInputAxisKeyBinding* instance, const FKey& value)
{
	instance->AxisKey = value;
}

CSEXPORT FInputAxisUnifiedDelegate& CSCONV Export_FInputAxisKeyBinding_Get_AxisDelegate(FInputAxisKeyBinding* instance)
{
	return instance->AxisDelegate;
}

CSEXPORT void CSCONV Export_FInputAxisKeyBinding(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FInputAxisKeyBinding_Get_AxisValue);
	REGISTER_FUNC(Export_FInputAxisKeyBinding_Set_AxisValue);
	REGISTER_FUNC(Export_FInputAxisKeyBinding_Get_AxisKey);
	REGISTER_FUNC(Export_FInputAxisKeyBinding_Set_AxisKey);
	REGISTER_FUNC(Export_FInputAxisKeyBinding_Get_AxisDelegate);
}
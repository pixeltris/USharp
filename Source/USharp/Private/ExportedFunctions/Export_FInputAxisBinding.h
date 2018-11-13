CSEXPORT void CSCONV Export_FInputAxisBinding_Get_AxisName(FInputAxisBinding* instance, FName& result)
{
	result = instance->AxisName;
}

CSEXPORT void CSCONV Export_FInputAxisBinding_Set_AxisName(FInputAxisBinding* instance, const FName& value)
{
	instance->AxisName = value;
}

CSEXPORT FInputAxisUnifiedDelegate& CSCONV Export_FInputAxisBinding_Get_AxisDelegate(FInputAxisBinding* instance)
{
	return instance->AxisDelegate;
}

CSEXPORT float CSCONV Export_FInputAxisBinding_Get_AxisValue(FInputAxisBinding* instance)
{
	return instance->AxisValue;
}

CSEXPORT void CSCONV Export_FInputAxisBinding_Set_AxisValue(FInputAxisBinding* instance, float value)
{
	instance->AxisValue = value;
}

CSEXPORT void CSCONV Export_FInputAxisBinding(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FInputAxisBinding_Get_AxisName);
	REGISTER_FUNC(Export_FInputAxisBinding_Set_AxisName);
	REGISTER_FUNC(Export_FInputAxisBinding_Get_AxisDelegate);
	REGISTER_FUNC(Export_FInputAxisBinding_Get_AxisValue);
	REGISTER_FUNC(Export_FInputAxisBinding_Set_AxisValue);
}
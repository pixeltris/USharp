CSEXPORT void CSCONV Export_FInputVectorAxisBinding_Get_AxisValue(FInputVectorAxisBinding* instance, FVector& result)
{
	result = instance->AxisValue;
}

CSEXPORT void CSCONV Export_FInputVectorAxisBinding_Set_AxisValue(FInputVectorAxisBinding* instance, const FVector& value)
{
	instance->AxisValue = value;
}

CSEXPORT void CSCONV Export_FInputVectorAxisBinding_Get_AxisKey(FInputVectorAxisBinding* instance, FKey& result)
{
	result = instance->AxisKey;
}

CSEXPORT void CSCONV Export_FInputVectorAxisBinding_Set_AxisKey(FInputVectorAxisBinding* instance, const FKey& value)
{
	instance->AxisKey = value;
}

CSEXPORT FInputVectorAxisUnifiedDelegate& CSCONV Export_FInputVectorAxisBinding_Get_AxisDelegate(FInputVectorAxisBinding* instance)
{
	return instance->AxisDelegate;
}

CSEXPORT void CSCONV Export_FInputVectorAxisBinding(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FInputVectorAxisBinding_Get_AxisValue);
	REGISTER_FUNC(Export_FInputVectorAxisBinding_Set_AxisValue);
	REGISTER_FUNC(Export_FInputVectorAxisBinding_Get_AxisKey);
	REGISTER_FUNC(Export_FInputVectorAxisBinding_Set_AxisKey);
	REGISTER_FUNC(Export_FInputVectorAxisBinding_Get_AxisDelegate);
}
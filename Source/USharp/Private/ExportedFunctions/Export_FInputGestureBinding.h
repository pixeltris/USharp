CSEXPORT float CSCONV Export_FInputGestureBinding_Get_GestureValue(FInputGestureBinding* instance)
{
	return instance->GestureValue;
}

CSEXPORT void CSCONV Export_FInputGestureBinding_Set_GestureValue(FInputGestureBinding* instance, float value)
{
	instance->GestureValue = value;
}

CSEXPORT void CSCONV Export_FInputGestureBinding_Get_GestureKey(FInputGestureBinding* instance, FKey& result)
{
	result =  instance->GestureKey;
}

CSEXPORT void CSCONV Export_FInputGestureBinding_Set_GestureKey(FInputGestureBinding* instance, const FKey& value)
{
	instance->GestureKey = value;
}

CSEXPORT FInputGestureUnifiedDelegate& CSCONV Export_FInputGestureBinding_Get_GestureDelegate(FInputGestureBinding* instance)
{
	return instance->GestureDelegate;
}

CSEXPORT void CSCONV Export_FInputGestureBinding(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FInputGestureBinding_Get_GestureValue);
	REGISTER_FUNC(Export_FInputGestureBinding_Set_GestureValue);
	REGISTER_FUNC(Export_FInputGestureBinding_Get_GestureKey);
	REGISTER_FUNC(Export_FInputGestureBinding_Set_GestureKey);
	REGISTER_FUNC(Export_FInputGestureBinding_Get_GestureDelegate);
}
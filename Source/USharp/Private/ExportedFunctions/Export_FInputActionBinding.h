CSEXPORT uint8 CSCONV Export_FInputActionBinding_Get_KeyEvent(FInputActionBinding* instance)
{
	return instance->KeyEvent;
}

CSEXPORT void CSCONV Export_FInputActionBinding_Set_KeyEvent(FInputActionBinding* instance, uint8 value)
{
	instance->KeyEvent = (TEnumAsByte<EInputEvent>)value;
}

CSEXPORT FInputActionUnifiedDelegate& CSCONV Export_FInputActionBinding_Get_ActionDelegate(FInputActionBinding* instance)
{
	return instance->ActionDelegate;
}

CSEXPORT void CSCONV Export_FInputActionBinding_GetActionName(FInputActionBinding* instance, FName& result)
{
	result = instance->GetActionName();
}

CSEXPORT csbool CSCONV Export_FInputActionBinding_IsPaired(FInputActionBinding* instance)
{
	return instance->IsPaired();
}

CSEXPORT void CSCONV Export_FInputActionBinding(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FInputActionBinding_Get_KeyEvent);
	REGISTER_FUNC(Export_FInputActionBinding_Set_KeyEvent);
	REGISTER_FUNC(Export_FInputActionBinding_Get_ActionDelegate);
	REGISTER_FUNC(Export_FInputActionBinding_GetActionName);
	REGISTER_FUNC(Export_FInputActionBinding_IsPaired);
}
CSEXPORT UFunction* CSCONV Export_UDelegateProperty_Get_SignatureFunction(UDelegateProperty* instance)
{
	return instance->SignatureFunction;
}

CSEXPORT void CSCONV Export_UDelegateProperty_Set_SignatureFunction(UDelegateProperty* instance, UFunction* value)
{
	instance->SignatureFunction = value;
}

CSEXPORT void CSCONV Export_UDelegateProperty(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UDelegateProperty_Get_SignatureFunction);
	REGISTER_FUNC(Export_UDelegateProperty_Set_SignatureFunction);
}
CSEXPORT UFunction* CSCONV Export_UMulticastDelegateProperty_Get_SignatureFunction(UMulticastDelegateProperty* instance)
{
	return instance->SignatureFunction;
}

CSEXPORT void CSCONV Export_UMulticastDelegateProperty_Set_SignatureFunction(UMulticastDelegateProperty* instance, UFunction* value)
{
	instance->SignatureFunction = value;
}

CSEXPORT void CSCONV Export_UMulticastDelegateProperty(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UMulticastDelegateProperty_Get_SignatureFunction);
	REGISTER_FUNC(Export_UMulticastDelegateProperty_Set_SignatureFunction);
}
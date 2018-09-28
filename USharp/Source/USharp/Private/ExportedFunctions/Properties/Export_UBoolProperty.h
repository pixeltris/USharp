CSEXPORT int CSCONV Export_UBoolProperty_GetBoolSize()
{
	return sizeof(bool);
}

CSEXPORT csbool CSCONV Export_UBoolProperty_GetPropertyValue(UBoolProperty* instance, void* Address)
{
	return instance->GetPropertyValue(Address);
}

CSEXPORT void CSCONV Export_UBoolProperty_SetPropertyValue(UBoolProperty* instance, void* Address, csbool Value)
{
	instance->SetPropertyValue(Address, !!Value);
}

CSEXPORT void CSCONV Export_UBoolProperty(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UBoolProperty_GetBoolSize);
	REGISTER_FUNC(Export_UBoolProperty_GetPropertyValue);
	REGISTER_FUNC(Export_UBoolProperty_SetPropertyValue);
}
CSEXPORT void CSCONV Export_UEnumProperty_SetEnum(UEnumProperty* instance, UEnum* InEnum)
{
	return instance->SetEnum(InEnum);
}

CSEXPORT UEnum* CSCONV Export_UEnumProperty_GetEnum(UEnumProperty* instance)
{
	return instance->GetEnum();
}

CSEXPORT UNumericProperty* CSCONV Export_UEnumProperty_GetUnderlyingProperty(UEnumProperty* instance)
{
	return instance->GetUnderlyingProperty();
}

CSEXPORT void CSCONV Export_UEnumProperty(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UEnumProperty_SetEnum);
	REGISTER_FUNC(Export_UEnumProperty_GetEnum);
	REGISTER_FUNC(Export_UEnumProperty_GetUnderlyingProperty);
}
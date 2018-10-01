CSEXPORT csbool CSCONV Export_UNumericProperty_IsFloatingPoint(UNumericProperty* instance)
{
	return instance->IsFloatingPoint();
}

CSEXPORT csbool CSCONV Export_UNumericProperty_IsInteger(UNumericProperty* instance)
{
	return instance->IsInteger();
}

CSEXPORT csbool CSCONV Export_UNumericProperty_IsEnum(UNumericProperty* instance)
{
	return instance->IsEnum();
}

CSEXPORT UEnum* CSCONV Export_UNumericProperty_GetIntPropertyEnum(UNumericProperty* instance)
{
	return instance->GetIntPropertyEnum();
}

CSEXPORT void CSCONV Export_UNumericProperty_SetIntPropertyValueUnsigned(UNumericProperty* instance, void* Data, uint64 Value)
{
	instance->SetIntPropertyValue(Data, Value);
}

CSEXPORT void CSCONV Export_UNumericProperty_SetIntPropertyValueSigned(UNumericProperty* instance, void* Data, int64 Value)
{
	instance->SetIntPropertyValue(Data, Value);
}

CSEXPORT void CSCONV Export_UNumericProperty_SetFloatingPointPropertyValue(UNumericProperty* instance, void* Data, double Value)
{
	instance->SetFloatingPointPropertyValue(Data, Value);
}

CSEXPORT void CSCONV Export_UNumericProperty_SetNumericPropertyValueFromString(UNumericProperty* instance, void* Data, const FString& Value)
{
	instance->SetNumericPropertyValueFromString(Data, *Value);
}

CSEXPORT int64 CSCONV Export_UNumericProperty_GetSignedIntPropertyValue(UNumericProperty* instance, void* Data)
{
	return instance->GetSignedIntPropertyValue(Data);
}

CSEXPORT uint64 CSCONV Export_UNumericProperty_GetUnsignedIntPropertyValue(UNumericProperty* instance, void* Data)
{
	return instance->GetUnsignedIntPropertyValue(Data);
}

CSEXPORT double CSCONV Export_UNumericProperty_GetFloatingPointPropertyValue(UNumericProperty* instance, void* Data)
{
	return instance->GetFloatingPointPropertyValue(Data);
}

CSEXPORT void CSCONV Export_UNumericProperty_GetNumericPropertyValueToString(UNumericProperty* instance, void* Data, FString& result)
{
	result = instance->GetNumericPropertyValueToString(Data);
}

CSEXPORT void CSCONV Export_UNumericProperty(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UNumericProperty_IsFloatingPoint);
	REGISTER_FUNC(Export_UNumericProperty_IsInteger);
	REGISTER_FUNC(Export_UNumericProperty_IsEnum);
	REGISTER_FUNC(Export_UNumericProperty_GetIntPropertyEnum);
	REGISTER_FUNC(Export_UNumericProperty_SetIntPropertyValueUnsigned);
	REGISTER_FUNC(Export_UNumericProperty_SetIntPropertyValueSigned);
	REGISTER_FUNC(Export_UNumericProperty_SetFloatingPointPropertyValue);
	REGISTER_FUNC(Export_UNumericProperty_SetNumericPropertyValueFromString);
	REGISTER_FUNC(Export_UNumericProperty_GetSignedIntPropertyValue);
	REGISTER_FUNC(Export_UNumericProperty_GetUnsignedIntPropertyValue);
	REGISTER_FUNC(Export_UNumericProperty_GetFloatingPointPropertyValue);
	REGISTER_FUNC(Export_UNumericProperty_GetNumericPropertyValueToString);
}
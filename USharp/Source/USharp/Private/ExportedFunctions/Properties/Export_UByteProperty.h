CSEXPORT UEnum* CSCONV Export_UByteProperty_Get_Enum(UByteProperty* instance)
{
	return instance->Enum;
}

CSEXPORT void CSCONV Export_UByteProperty_Set_Enum(UByteProperty* instance, UEnum* value)
{
	instance->Enum = value;
}

CSEXPORT void CSCONV Export_UByteProperty(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UByteProperty_Get_Enum);
	REGISTER_FUNC(Export_UByteProperty_Set_Enum);
}
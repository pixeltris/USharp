CSEXPORT UProperty* CSCONV Export_UArrayProperty_Get_Inner(UArrayProperty* instance)
{
	return instance->Inner;
}

CSEXPORT void CSCONV Export_UArrayProperty_Set_Inner(UArrayProperty* instance, UProperty* value)
{
	instance->Inner = value;
}
CSEXPORT void CSCONV Export_UArrayProperty(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UArrayProperty_Get_Inner);
	REGISTER_FUNC(Export_UArrayProperty_Set_Inner);
}
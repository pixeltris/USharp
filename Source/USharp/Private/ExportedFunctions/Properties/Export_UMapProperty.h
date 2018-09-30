CSEXPORT UProperty* CSCONV Export_UMapProperty_Get_KeyProp(UMapProperty* instance)
{
	return instance->KeyProp;
}

CSEXPORT void CSCONV Export_UMapProperty_Set_KeyProp(UMapProperty* instance, UProperty* value)
{
	instance->KeyProp = value;
}

CSEXPORT UProperty* CSCONV Export_UMapProperty_Get_ValueProp(UMapProperty* instance)
{
	return instance->ValueProp;
}

CSEXPORT void CSCONV Export_UMapProperty_Set_ValueProp(UMapProperty* instance, UProperty* value)
{
	instance->ValueProp = value;
}

CSEXPORT FScriptMapLayout CSCONV Export_UMapProperty_Get_MapLayout(UMapProperty* instance)
{
	return instance->MapLayout;
}

CSEXPORT void CSCONV Export_UMapProperty_Set_MapLayout(UMapProperty* instance, FScriptMapLayout value)
{
	instance->MapLayout = value;
}

CSEXPORT void CSCONV Export_UMapProperty(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UMapProperty_Get_KeyProp);
	REGISTER_FUNC(Export_UMapProperty_Set_KeyProp);
	REGISTER_FUNC(Export_UMapProperty_Get_ValueProp);
	REGISTER_FUNC(Export_UMapProperty_Set_ValueProp);
	REGISTER_FUNC(Export_UMapProperty_Get_MapLayout);
	REGISTER_FUNC(Export_UMapProperty_Set_MapLayout);
}
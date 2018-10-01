CSEXPORT UProperty* CSCONV Export_USetProperty_Get_ElementProp(USetProperty* instance)
{
	return instance->ElementProp;
}

CSEXPORT void CSCONV Export_USetProperty_Set_ElementProp(USetProperty* instance, UProperty* value)
{
	instance->ElementProp = value;
}

CSEXPORT FScriptSetLayout CSCONV Export_USetProperty_Get_SetLayout(USetProperty* instance)
{
	return instance->SetLayout;
}

CSEXPORT void CSCONV Export_USetProperty_Set_SetLayout(USetProperty* instance, FScriptSetLayout value)
{
	instance->SetLayout = value;
}

CSEXPORT void CSCONV Export_USetProperty(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_USetProperty_Get_ElementProp);
	REGISTER_FUNC(Export_USetProperty_Set_ElementProp);
	REGISTER_FUNC(Export_USetProperty_Get_SetLayout);
	REGISTER_FUNC(Export_USetProperty_Set_SetLayout);
}
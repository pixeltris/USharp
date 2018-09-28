CSEXPORT UScriptStruct* CSCONV Export_UStructProperty_Get_Struct(UStructProperty* instance)
{
	return instance->Struct;
}

CSEXPORT void CSCONV Export_UStructProperty_Set_Struct(UStructProperty* instance, UScriptStruct* value)
{
	instance->Struct = value;
}

CSEXPORT void CSCONV Export_UStructProperty(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UStructProperty_Get_Struct);
	REGISTER_FUNC(Export_UStructProperty_Set_Struct);
}
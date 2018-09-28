CSEXPORT UClass* CSCONV Export_UClassProperty_Get_MetaClass(UClassProperty* instance)
{
	return instance->MetaClass;
}

CSEXPORT void CSCONV Export_UClassProperty_Set_MetaClass(UClassProperty* instance, UClass* value)
{
	instance->MetaClass = value;
}

CSEXPORT void CSCONV Export_UClassProperty_SetMetaClass(UClassProperty* instance, UClass* NewMetaClass)
{
	instance->SetMetaClass(NewMetaClass);
}

CSEXPORT void CSCONV Export_UClassProperty(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UClassProperty_Get_MetaClass);
	REGISTER_FUNC(Export_UClassProperty_Set_MetaClass);
	REGISTER_FUNC(Export_UClassProperty_SetMetaClass);
}
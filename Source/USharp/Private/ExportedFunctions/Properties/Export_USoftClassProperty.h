CSEXPORT UClass* CSCONV Export_USoftClassProperty_Get_MetaClass(USoftClassProperty* instance)
{
	return instance->MetaClass;
}

CSEXPORT void CSCONV Export_USoftClassProperty_Set_MetaClass(USoftClassProperty* instance, UClass* value)
{
	instance->MetaClass = value;
}

CSEXPORT void CSCONV Export_USoftClassProperty_SetMetaClass(USoftClassProperty* instance, UClass* NewMetaClass)
{
	instance->SetMetaClass(NewMetaClass);
}

CSEXPORT void CSCONV Export_USoftClassProperty(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_USoftClassProperty_Get_MetaClass);
	REGISTER_FUNC(Export_USoftClassProperty_Set_MetaClass);
	REGISTER_FUNC(Export_USoftClassProperty_SetMetaClass);
}
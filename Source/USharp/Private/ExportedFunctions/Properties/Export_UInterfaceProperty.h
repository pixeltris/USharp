CSEXPORT UClass* CSCONV Export_UInterfaceProperty_Get_InterfaceClass(UInterfaceProperty* instance)
{
	return instance->InterfaceClass;
}

CSEXPORT void CSCONV Export_UInterfaceProperty_Set_InterfaceClass(UInterfaceProperty* instance, UClass* value)
{
	instance->InterfaceClass = value;
}

CSEXPORT void CSCONV Export_UInterfaceProperty_SetInterfaceClass(UInterfaceProperty* instance, UClass* NewInterfaceClass)
{
	instance->SetInterfaceClass(NewInterfaceClass);
}

CSEXPORT void CSCONV Export_UInterfaceProperty(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UInterfaceProperty_Get_InterfaceClass);
	REGISTER_FUNC(Export_UInterfaceProperty_Set_InterfaceClass);
	REGISTER_FUNC(Export_UInterfaceProperty_SetInterfaceClass);
}
CSEXPORT UClass* CSCONV Export_UObjectPropertyBase_Get_PropertyClass(UObjectPropertyBase* instance)
{
	return instance->PropertyClass;
}

CSEXPORT void CSCONV Export_UObjectPropertyBase_Set_PropertyClass(UObjectPropertyBase* instance, UClass* Value)
{
	instance->PropertyClass = Value;
}

CSEXPORT void CSCONV Export_UObjectPropertyBase_GetCPPTypeCustom(UObjectPropertyBase* instance, FString* ExtendedTypeText, uint32 CPPExportFlags, const FString& InnerNativeTypeName, FString& result)
{
	result = instance->GetCPPTypeCustom(ExtendedTypeText, CPPExportFlags, InnerNativeTypeName);
}

CSEXPORT csbool CSCONV Export_UObjectPropertyBase_ParseObjectPropertyValue(const UProperty* Property, UObject* OwnerObject, UClass* RequiredMetaClass, uint32 PortFlags, const FString& Buffer, UObject*& out_ResolvedValue)
{
	const TCHAR* BufferChars = *Buffer;
	return UObjectPropertyBase::ParseObjectPropertyValue(Property, OwnerObject, RequiredMetaClass, PortFlags, BufferChars, out_ResolvedValue);
}

CSEXPORT UObject* CSCONV Export_UObjectPropertyBase_FindImportedObject(const UProperty* Property, UObject* OwnerObject, UClass* ObjectClass, UClass* RequiredMetaClass, const FString& Text, uint32 PortFlags)
{
	return UObjectPropertyBase::FindImportedObject(Property, OwnerObject, ObjectClass, RequiredMetaClass, *Text, PortFlags);
}

CSEXPORT void CSCONV Export_UObjectPropertyBase_GetExportPath(const UObject* Object, const UObject* Parent, const UObject* ExportRootScope, const uint32 PortFlags, FString& result)
{
	result = UObjectPropertyBase::GetExportPath(Object, Parent, ExportRootScope, PortFlags);
}

CSEXPORT UObject* CSCONV Export_UObjectPropertyBase_GetObjectPropertyValue(UObjectPropertyBase* instance, const void* PropertyValueAddress)
{
	return instance->GetObjectPropertyValue(PropertyValueAddress);
}

CSEXPORT UObject* CSCONV Export_UObjectPropertyBase_GetObjectPropertyValue_InContainer(UObjectPropertyBase* instance, const void* PropertyValueAddress, int32 ArrayIndex)
{
	return instance->GetObjectPropertyValue_InContainer(PropertyValueAddress, ArrayIndex);
}

CSEXPORT void CSCONV Export_UObjectPropertyBase_SetObjectPropertyValue(UObjectPropertyBase* instance, void* PropertyValueAddress, UObject* Value)
{
	instance->SetObjectPropertyValue(PropertyValueAddress, Value);
}

CSEXPORT void CSCONV Export_UObjectPropertyBase_SetObjectPropertyValue_InContainer(UObjectPropertyBase* instance, void* PropertyValueAddress, UObject* Value, int32 ArrayIndex)
{
	instance->SetObjectPropertyValue_InContainer(PropertyValueAddress, Value, ArrayIndex);
}

CSEXPORT void CSCONV Export_UObjectPropertyBase_SetPropertyClass(UObjectPropertyBase* instance, UClass* NewPropertyClass)
{
	instance->SetPropertyClass(NewPropertyClass);
}

CSEXPORT void CSCONV Export_UObjectPropertyBase(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UObjectPropertyBase_Get_PropertyClass);
	REGISTER_FUNC(Export_UObjectPropertyBase_Set_PropertyClass);
	REGISTER_FUNC(Export_UObjectPropertyBase_GetCPPTypeCustom);
	REGISTER_FUNC(Export_UObjectPropertyBase_ParseObjectPropertyValue);
	REGISTER_FUNC(Export_UObjectPropertyBase_FindImportedObject);
	REGISTER_FUNC(Export_UObjectPropertyBase_GetExportPath);
	REGISTER_FUNC(Export_UObjectPropertyBase_GetObjectPropertyValue);
	REGISTER_FUNC(Export_UObjectPropertyBase_GetObjectPropertyValue_InContainer);
	REGISTER_FUNC(Export_UObjectPropertyBase_SetObjectPropertyValue);
	REGISTER_FUNC(Export_UObjectPropertyBase_SetObjectPropertyValue_InContainer);
	REGISTER_FUNC(Export_UObjectPropertyBase_SetPropertyClass);
}
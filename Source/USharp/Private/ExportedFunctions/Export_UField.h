CSEXPORT UField* CSCONV Export_UField_Get_Next(UField* instance)
{
	return instance->Next;
}

CSEXPORT void CSCONV Export_UField_Set_Next(UField* instance, UField* value)
{
	instance->Next = value;
}

CSEXPORT void CSCONV Export_UField_AddCppProperty(UField* instance, UProperty* Property)
{
	instance->AddCppProperty(Property);
}

CSEXPORT void CSCONV Export_UField_Bind(UField* instance)
{
	instance->Bind();
}

CSEXPORT UClass* CSCONV Export_UField_GetOwnerClass(UField* instance)
{
	return instance->GetOwnerClass();
}

CSEXPORT UStruct* CSCONV Export_UField_GetOwnerStruct(UField* instance)
{
	return instance->GetOwnerStruct();
}

#if WITH_EDITOR || HACK_HEADER_GENERATOR
CSEXPORT void CSCONV Export_UField_GetDisplayName(UField* instance, FString& result)
{
	result = instance->GetDisplayNameText().ToString();
}

CSEXPORT void CSCONV Export_UField_GetToolTip(UField* instance, FString& result)
{
	result = instance->GetToolTipText().ToString();
}

CSEXPORT csbool CSCONV Export_UField_HasMetaData(UField* instance, const FString& Key)
{
	return instance->HasMetaData(*Key);
}

CSEXPORT csbool CSCONV Export_UField_HasMetaDataF(UField* instance, const FName& Key)
{
	return instance->HasMetaData(Key);
}

CSEXPORT void CSCONV Export_UField_GetMetaData(UField* instance, const FString& Key, FString& result)
{
	result = instance->GetMetaData(*Key);
}

CSEXPORT void CSCONV Export_UField_GetMetaDataF(UField* instance, const FName& Key, FString& result)
{
	result = instance->GetMetaData(Key);
}

CSEXPORT void CSCONV Export_UField_SetMetaData(UField* instance, const FString& Key, const FString& InValue)
{
	instance->SetMetaData(*Key, *InValue);
}

CSEXPORT void CSCONV Export_UField_SetMetaDataF(UField* instance, const FName& Key, const FString& InValue)
{
	instance->SetMetaData(Key, *InValue);
}

CSEXPORT csbool CSCONV Export_UField_GetBoolMetaData(UField* instance, const FString& Key)
{
	return instance->GetBoolMetaData(*Key);
}

CSEXPORT csbool CSCONV Export_UField_GetBoolMetaDataF(UField* instance, const FName& Key)
{
	return instance->GetBoolMetaData(Key);
}

CSEXPORT int32 CSCONV Export_UField_GetINTMetaData(UField* instance, const FString& Key)
{
	return instance->GetINTMetaData(*Key);
}

CSEXPORT int32 CSCONV Export_UField_GetINTMetaDataF(UField* instance, const FName& Key)
{
	return instance->GetINTMetaData(Key);
}

CSEXPORT float CSCONV Export_UField_GetFLOATMetaData(UField* instance, const FString& Key)
{
	return instance->GetFLOATMetaData(*Key);
}

CSEXPORT float CSCONV Export_UField_GetFLOATMetaDataF(UField* instance, const FName& Key)
{
	return instance->GetFLOATMetaData(Key);
}

CSEXPORT UClass* CSCONV Export_UField_GetClassMetaData(UField* instance, const FString& Key)
{
	return instance->GetClassMetaData(*Key);
}

CSEXPORT UClass* CSCONV Export_UField_GetClassMetaDataF(UField* instance, const FName& Key)
{
	return instance->GetClassMetaData(Key);
}

CSEXPORT void CSCONV Export_UField_RemoveMetaData(UField* instance, const FString& Key)
{
	instance->RemoveMetaData(*Key);
}

CSEXPORT void CSCONV Export_UField_RemoveMetaDataF(UField* instance, const FName& Key)
{
	instance->RemoveMetaData(Key);
}
#endif

CSEXPORT void CSCONV Export_UField(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UField_Get_Next);
	REGISTER_FUNC(Export_UField_Set_Next);
	REGISTER_FUNC(Export_UField_AddCppProperty);
	REGISTER_FUNC(Export_UField_Bind);
	REGISTER_FUNC(Export_UField_GetOwnerClass);
	REGISTER_FUNC(Export_UField_GetOwnerStruct);
#if WITH_EDITOR || HACK_HEADER_GENERATOR
	REGISTER_FUNC(Export_UField_GetDisplayName);
	REGISTER_FUNC(Export_UField_GetToolTip);
	REGISTER_FUNC(Export_UField_HasMetaData);
	REGISTER_FUNC(Export_UField_HasMetaDataF);
	REGISTER_FUNC(Export_UField_GetMetaData);
	REGISTER_FUNC(Export_UField_GetMetaDataF);
	REGISTER_FUNC(Export_UField_SetMetaData);
	REGISTER_FUNC(Export_UField_SetMetaDataF);
	REGISTER_FUNC(Export_UField_GetBoolMetaData);
	REGISTER_FUNC(Export_UField_GetBoolMetaDataF);
	REGISTER_FUNC(Export_UField_GetINTMetaData);
	REGISTER_FUNC(Export_UField_GetINTMetaDataF);
	REGISTER_FUNC(Export_UField_GetFLOATMetaData);
	REGISTER_FUNC(Export_UField_GetFLOATMetaDataF);
	REGISTER_FUNC(Export_UField_GetClassMetaData);
	REGISTER_FUNC(Export_UField_GetClassMetaDataF);
	REGISTER_FUNC(Export_UField_RemoveMetaData);
	REGISTER_FUNC(Export_UField_RemoveMetaDataF);
#endif
}
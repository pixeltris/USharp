CSEXPORT void CSCONV Export_UMetaData_GetValue(UMetaData* instance, UObject* Object, const FString& Key, FString& result)
{
	result = instance->GetValue(Object, *Key);
}

CSEXPORT void CSCONV Export_UMetaData_GetValueFName(UMetaData* instance, UObject* Object, const FName& Key, FString& result)
{
	result = instance->GetValue(Object, Key);
}

CSEXPORT csbool CSCONV Export_UMetaData_HasValue(UMetaData* instance, UObject* Object, const FString& Key)
{
	return instance->HasValue(Object, *Key);
}

CSEXPORT csbool CSCONV Export_UMetaData_HasValueFName(UMetaData* instance, UObject* Object, const FName& Key)
{
	return instance->HasValue(Object, Key);
}

CSEXPORT csbool CSCONV Export_UMetaData_HasObjectValues(UMetaData* instance, UObject* Object)
{
	return instance->HasObjectValues(Object);
}

CSEXPORT void CSCONV Export_UMetaData_SetObjectValues(UMetaData* instance, UObject* Object, TArray<FName>& Keys, TArray<FString>& Values)
{
	TMap<FName, FString> ObjectValues;
	if(Keys.Num() == Values.Num())
	{
		for(int32 Index = 0; Index < Keys.Num(); ++Index)
		{
			ObjectValues.Add(Keys[Index], Values[Index]);
		}
	}
	instance->SetObjectValues(Object, ObjectValues);
}

CSEXPORT void CSCONV Export_UMetaData_SetValue(UMetaData* instance, UObject* Object, const FString& Key, const FString& Value)
{
	instance->SetValue(Object, *Key, *Value);
}

CSEXPORT void CSCONV Export_UMetaData_SetValueFName(UMetaData* instance, UObject* Object, const FName& Key, const FString& Value)
{
	instance->SetValue(Object, Key, *Value);
}

CSEXPORT void CSCONV Export_UMetaData_RemoveValue(UMetaData* instance, UObject* Object, const FString& Key)
{
	instance->RemoveValue(Object, *Key);
}

CSEXPORT void CSCONV Export_UMetaData_RemoveValueFName(UMetaData* instance, UObject* Object, const FName& Key)
{
	instance->RemoveValue(Object, Key);
}

CSEXPORT void CSCONV Export_UMetaData_GetMapForObject(UObject* Object, TArray<FName>& Keys, TArray<FString>& Values)
{	
	TMap<FName, FString>* ObjectValues = UMetaData::GetMapForObject(Object);
	if(ObjectValues != nullptr)
	{
		for(const TPair<FName, FString>& Value : *ObjectValues)
		{
			Keys.Add(Value.Key);
			Values.Add(Value.Value);
		}
	}
}

CSEXPORT void CSCONV Export_UMetaData_CopyMetadata(UObject* SourceObject, UObject* DestObject)
{
	UMetaData::CopyMetadata(SourceObject, DestObject);
}

CSEXPORT void CSCONV Export_UMetaData_RemoveMetaDataOutsidePackage(UMetaData* instance)
{
	instance->RemoveMetaDataOutsidePackage();
}

CSEXPORT void CSCONV Export_UMetaData(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UMetaData_GetValue);
	REGISTER_FUNC(Export_UMetaData_GetValueFName);
	REGISTER_FUNC(Export_UMetaData_HasValue);
	REGISTER_FUNC(Export_UMetaData_HasValueFName);
	REGISTER_FUNC(Export_UMetaData_HasObjectValues);
	REGISTER_FUNC(Export_UMetaData_SetObjectValues);
	REGISTER_FUNC(Export_UMetaData_SetValue);
	REGISTER_FUNC(Export_UMetaData_SetValueFName);
	REGISTER_FUNC(Export_UMetaData_RemoveValue);
	REGISTER_FUNC(Export_UMetaData_RemoveValueFName);
	REGISTER_FUNC(Export_UMetaData_GetMapForObject);
	REGISTER_FUNC(Export_UMetaData_CopyMetadata);
	REGISTER_FUNC(Export_UMetaData_RemoveMetaDataOutsidePackage);
}
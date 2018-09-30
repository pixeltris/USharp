CSEXPORT void CSCONV Export_UEnum_Get_CppType(UEnum* instance, FString& result)
{
	result = instance->CppType;
}

CSEXPORT void CSCONV Export_UEnum_Set_CppType(UEnum* instance, const FString& value)
{
	instance->CppType = value;
}

CSEXPORT int32 CSCONV Export_UEnum_GetIndexByValue(UEnum* instance, int64 Value)
{
	return instance->GetIndexByValue(Value);
}

CSEXPORT int64 CSCONV Export_UEnum_GetValueByIndex(UEnum* instance, int32 Index)
{
	return instance->GetValueByIndex(Index);
}

CSEXPORT void CSCONV Export_UEnum_GetNameByIndex(UEnum* instance, int32 Index, FName& result)
{
	result = instance->GetNameByIndex(Index);
}

CSEXPORT int32 CSCONV Export_UEnum_GetIndexByName(UEnum* instance, const FName& InName, EGetByNameFlags Flags)
{
	return instance->GetIndexByName(InName, Flags);
}

CSEXPORT void CSCONV Export_UEnum_GetNameByValue(UEnum* instance, int64 InValue, FName& result)
{
	result = instance->GetNameByValue(InValue);
}

CSEXPORT int32 CSCONV Export_UEnum_GetValueByName(UEnum* instance, const FName& InName, EGetByNameFlags Flags)
{
	return instance->GetValueByName(InName, Flags);
}

CSEXPORT void CSCONV Export_UEnum_GetNameStringByIndex(UEnum* instance, int32 Index, FString& result)
{
	result = instance->GetNameStringByIndex(Index);
}

CSEXPORT int32 CSCONV Export_UEnum_GetIndexByNameString(UEnum* instance, const FString& SearchString, EGetByNameFlags Flags)
{
	return instance->GetIndexByNameString(SearchString, Flags);	
}

CSEXPORT void CSCONV Export_UEnum_GetNameStringByValue(UEnum* instance, int64 InValue, FString& result)
{
	result = instance->GetNameStringByValue(InValue);
}

CSEXPORT int64 CSCONV Export_UEnum_GetValueByNameString(UEnum* instance, const FString& SearchString, EGetByNameFlags Flags)
{
	return instance->GetValueByNameString(SearchString, Flags);
}

CSEXPORT void CSCONV Export_UEnum_GetDisplayNameTextStringByIndex(UEnum* instance, int32 InIndex, FString& result)
{
	result = instance->GetDisplayNameTextByIndex(InIndex).ToString();
}

CSEXPORT void CSCONV Export_UEnum_GetDisplayNameTextStringByValue(UEnum* instance, int64 InValue, FString& result)
{
	result = instance->GetDisplayNameTextByValue(InValue).ToString();
}

CSEXPORT int64 CSCONV Export_UEnum_GetMaxEnumValue(UEnum* instance)
{
	return instance->GetMaxEnumValue();
}

CSEXPORT csbool CSCONV Export_UEnum_IsValidEnumValue(UEnum* instance, int64 InValue)
{
	return instance->IsValidEnumValue(InValue);
}

CSEXPORT csbool CSCONV Export_UEnum_IsValidEnumName(UEnum* instance, const FName& InName)
{
	return instance->IsValidEnumName(InName);
}

CSEXPORT void CSCONV Export_UEnum_RemoveNamesFromMasterList(UEnum* instance)
{
	instance->RemoveNamesFromMasterList();
}

CSEXPORT int64 CSCONV Export_UEnum_ResolveEnumerator(UEnum* instance, FArchive& Ar, int64 EnumeratorIndex)
{
	return instance->ResolveEnumerator(Ar, EnumeratorIndex);
}

CSEXPORT int32 CSCONV Export_UEnum_GetCppForm(UEnum* instance)
{	
	return (int32)instance->GetCppForm();
}

CSEXPORT csbool CSCONV Export_UEnum_IsFullEnumName(const FString& InEnumName)
{
	return UEnum::IsFullEnumName(*InEnumName);
}

CSEXPORT void CSCONV Export_UEnum_GenerateFullEnumName(UEnum* instance, const FString& InEnumName, FString& result)
{
	result = instance->GenerateFullEnumName(*InEnumName);
}

CSEXPORT int64 CSCONV Export_UEnum_LookupEnumName(const FName& TestName, UEnum** FoundEnum)
{
	return UEnum::LookupEnumName(TestName, FoundEnum);
}

CSEXPORT int64 CSCONV Export_UEnum_LookupEnumNameSlow(const FString& InTestShortName, UEnum** FoundEnum)
{
	return UEnum::LookupEnumNameSlow(*InTestShortName, FoundEnum);
}

CSEXPORT int64 CSCONV Export_UEnum_ParseEnum(const FString& Str)
{
	const TCHAR* Chars = *Str;
	return UEnum::ParseEnum(Chars);
}

CSEXPORT csbool CSCONV Export_UEnum_SetEnums(UEnum* instance, TArray<FName>& InNames, TArray<int64>& InValues, UEnum::ECppForm InCppForm, csbool bAddMaxKeyIfMissing)
{
	TArray<TPair<FName, int64>> Names;
	if(InNames.Num() != InValues.Num())
	{
		return false;
	}
	for(int32 Index = 0; Index < InNames.Num(); ++Index)
	{
		Names.Add(TPairInitializer<FName, int64>(InNames[Index], InValues[Index]));
	}
	return instance->SetEnums(Names, InCppForm, !!bAddMaxKeyIfMissing);
}

CSEXPORT int32 CSCONV Export_UEnum_NumEnums(UEnum* instance)
{
	return instance->NumEnums();
}

CSEXPORT void CSCONV Export_UEnum_GenerateEnumPrefix(UEnum* instance, FString& result)
{
	result = instance->GenerateEnumPrefix();
}

#if WITH_EDITOR
CSEXPORT void CSCONV Export_UEnum_GetToolTipByIndex(UEnum* instance, int32 NameIndex, FString& result)
{
	result = instance->GetToolTipTextByIndex(NameIndex).ToString();
}

CSEXPORT csbool CSCONV Export_UEnum_HasMetaData(UEnum* instance, const FString& Key, int32 NameIndex)
{
	return instance->HasMetaData(*Key, NameIndex);
}

CSEXPORT void CSCONV Export_UEnum_GetMetaData(UEnum* instance, const FString& Key, int32 NameIndex, FString& result)
{
	result = instance->GetMetaData(*Key, NameIndex);
}

CSEXPORT void CSCONV Export_UEnum_SetMetaData(UEnum* instance, const FString& Key, const FString& InValue, int32 NameIndex)
{
	instance->SetMetaData(*Key, *InValue, NameIndex);
}

CSEXPORT void CSCONV Export_UEnum_RemoveMetaData(UEnum* instance, const FString& Key, int32 NameIndex)
{
	instance->RemoveMetaData(*Key, NameIndex);
}
#endif

CSEXPORT void CSCONV Export_UEnum(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UEnum_Get_CppType);
	REGISTER_FUNC(Export_UEnum_Set_CppType);
	REGISTER_FUNC(Export_UEnum_GetIndexByValue);
	REGISTER_FUNC(Export_UEnum_GetValueByIndex);
	REGISTER_FUNC(Export_UEnum_GetNameByIndex);	
	REGISTER_FUNC(Export_UEnum_GetIndexByName);
	REGISTER_FUNC(Export_UEnum_GetNameByValue);
	REGISTER_FUNC(Export_UEnum_GetValueByName);
	REGISTER_FUNC(Export_UEnum_GetNameStringByIndex);
	REGISTER_FUNC(Export_UEnum_GetIndexByNameString);
	REGISTER_FUNC(Export_UEnum_GetNameStringByValue);
	REGISTER_FUNC(Export_UEnum_GetValueByNameString);
	REGISTER_FUNC(Export_UEnum_GetDisplayNameTextStringByIndex);
	REGISTER_FUNC(Export_UEnum_GetDisplayNameTextStringByValue);
	REGISTER_FUNC(Export_UEnum_GetMaxEnumValue);
	REGISTER_FUNC(Export_UEnum_IsValidEnumValue);
	REGISTER_FUNC(Export_UEnum_IsValidEnumName);
	REGISTER_FUNC(Export_UEnum_RemoveNamesFromMasterList);
	REGISTER_FUNC(Export_UEnum_ResolveEnumerator);
	REGISTER_FUNC(Export_UEnum_GetCppForm);
	REGISTER_FUNC(Export_UEnum_IsFullEnumName);
	REGISTER_FUNC(Export_UEnum_GenerateFullEnumName);
	REGISTER_FUNC(Export_UEnum_LookupEnumName);
	REGISTER_FUNC(Export_UEnum_LookupEnumNameSlow);
	REGISTER_FUNC(Export_UEnum_ParseEnum);
	REGISTER_FUNC(Export_UEnum_SetEnums);
	REGISTER_FUNC(Export_UEnum_NumEnums);
	REGISTER_FUNC(Export_UEnum_GenerateEnumPrefix);
#if WITH_EDITOR
	REGISTER_FUNC(Export_UEnum_GetToolTipByIndex);
	REGISTER_FUNC(Export_UEnum_HasMetaData);
	REGISTER_FUNC(Export_UEnum_GetMetaData);
	REGISTER_FUNC(Export_UEnum_SetMetaData);
	REGISTER_FUNC(Export_UEnum_RemoveMetaData);
#endif
}
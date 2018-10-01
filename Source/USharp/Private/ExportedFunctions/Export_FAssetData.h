CSEXPORT void CSCONV Export_FAssetData_Get_TagsAndValues(FAssetData* instance, TArray<FName>& outTags, TArray<FString>& outValues)
{
	for(const TPair<FName, FString>& Pair : instance->TagsAndValues)
	{
		outTags.Add(Pair.Key);
		outValues.Add(Pair.Value);
	}
}

CSEXPORT csbool CSCONV Export_FAssetData_IsValid(FAssetData* instance)
{
	return instance->IsValid();
}

CSEXPORT csbool CSCONV Export_FAssetData_IsUAsset(FAssetData* instance)
{
	return instance->IsUAsset();
}

CSEXPORT csbool CSCONV Export_FAssetData_IsRedirector(FAssetData* instance)
{
	return instance->IsRedirector();
}

CSEXPORT void CSCONV Export_FAssetData_GetFullName(FAssetData* instance, FString& result)
{
	result = instance->GetFullName();
}

CSEXPORT void CSCONV Export_FAssetData_GetExportTextName(FAssetData* instance, FString& result)
{
	result = instance->GetExportTextName();
}

CSEXPORT void CSCONV Export_FAssetData(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FAssetData_Get_TagsAndValues);
	REGISTER_FUNC(Export_FAssetData_IsValid);
	REGISTER_FUNC(Export_FAssetData_IsUAsset);
	REGISTER_FUNC(Export_FAssetData_IsRedirector);
	REGISTER_FUNC(Export_FAssetData_GetFullName);
	REGISTER_FUNC(Export_FAssetData_GetExportTextName);
}
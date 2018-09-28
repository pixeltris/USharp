CSEXPORT void CSCONV Export_FSoftObjectPath_ToString(FSoftObjectPath* instance, FString& result)
{
	result = instance->ToString();
}

CSEXPORT void CSCONV Export_FSoftObjectPath_GetLongPackageName(FSoftObjectPath* instance, FString& result)
{
	result = instance->GetLongPackageName();
}

CSEXPORT void CSCONV Export_FSoftObjectPath_GetAssetName(FSoftObjectPath* instance, FString& result)
{
	result = instance->GetAssetName();
}

CSEXPORT void CSCONV Export_FSoftObjectPath_SetPath(FSoftObjectPath* instance, const FString& Path)
{
	instance->SetPath(Path);
}

CSEXPORT UObject* CSCONV Export_FSoftObjectPath_TryLoad(FSoftObjectPath* instance)
{
	return instance->TryLoad();
}

CSEXPORT UObject* CSCONV Export_FSoftObjectPath_ResolveObject(FSoftObjectPath* instance)
{
	return instance->ResolveObject();
}

CSEXPORT void CSCONV Export_FSoftObjectPath_Reset(FSoftObjectPath* instance)
{
	instance->Reset();
}

CSEXPORT csbool CSCONV Export_FSoftObjectPath_IsValid(FSoftObjectPath* instance)
{
	return instance->IsValid();
}

CSEXPORT csbool CSCONV Export_FSoftObjectPath_IsNull(FSoftObjectPath* instance)
{
	return instance->IsNull();
}

CSEXPORT csbool CSCONV Export_FSoftObjectPath_IsAsset(FSoftObjectPath* instance)
{
	return instance->IsAsset();
}

CSEXPORT void CSCONV Export_FSoftObjectPath(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FSoftObjectPath_ToString);
	REGISTER_FUNC(Export_FSoftObjectPath_GetLongPackageName);
	REGISTER_FUNC(Export_FSoftObjectPath_GetAssetName);
	REGISTER_FUNC(Export_FSoftObjectPath_SetPath);
	REGISTER_FUNC(Export_FSoftObjectPath_TryLoad);
	REGISTER_FUNC(Export_FSoftObjectPath_ResolveObject);
	REGISTER_FUNC(Export_FSoftObjectPath_Reset);
	REGISTER_FUNC(Export_FSoftObjectPath_IsValid);
	REGISTER_FUNC(Export_FSoftObjectPath_IsNull);
	REGISTER_FUNC(Export_FSoftObjectPath_IsAsset);
}
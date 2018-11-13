CSEXPORT void CSCONV Export_FKey_CopyFrom(FKey& Value, FKey& CopyFrom)
{
	// This should update the TSharedPtr<FKeyDetails> for us
	Value = CopyFrom;
}

CSEXPORT FKeyDetails* CSCONV Export_FKey_GetKeyDetails(FKey& Key)
{
	return EKeys::GetKeyDetails(Key).Get();
}

CSEXPORT int32 CSCONV Export_FKey_GetKeyDetailsRefCount(FKey& Key)
{
	TSharedPtr<FKeyDetails> KeyDetails = EKeys::GetKeyDetails(Key);
	if (KeyDetails.IsValid())
	{
		return KeyDetails.GetSharedReferenceCount();
	}
	return 0;
}

CSEXPORT void CSCONV Export_FKey_GetKeyDetailsRef(FKey& Key, TSharedPtr<FKeyDetails>& result)
{
	result = EKeys::GetKeyDetails(Key);
}

CSEXPORT csbool CSCONV Export_FKey_IsValid(FKey* instance)
{
	return instance->IsValid();
}

CSEXPORT csbool CSCONV Export_FKey_IsModifierKey(FKey* instance)
{
	return instance->IsModifierKey();
}

CSEXPORT csbool CSCONV Export_FKey_IsGamepadKey(FKey* instance)
{
	return instance->IsGamepadKey();
}

CSEXPORT csbool CSCONV Export_FKey_IsMouseButton(FKey* instance)
{
	return instance->IsMouseButton();
}

CSEXPORT csbool CSCONV Export_FKey_IsFloatAxis(FKey* instance)
{
	return instance->IsFloatAxis();
}

CSEXPORT csbool CSCONV Export_FKey_IsVectorAxis(FKey* instance)
{
	return instance->IsVectorAxis();
}

CSEXPORT csbool CSCONV Export_FKey_IsBindableInBlueprints(FKey* instance)
{
	return instance->IsBindableInBlueprints();
}

CSEXPORT csbool CSCONV Export_FKey_ShouldUpdateAxisWithoutSamples(FKey* instance)
{
	return instance->ShouldUpdateAxisWithoutSamples();
}

CSEXPORT void CSCONV Export_FKey_GetDisplayName(FKey* instance, FText& result)
{
	result = instance->GetDisplayName();
}

CSEXPORT void CSCONV Export_FKey_GetDisplayNameString(FKey* instance, FString& result)
{
	result = instance->GetDisplayName().ToString();
}

CSEXPORT void CSCONV Export_FKey_GetMenuCategory(FKey* instance, FName& result)
{
	result = instance->GetMenuCategory();
}

CSEXPORT void CSCONV Export_FKey(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FKey_CopyFrom);
	REGISTER_FUNC(Export_FKey_GetKeyDetails);
	REGISTER_FUNC(Export_FKey_GetKeyDetailsRefCount);
	REGISTER_FUNC(Export_FKey_GetKeyDetailsRef);
	REGISTER_FUNC(Export_FKey_IsValid);
	REGISTER_FUNC(Export_FKey_IsModifierKey);
	REGISTER_FUNC(Export_FKey_IsGamepadKey);
	REGISTER_FUNC(Export_FKey_IsMouseButton);
	REGISTER_FUNC(Export_FKey_IsFloatAxis);
	REGISTER_FUNC(Export_FKey_IsVectorAxis);
	REGISTER_FUNC(Export_FKey_IsBindableInBlueprints);
	REGISTER_FUNC(Export_FKey_ShouldUpdateAxisWithoutSamples);
	REGISTER_FUNC(Export_FKey_GetDisplayName);
	REGISTER_FUNC(Export_FKey_GetDisplayNameString);
	REGISTER_FUNC(Export_FKey_GetMenuCategory);
}
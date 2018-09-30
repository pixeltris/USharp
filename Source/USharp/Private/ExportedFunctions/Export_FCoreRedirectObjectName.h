CSEXPORT void CSCONV Export_FCoreRedirectObjectName_CtorString(const FString& InString, FCoreRedirectObjectName& result)
{
	result = FCoreRedirectObjectName(InString);
}

CSEXPORT void CSCONV Export_FCoreRedirectObjectName_CtorObject(const class UObject* Object, FCoreRedirectObjectName& result)
{
	result = FCoreRedirectObjectName(Object);
}

CSEXPORT void CSCONV Export_FCoreRedirectObjectName_ToString(FCoreRedirectObjectName* instance, FString& result)
{
	result = instance->ToString();
}

CSEXPORT void CSCONV Export_FCoreRedirectObjectName_Reset(FCoreRedirectObjectName* instance)
{
	instance->Reset();
}

CSEXPORT csbool CSCONV Export_FCoreRedirectObjectName_Matches(FCoreRedirectObjectName* instance, const FCoreRedirectObjectName& Other, bool bCheckSubstring)
{
	return instance->Matches(Other, bCheckSubstring);
}

CSEXPORT int32 CSCONV Export_FCoreRedirectObjectName_MatchScore(FCoreRedirectObjectName* instance, const FCoreRedirectObjectName& Other)
{
	return instance->MatchScore(Other);
}

CSEXPORT void CSCONV Export_FCoreRedirectObjectName_GetSearchKey(FCoreRedirectObjectName* instance, ECoreRedirectFlags Type, FName& result)
{
	result = instance->GetSearchKey(Type);
}

CSEXPORT csbool CSCONV Export_FCoreRedirectObjectName_IsValid(FCoreRedirectObjectName* instance)
{
	return instance->IsValid();
}

CSEXPORT csbool CSCONV Export_FCoreRedirectObjectName_HasValidCharacters(FCoreRedirectObjectName* instance)
{
	return instance->HasValidCharacters();
}

CSEXPORT csbool CSCONV Export_FCoreRedirectObjectName_ExpandNames(const FString& FullString, FName& OutName, FName& OutOuter, FName& OutPackage)
{
	return FCoreRedirectObjectName::ExpandNames(FullString, OutName, OutOuter, OutPackage);
}

CSEXPORT void CSCONV Export_FCoreRedirectObjectName_CombineNames(const FName& NewName, const FName& NewOuter, const FName& NewPackage, FString& result)
{
	result = FCoreRedirectObjectName::CombineNames(NewName, NewOuter, NewPackage);
}

CSEXPORT void CSCONV Export_FCoreRedirectObjectName(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FCoreRedirectObjectName_CtorString);
	REGISTER_FUNC(Export_FCoreRedirectObjectName_CtorObject);
	REGISTER_FUNC(Export_FCoreRedirectObjectName_ToString);
	REGISTER_FUNC(Export_FCoreRedirectObjectName_Reset);
	REGISTER_FUNC(Export_FCoreRedirectObjectName_Matches);
	REGISTER_FUNC(Export_FCoreRedirectObjectName_MatchScore);
	REGISTER_FUNC(Export_FCoreRedirectObjectName_GetSearchKey);
	REGISTER_FUNC(Export_FCoreRedirectObjectName_IsValid);
	REGISTER_FUNC(Export_FCoreRedirectObjectName_HasValidCharacters);
	REGISTER_FUNC(Export_FCoreRedirectObjectName_ExpandNames);
	REGISTER_FUNC(Export_FCoreRedirectObjectName_CombineNames);
}
CSEXPORT FCoreRedirectObjectName CSCONV Export_FCoreRedirects_GetRedirectedName(ECoreRedirectFlags Type, const FCoreRedirectObjectName& OldObjectName)
{
	return FCoreRedirects::GetRedirectedName(Type, OldObjectName);
}

CSEXPORT csbool CSCONV Export_FCoreRedirects_IsKnownMissing(ECoreRedirectFlags Type, const FCoreRedirectObjectName& ObjectName)
{
	return FCoreRedirects::IsKnownMissing(Type, ObjectName);
}

CSEXPORT csbool CSCONV Export_FCoreRedirects_AddKnownMissing(ECoreRedirectFlags Type, const FCoreRedirectObjectName& ObjectName)
{
	return FCoreRedirects::AddKnownMissing(Type, ObjectName);
}

CSEXPORT csbool CSCONV Export_FCoreRedirects_RemoveKnownMissing(ECoreRedirectFlags Type, const FCoreRedirectObjectName& ObjectName)
{
	return FCoreRedirects::RemoveKnownMissing(Type, ObjectName);
}

CSEXPORT csbool CSCONV Export_FCoreRedirects_FindPreviousNames(ECoreRedirectFlags Type, const FCoreRedirectObjectName& NewObjectName, TArray<FCoreRedirectObjectName>& PreviousNames)
{
	return FCoreRedirects::FindPreviousNames(Type, NewObjectName, PreviousNames);
}

CSEXPORT csbool CSCONV Export_FCoreRedirects_ReadRedirectsFromIni(const FString& IniName)
{
	return FCoreRedirects::ReadRedirectsFromIni(IniName);
}

CSEXPORT csbool CSCONV Export_FCoreRedirects_IsInitialized()
{
	return FCoreRedirects::IsInitialized();
}

CSEXPORT ECoreRedirectFlags CSCONV Export_FCoreRedirects_GetFlagsForTypeName(const FName& PackageName, const FName& TypeName)
{
	return FCoreRedirects::GetFlagsForTypeName(PackageName, TypeName);
}

CSEXPORT ECoreRedirectFlags CSCONV Export_FCoreRedirects_GetFlagsForTypeClass(UClass *TypeClass)
{
	return FCoreRedirects::GetFlagsForTypeClass(TypeClass);
}

CSEXPORT void CSCONV Export_FCoreRedirects(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FCoreRedirects_GetRedirectedName);
	REGISTER_FUNC(Export_FCoreRedirects_IsKnownMissing);
	REGISTER_FUNC(Export_FCoreRedirects_AddKnownMissing);
	REGISTER_FUNC(Export_FCoreRedirects_RemoveKnownMissing);
	REGISTER_FUNC(Export_FCoreRedirects_FindPreviousNames);
	REGISTER_FUNC(Export_FCoreRedirects_ReadRedirectsFromIni);
	REGISTER_FUNC(Export_FCoreRedirects_IsInitialized);
	REGISTER_FUNC(Export_FCoreRedirects_GetFlagsForTypeName);
	REGISTER_FUNC(Export_FCoreRedirects_GetFlagsForTypeClass);
}
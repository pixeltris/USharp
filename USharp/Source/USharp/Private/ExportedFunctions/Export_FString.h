CSEXPORT int32 CSCONV Export_FString_GetCharSize()
{
	return sizeof(TCHAR);
}

CSEXPORT void CSCONV Export_FString_FromCharPtr(TCHAR* Str, FString& result)
{
	result = Str;
}

CSEXPORT void CSCONV Export_FString(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FString_GetCharSize);
	REGISTER_FUNC(Export_FString_FromCharPtr);
}
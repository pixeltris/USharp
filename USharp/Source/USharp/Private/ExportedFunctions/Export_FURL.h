CSEXPORT void CSCONV Export_FURL_Get_Protocol(FURL* instance, FString& result)
{
	result = instance->Protocol;
}

CSEXPORT void CSCONV Export_FURL_Set_Protocol(FURL* instance, const FString& value)
{
	instance->Protocol = value;
}

CSEXPORT void CSCONV Export_FURL_Get_Host(FURL* instance, FString& result)
{
	result = instance->Host;
}

CSEXPORT void CSCONV Export_FURL_Set_Host(FURL* instance, const FString& value)
{
	instance->Host = value;
}

CSEXPORT void CSCONV Export_FURL_Get_Map(FURL* instance, FString& result)
{
	result = instance->Map;
}

CSEXPORT void CSCONV Export_FURL_Set_Map(FURL* instance, const FString& value)
{
	instance->Map = value;
}

CSEXPORT void CSCONV Export_FURL_Get_RedirectURL(FURL* instance, FString& result)
{
	result = instance->RedirectURL;
}

CSEXPORT void CSCONV Export_FURL_Set_RedirectURL(FURL* instance, const FString& value)
{
	instance->RedirectURL = value;
}

CSEXPORT TArray<FString>& CSCONV Export_FURL_Get_Op(FURL* instance)
{
	return instance->Op;
}

CSEXPORT void CSCONV Export_FURL_Get_Portal(FURL* instance, FString& result)
{
	result = instance->Portal;
}

CSEXPORT void CSCONV Export_FURL_Set_Portal(FURL* instance, const FString& value)
{
	instance->Portal = value;
}

CSEXPORT int32 CSCONV Export_FURL_Get_Valid(FURL* instance)
{
	return instance->Valid;
}

CSEXPORT void CSCONV Export_FURL_Set_Valid(FURL* instance, int32 value)
{
	instance->Valid = value;
}

CSEXPORT void CSCONV Export_FURL(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FURL_Get_Protocol);
	REGISTER_FUNC(Export_FURL_Set_Protocol);
	REGISTER_FUNC(Export_FURL_Get_Host);
	REGISTER_FUNC(Export_FURL_Set_Host);
	REGISTER_FUNC(Export_FURL_Get_Map);
	REGISTER_FUNC(Export_FURL_Set_Map);
	REGISTER_FUNC(Export_FURL_Get_RedirectURL);
	REGISTER_FUNC(Export_FURL_Set_RedirectURL);
	REGISTER_FUNC(Export_FURL_Get_Op);
	REGISTER_FUNC(Export_FURL_Get_Portal);
	REGISTER_FUNC(Export_FURL_Set_Portal);
	REGISTER_FUNC(Export_FURL_Get_Valid);
	REGISTER_FUNC(Export_FURL_Set_Valid);
}
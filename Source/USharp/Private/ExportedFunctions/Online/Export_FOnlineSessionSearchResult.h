CSEXPORT FOnlineSession* CSCONV Export_FOnlineSessionSearchResult_Get_Session(FOnlineSessionSearchResult* instance)
{
	return &instance->Session;
}

CSEXPORT int32 CSCONV Export_FOnlineSessionSearchResult_Get_PingInMs(FOnlineSessionSearchResult* instance)
{
	return instance->PingInMs;
}

CSEXPORT void CSCONV Export_FOnlineSessionSearchResult_Set_PingInMs(FOnlineSessionSearchResult* instance, int32 value)
{
	instance->PingInMs = value;
}

CSEXPORT void CSCONV Export_FOnlineSessionSearchResult(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FOnlineSessionSearchResult_Get_Session);
	REGISTER_FUNC(Export_FOnlineSessionSearchResult_Get_PingInMs);
	REGISTER_FUNC(Export_FOnlineSessionSearchResult_Set_PingInMs);
}
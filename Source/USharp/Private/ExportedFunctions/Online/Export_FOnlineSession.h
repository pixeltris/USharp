CSEXPORT void CSCONV Export_FOnlineSession_Get_OwningUserId(FOnlineSession* instance, TSharedPtr<const FUniqueNetId>& result)
{
	result = instance->OwningUserId;
}

CSEXPORT void CSCONV Export_FOnlineSession_Set_OwningUserId(FOnlineSession* instance, TSharedPtr<const FUniqueNetId>& value)
{
	instance->OwningUserId = value;
}

CSEXPORT void CSCONV Export_FOnlineSession_Get_OwningUserName(FOnlineSession* instance, FString& result)
{
	result = instance->OwningUserName;
}

CSEXPORT void CSCONV Export_FOnlineSession_Set_OwningUserName(FOnlineSession* instance, FString& value)
{
	instance->OwningUserName = value;
}

CSEXPORT FOnlineSessionSettings* CSCONV Export_FOnlineSession_Get_SessionSettings(FOnlineSession* instance)
{
	return &instance->SessionSettings;
}

CSEXPORT void CSCONV Export_FOnlineSession_Get_SessionInfo(FOnlineSession* instance, TSharedPtr<FOnlineSessionInfo>& result)
{
	result = instance->SessionInfo;
}

CSEXPORT void CSCONV Export_FOnlineSession_Set_SessionInfo(FOnlineSession* instance, TSharedPtr<FOnlineSessionInfo>& value)
{
	instance->SessionInfo = value;
}

CSEXPORT int32 CSCONV Export_FOnlineSession_Get_NumOpenPrivateConnections(FOnlineSession* instance)
{
	return instance->NumOpenPrivateConnections;
}

CSEXPORT void CSCONV Export_FOnlineSession_Set_NumOpenPrivateConnections(FOnlineSession* instance, int32 value)
{
	instance->NumOpenPrivateConnections = value;
}

CSEXPORT int32 CSCONV Export_FOnlineSession_Get_NumOpenPublicConnections(FOnlineSession* instance)
{
	return instance->NumOpenPublicConnections;
}

CSEXPORT void CSCONV Export_FOnlineSession_Set_NumOpenPublicConnections(FOnlineSession* instance, int32 value)
{
	instance->NumOpenPublicConnections = value;
}

CSEXPORT void CSCONV Export_FOnlineSession(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FOnlineSession_Get_OwningUserId);
	REGISTER_FUNC(Export_FOnlineSession_Set_OwningUserId);
	REGISTER_FUNC(Export_FOnlineSession_Get_OwningUserName);
	REGISTER_FUNC(Export_FOnlineSession_Set_OwningUserName);
	REGISTER_FUNC(Export_FOnlineSession_Get_SessionSettings);
	REGISTER_FUNC(Export_FOnlineSession_Get_SessionInfo);
	REGISTER_FUNC(Export_FOnlineSession_Set_SessionInfo);
	REGISTER_FUNC(Export_FOnlineSession_Get_NumOpenPrivateConnections);
	REGISTER_FUNC(Export_FOnlineSession_Set_NumOpenPrivateConnections);
	REGISTER_FUNC(Export_FOnlineSession_Get_NumOpenPublicConnections);
	REGISTER_FUNC(Export_FOnlineSession_Set_NumOpenPublicConnections);
}
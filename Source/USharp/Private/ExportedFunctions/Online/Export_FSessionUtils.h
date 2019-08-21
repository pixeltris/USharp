CSEXPORT int32 CSCONV Export_FSessionUtils_GetSessionState(const FName& SessionName)
{
	IOnlineSessionPtr SessionInterface = Online::GetSessionInterface();
	if (SessionInterface.IsValid())
	{
		return (int32)SessionInterface->GetSessionState(SessionName);
	}
	return 0;
}

CSEXPORT csbool CSCONV Export_FSessionUtils_CancelFindSessions()
{
	IOnlineSessionPtr SessionInterface = Online::GetSessionInterface();
	if (SessionInterface.IsValid())
	{
		return SessionInterface->CancelFindSessions();
	}
	return 0;
}

CSEXPORT int32 CSCONV Export_FSessionUtils_GetNumSessions()
{
	IOnlineSessionPtr SessionInterface = Online::GetSessionInterface();
	if (SessionInterface.IsValid())
	{
		return SessionInterface->GetNumSessions();
	}
	return 0;
}

CSEXPORT void CSCONV Export_FSessionUtils(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FSessionUtils_GetSessionState);
	REGISTER_FUNC(Export_FSessionUtils_CancelFindSessions);
	REGISTER_FUNC(Export_FSessionUtils_GetNumSessions);
}
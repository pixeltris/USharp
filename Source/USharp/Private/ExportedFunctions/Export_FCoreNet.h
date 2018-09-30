CSEXPORT void CSCONV Export_FCoreNet_RPC_ResetLastFailedReason()
{
	RPC_ResetLastFailedReason();
}

CSEXPORT void CSCONV Export_FCoreNet_RPC_ValidateFailed(FString& Reason)
{
	// Cache the string as the GLastRPCFailedReason holds onto the TCHAR*
	static FString CachedReason;
	CachedReason = Reason;
	RPC_ValidateFailed(*CachedReason);
}

CSEXPORT void CSCONV Export_FCoreNet_RPC_GetLastFailedReason(FString& result)
{
	result = RPC_GetLastFailedReason();
}

CSEXPORT void CSCONV Export_FCoreNet(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FCoreNet_RPC_ResetLastFailedReason);
	REGISTER_FUNC(Export_FCoreNet_RPC_ValidateFailed);
	REGISTER_FUNC(Export_FCoreNet_RPC_GetLastFailedReason);
}
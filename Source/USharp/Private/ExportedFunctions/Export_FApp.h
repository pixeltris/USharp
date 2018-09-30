CSEXPORT void CSCONV Export_FApp_GetBranchName(FString& result)
{
	result = FApp::GetBranchName();
}

CSEXPORT int32/*EBuildConfigurations::Type*/ CSCONV Export_FApp_GetBuildConfiguration()
{
	return FApp::GetBuildConfiguration();
}

CSEXPORT csbool CSCONV Export_FApp_IsRunningDebug()
{
	return FApp::IsRunningDebug();
}

CSEXPORT void CSCONV Export_FApp_GetBuildVersion(FString& result)
{
	result = FApp::GetBuildVersion();
}

CSEXPORT void CSCONV Export_FApp_GetDeploymentName(FString& result)
{
	result = FApp::GetDeploymentName();
}

CSEXPORT void CSCONV Export_FApp_GetBuildDate(FString& result)
{
	result = FApp::GetBuildDate();
}

CSEXPORT csbool CSCONV Export_FApp_GetEngineIsPromotedBuild()
{
	return FApp::GetEngineIsPromotedBuild();
}

CSEXPORT void CSCONV Export_FApp_GetEpicProductIdentifier(FString& result)
{
	result = FApp::GetEpicProductIdentifier();
}

CSEXPORT void CSCONV Export_FApp_GetProjectName(FString& result)
{
	result = FApp::GetProjectName();
}

CSEXPORT void CSCONV Export_FApp_GetName(FString& result)
{
	result = FApp::GetName();
}

CSEXPORT csbool CSCONV Export_FApp_HasProjectName()
{
	return FApp::HasProjectName();
}

CSEXPORT csbool CSCONV Export_FApp_IsGame()
{
	return FApp::IsGame();
}

CSEXPORT csbool CSCONV Export_FApp_IsProjectNameEmpty()
{
	return FApp::IsProjectNameEmpty();
}

CSEXPORT void CSCONV Export_FApp_SetProjectName(const FString& InProjectName)
{
	FApp::SetProjectName(*InProjectName);
}

CSEXPORT void CSCONV Export_FApp_AuthorizeUser(const FString& UserName)
{
	FApp::AuthorizeUser(UserName);
}

CSEXPORT void CSCONV Export_FApp_DenyAllUsers()
{
	FApp::DenyAllUsers();
}

CSEXPORT void CSCONV Export_FApp_DenyUser(const FString& UserName)
{
	FApp::DenyUser(UserName);
}

CSEXPORT void CSCONV Export_FApp_GetInstanceId(FGuid& result)
{
	result = FApp::GetInstanceId();
}

CSEXPORT void CSCONV Export_FApp_GetInstanceName(FString& result)
{
	result = FApp::GetInstanceName();
}

CSEXPORT void CSCONV Export_FApp_GetSessionId(FGuid& result)
{
	result = FApp::GetSessionId();
}

CSEXPORT void CSCONV Export_FApp_GetSessionName(FString& result)
{
	result = FApp::GetSessionName();
}

CSEXPORT void CSCONV Export_FApp_GetSessionOwner(FString& result)
{
	result = FApp::GetSessionOwner();
}

CSEXPORT void CSCONV Export_FApp_InitializeSession()
{
	FApp::InitializeSession();
}

CSEXPORT csbool CSCONV Export_FApp_IsAuthorizedUser(const FString& UserName)
{
	return FApp::IsAuthorizedUser(UserName);
}

CSEXPORT csbool CSCONV Export_FApp_IsStandalone()
{
	return FApp::IsStandalone();
}

CSEXPORT csbool CSCONV Export_FApp_IsThisInstance(const FGuid& InInstanceId)
{
	return FApp::IsThisInstance(InInstanceId);
}

CSEXPORT void CSCONV Export_FApp_SetSessionName(const FString& NewName)
{
	FApp::SetSessionName(NewName);
}

CSEXPORT void CSCONV Export_FApp_SetSessionOwner(const FString& NewOwner)
{
	FApp::SetSessionOwner(NewOwner);
}

CSEXPORT csbool CSCONV Export_FApp_CanEverRender()
{
	return FApp::CanEverRender();
}

CSEXPORT csbool CSCONV Export_FApp_IsInstalled()
{
	return FApp::IsInstalled();
}

CSEXPORT csbool CSCONV Export_FApp_IsEngineInstalled()
{
	return FApp::IsEngineInstalled();
}

CSEXPORT csbool CSCONV Export_FApp_IsUnattended()
{
	return FApp::IsUnattended();
}

CSEXPORT csbool CSCONV Export_FApp_ShouldUseThreadingForPerformance()
{
	return FApp::ShouldUseThreadingForPerformance();
}

CSEXPORT csbool CSCONV Export_FApp_IsBenchmarking()
{
	return FApp::IsBenchmarking();
}

CSEXPORT void CSCONV Export_FApp_SetBenchmarking(csbool bVal)
{
	FApp::SetBenchmarking(!!bVal);
}

CSEXPORT double CSCONV Export_FApp_GetFixedDeltaTime()
{
	return FApp::GetFixedDeltaTime();
}

CSEXPORT void CSCONV Export_FApp_SetFixedDeltaTime(double Seconds)
{
	FApp::SetFixedDeltaTime(Seconds);
}

CSEXPORT csbool CSCONV Export_FApp_UseFixedTimeStep()
{
	return FApp::UseFixedTimeStep();
}

CSEXPORT void CSCONV Export_FApp_SetUseFixedTimeStep(csbool bVal)
{
	FApp::SetUseFixedTimeStep(!!bVal);
}

CSEXPORT double CSCONV Export_FApp_GetCurrentTime()
{
	return FApp::GetCurrentTime();
}

CSEXPORT void CSCONV Export_FApp_SetCurrentTime(double Seconds)
{
	FApp::SetCurrentTime(Seconds);
}

CSEXPORT double CSCONV Export_FApp_GetLastTime()
{
	return FApp::GetLastTime();
}

CSEXPORT void CSCONV Export_FApp_UpdateLastTime()
{
	FApp::UpdateLastTime();
}

CSEXPORT double CSCONV Export_FApp_GetDeltaTime()
{
	return FApp::GetDeltaTime();
}

CSEXPORT void CSCONV Export_FApp_SetDeltaTime(double Seconds)
{
	FApp::SetDeltaTime(Seconds);
}

CSEXPORT double CSCONV Export_FApp_GetIdleTime()
{
	return FApp::GetIdleTime();
}

CSEXPORT void CSCONV Export_FApp_SetIdleTime(double Seconds)
{
	FApp::SetIdleTime(Seconds);
}

CSEXPORT float CSCONV Export_FApp_GetVolumeMultiplier()
{
	return FApp::GetVolumeMultiplier();
}

CSEXPORT void CSCONV Export_FApp_SetVolumeMultiplier(float InVolumeMultiplier)
{
	FApp::SetVolumeMultiplier(InVolumeMultiplier);
}

CSEXPORT float CSCONV Export_FApp_GetUnfocusedVolumeMultiplier()
{
	return FApp::GetUnfocusedVolumeMultiplier();
}

CSEXPORT void CSCONV Export_FApp_SetUnfocusedVolumeMultiplier(float InVolumeMultiplier)
{
	FApp::SetUnfocusedVolumeMultiplier(InVolumeMultiplier);
}

CSEXPORT void CSCONV Export_FApp_SetUseVRFocus(csbool bInUseVRFocus)
{
	FApp::SetUseVRFocus(!!bInUseVRFocus);
}

CSEXPORT csbool CSCONV Export_FApp_UseVRFocus()
{
	return FApp::UseVRFocus();
}

CSEXPORT void CSCONV Export_FApp_SetHasVRFocus(csbool bInUseVRFocus)
{
	FApp::SetHasVRFocus(!!bInUseVRFocus);
}

CSEXPORT csbool CSCONV Export_FApp_HasVRFocus()
{
	return FApp::HasVRFocus();
}

CSEXPORT csbool CSCONV Export_FApp_Get_bUseFixedSeed()
{
	return FApp::bUseFixedSeed;
}

CSEXPORT void CSCONV Export_FApp_Set_bUseFixedSeed(csbool value)
{
	FApp::bUseFixedSeed = !!value;
}

CSEXPORT void CSCONV Export_FApp(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FApp_GetBranchName);
	REGISTER_FUNC(Export_FApp_GetBuildConfiguration);
	REGISTER_FUNC(Export_FApp_IsRunningDebug);
	REGISTER_FUNC(Export_FApp_GetBuildVersion);
	REGISTER_FUNC(Export_FApp_GetDeploymentName);
	REGISTER_FUNC(Export_FApp_GetBuildDate);
	REGISTER_FUNC(Export_FApp_GetEngineIsPromotedBuild);
	REGISTER_FUNC(Export_FApp_GetEpicProductIdentifier);
	REGISTER_FUNC(Export_FApp_GetProjectName);
	REGISTER_FUNC(Export_FApp_GetName);
	REGISTER_FUNC(Export_FApp_HasProjectName);
	REGISTER_FUNC(Export_FApp_IsGame);
	REGISTER_FUNC(Export_FApp_IsProjectNameEmpty);
	REGISTER_FUNC(Export_FApp_SetProjectName);
	REGISTER_FUNC(Export_FApp_AuthorizeUser);
	REGISTER_FUNC(Export_FApp_DenyAllUsers);
	REGISTER_FUNC(Export_FApp_DenyUser);
	REGISTER_FUNC(Export_FApp_GetInstanceId);
	REGISTER_FUNC(Export_FApp_GetInstanceName);
	REGISTER_FUNC(Export_FApp_GetSessionId);
	REGISTER_FUNC(Export_FApp_GetSessionName);
	REGISTER_FUNC(Export_FApp_GetSessionOwner);
	REGISTER_FUNC(Export_FApp_InitializeSession);
	REGISTER_FUNC(Export_FApp_IsAuthorizedUser);
	REGISTER_FUNC(Export_FApp_IsStandalone);
	REGISTER_FUNC(Export_FApp_IsThisInstance);
	REGISTER_FUNC(Export_FApp_SetSessionName);
	REGISTER_FUNC(Export_FApp_SetSessionOwner);
	REGISTER_FUNC(Export_FApp_CanEverRender);
	REGISTER_FUNC(Export_FApp_IsInstalled);
	REGISTER_FUNC(Export_FApp_IsEngineInstalled);
	REGISTER_FUNC(Export_FApp_IsUnattended);
	REGISTER_FUNC(Export_FApp_ShouldUseThreadingForPerformance);
	REGISTER_FUNC(Export_FApp_IsBenchmarking);
	REGISTER_FUNC(Export_FApp_SetBenchmarking);
	REGISTER_FUNC(Export_FApp_GetFixedDeltaTime);
	REGISTER_FUNC(Export_FApp_SetFixedDeltaTime);
	REGISTER_FUNC(Export_FApp_UseFixedTimeStep);
	REGISTER_FUNC(Export_FApp_SetUseFixedTimeStep);
	REGISTER_FUNC(Export_FApp_GetCurrentTime);
	REGISTER_FUNC(Export_FApp_SetCurrentTime);
	REGISTER_FUNC(Export_FApp_GetLastTime);
	REGISTER_FUNC(Export_FApp_UpdateLastTime);
	REGISTER_FUNC(Export_FApp_GetDeltaTime);
	REGISTER_FUNC(Export_FApp_SetDeltaTime);
	REGISTER_FUNC(Export_FApp_GetIdleTime);
	REGISTER_FUNC(Export_FApp_SetIdleTime);
	REGISTER_FUNC(Export_FApp_GetVolumeMultiplier);
	REGISTER_FUNC(Export_FApp_SetVolumeMultiplier);
	REGISTER_FUNC(Export_FApp_GetUnfocusedVolumeMultiplier);
	REGISTER_FUNC(Export_FApp_SetUnfocusedVolumeMultiplier);
	REGISTER_FUNC(Export_FApp_SetUseVRFocus);
	REGISTER_FUNC(Export_FApp_UseVRFocus);
	REGISTER_FUNC(Export_FApp_SetHasVRFocus);
	REGISTER_FUNC(Export_FApp_HasVRFocus);
	REGISTER_FUNC(Export_FApp_Get_bUseFixedSeed);
	REGISTER_FUNC(Export_FApp_Set_bUseFixedSeed);
}
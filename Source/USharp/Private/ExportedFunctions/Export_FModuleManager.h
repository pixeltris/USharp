CSEXPORT FModuleManager& CSCONV Export_FModuleManager_Get()
{
	return FModuleManager::Get();
}

CSEXPORT void CSCONV Export_FModuleManager_AbandonModule(FModuleManager* instance, const FName& InModuleName)
{
	instance->AbandonModule(InModuleName);
}

CSEXPORT void CSCONV Export_FModuleManager_AddModule(FModuleManager* instance, const FName& InModuleName)
{
	instance->AddModule(InModuleName);
}

CSEXPORT IModuleInterface* CSCONV Export_FModuleManager_GetModule(FModuleManager* instance, const FName& InModuleName)
{
	return instance->GetModule(InModuleName);
}

CSEXPORT csbool CSCONV Export_FModuleManager_IsModuleLoaded(FModuleManager* instance, const FName& InModuleName)
{
	return instance->IsModuleLoaded(InModuleName);
}

CSEXPORT IModuleInterface* CSCONV Export_FModuleManager_LoadModule(FModuleManager* instance, const FName& InModuleName)
{
	return instance->LoadModule(InModuleName);
}

CSEXPORT IModuleInterface& CSCONV Export_FModuleManager_LoadModuleChecked(FModuleManager* instance, const FName& InModuleName)
{
	return instance->LoadModuleChecked(InModuleName);
}

CSEXPORT csbool CSCONV Export_FModuleManager_LoadModuleWithCallback(FModuleManager* instance, const FName& InModuleName, FOutputDevice *ArPtr)
{
	// Allow the output device device to be null and use a temp output device if so
	if (ArPtr == nullptr)
	{
		FOutputDeviceDebug Ar;
		return instance->LoadModuleWithCallback(InModuleName, Ar);
	}
	else
	{
		return instance->LoadModuleWithCallback(InModuleName, *ArPtr);
	}
}

CSEXPORT IModuleInterface* CSCONV Export_FModuleManager_LoadModuleWithFailureReason(FModuleManager* instance, const FName& InModuleName, EModuleLoadResult& OutFailureReason)
{
	return instance->LoadModuleWithFailureReason(InModuleName, OutFailureReason);
}

CSEXPORT csbool CSCONV Export_FModuleManager_QueryModule(FModuleManager* instance, const FName& InModuleName, FModuleStatusInterop& OutModuleStatus)
{
	FModuleStatus OutModuleStatusNative;
	csbool Result = instance->QueryModule(InModuleName, OutModuleStatusNative);
	OutModuleStatus = FModuleStatusInterop::FromNative(OutModuleStatusNative);
	return Result;
}

CSEXPORT void CSCONV Export_FModuleManager_QueryModules(FModuleManager* instance, TArray<FModuleStatusInterop>& OutModuleStatuses)
{
	TArray<FModuleStatus> OutModuleStatusNative;
	instance->QueryModules(OutModuleStatusNative);
	for(FModuleStatus& ModuleStatus : OutModuleStatusNative)
	{
		OutModuleStatuses.Add(FModuleStatusInterop::FromNative(ModuleStatus));
	}
}

CSEXPORT void CSCONV Export_FModuleManager_FindModules(FModuleManager* instance, const FString& WildcardWithoutExtension, TArray<FName>& OutModules)
{
	instance->FindModules(*WildcardWithoutExtension, OutModules);
}

CSEXPORT csbool CSCONV Export_FModuleManager_ModuleExists(FModuleManager* instance, const FString& ModuleName)
{
	return instance->ModuleExists(*ModuleName);
}

CSEXPORT int32 CSCONV Export_FModuleManager_GetModuleCount(FModuleManager* instance)
{
	return instance->GetModuleCount();
}

CSEXPORT void CSCONV Export_FModuleManager_StartProcessingNewlyLoadedObjects(FModuleManager* instance)
{
	return instance->StartProcessingNewlyLoadedObjects();
}

CSEXPORT void CSCONV Export_FModuleManager_AddBinariesDirectory(FModuleManager* instance, const FString& InDirectory, csbool bIsGameDirectory)
{
	instance->AddBinariesDirectory(*InDirectory, !!bIsGameDirectory);
}

CSEXPORT void CSCONV Export_FModuleManager_SetGameBinariesDirectory(FModuleManager* instance, const FString& InDirectory)
{
	instance->SetGameBinariesDirectory(*InDirectory);
}

CSEXPORT void CSCONV Export_FModuleManager_GetGameBinariesDirectory(FModuleManager* instance, FString& result)
{
	result = instance->GetGameBinariesDirectory();
}

#if !IS_MONOLITHIC
CSEXPORT csbool CSCONV Export_FModuleManager_IsModuleUpToDate(FModuleManager* instance, const FName& InModuleName)
{
	return instance->IsModuleUpToDate(InModuleName);
}
#endif

CSEXPORT csbool CSCONV Export_FModuleManager_DoesLoadedModuleHaveUObjects(FModuleManager* instance, const FName& ModuleName)
{
	return instance->DoesLoadedModuleHaveUObjects(ModuleName);
}

#if !IS_MONOLITHIC
CSEXPORT void CSCONV Export_FModuleManager_GetModuleFilename(FModuleManager* instance, const FName& ModuleName, FString& result)
{
	result = instance->GetModuleFilename(ModuleName);
}
#endif

CSEXPORT void CSCONV Export_FModuleManager_Reg_ModulesChanged(FModuleManager* instance, void(CSCONV *handler)(const FName&, EModuleChangeReason), FDelegateHandle* handle, csbool enable)
{
	// Changing the signature as we can't use FName directly due to marshaling issues
	REGISTER_LAMBDA(FModuleManager::Get().OnModulesChanged(),
		[handler](FName ModuleName, EModuleChangeReason Reason)
		{
			handler(ModuleName, Reason);
		});
}

CSEXPORT void CSCONV Export_FModuleManager_Reg_ProcessLoadedObjectsHandler(FModuleManager* instance, void(CSCONV *handler)(), FDelegateHandle* handle, csbool enable)
{
	//REGISTER_DELEGATE(FModuleManager::Get().OnProcessLoadedObjectsCallback());
	REGISTER_LAMBDA_SIMPLE(FModuleManager::Get().OnProcessLoadedObjectsCallback());
}

CSEXPORT void CSCONV Export_FModuleManager(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FModuleManager_Get);
	REGISTER_FUNC(Export_FModuleManager_AbandonModule);
	REGISTER_FUNC(Export_FModuleManager_AddModule);
	REGISTER_FUNC(Export_FModuleManager_GetModule);
	REGISTER_FUNC(Export_FModuleManager_IsModuleLoaded);
	REGISTER_FUNC(Export_FModuleManager_LoadModule);
	REGISTER_FUNC(Export_FModuleManager_LoadModuleChecked);
	REGISTER_FUNC(Export_FModuleManager_LoadModuleWithCallback);
	REGISTER_FUNC(Export_FModuleManager_LoadModuleWithFailureReason);
	REGISTER_FUNC(Export_FModuleManager_QueryModule);
	REGISTER_FUNC(Export_FModuleManager_QueryModules);
	REGISTER_FUNC(Export_FModuleManager_FindModules);
	REGISTER_FUNC(Export_FModuleManager_ModuleExists);
	REGISTER_FUNC(Export_FModuleManager_GetModuleCount);
	REGISTER_FUNC(Export_FModuleManager_StartProcessingNewlyLoadedObjects);
	REGISTER_FUNC(Export_FModuleManager_AddBinariesDirectory);
	REGISTER_FUNC(Export_FModuleManager_SetGameBinariesDirectory);
	REGISTER_FUNC(Export_FModuleManager_GetGameBinariesDirectory);
#if !IS_MONOLITHIC
	REGISTER_FUNC(Export_FModuleManager_IsModuleUpToDate);
#endif
	REGISTER_FUNC(Export_FModuleManager_DoesLoadedModuleHaveUObjects);
#if !IS_MONOLITHIC
	REGISTER_FUNC(Export_FModuleManager_GetModuleFilename);
#endif
	REGISTER_FUNC(Export_FModuleManager_Reg_ModulesChanged);
	REGISTER_FUNC(Export_FModuleManager_Reg_ProcessLoadedObjectsHandler);	
}
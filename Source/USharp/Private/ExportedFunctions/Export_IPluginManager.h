CSEXPORT IPluginManager& CSCONV Export_IPluginManager_Get()
{
	return IPluginManager::Get();
}

CSEXPORT void CSCONV Export_IPluginManager_RefreshPluginsList(IPluginManager* instance)
{
	instance->RefreshPluginsList();
}

CSEXPORT csbool CSCONV Export_IPluginManager_LoadModulesForEnabledPlugins(IPluginManager* instance, /*ELoadingPhase::Type*/int32 LoadingPhase)
{
	return instance->LoadModulesForEnabledPlugins((ELoadingPhase::Type)LoadingPhase);
}

CSEXPORT void CSCONV Export_IPluginManager_GetLocalizationPathsForEnabledPlugins(IPluginManager* instance, TArray<FString>& OutLocResPaths)
{
	instance->GetLocalizationPathsForEnabledPlugins(OutLocResPaths);
}

CSEXPORT csbool CSCONV Export_IPluginManager_AreRequiredPluginsAvailable(IPluginManager* instance)
{
	return instance->AreRequiredPluginsAvailable();
}

#if !IS_MONOLITHIC
CSEXPORT csbool CSCONV Export_IPluginManager_CheckModuleCompatibility(IPluginManager* instance, TArray<FString>& OutIncompatibleModules)
{
	return instance->CheckModuleCompatibility(OutIncompatibleModules);
}
#endif

CSEXPORT void CSCONV Export_IPluginManager_FindPlugin(IPluginManager* instance, const FString& Name, TSharedPtr<IPlugin>& result)
{
	result = instance->FindPlugin(Name);
}

CSEXPORT void CSCONV Export_IPluginManager_GetEnabledPlugins(IPluginManager* instance, TArray<TSharedRef<IPlugin>>& result)
{
	result = instance->GetEnabledPlugins();
}

CSEXPORT void CSCONV Export_IPluginManager_GetEnabledPluginsWithContent(IPluginManager* instance, TArray<TSharedRef<IPlugin>>& result)
{
	result = instance->GetEnabledPluginsWithContent();
}

CSEXPORT void CSCONV Export_IPluginManager_GetDiscoveredPlugins(IPluginManager* instance, TArray<TSharedRef<IPlugin>>& result)
{
	result = instance->GetDiscoveredPlugins();
}

CSEXPORT void CSCONV Export_IPluginManager_AddPluginSearchPath(IPluginManager* instance, const FString& ExtraDiscoveryPath, csbool bRefresh)
{
	instance->AddPluginSearchPath(ExtraDiscoveryPath, bRefresh);
}

CSEXPORT void CSCONV Export_IPluginManager_GetPluginsWithPakFile(IPluginManager* instance, TArray<TSharedRef<IPlugin>>& result)
{
	result = instance->GetPluginsWithPakFile();
}

CSEXPORT void CSCONV Export_IPluginManager_MountNewlyCreatedPlugin(IPluginManager* instance, const FString& PluginName)
{
	instance->MountNewlyCreatedPlugin(PluginName);
}

CSEXPORT void CSCONV Export_IPluginManager(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_IPluginManager_Get);
	REGISTER_FUNC(Export_IPluginManager_RefreshPluginsList);
	REGISTER_FUNC(Export_IPluginManager_LoadModulesForEnabledPlugins);
	REGISTER_FUNC(Export_IPluginManager_GetLocalizationPathsForEnabledPlugins);
	REGISTER_FUNC(Export_IPluginManager_AreRequiredPluginsAvailable);
#if !IS_MONOLITHIC
	REGISTER_FUNC(Export_IPluginManager_CheckModuleCompatibility);
#endif
	REGISTER_FUNC(Export_IPluginManager_FindPlugin);
	REGISTER_FUNC(Export_IPluginManager_GetEnabledPlugins);
	REGISTER_FUNC(Export_IPluginManager_GetEnabledPluginsWithContent);
	REGISTER_FUNC(Export_IPluginManager_GetDiscoveredPlugins);
	REGISTER_FUNC(Export_IPluginManager_AddPluginSearchPath);
	REGISTER_FUNC(Export_IPluginManager_GetPluginsWithPakFile);
	REGISTER_FUNC(Export_IPluginManager_MountNewlyCreatedPlugin);
}
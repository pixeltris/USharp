CSEXPORT void CSCONV Export_IPlugin_GetName(IPlugin* instance, FString& result)
{
	result = instance->GetName();
}

CSEXPORT void CSCONV Export_IPlugin_GetDescriptorFileName(IPlugin* instance, FString& result)
{
	result = instance->GetDescriptorFileName();
}

CSEXPORT void CSCONV Export_IPlugin_GetBaseDir(IPlugin* instance, FString& result)
{
	result = instance->GetBaseDir();
}

CSEXPORT void CSCONV Export_IPlugin_GetContentDir(IPlugin* instance, FString& result)
{
	result = instance->GetContentDir();
}

CSEXPORT void CSCONV Export_IPlugin_GetMountedAssetPath(IPlugin* instance, FString& result)
{
	result = instance->GetMountedAssetPath();
}

CSEXPORT /*EPluginType*/int32 CSCONV Export_IPlugin_GetType(IPlugin* instance)
{
	return (int32)instance->GetType();
}

CSEXPORT csbool CSCONV Export_IPlugin_IsEnabled(IPlugin* instance)
{
	return instance->IsEnabled();
}

CSEXPORT csbool CSCONV Export_IPlugin_IsEnabledByDefault(IPlugin* instance)
{
	return instance->IsEnabledByDefault();
}

CSEXPORT csbool CSCONV Export_IPlugin_IsHidden(IPlugin* instance)
{
	return instance->IsHidden();
}

CSEXPORT csbool CSCONV Export_IPlugin_CanContainContent(IPlugin* instance)
{
	return instance->CanContainContent();
}

CSEXPORT /*EPluginLoadedFrom*/int32 CSCONV Export_IPlugin_GetLoadedFrom(IPlugin* instance)
{
	return (int32)instance->GetLoadedFrom();
}

CSEXPORT void CSCONV Export_IPlugin(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_IPlugin_GetName);
	REGISTER_FUNC(Export_IPlugin_GetDescriptorFileName);
	REGISTER_FUNC(Export_IPlugin_GetBaseDir);
	REGISTER_FUNC(Export_IPlugin_GetContentDir);
	REGISTER_FUNC(Export_IPlugin_GetMountedAssetPath);
	REGISTER_FUNC(Export_IPlugin_GetType);
	REGISTER_FUNC(Export_IPlugin_IsEnabled);
	REGISTER_FUNC(Export_IPlugin_IsEnabledByDefault);
	REGISTER_FUNC(Export_IPlugin_IsHidden);
	REGISTER_FUNC(Export_IPlugin_CanContainContent);
	REGISTER_FUNC(Export_IPlugin_GetLoadedFrom);
}
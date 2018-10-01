CSEXPORT void CSCONV Export_FPlatformProperties_GetPhysicsFormat(FString& result)
{
	result = FPlatformProperties::GetPhysicsFormat();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_HasEditorOnlyData()
{
	return FPlatformProperties::HasEditorOnlyData();
}

CSEXPORT void CSCONV Export_FPlatformProperties_IniPlatformName(FString& result)
{
	result = FPlatformProperties::IniPlatformName();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_IsGameOnly()
{
	return FPlatformProperties::IsGameOnly();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_IsServerOnly()
{
	return FPlatformProperties::IsServerOnly();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_IsClientOnly()
{
	return FPlatformProperties::IsClientOnly();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_IsMonolithicBuild()
{
	return FPlatformProperties::IsMonolithicBuild();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_IsProgram()
{
	return FPlatformProperties::IsProgram();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_IsLittleEndian()
{
	return FPlatformProperties::IsLittleEndian();
}

CSEXPORT void CSCONV Export_FPlatformProperties_PlatformName(FString& result)
{
	result = FPlatformProperties::PlatformName();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_RequiresCookedData()
{
	return FPlatformProperties::RequiresCookedData();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_SupportsBuildTarget(int32 BuildTarget)
{
	return FPlatformProperties::SupportsBuildTarget((EBuildTargets::Type)BuildTarget);
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_SupportsAutoSDK()
{
	return FPlatformProperties::SupportsAutoSDK();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_SupportsGrayscaleSRGB()
{
	return FPlatformProperties::SupportsGrayscaleSRGB();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_SupportsMultipleGameInstances()
{
	return FPlatformProperties::SupportsMultipleGameInstances();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_SupportsTessellation()
{
	return FPlatformProperties::SupportsTessellation();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_SupportsWindowedMode()
{
	return FPlatformProperties::SupportsWindowedMode();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_AllowsFramerateSmoothing()
{
	return FPlatformProperties::AllowsFramerateSmoothing();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_SupportsAudioStreaming()
{
	return FPlatformProperties::SupportsAudioStreaming();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_SupportsHighQualityLightmaps()
{
	return FPlatformProperties::SupportsHighQualityLightmaps();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_SupportsLowQualityLightmaps()
{
	return FPlatformProperties::SupportsLowQualityLightmaps();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_SupportsDistanceFieldShadows()
{
	return FPlatformProperties::SupportsDistanceFieldShadows();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_SupportsTextureStreaming()
{
	return FPlatformProperties::SupportsTextureStreaming();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_HasFixedResolution()
{
	return FPlatformProperties::HasFixedResolution();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_SupportsMinimize()
{
	return FPlatformProperties::SupportsMinimize();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_SupportsQuit()
{
	return FPlatformProperties::SupportsQuit();
}

CSEXPORT csbool CSCONV Export_FPlatformProperties_AllowsCallStackDumpDuringAssert()
{
	return FPlatformProperties::AllowsCallStackDumpDuringAssert();
}

CSEXPORT void CSCONV Export_FPlatformProperties(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FPlatformProperties_GetPhysicsFormat);
	REGISTER_FUNC(Export_FPlatformProperties_HasEditorOnlyData);
	REGISTER_FUNC(Export_FPlatformProperties_IniPlatformName);
	REGISTER_FUNC(Export_FPlatformProperties_IsGameOnly);
	REGISTER_FUNC(Export_FPlatformProperties_IsServerOnly);
	REGISTER_FUNC(Export_FPlatformProperties_IsClientOnly);
	REGISTER_FUNC(Export_FPlatformProperties_IsMonolithicBuild);
	REGISTER_FUNC(Export_FPlatformProperties_IsProgram);
	REGISTER_FUNC(Export_FPlatformProperties_IsLittleEndian);
	REGISTER_FUNC(Export_FPlatformProperties_PlatformName);
	REGISTER_FUNC(Export_FPlatformProperties_RequiresCookedData);
	REGISTER_FUNC(Export_FPlatformProperties_SupportsBuildTarget);
	REGISTER_FUNC(Export_FPlatformProperties_SupportsAutoSDK);
	REGISTER_FUNC(Export_FPlatformProperties_SupportsGrayscaleSRGB);
	REGISTER_FUNC(Export_FPlatformProperties_SupportsMultipleGameInstances);
	REGISTER_FUNC(Export_FPlatformProperties_SupportsTessellation);
	REGISTER_FUNC(Export_FPlatformProperties_SupportsWindowedMode);
	REGISTER_FUNC(Export_FPlatformProperties_AllowsFramerateSmoothing);
	REGISTER_FUNC(Export_FPlatformProperties_SupportsAudioStreaming);
	REGISTER_FUNC(Export_FPlatformProperties_SupportsHighQualityLightmaps);
	REGISTER_FUNC(Export_FPlatformProperties_SupportsLowQualityLightmaps);
	REGISTER_FUNC(Export_FPlatformProperties_SupportsDistanceFieldShadows);
	REGISTER_FUNC(Export_FPlatformProperties_SupportsTextureStreaming);
	REGISTER_FUNC(Export_FPlatformProperties_HasFixedResolution);
	REGISTER_FUNC(Export_FPlatformProperties_SupportsMinimize);
	REGISTER_FUNC(Export_FPlatformProperties_SupportsQuit);
	REGISTER_FUNC(Export_FPlatformProperties_AllowsCallStackDumpDuringAssert);
}
CSEXPORT csbool CSCONV Export_FPaths_ShouldSaveToUserDir()
{
	return FPaths::ShouldSaveToUserDir();
}

CSEXPORT void CSCONV Export_FPaths_LaunchDir(FString& result)
{
	result = FPaths::LaunchDir();
}

CSEXPORT void CSCONV Export_FPaths_EngineDir(FString& result)
{
	result = FPaths::EngineDir();
}

CSEXPORT void CSCONV Export_FPaths_EngineUserDir(FString& result)
{
	result = FPaths::EngineUserDir();
}

CSEXPORT void CSCONV Export_FPaths_EngineVersionAgnosticUserDir(FString& result)
{
	result = FPaths::EngineVersionAgnosticUserDir();
}

CSEXPORT void CSCONV Export_FPaths_EngineContentDir(FString& result)
{
	result = FPaths::EngineContentDir();
}

CSEXPORT void CSCONV Export_FPaths_EngineConfigDir(FString& result)
{
	result = FPaths::EngineConfigDir();
}

CSEXPORT void CSCONV Export_FPaths_EngineIntermediateDir(FString& result)
{
	result = FPaths::EngineIntermediateDir();
}

CSEXPORT void CSCONV Export_FPaths_EngineSavedDir(FString& result)
{
	result = FPaths::EngineSavedDir();
}

CSEXPORT void CSCONV Export_FPaths_EnginePluginsDir(FString& result)
{
	result = FPaths::EnginePluginsDir();
}

CSEXPORT void CSCONV Export_FPaths_RootDir(FString& result)
{
	result = FPaths::RootDir();
}

CSEXPORT void CSCONV Export_FPaths_ProjectDir(FString& result)
{
	result = FPaths::ProjectDir();
}

CSEXPORT void CSCONV Export_FPaths_ProjectUserDir(FString& result)
{
	result = FPaths::ProjectUserDir();
}

CSEXPORT void CSCONV Export_FPaths_ProjectContentDir(FString& result)
{
	result = FPaths::ProjectContentDir();
}

CSEXPORT void CSCONV Export_FPaths_ProjectConfigDir(FString& result)
{
	result = FPaths::ProjectConfigDir();
}

CSEXPORT void CSCONV Export_FPaths_ProjectSavedDir(FString& result)
{
	result = FPaths::ProjectSavedDir();
}

CSEXPORT void CSCONV Export_FPaths_ProjectIntermediateDir(FString& result)
{
	result = FPaths::ProjectIntermediateDir();
}

CSEXPORT void CSCONV Export_FPaths_ProjectPluginsDir(FString& result)
{
	result = FPaths::ProjectPluginsDir();
}

CSEXPORT void CSCONV Export_FPaths_ProjectPersistentDownloadDir(FString& result)
{
	result = FPaths::ProjectPersistentDownloadDir();
}

CSEXPORT void CSCONV Export_FPaths_SourceConfigDir(FString& result)
{
	result = FPaths::SourceConfigDir();
}

CSEXPORT void CSCONV Export_FPaths_GeneratedConfigDir(FString& result)
{
	result = FPaths::GeneratedConfigDir();
}

CSEXPORT void CSCONV Export_FPaths_SandboxesDir(FString& result)
{
	result = FPaths::SandboxesDir();
}

CSEXPORT void CSCONV Export_FPaths_ProfilingDir(FString& result)
{
	result = FPaths::ProfilingDir();
}

CSEXPORT void CSCONV Export_FPaths_ScreenShotDir(FString& result)
{
	result = FPaths::ScreenShotDir();
}

CSEXPORT void CSCONV Export_FPaths_BugItDir(FString& result)
{
	result = FPaths::BugItDir();
}

CSEXPORT void CSCONV Export_FPaths_VideoCaptureDir(FString& result)
{
	result = FPaths::VideoCaptureDir();
}

CSEXPORT void CSCONV Export_FPaths_ProjectLogDir(FString& result)
{
	result = FPaths::ProjectLogDir();
}

CSEXPORT void CSCONV Export_FPaths_AutomationDir(FString& result)
{
	result = FPaths::AutomationDir();
}

CSEXPORT void CSCONV Export_FPaths_AutomationTransientDir(FString& result)
{
	result = FPaths::AutomationTransientDir();
}

CSEXPORT void CSCONV Export_FPaths_AutomationLogDir(FString& result)
{
	result = FPaths::AutomationLogDir();
}

CSEXPORT void CSCONV Export_FPaths_CloudDir(FString& result)
{
	result = FPaths::CloudDir();
}

CSEXPORT void CSCONV Export_FPaths_GameDevelopersDir(FString& result)
{
	result = FPaths::GameDevelopersDir();
}

CSEXPORT void CSCONV Export_FPaths_GameUserDeveloperDir(FString& result)
{
	result = FPaths::GameUserDeveloperDir();
}

CSEXPORT void CSCONV Export_FPaths_DiffDir(FString& result)
{
	result = FPaths::DiffDir();
}

CSEXPORT const TArray<FString>& CSCONV Export_FPaths_GetEngineLocalizationPaths()
{
	return FPaths::GetEngineLocalizationPaths();
}

CSEXPORT const TArray<FString>& CSCONV Export_FPaths_GetEditorLocalizationPaths()
{
	return FPaths::GetEditorLocalizationPaths();
}

CSEXPORT const TArray<FString>& CSCONV Export_FPaths_GetPropertyNameLocalizationPaths()
{
	return FPaths::GetPropertyNameLocalizationPaths();
}

CSEXPORT const TArray<FString>& CSCONV Export_FPaths_GetToolTipLocalizationPaths()
{
	return FPaths::GetToolTipLocalizationPaths();
}

CSEXPORT const TArray<FString>& CSCONV Export_FPaths_GetGameLocalizationPaths()
{
	return FPaths::GetGameLocalizationPaths();
}

CSEXPORT void CSCONV Export_FPaths_GameAgnosticSavedDir(FString& result)
{
	result = FPaths::GameAgnosticSavedDir();
}

CSEXPORT void CSCONV Export_FPaths_EngineSourceDir(FString& result)
{
	result = FPaths::EngineSourceDir();
}

CSEXPORT void CSCONV Export_FPaths_GameSourceDir(FString& result)
{
	result = FPaths::GameSourceDir();
}

CSEXPORT void CSCONV Export_FPaths_FeaturePackDir(FString& result)
{
	result = FPaths::FeaturePackDir();
}

CSEXPORT csbool CSCONV Export_FPaths_IsProjectFilePathSet()
{
	return FPaths::IsProjectFilePathSet();
}

CSEXPORT void CSCONV Export_FPaths_GetProjectFilePath(FString& result)
{
	result = FPaths::GetProjectFilePath();
}

CSEXPORT void CSCONV Export_FPaths_SetProjectFilePath(const FString& NewGameProjectFilePath)
{
	FPaths::SetProjectFilePath(NewGameProjectFilePath);
}

CSEXPORT void CSCONV Export_FPaths_GetExtension(const FString& InPath, csbool bIncludeDot, FString& result)
{
	result = FPaths::GetExtension(InPath, !!bIncludeDot);
}

CSEXPORT void CSCONV Export_FPaths_GetCleanFilename(const FString& InPath, FString& result)
{
	result = FPaths::GetCleanFilename(InPath);
}

CSEXPORT void CSCONV Export_FPaths_GetBaseFilename(const FString& InPath, csbool bRemovePath, FString& result)
{
	result = FPaths::GetBaseFilename(InPath, !!bRemovePath);
}

CSEXPORT void CSCONV Export_FPaths_GetPath(const FString& InPath, FString& result)
{
	result = FPaths::GetPath(InPath);
}

CSEXPORT void CSCONV Export_FPaths_ChangeExtension(const FString& InPath, const FString& InNewExtension, FString& result)
{
	result = FPaths::ChangeExtension(InPath, InNewExtension);
}

CSEXPORT csbool CSCONV Export_FPaths_FileExists(const FString& InPath)
{
	return FPaths::FileExists(InPath);
}

CSEXPORT csbool CSCONV Export_FPaths_DirectoryExists(const FString& InPath)
{
	return FPaths::DirectoryExists(InPath);
}

CSEXPORT csbool CSCONV Export_FPaths_IsDrive(const FString& InPath)
{
	return FPaths::IsDrive(InPath);
}

CSEXPORT csbool CSCONV Export_FPaths_IsRelative(const FString& InPath)
{
	return FPaths::IsRelative(InPath);
}

CSEXPORT void CSCONV Export_FPaths_NormalizeFilename(FString& InPath)
{
	FPaths::NormalizeFilename(InPath);
}

CSEXPORT csbool CSCONV Export_FPaths_IsSamePath(const FString& PathA, const FString& PathB)
{
	return FPaths::IsSamePath(PathA, PathB);
}

CSEXPORT void CSCONV Export_FPaths_NormalizeDirectoryName(FString& InPath)
{
	FPaths::NormalizeDirectoryName(InPath);
}

CSEXPORT csbool CSCONV Export_FPaths_CollapseRelativeDirectories(FString& InPath)
{
	return FPaths::CollapseRelativeDirectories(InPath);
}

CSEXPORT void CSCONV Export_FPaths_RemoveDuplicateSlashes(FString& InPath)
{
	FPaths::RemoveDuplicateSlashes(InPath);
}

CSEXPORT void CSCONV Export_FPaths_MakeStandardFilename(FString& InPath)
{
	FPaths::MakeStandardFilename(InPath);
}

CSEXPORT void CSCONV Export_FPaths_MakePlatformFilename(FString& InPath)
{
	FPaths::MakePlatformFilename(InPath);
}

CSEXPORT csbool CSCONV Export_FPaths_MakePathRelativeTo(FString& InPath, const FString& InRelativeTo)
{
	return FPaths::MakePathRelativeTo(InPath, *InRelativeTo);
}

CSEXPORT void CSCONV Export_FPaths_ConvertRelativePathToFull(const FString& InPath, FString& result)
{
	result = FPaths::ConvertRelativePathToFull(InPath);
}

CSEXPORT void CSCONV Export_FPaths_ConvertRelativePathToFullBase(const FString& BasePath, const FString& InPath, FString& result)
{
	result = FPaths::ConvertRelativePathToFull(BasePath, InPath);
}

CSEXPORT void CSCONV Export_FPaths_ConvertToSandboxPath(const FString& InPath, const FString& InSandboxName, FString& result)
{
	result = FPaths::ConvertToSandboxPath(InPath, *InSandboxName);
}

CSEXPORT void CSCONV Export_FPaths_ConvertFromSandboxPath(const FString& InPath, const FString& InSandboxName, FString& result)
{
	result = FPaths::ConvertFromSandboxPath(InPath, *InSandboxName);
}

CSEXPORT void CSCONV Export_FPaths_CreateTempFilename(const FString& Path, const FString& Prefix, const FString& Extension, FString& result)
{
	result = FPaths::CreateTempFilename(*Path, *Prefix, *Extension);
}

CSEXPORT csbool CSCONV Export_FPaths_ValidatePath(const FString& InPath, FString& OutReason)
{
	FText OutReasonText;
	csbool Result = FPaths::ValidatePath(InPath, &OutReasonText);
	OutReason = OutReasonText.ToString();
	return Result;
}

CSEXPORT void CSCONV Export_FPaths_Split(const FString& InPath, FString& PathPart, FString& FilenamePart, FString& ExtensionPart)
{
	return FPaths::Split(InPath, PathPart, FilenamePart, ExtensionPart);
}

CSEXPORT void CSCONV Export_FPaths_GetRelativePathToRoot(FString& result)
{
	result = FPaths::GetRelativePathToRoot();
}

CSEXPORT void CSCONV Export_FPaths_Combine(const FString& PathA, const FString& PathB, FString& result)
{
	result = FPaths::Combine(*PathA, *PathB);
}

CSEXPORT void CSCONV Export_FPaths(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FPaths_ShouldSaveToUserDir);
	REGISTER_FUNC(Export_FPaths_LaunchDir);
	REGISTER_FUNC(Export_FPaths_EngineDir);
	REGISTER_FUNC(Export_FPaths_EngineUserDir);
	REGISTER_FUNC(Export_FPaths_EngineVersionAgnosticUserDir);
	REGISTER_FUNC(Export_FPaths_EngineContentDir);
	REGISTER_FUNC(Export_FPaths_EngineConfigDir);
	REGISTER_FUNC(Export_FPaths_EngineIntermediateDir);
	REGISTER_FUNC(Export_FPaths_EngineSavedDir);
	REGISTER_FUNC(Export_FPaths_EnginePluginsDir);
	REGISTER_FUNC(Export_FPaths_RootDir);
	REGISTER_FUNC(Export_FPaths_ProjectDir);
	REGISTER_FUNC(Export_FPaths_ProjectUserDir);
	REGISTER_FUNC(Export_FPaths_ProjectContentDir);
	REGISTER_FUNC(Export_FPaths_ProjectConfigDir);
	REGISTER_FUNC(Export_FPaths_ProjectSavedDir);
	REGISTER_FUNC(Export_FPaths_ProjectIntermediateDir);
	REGISTER_FUNC(Export_FPaths_ProjectPluginsDir);
	REGISTER_FUNC(Export_FPaths_ProjectPersistentDownloadDir);
	REGISTER_FUNC(Export_FPaths_SourceConfigDir);
	REGISTER_FUNC(Export_FPaths_GeneratedConfigDir);
	REGISTER_FUNC(Export_FPaths_SandboxesDir);
	REGISTER_FUNC(Export_FPaths_ProfilingDir);
	REGISTER_FUNC(Export_FPaths_ScreenShotDir);
	REGISTER_FUNC(Export_FPaths_BugItDir);
	REGISTER_FUNC(Export_FPaths_VideoCaptureDir);
	REGISTER_FUNC(Export_FPaths_ProjectLogDir);
	REGISTER_FUNC(Export_FPaths_AutomationDir);
	REGISTER_FUNC(Export_FPaths_AutomationTransientDir);
	REGISTER_FUNC(Export_FPaths_AutomationLogDir);
	REGISTER_FUNC(Export_FPaths_CloudDir);
	REGISTER_FUNC(Export_FPaths_GameDevelopersDir);
	REGISTER_FUNC(Export_FPaths_GameUserDeveloperDir);
	REGISTER_FUNC(Export_FPaths_DiffDir);
	REGISTER_FUNC(Export_FPaths_GetEngineLocalizationPaths);
	REGISTER_FUNC(Export_FPaths_GetEditorLocalizationPaths);
	REGISTER_FUNC(Export_FPaths_GetPropertyNameLocalizationPaths);
	REGISTER_FUNC(Export_FPaths_GetToolTipLocalizationPaths);
	REGISTER_FUNC(Export_FPaths_GetGameLocalizationPaths);
	REGISTER_FUNC(Export_FPaths_GameAgnosticSavedDir);
	REGISTER_FUNC(Export_FPaths_EngineSourceDir);
	REGISTER_FUNC(Export_FPaths_GameSourceDir);
	REGISTER_FUNC(Export_FPaths_FeaturePackDir);
	REGISTER_FUNC(Export_FPaths_IsProjectFilePathSet);
	REGISTER_FUNC(Export_FPaths_GetProjectFilePath);
	REGISTER_FUNC(Export_FPaths_SetProjectFilePath);
	REGISTER_FUNC(Export_FPaths_GetExtension);
	REGISTER_FUNC(Export_FPaths_GetCleanFilename);
	REGISTER_FUNC(Export_FPaths_GetBaseFilename);
	REGISTER_FUNC(Export_FPaths_GetPath);
	REGISTER_FUNC(Export_FPaths_ChangeExtension);
	REGISTER_FUNC(Export_FPaths_FileExists);
	REGISTER_FUNC(Export_FPaths_DirectoryExists);
	REGISTER_FUNC(Export_FPaths_IsDrive);
	REGISTER_FUNC(Export_FPaths_IsRelative);
	REGISTER_FUNC(Export_FPaths_NormalizeFilename);
	REGISTER_FUNC(Export_FPaths_IsSamePath);
	REGISTER_FUNC(Export_FPaths_NormalizeDirectoryName);
	REGISTER_FUNC(Export_FPaths_CollapseRelativeDirectories);
	REGISTER_FUNC(Export_FPaths_RemoveDuplicateSlashes);
	REGISTER_FUNC(Export_FPaths_MakeStandardFilename);
	REGISTER_FUNC(Export_FPaths_MakePlatformFilename);
	REGISTER_FUNC(Export_FPaths_MakePathRelativeTo);
	REGISTER_FUNC(Export_FPaths_ConvertRelativePathToFull);
	REGISTER_FUNC(Export_FPaths_ConvertRelativePathToFullBase);
	REGISTER_FUNC(Export_FPaths_ConvertToSandboxPath);
	REGISTER_FUNC(Export_FPaths_ConvertFromSandboxPath);
	REGISTER_FUNC(Export_FPaths_CreateTempFilename);
	REGISTER_FUNC(Export_FPaths_ValidatePath);
	REGISTER_FUNC(Export_FPaths_Split);
	REGISTER_FUNC(Export_FPaths_GetRelativePathToRoot);
	REGISTER_FUNC(Export_FPaths_Combine);
}
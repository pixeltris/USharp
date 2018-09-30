CSEXPORT void CSCONV Export_FPackageName_ConvertToLongScriptPackageName(const FString& InShortName, FString& result)
{
	result = FPackageName::ConvertToLongScriptPackageName(*InShortName);
}

CSEXPORT void CSCONV Export_FPackageName_RegisterShortPackageNamesForUObjectModules()
{
	FPackageName::RegisterShortPackageNamesForUObjectModules();
}

CSEXPORT csbool CSCONV Export_FPackageName_FindScriptPackageName(const FName& InShortName, FName& OutScriptPackageName)
{
	FName* Result = FPackageName::FindScriptPackageName(InShortName);
	if(Result == nullptr)
	{
		OutScriptPackageName = NAME_None;
		return false;
	}
	OutScriptPackageName = *Result;
	return true;
}

CSEXPORT csbool CSCONV Export_FPackageName_TryConvertFilenameToLongPackageName(const FString& InFilename, FString& OutPackageName, FString* OutFailureReason)
{
	return FPackageName::TryConvertFilenameToLongPackageName(InFilename, OutPackageName, OutFailureReason);
}

CSEXPORT void CSCONV Export_FPackageName_FilenameToLongPackageName(const FString& InFilename, FString& result)
{
	result = FPackageName::FilenameToLongPackageName(InFilename);
}

CSEXPORT csbool CSCONV Export_FPackageName_TryConvertLongPackageNameToFilename(const FString& InLongPackageName, FString& OutFilename, const FString& InExtension)
{
	return FPackageName::TryConvertLongPackageNameToFilename(InLongPackageName, OutFilename, InExtension);
}

CSEXPORT void CSCONV Export_FPackageName_LongPackageNameToFilename(const FString& InLongPackageName, const FString& InExtension, FString& result)
{
	result = FPackageName::LongPackageNameToFilename(InLongPackageName, InExtension);
}

CSEXPORT void CSCONV Export_FPackageName_GetLongPackagePath(const FString& InLongPackageName, FString& result)
{
	result = FPackageName::GetLongPackagePath(InLongPackageName);
}

CSEXPORT csbool CSCONV Export_FPackageName_SplitLongPackageName(const FString& InLongPackageName, FString& OutPackageRoot, FString& OutPackagePath, FString& OutPackageName, const csbool bStripRootLeadingSlash)
{
	return FPackageName::SplitLongPackageName(InLongPackageName, OutPackageRoot, OutPackagePath, OutPackageName, !!bStripRootLeadingSlash);
}

CSEXPORT void CSCONV Export_FPackageName_GetLongPackageAssetName(const FString& InLongPackageName, FString& result)
{
	result = FPackageName::GetLongPackageAssetName(InLongPackageName);
}

CSEXPORT csbool CSCONV Export_FPackageName_IsValidLongPackageName(const FString& InLongPackageName, csbool bIncludeReadOnlyRoots, FString& OutReason)
{
	FText OutReasonText;
	csbool Result = FPackageName::IsValidLongPackageName(InLongPackageName, !!bIncludeReadOnlyRoots, &OutReasonText);
	OutReason = OutReasonText.ToString();
	return Result;
}

CSEXPORT csbool CSCONV Export_FPackageName_IsShortPackageName(const FString& PossiblyLongName)
{
	return FPackageName::IsShortPackageName(PossiblyLongName);
}

CSEXPORT csbool CSCONV Export_FPackageName_IsShortPackageFName(const FName& PossiblyLongName)
{
	return FPackageName::IsShortPackageName(PossiblyLongName);
}

CSEXPORT void CSCONV Export_FPackageName_GetShortNameFromPackage(const UPackage* Package, FString& result)
{
	result = FPackageName::GetShortName(Package);
}

CSEXPORT void CSCONV Export_FPackageName_GetShortNameFromFString(const FString& LongName, FString& result)
{
	result = FPackageName::GetShortName(LongName);
}

CSEXPORT void CSCONV Export_FPackageName_GetShortNameFromFName(const FName& LongName, FString& result)
{
	result = FPackageName::GetShortName(LongName);
}

CSEXPORT void CSCONV Export_FPackageName_GetShortFNameFromFString(const FString& LongName, FName& result)
{
	result = FPackageName::GetShortFName(LongName);
}

CSEXPORT void CSCONV Export_FPackageName_GetShortFNameFromFName(const FName& LongName, FName& result)
{
	result = FPackageName::GetShortFName(LongName);
}

CSEXPORT void CSCONV Export_FPackageName_RegisterMountPoint(const FString& RootPath, const FString& ContentPath)
{
	FPackageName::RegisterMountPoint(RootPath, ContentPath);
}

CSEXPORT void CSCONV Export_FPackageName_UnRegisterMountPoint(const FString& RootPath, const FString& ContentPath)
{
	FPackageName::UnRegisterMountPoint(RootPath, ContentPath);
}

CSEXPORT void CSCONV Export_FPackageName_GetPackageMountPoint(const FString& InPackagePath, FName& result)
{
	result = FPackageName::GetPackageMountPoint(InPackagePath);
}

CSEXPORT csbool CSCONV Export_FPackageName_DoesPackageExist(const FString& LongPackageName, const FGuid* Guid, FString* OutFilename)
{
	return FPackageName::DoesPackageExist(LongPackageName, Guid, OutFilename);
}

CSEXPORT csbool CSCONV Export_FPackageName_SearchForPackageOnDisk(const FString& PackageName, FString* OutLongPackageName, FString* OutFilename)
{
	return FPackageName::SearchForPackageOnDisk(PackageName, OutLongPackageName, OutFilename);
}

CSEXPORT csbool CSCONV Export_FPackageName_TryConvertShortPackagePathToLongInObjectPath(const FString& ObjectPath, FString& ConvertedObjectPath)
{
	return FPackageName::TryConvertShortPackagePathToLongInObjectPath(ObjectPath, ConvertedObjectPath);
}

CSEXPORT void CSCONV Export_FPackageName_GetNormalizedObjectPath(const FString& ObjectPath, FString& result)
{
	result = FPackageName::GetNormalizedObjectPath(ObjectPath);
}

CSEXPORT void CSCONV Export_FPackageName_GetLocalizedPackagePath(const FString& InSourcePackagePath, FString& result)
{
	result = FPackageName::GetLocalizedPackagePath(InSourcePackagePath);
}

CSEXPORT void CSCONV Export_FPackageName_GetLocalizedPackagePathWithCulture(const FString& InSourcePackagePath, const FString& InCultureName, FString& result)
{
	result = FPackageName::GetLocalizedPackagePath(InSourcePackagePath, InCultureName);
}

CSEXPORT void CSCONV Export_FPackageName_GetAssetPackageExtension(FString& result)
{
	result = FPackageName::GetAssetPackageExtension();
}

CSEXPORT void CSCONV Export_FPackageName_GetMapPackageExtension(FString& result)
{
	result = FPackageName::GetMapPackageExtension();
}

CSEXPORT csbool CSCONV Export_FPackageName_IsPackageExtension(const FString& Ext)
{
	return FPackageName::IsPackageExtension(*Ext);
}

CSEXPORT csbool CSCONV Export_FPackageName_IsPackageFilename(const FString& Filename)
{
	return FPackageName::IsPackageFilename(Filename);
}

CSEXPORT csbool CSCONV Export_FPackageName_FindPackagesInDirectory(TArray<FString>& OutPackages, const FString& RootDir)
{
	return FPackageName::FindPackagesInDirectory(OutPackages, RootDir);
}

CSEXPORT void CSCONV Export_FPackageName_QueryRootContentPaths(TArray<FString>& OutRootContentPaths)
{
	FPackageName::QueryRootContentPaths(OutRootContentPaths);
}

CSEXPORT void CSCONV Export_FPackageName_EnsureContentPathsAreRegistered()
{
	FPackageName::EnsureContentPathsAreRegistered();
}

CSEXPORT csbool CSCONV Export_FPackageName_ParseExportTextPath(const FString& InExportTextPath, FString* OutClassName, FString* OutObjectPath)
{
	return FPackageName::ParseExportTextPath(InExportTextPath, OutClassName, OutObjectPath);
}

CSEXPORT void CSCONV Export_FPackageName_ExportTextPathToObjectPath(const FString& InExportTextPath, FString& result)
{
	result = FPackageName::ExportTextPathToObjectPath(InExportTextPath);
}

CSEXPORT void CSCONV Export_FPackageName_ObjectPathToPackageName(const FString& InObjectPath, FString& result)
{
	result = FPackageName::ObjectPathToPackageName(InObjectPath);
}

CSEXPORT void CSCONV Export_FPackageName_ObjectPathToObjectName(const FString& InObjectPath, FString& result)
{
	result = FPackageName::ObjectPathToObjectName(InObjectPath);
}

CSEXPORT csbool CSCONV Export_FPackageName_IsScriptPackage(const FString& InPackageName)
{
	return FPackageName::IsScriptPackage(InPackageName);
}

CSEXPORT csbool CSCONV Export_FPackageName_IsLocalizedPackage(const FString& InPackageName)
{
	return FPackageName::IsLocalizedPackage(InPackageName);
}

CSEXPORT csbool CSCONV Export_FPackageName_DoesPackageNameContainInvalidCharacters(const FString& InLongPackageName, FString& OutReason)
{
	FText OutReasonText;
	csbool Result = FPackageName::DoesPackageNameContainInvalidCharacters(InLongPackageName, &OutReasonText);
	OutReason = OutReasonText.ToString();
	return Result;
}

CSEXPORT csbool CSCONV Export_FPackageName_FindPackageFileWithoutExtension(const FString& InPackageFilename, FString& OutFilename)
{
	return FPackageName::FindPackageFileWithoutExtension(InPackageFilename, OutFilename);
}

CSEXPORT void CSCONV Export_FPackageName(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FPackageName_ConvertToLongScriptPackageName);
	REGISTER_FUNC(Export_FPackageName_RegisterShortPackageNamesForUObjectModules);
	REGISTER_FUNC(Export_FPackageName_FindScriptPackageName);
	REGISTER_FUNC(Export_FPackageName_TryConvertFilenameToLongPackageName);
	REGISTER_FUNC(Export_FPackageName_FilenameToLongPackageName);
	REGISTER_FUNC(Export_FPackageName_TryConvertLongPackageNameToFilename);
	REGISTER_FUNC(Export_FPackageName_LongPackageNameToFilename);
	REGISTER_FUNC(Export_FPackageName_GetLongPackagePath);
	REGISTER_FUNC(Export_FPackageName_SplitLongPackageName);
	REGISTER_FUNC(Export_FPackageName_GetLongPackageAssetName);
	REGISTER_FUNC(Export_FPackageName_IsValidLongPackageName);
	REGISTER_FUNC(Export_FPackageName_IsShortPackageName);
	REGISTER_FUNC(Export_FPackageName_IsShortPackageFName);
	REGISTER_FUNC(Export_FPackageName_GetShortNameFromPackage);
	REGISTER_FUNC(Export_FPackageName_GetShortNameFromFString);
	REGISTER_FUNC(Export_FPackageName_GetShortNameFromFName);
	REGISTER_FUNC(Export_FPackageName_GetShortFNameFromFString);
	REGISTER_FUNC(Export_FPackageName_GetShortFNameFromFName);
	REGISTER_FUNC(Export_FPackageName_RegisterMountPoint);
	REGISTER_FUNC(Export_FPackageName_UnRegisterMountPoint);
	REGISTER_FUNC(Export_FPackageName_GetPackageMountPoint);
	REGISTER_FUNC(Export_FPackageName_DoesPackageExist);
	REGISTER_FUNC(Export_FPackageName_SearchForPackageOnDisk);
	REGISTER_FUNC(Export_FPackageName_TryConvertShortPackagePathToLongInObjectPath);
	REGISTER_FUNC(Export_FPackageName_GetNormalizedObjectPath);
	REGISTER_FUNC(Export_FPackageName_GetLocalizedPackagePath);
	REGISTER_FUNC(Export_FPackageName_GetLocalizedPackagePathWithCulture);
	REGISTER_FUNC(Export_FPackageName_GetAssetPackageExtension);
	REGISTER_FUNC(Export_FPackageName_GetMapPackageExtension);
	REGISTER_FUNC(Export_FPackageName_IsPackageExtension);
	REGISTER_FUNC(Export_FPackageName_IsPackageFilename);
	REGISTER_FUNC(Export_FPackageName_FindPackagesInDirectory);
	REGISTER_FUNC(Export_FPackageName_QueryRootContentPaths);
	REGISTER_FUNC(Export_FPackageName_EnsureContentPathsAreRegistered);
	REGISTER_FUNC(Export_FPackageName_ParseExportTextPath);
	REGISTER_FUNC(Export_FPackageName_ExportTextPathToObjectPath);
	REGISTER_FUNC(Export_FPackageName_ObjectPathToPackageName);
	REGISTER_FUNC(Export_FPackageName_ObjectPathToObjectName);
	REGISTER_FUNC(Export_FPackageName_IsScriptPackage);
	REGISTER_FUNC(Export_FPackageName_IsLocalizedPackage);
	REGISTER_FUNC(Export_FPackageName_DoesPackageNameContainInvalidCharacters);
	REGISTER_FUNC(Export_FPackageName_FindPackageFileWithoutExtension);
}
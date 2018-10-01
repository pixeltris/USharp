#include "ModulePaths.h"
#include "USharpPCH.h"
#include "Interfaces/IPluginManager.h"

TOptional<TMap<FName, FString>> FModulePaths::ModulePathsCache;
bool FModulePaths::BuiltDirectories = false;
TArray<FString> FModulePaths::GameBinariesDirectories;
TArray<FString> FModulePaths::EngineBinariesDirectories;

void FModulePaths::FindModulePaths(const TCHAR* NamePattern, TMap<FName, FString> &OutModulePaths, bool bCanUseCache /*= true*/)
{
	if (!BuiltDirectories)
	{
		BuildDirectories();
	}

	if (!ModulePathsCache)
	{
		ModulePathsCache.Emplace();
		const bool bCanUseCacheWhileGeneratingIt = false;
		FindModulePaths(TEXT("*"), ModulePathsCache.GetValue(), bCanUseCacheWhileGeneratingIt);
	}
	
	if (bCanUseCache)
	{
		// Try to use cache first
		if (const FString* ModulePathPtr = ModulePathsCache->Find(NamePattern))
		{
			OutModulePaths.Add(FName(NamePattern), *ModulePathPtr);
			return;
		}
	}
	
	// Search through the engine directory
	FindModulePathsInDirectory(FPlatformProcess::GetModulesDirectory(), false, NamePattern, OutModulePaths);
	
	// Search any engine directories
	for (int Idx = 0; Idx < EngineBinariesDirectories.Num(); Idx++)
	{
		FindModulePathsInDirectory(EngineBinariesDirectories[Idx], false, NamePattern, OutModulePaths);
	}
	
	// Search any game directories
	for (int Idx = 0; Idx < GameBinariesDirectories.Num(); Idx++)
	{
		FindModulePathsInDirectory(GameBinariesDirectories[Idx], true, NamePattern, OutModulePaths);
	}
}

void FModulePaths::FindModulePathsInDirectory(const FString& InDirectoryName, bool bIsGameDirectory, const TCHAR* NamePattern, TMap<FName, FString> &OutModulePaths)
{
	// Get the prefix and suffix for module filenames
	FString ModulePrefix, ModuleSuffix;
	GetModuleFilenameFormat(bIsGameDirectory, ModulePrefix, ModuleSuffix);

	// Find all the files
	TArray<FString> FullFileNames;
	IFileManager::Get().FindFilesRecursive(FullFileNames, *InDirectoryName, *(ModulePrefix + NamePattern + ModuleSuffix), true, false);

	// Parse all the matching module names
	for (int32 Idx = 0; Idx < FullFileNames.Num(); Idx++)
	{
		const FString &FullFileName = FullFileNames[Idx];

		// On Mac OS X the separate debug symbol format is the dSYM bundle, which is a bundle folder hierarchy containing a .dylib full of Mach-O formatted DWARF debug symbols, these are not loadable modules, so we mustn't ever try and use them. If we don't eliminate this here then it will appear in the module paths & cause errors later on which cannot be recovered from.
#if PLATFORM_MAC
		if (FullFileName.Contains(".dSYM"))
		{
			continue;
		}
#endif

		FString FileName = FPaths::GetCleanFilename(FullFileName);
		if (FileName.StartsWith(ModulePrefix) && FileName.EndsWith(ModuleSuffix))
		{
			FString ModuleName = FileName.Mid(ModulePrefix.Len(), FileName.Len() - ModulePrefix.Len() - ModuleSuffix.Len());
			if (!ModuleName.EndsWith("-Debug") && !ModuleName.EndsWith("-Shipping") && !ModuleName.EndsWith("-Test") && !ModuleName.EndsWith("-DebugGame"))
			{
				OutModulePaths.Add(FName(*ModuleName), FullFileName);
			}
		}
	}
}

void FModulePaths::GetModuleFilenameFormat(bool bGameModule, FString& OutPrefix, FString& OutSuffix)
{
	// Get the module configuration for this directory type
	const TCHAR* ConfigSuffix = NULL;
	switch (FApp::GetBuildConfiguration())
	{
	case EBuildConfigurations::Debug:
		ConfigSuffix = TEXT("-Debug");
		break;
	case EBuildConfigurations::DebugGame:
		ConfigSuffix = bGameModule ? TEXT("-DebugGame") : NULL;
		break;
	case EBuildConfigurations::Development:
		ConfigSuffix = NULL;
		break;
	case EBuildConfigurations::Test:
		ConfigSuffix = TEXT("-Test");
		break;
	case EBuildConfigurations::Shipping:
		ConfigSuffix = TEXT("-Shipping");
		break;
	default:
		check(false);
		break;
	}

	// Get the base name for modules of this application
	OutPrefix = FPlatformProcess::GetModulePrefix() + FPaths::GetBaseFilename(FPlatformProcess::ExecutableName());
	if (OutPrefix.Contains(TEXT("-"), ESearchCase::CaseSensitive))
	{
		OutPrefix = OutPrefix.Left(OutPrefix.Find(TEXT("-"), ESearchCase::CaseSensitive) + 1);
	}
	else
	{
		OutPrefix += TEXT("-");
	}

	// Get the suffix for each module
	OutSuffix.Empty();
	if (ConfigSuffix != NULL)
	{
		OutSuffix += TEXT("-");
		OutSuffix += FPlatformProcess::GetBinariesSubdirectory();
		OutSuffix += ConfigSuffix;
	}
	OutSuffix += TEXT(".");
	OutSuffix += FPlatformProcess::GetModuleExtension();
}

void FModulePaths::BuildDirectories()
{
	// From Engine\Source\Runtime\Launch\Private\LaunchEngineLoop.cpp
	if (FApp::HasProjectName())
	{
		const FString ProjectBinariesDirectory = FPaths::Combine(FPlatformMisc::ProjectDir(), TEXT("Binaries"), FPlatformProcess::GetBinariesSubdirectory());
		GameBinariesDirectories.Add(*ProjectBinariesDirectory);
	}

	// From Engine\Source\Runtime\Projects\Private\PluginManager.cpp
	for (TSharedRef<IPlugin>& Plugin : IPluginManager::Get().GetDiscoveredPlugins())
	{
		if (Plugin->IsEnabled())
		{
			// Add the plugin binaries directory
			const FString PluginBinariesPath = FPaths::Combine(*FPaths::GetPath(Plugin->GetDescriptorFileName()), TEXT("Binaries"), FPlatformProcess::GetBinariesSubdirectory());
			AddBinariesDirectory(*PluginBinariesPath, Plugin->GetLoadedFrom() == EPluginLoadedFrom::Project);
		}
	}

	// From FModuleManager::Get()
	//temp workaround for IPlatformFile being used for FPaths::DirectoryExists before main() sets up the commandline.
#if PLATFORM_DESKTOP
	// Ensure that dependency dlls can be found in restricted sub directories
	const TCHAR* RestrictedFolderNames[] = { TEXT("NoRedist"), TEXT("NotForLicensees"), TEXT("CarefullyRedist") };
	FString ModuleDir = FPlatformProcess::GetModulesDirectory();
	for (const TCHAR* RestrictedFolderName : RestrictedFolderNames)
	{
		FString RestrictedFolder = ModuleDir / RestrictedFolderName;
		if (FPaths::DirectoryExists(RestrictedFolder))
		{
			AddBinariesDirectory(*RestrictedFolder, false);
		}
	}
#endif

	BuiltDirectories = true;
}

void FModulePaths::AddBinariesDirectory(const TCHAR *InDirectory, bool bIsGameDirectory)
{
	if (bIsGameDirectory)
	{
		GameBinariesDirectories.Add(InDirectory);
	}
	else
	{
		EngineBinariesDirectories.Add(InDirectory);
	}

	// Also recurse into restricted sub-folders, if they exist
	const TCHAR* RestrictedFolderNames[] = { TEXT("NoRedist"), TEXT("NotForLicensees"), TEXT("CarefullyRedist") };
	for (const TCHAR* RestrictedFolderName : RestrictedFolderNames)
	{
		FString RestrictedFolder = FPaths::Combine(InDirectory, RestrictedFolderName);
		if (FPaths::DirectoryExists(RestrictedFolder))
		{
			AddBinariesDirectory(*RestrictedFolder, bIsGameDirectory);
		}
	}
}
#include "ModulePaths.h"
#include "USharpPCH.h"
#include "Interfaces/IPluginManager.h"
#include "Modules/ModuleManifest.h"

DEFINE_LOG_CATEGORY_STATIC(LogModuleManager, Log, All);

bool FModulePaths::BuiltDirectories = false;
TArray<FString> FModulePaths::GameBinariesDirectories;
TArray<FString> FModulePaths::EngineBinariesDirectories;
TOptional<TMap<FName, FString>> FModulePaths::ModulePathsCache;
TOptional<FString> FModulePaths::BuildId;

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
		
		// Wildcard for all items
		if (FCString::Strcmp(NamePattern, TEXT("*")) == 0)
		{
			OutModulePaths = ModulePathsCache.GetValue();
			return;
		}
		
		// Wildcard search
		if (FCString::Strchr(NamePattern, TEXT('*')) || FCString::Strchr(NamePattern, TEXT('?')))
		{
			bool bFoundItems = false;
			FString NamePatternString(NamePattern);
			for (const TPair<FName, FString>& CacheIt : ModulePathsCache.GetValue())			
			{
				if (CacheIt.Key.ToString().MatchesWildcard(NamePatternString))
				{
					OutModulePaths.Add(CacheIt.Key, *CacheIt.Value);
					bFoundItems = true;
				}
			}
			
			if (bFoundItems)
			{
				return;
			}
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
	// Figure out the BuildId if it's not already set.
	if (!BuildId.IsSet())
	{
		FString FileName = FModuleManifest::GetFileName(FPlatformProcess::GetModulesDirectory(), false);
		
		FModuleManifest Manifest;
		if (!FModuleManifest::TryRead(FileName, Manifest))
		{
			UE_LOG(LogModuleManager, Fatal, TEXT("Unable to read module manifest from '%s'. Module manifests are generated at build time, and must be present to locate modules at runtime."), *FileName)
		}
		
		BuildId = Manifest.BuildId;
	}

	// Find all the directories to search through, including the base directory
	TArray<FString> SearchDirectoryNames;
	IFileManager::Get().FindFilesRecursive(SearchDirectoryNames, *InDirectoryName, TEXT("*"), false, true);
	SearchDirectoryNames.Insert(InDirectoryName, 0);
	
	// Enumerate the modules in each directory
	for(const FString& SearchDirectoryName: SearchDirectoryNames)
	{
		FModuleManifest Manifest;
		if (FModuleManifest::TryRead(FModuleManifest::GetFileName(SearchDirectoryName, bIsGameDirectory), Manifest) && Manifest.BuildId == BuildId.GetValue())
		{
			for (const TPair<FString, FString>& Pair : Manifest.ModuleNameToFileName)
			{
				if (Pair.Key.MatchesWildcard(NamePattern))
				{
					OutModulePaths.Add(FName(*Pair.Key), *FPaths::Combine(*SearchDirectoryName, *Pair.Value));
				}
			}
		}
	}
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
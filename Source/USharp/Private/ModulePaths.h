#pragma once

// This is a copy of private method FindModulePaths in FModuleManager (Engine\Source\Runtime\Core\Public\Modules\ModuleManager.h)

class FModulePaths
{
private:
	/** Adds an engine binaries directory. */
	static void AddBinariesDirectory(const TCHAR *InDirectory, bool bIsGameDirectory);

	/** Finds modules matching a given name wildcard within a given directory. */
	static void FindModulePathsInDirectory(const FString &DirectoryName, bool bIsGameDirectory, const TCHAR *NamePattern, TMap<FName, FString> &OutModulePaths);

	/** Gets the prefix and suffix for a module file */
	static void GetModuleFilenameFormat(bool bGameModule, FString& OutPrefix, FString& OutSuffix);

	static void BuildDirectories();

	static bool BuiltDirectories;

	/** Array of game binaries directories. */
	static TArray<FString> GameBinariesDirectories;

	/** Array of engine binaries directories. */
	static TArray<FString> EngineBinariesDirectories;

	/** Cache of known module paths. Used for performance. Can increase editor startup times by up to 30% */
	static TOptional<TMap<FName, FString>> ModulePathsCache;

public:
	/** Finds modules matching a given name wildcard. */
	static void FindModulePaths(const TCHAR *NamePattern, TMap<FName, FString> &OutModulePaths, bool bCanUseCache = true);		
};
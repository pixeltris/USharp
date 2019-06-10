// Helper to find .NET runtimes from their installed locations (.NET Core / Mono)
// Keep up to date with the C# version in PluginInstaller

#include "Misc/DefaultValueHelper.h"

struct FVersion
{
	FVersion(int32 InMajor, int32 InMinor, int32 InBuild, int32 InRevision)
		: Major(InMajor)
		, Minor(InMinor)
		, Build(InBuild)
		, Revision(InRevision)
	{
		check(InMajor >= 0 && InMinor >= 0 && InBuild >= 0 && InRevision >= 0);
	}
	
	FVersion(int32 InMajor, int32 InMinor, int32 InBuild)
		: Major(InMajor)
		, Minor(InMinor)
		, Build(InBuild)
		, Revision(-1)
	{
		check(InMajor >= 0 && InMinor >= 0 && InBuild >= 0);
	}
	
	FVersion(int32 InMajor, int32 InMinor)
		: Major(InMajor)
		, Minor(InMinor)
		, Build(-1)
		, Revision(-1)
	{
		check(InMajor >= 0 && InMinor >= 0);
	}
	
	FVersion()
		: Major(0)
		, Minor(0)
		, Build(-1)
		, Revision(-1)
	{ }
	
	FVersion(FString InVersion)
	{
		*this = FVersion::Parse(InVersion);
	}
	
	bool IsDefault()
	{
		return Major == 0 && Minor == 0 && Build == -1 && Revision == -1;
	}
	
	bool operator==(const FVersion& Other) const
	{
		return Major == Other.Major && Minor == Other.Minor && Build == Other.Build && Revision == Other.Revision;
	}
	
	bool operator!=(const FVersion& Other) const
	{
		return !(*this == Other);
	}
	
	bool operator<(const FVersion& Other) const
	{
		return this->CompareTo(Other) < 0;
	}
	
	bool operator>(const FVersion& Other) const
	{
		return this->CompareTo(Other) > 0;
	}
	
	bool operator<=(const FVersion& Other) const
	{
		return this->CompareTo(Other) <= 0;
	}
	
	bool operator>=(const FVersion& Other) const
	{
		return this->CompareTo(Other) >= 0;
	}
	
	int CompareTo(const FVersion& Other) const
	{
		if (Major != Other.Major)
		{
			if (Major > Other.Major)
			{
				return 1;
			}
			else
			{
				return -1;
			}
		}
		
		if (Minor != Other.Minor)
		{
			if (Minor > Other.Minor)
			{
				return 1;
			}
			else
			{
				return -1;
			}
		}
		
		if (Build != Other.Build)
		{
			if (Build > Other.Build)
			{
				return 1;
			}
			else
			{
				return -1;
			}
		}
		
		if (Revision != Other.Revision)
		{
			if (Revision > Other.Revision)
			{
				return 1;
			}
			else
			{
				return -1;
			}
		}
		
		return 0;
	}
	
	friend uint32 GetTypeHash(const FVersion& Other)
	{
		uint32 Hash = GetTypeHash((uint32)Other.Major);
		Hash = HashCombine(Hash, (uint32)Other.Minor);
		Hash = HashCombine(Hash, (uint32)Other.Build);
		Hash = HashCombine(Hash, (uint32)Other.Revision);
		return Hash;
	}
	
	FString ToString()
	{
		if (Build < 0)
		{
			return ToString(2);
		}
		if (Revision < 0)
		{
			return ToString(3);
		}
		return ToString(4);
	}
	
	FString ToString(int FieldCount)
	{
		switch (FieldCount)
		{
			case 0:
				return TEXT("");
			case 1:
				return FString::Printf(TEXT("%d"), Major);
			case 2:
				return FString::Printf(TEXT("%d.%d"), Major, Minor);
			case 3:
				return FString::Printf(TEXT("%d.%d.%d"), Major, Minor, (Build >= 0 ? Build : 0));
			default:
				return FString::Printf(TEXT("%d.%d.%d"), Major, Minor, (Build >= 0 ? Build : 0), (Revision >= 0 ? Revision : 0));
		}
	}
	
	static FVersion Parse(const FString& Input)
	{
		FVersion Result;
		TryParse(Input, Result);
		return Result;
	}
	
	static bool TryParse(const FString& Input, FVersion& Result)
	{
		TArray<FString> Splitted;
		Input.ParseIntoArray(Splitted, TEXT("."), false);
		if (Splitted.Num() < 2)
		{
			Result = FVersion();
			return false;
		}
		
		if (!FDefaultValueHelper::ParseInt(Splitted[0], Result.Major) || Result.Major < 0 ||
			!FDefaultValueHelper::ParseInt(Splitted[1], Result.Minor) || Result.Minor < 0)
		{
			Result = FVersion();
			return false;
		}
		
		if (Splitted.Num() >= 3 && (!FDefaultValueHelper::ParseInt(Splitted[2], Result.Build) || Result.Build < 0))
		{
			Result = FVersion();
			return false;
		}
		
		if (Splitted.Num() >= 4 && (!FDefaultValueHelper::ParseInt(Splitted[3], Result.Revision) || Result.Revision < 0))
		{
			Result = FVersion();
			return false;
		}
		
		return true;
	}

	int32 Major;
	int32 Minor;
	int32 Build;
	int32 Revision;
};

FString FindMonoInstallPath()
{
	IFileManager& FileManager = IFileManager::Get();
#if PLATFORM_WINDOWS
	FString MonoPath;
	#if PLATFORM_64BITS
		FWindowsPlatformMisc::QueryRegKey(HKEY_LOCAL_MACHINE, TEXT("SOFTWARE\\Mono"), TEXT("SdkInstallRoot"), MonoPath);
	#else
		if (FPlatformMisc::Is64bitOperatingSystem())
		{
			FWindowsPlatformMisc::QueryRegKey(HKEY_LOCAL_MACHINE, TEXT("SOFTWARE\\Wow6432Node\\Mono"), TEXT("SdkInstallRoot"), MonoPath);
		}
		else
		{
			FWindowsPlatformMisc::QueryRegKey(HKEY_LOCAL_MACHINE, TEXT("SOFTWARE\\Mono"), TEXT("SdkInstallRoot"), MonoPath);
		}
	#endif
	return MonoPath;
#elif PLATFORM_MAC
	const TCHAR* MacVersionsPath = TEXT("/Library/Frameworks/Mono.framework/Versions/");
	if (FileManager.DirectoryExists(MacVersionsPath))
	{
		FString LatestVersionDir;
		FVersion LatestVersion;
		
		TArray<FString> DirNames;
		FString SearchDir = FString(MacVersionsPath) / TEXT("*");
		FileManager.FindFiles(DirNames, *SearchDir, false, true);
		for (const FString& DirName : DirNames)
		{
			FVersion Version;
			if (FVersion::TryParse(DirName, Version))
			{
				if (LatestVersionDir.IsEmpty() || Version > LatestVersion)
				{
					LatestVersionDir = FPaths::Combine(MacVersionsPath, DirName);
					LatestVersion = Version;
				}
			}
		}
		
		if (!LatestVersionDir.IsEmpty())
		{
			return FPaths::Combine(LatestVersionDir, TEXT("bin"));
		}
	}
#elif PLATFORM_LINUX
	return TEXT("/usr/bin/");
#endif
	return TEXT("");
}

bool TryParseCoreCLRVersion(FString FolderName, FVersion& OutVersion)
{
	int32 Len = FolderName.Len();
	for (int32 i = 0; i < Len; i++)
	{
		if (!FChar::IsDigit(FolderName[i]) && FolderName[i] != '.')
		{
			FolderName = FolderName.Left(i);
			break;
		}
	}
	return FVersion::TryParse(FolderName, OutVersion);
}

FString FindLatestCoreCLRFolder(FString BaseDir, FVersion& OutLatestVersion)
{
	FString LatestVersionDir;
	OutLatestVersion = FVersion();
	IFileManager& FileManager = IFileManager::Get();
	if (FileManager.DirectoryExists(*BaseDir))
	{
		TArray<FString> VersionDirNames;
		FString SearchDir = BaseDir / TEXT("*");
		FileManager.FindFiles(VersionDirNames, *SearchDir, false, true);
		for (const FString& VersionDirName : VersionDirNames)
		{
			FVersion Version;
			if (TryParseCoreCLRVersion(VersionDirName, Version) &&
				(LatestVersionDir.IsEmpty() || Version > OutLatestVersion))
			{
				LatestVersionDir = FPaths::Combine(BaseDir, VersionDirName);
				OutLatestVersion = Version;
			}
		}
	}
	return LatestVersionDir;
}

FString FindLatestCoreCLRFolder(FString Dir)
{
	FVersion Version;
	return FindLatestCoreCLRFolder(Dir, Version);
}

bool FindCoreCLRPaths(FString BasePath, FString& OutSdk, FString& OutAspNetCore, FString& OutNetCore, FString& OutWindowsDesktop)
{
	OutSdk = TEXT("");
	OutAspNetCore = TEXT("");
	OutNetCore = TEXT("");
	OutWindowsDesktop = TEXT("");
	
	if (!BasePath.IsEmpty() && FPaths::DirectoryExists(BasePath))
	{
		FString SdkBaseDir = FPaths::Combine(BasePath, TEXT("sdk"));
		FString SharedDir = FPaths::Combine(BasePath, TEXT("shared"));
		
		FVersion NetCoreVersion;
		
		OutSdk = FindLatestCoreCLRFolder(SdkBaseDir);
		OutAspNetCore = FindLatestCoreCLRFolder(FPaths::Combine(SharedDir, TEXT("Microsoft.AspNetCore.App")));
		OutNetCore = FindLatestCoreCLRFolder(FPaths::Combine(SharedDir, TEXT("Microsoft.NETCore.App")), NetCoreVersion);
		OutWindowsDesktop = FindLatestCoreCLRFolder(FPaths::Combine(SharedDir, TEXT("Microsoft.WindowsDesktop.App")));
		
		return !OutNetCore.IsEmpty() && NetCoreVersion >= FVersion(3, 0);
	}
	return false;
}

FString FindCoreCLRInstallPathBase()
{
	IFileManager& FileManager = IFileManager::Get();
#if PLATFORM_WINDOWS
	FString LatestVersionDir;
	FVersion LatestVersion;
	TCHAR* BaseKeyName = TEXT("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\MiniDumpAuxiliaryDlls");
	#if PLATFORM_64BITS
		if (!FPlatformMisc::Is64bitOperatingSystem())
		{
			BaseKeyName = TEXT("SOFTWARE\\Wow6432Node\\Microsoft\\Windows NT\\CurrentVersion\\MiniDumpAuxiliaryDlls");
		}
	#endif
	HKEY hBaseKey;
	if (RegOpenKeyEx(HKEY_LOCAL_MACHINE, BaseKeyName, 0, KEY_READ, &hBaseKey) == ERROR_SUCCESS)
	{
		DWORD NumSubKeys, MaxNameSize;
		if (RegQueryInfoKey(hBaseKey, NULL, NULL, NULL, &NumSubKeys, &MaxNameSize, NULL, NULL, NULL, NULL, NULL, NULL) == ERROR_SUCCESS)
		{
			TCHAR* SubKeyName = (TCHAR*)FMemory::Malloc(MaxNameSize);
			if (SubKeyName)
			{
				for (DWORD i = 0; i < NumSubKeys; i++)
				{
					DWORD NameSize = MaxNameSize;
					if (RegEnumKeyEx(hBaseKey, i, SubKeyName, &NameSize, NULL, NULL, NULL, NULL) == ERROR_SUCCESS)
					{
						check(NameSize < MaxNameSize);
						
						FString DllPath = SubKeyName;
						if (FileManager.FileExists(*DllPath) && FPaths::GetCleanFilename(DllPath).Equals(TEXT("coreclr.dll"), ESearchCase::IgnoreCase))
						{
							FString DllDir = FPaths::GetPath(DllPath);
							FString NetCoreAppDir = FPaths::Combine(DllDir, TEXT("../"));// Microsoft.NETCore.App dir
							FString DotNetDir = FPaths::Combine(DllDir, TEXT("../../../"));// Root dir
							if (FPaths::DirectoryExists(DotNetDir) && FPaths::DirectoryExists(FPaths::Combine(DotNetDir, TEXT("shared"))))
							{
								NetCoreAppDir = FPaths::ConvertRelativePathToFull(NetCoreAppDir);
								DotNetDir = FPaths::ConvertRelativePathToFull(DotNetDir);
								
								FVersion Version;
								FindLatestCoreCLRFolder(NetCoreAppDir, Version);
								if (!Version.IsDefault() && Version > LatestVersion)
								{
									LatestVersionDir = DotNetDir;
									LatestVersion = Version;
								}
							}
						}
					}
				}
				FMemory::Free(SubKeyName);
			}
		}
	}
	return LatestVersionDir;
#elif PLATFORM_MAC
	return TEXT("/usr/local/share/dotnet/");
#elif PLATFORM_LINUX
	return TEXT("/usr/share/dotnet/");
#endif
	return TEXT("");
}

FString FindCoreCLRInstallPath()
{
	FString BasePath = FindCoreCLRInstallPathBase();
	if (!BasePath.IsEmpty() && FPaths::DirectoryExists(BasePath))
	{
		FString CoreSdkDir, CoreAspNetCoreDir, CoreNetCoreDir, CoreWindowsDesktopDir;
		if (FindCoreCLRPaths(BasePath, CoreSdkDir, CoreAspNetCoreDir, CoreNetCoreDir, CoreWindowsDesktopDir))
		{
			return CoreNetCoreDir;
		}
	}
	return TEXT("");
}
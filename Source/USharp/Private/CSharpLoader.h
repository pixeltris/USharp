#pragma once

#if PLATFORM_WINDOWS

#include "Windows/AllowWindowsPlatformTypes.h"

// Define WIN32_LEAN_AND_MEAN to exclude rarely-used services from windows headers.
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <metahost.h>
#pragma comment(lib, "mscoree.lib")

#include "Windows/HideWindowsPlatformTypes.h"

#endif

#include "USharpPCH.h"

class CSharpLoader
{
private:
	bool usingMono;	
	bool runtimeLoaded;
	bool assemblyLoaded;
	void* monoDomain;	
#if PLATFORM_WINDOWS
	ICLRMetaHost* metaHost;
	ICLRRuntimeHost* runtimeHost;
	ICLRRuntimeInfo *runtimeInfo;
#endif
	TArray<FString> csharpPaths;
	TArray<FString> monoLibPaths;

	static CSharpLoader* singleton;

	CSharpLoader();
	TArray<FString> GetRuntimeVersions(bool mono);
	bool ShouldLoadMono();
	bool LoadRuntimeMono();
	bool LoadRuntimeMs();
	bool LoadRuntime();	
	void SetupPaths();
	FString GetMonoDllPath();
	FString GetAssemblyPath(FString assemblyName);
	bool GetAssemblyPath(FString assemblyPath, FString& outAssemblyPath, bool showLoadError);
	FString GetLoadErrorReason(int32 retVal);	
public:
	static CSharpLoader* GetInstance();
	bool Load(FString assemblyPath, FString customArgs, FString loaderAssemblyPath, bool loaderEnabled);
	bool IsLoaded();
	void LogLoaderError(FString error);
	static FString GetPluginBinariesDir();
};
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

enum class EDotNetRuntime : int32
{
	None = 0x00000000,
	CLR = 0x00000001,
	Mono = 0x00000002,
	Dual = CLR | Mono
};

inline EDotNetRuntime& operator |=(EDotNetRuntime& a, EDotNetRuntime b)
{
	return a = (EDotNetRuntime)((int32)a | (int32)b);
}

struct SharedRuntimeState
{
public:
	EDotNetRuntime DesiredRuntimes;
	EDotNetRuntime InitializedRuntimes;

	EDotNetRuntime LoadedRuntimes;
	EDotNetRuntime ActiveRuntime;
	EDotNetRuntime NextRuntime;
	int32 IsActiveRuntimeComplete;
	uint32 RuntimeCounter;

	int32 HotReloadDataLen;
	int32 HotReloadDataLenInMemory;
	uint8* HotReloadData;

	int32 HotReloadAssemblyPathsLen;
	int32 HotReloadAssemblyPathsLenInMemory;
	uint8* HotReloadAssemblyPaths;

	int32 StructSize;

	void*(*Malloc)(SIZE_T, uint32);
	void*(*Realloc)(void*, SIZE_T, uint32);
	void(*Free)(void*);
};

class CSharpLoader
{
private:
	SharedRuntimeState runtimeState;
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
	FString RuntimeTypeToString(EDotNetRuntime type);
	bool RuntimeTypeHasFlag(EDotNetRuntime type, EDotNetRuntime flagToCheck);
	bool LoadRuntimeMono();
	bool LoadRuntimeMs();
	bool LoadRuntime(bool loaderEnabled);
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
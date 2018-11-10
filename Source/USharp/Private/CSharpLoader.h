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
	CLR = 0x00000001,//.NET Framework
	Mono = 0x00000002,//Mono
	CoreCLR = 0x00000004//.NET Core
};

inline EDotNetRuntime& operator |=(EDotNetRuntime& a, EDotNetRuntime b)
{
	return a = (EDotNetRuntime)((int32)a | (int32)b);
}

// Global state to be shared between Mono/CLR. Should only be accessed on the game thread.
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

	void*(*Malloc)(SIZE_T, uint32);
	void*(*Realloc)(void*, SIZE_T, uint32);
	void(*Free)(void*);
	void(*MessageBox)(char*, char*);
	void(*LogMsg)(uint8, char*);
	
	int32 StructSize;
};

class CSharpLoader
{
private:
	SharedRuntimeState runtimeState;
	void* coreCLRHandle;
	void* monoDomain;	
#if PLATFORM_WINDOWS
	ICLRMetaHost* metaHost;
	ICLRRuntimeHost* runtimeHost;
	ICLRRuntimeInfo *runtimeInfo;
#endif
	TArray<FString> csharpPaths;
	TArray<FString> monoLibPaths;
	TArray<FString> coreCLRLibPaths;

	static CSharpLoader* singleton;

	CSharpLoader();
	TArray<FString> GetRuntimeVersions(EDotNetRuntime runtime);
	FString RuntimeTypeToString(EDotNetRuntime type);
	bool RuntimeTypeHasFlag(EDotNetRuntime type, EDotNetRuntime flagToCheck);
	bool LoadRuntimeMono();
	bool LoadRuntimeCoreCLR();
	bool LoadRuntimeCLR();
	bool LoadRuntimes(bool loaderEnabled);
	void SetupPaths();
	FString GetLibPath(const FString& dllName, const TArray<FString>& libPaths);
	FString GetMonoDllPath();
	FString GetCoreCLRDllPath();
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
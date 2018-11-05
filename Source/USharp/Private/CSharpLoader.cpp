#include "CSharpLoader.h"
#include "USharpPCH.h"
#include "ExportedFunctions/ExportedFunctions.h"
#include "CSharpProjectGeneration.h"

//#define FORCE_MONO 1
#define MONO_SGEN 1
#define STATIC_MONO_LINK PLATFORM_IOS

#ifndef FORCE_MONO
#define FORCE_MONO 0
#endif

#ifndef MONO_SGEN
#define MONO_SGEN 0
#endif

#if PLATFORM_LINUX
#define DLL_EXTENSION TEXT(".so")
#elif PLATFORM_APPLE
#define DLL_EXTENSION TEXT(".dylib")
#else
#define DLL_EXTENSION TEXT(".dll")
#endif

#if !STATIC_MONO_LINK
typedef enum
{
	MONO_DEBUG_FORMAT_NONE,
	MONO_DEBUG_FORMAT_MONO,
	/* Deprecated, the mdb debugger is not longer supported. */
	MONO_DEBUG_FORMAT_DEBUGGER
} MonoDebugFormat;

typedef int32 mono_bool;
typedef void MonoDomain;
typedef void MonoAssembly;
typedef void MonoImage;
typedef void MonoMethodDesc;
typedef void MonoMethod;
typedef void MonoObject;
typedef void MonoString;
typedef void MonoArray;
typedef int32 gint32;

struct _MonoObject
{
	void *vtable;
	void *synchronisation;
};

struct MonoException
{
	_MonoObject object;
	MonoString *class_name;
	MonoString *message;
	MonoObject *_data;
	MonoObject *inner_ex;
	MonoString *help_link;
	/* Stores the IPs and the generic sharing infos
	(vtable/MRGCTX) of the frames. */
	MonoArray  *trace_ips;
	MonoString *stack_trace;
	MonoString *remote_stack_trace;
	gint32	    remote_stack_index;
	/* Dynamic methods referenced by the stack trace */
	MonoObject *dynamic_methods;
	gint32	    hresult;
	MonoString *source;
	MonoObject *serialization_manager;
	MonoObject *captured_traces;
	MonoArray  *native_trace_ips;
	gint32 caught_in_unmanaged;
};

typedef void(*import__mono_config_parse)(const char* filename);
typedef void(*import__mono_domain_set_config)(MonoDomain *domain, const char* base_dir, const char* config_file_name);
typedef void(*import__mono_set_dirs)(const char* assembly_dir, const char* config_dir);
typedef void(*import__mono_debug_init)(MonoDebugFormat format);
typedef MonoDomain*(*import__mono_jit_init_version)(const char* domain_name, const char* runtime_version);
typedef MonoDomain*(*import__mono_jit_init)(const char* file);
typedef void(*import__mono_jit_cleanup)(MonoDomain* domain);
typedef MonoAssembly*(*import__mono_domain_assembly_open)(MonoDomain* domain, const char* name);
typedef MonoImage*(*import__mono_assembly_get_image)(MonoAssembly* assembly);
typedef MonoMethodDesc*(*import__mono_method_desc_new)(const char* name, mono_bool include_namespace);
typedef MonoMethod*(*import__mono_method_desc_search_in_image)(MonoMethodDesc* desc, MonoImage* image);
typedef void(*import__mono_method_desc_free)(MonoMethodDesc* desc);
typedef MonoObject*(*import__mono_runtime_invoke)(MonoMethod* method, void* obj, void** params, MonoException** exc);
typedef void*(*import__mono_object_unbox)(MonoObject* obj);
typedef MonoString*(*import__mono_string_new)(MonoDomain *domain, const char *text);
typedef char*(*import__mono_string_to_utf8)(MonoString* string_obj);

import__mono_config_parse mono_config_parse;
import__mono_domain_set_config mono_domain_set_config;
import__mono_set_dirs mono_set_dirs;
import__mono_debug_init mono_debug_init;
import__mono_jit_init_version mono_jit_init_version;
import__mono_jit_init mono_jit_init;
import__mono_jit_cleanup mono_jit_cleanup;
import__mono_domain_assembly_open mono_domain_assembly_open;
import__mono_assembly_get_image mono_assembly_get_image;
import__mono_method_desc_new mono_method_desc_new;
import__mono_method_desc_search_in_image mono_method_desc_search_in_image;
import__mono_method_desc_free mono_method_desc_free;
import__mono_runtime_invoke mono_runtime_invoke;
import__mono_object_unbox mono_object_unbox;
import__mono_string_new mono_string_new;
import__mono_string_to_utf8 mono_string_to_utf8;
#endif

CSharpLoader* CSharpLoader::singleton = NULL;

CSharpLoader::CSharpLoader()
{
	monoDomain = NULL;
#if PLATFORM_WINDOWS
	metaHost = NULL;
	runtimeHost = NULL;
	runtimeInfo = NULL;
#endif

	runtimeState = {};
	runtimeState.Malloc = &FMemory::Malloc;
	runtimeState.Realloc = &FMemory::Realloc;
	runtimeState.Free = &FMemory::Free;
	runtimeState.StructSize = (int32)sizeof(SharedRuntimeState);

	SetupPaths();

#if FORCE_MONO
	runtimeState.DesiredRuntimes = EDotNetRuntime::Mono
#else
	// Find the desired runtime type
	FString monoDllPath = GetMonoDllPath();
	if (FPaths::FileExists(monoDllPath))
	{
		FString dir = FPaths::GetPath(monoDllPath);
		FString runtimeInfoFile = FPaths::ConvertRelativePathToFull(FPaths::Combine(dir, TEXT("DotNetRuntime.txt")));

		TArray<FString> lines;
		if (FFileHelper::LoadANSITextFileToStrings(*runtimeInfoFile, NULL, lines))
		{
			for (FString line : lines)
			{
				line.TrimStartAndEndInline();
				if (line.Equals(TEXT("mono"), ESearchCase::IgnoreCase))
				{
					runtimeState.DesiredRuntimes |= EDotNetRuntime::Mono;
				}
				else if (line.Equals(TEXT("dual"), ESearchCase::IgnoreCase))
				{
					runtimeState.DesiredRuntimes |= EDotNetRuntime::Dual;
				}
				else if (line.Equals(TEXT("clr"), ESearchCase::IgnoreCase))
				{
					runtimeState.DesiredRuntimes |= EDotNetRuntime::CLR;
				}
			}
		}
	}
	if (runtimeState.DesiredRuntimes == EDotNetRuntime::None)
	{
		runtimeState.DesiredRuntimes = EDotNetRuntime::CLR;
	}
#endif
}

CSharpLoader* CSharpLoader::GetInstance()
{
	if (!singleton)
	{
		singleton = new CSharpLoader();
	}
	return singleton;
}

void CSharpLoader::SetupPaths()
{
	FString PluginsBaseDir = GetPluginBinariesDir();

	// Managed plugins should be under "/Binaries/Managed/"
	csharpPaths.Add(FPaths::Combine(*PluginsBaseDir, TEXT("Managed")));
	
	// Mono should be under "/Binaries/Mono/" or "/Binaries/ThirdParty/Mono/"
	monoLibPaths.Add(FPaths::Combine(*PluginsBaseDir, TEXT("Mono/")));
	monoLibPaths.Add(FPaths::Combine(*PluginsBaseDir, TEXT("ThirdParty/Mono/")));
}

FString CSharpLoader::GetMonoDllPath()
{
#if MONO_SGEN
	FString dllName = FString("mono-2.0-sgen") + DLL_EXTENSION;
#else
	FString dllName = FString("mono-2.0-boehm") + DLL_EXTENSION;
#endif

	for (FString path : monoLibPaths)
	{
		FString dllPath = FPaths::Combine(*path, *dllName);
		if (FPaths::FileExists(dllPath))
		{
			return dllPath;
		}
	}

	return dllName;
}

FString CSharpLoader::GetAssemblyPath(FString assemblyName)
{
	// TODO: Get the current path of the current plugin
	// Get the managed assembly from that path
	
	for (FString path : csharpPaths)
	{
		FString assemblyPath = FPaths::Combine(*path, *assemblyName);
		if (FPaths::FileExists(assemblyPath))
		{
			return assemblyPath;
		}
	}
	
	return TEXT("");
}

bool CSharpLoader::GetAssemblyPath(FString assemblyPath, FString& outAssemblyPath, bool showLoadError)
{
	FString fullAssemblyPath = assemblyPath;
	if (!FPaths::FileExists(fullAssemblyPath))
	{
		fullAssemblyPath = GetAssemblyPath(assemblyPath);
	}
	if (!FPaths::FileExists(fullAssemblyPath))
	{
		if (showLoadError)
		{		
			FString searchPaths;
			for (FString path : csharpPaths)
			{
				searchPaths += path + "\n";
			}		
			LogLoaderError(FString::Printf(TEXT("Couldn't find the assembly file: '%s'\nSearch paths:\n%s"), *assemblyPath, *searchPaths));
		}
		return false;
	}
	outAssemblyPath = fullAssemblyPath;
	return true;
}

TArray<FString> CSharpLoader::GetRuntimeVersions(bool mono)
{
	TArray<FString> runtimeVersions;

	TArray<FString> versions;
	versions.Add("v4.0.30319");
	versions.Add("v2.0.50727");
	versions.Add("v1.1.4322");
	versions.Add("v1.0.3705");

#if PLATFORM_WINDOWS
	if (!mono)
	{
		TCHAR winDir[MAX_PATH];
		GetWindowsDirectory(winDir, MAX_PATH);

		for (auto version : versions)
		{
			FString path = FString(winDir) / TEXT("Microsoft.NET/Framework") / version;
			if (IFileManager::Get().DirectoryExists(*path))
			{
				runtimeVersions.Add(version);
			}
		}
	}
#endif

	if (mono)
	{
		for (auto version : versions)
		{
			runtimeVersions.Add(version);
		}
	}

	return runtimeVersions;
}

bool CSharpLoader::LoadRuntimeMono()
{
	FString dllPath = GetMonoDllPath();
	if (!FPaths::FileExists(dllPath))
	{
		return false;
	}

	TArray<FString> runtimeVersions = GetRuntimeVersions(true);
	if (runtimeVersions.Num() == 0)
	{
		return false;
	}
	FString runtimeVersion = runtimeVersions[0];
	FString monoDirectory = FPaths::GetPath(dllPath);

	void* dllHandle = FPlatformProcess::GetDllHandle(*dllPath);
	if (dllHandle == NULL)
		return false;
	
#if !STATIC_MONO_LINK
	mono_config_parse = (import__mono_config_parse)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_config_parse"));
	mono_domain_set_config = (import__mono_domain_set_config)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_domain_set_config"));
	mono_set_dirs = (import__mono_set_dirs)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_set_dirs"));
	mono_debug_init = (import__mono_debug_init)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_debug_init"));
	mono_jit_init_version = (import__mono_jit_init_version)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_jit_init_version"));
	mono_jit_init = (import__mono_jit_init)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_jit_init"));
	mono_jit_cleanup = (import__mono_jit_cleanup)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_jit_cleanup"));
	mono_domain_assembly_open = (import__mono_domain_assembly_open)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_domain_assembly_open"));
	mono_assembly_get_image = (import__mono_assembly_get_image)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_assembly_get_image"));
	mono_method_desc_new = (import__mono_method_desc_new)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_method_desc_new"));
	mono_method_desc_search_in_image = (import__mono_method_desc_search_in_image)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_method_desc_search_in_image"));
	mono_method_desc_free = (import__mono_method_desc_free)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_method_desc_free"));
	mono_runtime_invoke = (import__mono_runtime_invoke)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_runtime_invoke"));
	mono_object_unbox = (import__mono_object_unbox)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_object_unbox"));
	mono_string_new = (import__mono_string_new)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_string_new"));
	mono_string_to_utf8 = (import__mono_string_to_utf8)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_string_to_utf8"));

	if (mono_config_parse == NULL ||
		mono_domain_set_config == NULL ||
		mono_set_dirs == NULL ||
		mono_debug_init == NULL ||
		mono_jit_init_version == NULL ||
		mono_jit_init == NULL ||
		mono_jit_cleanup == NULL ||
		mono_domain_assembly_open == NULL ||
		mono_assembly_get_image == NULL ||
		mono_method_desc_new == NULL ||
		mono_method_desc_search_in_image == NULL ||
		mono_method_desc_free == NULL ||
		mono_runtime_invoke == NULL ||
		mono_object_unbox == NULL ||
		mono_string_new == NULL ||
		mono_string_to_utf8 == NULL)
	{
		return false;
	}
#endif

	FString assemblyDir = FPaths::Combine(*monoDirectory, TEXT("lib"));
	FString configDir = FPaths::Combine(*monoDirectory, TEXT("etc"));
	mono_set_dirs(TCHAR_TO_ANSI(*assemblyDir), TCHAR_TO_ANSI(*configDir));
#if UE_BUILD_DEBUG || UE_BUILD_DEVELOPMENT
	mono_debug_init(MONO_DEBUG_FORMAT_MONO);
#endif
	mono_config_parse(NULL);
	monoDomain = (void*)mono_jit_init_version("DefaultDomain", TCHAR_TO_ANSI(*runtimeVersion));

	// Workaround to avoid this exception:
	// System.Configuration.ConfigurationErrorsException: Error Initializing the configuration system.
	// ---> System.ArgumentException: The 'ExeConfigFilename' argument cannot be null.
	mono_domain_set_config(monoDomain, ".", "");

	return monoDomain != NULL;
}

bool CSharpLoader::LoadRuntimeMs()
{
#if PLATFORM_WINDOWS
	TArray<FString> runtimeVersions = GetRuntimeVersions(false);
	if (runtimeVersions.Num() == 0)
	{
		return false;
	}
	FString runtimeVersion = runtimeVersions[0];

	metaHost = NULL;
	runtimeHost = NULL;

	HRESULT Result = CLRCreateInstance(CLSID_CLRMetaHost, IID_ICLRMetaHost, (LPVOID*)&metaHost);
	if (SUCCEEDED(Result))
	{
		// Could grab the version from the dll but using the latest .NET framework allows us to mix versions
		/*TCHAR NetFrameworkVersion[255];
		uint32 VersionLength = 255;
		Result = metaHost->GetVersionFromFile(*GetAssemblyName(), NetFrameworkVersion, (unsigned long*)&VersionLength);
		if (SUCCEEDED(Result))
		{
			ICLRRuntimeInfo *RuntimeInfo = NULL;
			Result = metaHost->GetRuntime(NetFrameworkVersion, IID_ICLRRuntimeInfo, (LPVOID*)&runtimeInfo);
			if (SUCCEEDED(Result))
			{
				Result = runtimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, (LPVOID*)&runtimeHost);
			}
		}*/

		runtimeInfo = NULL;
		Result = metaHost->GetRuntime(*runtimeVersion, IID_ICLRRuntimeInfo, (LPVOID*)&runtimeInfo);
		if (SUCCEEDED(Result))
		{
			Result = runtimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, (LPVOID*)&runtimeHost);
		}
	}
	if (SUCCEEDED(Result))
	{
		Result = runtimeHost->Start();
		if (SUCCEEDED(Result))
		{
			Result = runtimeInfo->BindAsLegacyV2Runtime();
		}
	}	
	if (FAILED(Result))
	{
		return false;
	}	

	return true;
#else
	return false;
#endif
}

bool CSharpLoader::LoadRuntime(bool loaderEnabled)
{
	if (runtimeState.InitializedRuntimes != EDotNetRuntime::None)
	{
		return true;
	}

	runtimeState.InitializedRuntimes = EDotNetRuntime::None;

#if PLATFORM_WINDOWS
	if (RuntimeTypeHasFlag(runtimeState.DesiredRuntimes, EDotNetRuntime::CLR) && LoadRuntimeMs())
	{
		runtimeState.InitializedRuntimes |= EDotNetRuntime::CLR;
		if (!RuntimeTypeHasFlag(runtimeState.DesiredRuntimes, EDotNetRuntime::Dual) || !loaderEnabled)
		{
			return true;
		}
	}
#endif

	if (RuntimeTypeHasFlag(runtimeState.DesiredRuntimes, EDotNetRuntime::Mono) && LoadRuntimeMono())
	{
		runtimeState.InitializedRuntimes |= EDotNetRuntime::Mono;
	}

	return runtimeState.InitializedRuntimes != EDotNetRuntime::None;
}

FString CSharpLoader::RuntimeTypeToString(EDotNetRuntime type)
{
	switch (type)
	{
		case EDotNetRuntime::CLR:
			return TEXT("CLR");
		case EDotNetRuntime::Mono:
			return TEXT("Mono");
		case EDotNetRuntime::Dual:
			return TEXT("Dual (CLR+Mono)");
		default:
		case EDotNetRuntime::None:
			return TEXT("Unknown");
	}
}

bool CSharpLoader::RuntimeTypeHasFlag(EDotNetRuntime type, EDotNetRuntime flagToCheck)
{
	return ((int32)type & (int32)flagToCheck) == (int32)flagToCheck;
}

bool CSharpLoader::Load(FString assemblyPath, FString customArgs, FString loaderAssemblyPath, bool loaderEnabled)
{
	if (IsLoaded())
	{
		return true;
	}

	if (!loaderEnabled && runtimeState.DesiredRuntimes == EDotNetRuntime::Dual)
	{
		// If we aren't using the Loader program and the target runtime is set to Dual then only initialize CLR
		runtimeState.DesiredRuntimes = EDotNetRuntime::CLR;
	}
	
	if (!LoadRuntime(loaderEnabled))
	{
		FString desiredRuntimeStr = RuntimeTypeToString(runtimeState.DesiredRuntimes);
		LogLoaderError(FString::Printf(TEXT("Failed to load .NET runtime %s"), *desiredRuntimeStr));
		return false;
	}

	FString entryPointMethodArg = FString::Printf(TEXT("RegisterFuncs=%lld|AddTicker=%lld|IsInGameThread=%lld|RuntimeState=%lld"),
		(int64)&RegisterFunctions, (int64)&Export_FTicker_AddStaticTicker, (int64)&Export_FThreading_IsInGameThread, (int64)&runtimeState);
	if (!customArgs.IsEmpty())
	{
		entryPointMethodArg += TEXT("|") + customArgs;
	}
	
	FString fullAssemblyPath = assemblyPath;
	if (!GetAssemblyPath(fullAssemblyPath, fullAssemblyPath, true))
	{
		return false;
	}
		
	if (loaderEnabled)
	{
		FString mainAssemblyPath = FPaths::ConvertRelativePathToFull(fullAssemblyPath);
		
		// Editor build: if the loader isn't found treat this is as an error
		// Shipping build: if the loader isn't found just load the main assembly directly
		if (GetAssemblyPath(loaderAssemblyPath, fullAssemblyPath, WITH_EDITOR))
		{
			entryPointMethodArg = TEXT("MainAssembly=") + mainAssemblyPath + TEXT("|") + entryPointMethodArg;
		}
		else
		{
#if WITH_EDITOR
			return false;
#endif
		}
	}

	CSharpProjectGeneration::GenerateProject();

	TCHAR* entryPointClass = TEXT("UnrealEngine.EntryPoint");
	TCHAR* entryPointMethod = TEXT("DllMain");

#if PLATFORM_WINDOWS
	if (RuntimeTypeHasFlag(runtimeState.InitializedRuntimes, EDotNetRuntime::CLR) && runtimeHost != NULL)
	{
		::DWORD retVal;
		HRESULT hr = runtimeHost->ExecuteInDefaultAppDomain(*fullAssemblyPath, entryPointClass, entryPointMethod, *entryPointMethodArg, &retVal);
		if (FAILED(hr) || retVal != 0)
		{
			FString reason = GetLoadErrorReason(retVal);
			LogLoaderError(FString::Printf(TEXT("Failed to call ExecuteInDefaultAppDomain. ErrorCode: 0x%08x (%u) Result: %d Reason: %s"), hr, hr, retVal, *reason));
		}
		else
		{
			runtimeState.LoadedRuntimes |= EDotNetRuntime::CLR;
		}
	}
#endif
	if (RuntimeTypeHasFlag(runtimeState.InitializedRuntimes, EDotNetRuntime::Mono) && monoDomain != NULL)
	{
		MonoAssembly* assembly = mono_domain_assembly_open((MonoDomain*)monoDomain, TCHAR_TO_ANSI(*fullAssemblyPath));
		if (assembly == NULL)
		{
			LogLoaderError(TEXT("mono_domain_assembly_open"));
			return false;
		}
		MonoImage* image = mono_assembly_get_image(assembly);
		if (image == NULL)
		{
			LogLoaderError(TEXT("mono_assembly_get_image"));
			return false;
		}
		MonoMethodDesc* methodDesc = mono_method_desc_new(TCHAR_TO_ANSI(*(FString(entryPointClass) + ":" + entryPointMethod)), true);
		if (methodDesc == NULL)
		{
			LogLoaderError(TEXT("mono_method_desc_new"));
			return false;
		}
		MonoMethod* method = mono_method_desc_search_in_image(methodDesc, image);
		if (method == NULL)
		{
			LogLoaderError(TEXT("mono_method_desc_search_in_image"));
			return false;
		}
		mono_method_desc_free(methodDesc);
		
		void* args = { (void*)mono_string_new((MonoDomain*)monoDomain, TCHAR_TO_ANSI(*entryPointMethodArg)) };
		MonoException* exception = NULL;
		MonoObject* result = mono_runtime_invoke(method, NULL, &args, &exception);
		int32 retVal = 0;
		if (exception != NULL || (retVal = *(int32*)mono_object_unbox(result)) != 0)
		{
			/*while (exception != NULL)
			{
				OutputDebugStringA(mono_string_to_utf8(exception->message));
				OutputDebugStringA("\n");
				exception = (MonoException*)exception->inner_ex;
			}
			OutputDebugStringA("\n");*/
			
			FString reason = GetLoadErrorReason(retVal);
			
			FString errorString = TEXT("Error on mono_runtime_invoke Reason: ");
			errorString.Append(reason);
			errorString.Append(TEXT("\n"));
			while (exception != NULL)
			{
				errorString.Append(FString::Printf(TEXT("%s\n"), UTF8_TO_TCHAR(mono_string_to_utf8(exception->message))));
				exception = (MonoException*)exception->inner_ex;
			}
			errorString.Append(TEXT("\n"));
			LogLoaderError(errorString);
			return false;
		}
		else
		{
			runtimeState.LoadedRuntimes |= EDotNetRuntime::Mono;
		}
	}

	return runtimeState.LoadedRuntimes != EDotNetRuntime::None;
}

FString CSharpLoader::GetLoadErrorReason(int32 retVal)
{
	FString reason;
	if (retVal == 1000)
	{
		reason = TEXT("Main assembly path not provided");
	}
	else if(retVal == 1001)
	{
		reason = TEXT("Couldn't find main assembly");
	}
	else if (retVal == 1002)
	{
		reason = TEXT("Main assembly must be in a sub folder to the loader");
	}
	else if (retVal == 1003)
	{
		reason = TEXT("Functions used to run the loader in the game thread are null");
	}
	else if(retVal == 1004)
	{
		reason = TEXT("Failed to load the main assembly");
	}
	return reason;
}

bool CSharpLoader::IsLoaded()
{
	return runtimeState.LoadedRuntimes != EDotNetRuntime::None;
}

void CSharpLoader::LogLoaderError(FString error)
{
	FText title = FText::FromString(TEXT("C# Loader Error"));
	FMessageDialog::Open(EAppMsgType::Ok, FText::FromString(error), &title);
}

FString CSharpLoader::GetPluginBinariesDir()
{
	// This gives up "/Binaries/XXXX/" where XXXX is the platform (Win32, Win64, Android, etc)
#if !IS_MONOLITHIC
	FString PluginsBaseDir = FPaths::GetPath(FModuleManager::Get().GetModuleFilename("USharp"));
#else
	// In monolithic builds there are no plugins (and such FModuleManager::GetModuleFilename doesn't exist)
	// FPlatformProcess::BaseDir() should be the binaries are?
	FString PluginsBaseDir = FPlatformProcess::BaseDir();
#endif
	// Move this up to "/Binaries/"
	PluginsBaseDir = FPaths::Combine(*PluginsBaseDir, TEXT("../"));

	return PluginsBaseDir;
}

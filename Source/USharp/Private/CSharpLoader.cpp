#include "CSharpLoader.h"
#include "USharpPCH.h"
#include "ExportedFunctions/ExportedFunctions.h"

#if PLATFORM_LINUX
#include <signal.h>
#endif

#if PLATFORM_ANDROID
#include "MonoAndroidBootstrap.h"
#endif

//#define FORCE_MONO
//#define MONO_VERBOSE_LOGGING // Enable this if mono fails to load or crashes without errors
#define MONO_STATIC_LINK PLATFORM_IOS

#if PLATFORM_LINUX || PLATFORM_ANDROID
#define DLL_EXTENSION TEXT(".so")
#elif PLATFORM_APPLE
#define DLL_EXTENSION TEXT(".dylib")
#else
#define DLL_EXTENSION TEXT(".dll")
#endif

#ifndef HRESULT
#define HRESULT int
#endif
#ifndef SUCCEEDED
#define SUCCEEDED(hr) ((hr) >= 0)
#endif
#ifndef FAILED
#define FAILED(hr) ((hr) < 0)
#endif


#if !MONO_STATIC_LINK
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

typedef void(*MonoPrintCallback)(const char* str, mono_bool is_stdout);
typedef void(*MonoLogCallback)(const char* log_domain, const char* log_level, const char* message, mono_bool fatal, void* user_data);
typedef void*(*MonoDlFallbackLoad) (const char *name, int flags, char **err, void *user_data);
typedef void*(*MonoDlFallbackSymbol) (void *handle, const char *name, char **err, void *user_data);
typedef void*(*MonoDlFallbackClose) (void *handle, void *user_data);

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
typedef void(*import__mono_trace_init)();
typedef void(*import__mono_trace_set_level_string)(const char* value);
typedef void(*import__mono_trace_set_mask_string)(const char* value);
typedef void(*import__mono_trace_set_log_handler)(MonoLogCallback callback, void* user_data);
typedef void(*import__mono_trace_set_print_handler)(MonoPrintCallback callback);
typedef void(*import__mono_trace_set_printerr_handler)(MonoPrintCallback callback);
typedef void*(*import__mono_dl_fallback_register)(MonoDlFallbackLoad load_func, MonoDlFallbackSymbol symbol_func, MonoDlFallbackClose close_func, void *user_data);

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
import__mono_trace_init mono_trace_init;
import__mono_trace_set_level_string mono_trace_set_level_string;
import__mono_trace_set_mask_string mono_trace_set_mask_string;
import__mono_trace_set_log_handler mono_trace_set_log_handler;
import__mono_trace_set_print_handler mono_trace_set_print_handler;
import__mono_trace_set_printerr_handler mono_trace_set_printerr_handler;
import__mono_dl_fallback_register mono_dl_fallback_register;
#endif

// CoreCLR functions
// https://github.com/dotnet/coreclr/blob/master/src/coreclr/hosts/inc/coreclrhost.h

typedef int(*import__coreclr_initialize)(const char* exePath, const char* appDomainFriendlyName, int propertyCount, const char** propertyKeys, const char** propertyValues, void** hostHandle, unsigned int* domainId);
typedef int(*import__coreclr_shutdown)(void* hostHandle, unsigned int domainId);
typedef int(*import__coreclr_shutdown_2)(void* hostHandle, unsigned int domainId, int* latchedExitCode);
typedef int(*import__coreclr_create_delegate)(void* hostHandle, unsigned int domainId, const char* entryPointAssemblyName, const char* entryPointTypeName, const char* entryPointMethodName, void** delegate);
typedef int(*import__coreclr_execute_assembly)(void* hostHandle, unsigned int domainId, int argc, const char** argv, const char* managedAssemblyPath, unsigned int* exitCode);

import__coreclr_initialize coreclr_initialize;
import__coreclr_shutdown coreclr_shutdown;
import__coreclr_shutdown_2 coreclr_shutdown_2;
import__coreclr_create_delegate coreclr_create_delegate;
import__coreclr_execute_assembly coreclr_execute_assembly;

// The signature of the C# entry point method
typedef int32(*ManagedEntryPointSig)(const char* arg);

CSharpLoader* CSharpLoader::singleton = NULL;

void* CSCONV RuntimeState_Malloc(SIZE_T Count, uint32 Alignment)
{
	return FMemory::Malloc(Count, Alignment);
}

void* CSCONV RuntimeState_Realloc(void* Original, SIZE_T Count, uint32 Alignment)
{
	return FMemory::Realloc(Original, Count, Alignment);
}

void CSCONV RuntimeState_Free(void* Original)
{
	FMemory::Free(Original);
}

void CSCONV RuntimeState_MessageBox(char* text, char* title)
{
	FText textTemp = FText::FromString(FString(ANSI_TO_TCHAR(text)));
	FText titleTemp = FText::FromString(FString(ANSI_TO_TCHAR(title)));
	FMessageDialog::Open(EAppMsgType::Ok, textTemp, &titleTemp);
}

void CSCONV RuntimeState_LogMsg(uint8 verbosity, char* message)
{
	FString categoryName = TEXT("USharp");
	FString messageStr = FString(ANSI_TO_TCHAR(message));
	FMsg::Logf(__FILE__, __LINE__, FName(*categoryName), (ELogVerbosity::Type)verbosity, TEXT("%s"), *messageStr);
}

CSharpLoader::CSharpLoader()
{
	coreCLRHandle = NULL;
	monoDomain = NULL;
#if PLATFORM_WINDOWS
	metaHost = NULL;
	runtimeHost = NULL;
	runtimeInfo = NULL;
#endif

	runtimeState = {};
	runtimeState.Malloc = &RuntimeState_Malloc;
	runtimeState.Realloc = &RuntimeState_Realloc;
	runtimeState.Free = &RuntimeState_Free;
	runtimeState.MessageBox = &RuntimeState_MessageBox;
	runtimeState.LogMsg = &RuntimeState_LogMsg;
	runtimeState.StructSize = (int32)sizeof(SharedRuntimeState);
	runtimeState.PlatformName = FPlatformProperties::IniPlatformName();

	SetupPaths();

#ifdef FORCE_MONO
	runtimeState.DesiredRuntimes = EDotNetRuntime::Mono
#else
	// Find the desired runtimes to use
	FString runtimeInfoFile = FPaths::Combine(GetManagedBinariesDir(), TEXT("Runtimes"), TEXT("DotNetRuntime.txt"));
	if (FPaths::FileExists(runtimeInfoFile))
	{
		runtimeInfoFile = FPaths::ConvertRelativePathToFull(runtimeInfoFile);

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
				else if (line.Equals(TEXT("clr"), ESearchCase::IgnoreCase))
				{
					runtimeState.DesiredRuntimes |= EDotNetRuntime::CLR;
				}
				else if (line.Equals(TEXT("coreclr"), ESearchCase::IgnoreCase))
				{
					runtimeState.DesiredRuntimes |= EDotNetRuntime::CoreCLR;
				}
			}
		}
	}
	if (runtimeState.DesiredRuntimes == EDotNetRuntime::None)
	{
#if PLATFORM_WINDOWS
		// .NET Framework / CLR is preferable for windows for good debugging support
		runtimeState.DesiredRuntimes = EDotNetRuntime::CLR;
#else
		runtimeState.DesiredRuntimes = EDotNetRuntime::Mono;
#endif
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
	FString ManagedBinDir = GetManagedBinariesDir();

	// Managed plugins should be under "/Binaries/Managed/"
	csharpPaths.Add(ManagedBinDir);
	
	// Mono should be under "/Binaries/Managed/Runtimes/Mono/[PLATFORM]/bin/"
	monoLibPaths.Add(FPaths::Combine(*ManagedBinDir, TEXT("Runtimes"), TEXT("Mono"), *GetPlatformString(), TEXT("bin")));

	// CoreCLR should be under "/Binaries/Managed/Runtimes/CoreCLR/[PLATFORM]/"
	coreCLRLibPaths.Add(FPaths::Combine(*ManagedBinDir, TEXT("Runtimes"), TEXT("CoreCLR"), *GetPlatformString()));
}

FString CSharpLoader::GetPlatformString()
{
#if PLATFORM_WINDOWS
	#if PLATFORM_64BITS
		return FString(TEXT("Win64"));
	#else
		return FString(TEXT("Win32"));
	#endif
#elif PLATFORM_LINUX
	return FString(TEXT("Linux"));
#elif PLATFORM_MAC
	return FString(TEXT("Mac"));
#elif PLATFORM_ANDROID
	return FString(TEXT("Android"));
#else
	check(0);
	return FString(TEXT("Unknown"));
#endif
}

FString CSharpLoader::GetLibPath(const FString& dllName, const TArray<FString>& libPaths)
{
	for (FString path : libPaths)
	{
		FString dllPath = FPaths::Combine(*path, *dllName);
		if (FPaths::FileExists(dllPath))
		{
			return dllPath;
		}
	}

	return dllName;
}

FString CSharpLoader::GetMonoDllPath()
{
#if PLATFORM_WINDOWS
	FString dllName = FString(TEXT("mono-2.0-sgen")) + DLL_EXTENSION;
#elif PLATFORM_LINUX
	FString dllName = FString(TEXT("mono-sgen"));
#elif PLATFORM_MAC
	FString dllName = FString(TEXT("mono-sgen64"));
#elif PLATFORM_ANDROID
	FString dllName = FString(TEXT("libmonosgen-2.0")) + DLL_EXTENSION;
#else
	FString dllName = FString(TEXT("mono-sgen"));
	check(0);
#endif
	return GetLibPath(dllName, monoLibPaths);
}

FString CSharpLoader::GetCoreCLRDllPath()
{
#if PLATFORM_WINDOWS
	FString dllName = FString(TEXT("coreclr")) + DLL_EXTENSION;
#else
	FString dllName = FString(TEXT("libcoreclr")) + DLL_EXTENSION;
#endif
	return GetLibPath(dllName, coreCLRLibPaths);
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
			// The calling CSharpLoader::Load() code expects absolute file paths
			return FPaths::ConvertRelativePathToFull(assemblyPath);
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

TArray<FString> CSharpLoader::GetRuntimeVersions(EDotNetRuntime runtime)
{
	TArray<FString> runtimeVersions;

	TArray<FString> versions;
	versions.Add("v4.0.30319");
	versions.Add("v2.0.50727");
	versions.Add("v1.1.4322");
	versions.Add("v1.0.3705");

#if PLATFORM_WINDOWS
	if (runtime == EDotNetRuntime::CLR)
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

	if (runtime == EDotNetRuntime::Mono)
	{
		for (auto version : versions)
		{
			runtimeVersions.Add(version);
		}
	}

	return runtimeVersions;
}

#ifdef MONO_VERBOSE_LOGGING
void OnMonoLog(const char* log_domain, const char* log_level, const char* message, mono_bool fatal, void* user_data)
{
	const FName MonoLogCategory(TEXT("USharp-Mono"));
	ELogVerbosity::Type Verbosity = fatal ? ELogVerbosity::Error : ELogVerbosity::Log;
	FMsg::Logf(__FILE__, __LINE__, MonoLogCategory, Verbosity, TEXT("(log)(%s)(%s) %s"),
		ANSI_TO_TCHAR(log_domain), ANSI_TO_TCHAR(log_level), ANSI_TO_TCHAR(message));
}

void OnMonoPrint(const char* str, mono_bool is_stdout)
{
	const FName MonoLogCategory(TEXT("USharp-Mono"));
	ELogVerbosity::Type Verbosity = is_stdout ? ELogVerbosity::Log : ELogVerbosity::Error;
	FMsg::Logf(__FILE__, __LINE__, MonoLogCategory, Verbosity, TEXT("(print) %s"), ANSI_TO_TCHAR(str));
}
#endif

bool CSharpLoader::LoadRuntimeMono()
{
	FString dllPath = GetMonoDllPath();
	if (!FPaths::FileExists(dllPath))
	{
		return false;
	}

	TArray<FString> runtimeVersions = GetRuntimeVersions(EDotNetRuntime::Mono);
	if (runtimeVersions.Num() == 0)
	{
		return false;
	}
	FString runtimeVersion = runtimeVersions[0];
	FString monoDirectory = FPaths::GetPath(dllPath);

	void* dllHandle = FPlatformProcess::GetDllHandle(*dllPath);
	if (dllHandle == NULL)
		return false;
	
#if !MONO_STATIC_LINK
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
	mono_trace_init = (import__mono_trace_init)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_trace_init"));
	mono_trace_set_level_string = (import__mono_trace_set_level_string)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_trace_set_level_string"));
	mono_trace_set_mask_string = (import__mono_trace_set_mask_string)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_trace_set_mask_string"));
	mono_trace_set_log_handler = (import__mono_trace_set_log_handler)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_trace_set_log_handler"));
	mono_trace_set_print_handler = (import__mono_trace_set_print_handler)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_trace_set_print_handler"));
	mono_trace_set_printerr_handler = (import__mono_trace_set_printerr_handler)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_trace_set_printerr_handler"));
	mono_dl_fallback_register = (import__mono_dl_fallback_register)FPlatformProcess::GetDllExport(dllHandle, TEXT("mono_dl_fallback_register"));

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
		mono_string_to_utf8 == NULL ||
		mono_trace_init == NULL ||
		mono_trace_set_level_string == NULL ||
		mono_trace_set_mask_string == NULL ||
		mono_trace_set_log_handler == NULL ||
		mono_trace_set_print_handler == NULL ||
		mono_trace_set_printerr_handler == NULL ||
		mono_dl_fallback_register == NULL)
	{
		return false;
	}
#endif

#if PLATFORM_LINUX
	// Free up a signals for mono (FUnixPlatformMisc::SetCrashHandler ignores all signals it doesn't use)
	int32 freedSignals = 0;
	const int32 requiredSignals = 3;// abort, suspend, restart
	const int32 targetFlags = SA_SIGINFO | SA_RESTART | SA_ONSTACK;// Set by SetCrashHandler()
	for (int i = SIGRTMAX - 1; i >= 0 && freedSignals < requiredSignals; --i)
	{
		struct sigaction sig;
		sigaction(i, NULL, &sig);
		if (sig.sa_handler == SIG_IGN && (sig.sa_flags & targetFlags) == targetFlags)
		{
			sig.sa_handler = SIG_DFL;
			sigaction(i, &sig, NULL);
			freedSignals++;
		}
	}
	check(freedSignals == requiredSignals);
#endif

#ifdef MONO_VERBOSE_LOGGING
	// mono_trace_init() doesn't seem to print anything on MacOS (regardless of environment variables)
	//mono_trace_init();

	mono_trace_set_level_string("debug");// error, critical, warning, message, info, debug
	mono_trace_set_mask_string("all");// all, asm, type, dll, gc, cfg, aot, security
	mono_trace_set_log_handler(OnMonoLog, NULL);
	mono_trace_set_print_handler(OnMonoPrint);
	mono_trace_set_printerr_handler(OnMonoPrint);
#endif

#if PLATFORM_ANDROID
	bootstrap_mono_android_init();
	mono_dl_fallback_register(bootstrap_mono_android_dlopen, bootstrap_mono_android_dlsym, NULL, NULL);
#endif

	FString assemblyDir = FPaths::Combine(*monoDirectory, TEXT(".."), TEXT("lib"));
	FString configDir = FPaths::Combine(*monoDirectory, TEXT(".."), TEXT("etc"));
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

bool CSharpLoader::LoadRuntimeCoreCLR()
{
	FString dllPath = GetCoreCLRDllPath();
	if (!FPaths::FileExists(dllPath))
	{
		return false;
	}

	void* dllHandle = FPlatformProcess::GetDllHandle(*dllPath);
	if (dllHandle == NULL)
	{
		return false;
	}

	coreclr_initialize = (import__coreclr_initialize)FPlatformProcess::GetDllExport(dllHandle, TEXT("coreclr_initialize"));
	coreclr_shutdown = (import__coreclr_shutdown)FPlatformProcess::GetDllExport(dllHandle, TEXT("coreclr_shutdown"));
	coreclr_shutdown_2 = (import__coreclr_shutdown_2)FPlatformProcess::GetDllExport(dllHandle, TEXT("coreclr_shutdown_2"));
	coreclr_create_delegate = (import__coreclr_create_delegate)FPlatformProcess::GetDllExport(dllHandle, TEXT("coreclr_create_delegate"));
	coreclr_execute_assembly = (import__coreclr_execute_assembly)FPlatformProcess::GetDllExport(dllHandle, TEXT("coreclr_execute_assembly"));

	if (coreclr_initialize == NULL ||
		coreclr_shutdown == NULL ||
		coreclr_shutdown_2 == NULL ||
		coreclr_create_delegate == NULL ||
		coreclr_execute_assembly == NULL)
	{
		return false;
	}

	return true;
}

bool CSharpLoader::LoadRuntimeCLR()
{
#if PLATFORM_WINDOWS
	TArray<FString> runtimeVersions = GetRuntimeVersions(EDotNetRuntime::CLR);
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

bool CSharpLoader::LoadRuntimes(bool loaderEnabled)
{
	if (runtimeState.InitializedRuntimes != EDotNetRuntime::None)
	{
		return true;
	}

#if PLATFORM_WINDOWS
	if ((runtimeState.InitializedRuntimes == EDotNetRuntime::None || loaderEnabled) &&
		RuntimeTypeHasFlag(runtimeState.DesiredRuntimes, EDotNetRuntime::CLR) && LoadRuntimeCLR())
	{
		runtimeState.InitializedRuntimes |= EDotNetRuntime::CLR;
	}
#endif

	if ((runtimeState.InitializedRuntimes == EDotNetRuntime::None || loaderEnabled) &&
		RuntimeTypeHasFlag(runtimeState.DesiredRuntimes, EDotNetRuntime::Mono) && LoadRuntimeMono())
	{
		runtimeState.InitializedRuntimes |= EDotNetRuntime::Mono;
	}

	if ((runtimeState.InitializedRuntimes == EDotNetRuntime::None || loaderEnabled) &&
		RuntimeTypeHasFlag(runtimeState.DesiredRuntimes, EDotNetRuntime::CoreCLR) && LoadRuntimeCoreCLR())
	{
		runtimeState.InitializedRuntimes |= EDotNetRuntime::CoreCLR;
	}

	return runtimeState.InitializedRuntimes != EDotNetRuntime::None;
}

FString CSharpLoader::RuntimeTypeToString(EDotNetRuntime type)
{
	if (type == EDotNetRuntime::None)
	{
		return TEXT("Unknown");
	}

	FString Result;
	if (RuntimeTypeHasFlag(type, EDotNetRuntime::CLR))
	{
		Result += TEXT(", CLR");
	}
	if (RuntimeTypeHasFlag(type, EDotNetRuntime::Mono))
	{
		Result += TEXT(", Mono");
	}
	if (RuntimeTypeHasFlag(type, EDotNetRuntime::CoreCLR))
	{
		Result += TEXT(", CoreCLR");
	}
	Result.RemoveFromStart(TEXT(", "));
	return Result;
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

	// Use absolute paths
	if (FPaths::FileExists(assemblyPath))
	{
		assemblyPath = FPaths::ConvertRelativePathToFull(assemblyPath);
	}
	if (FPaths::FileExists(loaderAssemblyPath))
	{
		loaderAssemblyPath = FPaths::ConvertRelativePathToFull(loaderAssemblyPath);
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
		FString mainAssemblyPath = fullAssemblyPath;

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
#else
			loaderEnabled = false;
#endif
		}
	}

	if (!LoadRuntimes(loaderEnabled))
	{
		FString desiredRuntimeStr = RuntimeTypeToString(runtimeState.DesiredRuntimes);
		LogLoaderError(FString::Printf(TEXT("Failed to load .NET runtimes (%s)"), *desiredRuntimeStr));
		return false;
	}

	const TCHAR* entryPointClass = TEXT("UnrealEngine.EntryPoint");
	const TCHAR* entryPointMethod = TEXT("DllMain");

#if PLATFORM_WINDOWS
	if (RuntimeTypeHasFlag(runtimeState.InitializedRuntimes, EDotNetRuntime::CLR))
	{
		::DWORD retVal;
		HRESULT hr = runtimeHost->ExecuteInDefaultAppDomain(TCHAR_TO_WCHAR(*fullAssemblyPath), TCHAR_TO_WCHAR(entryPointClass), 
			TCHAR_TO_WCHAR(entryPointMethod), TCHAR_TO_WCHAR(*entryPointMethodArg), &retVal);
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
	if (RuntimeTypeHasFlag(runtimeState.InitializedRuntimes, EDotNetRuntime::CoreCLR))
	{
#if PLATFORM_WINDOWS
		FString fileSplitter = TEXT(";");
#else
		FString fileSplitter = TEXT(":");
#endif

		FString assemblName = FPaths::GetBaseFilename(fullAssemblyPath);
		FString assemblyDir = FPaths::GetPath(fullAssemblyPath);
		FString coreCLRDir = FPaths::ConvertRelativePathToFull(FPaths::GetPath(GetCoreCLRDllPath()));

		// Use both the CoreCLR directory and the target assembly directory for APP_PATHS so that
		// it can resolve CoreCLR system assemblies
		FString appPaths = coreCLRDir + fileSplitter + assemblyDir;
		auto appPathsCStr = StringCast<ANSICHAR>(*appPaths);

		// I'm not sure if this should be the path of the target assembly or the path of the running exe
		// It doesn't seem to matter what you use here.
		FString exePath = fullAssemblyPath;
		auto exePathCStr = StringCast<ANSICHAR>(*exePath);

		// We may need to trust more assemblies, for now just add our target assembly
		FString trustedAssemblies = fullAssemblyPath;
		auto trustedAssembliesCtr = StringCast<ANSICHAR>(*trustedAssemblies);

		// FString dllPath = GetCoreCLRDllPath();
		const char* propertyKeys[] =
		{
			"TRUSTED_PLATFORM_ASSEMBLIES",
			"APP_PATHS"
		};
		const char* propertyValues[] =
		{
			// TRUSTED_PLATFORM_ASSEMBLIES
			trustedAssembliesCtr.Get(),
			// APP_PATHS
			appPathsCStr.Get()
		};

		unsigned int domainId;

		int hr = coreclr_initialize(
			exePathCStr.Get(),
			"USharpAppDomain",
			sizeof(propertyValues) / sizeof(char*),
			propertyKeys,
			propertyValues,
			&coreCLRHandle,
			&domainId);

		if (FAILED(hr))
		{
			LogLoaderError(FString::Printf(TEXT("coreclr_initialize failed. ErrorCode: 0x%08x (%u)"), hr, hr));
		}
		else
		{
			ManagedEntryPointSig entryPoint;

			hr = coreclr_create_delegate(
				coreCLRHandle,
				domainId,
				TCHAR_TO_ANSI(*assemblName),
				TCHAR_TO_ANSI(entryPointClass),
				TCHAR_TO_ANSI(entryPointMethod),
				(void**)(&entryPoint));

			if (FAILED(hr))
			{
				LogLoaderError(FString::Printf(TEXT("coreclr_create_delegate failed. ErrorCode: 0x%08x (%u)"), hr, hr));
			}
			else
			{
				int32 retVal = entryPoint(TCHAR_TO_ANSI(*entryPointMethodArg));
				if (retVal != 0)
				{
					FString reason = GetLoadErrorReason(retVal);
					LogLoaderError(FString::Printf(TEXT(".NET Core failed to call the assembly entry point. Reason: %s"), *reason));
				}
				else
				{
					runtimeState.LoadedRuntimes |= EDotNetRuntime::CoreCLR;
				}
			}
		}
	}
	if (RuntimeTypeHasFlag(runtimeState.InitializedRuntimes, EDotNetRuntime::Mono))
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
	switch (retVal)
	{
		case 1000: return TEXT("Main assembly path not provided");
		case 1001: return TEXT("Couldn't find main assembly");
		case 1002: return TEXT("Main assembly must be in a sub folder to the loader");
		case 1003: return TEXT("Functions used to run the loader in the game thread are null");
		case 1004: return TEXT("Failed to load the main assembly");
		case 1005: return TEXT("An exception occured when calling the C# entry point function");
		default: return TEXT("");
	}
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

FString CSharpLoader::GetManagedBinariesDir()
{
#if PLATFORM_ANDROID
	// We can store Mono / managed game dlls in either the .apk under /assets/ or in the UE4 .obb archive.
	// Mono can't be loaded directly from the .obb / .apk so it must first be copied to another location.

	extern FString GExternalFilePath;
	FString TargetDir = FPaths::Combine(GExternalFilePath, TEXT("USharp"), TEXT("Managed"));
	if (FPaths::DirectoryExists(TargetDir))
	{
		return TargetDir;
	}
	
	IFileManager& FileManager = IFileManager::Get();
	int32 NumCopied = 0;
	int32 NumCopyFailed = 0;
	
	FString SrcDir;
	
	// Safer to prepend the DllPathInAssetsFolder string with FString(FPlatformProcess::BaseDir())?
	FString DllPathInAssetsFolder = FString(TEXT("Binaries")) / TEXT("Managed") / TEXT("UnrealEngine.Runtime.dll");
	if (FPaths::FileExists(DllPathInAssetsFolder))
	{
		// The files are in the main apk in the /assets/ folder
		SrcDir = FString(TEXT("Binaries")) / TEXT("Managed");
	}
	else
	{
		// The files are in an .obb archive under /ProjectName/Binaries
		// NOTE: 20tab's UnrealEnginePython implementation (which is based on FAndroidPlatformFile::Initialize?) does some additional .obb scanning. TODO: Look into.
		SrcDir = FPaths::Combine(FPaths::ProjectDir(), TEXT("Binaries"), TEXT("Managed"));
	}
	
	FString AndroidFileListFile = SrcDir / TEXT("AndroidFileList.txt");
	if (FPaths::FileExists(AndroidFileListFile))
	{
		TArray<FString> AndrodFileList;
		if (FFileHelper::LoadFileToStringArray(AndrodFileList, *AndroidFileListFile))
		{
			// This currently wont copy empty directories. Hopefully this wont be an issue.
			for (FString FileName : AndrodFileList)
			{
				FString SrcFileName = SrcDir / FileName;
				TArray<uint8> MemFile;
				if (FFileHelper::LoadFileToArray(MemFile, *SrcFileName, 0))
				{
					FString DestFileName = TargetDir / FileName;
					if (FFileHelper::SaveArrayToFile(MemFile, *DestFileName, &FileManager))
					{
						NumCopied++;
					}
					else
					{
						NumCopyFailed++;
					}
				}
				else
				{
					NumCopyFailed++;
				}
			}
		}
	}
	
	if (NumCopyFailed > 0)
	{
		LogLoaderError(FString::Printf(TEXT("Failed unpack USharp files. Copied:%d failed:%d"), NumCopied, NumCopyFailed));
	}
	
	return TargetDir;
#endif
	// This gives up "/Binaries/XXXX/" where XXXX is the platform (Win32, Win64, Android, etc)
#if !IS_MONOLITHIC
	FString Dir = FPaths::GetPath(FModuleManager::Get().GetModuleFilename("USharp"));
#else
	// In monolithic builds there are no plugins (and such FModuleManager::GetModuleFilename doesn't exist)
	// FPlatformProcess::BaseDir() should be the binaries are?
	FString Dir = FPlatformProcess::BaseDir();
#endif
	// Move to "/Binaries/Managed/"
	Dir = FPaths::Combine(Dir, TEXT("../"), TEXT("Managed"));

	return Dir;
}

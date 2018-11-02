#pragma once

// For UEngine / GEngine access (Export_AActor / Export_FEngineGlobals)
// Note: This slows down compile time around 1.5x-2x
#define SUPPRESS_MONOLITHIC_HEADER_WARNINGS // Suppress warning added in 4.20 for now
#include "Engine.h"

#if WITH_EDITOR
// For Export_UUserDefinedStructEditorData
#include "UserDefinedStructure/UserDefinedStructEditorData.h"
#endif

// For Export_UUserDefinedEnum
#include "Engine/UserDefinedEnum.h"

// For Export_UUserDefinedStruct
#include "Engine/UserDefinedStruct.h"

// For Export_UBlueprintGeneratedClass
#include "Engine/BlueprintGeneratedClass.h"

// For Export_ULevel
#include "Engine/Level.h"

// For Export_FARFilter
#include "AssetRegistryModule.h"//AssetRegistry

// For hotreload
#include "Misc/HotReloadInterface.h"

// For FCoreRedirect
#include "UObject/CoreRedirects.h"//CoreUObject
// For UGameplayStatics
#include "Kismet/GameplayStatics.h" //GameplayStatics

#if WITH_EDITOR
// For Export_SharpHotReloadUtils.h
#include "Kismet2/EnumEditorUtils.h"
#include "Kismet2/BlueprintEditorUtils.h"
#include "Kismet2/CompilerResultsLog.h"//UnrealEd
#include "KismetCompilerModule.h"//KismetCompiler
#include "NodeDependingOnEnumInterface.h"//BlueprintGraph
#include "K2Node_Variable.h"//BlueprintGraph
// For SharpHotReloadStruct.h
#include "KismetCompilerMisc.h"//KismetCompiler
#include "KismetCompiler.h"//KismetCompiler
#include "K2Node_StructOperation.h"//BlueprintGraph
#include "EdMode.h"//UnrealEd
// For SharpHotReloadDelegate.h
#include "K2Node_CallFunction.h"//BlueprintGraph
#include "K2Node_BaseMCDelegate.h"//BlueprintGraph
#include "K2Node_CallDelegate.h"//BlueprintGraph
#include "K2Node_AddDelegate.h"//BlueprintGraph
#include "K2Node_RemoveDelegate.h"//BlueprintGraph
#include "K2Node_CustomEvent.h"//BlueprintGraph
#endif

#define CSCONV __stdcall
#define CSEXPORT

//#ifdef __cplusplus
//#define CSEXPORT_FULL extern "C" __declspec (dllexport)
//#else
//#define CSEXPORT_FULL __declspec (dllexport)
//#endif

// Don't pass TEnumAsByte<> directly between C# / C++ (different layout depending on compiler)

// Use a fixed sized bool size for marshalling due to sizeof(bool) being implementation defined
// Note: This requires the use of wrappers for native structs / function callbacks using bool
typedef int32 csbool;

#define REGISTER_FUNC(FunctionName) registerFunc(&FunctionName, #FunctionName);

#define REGISTER_LAMBDA(Delegate, Lambda) \
	if (enable) *handle = Delegate.AddLambda(Lambda); \
	else { Delegate.Remove(*handle); *handle = FDelegateHandle(); } \

#define REGISTER_DELEGATE(Delegate) \
	if (enable) *handle = Delegate.AddStatic(handler); \
	else { Delegate.Remove(*handle); *handle = FDelegateHandle(); } \

#define BIND_LAMBDA(Delegate, Lambda) \
	if (enable) Delegate.BindLambda(Lambda); \
	else Delegate.Unbind(); \

#define BIND_DELEGATE(Delegate) \
	if (enable) Delegate.BindStatic(handler); \
	else Delegate.Unbind(); \

typedef void(CSCONV *RegisterFunc)(void*, const char*);

// Any structures which have bool need to be remapped here due to sizeof(bool) being implementation defined

struct FImplementedInterfaceInterop
{
	/** the interface class */
	UClass* Class;
	/** the pointer offset of the interface's vtable */
	int32 PointerOffset;
	/** whether or not this interface has been implemented via K2 */
	csbool bImplementedByK2;
	
	static FImplementedInterfaceInterop FromNative(const FImplementedInterface& Instance)
	{
		FImplementedInterfaceInterop Result;
		Result.Class = Instance.Class;
		Result.PointerOffset = Instance.PointerOffset;
		Result.bImplementedByK2 = Instance.bImplementedByK2;
		return Result;
	}
	
	static FImplementedInterface ToNative(const FImplementedInterfaceInterop& Instance)
	{
		FImplementedInterface Result;
		Result.Class = Instance.Class;
		Result.PointerOffset = Instance.PointerOffset;
		Result.bImplementedByK2 = !!Instance.bImplementedByK2;
		return Result;
	}
};

struct FModuleStatusInterop
{
	/** Short name for this module. */
	FString Name;
	/** Full path to this module file on disk. */
	FString FilePath;
	/** Whether the module is currently loaded or not. */
	csbool bIsLoaded;
	/** Whether this module contains game play code. */
	csbool bIsGameModule;

	static FModuleStatusInterop FromNative(const FModuleStatus& Instance)
	{
		FModuleStatusInterop Result;
		Result.Name = Instance.Name;
		Result.FilePath = Instance.FilePath;
		Result.bIsLoaded = Instance.bIsLoaded;
		Result.bIsGameModule = Instance.bIsGameModule;
		return Result;
	}
	
	static FModuleStatus ToNative(const FModuleStatusInterop& Instance)
	{
		FModuleStatus Result;
		Result.Name = Instance.Name;
		Result.FilePath = Instance.FilePath;
		Result.bIsLoaded = !!Instance.bIsLoaded;
		Result.bIsGameModule = !!Instance.bIsGameModule;
		return Result;
	}
};

struct FCopyPropertiesForUnrelatedObjectsParamsInterop
{
	csbool bAggressiveDefaultSubobjectReplacement;
	csbool bDoDelta;
	csbool bReplaceObjectClassReferences;
	csbool bCopyDeprecatedProperties;
	csbool bPreserveRootComponent;

	/** Skips copying properties with BlueprintCompilerGeneratedDefaults metadata */
	csbool bSkipCompilerGeneratedDefaults;
	csbool bNotifyObjectReplacement;
	csbool bClearReferences;

	static FCopyPropertiesForUnrelatedObjectsParamsInterop FromNative(const UEngine::FCopyPropertiesForUnrelatedObjectsParams& Instance)
	{
		FCopyPropertiesForUnrelatedObjectsParamsInterop Result;
		Result.bAggressiveDefaultSubobjectReplacement = Instance.bAggressiveDefaultSubobjectReplacement;
		Result.bDoDelta = Instance.bDoDelta;
		Result.bReplaceObjectClassReferences = Instance.bReplaceObjectClassReferences;
		Result.bCopyDeprecatedProperties = Instance.bCopyDeprecatedProperties;
		Result.bPreserveRootComponent = Instance.bPreserveRootComponent;
		Result.bSkipCompilerGeneratedDefaults = Instance.bSkipCompilerGeneratedDefaults;
		Result.bNotifyObjectReplacement = Instance.bNotifyObjectReplacement;
		Result.bClearReferences = Instance.bClearReferences;
		return Result;
	}

	static UEngine::FCopyPropertiesForUnrelatedObjectsParams ToNative(const FCopyPropertiesForUnrelatedObjectsParamsInterop& Instance)
	{
		UEngine::FCopyPropertiesForUnrelatedObjectsParams Result;
		Result.bAggressiveDefaultSubobjectReplacement = !!Instance.bAggressiveDefaultSubobjectReplacement;
		Result.bDoDelta = !!Instance.bDoDelta;
		Result.bReplaceObjectClassReferences = !!Instance.bReplaceObjectClassReferences;
		Result.bCopyDeprecatedProperties = !!Instance.bCopyDeprecatedProperties;
		Result.bPreserveRootComponent = !!Instance.bPreserveRootComponent;
		Result.bSkipCompilerGeneratedDefaults = !!Instance.bSkipCompilerGeneratedDefaults;
		Result.bNotifyObjectReplacement = !!Instance.bNotifyObjectReplacement;
		Result.bClearReferences = !!Instance.bClearReferences;
		return Result;
	}
};
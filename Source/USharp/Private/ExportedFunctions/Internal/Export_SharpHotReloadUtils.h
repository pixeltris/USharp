// Functions related to updating the editor for changes in structs/enums/blueprints
// These are mostly slightly modified existing functions

#if WITH_EDITOR
#include "HotReload/SharpHotReloadClassReinstancer.h"
#include "HotReload/SharpHotReloadStruct.h"
#include "HotReload/SharpHotReloadDelegate.h"
#include "HotReload/SharpHotReloadEnum.h"

CSEXPORT void CSCONV Export_SharpHotReloadUtils_UpdateEnum(UEnum* Enum, TArray<FName>& InOldNames, TArray<int64>& InOldValues, csbool bResolveData)
{
	SharpHotReloadUtils_UpdateEnum(Enum, InOldNames, InOldValues, bResolveData);
}

CSEXPORT void CSCONV Export_SharpHotReloadUtils_UpdateDelegates(TArray<UFunction*>& Delegates)
{
	SharpHotReloadUtils_UpdateDelegates(Delegates);
}

CSEXPORT void CSCONV Export_SharpHotReloadUtils_PreUpdateStructs(TArray<UScriptStruct*>& SharpStructs, TSet<UBlueprint*>** OutBlueprintsToRecompile, TArray<UUserDefinedStruct*>** OutChangedStructs)
{
	SharpHotReloadUtils_PreUpdateStructs(SharpStructs, OutBlueprintsToRecompile, OutChangedStructs);
}

CSEXPORT void CSCONV Export_SharpHotReloadUtils_PostUpdateStructs(TArray<UScriptStruct*>& SharpChangedStructsOld, TArray<UScriptStruct*>& SharpChangedStructsNew, TSet<UBlueprint*>* InBlueprintsToRecompile, TArray<UUserDefinedStruct*>* InChangedStructs)
{
	SharpHotReloadUtils_PostUpdateStructs(SharpChangedStructsOld, SharpChangedStructsNew, InBlueprintsToRecompile, InChangedStructs);
}

CSEXPORT FSharpHotReloadClassReinstancer* CSCONV Export_SharpHotReloadUtils_CreateClassReinstancer(UClass* InNewClass, UClass* InOldClass)
{
	return new FSharpHotReloadClassReinstancer(InNewClass, InOldClass);
}

CSEXPORT void CSCONV Export_SharpHotReloadUtils_ReinstanceClass(FSharpHotReloadClassReinstancer* Reinstancer)
{
	// Required to satisfy a check in FBlueprintCompileReinstancer::ReplaceInstancesOfClass_Inner - check(OldClass != NewClass || GIsHotReload);
	TGuardValue<bool> GuardIsHotReload(GIsHotReload, true);

	// ReinstanceObjectsAndUpdateDefaults requires the pointer to be a TSharedPtr. The pointer should get destroyed at the end of this method.
	TSharedPtr<FSharpHotReloadClassReinstancer> Shared = MakeShareable(Reinstancer);

	if (Reinstancer->ClassNeedsReinstancing())
	{
		Reinstancer->ReinstanceObjectsAndUpdateDefaults();
	}
}

CSEXPORT void CSCONV Export_SharpHotReloadUtils_FinalizeClasses()
{
	FSharpHotReloadClassReinstancer::Finalize();
}

CSEXPORT void CSCONV Export_SharpHotReloadUtils_BroadcastOnHotReload(csbool wasTriggeredAutomatically)
{
#if WITH_HOT_RELOAD
	IHotReloadInterface& HotReloadSupport = FModuleManager::LoadModuleChecked<IHotReloadInterface>("HotReload");
	HotReloadSupport.OnHotReload().Broadcast(!!wasTriggeredAutomatically);
#endif
}
#endif

csbool USharpMinHotReload = false;

CSEXPORT csbool CSCONV Export_SharpHotReloadUtils_Get_MinimalHotReload()
{
	return USharpMinHotReload;
}

CSEXPORT void CSCONV Export_SharpHotReloadUtils_Set_MinimalHotReload(csbool value)
{
	USharpMinHotReload = value;
}

CSEXPORT void CSCONV Export_SharpHotReloadUtils(RegisterFunc registerFunc)
{
#if WITH_EDITOR
	REGISTER_FUNC(Export_SharpHotReloadUtils_UpdateEnum);
	REGISTER_FUNC(Export_SharpHotReloadUtils_UpdateDelegates);
	REGISTER_FUNC(Export_SharpHotReloadUtils_PreUpdateStructs);
	REGISTER_FUNC(Export_SharpHotReloadUtils_PostUpdateStructs);
	REGISTER_FUNC(Export_SharpHotReloadUtils_CreateClassReinstancer);
	REGISTER_FUNC(Export_SharpHotReloadUtils_ReinstanceClass);
	REGISTER_FUNC(Export_SharpHotReloadUtils_FinalizeClasses);
	REGISTER_FUNC(Export_SharpHotReloadUtils_BroadcastOnHotReload);
#endif
	REGISTER_FUNC(Export_SharpHotReloadUtils_Get_MinimalHotReload);
	REGISTER_FUNC(Export_SharpHotReloadUtils_Set_MinimalHotReload);
}
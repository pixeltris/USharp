#pragma once
#include "ExportedFunctionsConventions.h"

#include "Export_UObject.h"
#include "Export_UObjectBase.h"
#include "Export_UObjectBaseUtility.h"
#include "Export_UObjectGlobals.h"
#include "Export_UObjectHash.h"
#include "Export_FObjectInitializer.h"

#include "Export_UField.h"
#include "Export_UStruct.h"
#include "Export_UClass.h"
#include "Export_UProperty.h"
#include "Export_UFunction.h"
#include "Export_UEnum.h"
#include "Export_UScriptStruct.h"
#include "Export_UUserDefinedStruct.h"
#include "Export_UBlueprintGeneratedClass.h"

#include "Export_UBlueprint.h"
#include "Export_UBlueprintCore.h"

#include "Export_UPackage.h"
#include "Export_UMetaData.h"

#include "Export_UArrayProperty.h"
#include "Export_UBoolProperty.h"
#include "Export_UByteProperty.h"
#include "Export_UClassProperty.h"
#include "Export_UDelegateProperty.h"
#include "Export_UEnumProperty.h"
#include "Export_UInterfaceProperty.h"
#include "Export_UMapProperty.h"
#include "Export_UMulticastDelegateProperty.h"
#include "Export_UNumericProperty.h"
#include "Export_UObjectPropertyBase.h"
#include "Export_USetProperty.h"
#include "Export_USoftClassProperty.h"
#include "Export_UStructProperty.h"

#include "Export_UEngine.h"
#include "Export_UGameEngine.h"
#include "Export_UWorld.h"
#include "Export_UGameplayStatics.h"
#include "Export_UGameInstance.h"
#include "Export_ULevel.h"
#include "Export_AActor.h"
#include "Export_UEditorEngine.h"

#include "Export_FURL.h"
#include "Export_FWorldContext.h"

#include "Export_FSoftObjectPtr.h"
#include "Export_FLazyObjectPtr.h"
#include "Export_FWeakObjectPtr.h"
#include "Export_FReferenceControllerOps.h"

#include "Export_FARFilter.h"
#include "Export_FAssetData.h"
#include "Export_FAssetRegistryModule.h"

#include "Export_FCoreDelegates.h"
#include "Export_FCoreUObjectDelegates.h"

#include "Export_FScriptArray.h"
#include "Export_FScriptMap.h"
#include "Export_FScriptSet.h"
#include "Export_FScriptSparseArray.h"
#include "Export_FBitReference.h"
#include "Export_FScriptBitArray.h"
#include "Export_FScriptDelegate.h"
#include "Export_FMulticastScriptDelegate.h"

#include "Export_FName.h"
#include "Export_FString.h"
#include "Export_FText.h"
#include "Export_FSoftObjectPath.h"
#include "Export_FPaths.h"
#include "Export_FMessageDialog.h"
#include "Export_FModuleManager.h"
#include "Export_FPackageName.h"
#include "Export_FLinkerLoad.h"
#include "Export_FCoreRedirectObjectName.h"
#include "Export_FCoreRedirects.h"
#include "Export_FThreading.h"
#include "Export_FUObjectArray.h"
#include "Export_FTimerManager.h"
#include "Export_FTicker.h"
#include "Export_FDelegateHandle.h"
#include "Export_FPlatformProperties.h"
#include "Export_FGlobals.h"
#include "Export_FBuildGlobals.h"
#include "Export_FAsync.h"
#include "Export_ICppStructOps.h"
#include "Export_FMemory.h"
#include "Export_FCoreNet.h"
#include "Export_FFrame.h"
#include "Export_FFrameRef.h"
#include "Export_FReferenceFinder.h"
#include "Export_TStatId.h"
#include "Export_FApp.h"
#include "Export_FMath.h"
#include "Export_FParse.h"

#include "Export_IConsoleObject.h"
#include "Export_IConsoleCommand.h"
#include "Export_IConsoleVariable.h"
#include "Export_IConsoleManager.h"

#include "Export_GCHelper.h"
#include "Export_FModulePaths.h"
#include "Export_SizeOfStruct.h"
#include "Export_ManagedUnrealType.h"
#include "Export_SharpHotReloadUtils.h"
#include "Export_USharpClass.h"
#include "Export_USharpStruct.h"
#include "Export_Classes.h"

CSEXPORT void CSCONV RegisterFunctions(RegisterFunc registerFunc)
{
	Export_UObject(registerFunc);
	Export_UObjectBase(registerFunc);
	Export_UObjectBaseUtility(registerFunc);
	Export_UObjectGlobals(registerFunc);
	Export_UObjectHash(registerFunc);
	Export_FObjectInitializer(registerFunc);
	
	Export_UField(registerFunc);
	Export_UStruct(registerFunc);
	Export_UClass(registerFunc);
	Export_UProperty(registerFunc);
	Export_UFunction(registerFunc);
	Export_UEnum(registerFunc);
	Export_UScriptStruct(registerFunc);
	Export_UUserDefinedStruct(registerFunc);
	Export_UBlueprintGeneratedClass(registerFunc);
	
	Export_UBlueprint(registerFunc);
	Export_UBlueprintCore(registerFunc);	
	
	Export_UPackage(registerFunc);
	Export_UMetaData(registerFunc);
	
	Export_UArrayProperty(registerFunc);
	Export_UBoolProperty(registerFunc);
	Export_UByteProperty(registerFunc);
	Export_UClassProperty(registerFunc);
	Export_UDelegateProperty(registerFunc);
	Export_UEnumProperty(registerFunc);
	Export_UInterfaceProperty(registerFunc);
	Export_UMapProperty(registerFunc);
	Export_UMulticastDelegateProperty(registerFunc);
	Export_UNumericProperty(registerFunc);
	Export_UObjectPropertyBase(registerFunc);
	Export_USetProperty(registerFunc);
	Export_USoftClassProperty(registerFunc);
	Export_UStructProperty(registerFunc);
	
	Export_UEngine(registerFunc);
	Export_UGameEngine(registerFunc);
	Export_UWorld(registerFunc);
	Export_UGameplayStatics(registerFunc);
	Export_UGameInstance(registerFunc);
	Export_ULevel(registerFunc);
	Export_AActor(registerFunc);
	Export_UEditorEngine(registerFunc);

	Export_FURL(registerFunc);
	Export_FWorldContext(registerFunc);
	
	Export_FSoftObjectPtr(registerFunc);
	Export_FLazyObjectPtr(registerFunc);
	Export_FWeakObjectPtr(registerFunc);
	Export_FReferenceControllerOps(registerFunc);
	
	Export_FARFilter(registerFunc);
	Export_FAssetData(registerFunc);
	Export_FAssetRegistryModule(registerFunc);
	
	Export_FCoreDelegates(registerFunc);
	Export_FCoreUObjectDelegates(registerFunc);
	
	Export_FScriptArray(registerFunc);
	Export_FScriptMap(registerFunc);
	Export_FScriptSet(registerFunc);
	Export_FScriptSparseArray(registerFunc);
	Export_FBitReference(registerFunc);
	Export_FScriptBitArray(registerFunc);
	Export_FScriptDelegate(registerFunc);
	Export_FMulticastScriptDelegate(registerFunc);
	
	Export_FName(registerFunc);
	Export_FString(registerFunc);
	Export_FText(registerFunc);
	Export_FSoftObjectPath(registerFunc);
	Export_FPaths(registerFunc);
	Export_FMessageDialog(registerFunc);
	Export_FModuleManager(registerFunc);
	Export_FPackageName(registerFunc);
	Export_FLinkerLoad(registerFunc);
	Export_FCoreRedirectObjectName(registerFunc);
	Export_FCoreRedirects(registerFunc);
	Export_FThreading(registerFunc);
	Export_FUObjectArray(registerFunc);
	Export_FTimerManager(registerFunc);
	Export_FTicker(registerFunc);
	Export_FDelegateHandle(registerFunc);
	Export_FPlatformProperties(registerFunc);
	Export_FGlobals(registerFunc);
	Export_FBuildGlobals(registerFunc);
	Export_FAsync(registerFunc);
	Export_ICppStructOps(registerFunc);
	Export_FMemory(registerFunc);
	Export_FCoreNet(registerFunc);
	Export_FFrame(registerFunc);
	Export_FFrameRef(registerFunc);
	Export_FReferenceFinder(registerFunc);
	Export_TStatId(registerFunc);
	Export_FApp(registerFunc);
	Export_FMath(registerFunc);
	Export_FParse(registerFunc);
	
	Export_IConsoleObject(registerFunc);
	Export_IConsoleCommand(registerFunc);
	Export_IConsoleVariable(registerFunc);
	Export_IConsoleManager(registerFunc);
	
	Export_GCHelper(registerFunc);
	Export_FModulePaths(registerFunc);
	Export_SizeOfStruct(registerFunc);
	Export_ManagedUnrealType(registerFunc);
	Export_SharpHotReloadUtils(registerFunc);
	Export_USharpClass(registerFunc);
	Export_USharpStruct(registerFunc);
	Export_Classes(registerFunc);
}
CSEXPORT void CSCONV Export_UObject_PostInitProperties(UObject* instance)
{
	instance->PostInitProperties();
}

CSEXPORT csbool CSCONV Export_UObject_PreSaveRoot(UObject* instance, const FString& Filename)
{
	return instance->PreSaveRoot(*Filename);
}

CSEXPORT void CSCONV Export_UObject_PostSaveRoot(UObject* instance, csbool bCleanupIsRequired)
{
	instance->PostSaveRoot(!!bCleanupIsRequired);
}

CSEXPORT void CSCONV Export_UObject_PreSave(UObject* instance, ITargetPlatform* TargetPlatform)
{
	instance->PreSave(TargetPlatform);
}

CSEXPORT csbool CSCONV Export_UObject_Modify(UObject* instance, csbool bAlwaysMarkDirty)
{
	return instance->Modify(!!bAlwaysMarkDirty);
}

#if WITH_EDITOR
CSEXPORT void CSCONV Export_UObject_LoadedFromAnotherClass(UObject* instance, const FName& OldClassName)
{
	instance->LoadedFromAnotherClass(OldClassName);
}
#endif

CSEXPORT void CSCONV Export_UObject_PostLoad(UObject* instance)
{
	instance->PostLoad();
}

CSEXPORT void CSCONV Export_UObject_PostLoadSubobjects(UObject* instance, FObjectInstancingGraph* OuterInstanceGraph)
{
	instance->PostLoadSubobjects(OuterInstanceGraph);
}

CSEXPORT void CSCONV Export_UObject_BeginDestroy(UObject* instance)
{
	instance->BeginDestroy();
}

CSEXPORT csbool CSCONV Export_UObject_IsReadyForFinishDestroy(UObject* instance)
{
	return instance->IsReadyForFinishDestroy();
}

#if WITH_EDITOR
CSEXPORT void CSCONV Export_UObject_PostLinkerChange(UObject* instance)
{
	instance->PostLinkerChange();
}
#endif

CSEXPORT void CSCONV Export_UObject_FinishDestroy(UObject* instance)
{
	instance->FinishDestroy();
}

CSEXPORT void CSCONV Export_UObject_Serialize(UObject* instance, FArchive& Ar)
{
	instance->Serialize(Ar);
}

CSEXPORT void CSCONV Export_UObject_ShutdownAfterError(UObject* instance)
{
	instance->ShutdownAfterError();
}

CSEXPORT void CSCONV Export_UObject_PostInterpChange(UObject* instance, UProperty* PropertyThatChanged)
{
	instance->PostInterpChange(PropertyThatChanged);
}

#if WITH_EDITOR
CSEXPORT void CSCONV Export_UObject_PreEditChange(UObject* instance, UProperty* PropertyAboutToChange)
{
	instance->PreEditChange(PropertyAboutToChange);
}

CSEXPORT void CSCONV Export_UObject_PreEditChangeChain(UObject* instance, class FEditPropertyChain& PropertyAboutToChange)
{
	instance->PreEditChange(PropertyAboutToChange);
}

CSEXPORT csbool CSCONV Export_UObject_CanEditChange(UObject* instance, const UProperty* InProperty)
{
	return instance->CanEditChange(InProperty);
}

CSEXPORT void CSCONV Export_UObject_PostEditChange(UObject* instance)
{
	instance->PostEditChange();
}

CSEXPORT void CSCONV Export_UObject_PostEditChangeProperty(UObject* instance, struct FPropertyChangedEvent& PropertyChangedEvent)
{
	instance->PostEditChangeProperty(PropertyChangedEvent);
}

CSEXPORT void CSCONV Export_UObject_PostEditChangeChainProperty(UObject* instance, struct FPropertyChangedChainEvent& PropertyChangedEvent)
{
	instance->PostEditChangeChainProperty(PropertyChangedEvent);
}

CSEXPORT TSharedPtr<ITransactionObjectAnnotation> CSCONV Export_UObject_GetTransactionAnnotation(UObject* instance)
{
	return instance->GetTransactionAnnotation();
}

CSEXPORT void CSCONV Export_UObject_PreEditUndo(UObject* instance)
{
	instance->PreEditUndo();
}

CSEXPORT void CSCONV Export_UObject_PostEditUndo(UObject* instance)
{
	instance->PostEditUndo();
}

CSEXPORT void CSCONV Export_UObject_PostEditUndoAnnotation(UObject* instance, TSharedPtr<ITransactionObjectAnnotation> TransactionAnnotation)
{
	instance->PostEditUndo(TransactionAnnotation);
}
#endif

CSEXPORT void CSCONV Export_UObject_PostRename(UObject* instance, UObject* OldOuter, const FName& OldName)
{
	instance->PostRename(OldOuter, OldName);
}

CSEXPORT void CSCONV Export_UObject_PostDuplicate(UObject* instance, csbool bDuplicateForPIE)
{
	instance->PostDuplicate(!!bDuplicateForPIE);
}

CSEXPORT csbool CSCONV Export_UObject_NeedsLoadForClient(UObject* instance)
{
	return instance->NeedsLoadForClient();
}

CSEXPORT csbool CSCONV Export_UObject_NeedsLoadForServer(UObject* instance)
{
	return instance->NeedsLoadForServer();
}

CSEXPORT csbool CSCONV Export_UObject_NeedsLoadForEditorGame(UObject* instance)
{
	return instance->NeedsLoadForEditorGame();
}

CSEXPORT csbool CSCONV Export_UObject_CanCreateInCurrentContext(UObject* Template)
{
	return UObject::CanCreateInCurrentContext(Template);
}

CSEXPORT void CSCONV Export_UObject_ExportCustomProperties(UObject* instance, FOutputDevice& Out, uint32 Indent)
{
	instance->ExportCustomProperties(Out, Indent);
}

CSEXPORT void CSCONV Export_UObject_ImportCustomProperties(UObject* instance, const FString& SourceText, FFeedbackContext* Warn)
{
	instance->ImportCustomProperties(*SourceText, Warn);
}

CSEXPORT void CSCONV Export_UObject_PostEditImport(UObject* instance)
{
	instance->PostEditImport();
}

CSEXPORT void CSCONV Export_UObject_PostReloadConfig(UObject* instance, class UProperty* PropertyThatWasLoaded)
{
	instance->PostReloadConfig(PropertyThatWasLoaded);
}

CSEXPORT csbool CSCONV Export_UObject_Rename(UObject* instance, const FString& NewName, UObject* NewOuter, ERenameFlags Flags)
{
	return instance->Rename(*NewName, NewOuter, Flags);
}

CSEXPORT void CSCONV Export_UObject_GetDesc(UObject* instance, FString& result)
{
	result = instance->GetDesc();
}

#if WITH_ENGINE
CSEXPORT class UWorld* CSCONV Export_UObject_GetWorld(UObject* instance)
{
	return instance->GetWorld();
}

CSEXPORT class UWorld* CSCONV Export_UObject_GetWorldChecked(UObject* instance, csbool& bSupported)
{
	bool bSupportedNative = !!bSupported;
	UWorld* Result = instance->GetWorldChecked(bSupportedNative);
	bSupported = bSupportedNative;
	return Result;
}

CSEXPORT csbool CSCONV Export_UObject_ImplementsGetWorld(UObject* instance)
{
	return instance->ImplementsGetWorld();
}
#endif

CSEXPORT csbool CSCONV Export_UObject_GetNativePropertyValues(UObject* instance, TMap<FString, FString>& out_PropertyValues, uint32 ExportFlags)
{
	return instance->GetNativePropertyValues(out_PropertyValues, ExportFlags);
}

CSEXPORT uint64 CSCONV Export_UObject_GetResourceSizeBytes(UObject* instance, EResourceSizeMode::Type Mode)
{
	return (uint64)instance->GetResourceSizeBytes(Mode);
}

CSEXPORT void CSCONV Export_UObject_GetExporterName(UObject* instance, FName& result)
{
	result = instance->GetExporterName();
}

CSEXPORT csbool CSCONV Export_UObject_IsLocalizedResource(UObject* instance)
{
	return instance->IsLocalizedResource();
}

CSEXPORT void CSCONV Export_UObject_AddReferencedObjects(UObject* InThis, FReferenceCollector& Collector)
{
	UObject::AddReferencedObjects(InThis, Collector);
}

CSEXPORT void CSCONV Export_UObject_CallAddReferencedObjects(UObject* instance, FReferenceCollector& Collector)
{
	instance->CallAddReferencedObjects(Collector);
}

CSEXPORT FRestoreForUObjectOverwrite* CSCONV Export_UObject_GetRestoreForUObjectOverwrite(UObject* instance)
{
	return instance->GetRestoreForUObjectOverwrite();
}

CSEXPORT csbool CSCONV Export_UObject_AreNativePropertiesIdenticalTo(UObject* instance, UObject* Other)
{
	return instance->AreNativePropertiesIdenticalTo(Other);
}

CSEXPORT void CSCONV Export_UObject_GetAssetRegistryTagsFromSearchableProperties(const UObject* Object, TArray<UObject::FAssetRegistryTag>& OutTags)
{
	UObject::FAssetRegistryTag::GetAssetRegistryTagsFromSearchableProperties(Object, OutTags);
}

CSEXPORT void CSCONV Export_UObject_GetAssetRegistryTags(UObject* instance, TArray<UObject::FAssetRegistryTag>& OutTags)
{
	instance->GetAssetRegistryTags(OutTags);
}

CSEXPORT csbool CSCONV Export_UObject_IsAsset(UObject* instance)
{
	return instance->IsAsset();
}

CSEXPORT csbool CSCONV Export_UObject_IsSafeForRootSet(UObject* instance)
{
	return instance->IsSafeForRootSet();
}

CSEXPORT void CSCONV Export_UObject_TagSubobjects(UObject* instance, EObjectFlags NewFlags)
{
	instance->TagSubobjects(NewFlags);
}

CSEXPORT void CSCONV Export_UObject_GetLifetimeReplicatedProps(UObject* instance, TArray<class FLifetimeProperty>& OutLifetimeProps)
{
	instance->GetLifetimeReplicatedProps(OutLifetimeProps);
}

CSEXPORT csbool CSCONV Export_UObject_IsNameStableForNetworking(UObject* instance)
{
	return instance->IsNameStableForNetworking();
}

CSEXPORT csbool CSCONV Export_UObject_IsFullNameStableForNetworking(UObject* instance)
{
	return instance->IsFullNameStableForNetworking();
}

CSEXPORT csbool CSCONV Export_UObject_IsSupportedForNetworking(UObject* instance)
{
	return instance->IsSupportedForNetworking();
}

CSEXPORT void CSCONV Export_UObject_GetSubobjectsWithStableNamesForNetworking(UObject* instance, TArray<UObject*>& ObjList)
{
	instance->GetSubobjectsWithStableNamesForNetworking(ObjList);
}

CSEXPORT void CSCONV Export_UObject_PreNetReceive(UObject* instance)
{
	instance->PreNetReceive();
}

CSEXPORT void CSCONV Export_UObject_PostNetReceive(UObject* instance)
{
	instance->PostNetReceive();
}

CSEXPORT csbool CSCONV Export_UObject_IsSelected(UObject* instance)
{
	return instance->IsSelected();
}

#if WITH_EDITOR
CSEXPORT void CSCONV Export_UObject_PropagatePreEditChange(UObject* instance, TArray<UObject*>& AffectedObjects, FEditPropertyChain& PropertyAboutToChange)
{
	instance->PropagatePreEditChange(AffectedObjects, PropertyAboutToChange);
}

CSEXPORT void CSCONV Export_UObject_PropagatePostEditChange(UObject* instance, TArray<UObject*>& AffectedObjects, FPropertyChangedChainEvent& PropertyChangedEvent)
{
	instance->PropagatePostEditChange(AffectedObjects, PropertyChangedEvent);
}
#endif

CSEXPORT void CSCONV Export_UObject_SerializeScriptProperties(UObject* instance, FArchive& Ar)
{
	instance->SerializeScriptProperties(Ar);
}

CSEXPORT void CSCONV Export_UObject_ReinitializeProperties(UObject* instance, UObject* SourceObject, struct FObjectInstancingGraph* InstanceGraph)
{
	instance->ReinitializeProperties(SourceObject, InstanceGraph);
}

CSEXPORT void CSCONV Export_UObject_GetDetailedInfo(UObject* instance, FString& result)
{
	result = instance->GetDetailedInfo();
}

CSEXPORT csbool CSCONV Export_UObject_ConditionalBeginDestroy(UObject* instance)
{
	return instance->ConditionalBeginDestroy();
}

CSEXPORT csbool CSCONV Export_UObject_ConditionalFinishDestroy(UObject* instance)
{
	return instance->ConditionalFinishDestroy();
}

CSEXPORT void CSCONV Export_UObject_ConditionalPostLoad(UObject* instance)
{
	instance->ConditionalPostLoad();
}

CSEXPORT void CSCONV Export_UObject_ConditionalPostLoadSubobjects(UObject* instance, struct FObjectInstancingGraph* OuterInstanceGraph)
{
	instance->ConditionalPostLoadSubobjects(OuterInstanceGraph);
}

#if WITH_EDITOR
CSEXPORT void CSCONV Export_UObject_BeginCacheForCookedPlatformData(UObject* instance, const ITargetPlatform* TargetPlatform)
{
	instance->BeginCacheForCookedPlatformData(TargetPlatform);
}

CSEXPORT void CSCONV Export_UObject_ClearCachedCookedPlatformData(UObject* instance, const ITargetPlatform* TargetPlatform)
{
	instance->ClearCachedCookedPlatformData(TargetPlatform);
}

CSEXPORT void CSCONV Export_UObject_ClearAllCachedCookedPlatformData(UObject* instance)
{
	instance->ClearAllCachedCookedPlatformData();
}

CSEXPORT csbool CSCONV Export_UObject_IsCachedCookedPlatformDataLoaded(UObject* instance, const ITargetPlatform* TargetPlatform)
{
	return instance->IsCachedCookedPlatformDataLoaded(TargetPlatform);
}
#endif

CSEXPORT csbool CSCONV Export_UObject_IsBasedOnArchetype(UObject* instance, const UObject* const SomeObject)
{
	return instance->IsBasedOnArchetype(SomeObject);
}

CSEXPORT UFunction* CSCONV Export_UObject_FindFunction(UObject* instance, const FName& InName)
{
	return instance->FindFunction(InName);
}

CSEXPORT UFunction* CSCONV Export_UObject_FindFunctionChecked(UObject* instance, const FName& InName)
{
	return instance->FindFunctionChecked(InName);
}

CSEXPORT void CSCONV Export_UObject_CollectDefaultSubobjects(UObject* instance, TArray<UObject*>& OutDefaultSubobjects, csbool bIncludeNestedSubobjects)
{
	instance->CollectDefaultSubobjects(OutDefaultSubobjects, !!bIncludeNestedSubobjects);
}

CSEXPORT csbool CSCONV Export_UObject_CheckDefaultSubobjects(UObject* instance, csbool bForceCheck)
{
	return instance->CheckDefaultSubobjects(!!bForceCheck);
}

CSEXPORT void CSCONV Export_UObject_SaveConfig(UObject* instance, uint64 Flags, const FString& Filename, FConfigCacheIni* Config)
{
	instance->SaveConfig(Flags, *Filename, Config);
}

CSEXPORT void CSCONV Export_UObject_UpdateDefaultConfigFile(UObject* instance)
{
	instance->UpdateDefaultConfigFile();
}

CSEXPORT void CSCONV Export_UObject_GetDefaultConfigFilename(UObject* instance, FString& result)
{
	result = instance->GetDefaultConfigFilename();
}

CSEXPORT void CSCONV Export_UObject_LoadConfig(UObject* instance, UClass* ConfigClass, const FString& Filename, uint32 PropagationFlags, class UProperty* PropertyToLoad)
{
	instance->LoadConfig(ConfigClass, *Filename, PropagationFlags, PropertyToLoad);
}

CSEXPORT void CSCONV Export_UObject_ReloadConfig(UObject* instance, UClass* ConfigClass, const FString& Filename, uint32 PropagationFlags, class UProperty* PropertyToLoad)
{
	instance->ReloadConfig(ConfigClass, *Filename, PropagationFlags, PropertyToLoad);
}

CSEXPORT void CSCONV Export_UObject_ParseParms(UObject* instance, const FString& Parms)
{
	instance->ParseParms(*Parms);
}

CSEXPORT void CSCONV Export_UObject_OutputReferencers(UObject* instance, FOutputDevice& Ar, FReferencerInformationList* Referencers)
{
	instance->OutputReferencers(Ar, Referencers);
}

CSEXPORT void CSCONV Export_UObject_RetrieveReferencers(UObject* instance, TArray<FReferencerInformation>* OutInternalReferencers, TArray<FReferencerInformation>* OutExternalReferencers)
{
	instance->RetrieveReferencers(OutInternalReferencers, OutExternalReferencers);
}

CSEXPORT void CSCONV Export_UObject_SetLinker(UObject* instance, FLinkerLoad* LinkerLoad, int32 LinkerIndex, csbool bShouldDetachExisting)
{
	instance->SetLinker(LinkerLoad, LinkerIndex, !!bShouldDetachExisting);
}

CSEXPORT UObject* CSCONV Export_UObject_GetArchetypeFromRequiredInfo(UClass* Class, UObject* Outer, const FName& Name, EObjectFlags ObjectFlags)
{
	return UObject::GetArchetypeFromRequiredInfo(Class, Outer, Name, ObjectFlags);
}

CSEXPORT UObject* CSCONV Export_UObject_GetArchetype(UObject* instance)
{
	return instance->GetArchetype();
}

CSEXPORT void CSCONV Export_UObject_GetArchetypeInstances(UObject* instance, TArray<UObject*>& Instances)
{
	instance->GetArchetypeInstances(Instances);
}

CSEXPORT void CSCONV Export_UObject_InstanceSubobjectTemplates(UObject* instance, struct FObjectInstancingGraph* InstanceGraph)
{
	instance->InstanceSubobjectTemplates(InstanceGraph);
}

CSEXPORT void CSCONV Export_UObject_ProcessEvent(UObject* instance, UFunction* Function, void* Parms)
{
	instance->ProcessEvent(Function, Parms);
}

CSEXPORT int32 CSCONV Export_UObject_GetFunctionCallspace(UObject* instance, UFunction* Function, void* Parameters, FFrame* Stack)
{
	return instance->GetFunctionCallspace(Function, Parameters, Stack);
}

CSEXPORT csbool CSCONV Export_UObject_CallRemoteFunction(UObject* instance, UFunction* Function, void* Parms, struct FOutParmRec* OutParms, FFrame* Stack)
{
	return instance->CallRemoteFunction(Function, Parms, OutParms, Stack);
}

CSEXPORT csbool CSCONV Export_UObject_CallFunctionByNameWithArguments(UObject* instance, const FString& Cmd, FOutputDevice* ArPtr, UObject* Executor, csbool bForceCallWithNonExec)
{
	// Allow the output device device to be null and use a temp output device if so
	if (ArPtr == nullptr)
	{
		FOutputDeviceDebug Ar;
		return instance->CallFunctionByNameWithArguments(*Cmd, Ar, Executor, !!bForceCallWithNonExec);		
	}
	else
	{
		return instance->CallFunctionByNameWithArguments(*Cmd, *ArPtr, Executor, !!bForceCallWithNonExec);
	}
}

CSEXPORT void CSCONV Export_UObject_CallFunction(UObject* instance, FFrame& Stack, RESULT_DECL, UFunction* Function)
{
	instance->CallFunction(Stack, RESULT_PARAM, Function);
}

CSEXPORT void CSCONV Export_UObject_ProcessInternal(UObject* instance, UObject* Context, FFrame& Stack, RESULT_DECL)
{
	instance->ProcessInternal(Context, Stack, RESULT_PARAM);
}

CSEXPORT csbool CSCONV Export_UObject_ProcessConsoleExec(UObject* instance, const FString& Cmd, FOutputDevice& Ar, UObject* Executor)
{
	return instance->ProcessConsoleExec(*Cmd, Ar, Executor);
}

CSEXPORT void CSCONV Export_UObject_SkipFunction(UObject* instance, FFrame& Stack, RESULT_DECL, UFunction* Function)
{
	instance->SkipFunction(Stack, RESULT_PARAM, Function);
}

CSEXPORT UClass* CSCONV Export_UObject_RegenerateClass(UObject* instance, UClass* ClassToRegenerate, UObject* PreviousCDO, TArray<UObject*>& ObjLoaded)
{
	return instance->RegenerateClass(ClassToRegenerate, PreviousCDO, ObjLoaded);
}

CSEXPORT csbool CSCONV Export_UObject_IsInBlueprint(UObject* instance)
{
	return instance->IsInBlueprint();
}

CSEXPORT void CSCONV Export_UObject_DestroyNonNativeProperties(UObject* instance)
{
	instance->DestroyNonNativeProperties();
}

void CSCONV Export_UObject(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UObject_PostInitProperties);
	REGISTER_FUNC(Export_UObject_PreSaveRoot);
	REGISTER_FUNC(Export_UObject_PostSaveRoot);
	REGISTER_FUNC(Export_UObject_PreSave);
	REGISTER_FUNC(Export_UObject_Modify);
#if WITH_EDITOR
	REGISTER_FUNC(Export_UObject_LoadedFromAnotherClass);
#endif
	REGISTER_FUNC(Export_UObject_PostLoad);
	REGISTER_FUNC(Export_UObject_PostLoadSubobjects);
	REGISTER_FUNC(Export_UObject_BeginDestroy);
	REGISTER_FUNC(Export_UObject_IsReadyForFinishDestroy);
#if WITH_EDITOR
	REGISTER_FUNC(Export_UObject_PostLinkerChange);
#endif
	REGISTER_FUNC(Export_UObject_FinishDestroy);
	REGISTER_FUNC(Export_UObject_Serialize);
	REGISTER_FUNC(Export_UObject_ShutdownAfterError);
	REGISTER_FUNC(Export_UObject_PostInterpChange);
#if WITH_EDITOR
	REGISTER_FUNC(Export_UObject_PreEditChange);
	REGISTER_FUNC(Export_UObject_PreEditChangeChain);
	REGISTER_FUNC(Export_UObject_CanEditChange);
	REGISTER_FUNC(Export_UObject_PostEditChange);
	REGISTER_FUNC(Export_UObject_PostEditChangeProperty);
	REGISTER_FUNC(Export_UObject_PostEditChangeChainProperty);
	REGISTER_FUNC(Export_UObject_GetTransactionAnnotation);
	REGISTER_FUNC(Export_UObject_PreEditUndo);
	REGISTER_FUNC(Export_UObject_PostEditUndo);
	REGISTER_FUNC(Export_UObject_PostEditUndoAnnotation);
#endif
	REGISTER_FUNC(Export_UObject_PostRename);
	REGISTER_FUNC(Export_UObject_PostDuplicate);
	REGISTER_FUNC(Export_UObject_NeedsLoadForClient);
	REGISTER_FUNC(Export_UObject_NeedsLoadForServer);
	REGISTER_FUNC(Export_UObject_NeedsLoadForEditorGame);
	REGISTER_FUNC(Export_UObject_CanCreateInCurrentContext);
	REGISTER_FUNC(Export_UObject_ExportCustomProperties);
	REGISTER_FUNC(Export_UObject_ImportCustomProperties);
	REGISTER_FUNC(Export_UObject_PostEditImport);
	REGISTER_FUNC(Export_UObject_PostReloadConfig);
	REGISTER_FUNC(Export_UObject_Rename);
	REGISTER_FUNC(Export_UObject_GetDesc);
#if WITH_ENGINE
	REGISTER_FUNC(Export_UObject_GetWorld);
	REGISTER_FUNC(Export_UObject_GetWorldChecked);
	REGISTER_FUNC(Export_UObject_ImplementsGetWorld);
#endif
	REGISTER_FUNC(Export_UObject_GetNativePropertyValues);
	REGISTER_FUNC(Export_UObject_GetResourceSizeBytes);
	REGISTER_FUNC(Export_UObject_GetExporterName);
	REGISTER_FUNC(Export_UObject_IsLocalizedResource);
	REGISTER_FUNC(Export_UObject_AddReferencedObjects);
	REGISTER_FUNC(Export_UObject_CallAddReferencedObjects);
	REGISTER_FUNC(Export_UObject_GetRestoreForUObjectOverwrite);
	REGISTER_FUNC(Export_UObject_AreNativePropertiesIdenticalTo);
	REGISTER_FUNC(Export_UObject_GetAssetRegistryTagsFromSearchableProperties);
	REGISTER_FUNC(Export_UObject_GetAssetRegistryTags);
	REGISTER_FUNC(Export_UObject_IsAsset);
	REGISTER_FUNC(Export_UObject_IsSafeForRootSet);
	REGISTER_FUNC(Export_UObject_TagSubobjects);
	REGISTER_FUNC(Export_UObject_GetLifetimeReplicatedProps);
	REGISTER_FUNC(Export_UObject_IsNameStableForNetworking);
	REGISTER_FUNC(Export_UObject_IsFullNameStableForNetworking);
	REGISTER_FUNC(Export_UObject_IsSupportedForNetworking);
	REGISTER_FUNC(Export_UObject_GetSubobjectsWithStableNamesForNetworking);
	REGISTER_FUNC(Export_UObject_PreNetReceive);
	REGISTER_FUNC(Export_UObject_PostNetReceive);
	REGISTER_FUNC(Export_UObject_IsSelected);
#if WITH_EDITOR
	REGISTER_FUNC(Export_UObject_PropagatePreEditChange);
	REGISTER_FUNC(Export_UObject_PropagatePostEditChange);
#endif
	REGISTER_FUNC(Export_UObject_SerializeScriptProperties);
	REGISTER_FUNC(Export_UObject_ReinitializeProperties);
	REGISTER_FUNC(Export_UObject_GetDetailedInfo);
	REGISTER_FUNC(Export_UObject_ConditionalBeginDestroy);
	REGISTER_FUNC(Export_UObject_ConditionalFinishDestroy);
	REGISTER_FUNC(Export_UObject_ConditionalPostLoad);
	REGISTER_FUNC(Export_UObject_ConditionalPostLoadSubobjects);
#if WITH_EDITOR
	REGISTER_FUNC(Export_UObject_BeginCacheForCookedPlatformData);
	REGISTER_FUNC(Export_UObject_ClearCachedCookedPlatformData);
	REGISTER_FUNC(Export_UObject_ClearAllCachedCookedPlatformData);
	REGISTER_FUNC(Export_UObject_IsCachedCookedPlatformDataLoaded);
#endif
	REGISTER_FUNC(Export_UObject_IsBasedOnArchetype);
	REGISTER_FUNC(Export_UObject_FindFunction);
	REGISTER_FUNC(Export_UObject_FindFunctionChecked);
	REGISTER_FUNC(Export_UObject_CollectDefaultSubobjects);
	REGISTER_FUNC(Export_UObject_CheckDefaultSubobjects);
	REGISTER_FUNC(Export_UObject_SaveConfig);
	REGISTER_FUNC(Export_UObject_UpdateDefaultConfigFile);
	REGISTER_FUNC(Export_UObject_GetDefaultConfigFilename);
	REGISTER_FUNC(Export_UObject_LoadConfig);
	REGISTER_FUNC(Export_UObject_ReloadConfig);
	REGISTER_FUNC(Export_UObject_ParseParms);
	REGISTER_FUNC(Export_UObject_OutputReferencers);
	REGISTER_FUNC(Export_UObject_RetrieveReferencers);
	REGISTER_FUNC(Export_UObject_SetLinker);
	REGISTER_FUNC(Export_UObject_GetArchetypeFromRequiredInfo);
	REGISTER_FUNC(Export_UObject_GetArchetype);
	REGISTER_FUNC(Export_UObject_GetArchetypeInstances);
	REGISTER_FUNC(Export_UObject_InstanceSubobjectTemplates);
	REGISTER_FUNC(Export_UObject_ProcessEvent);
	REGISTER_FUNC(Export_UObject_GetFunctionCallspace);
	REGISTER_FUNC(Export_UObject_CallRemoteFunction);
	REGISTER_FUNC(Export_UObject_CallFunctionByNameWithArguments);
	REGISTER_FUNC(Export_UObject_CallFunction);
	REGISTER_FUNC(Export_UObject_ProcessInternal);
	REGISTER_FUNC(Export_UObject_ProcessConsoleExec);
	REGISTER_FUNC(Export_UObject_SkipFunction);
	REGISTER_FUNC(Export_UObject_RegenerateClass);
	REGISTER_FUNC(Export_UObject_IsInBlueprint);
	REGISTER_FUNC(Export_UObject_DestroyNonNativeProperties);
}
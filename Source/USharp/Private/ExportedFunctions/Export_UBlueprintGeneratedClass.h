CSEXPORT int32 CSCONV Export_UBlueprintGeneratedClass_Get_NumReplicatedProperties(UBlueprintGeneratedClass* instance)
{
	return instance->NumReplicatedProperties;
}

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_Set_NumReplicatedProperties(UBlueprintGeneratedClass* instance, int32 value)
{
	instance->NumReplicatedProperties = value;
}

CSEXPORT TArray<class UDynamicBlueprintBinding*>& CSCONV Export_UBlueprintGeneratedClass_Get_DynamicBindingObjects(UBlueprintGeneratedClass* instance)
{
	return instance->DynamicBindingObjects;
}

CSEXPORT TArray<class UActorComponent*>& CSCONV Export_UBlueprintGeneratedClass_Get_ComponentTemplates(UBlueprintGeneratedClass* instance)
{
	return instance->ComponentTemplates;
}

CSEXPORT TArray<class UTimelineTemplate*>& CSCONV Export_UBlueprintGeneratedClass_Get_Timelines(UBlueprintGeneratedClass* instance)
{
	return instance->Timelines;
}

CSEXPORT class USimpleConstructionScript* CSCONV Export_UBlueprintGeneratedClass_Get_SimpleConstructionScript(UBlueprintGeneratedClass* instance)
{
	return instance->SimpleConstructionScript;
}

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_Set_SimpleConstructionScript(UBlueprintGeneratedClass* instance, class USimpleConstructionScript* value)
{
	instance->SimpleConstructionScript = value;
}

CSEXPORT class UInheritableComponentHandler* CSCONV Export_UBlueprintGeneratedClass_Get_InheritableComponentHandler(UBlueprintGeneratedClass* instance)
{
	return instance->InheritableComponentHandler;
}

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_Set_InheritableComponentHandler(UBlueprintGeneratedClass* instance, class UInheritableComponentHandler* value)
{
	instance->InheritableComponentHandler = value;
}

CSEXPORT UStructProperty* CSCONV Export_UBlueprintGeneratedClass_Get_UberGraphFramePointerProperty(UBlueprintGeneratedClass* instance)
{
	return instance->UberGraphFramePointerProperty;
}

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_Set_UberGraphFramePointerProperty(UBlueprintGeneratedClass* instance, UStructProperty* value)
{
	instance->UberGraphFramePointerProperty = value;
}

CSEXPORT UFunction* CSCONV Export_UBlueprintGeneratedClass_Get_UberGraphFunction(UBlueprintGeneratedClass* instance)
{
	return instance->UberGraphFunction;
}

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_Set_UberGraphFunction(UBlueprintGeneratedClass* instance, UFunction* value)
{
	instance->UberGraphFunction = value;
}

#if WITH_EDITORONLY_DATA
CSEXPORT UObject* CSCONV Export_UBlueprintGeneratedClass_Get_OverridenArchetypeForCDO(UBlueprintGeneratedClass* instance)
{
	return instance->OverridenArchetypeForCDO;
}

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_Set_OverridenArchetypeForCDO(UBlueprintGeneratedClass* instance, UObject* value)
{
	instance->OverridenArchetypeForCDO = value;
}

CSEXPORT TMap<FName,FGuid>& CSCONV Export_UBlueprintGeneratedClass_Get_PropertyGuids(UBlueprintGeneratedClass* instance)
{
	return instance->PropertyGuids;
}
#endif

CSEXPORT TMap<FName, struct FBlueprintCookedComponentInstancingData>& CSCONV Export_UBlueprintGeneratedClass_Get_CookedComponentInstancingData(UBlueprintGeneratedClass* instance)
{
	return instance->CookedComponentInstancingData;
}

CSEXPORT csbool CSCONV Export_UBlueprintGeneratedClass_GetGeneratedClassesHierarchy(const UClass* InClass, TArray<const UBlueprintGeneratedClass*>& OutBPGClasses)
{
	return UBlueprintGeneratedClass::GetGeneratedClassesHierarchy(InClass, OutBPGClasses);
}

CSEXPORT UInheritableComponentHandler* CSCONV Export_UBlueprintGeneratedClass_GetInheritableComponentHandler(UBlueprintGeneratedClass* instance, const csbool bCreateIfNecessary)
{
	return instance->GetInheritableComponentHandler(!!bCreateIfNecessary);
}

CSEXPORT UActorComponent* CSCONV Export_UBlueprintGeneratedClass_FindComponentTemplateByName(UBlueprintGeneratedClass* instance, const FName& TemplateName)
{
	return instance->FindComponentTemplateByName(TemplateName);
}

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_CreateComponentsForActor(const UClass* ThisClass, AActor* Actor)
{
	UBlueprintGeneratedClass::CreateComponentsForActor(ThisClass, Actor);
}

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_CreateTimelineComponent(AActor* Actor, const UTimelineTemplate* TimelineTemplate)
{
	UBlueprintGeneratedClass::CreateTimelineComponent(Actor, TimelineTemplate);
}

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_UpdateCustomPropertyListForPostConstruction(UBlueprintGeneratedClass* instance)
{
	instance->UpdateCustomPropertyListForPostConstruction();
}

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_AddReferencedObjectsInUbergraphFrame(UObject* InThis, FReferenceCollector& Collector)
{
	UBlueprintGeneratedClass::AddReferencedObjectsInUbergraphFrame(InThis, Collector);
}

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_GetUberGraphFrameName(FName& result)
{
	result = UBlueprintGeneratedClass::GetUberGraphFrameName();
}

CSEXPORT csbool CSCONV Export_UBlueprintGeneratedClass_UsePersistentUberGraphFrame()
{
	return UBlueprintGeneratedClass::UsePersistentUberGraphFrame();
}

#if WITH_EDITORONLY_DATA
CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_Get_DebugData(UBlueprintGeneratedClass* instance, FBlueprintDebugData& result)
{
	result = instance->DebugData;
}

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_Set_DebugData(UBlueprintGeneratedClass* instance, const FBlueprintDebugData& value)
{
	instance->DebugData = value;
}

CSEXPORT FBlueprintDebugData& CSCONV Export_UBlueprintGeneratedClass_GetDebugData(UBlueprintGeneratedClass* instance)
{
	return instance->GetDebugData();
}
#endif

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_BindDynamicDelegates(const UClass* ThisClass, UObject* InInstance)
{
	UBlueprintGeneratedClass::BindDynamicDelegates(ThisClass, InInstance);
}

CSEXPORT UDynamicBlueprintBinding* CSCONV Export_UBlueprintGeneratedClass_GetDynamicBindingObject(const UClass* ThisClass, UClass* BindingClass)
{
	return UBlueprintGeneratedClass::GetDynamicBindingObject(ThisClass, BindingClass);
}

#if WITH_EDITOR
CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_UnbindDynamicDelegates(const UClass* ThisClass, UObject* InInstance)
{
	UBlueprintGeneratedClass::UnbindDynamicDelegates(ThisClass, InInstance);
}

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_UnbindDynamicDelegatesForProperty(UBlueprintGeneratedClass* instance, UObject* InInstance, const UObjectProperty* InObjectProperty)
{
	instance->UnbindDynamicDelegatesForProperty(InInstance, InObjectProperty);
}
#endif

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_GetLifetimeBlueprintReplicationList(UBlueprintGeneratedClass* instance, TArray<class FLifetimeProperty>& OutLifetimeProps)
{
	instance->GetLifetimeBlueprintReplicationList(OutLifetimeProps);
}

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass_InstancePreReplication(UBlueprintGeneratedClass* instance, UObject* Obj, class IRepChangedPropertyTracker& ChangedPropertyTracker)
{
	instance->InstancePreReplication(Obj, ChangedPropertyTracker);
}

CSEXPORT void CSCONV Export_UBlueprintGeneratedClass(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Get_NumReplicatedProperties);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Set_NumReplicatedProperties);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Get_DynamicBindingObjects);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Get_ComponentTemplates);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Get_Timelines);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Get_SimpleConstructionScript);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Set_SimpleConstructionScript);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Get_InheritableComponentHandler);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Set_InheritableComponentHandler);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Get_UberGraphFramePointerProperty);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Set_UberGraphFramePointerProperty);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Get_UberGraphFunction);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Set_UberGraphFunction);
#if WITH_EDITORONLY_DATA
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Get_OverridenArchetypeForCDO);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Set_OverridenArchetypeForCDO);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Get_PropertyGuids);
#endif	
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Get_CookedComponentInstancingData);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_GetGeneratedClassesHierarchy);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_GetInheritableComponentHandler);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_FindComponentTemplateByName);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_CreateComponentsForActor);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_CreateTimelineComponent);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_UpdateCustomPropertyListForPostConstruction);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_AddReferencedObjectsInUbergraphFrame);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_GetUberGraphFrameName);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_UsePersistentUberGraphFrame);
#if WITH_EDITORONLY_DATA	
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Get_DebugData);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_Set_DebugData);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_GetDebugData);
#endif	
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_BindDynamicDelegates);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_GetDynamicBindingObject);
#if WITH_EDITOR	
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_UnbindDynamicDelegates);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_UnbindDynamicDelegatesForProperty);
#endif
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_GetLifetimeBlueprintReplicationList);
	REGISTER_FUNC(Export_UBlueprintGeneratedClass_InstancePreReplication);
}
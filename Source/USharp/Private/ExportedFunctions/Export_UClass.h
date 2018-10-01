CSEXPORT UClass::ClassConstructorType Export_UClass_Get_ClassConstructor(UClass* instance)
{
	return instance->ClassConstructor;
}

CSEXPORT void Export_UClass_Set_ClassConstructor(UClass* instance, UClass::ClassConstructorType ClassConstructor)
{
	instance->ClassConstructor = ClassConstructor;
}

CSEXPORT void Export_UClass_Call_ClassConstructor(UClass* instance, const FObjectInitializer& ObjectInitializer)
{
	instance->ClassConstructor(ObjectInitializer);
}

CSEXPORT void Export_UClass_Call_ClassConstructorDirectly(UClass::ClassConstructorType ClassConstructor, const FObjectInitializer& ObjectInitializer)
{
	ClassConstructor(ObjectInitializer);
}

CSEXPORT UClass::ClassVTableHelperCtorCallerType Export_UClass_Get_ClassVTableHelperCtorCaller(UClass* instance)
{
	return instance->ClassVTableHelperCtorCaller;
}

CSEXPORT void Export_UClass_Set_ClassVTableHelperCtorCaller(UClass* instance, UClass::ClassVTableHelperCtorCallerType ClassVTableHelperCtorCaller)
{
	instance->ClassVTableHelperCtorCaller = ClassVTableHelperCtorCaller;
}

CSEXPORT UObject* Export_UClass_Call_ClassVTableHelperCtorCaller(UClass* instance, FVTableHelper& Helper)
{
	return instance->ClassVTableHelperCtorCaller(Helper);
}

CSEXPORT UClass::ClassAddReferencedObjectsType Export_UClass_Get_ClassAddReferencedObjects(UClass* instance)
{
	return instance->ClassAddReferencedObjects;
}

CSEXPORT void Export_UClass_Set_ClassAddReferencedObjects(UClass* instance, UClass::ClassAddReferencedObjectsType ClassAddReferencedObjects)
{
	instance->ClassAddReferencedObjects = ClassAddReferencedObjects;
}

CSEXPORT void Export_UClass_Call_ClassAddReferencedObjects(UClass* instance, UObject* InThis, class FReferenceCollector& Collector)
{
	instance->ClassAddReferencedObjects(InThis, Collector);
}

CSEXPORT EClassFlags CSCONV Export_UClass_Get_ClassFlags(UClass* instance)
{
	return instance->ClassFlags;
}

CSEXPORT void CSCONV Export_UClass_Set_ClassFlags(UClass* instance, EClassFlags value)
{
	instance->ClassFlags = value;
}

CSEXPORT EClassCastFlags CSCONV Export_UClass_Get_ClassCastFlags(UClass* instance)
{
	return instance->ClassCastFlags;
}

CSEXPORT void CSCONV Export_UClass_Set_ClassCastFlags(UClass* instance, EClassCastFlags value)
{
	instance->ClassCastFlags = value;
}

CSEXPORT int32 CSCONV Export_UClass_Get_ClassUnique(UClass* instance)
{
	return instance->ClassUnique;
}

CSEXPORT void CSCONV Export_UClass_Set_ClassUnique(UClass* instance, int32 value)
{
	instance->ClassUnique = value;
}

CSEXPORT UClass* CSCONV Export_UClass_Get_ClassWithin(UClass* instance)
{
	return instance->ClassWithin;
}

CSEXPORT void CSCONV Export_UClass_Set_ClassWithin(UClass* instance, UClass* value)
{
	instance->ClassWithin = value;
}

CSEXPORT UObject* CSCONV Export_UClass_Get_ClassGeneratedBy(UClass* instance)
{
	return instance->ClassGeneratedBy;
}

CSEXPORT void CSCONV Export_UClass_Set_ClassGeneratedBy(UClass* instance, UObject* value)
{
	instance->ClassGeneratedBy = value;
}

CSEXPORT void CSCONV Export_UClass_Get_ClassConfigName(UClass* instance, FName& result)
{
	result = instance->ClassConfigName;
}

CSEXPORT void CSCONV Export_UClass_Set_ClassConfigName(UClass* instance, const FName& value)
{
	instance->ClassConfigName = value;
}

CSEXPORT TArray<FRepRecord>& CSCONV Export_UClass_Get_ClassReps(UClass* instance)
{
	return instance->ClassReps;
}

CSEXPORT void CSCONV Export_UClass_Set_ClassReps(UClass* instance, TArray<FRepRecord>& value)
{
	instance->ClassReps = value;
}

CSEXPORT TArray<UField*>& CSCONV Export_UClass_Get_NetFields(UClass* instance)
{
	return instance->NetFields;
}

CSEXPORT void CSCONV Export_UClass_Set_NetFields(UClass* instance, TArray<UField*>& value)
{
	instance->NetFields = value;
}

#if WITH_EDITOR || HACK_HEADER_GENERATOR
CSEXPORT void CSCONV Export_UClass_GetHideFunctions(UClass* instance, TArray<FString>& OutHideFunctions)
{
	instance->GetHideFunctions(OutHideFunctions);
}

CSEXPORT csbool CSCONV Export_UClass_IsFunctionHidden(UClass* instance, const FString& InFunction)
{
	return instance->IsFunctionHidden(*InFunction);
}

CSEXPORT void CSCONV Export_UClass_GetAutoExpandCategories(UClass* instance, TArray<FString>& OutAutoExpandCategories)
{
	instance->GetAutoExpandCategories(OutAutoExpandCategories);
}

CSEXPORT csbool CSCONV Export_UClass_IsAutoExpandCategory(UClass* instance, const FString& InCategory)
{
	return instance->IsAutoExpandCategory(*InCategory);
}

CSEXPORT void CSCONV Export_UClass_GetAutoCollapseCategories(UClass* instance, TArray<FString>& OutAutoCollapseCategories)
{
	instance->GetAutoCollapseCategories(OutAutoCollapseCategories);
}

CSEXPORT csbool CSCONV Export_UClass_IsAutoCollapseCategory(UClass* instance, const FString& InCategory)
{
	return instance->IsAutoCollapseCategory(*InCategory);
}

CSEXPORT void CSCONV Export_UClass_GetClassGroupNames(UClass* instance, TArray<FString>& OutClassGroupNames)
{
	instance->GetClassGroupNames(OutClassGroupNames);
}

CSEXPORT csbool CSCONV Export_UClass_IsClassGroupName(UClass* instance, const FString& InGroupName)
{
	return instance->IsClassGroupName(*InGroupName);
}
#endif

CSEXPORT void CSCONV Export_UClass_CallAddReferencedObjects(UClass* instance, UObject* This, FReferenceCollector& Collector)
{
	instance->CallAddReferencedObjects(This, Collector);
}

CSEXPORT UObject* CSCONV Export_UClass_Get_ClassDefaultObject(UClass* instance)
{
	return instance->ClassDefaultObject;
}

CSEXPORT void CSCONV Export_UClass_Set_ClassDefaultObject(UClass* instance, UObject* value)
{
	instance->ClassDefaultObject = value;
}

CSEXPORT csbool CSCONV Export_UClass_Get_bCooked(UClass* instance)
{
	return instance->bCooked;
}

CSEXPORT void CSCONV Export_UClass_Set_bCooked(UClass* instance, csbool bCooked)
{
	instance->bCooked = !!bCooked;
}

CSEXPORT void CSCONV Export_UClass_Get_Interfaces(UClass* instance, TArray<FImplementedInterfaceInterop>& OutInterfaces)
{
	for(FImplementedInterface& Interface : instance->Interfaces)
	{
		OutInterfaces.Add(FImplementedInterfaceInterop::FromNative(Interface));
	}
}

CSEXPORT void CSCONV Export_UClass_Set_Interfaces(UClass* instance, TArray<FImplementedInterfaceInterop>& value)
{
	TArray<FImplementedInterface> Interfaces;
	for(FImplementedInterfaceInterop& Interface : value)
	{
		Interfaces.Add(FImplementedInterfaceInterop::ToNative(Interface));
	}
	instance->Interfaces = Interfaces;
}

// Possible RVO, use TArray<FImplementedInterfaceInterop>&* outValue instead?
CSEXPORT void* CSCONV Export_UClass_Get_InterfacesRef(UClass* instance)
{
	return (void*)(&instance->Interfaces);
}

CSEXPORT void CSCONV Export_UClass_PrependStreamWithSuperClass(UClass* instance, UClass& SuperClass)
{
	instance->PrependStreamWithSuperClass(SuperClass);
}

CSEXPORT FGCReferenceTokenStream CSCONV Export_UClass_Get_ReferenceTokenStream(UClass* instance)
{
	return instance->ReferenceTokenStream;
}

CSEXPORT void CSCONV Export_UClass_Set_ReferenceTokenStream(UClass* instance, FGCReferenceTokenStream value)
{
	instance->ReferenceTokenStream = value;
}

#if !(UE_BUILD_TEST || UE_BUILD_SHIPPING)
CSEXPORT FGCDebugReferenceTokenMap CSCONV Export_UClass_Get_DebugTokenMap(UClass* instance)
{
	return instance->DebugTokenMap;
}

CSEXPORT void CSCONV Export_UClass_Set_DebugTokenMap(UClass* instance, FGCDebugReferenceTokenMap value)
{
	instance->DebugTokenMap = value;
}
#endif

CSEXPORT TArray<FNativeFunctionLookup>& CSCONV Export_UClass_Get_NativeFunctionLookupTable(UClass* instance)
{
	return instance->NativeFunctionLookupTable;
}

CSEXPORT void CSCONV Export_UClass_Set_NativeFunctionLookupTable(UClass* instance, TArray<FNativeFunctionLookup>& value)
{
	instance->NativeFunctionLookupTable = value;
}

#if WITH_HOT_RELOAD
CSEXPORT csbool CSCONV Export_UClass_ReplaceNativeFunction(UClass* instance, const FName& InName, FNativeFuncPtr InPointer, csbool bAddToFunctionRemapTable)
{
	return instance->ReplaceNativeFunction(InName, InPointer, !!bAddToFunctionRemapTable);
}
#endif

CSEXPORT UClass* CSCONV Export_UClass_GetAuthoritativeClass(UClass* instance)
{
	return instance->GetAuthoritativeClass();
}

CSEXPORT void CSCONV Export_UClass_AddNativeFunction(UClass* instance, const FString& InName, FNativeFuncPtr InPointer)
{
	instance->AddNativeFunction(*InName, InPointer);
}

CSEXPORT void CSCONV Export_UClass_AddFunctionToFunctionMap(UClass* instance, UFunction* Function, const FName& FuncName)
{
	instance->AddFunctionToFunctionMap(Function, FuncName);
}

CSEXPORT UFunction* CSCONV Export_UClass_FindFunctionByName(UClass* instance, const FName& InName, csbool IncludeSuper)
{
	return instance->FindFunctionByName(InName, IncludeSuper ? EIncludeSuperFlag::IncludeSuper : EIncludeSuperFlag::ExcludeSuper);
}

CSEXPORT void CSCONV Export_UClass_GetConfigName(UClass* instance, FString& result)
{
	result = instance->GetConfigName();
}

CSEXPORT UClass* CSCONV Export_UClass_GetSuperClass(UClass* instance)
{
	return instance->GetSuperClass();
}

CSEXPORT class FFeedbackContext& CSCONV Export_UClass_GetDefaultPropertiesFeedbackContext()
{
	return UClass::GetDefaultPropertiesFeedbackContext();
}

CSEXPORT int32 CSCONV Export_UClass_GetDefaultsCount(UClass* instance)
{
	return instance->GetDefaultsCount();
}

CSEXPORT UObject* CSCONV Export_UClass_GetDefaultObject(UClass* instance, csbool bCreateIfNeeded)
{
	return instance->GetDefaultObject(!!bCreateIfNeeded);
}

CSEXPORT void CSCONV Export_UClass_GetDefaultObjectName(UClass* instance, FName& result)
{
	result = instance->GetDefaultObjectName();
}

CSEXPORT uint8* CSCONV Export_UClass_GetPersistentUberGraphFrame(UClass* instance, UObject* Obj, UFunction* FuncToCheck)
{
	return instance->GetPersistentUberGraphFrame(Obj, FuncToCheck);
}

CSEXPORT void CSCONV Export_UClass_CreatePersistentUberGraphFrame(UClass* instance, UObject* Obj)
{
	instance->CreatePersistentUberGraphFrame(Obj);
}

CSEXPORT void CSCONV Export_UClass_DestroyPersistentUberGraphFrame(UClass* instance, UObject* Obj)
{
	instance->DestroyPersistentUberGraphFrame(Obj);
}

CSEXPORT UObject* CSCONV Export_UClass_GetDefaultSubobjectByName(UClass* instance, const FName& ToFind)
{
	return instance->GetDefaultSubobjectByName(ToFind);
}

CSEXPORT void CSCONV Export_UClass_AddDefaultSubobject(UClass* instance, UObject* NewSubobject, UClass* BaseClass)
{
	instance->AddDefaultSubobject(NewSubobject, BaseClass);
}

CSEXPORT void CSCONV Export_UClass_GetDefaultObjectSubobjects(UClass* instance, TArray<UObject*>& OutDefaultSubobjects)
{
	instance->GetDefaultObjectSubobjects(OutDefaultSubobjects);
}

CSEXPORT csbool CSCONV Export_UClass_HasAnyClassFlags(UClass* instance, EClassFlags FlagsToCheck)
{
	return instance->HasAnyClassFlags(FlagsToCheck);
}

CSEXPORT csbool CSCONV Export_UClass_HasAllClassFlags(UClass* instance, EClassFlags FlagsToCheck)
{
	return instance->HasAllClassFlags(FlagsToCheck);
}

CSEXPORT EClassFlags CSCONV Export_UClass_GetClassFlags(UClass* instance)
{
	return instance->GetClassFlags();
}

CSEXPORT csbool CSCONV Export_UClass_HasAnyCastFlag(UClass* instance, EClassCastFlags FlagToCheck)
{
	return instance->HasAnyCastFlag(FlagToCheck);
}

CSEXPORT csbool CSCONV Export_UClass_HasAllCastFlags(UClass* instance, EClassCastFlags FlagToCheck)
{
	return instance->HasAllCastFlags(FlagToCheck);
}

CSEXPORT void CSCONV Export_UClass_GetDescription(UClass* instance, FString& result)
{
	result = instance->GetDescription();
}

CSEXPORT void CSCONV Export_UClass_EmitObjectReference(UClass* instance, int32 Offset, const FName& DebugName, EGCReferenceType Kind)
{
	instance->EmitObjectReference(Offset, DebugName, Kind);
}

CSEXPORT void CSCONV Export_UClass_EmitObjectArrayReference(UClass* instance, int32 Offset, const FName& DebugName)
{
	instance->EmitObjectArrayReference(Offset, DebugName);
}

CSEXPORT int32 CSCONV Export_UClass_EmitStructArrayBegin(UClass* instance, int32 Offset, const FName& DebugName, int32 Stride)
{
	return instance->EmitStructArrayBegin(Offset, DebugName, Stride);
}

CSEXPORT void CSCONV Export_UClass_EmitStructArrayEnd(UClass* instance, uint32 SkipIndexIndex)
{
	instance->EmitStructArrayEnd(SkipIndexIndex);
}

CSEXPORT void CSCONV Export_UClass_EmitFixedArrayBegin(UClass* instance, int32 Offset, const FName& DebugName, int32 Stride, int32 Count)
{
	instance->EmitFixedArrayBegin(Offset, DebugName, Stride, Count);
}

CSEXPORT void CSCONV Export_UClass_EmitFixedArrayEnd(UClass* instance)
{
	instance->EmitFixedArrayEnd();
}

CSEXPORT void CSCONV Export_UClass_AssembleReferenceTokenStream(UClass* instance, csbool bForce)
{
	instance->AssembleReferenceTokenStream(!!bForce);
}

CSEXPORT csbool CSCONV Export_UClass_ImplementsInterface(UClass* instance, const class UClass* SomeInterface)
{
	return instance->ImplementsInterface(SomeInterface);
}

CSEXPORT void CSCONV Export_UClass_SerializeDefaultObject(UClass* instance, UObject* Object, FArchive& Ar)
{
	instance->SerializeDefaultObject(Object, Ar);
}

CSEXPORT void CSCONV Export_UClass_PurgeClass(UClass* instance, csbool bRecompilingOnLoad)
{
	instance->PurgeClass(!!bRecompilingOnLoad);
}

CSEXPORT UClass* CSCONV Export_UClass_FindCommonBase(UClass* InClassA, UClass* InClassB)
{
	return UClass::FindCommonBase(InClassA, InClassB);
}

CSEXPORT UClass* CSCONV Export_UClass_FindCommonBaseMany(const TArray<UClass*>& InClasses)
{
	return UClass::FindCommonBase(InClasses);
}

CSEXPORT csbool CSCONV Export_UClass_IsFunctionImplementedInBlueprint(UClass* instance, const FName& InFunctionName)
{
	return instance->IsFunctionImplementedInBlueprint(InFunctionName);
}

CSEXPORT csbool CSCONV Export_UClass_HasProperty(UClass* instance, UProperty* InProperty)
{
	return instance->HasProperty(InProperty);
}

CSEXPORT void CSCONV Export_UClass(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UClass_Get_ClassConstructor);
	REGISTER_FUNC(Export_UClass_Set_ClassConstructor);
	REGISTER_FUNC(Export_UClass_Call_ClassConstructor);
	REGISTER_FUNC(Export_UClass_Call_ClassConstructorDirectly);
	REGISTER_FUNC(Export_UClass_Get_ClassVTableHelperCtorCaller);
	REGISTER_FUNC(Export_UClass_Set_ClassVTableHelperCtorCaller);
	REGISTER_FUNC(Export_UClass_Call_ClassVTableHelperCtorCaller);	
	REGISTER_FUNC(Export_UClass_Get_ClassAddReferencedObjects);
	REGISTER_FUNC(Export_UClass_Set_ClassAddReferencedObjects);	
	REGISTER_FUNC(Export_UClass_Call_ClassAddReferencedObjects);
	REGISTER_FUNC(Export_UClass_Get_ClassFlags);
	REGISTER_FUNC(Export_UClass_Set_ClassFlags);
	REGISTER_FUNC(Export_UClass_Get_ClassCastFlags);
	REGISTER_FUNC(Export_UClass_Set_ClassCastFlags);
	REGISTER_FUNC(Export_UClass_Get_ClassUnique);
	REGISTER_FUNC(Export_UClass_Set_ClassUnique);
	REGISTER_FUNC(Export_UClass_Get_ClassWithin);
	REGISTER_FUNC(Export_UClass_Set_ClassWithin);
	REGISTER_FUNC(Export_UClass_Get_ClassGeneratedBy);
	REGISTER_FUNC(Export_UClass_Set_ClassGeneratedBy);
	REGISTER_FUNC(Export_UClass_Get_ClassConfigName);
	REGISTER_FUNC(Export_UClass_Set_ClassConfigName);
	REGISTER_FUNC(Export_UClass_Get_ClassReps);
	REGISTER_FUNC(Export_UClass_Set_ClassReps);
	REGISTER_FUNC(Export_UClass_Get_NetFields);
	REGISTER_FUNC(Export_UClass_Set_NetFields);
#if WITH_EDITOR || HACK_HEADER_GENERATOR 
	REGISTER_FUNC(Export_UClass_GetHideFunctions);
	REGISTER_FUNC(Export_UClass_IsFunctionHidden);
	REGISTER_FUNC(Export_UClass_GetAutoExpandCategories);
	REGISTER_FUNC(Export_UClass_IsAutoExpandCategory);
	REGISTER_FUNC(Export_UClass_GetAutoCollapseCategories);
	REGISTER_FUNC(Export_UClass_IsAutoCollapseCategory);
	REGISTER_FUNC(Export_UClass_GetClassGroupNames);
	REGISTER_FUNC(Export_UClass_IsClassGroupName);
#endif
	REGISTER_FUNC(Export_UClass_CallAddReferencedObjects);
	REGISTER_FUNC(Export_UClass_Get_ClassDefaultObject);
	REGISTER_FUNC(Export_UClass_Set_ClassDefaultObject);
	REGISTER_FUNC(Export_UClass_Get_bCooked);
	REGISTER_FUNC(Export_UClass_Set_bCooked);
	REGISTER_FUNC(Export_UClass_Get_Interfaces);
	REGISTER_FUNC(Export_UClass_Set_Interfaces);
	REGISTER_FUNC(Export_UClass_Get_InterfacesRef);
	REGISTER_FUNC(Export_UClass_PrependStreamWithSuperClass);
	REGISTER_FUNC(Export_UClass_Get_ReferenceTokenStream);
	REGISTER_FUNC(Export_UClass_Set_ReferenceTokenStream);
#if !(UE_BUILD_TEST || UE_BUILD_SHIPPING)
	REGISTER_FUNC(Export_UClass_Get_DebugTokenMap);
	REGISTER_FUNC(Export_UClass_Set_DebugTokenMap);
#endif
	REGISTER_FUNC(Export_UClass_Get_NativeFunctionLookupTable);
	REGISTER_FUNC(Export_UClass_Set_NativeFunctionLookupTable);
#if WITH_HOT_RELOAD
	REGISTER_FUNC(Export_UClass_ReplaceNativeFunction);
#endif
	REGISTER_FUNC(Export_UClass_GetAuthoritativeClass);
	REGISTER_FUNC(Export_UClass_AddNativeFunction);
	REGISTER_FUNC(Export_UClass_AddFunctionToFunctionMap);
	REGISTER_FUNC(Export_UClass_FindFunctionByName);
	REGISTER_FUNC(Export_UClass_GetConfigName);
	REGISTER_FUNC(Export_UClass_GetSuperClass);
	REGISTER_FUNC(Export_UClass_GetDefaultPropertiesFeedbackContext);
	REGISTER_FUNC(Export_UClass_GetDefaultsCount);
	REGISTER_FUNC(Export_UClass_GetDefaultObject);
	REGISTER_FUNC(Export_UClass_GetDefaultObjectName);
	REGISTER_FUNC(Export_UClass_GetPersistentUberGraphFrame);
	REGISTER_FUNC(Export_UClass_CreatePersistentUberGraphFrame);
	REGISTER_FUNC(Export_UClass_DestroyPersistentUberGraphFrame);
	REGISTER_FUNC(Export_UClass_GetDefaultSubobjectByName);
	REGISTER_FUNC(Export_UClass_AddDefaultSubobject);
	REGISTER_FUNC(Export_UClass_GetDefaultObjectSubobjects);
	REGISTER_FUNC(Export_UClass_HasAnyClassFlags);
	REGISTER_FUNC(Export_UClass_HasAllClassFlags);
	REGISTER_FUNC(Export_UClass_GetClassFlags);
	REGISTER_FUNC(Export_UClass_HasAnyCastFlag);
	REGISTER_FUNC(Export_UClass_HasAllCastFlags);
	REGISTER_FUNC(Export_UClass_GetDescription);
	REGISTER_FUNC(Export_UClass_EmitObjectReference);
	REGISTER_FUNC(Export_UClass_EmitObjectArrayReference);
	REGISTER_FUNC(Export_UClass_EmitStructArrayBegin);
	REGISTER_FUNC(Export_UClass_EmitStructArrayEnd);
	REGISTER_FUNC(Export_UClass_EmitFixedArrayBegin);
	REGISTER_FUNC(Export_UClass_EmitFixedArrayEnd);
	REGISTER_FUNC(Export_UClass_AssembleReferenceTokenStream);
	REGISTER_FUNC(Export_UClass_ImplementsInterface);
	REGISTER_FUNC(Export_UClass_SerializeDefaultObject);
	REGISTER_FUNC(Export_UClass_PurgeClass);
	REGISTER_FUNC(Export_UClass_FindCommonBase);
	REGISTER_FUNC(Export_UClass_FindCommonBaseMany);
	REGISTER_FUNC(Export_UClass_IsFunctionImplementedInBlueprint);
	REGISTER_FUNC(Export_UClass_HasProperty);
}
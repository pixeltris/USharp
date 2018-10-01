CSEXPORT csbool CSCONV Export_UObjectGlobals_Get_GIsSavingPackage()
{
	return GIsSavingPackage;
}

CSEXPORT csbool CSCONV Export_UObjectGlobals_IsGarbageCollecting()
{
	return IsGarbageCollecting();
}

CSEXPORT void CSCONV Export_UObjectGlobals_CollectGarbage(EObjectFlags KeepFlags, csbool bPerformFullPurge)
{
	CollectGarbage(KeepFlags, !!bPerformFullPurge);
}

CSEXPORT void CSCONV Export_UObjectGlobals_CollectGarbageDefault()
{
	CollectGarbage(GARBAGE_COLLECTION_KEEPFLAGS);
}

CSEXPORT csbool CSCONV Export_UObjectGlobals_TryCollectGarbage(EObjectFlags KeepFlags, csbool bPerformFullPurge)
{
	return TryCollectGarbage(KeepFlags, !!bPerformFullPurge);
}

CSEXPORT csbool CSCONV Export_UObjectGlobals_TryCollectGarbageDefault()
{
	return TryCollectGarbage(GARBAGE_COLLECTION_KEEPFLAGS);
}

CSEXPORT csbool CSCONV Export_UObjectGlobals_IsIncrementalPurgePending()
{
	return IsIncrementalPurgePending();
}

CSEXPORT void CSCONV Export_UObjectGlobals_IncrementalPurgeGarbage(csbool bUseTimeLimit, float TimeLimit)
{
	IncrementalPurgeGarbage(!!bUseTimeLimit, TimeLimit);
}

CSEXPORT void CSCONV Export_UObjectGlobals_MakeUniqueObjectName(UObject* Outer, UClass* Class, const FName& BaseName, FName& result)
{
	result = MakeUniqueObjectName(Outer, Class, BaseName);
}

CSEXPORT void CSCONV Export_UObjectGlobals_MakeObjectNameFromDisplayLabel(const FString& DisplayLabel, const FName& CurrentObjectName, FName& result)
{
	result = MakeObjectNameFromDisplayLabel(DisplayLabel, CurrentObjectName);
}

CSEXPORT csbool CSCONV Export_UObjectGlobals_IsReferenced(UObject*& Res, EObjectFlags KeepFlags, EInternalObjectFlags InternalKeepFlags, csbool bCheckSubObjects, FReferencerInformationList* FoundReferences)
{
	return IsReferenced(Res, KeepFlags, InternalKeepFlags, !!bCheckSubObjects, FoundReferences);
}

CSEXPORT csbool CSCONV Export_UObjectGlobals_IsLoading()
{
	return IsLoading();
}

CSEXPORT UPackage* CSCONV Export_UObjectGlobals_GetTransientPackage()
{
	return GetTransientPackage();
}

#if DO_CHECK
CSEXPORT void CSCONV Export_UObjectGlobals_CheckIsClassChildOf_Internal(UClass* Parent, UClass* Child)
{
	CheckIsClassChildOf_Internal(Parent, Child);
}
#endif

CSEXPORT UObject* CSCONV Export_UObjectGlobals_StaticConstructObject_Internal(UClass* Class, UObject* InOuter, const FName& Name, EObjectFlags SetFlags, EInternalObjectFlags InternalSetFlags, UObject* Template, csbool bCopyTransientsFromClassDefaults, struct FObjectInstancingGraph* InstanceGraph)
{
	return StaticConstructObject_Internal(Class, InOuter, Name, SetFlags, InternalSetFlags, Template, !!bCopyTransientsFromClassDefaults, InstanceGraph);
}

CSEXPORT UObject* CSCONV Export_UObjectGlobals_StaticDuplicateObject(UObject const* SourceObject, UObject* DestOuter, const FName& DestName, EObjectFlags FlagMask, UClass* DestClass, EDuplicateMode::Type DuplicateMode, EInternalObjectFlags InternalFlagsMask)
{
	return StaticDuplicateObject(SourceObject, DestOuter, DestName, FlagMask, DestClass, DuplicateMode, InternalFlagsMask);
}

CSEXPORT UObject* CSCONV Export_UObjectGlobals_StaticFindObjectFast(UClass* Class, UObject* InOuter, const FName& InName, csbool ExactClass, csbool AnyPackage, EObjectFlags ExclusiveFlags, EInternalObjectFlags ExclusiveInternalFlags)
{
	return StaticFindObjectFast(Class, InOuter, InName, !!ExactClass, !!AnyPackage, ExclusiveFlags, ExclusiveInternalFlags);
}

CSEXPORT UObject* CSCONV Export_UObjectGlobals_StaticFindObject(UClass* Class, UObject* InOuter, const FString& Name, csbool ExactClass)
{
	return StaticFindObject(Class, InOuter, *Name, !!ExactClass);
}

CSEXPORT UObject* CSCONV Export_UObjectGlobals_StaticFindObjectChecked(UClass* Class, UObject* InOuter, const FString& Name, csbool ExactClass)
{
	return StaticFindObjectChecked(Class, InOuter, *Name, !!ExactClass);
}

CSEXPORT UObject* CSCONV Export_UObjectGlobals_StaticFindObjectSafe(UClass* Class, UObject* InOuter, const FString& Name, csbool ExactClass)
{
	return StaticFindObjectSafe(Class, InOuter, *Name, !!ExactClass);
}

CSEXPORT UObject* CSCONV Export_UObjectGlobals_StaticLoadObject(UClass* Class, UObject* InOuter, const FString& Name, const FString& Filename, uint32 LoadFlags, UPackageMap* Sandbox, csbool bAllowObjectReconciliation)
{
	return StaticLoadObject(Class, InOuter, *Name, *Filename, LoadFlags, Sandbox, !!bAllowObjectReconciliation);
}

CSEXPORT UClass* CSCONV Export_UObjectGlobals_StaticLoadClass(UClass* BaseClass, UObject* InOuter, const FString& Name, const FString& Filename, uint32 LoadFlags, UPackageMap* Sandbox)
{
	return StaticLoadClass(BaseClass, InOuter, *Name, *Filename, LoadFlags, Sandbox);
}

CSEXPORT UPackage* CSCONV Export_UObjectGlobals_LoadPackage(UPackage* InOuter, const FString& InLongPackageName, uint32 LoadFlags)
{
	return LoadPackage(InOuter, *InLongPackageName, LoadFlags);
}

CSEXPORT UPackage* CSCONV Export_UObjectGlobals_FindPackage(UPackage* InOuter, const FString& PackageName)
{
	return FindPackage(InOuter, *PackageName);
}

CSEXPORT UPackage* CSCONV Export_UObjectGlobals_CreatePackage(UPackage* InOuter, const FString& PackageName)
{
	return CreatePackage(InOuter, *PackageName);
}

CSEXPORT UObject* CSCONV Export_UObjectGlobals_StaticAllocateObject(UClass* Class, UObject* InOuter, const FName& Name, EObjectFlags SetFlags, EInternalObjectFlags InternalSetFlags, csbool bCanReuseSubobjects, csbool* bOutReusedSubobject)
{	
	UObject* Result = nullptr;
	if(bOutReusedSubobject != nullptr)
	{
		bool bOutReusedSubobjectNative;
		StaticAllocateObject(Class, InOuter, Name, SetFlags, InternalSetFlags, !!bCanReuseSubobjects, &bOutReusedSubobjectNative);
		*bOutReusedSubobject = bOutReusedSubobjectNative;
	}
	else
	{
		Result = StaticAllocateObject(Class, InOuter, Name, SetFlags, InternalSetFlags, !!bCanReuseSubobjects, nullptr);
	}
	return Result;
}

CSEXPORT void CSCONV Export_UObjectGlobals(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UObjectGlobals_Get_GIsSavingPackage);
	REGISTER_FUNC(Export_UObjectGlobals_IsGarbageCollecting);
	REGISTER_FUNC(Export_UObjectGlobals_CollectGarbage);
	REGISTER_FUNC(Export_UObjectGlobals_CollectGarbageDefault);
	REGISTER_FUNC(Export_UObjectGlobals_TryCollectGarbage);
	REGISTER_FUNC(Export_UObjectGlobals_TryCollectGarbageDefault);
	REGISTER_FUNC(Export_UObjectGlobals_IsIncrementalPurgePending);
	REGISTER_FUNC(Export_UObjectGlobals_IncrementalPurgeGarbage);
	REGISTER_FUNC(Export_UObjectGlobals_MakeUniqueObjectName);
	REGISTER_FUNC(Export_UObjectGlobals_MakeObjectNameFromDisplayLabel);
	REGISTER_FUNC(Export_UObjectGlobals_IsReferenced);
	REGISTER_FUNC(Export_UObjectGlobals_IsLoading);
	REGISTER_FUNC(Export_UObjectGlobals_GetTransientPackage);
#if DO_CHECK
	REGISTER_FUNC(Export_UObjectGlobals_CheckIsClassChildOf_Internal);
#endif
	REGISTER_FUNC(Export_UObjectGlobals_StaticConstructObject_Internal);
	REGISTER_FUNC(Export_UObjectGlobals_StaticDuplicateObject);
	REGISTER_FUNC(Export_UObjectGlobals_StaticFindObjectFast);
	REGISTER_FUNC(Export_UObjectGlobals_StaticFindObject);
	REGISTER_FUNC(Export_UObjectGlobals_StaticFindObjectChecked);
	REGISTER_FUNC(Export_UObjectGlobals_StaticFindObjectSafe);
	REGISTER_FUNC(Export_UObjectGlobals_StaticLoadObject);
	REGISTER_FUNC(Export_UObjectGlobals_StaticLoadClass);
	REGISTER_FUNC(Export_UObjectGlobals_LoadPackage);
	REGISTER_FUNC(Export_UObjectGlobals_FindPackage);
	REGISTER_FUNC(Export_UObjectGlobals_CreatePackage);
	REGISTER_FUNC(Export_UObjectGlobals_StaticAllocateObject);
}
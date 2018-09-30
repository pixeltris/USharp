CSEXPORT void CSCONV Export_UObjectBaseUtility_SetFlags(UObjectBaseUtility* instance, EObjectFlags NewFlags)
{
	instance->SetFlags(NewFlags);
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_ClearFlags(UObjectBaseUtility* instance, EObjectFlags NewFlags)
{
	instance->ClearFlags(NewFlags);
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_HasAnyFlags(UObjectBaseUtility* instance, EObjectFlags FlagsToCheck)
{
	return instance->HasAnyFlags(FlagsToCheck);
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_HasAllFlags(UObjectBaseUtility* instance, EObjectFlags FlagsToCheck)
{
	return instance->HasAllFlags(FlagsToCheck);
}

CSEXPORT EObjectFlags CSCONV Export_UObjectBaseUtility_GetMaskedFlags(UObjectBaseUtility* instance, EObjectFlags Mask)
{
	return instance->GetMaskedFlags(Mask);
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_Mark(UObjectBaseUtility* instance, EObjectMark Marks)
{
	instance->Mark(Marks);
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_UnMark(UObjectBaseUtility* instance, EObjectMark Marks)
{
	instance->UnMark(Marks);
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_HasAnyMarks(UObjectBaseUtility* instance, EObjectMark Marks)
{
	return instance->HasAnyMarks(Marks);
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_HasAllMarks(UObjectBaseUtility* instance, EObjectMark Marks)
{
	return instance->HasAllMarks(Marks);
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_IsPendingKill(UObjectBaseUtility* instance)
{
	return instance->IsPendingKill();
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_MarkPendingKill(UObjectBaseUtility* instance)
{
	instance->MarkPendingKill();
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_ClearPendingKill(UObjectBaseUtility* instance)
{
	instance->ClearPendingKill();
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_AddToRoot(UObjectBaseUtility* instance)
{
	instance->AddToRoot();
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_RemoveFromRoot(UObjectBaseUtility* instance)
{
	instance->RemoveFromRoot();
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_IsRooted(UObjectBaseUtility* instance)
{
	return instance->IsRooted();
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_ThisThreadAtomicallyClearedRFUnreachable(UObjectBaseUtility* instance)
{
	return instance->ThisThreadAtomicallyClearedRFUnreachable();
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_IsUnreachable(UObjectBaseUtility* instance)
{
	return instance->IsUnreachable();
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_IsPendingKillOrUnreachable(UObjectBaseUtility* instance)
{
	return instance->IsPendingKillOrUnreachable();
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_IsNative(UObjectBaseUtility* instance)
{
	return instance->IsNative();
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_SetInternalFlags(UObjectBaseUtility* instance, EInternalObjectFlags FlagsToSet)
{
	instance->SetInternalFlags(FlagsToSet);
}

CSEXPORT EInternalObjectFlags CSCONV Export_UObjectBaseUtility_GetInternalFlags(UObjectBaseUtility* instance)
{
	return instance->GetInternalFlags();
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_HasAnyInternalFlags(UObjectBaseUtility* instance, EInternalObjectFlags FlagsToCheck)
{
	return instance->HasAnyInternalFlags(FlagsToCheck);
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_ClearInternalFlags(UObjectBaseUtility* instance, EInternalObjectFlags FlagsToClear)
{
	instance->ClearInternalFlags(FlagsToClear);
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_AtomicallyClearInternalFlags(UObjectBaseUtility* instance, EInternalObjectFlags FlagsToClear)
{
	return instance->AtomicallyClearInternalFlags(FlagsToClear);
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_GetFullName(UObjectBaseUtility* instance, const UObject* StopOuter, FString& result)
{
	result = instance->GetFullName(StopOuter);
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_GetPathName(UObjectBaseUtility* instance, const UObject* StopOuter, FString& result)
{
	result = instance->GetPathName(StopOuter);
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_CanBeClusterRoot(UObjectBaseUtility* instance)
{
	return instance->CanBeClusterRoot();
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_CanBeInCluster(UObjectBaseUtility* instance)
{
	return instance->CanBeInCluster();
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_CreateCluster(UObjectBaseUtility* instance)
{
	instance->CreateCluster();
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_OnClusterMarkedAsPendingKill(UObjectBaseUtility* instance)
{
	instance->OnClusterMarkedAsPendingKill();
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_AddToCluster(UObjectBaseUtility* instance, UObjectBaseUtility* ClusterRootOrObjectFromCluster, csbool bAddAsMutableObject)
{
	instance->AddToCluster(ClusterRootOrObjectFromCluster, !!bAddAsMutableObject);
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_GetFullGroupName(UObjectBaseUtility* instance, csbool bStartWithOuter, FString& result)
{
	result = instance->GetFullGroupName(!!bStartWithOuter);
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_GetName(UObjectBaseUtility* instance, FString& result)
{
	result = instance->GetName();
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_GetNameOut(UObjectBaseUtility* instance, FString& ResultString)
{
	instance->GetName(ResultString);
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_AppendName(UObjectBaseUtility* instance, FString& ResultString)
{
	instance->AppendName(ResultString);
}

CSEXPORT UPackage* CSCONV Export_UObjectBaseUtility_GetOutermost(UObjectBaseUtility* instance)
{
	return instance->GetOutermost();
}

CSEXPORT void CSCONV Export_UObjectBaseUtility_MarkPackageDirty(UObjectBaseUtility* instance)
{
	instance->MarkPackageDirty();
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_IsTemplate(UObjectBaseUtility* instance, EObjectFlags TemplateTypes)
{
	return instance->IsTemplate(TemplateTypes);
}

CSEXPORT UObject* CSCONV Export_UObjectBaseUtility_GetTypedOuter(UObjectBaseUtility* instance, UClass* Target)
{
	return instance->GetTypedOuter(Target);
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_IsIn(UObjectBaseUtility* instance, const UObject* SomeOuter)
{
	return instance->IsIn(SomeOuter);
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_IsInA(UObjectBaseUtility* instance, const UClass* SomeBaseClass)
{
	return instance->IsInA(SomeBaseClass);
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_RootPackageHasAnyFlags(UObjectBaseUtility* instance, uint32 CheckFlagMask)
{
	return instance->RootPackageHasAnyFlags(CheckFlagMask);
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_IsA(UObjectBaseUtility* instance, const UClass* SomeBaseClass)
{
	return instance->IsA(SomeBaseClass);
}

CSEXPORT const UClass* CSCONV Export_UObjectBaseUtility_FindNearestCommonBaseClass(UObjectBaseUtility* instance, const UClass* TestClass)
{
	return instance->FindNearestCommonBaseClass(TestClass);
}

CSEXPORT void* CSCONV Export_UObjectBaseUtility_GetInterfaceAddress(UObjectBaseUtility* instance, UClass* InterfaceClass)
{
	return instance->GetInterfaceAddress(InterfaceClass);
}

CSEXPORT void* CSCONV Export_UObjectBaseUtility_GetNativeInterfaceAddress(UObjectBaseUtility* instance, UClass* InterfaceClass)
{
	return instance->GetNativeInterfaceAddress(InterfaceClass);
}

CSEXPORT csbool CSCONV Export_UObjectBaseUtility_IsDefaultSubobject(UObjectBaseUtility* instance)
{
	return instance->IsDefaultSubobject();
}

CSEXPORT FLinkerLoad* CSCONV Export_UObjectBaseUtility_GetLinker(UObjectBaseUtility* instance)
{
	return instance->GetLinker();
}

CSEXPORT int32 CSCONV Export_UObjectBaseUtility_GetLinkerIndex(UObjectBaseUtility* instance)
{
	return instance->GetLinkerIndex();
}

CSEXPORT int32 CSCONV Export_UObjectBaseUtility_GetLinkerUE4Version(UObjectBaseUtility* instance)
{
	return instance->GetLinkerUE4Version();
}

CSEXPORT int32 CSCONV Export_UObjectBaseUtility_GetLinkerLicenseeUE4Version(UObjectBaseUtility* instance)
{
	return instance->GetLinkerLicenseeUE4Version();
}

CSEXPORT int32 CSCONV Export_UObjectBaseUtility_GetLinkerCustomVersion(UObjectBaseUtility* instance, const FGuid& CustomVersionKey)
{
	return instance->GetLinkerCustomVersion(CustomVersionKey);
}

CSEXPORT void CSCONV Export_UObjectBaseUtility(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UObjectBaseUtility_SetFlags);
	REGISTER_FUNC(Export_UObjectBaseUtility_ClearFlags);
	REGISTER_FUNC(Export_UObjectBaseUtility_HasAnyFlags);
	REGISTER_FUNC(Export_UObjectBaseUtility_HasAllFlags);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetMaskedFlags);
	REGISTER_FUNC(Export_UObjectBaseUtility_Mark);
	REGISTER_FUNC(Export_UObjectBaseUtility_UnMark);
	REGISTER_FUNC(Export_UObjectBaseUtility_HasAnyMarks);
	REGISTER_FUNC(Export_UObjectBaseUtility_HasAllMarks);
	REGISTER_FUNC(Export_UObjectBaseUtility_IsPendingKill);
	REGISTER_FUNC(Export_UObjectBaseUtility_MarkPendingKill);
	REGISTER_FUNC(Export_UObjectBaseUtility_ClearPendingKill);
	REGISTER_FUNC(Export_UObjectBaseUtility_AddToRoot);
	REGISTER_FUNC(Export_UObjectBaseUtility_RemoveFromRoot);
	REGISTER_FUNC(Export_UObjectBaseUtility_IsRooted);
	REGISTER_FUNC(Export_UObjectBaseUtility_ThisThreadAtomicallyClearedRFUnreachable);
	REGISTER_FUNC(Export_UObjectBaseUtility_IsUnreachable);
	REGISTER_FUNC(Export_UObjectBaseUtility_IsPendingKillOrUnreachable);
	REGISTER_FUNC(Export_UObjectBaseUtility_IsNative);
	REGISTER_FUNC(Export_UObjectBaseUtility_SetInternalFlags);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetInternalFlags);
	REGISTER_FUNC(Export_UObjectBaseUtility_HasAnyInternalFlags);
	REGISTER_FUNC(Export_UObjectBaseUtility_ClearInternalFlags);
	REGISTER_FUNC(Export_UObjectBaseUtility_AtomicallyClearInternalFlags);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetFullName);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetPathName);
	REGISTER_FUNC(Export_UObjectBaseUtility_CanBeClusterRoot);
	REGISTER_FUNC(Export_UObjectBaseUtility_CanBeInCluster);
	REGISTER_FUNC(Export_UObjectBaseUtility_CreateCluster);
	REGISTER_FUNC(Export_UObjectBaseUtility_OnClusterMarkedAsPendingKill);
	REGISTER_FUNC(Export_UObjectBaseUtility_AddToCluster);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetFullGroupName);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetName);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetNameOut);
	REGISTER_FUNC(Export_UObjectBaseUtility_AppendName);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetOutermost);
	REGISTER_FUNC(Export_UObjectBaseUtility_MarkPackageDirty);
	REGISTER_FUNC(Export_UObjectBaseUtility_IsTemplate);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetTypedOuter);
	REGISTER_FUNC(Export_UObjectBaseUtility_IsIn);
	REGISTER_FUNC(Export_UObjectBaseUtility_IsInA);
	REGISTER_FUNC(Export_UObjectBaseUtility_RootPackageHasAnyFlags);
	REGISTER_FUNC(Export_UObjectBaseUtility_IsA);
	REGISTER_FUNC(Export_UObjectBaseUtility_FindNearestCommonBaseClass);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetInterfaceAddress);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetNativeInterfaceAddress);
	REGISTER_FUNC(Export_UObjectBaseUtility_IsDefaultSubobject);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetLinker);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetLinkerIndex);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetLinkerUE4Version);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetLinkerLicenseeUE4Version);
	REGISTER_FUNC(Export_UObjectBaseUtility_GetLinkerCustomVersion);
}
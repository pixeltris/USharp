CSEXPORT UField* CSCONV Export_UStruct_Get_Children(UStruct* instance)
{
	return instance->Children;
}

CSEXPORT void CSCONV Export_UStruct_Set_Children(UStruct* instance, UField* value)
{
	instance->Children = value;
}

CSEXPORT int32 CSCONV Export_UStruct_Get_PropertiesSize(UStruct* instance)
{
	return instance->PropertiesSize;
}

CSEXPORT void CSCONV Export_UStruct_Set_PropertiesSize(UStruct* instance, int32 value)
{
	instance->PropertiesSize = value;
}

CSEXPORT TArray<uint8>& CSCONV Export_UStruct_Get_Script(UStruct* instance)
{
	return instance->Script;
}

CSEXPORT void CSCONV Export_UStruct_Set_Script(UStruct* instance, TArray<uint8>& value)
{
	instance->Script = value;
}

CSEXPORT int32 CSCONV Export_UStruct_Get_MinAlignment(UStruct* instance)
{
	return instance->MinAlignment;
}

CSEXPORT void CSCONV Export_UStruct_Set_MinAlignment(UStruct* instance, int32 value)
{
	instance->MinAlignment = value;
}

CSEXPORT UProperty* CSCONV Export_UStruct_Get_PropertyLink(UStruct* instance)
{
	return instance->PropertyLink;
}

CSEXPORT void CSCONV Export_UStruct_Set_PropertyLink(UStruct* instance, UProperty* value)
{
	instance->PropertyLink = value;
}

CSEXPORT UProperty* CSCONV Export_UStruct_Get_RefLink(UStruct* instance)
{
	return instance->RefLink;
}

CSEXPORT void CSCONV Export_UStruct_Set_RefLink(UStruct* instance, UProperty* value)
{
	instance->RefLink = value;
}

CSEXPORT UProperty* CSCONV Export_UStruct_Get_DestructorLink(UStruct* instance)
{
	return instance->DestructorLink;
}

CSEXPORT void CSCONV Export_UStruct_Set_DestructorLink(UStruct* instance, UProperty* value)
{
	instance->DestructorLink = value;
}

CSEXPORT UProperty* CSCONV Export_UStruct_Get_PostConstructLink(UStruct* instance)
{
	return instance->PostConstructLink;
}

CSEXPORT void CSCONV Export_UStruct_Set_PostConstructLink(UStruct* instance, UProperty* value)
{
	instance->PropertyLink = value;
}

CSEXPORT TArray<UObject*>& CSCONV Export_UStruct_Get_ScriptObjectReferences(UStruct* instance)
{
	return instance->ScriptObjectReferences;
}

CSEXPORT void CSCONV Export_UStruct_Set_ScriptObjectReferences(UStruct* instance, TArray<UObject*>& value)
{
	instance->ScriptObjectReferences = value;
}

CSEXPORT void CSCONV Export_UStruct_AddReferencedObjects(UObject* InThis, FReferenceCollector& Collector)
{
	UStruct::AddReferencedObjects(InThis, Collector);
}

CSEXPORT UProperty* CSCONV Export_UStruct_FindPropertyByName(UStruct* instance, const FName& Name)
{
	return instance->FindPropertyByName(Name);
}

CSEXPORT void CSCONV Export_UStruct_InstanceSubobjectTemplates(UStruct* instance, void* Data, void const* DefaultData, UStruct* DefaultStruct, UObject* Owner, FObjectInstancingGraph* InstanceGraph)
{
	instance->InstanceSubobjectTemplates(Data, DefaultData, DefaultStruct, Owner, InstanceGraph);
}

CSEXPORT UStruct* CSCONV Export_UStruct_GetInheritanceSuper(UStruct* instance)
{
	return instance->GetInheritanceSuper();
}

CSEXPORT void CSCONV Export_UStruct_StaticLink(UStruct* instance, csbool bRelinkExistingProperties)
{
	instance->StaticLink(!!bRelinkExistingProperties);
}

CSEXPORT void CSCONV Export_UStruct_Link(UStruct* instance, FArchive& Ar, csbool bRelinkExistingProperties)
{
	instance->Link(Ar, !!bRelinkExistingProperties);
}

CSEXPORT void CSCONV Export_UStruct_SerializeBin(UStruct* instance, FArchive& Ar, void* Data)
{
	instance->SerializeBin(Ar, Data);
}

CSEXPORT void CSCONV Export_UStruct_SerializeBinEx(UStruct* instance, FArchive& Ar, void* Data, void const* DefaultData, UStruct* DefaultStruct)
{
	instance->SerializeBinEx(Ar, Data, DefaultData, DefaultStruct);
}

CSEXPORT void CSCONV Export_UStruct_SerializeTaggedProperties(UStruct* instance, FArchive& Ar, uint8* Data, UStruct* DefaultsStruct, uint8* Defaults, const UObject* BreakRecursionIfFullyLoad)
{
	instance->SerializeTaggedProperties(Ar, Data, DefaultsStruct, Defaults, BreakRecursionIfFullyLoad);
}

CSEXPORT void CSCONV Export_UStruct_InitializeStruct(UStruct* instance, void* Dest, int32 ArrayDim)
{
	instance->InitializeStruct(Dest, ArrayDim);
}

CSEXPORT void CSCONV Export_UStruct_DestroyStruct(UStruct* instance, void* Dest, int32 ArrayDim)
{
	instance->DestroyStruct(Dest, ArrayDim);
}

CSEXPORT EExprToken CSCONV Export_UStruct_SerializeExpr(UStruct* instance, int32& iCode, FArchive& Ar)
{
	return instance->SerializeExpr(iCode, Ar);
}

CSEXPORT void CSCONV Export_UStruct_TagSubobjects(UStruct* instance, EObjectFlags NewFlags)
{
	instance->TagSubobjects(NewFlags);
}

CSEXPORT void CSCONV Export_UStruct_GetPrefixCPP(UStruct* instance, FString& result)
{
	result = instance->GetPrefixCPP();
}

CSEXPORT int32 CSCONV Export_UStruct_GetPropertiesSize(UStruct* instance)
{
	return instance->GetPropertiesSize();
}

CSEXPORT int32 CSCONV Export_UStruct_GetMinAlignment(UStruct* instance)
{
	return instance->GetMinAlignment();
}

CSEXPORT int32 CSCONV Export_UStruct_GetStructureSize(UStruct* instance)
{
	return instance->GetStructureSize();
}

CSEXPORT void CSCONV Export_UStruct_SetPropertiesSize(UStruct* instance, int32 NewSize)
{
	return instance->SetPropertiesSize(NewSize);
}

CSEXPORT csbool CSCONV Export_UStruct_IsChildOf(UStruct* instance, const UStruct* SomeBase)
{
	return instance->IsChildOf(SomeBase);
}

CSEXPORT UStruct* CSCONV Export_UStruct_GetSuperStruct(UStruct* instance)
{
	return instance->GetSuperStruct();
}

CSEXPORT void CSCONV Export_UStruct_SetSuperStruct(UStruct* instance, UStruct* NewSuperStruct)
{
	instance->SetSuperStruct(NewSuperStruct);
}

#if WITH_EDITOR
CSEXPORT csbool CSCONV Export_UStruct_GetBoolMetaDataHierarchical(UStruct* instance, const FName& Key)
{
	return instance->GetBoolMetaDataHierarchical(Key);
}

CSEXPORT csbool CSCONV Export_UStruct_GetStringMetaDataHierarchical(UStruct* instance, const FName& Key, FString* OutValue)
{
	return instance->GetStringMetaDataHierarchical(Key, OutValue);
}
#endif

CSEXPORT void CSCONV Export_UStruct(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UStruct_Get_Children);
	REGISTER_FUNC(Export_UStruct_Set_Children);
	REGISTER_FUNC(Export_UStruct_Get_PropertiesSize);
	REGISTER_FUNC(Export_UStruct_Set_PropertiesSize);
	REGISTER_FUNC(Export_UStruct_Get_Script);
	REGISTER_FUNC(Export_UStruct_Set_Script);
	REGISTER_FUNC(Export_UStruct_Get_MinAlignment);
	REGISTER_FUNC(Export_UStruct_Set_MinAlignment);
	REGISTER_FUNC(Export_UStruct_Get_PropertyLink);
	REGISTER_FUNC(Export_UStruct_Set_PropertyLink);
	REGISTER_FUNC(Export_UStruct_Get_RefLink);
	REGISTER_FUNC(Export_UStruct_Set_RefLink);
	REGISTER_FUNC(Export_UStruct_Get_DestructorLink);
	REGISTER_FUNC(Export_UStruct_Set_DestructorLink);
	REGISTER_FUNC(Export_UStruct_Get_PostConstructLink);
	REGISTER_FUNC(Export_UStruct_Set_PostConstructLink);
	REGISTER_FUNC(Export_UStruct_Get_ScriptObjectReferences);
	REGISTER_FUNC(Export_UStruct_Set_ScriptObjectReferences);
	REGISTER_FUNC(Export_UStruct_AddReferencedObjects);
	REGISTER_FUNC(Export_UStruct_FindPropertyByName);
	REGISTER_FUNC(Export_UStruct_InstanceSubobjectTemplates);
	REGISTER_FUNC(Export_UStruct_GetInheritanceSuper);
	REGISTER_FUNC(Export_UStruct_StaticLink);
	REGISTER_FUNC(Export_UStruct_Link);
	REGISTER_FUNC(Export_UStruct_SerializeBin);
	REGISTER_FUNC(Export_UStruct_SerializeBinEx);
	REGISTER_FUNC(Export_UStruct_SerializeTaggedProperties);
	REGISTER_FUNC(Export_UStruct_InitializeStruct);
	REGISTER_FUNC(Export_UStruct_DestroyStruct);
	REGISTER_FUNC(Export_UStruct_SerializeExpr);
	REGISTER_FUNC(Export_UStruct_TagSubobjects);
	REGISTER_FUNC(Export_UStruct_GetPrefixCPP);
	REGISTER_FUNC(Export_UStruct_GetPropertiesSize);
	REGISTER_FUNC(Export_UStruct_GetMinAlignment);
	REGISTER_FUNC(Export_UStruct_GetStructureSize);
	REGISTER_FUNC(Export_UStruct_SetPropertiesSize);
	REGISTER_FUNC(Export_UStruct_IsChildOf);
	REGISTER_FUNC(Export_UStruct_GetSuperStruct);
	REGISTER_FUNC(Export_UStruct_SetSuperStruct);
#if WITH_EDITOR
	REGISTER_FUNC(Export_UStruct_GetBoolMetaDataHierarchical);
	REGISTER_FUNC(Export_UStruct_GetStringMetaDataHierarchical);
#endif
}
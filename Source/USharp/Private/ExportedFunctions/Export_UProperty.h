CSEXPORT int32 CSCONV Export_UProperty_Get_ArrayDim(UProperty* instance)
{
	return instance->ArrayDim;
}

CSEXPORT void CSCONV Export_UProperty_Set_ArrayDim(UProperty* instance, int32 value)
{
	instance->ArrayDim = value;
}

CSEXPORT int32 CSCONV Export_UProperty_Get_ElementSize(UProperty* instance)
{
	return instance->ElementSize;
}

CSEXPORT void CSCONV Export_UProperty_Set_ElementSize(UProperty* instance, int32 value)
{
	instance->ElementSize = value;
}

CSEXPORT EPropertyFlags CSCONV Export_UProperty_Get_PropertyFlags(UProperty* instance)
{
	return instance->PropertyFlags;
}

CSEXPORT void CSCONV Export_UProperty_Set_PropertyFlags(UProperty* instance, EPropertyFlags value)
{
	instance->PropertyFlags = value;
}

CSEXPORT uint16 CSCONV Export_UProperty_Get_RepIndex(UProperty* instance)
{
	return instance->RepIndex;
}

CSEXPORT void CSCONV Export_UProperty_Set_RepIndex(UProperty* instance, uint16 value)
{
	instance->RepIndex = value;
}

CSEXPORT void CSCONV Export_UProperty_Get_RepNotifyFunc(UProperty* instance, FName& result)
{
	result = instance->RepNotifyFunc;
}

CSEXPORT void CSCONV Export_UProperty_Set_RepNotifyFunc(UProperty* instance, const FName& value)
{
	instance->RepNotifyFunc = value;
}

CSEXPORT UProperty* CSCONV Export_UProperty_Get_PropertyLinkNext(UProperty* instance)
{
	return instance->PropertyLinkNext;
}

CSEXPORT void CSCONV Export_UProperty_Set_PropertyLinkNext(UProperty* instance, UProperty* value)
{
	instance->PropertyLinkNext = value;
}

CSEXPORT UProperty* CSCONV Export_UProperty_Get_NextRef(UProperty* instance)
{
	return instance->NextRef;
}

CSEXPORT void CSCONV Export_UProperty_Set_NextRef(UProperty* instance, UProperty* value)
{
	instance->NextRef = value;
}

CSEXPORT UProperty* CSCONV Export_UProperty_Get_DestructorLinkNext(UProperty* instance)
{
	return instance->DestructorLinkNext;
}

CSEXPORT void CSCONV Export_UProperty_Set_DestructorLinkNext(UProperty* instance, UProperty* value)
{
	instance->DestructorLinkNext = value;
}

CSEXPORT UProperty* CSCONV Export_UProperty_Get_PostConstructLinkNext(UProperty* instance)
{
	return instance->PostConstructLinkNext;
}

CSEXPORT void CSCONV Export_UProperty_Set_PostConstructLinkNext(UProperty* instance, UProperty* value)
{
	instance->PostConstructLinkNext = value;
}

CSEXPORT void CSCONV Export_UProperty_ImportSingleProperty(const FString& Str, void* DestData, class UStruct* ObjectStruct, UObject* SubobjectOuter, int32 PortFlags, FOutputDevice* Warn, TArray<struct FDefinedProperty>& DefinedProperties, FString& result)
{
	result = UProperty::ImportSingleProperty(*Str, DestData, ObjectStruct, SubobjectOuter, PortFlags, Warn, DefinedProperties);
}

CSEXPORT void CSCONV Export_UProperty_ExportCppDeclaration(UProperty* instance, FOutputDevice& Out, EExportedDeclaration::Type DeclarationType, const FString& ArrayDimOverride, uint32 AdditionalExportCPPFlags)
{
	instance->ExportCppDeclaration(Out, DeclarationType, *ArrayDimOverride, AdditionalExportCPPFlags);
}

CSEXPORT void CSCONV Export_UProperty_GetCPPMacroType(UProperty* instance, FString& ExtendedTypeText, FString& result)
{
	result = instance->GetCPPMacroType(ExtendedTypeText);
}

CSEXPORT csbool CSCONV Export_UProperty_PassCPPArgsByRef(UProperty* instance)
{
	return instance->PassCPPArgsByRef();
}

CSEXPORT void CSCONV Export_UProperty_GetNameCPP(UProperty* instance, FString& result)
{
	result = instance->GetNameCPP();
}

CSEXPORT void CSCONV Export_UProperty_GetCPPType(UProperty* instance, FString* ExtendedTypeText, uint32 CPPExportFlags, FString& result)
{
	result = instance->GetCPPType(ExtendedTypeText, CPPExportFlags);
}

CSEXPORT int32 CSCONV Export_UProperty_GetOffset_ForDebug(UProperty* instance)
{
	return instance->GetOffset_ForDebug();
}

CSEXPORT int32 CSCONV Export_UProperty_GetOffset_ForUFunction(UProperty* instance)
{
	return instance->GetOffset_ForUFunction();
}

CSEXPORT int32 CSCONV Export_UProperty_GetOffset_ForGC(UProperty* instance)
{
	return instance->GetOffset_ForGC();
}

CSEXPORT int32 CSCONV Export_UProperty_GetOffset_ForInternal(UProperty* instance)
{
	return instance->GetOffset_ForInternal();
}

CSEXPORT int32 CSCONV Export_UProperty_GetOffset_ReplaceWith_ContainerPtrToValuePtr(UProperty* instance)
{
	return instance->GetOffset_ReplaceWith_ContainerPtrToValuePtr();
}

CSEXPORT void CSCONV Export_UProperty_LinkWithoutChangingOffset(UProperty* instance, FArchive& Ar)
{
	instance->LinkWithoutChangingOffset(Ar);
}

CSEXPORT int32 CSCONV Export_UProperty_Link(UProperty* instance, FArchive& Ar)
{
	return instance->Link(Ar);
}

CSEXPORT csbool CSCONV Export_UProperty_Identical(UProperty* instance, const void* A, const void* B, uint32 PortFlags)
{
	return instance->Identical(A, B, PortFlags);
}

CSEXPORT csbool CSCONV Export_UProperty_Identical_InContainer(UProperty* instance, const void* A, const void* B, int32 ArrayIndex, uint32 PortFlags)
{
	return instance->Identical_InContainer(A, B, ArrayIndex, PortFlags);
}

CSEXPORT csbool CSCONV Export_UProperty_NetSerializeItem(UProperty* instance, FArchive& Ar, UPackageMap* Map, void* Data, TArray<uint8>* MetaData)
{
	return instance->NetSerializeItem(Ar, Map, Data, MetaData);
}

CSEXPORT void CSCONV Export_UProperty_ExportTextItem(UProperty* instance, FString& ValueStr, const void* PropertyValue, const void* DefaultValue, UObject* Parent, int32 PortFlags, UObject* ExportRootScope)
{
	instance->ExportTextItem(ValueStr, PropertyValue, DefaultValue, Parent, PortFlags, ExportRootScope);
}

CSEXPORT void CSCONV Export_UProperty_ImportText(UProperty* instance, const FString& Buffer, void* Data, int32 PortFlags, UObject* OwnerObject, FOutputDevice* ErrorText, FString& result)
{
	result = instance->ImportText(*Buffer, Data, PortFlags, OwnerObject, ErrorText);
}

CSEXPORT csbool CSCONV Export_UProperty_ExportText_Direct(UProperty* instance, FString& ValueStr, const void* Data, const void* Delta, UObject* Parent, int32 PortFlags, UObject* ExportRootScope)
{
	return instance->ExportText_Direct(ValueStr, Data, Delta, Parent, PortFlags, ExportRootScope);
}

CSEXPORT csbool CSCONV Export_UProperty_ExportText_InContainer(UProperty* instance, int32 Index, FString& ValueStr, const void* Data, const void* Delta, UObject* Parent, int32 PortFlags, UObject* ExportRootScope)
{
	return instance->ExportText_InContainer(Index, ValueStr, Data, Delta, Parent, PortFlags, ExportRootScope);
}

CSEXPORT void* CSCONV Export_UProperty_ContainerUObjectPtrToValuePtr(UProperty* instance, UObject* ContainerPtr, int32 ArrayIndex)
{
	return instance->ContainerPtrToValuePtr<void>(ContainerPtr, ArrayIndex);
}

CSEXPORT void* CSCONV Export_UProperty_ContainerVoidPtrToValuePtr(UProperty* instance, void* ContainerPtr, int32 ArrayIndex)
{
	return instance->ContainerPtrToValuePtr<void>(ContainerPtr, ArrayIndex);
}

CSEXPORT void* CSCONV Export_UProperty_ContainerUObjectPtrToValuePtrForDefaults(UProperty* instance, UStruct* ContainerClass, UObject* ContainerPtr, int32 ArrayIndex)
{
	return instance->ContainerPtrToValuePtrForDefaults<void>(ContainerClass, ContainerPtr, ArrayIndex);
}

CSEXPORT void* CSCONV Export_UProperty_ContainerVoidPtrToValuePtrForDefaults(UProperty* instance, UStruct* ContainerClass, void* ContainerPtr, int32 ArrayIndex)
{
	return instance->ContainerPtrToValuePtrForDefaults<void>(ContainerClass, ContainerPtr, ArrayIndex);
}

CSEXPORT csbool CSCONV Export_UProperty_IsInContainer(UProperty* instance, int32 ContainerSize)
{
	return instance->IsInContainer(ContainerSize);
}

CSEXPORT csbool CSCONV Export_UProperty_IsInContainerStruct(UProperty* instance, UStruct* ContainerClass)
{
	return instance->IsInContainer(ContainerClass);
}

CSEXPORT void CSCONV Export_UProperty_CopySingleValue(UProperty* instance, void* Dest, void const* Src)
{
	instance->CopySingleValue(Dest, Src);
}

CSEXPORT uint32 CSCONV Export_UProperty_GetValueTypeHash(UProperty* instance, const void* Src)
{
	return instance->GetValueTypeHash(Src);
}

CSEXPORT void CSCONV Export_UProperty_CopyCompleteValue(UProperty* instance, void* Dest, void const* Src)
{
	instance->CopyCompleteValue(Dest, Src);
}

CSEXPORT void CSCONV Export_UProperty_CopyCompleteValue_InContainer(UProperty* instance, void* Dest, void const* Src)
{
	instance->CopyCompleteValue_InContainer(Dest, Src);
}

CSEXPORT void CSCONV Export_UProperty_CopySingleValueToScriptVM(UProperty* instance, void* Dest, void const* Src)
{
	instance->CopySingleValueToScriptVM(Dest, Src);
}

CSEXPORT void CSCONV Export_UProperty_CopyCompleteValueToScriptVM(UProperty* instance, void* Dest, void const* Src)
{
	instance->CopyCompleteValueToScriptVM(Dest, Src);
}

CSEXPORT void CSCONV Export_UProperty_CopySingleValueFromScriptVM(UProperty* instance, void* Dest, void const* Src)
{
	instance->CopySingleValueFromScriptVM(Dest, Src);
}

CSEXPORT void CSCONV Export_UProperty_CopyCompleteValueFromScriptVM(UProperty* instance, void* Dest, void const* Src)
{
	instance->CopyCompleteValueFromScriptVM(Dest, Src);
}

CSEXPORT void CSCONV Export_UProperty_ClearValue(UProperty* instance, void* Data)
{
	instance->ClearValue(Data);
}

CSEXPORT void CSCONV Export_UProperty_ClearValue_InContainer(UProperty* instance, void* Data, int32 ArrayIndex)
{
	instance->ClearValue_InContainer(Data, ArrayIndex);
}

CSEXPORT void CSCONV Export_UProperty_DestroyValue(UProperty* instance, void* Dest)
{
	instance->DestroyValue(Dest);
}

CSEXPORT void CSCONV Export_UProperty_DestroyValue_InContainer(UProperty* instance, void* Dest)
{
	instance->DestroyValue_InContainer(Dest);
}

CSEXPORT void CSCONV Export_UProperty_InitializeValue(UProperty* instance, void* Dest)
{
	instance->InitializeValue(Dest);
}

CSEXPORT void CSCONV Export_UProperty_InitializeValue_InContainer(UProperty* instance, void* Dest)
{
	instance->InitializeValue_InContainer(Dest);
}

CSEXPORT csbool CSCONV Export_UProperty_ValidateImportFlags(UProperty* instance, uint32 PortFlags, FOutputDevice* ErrorText)
{
	return instance->ValidateImportFlags(PortFlags, ErrorText);
}

CSEXPORT csbool CSCONV Export_UProperty_ShouldPort(UProperty* instance, uint32 PortFlags)
{
	return instance->ShouldPort(PortFlags);
}

CSEXPORT void CSCONV Export_UProperty_GetID(UProperty* instance, FName& result)
{
	result = instance->GetID();
}

CSEXPORT void CSCONV Export_UProperty_InstanceSubobjects(UProperty* instance, void* Data, void const* DefaultData, UObject* Owner, struct FObjectInstancingGraph* InstanceGraph)
{
	instance->InstanceSubobjects(Data, DefaultData, Owner, InstanceGraph);
}

CSEXPORT int32 CSCONV Export_UProperty_GetMinAlignment(UProperty* instance)
{
	return instance->GetMinAlignment();
}

CSEXPORT csbool CSCONV Export_UProperty_ContainsObjectReference(UProperty* instance, TArray<const UStructProperty*>& EncounteredStructProps)
{
	return instance->ContainsObjectReference(EncounteredStructProps);
}

CSEXPORT csbool CSCONV Export_UProperty_ContainsWeakObjectReference(UProperty* instance)
{
	return instance->ContainsWeakObjectReference();
}

CSEXPORT csbool CSCONV Export_UProperty_ContainsInstancedObjectProperty(UProperty* instance)
{
	return instance->ContainsInstancedObjectProperty();
}

CSEXPORT void CSCONV Export_UProperty_EmitReferenceInfo(UProperty* instance, UClass& OwnerClass, int32 BaseOffset, TArray<const UStructProperty*>& EncounteredStructProps)
{
	instance->EmitReferenceInfo(OwnerClass, BaseOffset, EncounteredStructProps);
}

CSEXPORT int32 CSCONV Export_UProperty_GetSize(UProperty* instance)
{
	return instance->GetSize();
}

CSEXPORT csbool CSCONV Export_UProperty_ShouldSerializeValue(UProperty* instance, FArchive& Ar)
{
	return instance->ShouldSerializeValue(Ar);
}

CSEXPORT csbool CSCONV Export_UProperty_ShouldDuplicateValue(UProperty* instance)
{
	return instance->ShouldDuplicateValue();
}

CSEXPORT UProperty* CSCONV Export_UProperty_GetOwnerProperty(UProperty* instance)
{
	return instance->GetOwnerProperty();
}

CSEXPORT EPropertyFlags CSCONV Export_UProperty_GetPropertyFlags(UProperty* instance)
{
	return instance->GetPropertyFlags();
}

CSEXPORT void CSCONV Export_UProperty_SetPropertyFlags(UProperty* instance, EPropertyFlags NewFlags)
{
	instance->SetPropertyFlags(NewFlags);
}

CSEXPORT void CSCONV Export_UProperty_ClearPropertyFlags(UProperty* instance, EPropertyFlags NewFlags)
{
	instance->ClearPropertyFlags(NewFlags);
}

CSEXPORT csbool CSCONV Export_UProperty_HasAnyPropertyFlags(UProperty* instance, uint64 FlagsToCheck)
{
	return instance->HasAnyPropertyFlags(FlagsToCheck);
}

CSEXPORT csbool CSCONV Export_UProperty_HasAllPropertyFlags(UProperty* instance, uint64 FlagsToCheck)
{
	return instance->HasAllPropertyFlags(FlagsToCheck);
}

CSEXPORT UProperty* CSCONV Export_UProperty_GetRepOwner(UProperty* instance)
{
	return instance->GetRepOwner();
}

CSEXPORT csbool CSCONV Export_UProperty_IsEditorOnlyProperty(UProperty* instance)
{
	return instance->IsEditorOnlyProperty();
}

CSEXPORT csbool CSCONV Export_UProperty_SameType(UProperty* instance, const UProperty* Other)
{
	return instance->SameType(Other);
}

CSEXPORT void CSCONV Export_UProperty(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UProperty_Get_ArrayDim);
	REGISTER_FUNC(Export_UProperty_Set_ArrayDim);
	REGISTER_FUNC(Export_UProperty_Get_ElementSize);
	REGISTER_FUNC(Export_UProperty_Set_ElementSize);
	REGISTER_FUNC(Export_UProperty_Get_PropertyFlags);
	REGISTER_FUNC(Export_UProperty_Set_PropertyFlags);
	REGISTER_FUNC(Export_UProperty_Get_RepIndex);
	REGISTER_FUNC(Export_UProperty_Set_RepIndex);
	REGISTER_FUNC(Export_UProperty_Get_RepNotifyFunc);
	REGISTER_FUNC(Export_UProperty_Set_RepNotifyFunc);
	REGISTER_FUNC(Export_UProperty_Get_PropertyLinkNext);
	REGISTER_FUNC(Export_UProperty_Set_PropertyLinkNext);
	REGISTER_FUNC(Export_UProperty_Get_NextRef);
	REGISTER_FUNC(Export_UProperty_Set_NextRef);
	REGISTER_FUNC(Export_UProperty_Get_DestructorLinkNext);
	REGISTER_FUNC(Export_UProperty_Set_DestructorLinkNext);
	REGISTER_FUNC(Export_UProperty_Get_PostConstructLinkNext);
	REGISTER_FUNC(Export_UProperty_Set_PostConstructLinkNext);
	REGISTER_FUNC(Export_UProperty_ImportSingleProperty);
	REGISTER_FUNC(Export_UProperty_ExportCppDeclaration);
	REGISTER_FUNC(Export_UProperty_GetCPPMacroType);
	REGISTER_FUNC(Export_UProperty_PassCPPArgsByRef);
	REGISTER_FUNC(Export_UProperty_GetNameCPP);
	REGISTER_FUNC(Export_UProperty_GetCPPType);
	REGISTER_FUNC(Export_UProperty_GetOffset_ForDebug);
	REGISTER_FUNC(Export_UProperty_GetOffset_ForUFunction);
	REGISTER_FUNC(Export_UProperty_GetOffset_ForGC);
	REGISTER_FUNC(Export_UProperty_GetOffset_ForInternal);
	REGISTER_FUNC(Export_UProperty_GetOffset_ReplaceWith_ContainerPtrToValuePtr);
	REGISTER_FUNC(Export_UProperty_LinkWithoutChangingOffset);
	REGISTER_FUNC(Export_UProperty_Link);
	REGISTER_FUNC(Export_UProperty_Identical);
	REGISTER_FUNC(Export_UProperty_Identical_InContainer);
	REGISTER_FUNC(Export_UProperty_NetSerializeItem);
	REGISTER_FUNC(Export_UProperty_ExportTextItem);
	REGISTER_FUNC(Export_UProperty_ImportText);
	REGISTER_FUNC(Export_UProperty_ExportText_Direct);
	REGISTER_FUNC(Export_UProperty_ExportText_InContainer);
	REGISTER_FUNC(Export_UProperty_ContainerUObjectPtrToValuePtr);
	REGISTER_FUNC(Export_UProperty_ContainerVoidPtrToValuePtr);
	REGISTER_FUNC(Export_UProperty_ContainerUObjectPtrToValuePtrForDefaults);
	REGISTER_FUNC(Export_UProperty_ContainerVoidPtrToValuePtrForDefaults);
	REGISTER_FUNC(Export_UProperty_IsInContainer);
	REGISTER_FUNC(Export_UProperty_IsInContainerStruct);
	REGISTER_FUNC(Export_UProperty_CopySingleValue);
	REGISTER_FUNC(Export_UProperty_GetValueTypeHash);
	REGISTER_FUNC(Export_UProperty_CopyCompleteValue);
	REGISTER_FUNC(Export_UProperty_CopyCompleteValue_InContainer);
	REGISTER_FUNC(Export_UProperty_CopySingleValueToScriptVM);
	REGISTER_FUNC(Export_UProperty_CopyCompleteValueToScriptVM);
	REGISTER_FUNC(Export_UProperty_CopySingleValueFromScriptVM);
	REGISTER_FUNC(Export_UProperty_CopyCompleteValueFromScriptVM);
	REGISTER_FUNC(Export_UProperty_ClearValue);
	REGISTER_FUNC(Export_UProperty_ClearValue_InContainer);
	REGISTER_FUNC(Export_UProperty_DestroyValue);
	REGISTER_FUNC(Export_UProperty_DestroyValue_InContainer);
	REGISTER_FUNC(Export_UProperty_InitializeValue);
	REGISTER_FUNC(Export_UProperty_InitializeValue_InContainer);
	REGISTER_FUNC(Export_UProperty_ValidateImportFlags);
	REGISTER_FUNC(Export_UProperty_ShouldPort);
	REGISTER_FUNC(Export_UProperty_GetID);
	REGISTER_FUNC(Export_UProperty_InstanceSubobjects);
	REGISTER_FUNC(Export_UProperty_GetMinAlignment);
	REGISTER_FUNC(Export_UProperty_ContainsObjectReference);
	REGISTER_FUNC(Export_UProperty_ContainsWeakObjectReference);
	REGISTER_FUNC(Export_UProperty_ContainsInstancedObjectProperty);
	REGISTER_FUNC(Export_UProperty_EmitReferenceInfo);
	REGISTER_FUNC(Export_UProperty_GetSize);
	REGISTER_FUNC(Export_UProperty_ShouldSerializeValue);
	REGISTER_FUNC(Export_UProperty_ShouldDuplicateValue);
	REGISTER_FUNC(Export_UProperty_GetOwnerProperty);
	REGISTER_FUNC(Export_UProperty_GetPropertyFlags);
	REGISTER_FUNC(Export_UProperty_SetPropertyFlags);
	REGISTER_FUNC(Export_UProperty_ClearPropertyFlags);
	REGISTER_FUNC(Export_UProperty_HasAnyPropertyFlags);
	REGISTER_FUNC(Export_UProperty_HasAllPropertyFlags);
	REGISTER_FUNC(Export_UProperty_GetRepOwner);
	REGISTER_FUNC(Export_UProperty_IsEditorOnlyProperty);
	REGISTER_FUNC(Export_UProperty_SameType);
}
CSEXPORT EStructFlags CSCONV Export_UScriptStruct_Get_StructFlags(UScriptStruct* instance)
{
	return instance->StructFlags;
}

CSEXPORT void CSCONV Export_UScriptStruct_Set_StructFlags(UScriptStruct* instance, EStructFlags value)
{
	instance->StructFlags = value;
}

CSEXPORT void CSCONV Export_UScriptStruct_DeferCppStructOps(const FName& Target, UScriptStruct::ICppStructOps* InCppStructOps)
{
	UScriptStruct::DeferCppStructOps(Target, InCppStructOps);
}

CSEXPORT void CSCONV Export_UScriptStruct_PrepareCppStructOps(UScriptStruct* instance)
{
	instance->PrepareCppStructOps();
}

CSEXPORT UScriptStruct::ICppStructOps* CSCONV Export_UScriptStruct_GetCppStructOps(UScriptStruct* instance)
{
	return instance->GetCppStructOps();
}

CSEXPORT void CSCONV Export_UScriptStruct_ClearCppStructOps(UScriptStruct* instance)
{
	instance->ClearCppStructOps();
}

CSEXPORT csbool CSCONV Export_UScriptStruct_HasDefaults(UScriptStruct* instance)
{
	return instance->HasDefaults();
}

CSEXPORT csbool CSCONV Export_UScriptStruct_ShouldSerializeAtomically(UScriptStruct* instance, FArchive& Ar)
{
	return instance->ShouldSerializeAtomically(Ar);
}

CSEXPORT csbool CSCONV Export_UScriptStruct_CompareScriptStruct(UScriptStruct* instance, const void* A, const void* B, uint32 PortFlags)
{
	return instance->CompareScriptStruct(A, B, PortFlags);
}

CSEXPORT void CSCONV Export_UScriptStruct_CopyScriptStruct(UScriptStruct* instance, void* Dest, void const* Src, int32 ArrayDim)
{
	instance->CopyScriptStruct(Dest, Src, ArrayDim);
}

CSEXPORT void CSCONV Export_UScriptStruct_ClearScriptStruct(UScriptStruct* instance, void* Dest, int32 ArrayDim)
{
	instance->ClearScriptStruct(Dest, ArrayDim);
}

CSEXPORT void CSCONV Export_UScriptStruct_RecursivelyPreload(UScriptStruct* instance)
{
	instance->RecursivelyPreload();
}

CSEXPORT void CSCONV Export_UScriptStruct_GetCustomGuid(UScriptStruct* instance, FGuid& result)
{
	result = instance->GetCustomGuid();
}

#if WITH_EDITOR
CSEXPORT void CSCONV Export_UScriptStruct_InitializeDefaultValue(UScriptStruct* instance, uint8* InStructData)
{
	instance->InitializeDefaultValue(InStructData);
}
#endif

CSEXPORT void CSCONV Export_UScriptStruct(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UScriptStruct_Get_StructFlags);
	REGISTER_FUNC(Export_UScriptStruct_Set_StructFlags);
	REGISTER_FUNC(Export_UScriptStruct_DeferCppStructOps);
	REGISTER_FUNC(Export_UScriptStruct_PrepareCppStructOps);
	REGISTER_FUNC(Export_UScriptStruct_GetCppStructOps);
	REGISTER_FUNC(Export_UScriptStruct_ClearCppStructOps);
	REGISTER_FUNC(Export_UScriptStruct_HasDefaults);
	REGISTER_FUNC(Export_UScriptStruct_ShouldSerializeAtomically);
	REGISTER_FUNC(Export_UScriptStruct_CompareScriptStruct);
	REGISTER_FUNC(Export_UScriptStruct_CopyScriptStruct);
	REGISTER_FUNC(Export_UScriptStruct_ClearScriptStruct);
	REGISTER_FUNC(Export_UScriptStruct_RecursivelyPreload);
	REGISTER_FUNC(Export_UScriptStruct_GetCustomGuid);
#if WITH_EDITOR
	REGISTER_FUNC(Export_UScriptStruct_InitializeDefaultValue);
#endif
}
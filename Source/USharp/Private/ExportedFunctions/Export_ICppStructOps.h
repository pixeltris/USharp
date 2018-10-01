CSEXPORT csbool CSCONV Export_ICppStructOps_HasNoopConstructor(UScriptStruct::ICppStructOps* instance)
{
	return instance->HasNoopConstructor();
}

CSEXPORT csbool CSCONV Export_ICppStructOps_HasZeroConstructor(UScriptStruct::ICppStructOps* instance)
{
	return instance->HasZeroConstructor();
}

CSEXPORT void CSCONV Export_ICppStructOps_Construct(UScriptStruct::ICppStructOps* instance, void* Dest)
{
	instance->Construct(Dest);
}

CSEXPORT csbool CSCONV Export_ICppStructOps_HasDestructor(UScriptStruct::ICppStructOps* instance)
{
	return instance->HasDestructor();
}

CSEXPORT void CSCONV Export_ICppStructOps_Destruct(UScriptStruct::ICppStructOps* instance, void* Dest)
{
	instance->Destruct(Dest);
}

CSEXPORT int32 CSCONV Export_ICppStructOps_GetSize(UScriptStruct::ICppStructOps* instance)
{
	return instance->GetSize();
}

CSEXPORT int32 CSCONV Export_ICppStructOps_GetAlignment(UScriptStruct::ICppStructOps* instance)
{
	return instance->GetAlignment();
}

CSEXPORT csbool CSCONV Export_ICppStructOps_IsPlainOldData(UScriptStruct::ICppStructOps* instance)
{
	return instance->IsPlainOldData();
}

CSEXPORT csbool CSCONV Export_ICppStructOps_HasCopy(UScriptStruct::ICppStructOps* instance)
{
	return instance->HasCopy();
}

CSEXPORT csbool CSCONV Export_ICppStructOps_Copy(UScriptStruct::ICppStructOps* instance, void* Dest, void* Src, int32 ArrayDim)
{
	return instance->Copy(Dest, Src, ArrayDim);
}

CSEXPORT csbool CSCONV Export_ICppStructOps_HasIdentical(UScriptStruct::ICppStructOps* instance)
{
	return instance->HasIdentical();
}

CSEXPORT csbool CSCONV Export_ICppStructOps_Identical(UScriptStruct::ICppStructOps* instance, void* A, void* B, uint32 PortFlags, csbool& bOutResult)
{
	bool bOutResultBool;
	csbool Result = instance->Identical(A, B, PortFlags, bOutResultBool);
	bOutResult = bOutResultBool;
	return Result;
}

CSEXPORT csbool CSCONV Export_ICppStructOps_HasGetTypeHash(UScriptStruct::ICppStructOps* instance)
{
	return instance->HasGetTypeHash();
}

CSEXPORT uint32 CSCONV Export_ICppStructOps_GetTypeHash(UScriptStruct::ICppStructOps* instance, const void* Src)
{
	return instance->GetTypeHash(Src);
}

CSEXPORT uint64 CSCONV Export_ICppStructOps_GetComputedPropertyFlags(UScriptStruct::ICppStructOps* instance)
{
	return instance->GetComputedPropertyFlags();	
}

CSEXPORT csbool CSCONV Export_ICppStructOps_IsAbstract(UScriptStruct::ICppStructOps* instance)
{
	return instance->IsAbstract();
}

CSEXPORT void CSCONV Export_ICppStructOps(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_ICppStructOps_HasNoopConstructor);
	REGISTER_FUNC(Export_ICppStructOps_HasZeroConstructor);
	REGISTER_FUNC(Export_ICppStructOps_Construct);
	REGISTER_FUNC(Export_ICppStructOps_HasDestructor);
	REGISTER_FUNC(Export_ICppStructOps_Destruct);
	REGISTER_FUNC(Export_ICppStructOps_GetSize);
	REGISTER_FUNC(Export_ICppStructOps_GetAlignment);
	REGISTER_FUNC(Export_ICppStructOps_IsPlainOldData);
	REGISTER_FUNC(Export_ICppStructOps_HasCopy);
	REGISTER_FUNC(Export_ICppStructOps_Copy);
	REGISTER_FUNC(Export_ICppStructOps_HasIdentical);
	REGISTER_FUNC(Export_ICppStructOps_Identical);
	REGISTER_FUNC(Export_ICppStructOps_HasGetTypeHash);
	REGISTER_FUNC(Export_ICppStructOps_GetTypeHash);
	REGISTER_FUNC(Export_ICppStructOps_GetComputedPropertyFlags);
	REGISTER_FUNC(Export_ICppStructOps_IsAbstract);
}
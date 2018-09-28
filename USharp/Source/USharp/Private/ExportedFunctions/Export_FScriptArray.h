CSEXPORT void* CSCONV Export_FScriptArray_GetData(FScriptArray* instance)
{
	return instance->GetData();
}

CSEXPORT csbool CSCONV Export_FScriptArray_IsValidIndex(FScriptArray* instance, int32 i)
{
	return instance->IsValidIndex(i);
}

CSEXPORT int32 CSCONV Export_FScriptArray_Num(FScriptArray* instance)
{
	return instance->Num();
}

CSEXPORT void CSCONV Export_FScriptArray_InsertZeroed(FScriptArray* instance, int32 Index, int32 Count, int32 NumBytesPerElement)
{
	instance->InsertZeroed(Index, Count, NumBytesPerElement);
}

CSEXPORT void CSCONV Export_FScriptArray_Insert(FScriptArray* instance, int32 Index, int32 Count, int32 NumBytesPerElement)
{
	instance->Insert(Index, Count, NumBytesPerElement);
}

CSEXPORT int32 CSCONV Export_FScriptArray_Add(FScriptArray* instance, int32 Count, int32 NumBytesPerElement)
{
	return instance->Add(Count, NumBytesPerElement);
}

CSEXPORT int32 CSCONV Export_FScriptArray_AddZeroed(FScriptArray* instance, int32 Count, int32 NumBytesPerElement)
{
	return instance->AddZeroed(Count, NumBytesPerElement);
}

CSEXPORT void CSCONV Export_FScriptArray_Shrink(FScriptArray* instance, int32 NumBytesPerElement)
{
	instance->Shrink(NumBytesPerElement);
}

CSEXPORT void CSCONV Export_FScriptArray_Empty(FScriptArray* instance, int32 Slack, int32 NumBytesPerElement)
{
	instance->Empty(Slack, NumBytesPerElement);
}

CSEXPORT void CSCONV Export_FScriptArray_SwapMemory(FScriptArray* instance, int32 A, int32 B, int32 NumBytesPerElement)
{
	instance->SwapMemory(A, B, NumBytesPerElement);
}

CSEXPORT void CSCONV Export_FScriptArray_CountBytes(FScriptArray* instance, FArchive& Ar, int32 NumBytesPerElement)
{
	instance->CountBytes(Ar, NumBytesPerElement);
}

CSEXPORT int32 CSCONV Export_FScriptArray_GetSlack(FScriptArray* instance)
{
	return instance->GetSlack();
}

CSEXPORT void CSCONV Export_FScriptArray_Remove(FScriptArray* instance, int32 Index, int32 Count, int32 NumBytesPerElement)
{
	instance->Remove(Index, Count, NumBytesPerElement);
}

CSEXPORT void CSCONV Export_FScriptArray_Destroy(FScriptArray* instance)
{
	instance->~FScriptArray();
}

CSEXPORT void CSCONV Export_FScriptArray(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FScriptArray_GetData);
	REGISTER_FUNC(Export_FScriptArray_IsValidIndex);
	REGISTER_FUNC(Export_FScriptArray_Num);
	REGISTER_FUNC(Export_FScriptArray_InsertZeroed);
	REGISTER_FUNC(Export_FScriptArray_Insert);
	REGISTER_FUNC(Export_FScriptArray_Add);
	REGISTER_FUNC(Export_FScriptArray_AddZeroed);
	REGISTER_FUNC(Export_FScriptArray_Shrink);
	REGISTER_FUNC(Export_FScriptArray_Empty);
	REGISTER_FUNC(Export_FScriptArray_SwapMemory);
	REGISTER_FUNC(Export_FScriptArray_CountBytes);
	REGISTER_FUNC(Export_FScriptArray_GetSlack);
	REGISTER_FUNC(Export_FScriptArray_Remove);
	REGISTER_FUNC(Export_FScriptArray_Destroy);
}
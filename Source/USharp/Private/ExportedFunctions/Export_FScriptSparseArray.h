CSEXPORT csbool CSCONV Export_FScriptSparseArray_IsValidIndex(FScriptSparseArray* instance, int32 Index)
{
	return instance->IsValidIndex(Index);
}

CSEXPORT int32 CSCONV Export_FScriptSparseArray_Num(FScriptSparseArray* instance)
{
	return instance->Num();
}

CSEXPORT int32 CSCONV Export_FScriptSparseArray_GetMaxIndex(FScriptSparseArray* instance)
{
	return instance->GetMaxIndex();
}

CSEXPORT void* CSCONV Export_FScriptSparseArray_GetData(FScriptSparseArray* instance, int32 Index, const FScriptSparseArrayLayout& Layout)
{
	return instance->GetData(Index, Layout);
}

CSEXPORT void CSCONV Export_FScriptSparseArray_Empty(FScriptSparseArray* instance, int32 Slack, const FScriptSparseArrayLayout& Layout)
{
	instance->Empty(Slack, Layout);
}

CSEXPORT int32 CSCONV Export_FScriptSparseArray_AddUninitialized(FScriptSparseArray* instance, const FScriptSparseArrayLayout& Layout)
{
	return instance->AddUninitialized(Layout);
}

CSEXPORT void CSCONV Export_FScriptSparseArray_RemoveAtUninitialized(FScriptSparseArray* instance, const FScriptSparseArrayLayout& Layout, int32 Index, int32 Count)
{
	instance->RemoveAtUninitialized(Layout, Index, Count);
}

CSEXPORT void CSCONV Export_FScriptSparseArray_Destroy(FScriptSparseArray* instance)
{
	instance->~FScriptSparseArray();
}

CSEXPORT FScriptSparseArrayLayout CSCONV Export_FScriptSparseArray_GetScriptLayout(int32 ElementSize, int32 ElementAlignment)
{
	return FScriptSparseArray::GetScriptLayout(ElementSize, ElementAlignment);
}

CSEXPORT void CSCONV Export_FScriptSparseArray(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FScriptSparseArray_IsValidIndex);
	REGISTER_FUNC(Export_FScriptSparseArray_Num);
	REGISTER_FUNC(Export_FScriptSparseArray_GetMaxIndex);
	REGISTER_FUNC(Export_FScriptSparseArray_GetData);
	REGISTER_FUNC(Export_FScriptSparseArray_Empty);
	REGISTER_FUNC(Export_FScriptSparseArray_AddUninitialized);
	REGISTER_FUNC(Export_FScriptSparseArray_RemoveAtUninitialized);
	REGISTER_FUNC(Export_FScriptSparseArray_Destroy);
	REGISTER_FUNC(Export_FScriptSparseArray_GetScriptLayout);
}
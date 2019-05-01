CSEXPORT csbool CSCONV Export_FScriptSet_IsValidIndex(FScriptSet* instance, int32 Index)
{
	return instance->IsValidIndex(Index);
}

CSEXPORT int32 CSCONV Export_FScriptSet_Num(FScriptSet* instance)
{
	return instance->Num();
}

CSEXPORT int32 CSCONV Export_FScriptSet_GetMaxIndex(FScriptSet* instance)
{
	return instance->GetMaxIndex();
}

CSEXPORT void* CSCONV Export_FScriptSet_GetData(FScriptSet* instance, int32 Index, const FScriptSetLayout& Layout)
{
	return instance->GetData(Index, Layout);
}

CSEXPORT void CSCONV Export_FScriptSet_Empty(FScriptSet* instance, int32 Slack, const FScriptSetLayout& Layout)
{
	instance->Empty(Slack, Layout);
}

CSEXPORT void CSCONV Export_FScriptSet_RemoveAt(FScriptSet* instance, int32 Index, const FScriptSetLayout& Layout)
{
	instance->RemoveAt(Index, Layout);
}

CSEXPORT int32 CSCONV Export_FScriptSet_AddUninitialized(FScriptSet* instance, const FScriptSetLayout& Layout)
{
	return instance->AddUninitialized(Layout);
}

CSEXPORT void CSCONV Export_FScriptSet_Rehash(FScriptSet* instance, const FScriptSetLayout& Layout, uint32(CSCONV *GetKeyHash)(const void*))
{
	instance->Rehash(
		Layout,
		[GetKeyHash](const void* InElement)
		{
			return GetKeyHash(InElement);
		});
}

CSEXPORT int32 CSCONV Export_FScriptSet_FindIndex(FScriptSet* instance, const void* Element, const FScriptSetLayout& Layout, uint32(CSCONV *GetKeyHash)(const void*), csbool(CSCONV *EqualityFn)(const void*, const void*))
{
	// Create a wrapper func due to bool return result conversion
	return instance->FindIndex(
		Element,
		Layout,
		[GetKeyHash](const void* InElement)
		{
			return GetKeyHash(InElement);
		},
		[EqualityFn](const void* A, const void* B)
		{
			return !!EqualityFn(A, B);
		});
}

CSEXPORT void CSCONV Export_FScriptSet_Add(FScriptSet* instance, const void* Element, const FScriptSetLayout& Layout, uint32(CSCONV *GetKeyHash)(const void*), csbool(CSCONV *EqualityFn)(const void*, const void*), void(CSCONV *ConstructFn)(void*), void(CSCONV *DestructFn)(void*))
{
	// Create a wrapper func due to bool return result conversion
	instance->Add(
		Element,
		Layout,
		[GetKeyHash](const void* InElement)
		{
			return GetKeyHash(InElement);
		},
		[EqualityFn](const void* A, const void* B)
		{
			return !!EqualityFn(A, B);
		},
		[ConstructFn](void* InElement)
		{
			ConstructFn(InElement);
		},
		[DestructFn](void* InElement)
		{
			DestructFn(InElement);
		});
}

CSEXPORT void CSCONV Export_FScriptSet_Destroy(FScriptSet* instance)
{
	instance->~FScriptSet();
}

CSEXPORT FScriptSetLayout CSCONV Export_FScriptSet_GetScriptLayout(int32 ElementSize, int32 ElementAlignment)
{
	return FScriptSet::GetScriptLayout(ElementSize, ElementAlignment);
}

CSEXPORT void CSCONV Export_FScriptSet(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FScriptSet_IsValidIndex);
	REGISTER_FUNC(Export_FScriptSet_Num);
	REGISTER_FUNC(Export_FScriptSet_GetMaxIndex);
	REGISTER_FUNC(Export_FScriptSet_GetData);
	REGISTER_FUNC(Export_FScriptSet_Empty);
	REGISTER_FUNC(Export_FScriptSet_RemoveAt);
	REGISTER_FUNC(Export_FScriptSet_AddUninitialized);
	REGISTER_FUNC(Export_FScriptSet_Rehash);
	REGISTER_FUNC(Export_FScriptSet_FindIndex);
	REGISTER_FUNC(Export_FScriptSet_Add);
	REGISTER_FUNC(Export_FScriptSet_Destroy);
	REGISTER_FUNC(Export_FScriptSet_GetScriptLayout);
}
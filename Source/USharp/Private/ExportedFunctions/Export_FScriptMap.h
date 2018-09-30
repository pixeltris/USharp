CSEXPORT int32 CSCONV Export_FScriptMap_FindPairIndex(FScriptMap* instance, const void* Key, const FScriptMapLayout& MapLayout, uint32(*GetKeyHash)(const void*), csbool(*KeyEqualityFn)(const void*, const void*))
{
	// Create a wrapper func due to bool return result conversion
	return instance->FindPairIndex(
		Key,
		MapLayout,
		GetKeyHash,
		[KeyEqualityFn](const void* InKey, const void* InPair)
		{
			return !!KeyEqualityFn(InKey, InPair);
		});
}

CSEXPORT uint8* CSCONV Export_FScriptMap_FindValue(FScriptMap* instance, const void* Key, const FScriptMapLayout& MapLayout, uint32(*GetKeyHash)(const void*), csbool(*KeyEqualityFn)(const void*, const void*))
{
	// Create a wrapper func due to bool return result conversion
	return instance->FindValue(
		Key,
		MapLayout,
		GetKeyHash,
		[KeyEqualityFn](const void* InKey, const void* InPair)
		{
			return !!KeyEqualityFn(InKey, InPair);
		});
}

CSEXPORT void CSCONV Export_FScriptMap_Add(FScriptMap* instance, const void* Key, const void* Value, const FScriptMapLayout& MapLayout, uint32(*GetKeyHash)(const void*), csbool(*KeyEqualityFn)(const void*, const void*), void(*KeyConstructAndAssignFn)(void*), void(*ValueConstructAndAssignFn)(void*), void(*ValueAssignFn)(void*), void(*DestructKeyFn)(void*), void(*DestructValueFn)(void*))
{
	// Create a wrapper func due to bool return result conversion
	instance->Add(
		Key,
		Value,
		MapLayout,
		GetKeyHash,
		[KeyEqualityFn](const void* InKey, const void* InPair)
		{
			return !!KeyEqualityFn(InKey, InPair);
		},
		KeyConstructAndAssignFn,
		ValueConstructAndAssignFn,
		ValueAssignFn,
		DestructKeyFn,
		DestructValueFn);
}

CSEXPORT void CSCONV Export_FScriptMap_Destroy(FScriptMap* instance)
{
	instance->~FScriptMap();
}

CSEXPORT FScriptMapLayout CSCONV Export_FScriptMap_GetScriptLayout(int32 KeySize, int32 KeyAlignment, int32 ValueSize, int32 ValueAlignment)
{
	return FScriptMap::GetScriptLayout(KeySize, KeyAlignment, ValueSize, ValueAlignment);
}

CSEXPORT void CSCONV Export_FScriptMap(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FScriptMap_Destroy);
	REGISTER_FUNC(Export_FScriptMap_GetScriptLayout);
	REGISTER_FUNC(Export_FScriptMap_FindPairIndex);
	REGISTER_FUNC(Export_FScriptMap_FindValue);
	REGISTER_FUNC(Export_FScriptMap_Add);
}
CSEXPORT int32 CSCONV Export_FScriptMap_FindPairIndex(FScriptMap* instance, const void* Key, const FScriptMapLayout& MapLayout, uint32(CSCONV *GetKeyHash)(const void*), csbool(CSCONV *KeyEqualityFn)(const void*, const void*))
{
	// Create a wrapper func due to bool return result conversion
	return instance->FindPairIndex(
		Key,
		MapLayout,
		[GetKeyHash](const void* InKey)
		{
			return GetKeyHash(InKey);
		},
		[KeyEqualityFn](const void* InKey, const void* InPair)
		{
			return !!KeyEqualityFn(InKey, InPair);
		});
}

CSEXPORT uint8* CSCONV Export_FScriptMap_FindValue(FScriptMap* instance, const void* Key, const FScriptMapLayout& MapLayout, uint32(CSCONV *GetKeyHash)(const void*), csbool(CSCONV *KeyEqualityFn)(const void*, const void*))
{
	// Create a wrapper func due to bool return result conversion
	return instance->FindValue(
		Key,
		MapLayout,
		[GetKeyHash](const void* InKey)
		{
			return GetKeyHash(InKey);
		},
		[KeyEqualityFn](const void* InKey, const void* InPair)
		{
			return !!KeyEqualityFn(InKey, InPair);
		});
}

CSEXPORT void CSCONV Export_FScriptMap_Add(FScriptMap* instance, const void* Key, const void* Value, const FScriptMapLayout& MapLayout, uint32(CSCONV *GetKeyHash)(const void*), csbool(CSCONV *KeyEqualityFn)(const void*, const void*), void(CSCONV *KeyConstructAndAssignFn)(void*), void(CSCONV *ValueConstructAndAssignFn)(void*), void(CSCONV *ValueAssignFn)(void*), void(CSCONV *DestructKeyFn)(void*), void(CSCONV *DestructValueFn)(void*))
{
	// Create a wrapper func due to bool return result conversion
	instance->Add(
		Key,
		Value,
		MapLayout,
		[GetKeyHash](const void* InKey)
		{
			return GetKeyHash(InKey);
		},
		[KeyEqualityFn](const void* InKey, const void* InPair)
		{
			return !!KeyEqualityFn(InKey, InPair);
		},
		[KeyConstructAndAssignFn](void* InKey)
		{
			KeyConstructAndAssignFn(InKey);
		},
		[ValueConstructAndAssignFn](void* InValue)
		{
			ValueConstructAndAssignFn(InValue);
		},
		[ValueAssignFn](void* InValue)
		{
			ValueAssignFn(InValue);
		},
		[DestructKeyFn](void* InKey)
		{
			DestructKeyFn(InKey);
		},
		[DestructValueFn](void* InValue)
		{
			DestructValueFn(InValue);
		});
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
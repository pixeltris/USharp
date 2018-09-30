CSEXPORT csbool CSCONV Export_FScriptBitArray_IsValidIndex(FScriptBitArray* instance, int32 Index)
{
	return instance->IsValidIndex(Index);
}

CSEXPORT FBitReference CSCONV Export_FScriptBitArray_Get(FScriptBitArray& instance, int32 Index)
{
	return instance[Index];
}

CSEXPORT void CSCONV Export_FScriptBitArray_Empty(FScriptBitArray* instance, int32 Slack)
{
	instance->Empty(Slack);
}

CSEXPORT int32 CSCONV Export_FScriptBitArray_Add(FScriptBitArray* instance, csbool Value)
{
	return instance->Add(!!Value);
}

CSEXPORT void CSCONV Export_FScriptBitArray_Destroy(FScriptBitArray* instance)
{
	instance->~FScriptBitArray();
}

CSEXPORT void CSCONV Export_FScriptBitArray(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FScriptBitArray_IsValidIndex);
	REGISTER_FUNC(Export_FScriptBitArray_Get);
	REGISTER_FUNC(Export_FScriptBitArray_Empty);
	REGISTER_FUNC(Export_FScriptBitArray_Add);
	REGISTER_FUNC(Export_FScriptBitArray_Destroy);
}
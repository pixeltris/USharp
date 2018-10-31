CSEXPORT void CSCONV Export_UEngine_CopyPropertiesForUnrelatedObjects(UObject* OldObject, UObject* NewObject, const FCopyPropertiesForUnrelatedObjectsParamsInterop& Params)
{
	UEngine::CopyPropertiesForUnrelatedObjects(OldObject, NewObject, FCopyPropertiesForUnrelatedObjectsParamsInterop::ToNative(Params));
}

CSEXPORT void CSCONV Export_UEngine_GetWorldContexts(UEngine* instance, TArray<const FWorldContext*>& result)
{
	// Unsure if TIndirectArray<> maps directly to TArray<> so recreating it as a TArray<> for now
	for (const FWorldContext& Context : instance->GetWorldContexts())
	{
		result.Add(&Context);
	}
}

CSEXPORT UWorld* CSCONV Export_UEngine_GetWorldFromContextObject(UEngine* instance, UObject* Object, EGetWorldErrorMode ErrorMode)
{
	return instance->GetWorldFromContextObject(Object, ErrorMode);
}

CSEXPORT void CSCONV Export_UEngine(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UEngine_CopyPropertiesForUnrelatedObjects);
	REGISTER_FUNC(Export_UEngine_GetWorldContexts);
	REGISTER_FUNC(Export_UEngine_GetWorldFromContextObject);
}
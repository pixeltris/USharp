CSEXPORT UObject* CSCONV Export_FLazyObjectPtr_Get(FLazyObjectPtr* instance)
{
	return instance->Get();
}

CSEXPORT void CSCONV Export_FLazyObjectPtr_SetUObject(FLazyObjectPtr* instance, UObject* value)
{
	*instance = value;
}

CSEXPORT void CSCONV Export_FLazyObjectPtr_SetFLazyObjectPtr(FLazyObjectPtr* instance, FLazyObjectPtr& value)
{
	*instance = value;
}

CSEXPORT csbool CSCONV Export_FLazyObjectPtr_IsPending(FLazyObjectPtr* instance)
{
	return instance->IsPending();
}

CSEXPORT csbool CSCONV Export_FLazyObjectPtr_IsValid(FLazyObjectPtr* instance)
{
	return instance->IsValid();
}

CSEXPORT csbool CSCONV Export_FLazyObjectPtr_IsStale(FLazyObjectPtr* instance)
{
	return instance->IsStale();
}

CSEXPORT csbool CSCONV Export_FLazyObjectPtr_IsNull(FLazyObjectPtr* instance)
{
	return instance->IsNull();
}

CSEXPORT void CSCONV Export_FLazyObjectPtr_Reset(FLazyObjectPtr* instance)
{
	instance->Reset();
}

CSEXPORT csbool CSCONV Export_FLazyObjectPtr_Equals(FLazyObjectPtr& instance, FLazyObjectPtr& compare)
{
	return instance == compare;
}

CSEXPORT uint32 CSCONV Export_FLazyObjectPtr_GetTypeHash(FLazyObjectPtr& instance)
{
	return GetTypeHash(instance);
}

CSEXPORT void CSCONV Export_FLazyObjectPtr(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FLazyObjectPtr_Get);
	REGISTER_FUNC(Export_FLazyObjectPtr_SetUObject);
	REGISTER_FUNC(Export_FLazyObjectPtr_SetFLazyObjectPtr);
	REGISTER_FUNC(Export_FLazyObjectPtr_IsPending);
	REGISTER_FUNC(Export_FLazyObjectPtr_IsValid);
	REGISTER_FUNC(Export_FLazyObjectPtr_IsStale);
	REGISTER_FUNC(Export_FLazyObjectPtr_IsNull);
	REGISTER_FUNC(Export_FLazyObjectPtr_Reset);
	REGISTER_FUNC(Export_FLazyObjectPtr_Equals);
	REGISTER_FUNC(Export_FLazyObjectPtr_GetTypeHash);
}
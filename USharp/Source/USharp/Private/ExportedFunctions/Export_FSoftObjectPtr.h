CSEXPORT UObject* CSCONV Export_FSoftObjectPtr_Get(FSoftObjectPtr* instance)
{
	return instance->Get();
}

CSEXPORT void CSCONV Export_FSoftObjectPtr_SetUObject(FSoftObjectPtr* instance, UObject* value)
{
	*instance = value;
}

CSEXPORT void CSCONV Export_FSoftObjectPtr_SetFWeakObjectPtr(FSoftObjectPtr* instance, FWeakObjectPtr& value)
{
	*instance = value;
}

CSEXPORT csbool CSCONV Export_FSoftObjectPtr_IsPending(FSoftObjectPtr* instance)
{
	return instance->IsPending();
}

CSEXPORT csbool CSCONV Export_FSoftObjectPtr_IsValid(FSoftObjectPtr* instance)
{
	return instance->IsValid();
}

CSEXPORT csbool CSCONV Export_FSoftObjectPtr_IsStale(FSoftObjectPtr* instance)
{
	return instance->IsStale();
}

CSEXPORT csbool CSCONV Export_FSoftObjectPtr_IsNull(FSoftObjectPtr* instance)
{
	return instance->IsNull();
}

CSEXPORT void CSCONV Export_FSoftObjectPtr_Reset(FSoftObjectPtr* instance)
{
	instance->Reset();
}

CSEXPORT UObject* CSCONV Export_FSoftObjectPtr_LoadSynchronous(FSoftObjectPtr* instance)
{
	return instance->LoadSynchronous();
}

CSEXPORT csbool CSCONV Export_FSoftObjectPtr_Equals(FSoftObjectPtr& instance, FSoftObjectPtr& compare)
{
	return instance == compare;
}

CSEXPORT uint32 CSCONV Export_FSoftObjectPtr_GetTypeHash(FSoftObjectPtr& instance)
{
	return GetTypeHash(instance);
}

CSEXPORT void CSCONV Export_FSoftObjectPtr(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FSoftObjectPtr_Get);
	REGISTER_FUNC(Export_FSoftObjectPtr_SetUObject);
	REGISTER_FUNC(Export_FSoftObjectPtr_SetFWeakObjectPtr);
	REGISTER_FUNC(Export_FSoftObjectPtr_IsPending);
	REGISTER_FUNC(Export_FSoftObjectPtr_IsValid);
	REGISTER_FUNC(Export_FSoftObjectPtr_IsStale);
	REGISTER_FUNC(Export_FSoftObjectPtr_IsNull);
	REGISTER_FUNC(Export_FSoftObjectPtr_Reset);
	REGISTER_FUNC(Export_FSoftObjectPtr_LoadSynchronous);
	REGISTER_FUNC(Export_FSoftObjectPtr_Equals);
	REGISTER_FUNC(Export_FSoftObjectPtr_GetTypeHash);
}
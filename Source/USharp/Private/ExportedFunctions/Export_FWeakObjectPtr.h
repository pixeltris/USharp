CSEXPORT UObject* CSCONV Export_FWeakObjectPtr_Get(FWeakObjectPtr* instance)
{
	return instance->Get();
}

CSEXPORT UObject* CSCONV Export_FWeakObjectPtr_GetEvenIfUnreachable(FWeakObjectPtr* instance)
{
	return instance->GetEvenIfUnreachable();
}

CSEXPORT void CSCONV Export_FWeakObjectPtr_SetUObject(FWeakObjectPtr* instance, UObject* value)
{
	*instance = value;
}

CSEXPORT void CSCONV Export_FWeakObjectPtr_SetFWeakObjectPtr(FWeakObjectPtr* instance, FWeakObjectPtr& value)
{
	*instance = value;
}

CSEXPORT csbool CSCONV Export_FWeakObjectPtr_IsValid(FWeakObjectPtr* instance, csbool bEvenIfPendingKill, csbool bThreadsafeTest)
{
	return instance->IsValid(!!bEvenIfPendingKill, !!bThreadsafeTest);	
}

CSEXPORT csbool CSCONV Export_FWeakObjectPtr_IsStale(FWeakObjectPtr* instance)
{
	return instance->IsStale();
}

CSEXPORT void CSCONV Export_FWeakObjectPtr_Reset(FWeakObjectPtr* instance)
{
	instance->Reset();
}

CSEXPORT csbool CSCONV Export_FWeakObjectPtr_Equals(FWeakObjectPtr& instance, FWeakObjectPtr& compare)
{
	return instance == compare;
}

CSEXPORT uint32 CSCONV Export_FWeakObjectPtr_GetTypeHash(FWeakObjectPtr& instance)
{
	return GetTypeHash(instance);
}

CSEXPORT void CSCONV Export_FWeakObjectPtr(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FWeakObjectPtr_Get);
	REGISTER_FUNC(Export_FWeakObjectPtr_GetEvenIfUnreachable);
	REGISTER_FUNC(Export_FWeakObjectPtr_SetUObject);
	REGISTER_FUNC(Export_FWeakObjectPtr_SetFWeakObjectPtr);
	REGISTER_FUNC(Export_FWeakObjectPtr_IsValid);
	REGISTER_FUNC(Export_FWeakObjectPtr_IsStale);
	REGISTER_FUNC(Export_FWeakObjectPtr_Reset);
	REGISTER_FUNC(Export_FWeakObjectPtr_Equals);
	REGISTER_FUNC(Export_FWeakObjectPtr_GetTypeHash);
}
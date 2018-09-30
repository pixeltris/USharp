CSEXPORT csbool CSCONV Export_UObjectBase_IsValidLowLevel(UObjectBase* instance)
{
	return instance->IsValidLowLevel();
}

CSEXPORT csbool CSCONV Export_UObjectBase_IsValidLowLevelFast(UObjectBase* instance, csbool bRecursive)
{
	return instance->IsValidLowLevelFast(!!bRecursive);
}

CSEXPORT uint32 CSCONV Export_UObjectBase_GetUniqueID(UObjectBase* instance)
{
	return instance->GetUniqueID();
}

CSEXPORT UClass* CSCONV Export_UObjectBase_GetClass(UObjectBase* instance)
{
	return instance->GetClass();
}

CSEXPORT UObject* CSCONV Export_UObjectBase_GetOuter(UObjectBase* instance)
{
	return instance->GetOuter();
}

CSEXPORT void CSCONV Export_UObjectBase_GetFName(UObjectBase* instance, FName& result)
{
	result = instance->GetFName();
}

CSEXPORT void CSCONV Export_UObjectBase_GetStatID(UObjectBase* instance, TStatId& result)
{
	result = instance->GetStatID();
}

CSEXPORT EObjectFlags CSCONV Export_UObjectBase_GetFlags(UObjectBase* instance)
{
	return instance->GetFlags();
}

CSEXPORT void CSCONV Export_UObjectBase_AtomicallySetFlags(UObjectBase* instance, EObjectFlags FlagsToAdd)
{
	instance->AtomicallySetFlags(FlagsToAdd);
}

CSEXPORT void CSCONV Export_UObjectBase_AtomicallyClearFlags(UObjectBase* instance, EObjectFlags FlagsToClear)
{
	instance->AtomicallyClearFlags(FlagsToClear);
}

CSEXPORT void CSCONV Export_UObjectBase_UObjectForceRegistration(UObjectBase* Object)
{
	UObjectForceRegistration(Object);
}

CSEXPORT void CSCONV Export_UObjectBase_ProcessNewlyLoadedUObjects()
{
	ProcessNewlyLoadedUObjects();
}

CSEXPORT void CSCONV Export_UObjectBase(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UObjectBase_IsValidLowLevel);
	REGISTER_FUNC(Export_UObjectBase_IsValidLowLevelFast);
	REGISTER_FUNC(Export_UObjectBase_GetUniqueID);
	REGISTER_FUNC(Export_UObjectBase_GetClass);
	REGISTER_FUNC(Export_UObjectBase_GetOuter);
	REGISTER_FUNC(Export_UObjectBase_GetFName);
	REGISTER_FUNC(Export_UObjectBase_GetStatID);
	REGISTER_FUNC(Export_UObjectBase_GetFlags);
	REGISTER_FUNC(Export_UObjectBase_AtomicallySetFlags);
	REGISTER_FUNC(Export_UObjectBase_AtomicallyClearFlags);
	REGISTER_FUNC(Export_UObjectBase_UObjectForceRegistration);
	REGISTER_FUNC(Export_UObjectBase_ProcessNewlyLoadedUObjects);
}
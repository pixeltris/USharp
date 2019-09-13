CSEXPORT UObject* CSCONV Export_FObjectInitializer_GetArchetype(FObjectInitializer* instance)
{
	return instance->GetArchetype();
}

CSEXPORT UObject* CSCONV Export_FObjectInitializer_GetObj(FObjectInitializer* instance)
{
	return instance->GetObj();
}

CSEXPORT UClass* CSCONV Export_FObjectInitializer_GetClass(FObjectInitializer* instance)
{
	return instance->GetClass();
}

CSEXPORT UObject* CSCONV Export_FObjectInitializer_CreateEditorOnlyDefaultSubobject(FObjectInitializer* instance, UObject* Outer, const FName& SubobjectName, UClass* ReturnType, csbool bTransient)
{
	return instance->CreateEditorOnlyDefaultSubobject(Outer, SubobjectName, ReturnType, !!bTransient);
}

CSEXPORT UObject* CSCONV Export_FObjectInitializer_CreateDefaultSubobject(FObjectInitializer* instance, UObject* Outer, const FName& SubobjectFName, UClass* ReturnType, UClass* ClassToCreateByDefault, csbool bIsRequired, csbool bIsTransient)
{
	return instance->CreateDefaultSubobject(Outer, SubobjectFName, ReturnType, ClassToCreateByDefault, !!bIsRequired, !!bIsTransient);
}

CSEXPORT FObjectInitializer const& CSCONV Export_FObjectInitializer_DoNotCreateDefaultSubobject(FObjectInitializer* instance, const FName& SubobjectName)
{
	return instance->DoNotCreateDefaultSubobject(SubobjectName);
}

CSEXPORT FObjectInitializer const& CSCONV Export_FObjectInitializer_DoNotCreateDefaultSubobjectStr(FObjectInitializer* instance, const FString& SubobjectName)
{
	return instance->DoNotCreateDefaultSubobject(*SubobjectName);
}

CSEXPORT csbool CSCONV Export_FObjectInitializer_IslegalOverride(FObjectInitializer* instance, const FName& InComponentName, class UClass *DerivedComponentClass, class UClass *BaseComponentClass)
{
	return instance->IslegalOverride(InComponentName, DerivedComponentClass, BaseComponentClass);
}

CSEXPORT void CSCONV Export_FObjectInitializer_FinalizeSubobjectClassInitialization(FObjectInitializer* instance)
{
	instance->FinalizeSubobjectClassInitialization();
}

CSEXPORT void CSCONV Export_FObjectInitializer_AssertIfInConstructor(UObject* Outer, const FString& ErrorMessage)
{
	FObjectInitializer::AssertIfInConstructor(Outer, *ErrorMessage);
}

CSEXPORT FObjectInitializer& CSCONV Export_FObjectInitializer_Get()
{
	return FObjectInitializer::Get();
}

CSEXPORT void CSCONV Export_FObjectInitializer(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FObjectInitializer_GetArchetype);
	REGISTER_FUNC(Export_FObjectInitializer_GetObj);
	REGISTER_FUNC(Export_FObjectInitializer_GetClass);
	REGISTER_FUNC(Export_FObjectInitializer_CreateEditorOnlyDefaultSubobject);
	REGISTER_FUNC(Export_FObjectInitializer_CreateDefaultSubobject);
	REGISTER_FUNC(Export_FObjectInitializer_DoNotCreateDefaultSubobject);
	REGISTER_FUNC(Export_FObjectInitializer_DoNotCreateDefaultSubobjectStr);
	REGISTER_FUNC(Export_FObjectInitializer_IslegalOverride);
	REGISTER_FUNC(Export_FObjectInitializer_FinalizeSubobjectClassInitialization);
	REGISTER_FUNC(Export_FObjectInitializer_AssertIfInConstructor);
	REGISTER_FUNC(Export_FObjectInitializer_Get);
}
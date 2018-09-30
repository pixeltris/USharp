CSEXPORT UClass* CSCONV Export_UBlueprintCore_Get_SkeletonGeneratedClass(UBlueprintCore* instance)
{
	return *instance->SkeletonGeneratedClass;
}

CSEXPORT void CSCONV Export_UBlueprintCore_Set_SkeletonGeneratedClass(UBlueprintCore* instance, UClass* value)
{
	instance->SkeletonGeneratedClass = value;
}

CSEXPORT UClass* CSCONV Export_UBlueprintCore_Get_GeneratedClass(UBlueprintCore* instance)
{
	return *instance->GeneratedClass;
}

CSEXPORT void CSCONV Export_UBlueprintCore_Set_GeneratedClass(UBlueprintCore* instance, UClass* value)
{
	instance->GeneratedClass = value;
}

CSEXPORT void CSCONV Export_UBlueprintCore(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UBlueprintCore_Get_SkeletonGeneratedClass);
	REGISTER_FUNC(Export_UBlueprintCore_Set_SkeletonGeneratedClass);
	REGISTER_FUNC(Export_UBlueprintCore_Get_GeneratedClass);
	REGISTER_FUNC(Export_UBlueprintCore_Set_GeneratedClass);
}
CSEXPORT UClass* CSCONV Export_UBlueprint_Get_ParentClass(UBlueprint* instance)
{
	return instance->ParentClass;
}

CSEXPORT void CSCONV Export_UBlueprint_Set_ParentClass(UBlueprint* instance, UClass* value)
{
	instance->ParentClass = value;
}

CSEXPORT UBlueprint* CSCONV Export_UBlueprint_GetBlueprintFromClass(const UClass* InClass)
{
	return UBlueprint::GetBlueprintFromClass(InClass);
}

CSEXPORT csbool CSCONV Export_UBlueprint_GetBlueprintHierarchyFromClass(const UClass* InClass, TArray<UBlueprint*>& OutBlueprintParents)
{
	return UBlueprint::GetBlueprintHierarchyFromClass(InClass, OutBlueprintParents);
}

CSEXPORT void CSCONV Export_UBlueprint(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UBlueprint_Get_ParentClass);
	REGISTER_FUNC(Export_UBlueprint_Set_ParentClass);
	REGISTER_FUNC(Export_UBlueprint_GetBlueprintFromClass);
	REGISTER_FUNC(Export_UBlueprint_GetBlueprintHierarchyFromClass);
}
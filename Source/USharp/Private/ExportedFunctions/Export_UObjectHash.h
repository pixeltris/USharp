CSEXPORT void CSCONV Export_UObjectHash_GetObjectsWithOuter(const class UObjectBase* Outer, TArray<UObject*>& Results, csbool bIncludeNestedObjects, EObjectFlags ExclusionFlags, EInternalObjectFlags ExclusionInternalFlags)
{
	GetObjectsWithOuter(Outer, Results, !!bIncludeNestedObjects, ExclusionFlags, ExclusionInternalFlags);
}

CSEXPORT UObjectBase* CSCONV Export_UObjectHash_FindObjectWithOuter(class UObjectBase* Outer, class UClass* ClassToLookFor, const FName& NameToLookFor)
{
	return FindObjectWithOuter(Outer, ClassToLookFor, NameToLookFor);
}

CSEXPORT void CSCONV Export_UObjectHash_GetObjectsOfClass(UClass* ClassToLookFor, TArray<UObject*>& Results, csbool bIncludeDerivedClasses, EObjectFlags AdditionalExcludeFlags, EInternalObjectFlags ExclusionInternalFlags)
{
	GetObjectsOfClass(ClassToLookFor, Results, !!bIncludeDerivedClasses, AdditionalExcludeFlags, ExclusionInternalFlags);
}

CSEXPORT void CSCONV Export_UObjectHash_GetDerivedClasses(UClass* ClassToLookFor, TArray<UClass*>& Results, csbool bRecursive)
{
	GetDerivedClasses(ClassToLookFor, Results, !!bRecursive);
}

CSEXPORT void CSCONV Export_UObjectHash(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UObjectHash_GetObjectsWithOuter);
	REGISTER_FUNC(Export_UObjectHash_FindObjectWithOuter);
	REGISTER_FUNC(Export_UObjectHash_GetObjectsOfClass);
	REGISTER_FUNC(Export_UObjectHash_GetDerivedClasses);
}
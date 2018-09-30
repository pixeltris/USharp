CSEXPORT FReferenceFinder* CSCONV Export_FReferenceFinder_New(TArray<UObject*>& InObjectArray, UObject* InOuter, csbool bInRequireDirectOuter, csbool bInShouldIgnoreArchetype, csbool bInSerializeRecursively, csbool bInShouldIgnoreTransient)
{
	return new FReferenceFinder(InObjectArray, InOuter, !!bInRequireDirectOuter, !!bInShouldIgnoreArchetype, !!bInSerializeRecursively, !!bInShouldIgnoreTransient);
}

CSEXPORT void CSCONV Export_FReferenceFinder_Delete(FReferenceFinder* instance)
{
	delete instance;
}

CSEXPORT void CSCONV Export_FReferenceFinder_FindReferences(FReferenceFinder* instance, UObject* Object, UObject* ReferencingObject, UProperty* ReferencingProperty)
{
	instance->FindReferences(Object, ReferencingObject, ReferencingProperty);
}

CSEXPORT void CSCONV Export_FReferenceFinder(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FReferenceFinder_New);
	REGISTER_FUNC(Export_FReferenceFinder_Delete);
	REGISTER_FUNC(Export_FReferenceFinder_FindReferences);
}
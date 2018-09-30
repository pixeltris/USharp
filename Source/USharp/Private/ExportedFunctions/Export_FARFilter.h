CSEXPORT FARFilter* CSCONV Export_FARFilter_New()
{
	return new FARFilter();
}

CSEXPORT void CSCONV Export_FARFilter_Delete(FARFilter* instance)
{
	delete instance;
}

CSEXPORT void CSCONV Export_FARFilter_Set_PackageNames(FARFilter* instance, const TArray<FName>& value)
{
	instance->PackageNames = value;
}

CSEXPORT void CSCONV Export_FARFilter_Set_PackagePaths(FARFilter* instance, const TArray<FName>& value)
{
	instance->PackagePaths = value;
}

CSEXPORT void CSCONV Export_FARFilter_Set_ObjectPaths(FARFilter* instance, const TArray<FName>& value)
{
	instance->ObjectPaths = value;
}

CSEXPORT void CSCONV Export_FARFilter_Set_ClassNames(FARFilter* instance, const TArray<FName>& value)
{
	instance->ClassNames = value;
}

CSEXPORT void CSCONV Export_FARFilter_Set_TagsAndValues(FARFilter* instance, const TArray<FName>& keys, const TArray<FString>& values)
{
	instance->TagsAndValues.Empty(keys.Num());
	if (keys.Num() == values.Num())
	{
		for (int32 Index = 0; Index < keys.Num(); ++Index)
		{
			instance->TagsAndValues.Add(keys[Index], values[Index]);
		}
	}
}

CSEXPORT void CSCONV Export_FARFilter_Set_RecursiveClassesExclusionSet(FARFilter* instance, const TArray<FName>& value)
{
	instance->RecursiveClassesExclusionSet.Empty(value.Num());
	instance->RecursiveClassesExclusionSet.Append(value);
}

CSEXPORT void CSCONV Export_FARFilter_Set_bRecursivePaths(FARFilter* instance, csbool value)
{
	instance->bRecursivePaths = !!value;
}

CSEXPORT void CSCONV Export_FARFilter_Set_bRecursiveClasses(FARFilter* instance, csbool value)
{
	instance->bRecursiveClasses = !!value;
}

CSEXPORT void CSCONV Export_FARFilter_Set_bIncludeOnlyOnDiskAssets(FARFilter* instance, csbool value)
{
	instance->bIncludeOnlyOnDiskAssets = !!value;
}

CSEXPORT void CSCONV Export_FARFilter(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FARFilter_New);
	REGISTER_FUNC(Export_FARFilter_Delete);
	REGISTER_FUNC(Export_FARFilter_Set_PackageNames);
	REGISTER_FUNC(Export_FARFilter_Set_PackagePaths);
	REGISTER_FUNC(Export_FARFilter_Set_ObjectPaths);
	REGISTER_FUNC(Export_FARFilter_Set_ClassNames);
	REGISTER_FUNC(Export_FARFilter_Set_TagsAndValues);
	REGISTER_FUNC(Export_FARFilter_Set_RecursiveClassesExclusionSet);
	REGISTER_FUNC(Export_FARFilter_Set_bRecursivePaths);
	REGISTER_FUNC(Export_FARFilter_Set_bRecursiveClasses);
	REGISTER_FUNC(Export_FARFilter_Set_bIncludeOnlyOnDiskAssets);
}
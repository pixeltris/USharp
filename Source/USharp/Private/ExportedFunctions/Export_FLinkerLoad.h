CSEXPORT void CSCONV Export_FLinkerLoad_FindPreviousNamesForClass(const FString& CurrentClassPath, csbool bIsInstance, TArray<FName>& result)
{
	result = FLinkerLoad::FindPreviousNamesForClass(CurrentClassPath, !!bIsInstance);
}

CSEXPORT void CSCONV Export_FLinkerLoad_FindNewNameForClass(const FName& OldClassName, csbool bIsInstance, FName& result)
{
	result = FLinkerLoad::FindNewNameForClass(OldClassName, !!bIsInstance);
}

CSEXPORT void CSCONV Export_FLinkerLoad_FindNewNameForEnum(const FName& OldEnumName, FName& result)
{
	result = FLinkerLoad::FindNewNameForEnum(OldEnumName);
}

CSEXPORT void CSCONV Export_FLinkerLoad_FindNewNameForStruct(const FName& OldStructName, FName& result)
{
	result = FLinkerLoad::FindNewNameForStruct(OldStructName);
}

CSEXPORT void CSCONV Export_FLinkerLoad_InvalidateExport(UObject* OldObject)
{
	FLinkerLoad::InvalidateExport(OldObject);
}

CSEXPORT void CSCONV Export_FLinkerLoad(RegisterFunc registerFunc)
{	
	REGISTER_FUNC(Export_FLinkerLoad_FindPreviousNamesForClass);
	REGISTER_FUNC(Export_FLinkerLoad_FindNewNameForClass);
	REGISTER_FUNC(Export_FLinkerLoad_FindNewNameForEnum);
	REGISTER_FUNC(Export_FLinkerLoad_FindNewNameForStruct);
	REGISTER_FUNC(Export_FLinkerLoad_InvalidateExport);
}
CSEXPORT void CSCONV Export_FText_CreateEmpty(FText& result)
{
	result = FText();
}

CSEXPORT void CSCONV Export_FText_CreateText(const FString& Namespace, const FString& Key, const FString& Literal, FText& result)
{
	result = FInternationalization::ForUseOnlyByLocMacroAndGraphNodeTextLiterals_CreateText(*Literal, *Namespace, *Key);
}

CSEXPORT void CSCONV Export_FText_GetInvariantTimeZone(FString& result)
{
	result = FText::GetInvariantTimeZone();
}

CSEXPORT csbool CSCONV Export_FText_FindText(const FString& Namespace, const FString& Key, FText& OutText, const FString* SourceString)
{
	if (SourceString != nullptr && SourceString->IsEmpty())
	{
		SourceString = nullptr;
	}
	return FText::FindText(Namespace, Key, OutText, SourceString);
}

CSEXPORT void CSCONV Export_FText_FromStringTable(const FName& InTableId, const FString& InKey, const EStringTableLoadingPolicy InLoadingPolicy, FText& result)
{
	result = FText::FromStringTable(InTableId, InKey, InLoadingPolicy);
}

CSEXPORT void CSCONV Export_FText_FromName(const FName& Val, FText& result)
{
	result = FText::FromName(Val);
}

CSEXPORT void CSCONV Export_FText_FromString(const FString& String, FText& result)
{
	result = FText::FromString(String);
}

CSEXPORT void CSCONV Export_FText_AsCultureInvariant(const FString& String, FText& result)
{
	result = FText::AsCultureInvariant(String);
}

CSEXPORT void CSCONV Export_FText_AsCultureInvariantText(const FText& Text, FText& result)
{
	result = FText::AsCultureInvariant(Text);
}

CSEXPORT void CSCONV Export_FText_ToString(FText* instance, FString& result)
{
	result = instance->ToString();
}

CSEXPORT void CSCONV Export_FText_BuildSourceString(FText* instance, FString& result)
{
	result = instance->BuildSourceString();
}

CSEXPORT csbool CSCONV Export_FText_IsNumeric(FText* instance)
{
	return instance->IsNumeric();
}

CSEXPORT int32 CSCONV Export_FText_CompareTo(FText* instance, const FText& Other, const ETextComparisonLevel::Type ComparisonLevel)
{
	return instance->CompareTo(Other, ComparisonLevel);
}

CSEXPORT int32 CSCONV Export_FText_CompareToCaseIgnored(FText* instance, const FText& Other)
{
	return instance->CompareToCaseIgnored(Other);
}

CSEXPORT csbool CSCONV Export_FText_EqualTo(FText* instance, const FText& Other, const ETextComparisonLevel::Type ComparisonLevel)
{
	return instance->EqualTo(Other, ComparisonLevel);
}

CSEXPORT csbool CSCONV Export_FText_EqualToCaseIgnored(FText* instance, const FText& Other)
{
	return instance->EqualToCaseIgnored(Other);
}

CSEXPORT csbool CSCONV Export_FText_IdenticalTo(FText* instance, const FText& Other)
{
	return instance->IdenticalTo(Other);
}

CSEXPORT csbool CSCONV Export_FText_IsEmpty(FText* instance)
{
	return instance->IsEmpty();
}

CSEXPORT csbool CSCONV Export_FText_IsEmptyOrWhitespace(FText* instance)
{
	return instance->IsEmptyOrWhitespace();
}

CSEXPORT void CSCONV Export_FText_ToLower(FText* instance, FText& result)
{
	result = instance->ToLower();
}

CSEXPORT void CSCONV Export_FText_ToUpper(FText* instance, FText& result)
{
	result = instance->ToUpper();
}

CSEXPORT void CSCONV Export_FText_TrimPreceding(FText* instance, FText& result)
{
	result = FText::TrimPreceding(*instance);
}

CSEXPORT void CSCONV Export_FText_TrimTrailing(FText* instance, FText& result)
{
	result = FText::TrimTrailing(*instance);
}

CSEXPORT void CSCONV Export_FText_TrimPrecedingAndTrailing(FText* instance, FText& result)
{
	result = FText::TrimPrecedingAndTrailing(*instance);
}

CSEXPORT csbool CSCONV Export_FText_IsTransient(FText* instance)
{
	return instance->IsTransient();
}

CSEXPORT csbool CSCONV Export_FText_IsCultureInvariant(FText* instance)
{
	return instance->IsCultureInvariant();
}

CSEXPORT csbool CSCONV Export_FText_IsFromStringTable(FText* instance)
{
	return instance->IsFromStringTable();
}

CSEXPORT csbool CSCONV Export_FText_ShouldGatherForLocalization(FText* instance)
{
	return instance->ShouldGatherForLocalization();
}

#if WITH_EDITOR
CSEXPORT void CSCONV Export_FText_ChangeKey(const FString& Namespace, const FString& Key, const FText& Text, FText& result)
{
	result = FText::ChangeKey(Namespace, Key, Text);
}
#endif

CSEXPORT void CSCONV Export_FText(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FText_CreateEmpty);
	REGISTER_FUNC(Export_FText_CreateText);
	REGISTER_FUNC(Export_FText_GetInvariantTimeZone);
	REGISTER_FUNC(Export_FText_FindText);
	REGISTER_FUNC(Export_FText_FromStringTable);
	REGISTER_FUNC(Export_FText_FromName);
	REGISTER_FUNC(Export_FText_FromString);
	REGISTER_FUNC(Export_FText_AsCultureInvariant);
	REGISTER_FUNC(Export_FText_AsCultureInvariantText);
	REGISTER_FUNC(Export_FText_ToString);
	REGISTER_FUNC(Export_FText_BuildSourceString);
	REGISTER_FUNC(Export_FText_IsNumeric);
	REGISTER_FUNC(Export_FText_CompareTo);
	REGISTER_FUNC(Export_FText_CompareToCaseIgnored);
	REGISTER_FUNC(Export_FText_EqualTo);
	REGISTER_FUNC(Export_FText_EqualToCaseIgnored);
	REGISTER_FUNC(Export_FText_IdenticalTo);
	REGISTER_FUNC(Export_FText_IsEmpty);
	REGISTER_FUNC(Export_FText_IsEmptyOrWhitespace);
	REGISTER_FUNC(Export_FText_ToLower);
	REGISTER_FUNC(Export_FText_ToUpper);
	REGISTER_FUNC(Export_FText_TrimPreceding);
	REGISTER_FUNC(Export_FText_TrimTrailing);
	REGISTER_FUNC(Export_FText_TrimPrecedingAndTrailing);
	REGISTER_FUNC(Export_FText_IsTransient);
	REGISTER_FUNC(Export_FText_IsCultureInvariant);
	REGISTER_FUNC(Export_FText_IsFromStringTable);
	REGISTER_FUNC(Export_FText_ShouldGatherForLocalization);
#if WITH_EDITOR
	REGISTER_FUNC(Export_FText_ChangeKey);
#endif
}
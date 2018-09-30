CSEXPORT void CSCONV Export_FName_FromEName(FName& OutName, EName N)
{
	OutName = FName(N);
}

CSEXPORT void CSCONV Export_FName_FromENameNumber(FName& OutName, EName N, int32 InNumber)
{
	OutName = FName(N, InNumber);
}

CSEXPORT void CSCONV Export_FName_FromString(FName& OutName, const FString& Str, EFindName FindType)
{
	OutName = FName(*Str, FindType);
}

CSEXPORT void CSCONV Export_FName_FromStringNumber(FName& OutName, const FString& Str, int32 InNumber, EFindName FindType)
{
	OutName = FName(*Str, InNumber, FindType);
}

CSEXPORT void CSCONV Export_FName_ToString(FName& instance, FString& result)
{
	result = instance.ToString();
}

CSEXPORT void CSCONV Export_FName_GetPlainNameString(FName& instance, FString& result)
{
	result = instance.GetPlainNameString();
}

CSEXPORT csbool CSCONV Export_FName_IsEqual(const FName& instance, FName& Other, const ENameCase CompareMethod, const csbool bCompareNumber)
{	
	return instance.IsEqual(Other, CompareMethod, !!bCompareNumber);
}

CSEXPORT int32 CSCONV Export_FName_Compare(const FName& instance, const FName& Other)
{
	return instance.Compare(Other);
}

CSEXPORT void CSCONV Export_FName(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FName_FromEName);
	REGISTER_FUNC(Export_FName_FromENameNumber);
	REGISTER_FUNC(Export_FName_FromString);
	REGISTER_FUNC(Export_FName_FromStringNumber);
	REGISTER_FUNC(Export_FName_ToString);
	REGISTER_FUNC(Export_FName_GetPlainNameString);
	REGISTER_FUNC(Export_FName_IsEqual);
	REGISTER_FUNC(Export_FName_Compare);
}
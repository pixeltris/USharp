CSEXPORT int32 CSCONV Export_IConsoleVariable_GetInt(IConsoleVariable* instance)
{
	return instance->GetInt();
}

CSEXPORT float CSCONV Export_IConsoleVariable_GetFloat(IConsoleVariable* instance)
{
	return instance->GetFloat();
}

CSEXPORT void CSCONV Export_IConsoleVariable_GetString(IConsoleVariable* instance, FString& result)
{
	result = instance->GetString();
}

CSEXPORT void CSCONV Export_IConsoleVariable_SetOnChangedCallback(IConsoleVariable* instance, void(*handler)(IConsoleVariable*))
{
	instance->SetOnChangedCallback(FConsoleVariableDelegate::CreateStatic(handler));
}

CSEXPORT void CSCONV Export_IConsoleVariable_ClearOnChangedCallback(IConsoleVariable* instance)
{
	FConsoleVariableDelegate EmptyDelegate;
	instance->SetOnChangedCallback(EmptyDelegate);
}

CSEXPORT void CSCONV Export_IConsoleVariable_SetInt(IConsoleVariable* instance, int32 InValue, EConsoleVariableFlags SetBy)
{
	instance->Set(InValue, SetBy);
}

CSEXPORT void CSCONV Export_IConsoleVariable_SetFloat(IConsoleVariable* instance, float InValue, EConsoleVariableFlags SetBy)
{
	instance->Set(InValue, SetBy);
}

CSEXPORT void CSCONV Export_IConsoleVariable_SetString(IConsoleVariable* instance, const FString& InValue, EConsoleVariableFlags SetBy)
{
	instance->Set(*InValue, SetBy);
}

CSEXPORT void CSCONV Export_IConsoleVariable(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_IConsoleVariable_GetInt);
	REGISTER_FUNC(Export_IConsoleVariable_GetFloat);
	REGISTER_FUNC(Export_IConsoleVariable_GetString);
	REGISTER_FUNC(Export_IConsoleVariable_SetOnChangedCallback);
	REGISTER_FUNC(Export_IConsoleVariable_ClearOnChangedCallback);
	REGISTER_FUNC(Export_IConsoleVariable_SetInt);
	REGISTER_FUNC(Export_IConsoleVariable_SetFloat);
	REGISTER_FUNC(Export_IConsoleVariable_SetString);
}
CSEXPORT void CSCONV Export_IConsoleObject_GetHelp(IConsoleObject* instance, FString& result)
{
	result = instance->GetHelp();
}

CSEXPORT void CSCONV Export_IConsoleObject_SetHelp(IConsoleObject* instance, const FString& Value)
{
	instance->SetHelp(*Value);
}

CSEXPORT EConsoleVariableFlags CSCONV Export_IConsoleObject_GetFlags(IConsoleObject* instance)
{
	return instance->GetFlags();
}

CSEXPORT void CSCONV Export_IConsoleObject_SetFlags(IConsoleObject* instance, EConsoleVariableFlags Value)
{
	instance->SetFlags(Value);
}

CSEXPORT void CSCONV Export_IConsoleObject_ClearFlags(IConsoleObject* instance, EConsoleVariableFlags Value)
{
	instance->ClearFlags(Value);
}

CSEXPORT csbool CSCONV Export_IConsoleObject_TestFlags(IConsoleObject* instance, EConsoleVariableFlags Value)
{
	return instance->TestFlags(Value);
}

CSEXPORT IConsoleVariable* CSCONV Export_IConsoleObject_AsVariable(IConsoleObject* instance)
{
	return instance->AsVariable();
}

CSEXPORT IConsoleCommand* CSCONV Export_IConsoleObject_AsCommand(IConsoleObject* instance)
{
	return instance->AsCommand();
}

CSEXPORT void CSCONV Export_IConsoleObject(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_IConsoleObject_GetHelp);
	REGISTER_FUNC(Export_IConsoleObject_SetHelp);
	REGISTER_FUNC(Export_IConsoleObject_GetFlags);
	REGISTER_FUNC(Export_IConsoleObject_SetFlags);
	REGISTER_FUNC(Export_IConsoleObject_ClearFlags);
	REGISTER_FUNC(Export_IConsoleObject_TestFlags);
	REGISTER_FUNC(Export_IConsoleObject_AsVariable);
	REGISTER_FUNC(Export_IConsoleObject_AsCommand);
}
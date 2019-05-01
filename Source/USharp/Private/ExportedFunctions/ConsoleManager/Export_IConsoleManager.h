CSEXPORT IConsoleManager& CSCONV Export_IConsoleManager_Get()
{
	return IConsoleManager::Get();
}

CSEXPORT IConsoleVariable* CSCONV Export_IConsoleManager_RegisterConsoleVariableInt(IConsoleManager* instance, const FString& Name, int32 DefaultValue, const FString& Help, uint32 Flags)
{
	return instance->RegisterConsoleVariable(*Name, DefaultValue, *Help, Flags);
}

CSEXPORT IConsoleVariable* CSCONV Export_IConsoleManager_RegisterConsoleVariableFloat(IConsoleManager* instance, const FString& Name, float DefaultValue, const FString& Help, uint32 Flags)
{
	return instance->RegisterConsoleVariable(*Name, DefaultValue, *Help, Flags);
}

CSEXPORT IConsoleVariable* CSCONV Export_IConsoleManager_RegisterConsoleVariableString(IConsoleManager* instance, const FString& Name, const FString& DefaultValue, const FString& Help, uint32 Flags)
{
	return instance->RegisterConsoleVariable(*Name, DefaultValue, *Help, Flags);
}

CSEXPORT void CSCONV Export_IConsoleManager_CallAllConsoleVariableSinks(IConsoleManager* instance)
{
	instance->CallAllConsoleVariableSinks();
}

CSEXPORT void CSCONV Export_IConsoleManager_RegisterConsoleVariableSink_Handle(IConsoleManager* instance, void(CSCONV *handler)(), FConsoleVariableSinkHandle& OutHandle)
{
	auto Callback = [handler]() { handler(); };
	OutHandle = instance->RegisterConsoleVariableSink_Handle(FConsoleCommandDelegate::CreateLambda([handler](){handler();}));
}

CSEXPORT void CSCONV Export_IConsoleManager_UnregisterConsoleVariableSink_Handle(IConsoleManager* instance, FConsoleVariableSinkHandle& Handle)
{
	instance->UnregisterConsoleVariableSink_Handle(Handle);
}

CSEXPORT IConsoleCommand* CSCONV Export_IConsoleManager_RegisterConsoleCommandDefault(IConsoleManager* instance, const FString& Name, const FString& Help, void(CSCONV *handler)(), uint32 Flags)
{
	auto Callback = [handler]() { handler(); };
	return instance->RegisterConsoleCommand(*Name, *Help, FConsoleCommandDelegate::CreateLambda(Callback), Flags);
}

CSEXPORT IConsoleCommand* CSCONV Export_IConsoleManager_RegisterConsoleCommandWithArgs(IConsoleManager* instance, const FString& Name, const FString& Help, void(CSCONV *handler)(const TArray<FString>&), uint32 Flags)
{
	auto Callback = [handler](const TArray<FString>& Args) { handler(Args); };
	return instance->RegisterConsoleCommand(*Name, *Help, FConsoleCommandWithArgsDelegate::CreateLambda(handler), Flags);
}

CSEXPORT IConsoleCommand* CSCONV Export_IConsoleManager_RegisterConsoleCommandWithWorld(IConsoleManager* instance, const FString& Name, const FString& Help, void(CSCONV *handler)(UWorld*), uint32 Flags)
{
	auto Callback = [handler](UWorld* World) { handler(World); };
	return instance->RegisterConsoleCommand(*Name, *Help, FConsoleCommandWithWorldDelegate::CreateLambda(Callback), Flags);
}

CSEXPORT IConsoleCommand* CSCONV Export_IConsoleManager_RegisterConsoleCommandWithWorldAndArgs(IConsoleManager* instance, const FString& Name, const FString& Help, void(CSCONV *handler)(const TArray<FString>&, UWorld*), uint32 Flags)
{
	auto Callback = [handler](const TArray<FString>& Args, UWorld* World) { handler(Args, World); };
	return instance->RegisterConsoleCommand(*Name, *Help, FConsoleCommandWithWorldAndArgsDelegate::CreateLambda(Callback), Flags);
}

CSEXPORT IConsoleCommand* CSCONV Export_IConsoleManager_RegisterConsoleCommandWithOutputDevice(IConsoleManager* instance, const FString& Name, const FString& Help, void(CSCONV *handler)(FOutputDevice&), uint32 Flags)
{
	auto Callback = [handler](FOutputDevice& OutputDevice) { handler(OutputDevice); };
	return instance->RegisterConsoleCommand(*Name, *Help, FConsoleCommandWithOutputDeviceDelegate::CreateLambda(Callback), Flags);
}

CSEXPORT IConsoleCommand* CSCONV Export_IConsoleManager_RegisterConsoleCommandExec(IConsoleManager* instance, const FString& Name, const FString& Help, uint32 Flags)
{
	return instance->RegisterConsoleCommand(*Name, *Help, Flags);
}

CSEXPORT void CSCONV Export_IConsoleManager_UnregisterConsoleObject(IConsoleManager* instance, IConsoleObject* ConsoleObject, csbool bKeepState)
{
	instance->UnregisterConsoleObject(ConsoleObject, !!bKeepState);
}

CSEXPORT IConsoleVariable* CSCONV Export_IConsoleManager_FindConsoleVariable(IConsoleManager* instance, const FString& Name)
{
	return instance->FindConsoleVariable(*Name);
}

CSEXPORT IConsoleObject* CSCONV Export_IConsoleManager_FindConsoleObject(IConsoleManager* instance, const FString& Name)
{
	return instance->FindConsoleObject(*Name);
}

CSEXPORT void CSCONV Export_IConsoleManager_ForEachConsoleObjectThatStartsWith(IConsoleManager* instance, void(CSCONV *handler)(const TCHAR*, IConsoleObject*), const FString& ThatStartsWith)
{
	auto Callback = [handler](const TCHAR* Name, IConsoleObject* CVar) { handler(Name, CVar); };
	instance->ForEachConsoleObjectThatStartsWith(FConsoleObjectVisitor::CreateLambda(Callback), *ThatStartsWith);
}

CSEXPORT void CSCONV Export_IConsoleManager_ForEachConsoleObjectThatContains(IConsoleManager* instance, void(CSCONV *handler)(const TCHAR*, IConsoleObject*), const FString& Contains)
{
	auto Callback = [handler](const TCHAR* Name, IConsoleObject* CVar) { handler(Name, CVar); };
	instance->ForEachConsoleObjectThatContains(FConsoleObjectVisitor::CreateLambda(Callback), *Contains);
}

CSEXPORT csbool CSCONV Export_IConsoleManager_ProcessUserConsoleInput(IConsoleManager* instance, const FString& Input, FOutputDevice& Ar, UWorld* InWorld)
{
	return instance->ProcessUserConsoleInput(*Input, Ar, InWorld);
}

CSEXPORT void CSCONV Export_IConsoleManager_AddConsoleHistoryEntry(IConsoleManager* instance, const FString& Key, const FString& Input)
{
	instance->AddConsoleHistoryEntry(*Key, *Input);
}

CSEXPORT void CSCONV Export_IConsoleManager_GetConsoleHistory(IConsoleManager* instance, const FString& Key, TArray<FString>& Out)
{
	instance->GetConsoleHistory(*Key, Out);
}

CSEXPORT csbool CSCONV Export_IConsoleManager_IsNameRegistered(IConsoleManager* instance, const FString& Input)
{	
	return instance->IsNameRegistered(*Input);
}

CSEXPORT void CSCONV Export_IConsoleManager(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_IConsoleManager_Get);
	REGISTER_FUNC(Export_IConsoleManager_RegisterConsoleVariableInt);
	REGISTER_FUNC(Export_IConsoleManager_RegisterConsoleVariableFloat);
	REGISTER_FUNC(Export_IConsoleManager_RegisterConsoleVariableString);
	REGISTER_FUNC(Export_IConsoleManager_CallAllConsoleVariableSinks);
	REGISTER_FUNC(Export_IConsoleManager_RegisterConsoleVariableSink_Handle);
	REGISTER_FUNC(Export_IConsoleManager_UnregisterConsoleVariableSink_Handle);
	REGISTER_FUNC(Export_IConsoleManager_RegisterConsoleCommandDefault);
	REGISTER_FUNC(Export_IConsoleManager_RegisterConsoleCommandWithArgs);
	REGISTER_FUNC(Export_IConsoleManager_RegisterConsoleCommandWithWorld);
	REGISTER_FUNC(Export_IConsoleManager_RegisterConsoleCommandWithWorldAndArgs);
	REGISTER_FUNC(Export_IConsoleManager_RegisterConsoleCommandWithOutputDevice);
	REGISTER_FUNC(Export_IConsoleManager_RegisterConsoleCommandExec);
	REGISTER_FUNC(Export_IConsoleManager_UnregisterConsoleObject);
	REGISTER_FUNC(Export_IConsoleManager_FindConsoleVariable);
	REGISTER_FUNC(Export_IConsoleManager_FindConsoleObject);
	REGISTER_FUNC(Export_IConsoleManager_ForEachConsoleObjectThatStartsWith);
	REGISTER_FUNC(Export_IConsoleManager_ForEachConsoleObjectThatContains);
	REGISTER_FUNC(Export_IConsoleManager_ProcessUserConsoleInput);
	REGISTER_FUNC(Export_IConsoleManager_AddConsoleHistoryEntry);
	REGISTER_FUNC(Export_IConsoleManager_GetConsoleHistory);
	REGISTER_FUNC(Export_IConsoleManager_IsNameRegistered);
}
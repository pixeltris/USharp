CSEXPORT void CSCONV Export_IConsoleCommand_Execute(IConsoleCommand* instance, const TArray<FString>& Args, UWorld* InWorld, FOutputDevice& OutputDevice)
{
	instance->Execute(Args, InWorld, OutputDevice);
}

CSEXPORT void CSCONV Export_IConsoleCommand(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_IConsoleCommand_Execute);
}
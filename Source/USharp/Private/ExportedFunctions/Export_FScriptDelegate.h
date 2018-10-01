CSEXPORT void CSCONV Export_FScriptDelegate_ProcessDelegate(FScriptDelegate* instance, void* Parameters)
{
	instance->ProcessDelegate<UObject>(Parameters);
}

CSEXPORT void CSCONV Export_FScriptDelegate(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FScriptDelegate_ProcessDelegate);
}
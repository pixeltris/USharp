CSEXPORT void CSCONV Export_FMulticastScriptDelegate_ProcessMulticastDelegate(FMulticastScriptDelegate* instance, void* Parameters)
{
	instance->ProcessMulticastDelegate<UObject>(Parameters);
}

CSEXPORT void CSCONV Export_FMulticastScriptDelegate(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FMulticastScriptDelegate_ProcessMulticastDelegate);
}
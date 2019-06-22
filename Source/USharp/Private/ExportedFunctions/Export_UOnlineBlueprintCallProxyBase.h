CSEXPORT void CSCONV Export_UOnlineBlueprintCallProxyBase_Activate(UOnlineBlueprintCallProxyBase* instance)
{
	instance->Activate();
}

CSEXPORT void CSCONV Export_UOnlineBlueprintCallProxyBase(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UOnlineBlueprintCallProxyBase_Activate);
}
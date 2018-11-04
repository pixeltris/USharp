CSEXPORT void CSCONV Export_UEngineDelegates_Reg_OnWorldAdded(void* instance, void(*handler)(UWorld*), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(GEngine->OnWorldAdded());
}

CSEXPORT void CSCONV Export_UEngineDelegates_Reg_OnWorldDestroyed(void* instance, void(*handler)(UWorld*), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(GEngine->OnWorldDestroyed());
}

CSEXPORT void CSCONV Export_UEngineDelegates(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UEngineDelegates_Reg_OnWorldAdded);
	REGISTER_FUNC(Export_UEngineDelegates_Reg_OnWorldDestroyed);
}
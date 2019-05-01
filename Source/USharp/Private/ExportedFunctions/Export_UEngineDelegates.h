CSEXPORT void CSCONV Export_UEngineDelegates_Reg_OnWorldAdded(void* instance, void(CSCONV *handler)(UWorld*), FDelegateHandle* handle, csbool enable)
{
	REGISTER_LAMBDA(GEngine->OnWorldAdded(),
		[handler](UWorld* World)
		{
			handler(World);
		});
}

CSEXPORT void CSCONV Export_UEngineDelegates_Reg_OnWorldDestroyed(void* instance, void(CSCONV *handler)(UWorld*), FDelegateHandle* handle, csbool enable)
{
	REGISTER_LAMBDA(GEngine->OnWorldDestroyed(),
		[handler](UWorld* World)
		{
			handler(World);
		});
}

CSEXPORT void CSCONV Export_UEngineDelegates(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UEngineDelegates_Reg_OnWorldAdded);
	REGISTER_FUNC(Export_UEngineDelegates_Reg_OnWorldDestroyed);
}
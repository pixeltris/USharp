CSEXPORT void CSCONV Export_FWorldDelegates_Reg_OnWorldCleanup(void* instance, void(*handler)(UWorld*, bool, bool), FDelegateHandle* handle, csbool enable)
{
	REGISTER_LAMBDA(FWorldDelegates::OnWorldCleanup,
		[handler](UWorld* World, bool bSessionEnded, bool bCleanupResources)
		{
			handler(World, bSessionEnded, bCleanupResources);
		});
}

CSEXPORT void CSCONV Export_FWorldDelegates_Reg_OnPostWorldCleanup(void* instance, void(*handler)(UWorld*, bool, bool), FDelegateHandle* handle, csbool enable)
{
	REGISTER_LAMBDA(FWorldDelegates::OnPostWorldCleanup,
		[handler](UWorld* World, bool bSessionEnded, bool bCleanupResources)
		{
			handler(World, bSessionEnded, bCleanupResources);
		});
}

CSEXPORT void CSCONV Export_FWorldDelegates(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FWorldDelegates_Reg_OnWorldCleanup);
	REGISTER_FUNC(Export_FWorldDelegates_Reg_OnPostWorldCleanup);
}
CSEXPORT void CSCONV Export_FWorldDelegates_Reg_OnPostWorldCreation(void* instance, void(*handler)(UWorld*), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FWorldDelegates::OnPostWorldCreation);
}

CSEXPORT void CSCONV Export_FWorldDelegates_Reg_OnPreWorldInitialization(void* instance, void(*handler)(UWorld*, UWorld::InitializationValues*), FDelegateHandle* handle, csbool enable)
{
	REGISTER_LAMBDA(FWorldDelegates::OnPreWorldInitialization,
		[handler](UWorld* World, UWorld::InitializationValues IVS)
		{
			handler(World, &IVS);
		});
}

CSEXPORT void CSCONV Export_FWorldDelegates_Reg_OnPostWorldInitialization(void* instance, void(*handler)(UWorld*, UWorld::InitializationValues*), FDelegateHandle* handle, csbool enable)
{
	REGISTER_LAMBDA(FWorldDelegates::OnPostWorldInitialization,
		[handler](UWorld* World, UWorld::InitializationValues IVS)
		{
			handler(World, &IVS);
		});
}

CSEXPORT void CSCONV Export_FWorldDelegates_Reg_OnPostDuplicate(void* instance, void(*handler)(UWorld*, csbool, TMap<UObject*, UObject*>&, TArray<UObject*>&), FDelegateHandle* handle, csbool enable)
{
	REGISTER_LAMBDA(FWorldDelegates::OnPostDuplicate,
		[handler](UWorld* World, bool bDuplicateForPIE, TMap<UObject*, UObject*>& ReplacementMap, TArray<UObject*>& ObjectsToFixReferences)
		{
			handler(World, bDuplicateForPIE, ReplacementMap, ObjectsToFixReferences);
		});
}

CSEXPORT void CSCONV Export_FWorldDelegates_Reg_OnWorldCleanup(void* instance, void(*handler)(UWorld*, csbool, csbool), FDelegateHandle* handle, csbool enable)
{
	REGISTER_LAMBDA(FWorldDelegates::OnWorldCleanup,
		[handler](UWorld* World, bool bSessionEnded, bool bCleanupResources)
		{
			handler(World, bSessionEnded, bCleanupResources);
		});
}

CSEXPORT void CSCONV Export_FWorldDelegates_Reg_OnPostWorldCleanup(void* instance, void(*handler)(UWorld*, csbool, csbool), FDelegateHandle* handle, csbool enable)
{
	REGISTER_LAMBDA(FWorldDelegates::OnPostWorldCleanup,
		[handler](UWorld* World, bool bSessionEnded, bool bCleanupResources)
		{
			handler(World, bSessionEnded, bCleanupResources);
		});
}

CSEXPORT void CSCONV Export_FWorldDelegates_Reg_OnPreWorldFinishDestroy(void* instance, void(*handler)(UWorld*), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FWorldDelegates::OnPreWorldFinishDestroy);
}

CSEXPORT void CSCONV Export_FWorldDelegates(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FWorldDelegates_Reg_OnPostWorldCreation);
	REGISTER_FUNC(Export_FWorldDelegates_Reg_OnPreWorldInitialization);
	REGISTER_FUNC(Export_FWorldDelegates_Reg_OnPostWorldInitialization);
	REGISTER_FUNC(Export_FWorldDelegates_Reg_OnPostDuplicate);
	REGISTER_FUNC(Export_FWorldDelegates_Reg_OnWorldCleanup);
	REGISTER_FUNC(Export_FWorldDelegates_Reg_OnPostWorldCleanup);
	REGISTER_FUNC(Export_FWorldDelegates_Reg_OnPreWorldFinishDestroy);
}
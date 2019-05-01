#if WITH_EDITOR
CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_OnObjectPropertyChanged(void* instance, void(CSCONV *handler)(UObject*, struct FPropertyChangedEvent&), FDelegateHandle* handle, csbool enable)
{
	//REGISTER_DELEGATE(FCoreUObjectDelegates::OnObjectPropertyChanged);
	REGISTER_LAMBDA(FCoreUObjectDelegates::OnObjectPropertyChanged,
		[handler](UObject* Object, struct FPropertyChangedEvent& EditPropertyChain)
		{
			handler(Object, EditPropertyChain);
		});
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_OnPreObjectPropertyChanged(void* instance, void(CSCONV *handler)(UObject*, const class FEditPropertyChain&), FDelegateHandle* handle, csbool enable)
{
	//REGISTER_DELEGATE(FCoreUObjectDelegates::OnPreObjectPropertyChanged);
	REGISTER_LAMBDA(FCoreUObjectDelegates::OnPreObjectPropertyChanged,
		[handler](UObject* Object, const class FEditPropertyChain& EditPropertyChain)
		{
			handler(Object, EditPropertyChain);
		});
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_OnObjectModified(void* instance, void(CSCONV *handler)(UObject*), FDelegateHandle* handle, csbool enable)
{
	//REGISTER_DELEGATE(FCoreUObjectDelegates::OnObjectModified);
	REGISTER_LAMBDA(FCoreUObjectDelegates::OnObjectModified,
		[handler](UObject* Object)
		{
			handler(Object);
		});
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_OnAssetLoaded(void* instance, void(CSCONV *handler)(UObject*), FDelegateHandle* handle, csbool enable)
{
	//REGISTER_DELEGATE(FCoreUObjectDelegates::OnAssetLoaded);
	REGISTER_LAMBDA(FCoreUObjectDelegates::OnAssetLoaded,
		[handler](UObject* Asset)
		{
			handler(Asset);
		});
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_OnObjectSaved(void* instance, void(CSCONV *handler)(UObject*), FDelegateHandle* handle, csbool enable)
{
	//REGISTER_DELEGATE(FCoreUObjectDelegates::OnObjectSaved);
	REGISTER_LAMBDA(FCoreUObjectDelegates::OnObjectSaved,
		[handler](UObject* Object)
		{
			handler(Object);
		});
}
#endif

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_PreLoadMap(void* instance, void(CSCONV *handler)(const FString&), FDelegateHandle* handle, csbool enable)
{
	//REGISTER_DELEGATE(FCoreUObjectDelegates::PreLoadMap);
	REGISTER_LAMBDA(FCoreUObjectDelegates::PreLoadMap,
		[handler](const FString& MapName )
		{
			handler(MapName);
		});
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_PostLoadMapWithWorld(void* instance, void(CSCONV *handler)(UWorld*), FDelegateHandle* handle, csbool enable)
{
	//REGISTER_DELEGATE(FCoreUObjectDelegates::PostLoadMapWithWorld);
	REGISTER_LAMBDA(FCoreUObjectDelegates::PostLoadMapWithWorld,
		[handler](UWorld* World)
		{
			handler(World);
		});
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_PostDemoPlay(void* instance, void(CSCONV *handler)(), FDelegateHandle* handle, csbool enable)
{
	//REGISTER_DELEGATE(FCoreUObjectDelegates::PostDemoPlay);
	REGISTER_LAMBDA_SIMPLE(FCoreUObjectDelegates::PostDemoPlay);
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_PreGarbageCollect(void* instance, void(CSCONV *handler)(), FDelegateHandle* handle, csbool enable)
{
	//REGISTER_DELEGATE(FCoreUObjectDelegates::GetPreGarbageCollectDelegate());
	REGISTER_LAMBDA_SIMPLE(FCoreUObjectDelegates::GetPreGarbageCollectDelegate());
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_PostGarbageCollect(void* instance, void(CSCONV *handler)(), FDelegateHandle* handle, int32 enable)
{
	//REGISTER_DELEGATE(FCoreUObjectDelegates::GetPostGarbageCollect());
	REGISTER_LAMBDA_SIMPLE(FCoreUObjectDelegates::GetPostGarbageCollect());
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates(RegisterFunc registerFunc)
{
#if WITH_EDITOR
	REGISTER_FUNC(Export_FCoreUObjectDelegates_Reg_OnObjectPropertyChanged);
	REGISTER_FUNC(Export_FCoreUObjectDelegates_Reg_OnPreObjectPropertyChanged);
	REGISTER_FUNC(Export_FCoreUObjectDelegates_Reg_OnObjectModified);
	REGISTER_FUNC(Export_FCoreUObjectDelegates_Reg_OnAssetLoaded);
	REGISTER_FUNC(Export_FCoreUObjectDelegates_Reg_OnObjectSaved);
#endif
	REGISTER_FUNC(Export_FCoreUObjectDelegates_Reg_PreLoadMap);
	REGISTER_FUNC(Export_FCoreUObjectDelegates_Reg_PostLoadMapWithWorld);
	REGISTER_FUNC(Export_FCoreUObjectDelegates_Reg_PostDemoPlay);
	REGISTER_FUNC(Export_FCoreUObjectDelegates_Reg_PreGarbageCollect);
	REGISTER_FUNC(Export_FCoreUObjectDelegates_Reg_PostGarbageCollect);
}
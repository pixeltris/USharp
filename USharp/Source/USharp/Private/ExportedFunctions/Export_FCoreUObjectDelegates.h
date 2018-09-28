CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_OnObjectPropertyChanged(void(*handler)(UObject*, struct FPropertyChangedEvent&), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreUObjectDelegates::OnObjectPropertyChanged);
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_OnPreObjectPropertyChanged(void(*handler)(UObject*, const class FEditPropertyChain&), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreUObjectDelegates::OnPreObjectPropertyChanged);
}

#if WITH_EDITOR
CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_OnObjectModified(void(*handler)(UObject*), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreUObjectDelegates::OnObjectModified);
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_OnAssetLoaded(void(*handler)(UObject*), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreUObjectDelegates::OnAssetLoaded);
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_OnObjectSaved(void(*handler)(UObject*), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreUObjectDelegates::OnObjectSaved);
}
#endif

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_PreLoadMap(void(*handler)(const FString&), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreUObjectDelegates::PreLoadMap);
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_PostLoadMapWithWorld(void(*handler)(UWorld*), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreUObjectDelegates::PostLoadMapWithWorld);
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_PostDemoPlay(void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreUObjectDelegates::PostDemoPlay);
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_PreGarbageCollect(void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreUObjectDelegates::GetPreGarbageCollectDelegate());
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates_Reg_PostGarbageCollect(void(*handler)(), FDelegateHandle* handle, int32 enable)
{
	REGISTER_DELEGATE(FCoreUObjectDelegates::GetPostGarbageCollect());
}

CSEXPORT void CSCONV Export_FCoreUObjectDelegates(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FCoreUObjectDelegates_Reg_OnObjectPropertyChanged);
	REGISTER_FUNC(Export_FCoreUObjectDelegates_Reg_OnPreObjectPropertyChanged);
#if WITH_EDITOR
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
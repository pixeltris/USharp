CSEXPORT TArray<FAssetData>* CSCONV Export_FAssetRegistryModule_GetAssets(const FARFilter& Filter)
{
	TArray<FAssetData>* Assets = new TArray<FAssetData>();
	FAssetRegistryModule& AssetRegistryModule = FModuleManager::GetModuleChecked<FAssetRegistryModule>(TEXT("AssetRegistry"));
	AssetRegistryModule.Get().GetAssets(Filter, *Assets);
	return Assets;
}

CSEXPORT void CSCONV Export_FAssetRegistryModule_DeleteAssetsArray(TArray<FAssetData>* Assets)
{
	delete Assets;
}

CSEXPORT void CSCONV Export_FAssetRegistryModule(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FAssetRegistryModule_GetAssets);
	REGISTER_FUNC(Export_FAssetRegistryModule_DeleteAssetsArray);
}
#include "ModulePaths.h"

CSEXPORT void CSCONV Export_FModulePaths_FindModulePaths(const FString& NamePattern, csbool bCanUseCache, TArray<FName>& OutKeys, TArray<FString>& OutValues)
{
	TMap<FName, FString> ModulePaths;
	FModulePaths::FindModulePaths(*NamePattern, ModulePaths, !!bCanUseCache);
	for(TPair<FName, FString>& ModulePath : ModulePaths)
	{
		OutKeys.Add(ModulePath.Key);
		OutValues.Add(ModulePath.Value);
	}
}

CSEXPORT void CSCONV Export_FModulePaths(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FModulePaths_FindModulePaths);
}
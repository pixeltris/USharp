#if WITH_EDITOR
CSEXPORT void CSCONV Export_ULevel_GetLevelBlueprints(ULevel* instance, TArray<UBlueprint*>& OutLevelBlueprints)
{
	OutLevelBlueprints = instance->GetLevelBlueprints();
}
#endif

CSEXPORT void CSCONV Export_ULevel(RegisterFunc registerFunc)
{
#if WITH_EDITOR
	REGISTER_FUNC(Export_ULevel_GetLevelBlueprints);
#endif
}
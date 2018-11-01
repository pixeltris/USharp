CSEXPORT void CSCONV Export_UGameplayStatics_GetAllActorsOfClass(const UObject* WorldContextObject, UClass* ActorClass, TArray<AActor*>& OutActors)
{
	UGameplayStatics::GetAllActorsOfClass(WorldContextObject, ActorClass, OutActors);
}

CSEXPORT void CSCONV Export_UGameplayStatics(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UGameplayStatics_GetAllActorsOfClass);
}
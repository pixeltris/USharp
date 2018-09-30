CSEXPORT float CSCONV Export_AActor_GetActorTimeDilationOrDefault(UObject* WorldContextObject)
{
	AActor* Actor = Cast<AActor>(WorldContextObject);
	if (Actor != nullptr)
	{
		return Actor->GetActorTimeDilation();
	}
	
	UWorld* World = nullptr;
	if (WorldContextObject != nullptr && GEngine != nullptr)
	{
		World = GEngine->GetWorldFromContextObject(WorldContextObject, EGetWorldErrorMode::ReturnNull);
	}
	if (World == nullptr)
	{
		// Should we even do this? There can be multiple worlds and it generally isn't safe to use
		// There is also internal NUTUtil::GetPrimaryWorld() which is equally unsafe
		World = GWorld;
	}
	if (World != nullptr)
	{
		// AActor uses GetEffectiveTimeDilation(), we should too?
		return World->GetWorldSettings()->GetEffectiveTimeDilation();
		//return World->GetWorldSettings()->TimeDilation;
	}
	return 1.0f;
}

CSEXPORT void CSCONV Export_AActor(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_AActor_GetActorTimeDilationOrDefault);
}
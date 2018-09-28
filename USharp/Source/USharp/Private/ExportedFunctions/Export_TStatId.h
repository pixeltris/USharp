CSEXPORT void CSCONV Export_TStatId_GetStatDescriptionANSI(TStatId* instance, FString& result)
{
	result = instance->GetStatDescriptionANSI();
}

CSEXPORT void CSCONV Export_TStatId_GetStatDescriptionWIDE(TStatId* instance, FString& result)
{
	result = instance->GetStatDescriptionWIDE();
}

CSEXPORT void CSCONV Export_TStatId(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_TStatId_GetStatDescriptionANSI);
	REGISTER_FUNC(Export_TStatId_GetStatDescriptionWIDE);
}
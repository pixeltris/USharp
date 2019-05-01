CSEXPORT void CSCONV Export_FTicker_Reg_CoreTicker(void* instance, csbool(CSCONV *handler)(float), FDelegateHandle* handle, csbool enable, float delay)
{
	if (enable)
	{
		*handle = FTicker::GetCoreTicker().AddTicker(FTickerDelegate::CreateLambda(
			[handler](float DeltaTime) -> bool
			{
				return !!handler(DeltaTime);
			}));
	}
	else
	{
		FTicker::GetCoreTicker().RemoveTicker(*handle);
	}
}

CSEXPORT void CSCONV Export_FTicker_Tick(float DeltaTime)
{
	FTicker::GetCoreTicker().Tick(DeltaTime);
}

CSEXPORT void CSCONV Export_FTicker_AddStaticTicker(csbool(CSCONV *handler)(float), float delay)
{
	FDelegateHandle Handle;
	Export_FTicker_Reg_CoreTicker(nullptr, handler, &Handle, true, delay);
}

CSEXPORT void CSCONV Export_FTicker(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FTicker_Reg_CoreTicker);
	REGISTER_FUNC(Export_FTicker_Tick);
	REGISTER_FUNC(Export_FTicker_AddStaticTicker);
}
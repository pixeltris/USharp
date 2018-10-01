class FTickerDelegateWrapper
{
public:
	csbool(*Handler)(float);
	FDelegateHandle Handle;
	
	bool Tick(float DeltaTime)
	{
		return !!Handler(DeltaTime);
	}
	
	void CreateHandle(float Delay)
	{
		Handle = FTicker::GetCoreTicker().AddTicker(FTickerDelegate::CreateRaw(this, &FTickerDelegateWrapper::Tick), Delay);
	}
};
TArray<FTickerDelegateWrapper*> ManagedTickerDelegates;

CSEXPORT void CSCONV Export_FTicker_Reg_CoreTicker(csbool(*handler)(float), FDelegateHandle* handle, csbool enable, float delay)
{
	if (enable)
	{
		FTickerDelegateWrapper* TickerDelegate = new FTickerDelegateWrapper();
		TickerDelegate->Handler = handler;
		TickerDelegate->CreateHandle(delay);
		*handle = TickerDelegate->Handle;
		ManagedTickerDelegates.Add(TickerDelegate);
	}
	else	
	{
		bool found = false;
	
		for (int32 Index = ManagedTickerDelegates.Num() - 1; Index >= 0; --Index)
		{
			FTickerDelegateWrapper* ManagedTickerDelegate = ManagedTickerDelegates[Index];
			if(ManagedTickerDelegate->Handle == *handle)
			{
				delete ManagedTickerDelegate;
				ManagedTickerDelegates.RemoveAt(Index);
				found = true;
			}
		}
		
		if (found)
		{
			FTicker::GetCoreTicker().RemoveTicker(*handle);
		}
	}
}

CSEXPORT void CSCONV Export_FTicker_Tick(float DeltaTime)
{
	FTicker::GetCoreTicker().Tick(DeltaTime);
}

CSEXPORT void CSCONV Export_FTicker(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FTicker_Reg_CoreTicker);
	REGISTER_FUNC(Export_FTicker_Tick);
}
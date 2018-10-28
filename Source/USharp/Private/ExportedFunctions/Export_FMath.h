CSEXPORT int32 CSCONV Export_FMath_Rand()
{
	return FMath::Rand();
}

CSEXPORT void CSCONV Export_FMath_RandInit(int32 Seed)
{
	FMath::RandInit(Seed);
}

CSEXPORT float CSCONV Export_FMath_FRand()
{
	return FMath::FRand();
}

CSEXPORT void CSCONV Export_FMath_SRandInit(int32 Seed)
{
	FMath::SRandInit(Seed);
}

CSEXPORT int32 CSCONV Export_FMath_GetRandSeed()
{
	return FMath::GetRandSeed();
}

CSEXPORT float CSCONV Export_FMath_SRand()
{
	return FMath::SRand();
}

CSEXPORT csbool CSCONV Export_FMath_MemoryTest(void* BaseAddress, uint32 NumBytes)
{
	return FMath::MemoryTest(BaseAddress, NumBytes);
}

CSEXPORT csbool CSCONV Export_FMath_Eval(FString& Str, float& OutValue)
{
	return FMath::Eval(Str, OutValue);
}

CSEXPORT void CSCONV Export_FMath(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FMath_Rand);
	REGISTER_FUNC(Export_FMath_RandInit);
	REGISTER_FUNC(Export_FMath_FRand);
	REGISTER_FUNC(Export_FMath_SRandInit);
	REGISTER_FUNC(Export_FMath_GetRandSeed);
	REGISTER_FUNC(Export_FMath_SRand);
	REGISTER_FUNC(Export_FMath_MemoryTest);
	REGISTER_FUNC(Export_FMath_Eval);
}
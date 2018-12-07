CSEXPORT void CSCONV Export_FPlatformMisc_RequestExit(csbool Force)
{
	FPlatformMisc::RequestExit(!!Force);
}

CSEXPORT void CSCONV Export_FPlatformMisc(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FPlatformMisc_RequestExit);
}
CSEXPORT csbool CSCONV Export_USharpSettings_Get_bDisableExceptionNotifier()
{
	return GetMutableDefault<USharpSettings>()->bDisableExceptionNotifier;
}

CSEXPORT void CSCONV Export_USharpSettings(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_USharpSettings_Get_bDisableExceptionNotifier);
}
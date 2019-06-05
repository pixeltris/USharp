CSEXPORT UWidget* CSCONV Export_UUserWidget_GetWidgetFromName(UUserWidget* instance, const FName& Name)
{
	return instance->GetWidgetFromName(Name);
}

CSEXPORT void CSCONV Export_UUserWidget(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UUserWidget_GetWidgetFromName);
}
#if WITH_EDITOR
CSEXPORT FTimerManager& CSCONV Export_UEditorEngine_GetTimerManager(UEditorEngine* instance)
{
	return instance->GetTimerManager().Get();
}

CSEXPORT FWorldContext* CSCONV Export_UEditorEngine_GetPIEWorldContext(UEditorEngine* instance)
{
	return instance->GetPIEWorldContext();
}
#endif

CSEXPORT void CSCONV Export_UEditorEngine(RegisterFunc registerFunc)
{
#if WITH_EDITOR
	REGISTER_FUNC(Export_UEditorEngine_GetTimerManager);
	REGISTER_FUNC(Export_UEditorEngine_GetPIEWorldContext);
#endif
}
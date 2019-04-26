#if WITH_EDITOR
CSEXPORT FFeedbackContext* CSCONV Export_FFeedbackContext_GetDesktopFeedbackContext()
{
	return FDesktopPlatformModule::Get()->GetNativeFeedbackContext();
}
#endif

CSEXPORT void CSCONV Export_FFeedbackContext_BeginSlowTask(FFeedbackContext* instance, FString& Task, csbool ShowProgressDialog, csbool bShowCancelButton)
{
	instance->BeginSlowTask(FText::FromString(Task), (bool)ShowProgressDialog, (bool)bShowCancelButton);
}

CSEXPORT void CSCONV Export_FFeedbackContext_UpdateProgress(FFeedbackContext* instance, int32 Numerator, int32 Denominator)
{
	instance->UpdateProgress(Numerator, Denominator);
}

CSEXPORT void CSCONV Export_FFeedbackContext_StatusUpdate(FFeedbackContext* instance, int32 Numerator, int32 Denominator, FString& StatusText)
{
	instance->StatusUpdate(Numerator, Denominator, FText::FromString(StatusText));
}

CSEXPORT void CSCONV Export_FFeedbackContext_StatusForceUpdate(FFeedbackContext* instance, int32 Numerator, int32 Denominator, FString& StatusText)
{
	instance->StatusForceUpdate(Numerator, Denominator, FText::FromString(StatusText));
}

CSEXPORT void CSCONV Export_FFeedbackContext_EndSlowTask(FFeedbackContext* instance)
{
	instance->EndSlowTask();
}

CSEXPORT void CSCONV Export_FFeedbackContext(RegisterFunc registerFunc)
{
#if WITH_EDITOR
	REGISTER_FUNC(Export_FFeedbackContext_GetDesktopFeedbackContext);
#endif
	REGISTER_FUNC(Export_FFeedbackContext_BeginSlowTask);
	REGISTER_FUNC(Export_FFeedbackContext_UpdateProgress);
	REGISTER_FUNC(Export_FFeedbackContext_StatusUpdate);
	REGISTER_FUNC(Export_FFeedbackContext_StatusForceUpdate);
	REGISTER_FUNC(Export_FFeedbackContext_EndSlowTask);
}
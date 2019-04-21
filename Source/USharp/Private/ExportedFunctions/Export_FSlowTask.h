CSEXPORT FSlowTask* CSCONV Export_FSlowTask_New(float InAmountOfWork, const FString& InDefaultMessage, csbool bInEnabled)
{
	return new FSlowTask(InAmountOfWork, FText::FromString(InDefaultMessage), (bool)bInEnabled);
}

CSEXPORT FScopedSlowTask* CSCONV Export_FSlowTask_New_FScopedSlowTask(float InAmountOfWork, const FString& InDefaultMessage, csbool bInEnabled)
{
	return new FScopedSlowTask(InAmountOfWork, FText::FromString(InDefaultMessage), (bool)bInEnabled);
}

CSEXPORT void CSCONV Export_FSlowTask_Delete(FSlowTask* instance)
{
	delete instance;
}

CSEXPORT void CSCONV Export_FSlowTask_Get_DefaultMessageStr(FSlowTask* instance, FString& result)
{
	result = instance->DefaultMessage.ToString();
}

CSEXPORT void CSCONV Export_FSlowTask_Set_DefaultMessageStr(FSlowTask* instance, const FString& value)
{
	instance->DefaultMessage = FText::FromString(value);
}

CSEXPORT void CSCONV Export_FSlowTask_Get_FrameMessageStr(FSlowTask* instance, FString& result)
{
	result = instance->FrameMessage.ToString();
}

CSEXPORT void CSCONV Export_FSlowTask_Set_FrameMessageStr(FSlowTask* instance, const FString& value)
{
	instance->FrameMessage = FText::FromString(value);
}

CSEXPORT float CSCONV Export_FSlowTask_Get_TotalAmountOfWork(FSlowTask* instance)
{
	return instance->TotalAmountOfWork;
}

CSEXPORT void CSCONV Export_FSlowTask_Set_TotalAmountOfWork(FSlowTask* instance, float value)
{
	instance->TotalAmountOfWork = value;
}

CSEXPORT float CSCONV Export_FSlowTask_Get_CompletedWork(FSlowTask* instance)
{
	return instance->CompletedWork;
}

CSEXPORT void CSCONV Export_FSlowTask_Set_CompletedWork(FSlowTask* instance, float value)
{
	instance->CompletedWork = value;
}

CSEXPORT float CSCONV Export_FSlowTask_Get_CurrentFrameScope(FSlowTask* instance)
{
	return instance->CurrentFrameScope;
}

CSEXPORT void CSCONV Export_FSlowTask_Set_CurrentFrameScope(FSlowTask* instance, float value)
{
	instance->CurrentFrameScope = value;
}

CSEXPORT /*ESlowTaskVisibility*/int32 CSCONV Export_FSlowTask_Get_Visibility(FSlowTask* instance)
{
	return (int32)instance->Visibility;
}

CSEXPORT void CSCONV Export_FSlowTask_Set_Visibility(FSlowTask* instance, /*ESlowTaskVisibility*/int32 value)
{
	instance->Visibility = (ESlowTaskVisibility)value;
}

CSEXPORT double CSCONV Export_FSlowTask_Get_StartTime(FSlowTask* instance)
{
	return instance->StartTime;
}

CSEXPORT void CSCONV Export_FSlowTask_Set_StartTime(FSlowTask* instance, double value)
{
	instance->StartTime = value;
}

CSEXPORT float CSCONV Export_FSlowTask_Get_OpenDialogThreshold(FSlowTask* instance, csbool& hasValue)
{
	hasValue = instance->OpenDialogThreshold.IsSet();
	return instance->OpenDialogThreshold.Get(0);
}

CSEXPORT void CSCONV Export_FSlowTask_Set_OpenDialogThreshold(FSlowTask* instance, float value, csbool hasValue)
{
	if (hasValue)
	{
		instance->OpenDialogThreshold = value;
	}
	else
	{
		instance->OpenDialogThreshold.Reset();
	}
}

CSEXPORT void CSCONV Export_FSlowTask_Initialize(FSlowTask* instance)
{
	instance->Initialize();
}

CSEXPORT void CSCONV Export_FSlowTask_Destroy(FSlowTask* instance)
{
	instance->Destroy();
}

CSEXPORT void CSCONV Export_FSlowTask_MakeDialogDelayed(FSlowTask* instance, float Threshold, csbool bShowCancelButton, csbool bAllowInPIE)
{
	instance->MakeDialogDelayed(Threshold, (bool)bShowCancelButton, (bool)bAllowInPIE);
}

CSEXPORT void CSCONV Export_FSlowTask_MakeDialog(FSlowTask* instance, csbool bShowCancelButton, csbool bAllowInPIE)
{
	instance->MakeDialog((bool)bShowCancelButton, (bool)bAllowInPIE);
}

CSEXPORT void CSCONV Export_FSlowTask_EnterProgressFrame(FSlowTask* instance, float ExpectedWorkThisFrame, const FString& Text)
{
	instance->EnterProgressFrame(ExpectedWorkThisFrame, FText::FromString(Text));
}

CSEXPORT void CSCONV Export_FSlowTask_GetCurrentMessage(FSlowTask* instance, FString& result)
{
	result = instance->GetCurrentMessage().ToString();
}

CSEXPORT csbool CSCONV Export_FSlowTask_ShouldCancel(FSlowTask* instance)
{
	return instance->ShouldCancel();
}

CSEXPORT void CSCONV Export_FSlowTask(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FSlowTask_New);
	REGISTER_FUNC(Export_FSlowTask_New_FScopedSlowTask);
	REGISTER_FUNC(Export_FSlowTask_Delete);
	REGISTER_FUNC(Export_FSlowTask_Get_DefaultMessageStr);
	REGISTER_FUNC(Export_FSlowTask_Set_DefaultMessageStr);
	REGISTER_FUNC(Export_FSlowTask_Get_FrameMessageStr);
	REGISTER_FUNC(Export_FSlowTask_Set_FrameMessageStr);
	REGISTER_FUNC(Export_FSlowTask_Get_TotalAmountOfWork);
	REGISTER_FUNC(Export_FSlowTask_Set_TotalAmountOfWork);
	REGISTER_FUNC(Export_FSlowTask_Get_CompletedWork);
	REGISTER_FUNC(Export_FSlowTask_Set_CompletedWork);
	REGISTER_FUNC(Export_FSlowTask_Get_CurrentFrameScope);
	REGISTER_FUNC(Export_FSlowTask_Set_CurrentFrameScope);
	REGISTER_FUNC(Export_FSlowTask_Get_Visibility);
	REGISTER_FUNC(Export_FSlowTask_Set_Visibility);
	REGISTER_FUNC(Export_FSlowTask_Get_StartTime);
	REGISTER_FUNC(Export_FSlowTask_Set_StartTime);
	REGISTER_FUNC(Export_FSlowTask_Get_OpenDialogThreshold);
	REGISTER_FUNC(Export_FSlowTask_Set_OpenDialogThreshold);
	REGISTER_FUNC(Export_FSlowTask_Initialize);
	REGISTER_FUNC(Export_FSlowTask_Destroy);
	REGISTER_FUNC(Export_FSlowTask_MakeDialogDelayed);
	REGISTER_FUNC(Export_FSlowTask_MakeDialog);
	REGISTER_FUNC(Export_FSlowTask_EnterProgressFrame);
	REGISTER_FUNC(Export_FSlowTask_GetCurrentMessage);
	REGISTER_FUNC(Export_FSlowTask_ShouldCancel);
}
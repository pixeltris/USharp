#if WITH_EDITOR
void RegisterOnPIEEvent(FEditorDelegates::FOnPIEEvent& event, void(*handler)(csbool), FDelegateHandle* handle, csbool enable)
{
	REGISTER_LAMBDA(event,
		[handler](bool bIsSimulating)
		{
			handler(bIsSimulating);
		});
}

CSEXPORT void CSCONV Export_FEditorDelegates_Reg_PreBeginPIE(void* instance, void(*handler)(csbool), FDelegateHandle* handle, csbool enable)
{
	RegisterOnPIEEvent(FEditorDelegates::PreBeginPIE, handler, handle, enable);
}

CSEXPORT void CSCONV Export_FEditorDelegates_Reg_BeginPIE(void* instance, void(*handler)(csbool), FDelegateHandle* handle, csbool enable)
{
	RegisterOnPIEEvent(FEditorDelegates::BeginPIE, handler, handle, enable);
}

CSEXPORT void CSCONV Export_FEditorDelegates_Reg_PostPIEStarted(void* instance, void(*handler)(csbool), FDelegateHandle* handle, csbool enable)
{
	RegisterOnPIEEvent(FEditorDelegates::PostPIEStarted, handler, handle, enable);
}

CSEXPORT void CSCONV Export_FEditorDelegates_Reg_PrePIEEnded(void* instance, void(*handler)(csbool), FDelegateHandle* handle, csbool enable)
{
	RegisterOnPIEEvent(FEditorDelegates::PrePIEEnded, handler, handle, enable);
}

CSEXPORT void CSCONV Export_FEditorDelegates_Reg_EndPIE(void* instance, void(*handler)(csbool), FDelegateHandle* handle, csbool enable)
{
	RegisterOnPIEEvent(FEditorDelegates::EndPIE, handler, handle, enable);
}

CSEXPORT void CSCONV Export_FEditorDelegates_Reg_PausePIE(void* instance, void(*handler)(csbool), FDelegateHandle* handle, csbool enable)
{
	RegisterOnPIEEvent(FEditorDelegates::PausePIE, handler, handle, enable);
}

CSEXPORT void CSCONV Export_FEditorDelegates_Reg_ResumePIE(void* instance, void(*handler)(csbool), FDelegateHandle* handle, csbool enable)
{
	RegisterOnPIEEvent(FEditorDelegates::ResumePIE, handler, handle, enable);
}

CSEXPORT void CSCONV Export_FEditorDelegates_Reg_SingleStepPIE(void* instance, void(*handler)(csbool), FDelegateHandle* handle, csbool enable)
{
	RegisterOnPIEEvent(FEditorDelegates::SingleStepPIE, handler, handle, enable);
}

CSEXPORT void CSCONV Export_FEditorDelegates_Reg_OnPreSwitchBeginPIEAndSIE(void* instance, void(*handler)(csbool), FDelegateHandle* handle, csbool enable)
{
	RegisterOnPIEEvent(FEditorDelegates::OnPreSwitchBeginPIEAndSIE, handler, handle, enable);
}

CSEXPORT void CSCONV Export_FEditorDelegates_Reg_OnSwitchBeginPIEAndSIE(void* instance, void(*handler)(csbool), FDelegateHandle* handle, csbool enable)
{
	RegisterOnPIEEvent(FEditorDelegates::OnSwitchBeginPIEAndSIE, handler, handle, enable);
}
#endif

CSEXPORT void CSCONV Export_FEditorDelegates(RegisterFunc registerFunc)
{
#if WITH_EDITOR
	REGISTER_FUNC(Export_FEditorDelegates_Reg_PreBeginPIE);
	REGISTER_FUNC(Export_FEditorDelegates_Reg_BeginPIE);
	REGISTER_FUNC(Export_FEditorDelegates_Reg_PostPIEStarted);
	REGISTER_FUNC(Export_FEditorDelegates_Reg_PrePIEEnded);
	REGISTER_FUNC(Export_FEditorDelegates_Reg_EndPIE);
	REGISTER_FUNC(Export_FEditorDelegates_Reg_PausePIE);
	REGISTER_FUNC(Export_FEditorDelegates_Reg_ResumePIE);
	REGISTER_FUNC(Export_FEditorDelegates_Reg_SingleStepPIE);
	REGISTER_FUNC(Export_FEditorDelegates_Reg_OnPreSwitchBeginPIEAndSIE);
	REGISTER_FUNC(Export_FEditorDelegates_Reg_OnSwitchBeginPIEAndSIE);
#endif
}
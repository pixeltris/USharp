CSEXPORT void CSCONV Export_FCoreDelegates_Reg_OnControllerConnectionChange(void* instance, void(*handler)(csbool, int32, int32), FDelegateHandle* handle, csbool enable)
{
	// Changing the signature as we can't use bool directly due to marshaling issues
	REGISTER_LAMBDA(FCoreDelegates::OnControllerConnectionChange,
		[handler](bool Connected, int32 UserId, int32 ControllerIndex)
		{
			handler(Connected, UserId, ControllerIndex);
		});
}

CSEXPORT void CSCONV Export_FCoreDelegates_Reg_OnHandleSystemEnsure(void* instance, void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreDelegates::OnHandleSystemEnsure);
}

CSEXPORT void CSCONV Export_FCoreDelegates_Reg_OnHandleSystemError(void* instance, void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreDelegates::OnHandleSystemError);
}

CSEXPORT void CSCONV Export_FCoreDelegates_Reg_OnShutdownAfterError(void* instance, void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreDelegates::OnShutdownAfterError);
}

CSEXPORT void CSCONV Export_FCoreDelegates_Reg_OnInit(void* instance, void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreDelegates::OnInit);
}

CSEXPORT void CSCONV Export_FCoreDelegates_Reg_OnPostEngineInit(void* instance, void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreDelegates::OnPostEngineInit);
}

CSEXPORT void CSCONV Export_FCoreDelegates_Reg_OnExit(void* instance, void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreDelegates::OnExit);
}

CSEXPORT void CSCONV Export_FCoreDelegates_Reg_OnPreExit(void* instance, void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreDelegates::OnPreExit);
}

CSEXPORT void CSCONV Export_FCoreDelegates_Reg_OnBeginFrame(void* instance, void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreDelegates::OnBeginFrame);
}

CSEXPORT void CSCONV Export_FCoreDelegates_Reg_OnEndFrame(void* instance, void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreDelegates::OnEndFrame);
}

CSEXPORT void CSCONV Export_FCoreDelegates_Reg_ApplicationWillDeactivateDelegate(void* instance, void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreDelegates::ApplicationWillDeactivateDelegate);
}

CSEXPORT void CSCONV Export_FCoreDelegates_Reg_ApplicationHasReactivatedDelegate(void* instance, void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreDelegates::ApplicationHasReactivatedDelegate);
}

CSEXPORT void CSCONV Export_FCoreDelegates_Reg_ApplicationWillEnterBackgroundDelegate(void* instance, void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreDelegates::ApplicationWillEnterBackgroundDelegate);
}

CSEXPORT void CSCONV Export_FCoreDelegates_Reg_ApplicationHasEnteredForegroundDelegate(void* instance, void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreDelegates::ApplicationHasEnteredForegroundDelegate);
}

CSEXPORT void CSCONV Export_FCoreDelegates_Reg_ApplicationWillTerminateDelegate(void* instance, void(*handler)(), FDelegateHandle* handle, csbool enable)
{
	REGISTER_DELEGATE(FCoreDelegates::ApplicationWillTerminateDelegate);
}

CSEXPORT void CSCONV Export_FCoreDelegates(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FCoreDelegates_Reg_OnControllerConnectionChange);
	REGISTER_FUNC(Export_FCoreDelegates_Reg_OnHandleSystemEnsure);
	REGISTER_FUNC(Export_FCoreDelegates_Reg_OnHandleSystemError);
	REGISTER_FUNC(Export_FCoreDelegates_Reg_OnShutdownAfterError);
	REGISTER_FUNC(Export_FCoreDelegates_Reg_OnInit);
	REGISTER_FUNC(Export_FCoreDelegates_Reg_OnPostEngineInit);
	REGISTER_FUNC(Export_FCoreDelegates_Reg_OnExit);
	REGISTER_FUNC(Export_FCoreDelegates_Reg_OnPreExit);
	REGISTER_FUNC(Export_FCoreDelegates_Reg_OnBeginFrame);
	REGISTER_FUNC(Export_FCoreDelegates_Reg_OnEndFrame);
	REGISTER_FUNC(Export_FCoreDelegates_Reg_ApplicationWillDeactivateDelegate);
	REGISTER_FUNC(Export_FCoreDelegates_Reg_ApplicationHasReactivatedDelegate);
	REGISTER_FUNC(Export_FCoreDelegates_Reg_ApplicationWillEnterBackgroundDelegate);
	REGISTER_FUNC(Export_FCoreDelegates_Reg_ApplicationHasEnteredForegroundDelegate);
	REGISTER_FUNC(Export_FCoreDelegates_Reg_ApplicationWillTerminateDelegate);
}
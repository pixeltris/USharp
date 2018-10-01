CSEXPORT void CSCONV Export_FDelegateHandle_GenerateNewHandle(FDelegateHandle& result)
{
	result = FDelegateHandle(FDelegateHandle::GenerateNewHandle);
}

CSEXPORT void CSCONV Export_FDelegateHandle(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FDelegateHandle_GenerateNewHandle);
}
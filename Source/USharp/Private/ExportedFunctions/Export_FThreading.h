CSEXPORT csbool CSCONV Export_FThreading_IsInGameThread()
{
	return IsInGameThread();
}

CSEXPORT csbool CSCONV Export_FThreading_IsInSlateThread()
{
	return IsInSlateThread();
}

CSEXPORT csbool CSCONV Export_FThreading_IsInRenderingThread()
{
	return IsInRenderingThread();
}

CSEXPORT csbool CSCONV Export_FThreading_IsInParallelRenderingThread()
{
	return IsInParallelRenderingThread();
}

CSEXPORT csbool CSCONV Export_FThreading_IsInActualRenderingThread()
{
	return IsInActualRenderingThread();
}

CSEXPORT csbool CSCONV Export_FThreading_IsInAsyncLoadingThread()
{
	return IsInAsyncLoadingThread();
}

CSEXPORT csbool CSCONV Export_FThreading_IsInRHIThread()
{
	return IsInRHIThread();
}

CSEXPORT csbool CSCONV Export_FThreading_IsRenderingThreadGameThread()
{
	return GRenderingThread && GRenderingThread->GetThreadID() == GGameThreadId;
}

CSEXPORT void CSCONV Export_FThreading(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FThreading_IsInGameThread);
	REGISTER_FUNC(Export_FThreading_IsInSlateThread);
	REGISTER_FUNC(Export_FThreading_IsInRenderingThread);
	REGISTER_FUNC(Export_FThreading_IsInParallelRenderingThread);	
	REGISTER_FUNC(Export_FThreading_IsInActualRenderingThread);
	REGISTER_FUNC(Export_FThreading_IsInAsyncLoadingThread);
	REGISTER_FUNC(Export_FThreading_IsInRHIThread);
	REGISTER_FUNC(Export_FThreading_IsRenderingThreadGameThread);
}
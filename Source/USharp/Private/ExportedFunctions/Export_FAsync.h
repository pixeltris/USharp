enum class EAsyncThreadType : int32
{	
	GameThread,
	RHIThread,
	RenderingThread,
	AnyThread
};

CSEXPORT void CSCONV Export_FAsync_AsyncTask(void(*handler)(), EAsyncThreadType Thread)
{
	TFunction<void()> Function = handler;
	
	ENamedThreads::Type NamedThread = ENamedThreads::GameThread;
	switch (Thread)
	{
		case EAsyncThreadType::GameThread:
			NamedThread = ENamedThreads::GameThread;
			break;
		
		case EAsyncThreadType::RHIThread:
			NamedThread = ENamedThreads::RHIThread;
			break;
			
		case EAsyncThreadType::RenderingThread:
			NamedThread = ENamedThreads::ActualRenderingThread;
			break;
			
		case EAsyncThreadType::AnyThread:
			NamedThread = ENamedThreads::AnyThread;
			break;
	}
	
	// Do we need to use MoveTemp here?
	AsyncTask(NamedThread, MoveTemp(Function));
}

CSEXPORT void CSCONV Export_FAsync(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FAsync_AsyncTask);
}
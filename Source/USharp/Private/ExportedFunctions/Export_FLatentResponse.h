CSEXPORT FLatentResponse& CSCONV Export_FLatentResponse_DoneIf(FLatentResponse* instance, csbool Condition)
{
	return instance->DoneIf((bool)Condition);
}

CSEXPORT FLatentResponse& CSCONV Export_FLatentResponse_TriggerLink(FLatentResponse* instance, const FName& ExecutionFunction, int32 LinkID, const FWeakObjectPtr& InCallbackTarget)
{
	return instance->TriggerLink(ExecutionFunction, LinkID, InCallbackTarget);
}

CSEXPORT FLatentResponse& CSCONV Export_FLatentResponse_FinishAndTriggerIf(FLatentResponse* instance, csbool Condition, const FName& ExecutionFunction, int32 LinkID, const FWeakObjectPtr& InCallbackTarget)
{
	return instance->FinishAndTriggerIf((bool)Condition, ExecutionFunction, LinkID, InCallbackTarget);
}

CSEXPORT float CSCONV Export_FLatentResponse_ElapsedTime(FLatentResponse* instance)
{
	return instance->ElapsedTime();
}

CSEXPORT void CSCONV Export_FLatentResponse(RegisterFunc registerFunc)
{	
	REGISTER_FUNC(Export_FLatentResponse_DoneIf);
	REGISTER_FUNC(Export_FLatentResponse_TriggerLink);
	REGISTER_FUNC(Export_FLatentResponse_FinishAndTriggerIf);
	REGISTER_FUNC(Export_FLatentResponse_ElapsedTime);
}
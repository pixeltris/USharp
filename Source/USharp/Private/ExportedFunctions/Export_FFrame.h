CSEXPORT void* CSCONV Export_FFrame_GetNodeOffset()
{
	FFrame* Ptr = (FFrame*)0;
	return (void*)&Ptr->Node;
}

CSEXPORT void* CSCONV Export_FFrame_GetbArrayContextFailedOffset()
{
	FFrame* Ptr = (FFrame*)0;
	return (void*)&Ptr->bArrayContextFailed;
}

CSEXPORT SIZE_T CSCONV Export_FFrame_GetFlowStackSize()
{
	return sizeof(FlowStackType);
}

CSEXPORT void CSCONV Export_FFrame_Set_bArrayContextFailed(FFrame* instance, csbool value)
{
	instance->bArrayContextFailed = (bool)value;
}

CSEXPORT void CSCONV Export_FFrame_Step(FFrame* instance, UObject* Context, void*const Result)
{
	instance->Step(Context, Result);
}

CSEXPORT void CSCONV Export_FFrame_StepExplicitProperty(FFrame* instance, void*const Result, UProperty* Property)
{
	instance->StepExplicitProperty(Result, Property);
}

CSEXPORT int8 CSCONV Export_FFrame_ReadInt8(FFrame* instance)
{
	return instance->ReadInt<int8>();
}

CSEXPORT uint8 CSCONV Export_FFrame_ReadUInt8(FFrame* instance)
{
	return instance->ReadInt<uint8>();
}

CSEXPORT int16 CSCONV Export_FFrame_ReadInt16(FFrame* instance)
{
	return instance->ReadInt<int16>();
}

CSEXPORT uint16 CSCONV Export_FFrame_ReadUInt16(FFrame* instance)
{
	return instance->ReadInt<uint16>();
}

CSEXPORT int32 CSCONV Export_FFrame_ReadInt32(FFrame* instance)
{
	return instance->ReadInt<int32>();
}

CSEXPORT uint32 CSCONV Export_FFrame_ReadUInt32(FFrame* instance)
{
	return instance->ReadInt<uint32>();
}

CSEXPORT int64 CSCONV Export_FFrame_ReadInt64(FFrame* instance)
{
	return instance->ReadInt<int64>();
}

CSEXPORT uint64 CSCONV Export_FFrame_ReadUInt64(FFrame* instance)
{
	return instance->ReadInt<uint64>();
}

CSEXPORT float CSCONV Export_FFrame_ReadFloat(FFrame* instance)
{
	return instance->ReadFloat();
}

CSEXPORT void CSCONV Export_FFrame_ReadName(FFrame* instance, FName& result)
{
	result = instance->ReadName();
}

CSEXPORT UObject* CSCONV Export_FFrame_ReadObject(FFrame* instance)
{
	return instance->ReadObject();
}

CSEXPORT int32 CSCONV Export_FFrame_ReadCodeSkipCount(FFrame* instance)
{
	return instance->ReadCodeSkipCount();
}

CSEXPORT int32 CSCONV Export_FFrame_ReadVariableSize(FFrame* instance, UProperty** ExpressionField)
{
	return instance->ReadVariableSize(ExpressionField);
}

CSEXPORT void CSCONV Export_FFrame_GetStackTrace(FFrame* instance, FString& result)
{
	result = instance->GetStackTrace();
}

CSEXPORT void CSCONV Export_FFrame_GetScriptCallstack(FString& result)
{
	result = FFrame::GetScriptCallstack();
}

CSEXPORT void CSCONV Export_FFrame(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FFrame_GetNodeOffset);
	REGISTER_FUNC(Export_FFrame_GetbArrayContextFailedOffset);
	REGISTER_FUNC(Export_FFrame_GetFlowStackSize);
	REGISTER_FUNC(Export_FFrame_Set_bArrayContextFailed);
	REGISTER_FUNC(Export_FFrame_Step);
	REGISTER_FUNC(Export_FFrame_StepExplicitProperty);
	REGISTER_FUNC(Export_FFrame_ReadInt8);
	REGISTER_FUNC(Export_FFrame_ReadUInt8);
	REGISTER_FUNC(Export_FFrame_ReadInt16);
	REGISTER_FUNC(Export_FFrame_ReadUInt16);
	REGISTER_FUNC(Export_FFrame_ReadInt32);
	REGISTER_FUNC(Export_FFrame_ReadUInt32);
	REGISTER_FUNC(Export_FFrame_ReadInt64);
	REGISTER_FUNC(Export_FFrame_ReadUInt64);
	REGISTER_FUNC(Export_FFrame_ReadFloat);
	REGISTER_FUNC(Export_FFrame_ReadName);
	REGISTER_FUNC(Export_FFrame_ReadObject);
	REGISTER_FUNC(Export_FFrame_ReadCodeSkipCount);
	REGISTER_FUNC(Export_FFrame_ReadVariableSize);
	REGISTER_FUNC(Export_FFrame_GetStackTrace);
	REGISTER_FUNC(Export_FFrame_GetScriptCallstack);
}
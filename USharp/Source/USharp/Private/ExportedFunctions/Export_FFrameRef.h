// This is just a copy of Export_FFrame used so that struct can be passed in as a param
// without having pin the struct

CSEXPORT void CSCONV Export_FFrameRef_Step(FFrame* instance, UObject* Context, void*const Result)
{
	instance->Step(Context, Result);
}

CSEXPORT void CSCONV Export_FFrameRef_StepExplicitProperty(FFrame* instance, void*const Result, UProperty* Property)
{
	instance->StepExplicitProperty(Result, Property);
}

CSEXPORT int8 CSCONV Export_FFrameRef_ReadInt8(FFrame* instance)
{
	return instance->ReadInt<int8>();
}

CSEXPORT uint8 CSCONV Export_FFrameRef_ReadUInt8(FFrame* instance)
{
	return instance->ReadInt<uint8>();
}

CSEXPORT int16 CSCONV Export_FFrameRef_ReadInt16(FFrame* instance)
{
	return instance->ReadInt<int16>();
}

CSEXPORT uint16 CSCONV Export_FFrameRef_ReadUInt16(FFrame* instance)
{
	return instance->ReadInt<uint16>();
}

CSEXPORT int32 CSCONV Export_FFrameRef_ReadInt32(FFrame* instance)
{
	return instance->ReadInt<int32>();
}

CSEXPORT uint32 CSCONV Export_FFrameRef_ReadUInt32(FFrame* instance)
{
	return instance->ReadInt<uint32>();
}

CSEXPORT int64 CSCONV Export_FFrameRef_ReadInt64(FFrame* instance)
{
	return instance->ReadInt<int64>();
}

CSEXPORT uint64 CSCONV Export_FFrameRef_ReadUInt64(FFrame* instance)
{
	return instance->ReadInt<uint64>();
}

CSEXPORT float CSCONV Export_FFrameRef_ReadFloat(FFrame* instance)
{
	return instance->ReadFloat();
}

CSEXPORT void CSCONV Export_FFrameRef_ReadName(FFrame* instance, FName& result)
{
	result = instance->ReadName();
}

CSEXPORT UObject* CSCONV Export_FFrameRef_ReadObject(FFrame* instance)
{
	return instance->ReadObject();
}

CSEXPORT int32 CSCONV Export_FFrameRef_ReadCodeSkipCount(FFrame* instance)
{
	return instance->ReadCodeSkipCount();
}

CSEXPORT int32 CSCONV Export_FFrameRef_ReadVariableSize(FFrame* instance, UProperty** ExpressionField)
{
	return instance->ReadVariableSize(ExpressionField);
}

CSEXPORT void CSCONV Export_FFrameRef_GetStackTrace(FFrame* instance, FString& result)
{
	result = instance->GetStackTrace();
}

CSEXPORT void CSCONV Export_FFrameRef_GetScriptCallstack(FString& result)
{
	result = FFrame::GetScriptCallstack();
}

CSEXPORT void CSCONV Export_FFrameRef(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FFrameRef_Step);
	REGISTER_FUNC(Export_FFrameRef_StepExplicitProperty);
	REGISTER_FUNC(Export_FFrameRef_ReadInt8);
	REGISTER_FUNC(Export_FFrameRef_ReadUInt8);
	REGISTER_FUNC(Export_FFrameRef_ReadInt16);
	REGISTER_FUNC(Export_FFrameRef_ReadUInt16);
	REGISTER_FUNC(Export_FFrameRef_ReadInt32);
	REGISTER_FUNC(Export_FFrameRef_ReadUInt32);
	REGISTER_FUNC(Export_FFrameRef_ReadInt64);
	REGISTER_FUNC(Export_FFrameRef_ReadUInt64);
	REGISTER_FUNC(Export_FFrameRef_ReadFloat);
	REGISTER_FUNC(Export_FFrameRef_ReadName);
	REGISTER_FUNC(Export_FFrameRef_ReadObject);
	REGISTER_FUNC(Export_FFrameRef_ReadCodeSkipCount);
	REGISTER_FUNC(Export_FFrameRef_ReadVariableSize);
	REGISTER_FUNC(Export_FFrameRef_GetStackTrace);
	REGISTER_FUNC(Export_FFrameRef_GetScriptCallstack);
}
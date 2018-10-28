CSEXPORT csbool CSCONV Export_FParse_Command(const FString& Stream, const FString& Match, csbool bParseMightTriggerExecution, FString& StreamResult)
{
	const TCHAR* Ptr = *Stream;
	csbool Success = FParse::Command(&Ptr, *Match, !!bParseMightTriggerExecution);
	StreamResult = Ptr;
	return Success;
}

CSEXPORT csbool CSCONV Export_FParse_Value_Name(const FString& Stream, const FString& Match, FName& Value)
{
	return FParse::Value(*Stream, *Match, Value);
}

CSEXPORT csbool CSCONV Export_FParse_Value_UInt32(const FString& Stream, const FString& Match, uint32& Value)
{
	return FParse::Value(*Stream, *Match, Value);
}

CSEXPORT csbool CSCONV Export_FParse_Value_Guid(const FString& Stream, const FString& Match, FGuid& Value)
{
	return FParse::Value(*Stream, *Match, Value);
}

CSEXPORT csbool CSCONV Export_FParse_Value_Byte(const FString& Stream, const FString& Match, uint8& Value)
{
	return FParse::Value(*Stream, *Match, Value);
}

CSEXPORT csbool CSCONV Export_FParse_Value_SByte(const FString& Stream, const FString& Match, int8& Value)
{
	return FParse::Value(*Stream, *Match, Value);
}

CSEXPORT csbool CSCONV Export_FParse_Value_UInt16(const FString& Stream, const FString& Match, uint16& Value)
{
	return FParse::Value(*Stream, *Match, Value);
}

CSEXPORT csbool CSCONV Export_FParse_Value_Int16(const FString& Stream, const FString& Match, int16& Value)
{
	return FParse::Value(*Stream, *Match, Value);
}

CSEXPORT csbool CSCONV Export_FParse_Value_Float(const FString& Stream, const FString& Match, float& Value)
{
	return FParse::Value(*Stream, *Match, Value);
}

CSEXPORT csbool CSCONV Export_FParse_Value_Int32(const FString& Stream, const FString& Match, int32& Value)
{
	return FParse::Value(*Stream, *Match, Value);
}

CSEXPORT csbool CSCONV Export_FParse_Value_Str(const FString& Stream, const FString& Match, FString& Value, csbool bShouldStopOnSeparator)
{
	return FParse::Value(*Stream, *Match, Value, !!bShouldStopOnSeparator);
}

CSEXPORT csbool CSCONV Export_FParse_Value_Int64(const FString& Stream, const FString& Match, int64& Value)
{
	return FParse::Value(*Stream, *Match, Value);
}

CSEXPORT csbool CSCONV Export_FParse_Value_UInt64(const FString& Stream, const FString& Match, uint64& Value)
{
	return FParse::Value(*Stream, *Match, Value);
}

CSEXPORT csbool CSCONV Export_FParse_Bool(const FString& Stream, const FString& Match, csbool& Value)
{
	bool BoolResult;
	csbool Success = FParse::Bool(*Stream, *Match, BoolResult);
	Value = BoolResult;
	return Success;
}

CSEXPORT csbool CSCONV Export_FParse_Line(const FString& Stream, FString& Result, csbool Exact, FString& StreamResult)
{
	const TCHAR* Ptr = *Stream;
	csbool Success = FParse::Line(&Ptr, Result, !!Exact);
	StreamResult = Ptr;
	return Success;
}

CSEXPORT csbool CSCONV Export_FParse_LineExtended(const FString& Stream, FString& Result, int32& LinesConsumed, csbool Exact, FString& StreamResult)
{
	const TCHAR* Ptr = *Stream;
	csbool Success = FParse::LineExtended(&Ptr, Result, LinesConsumed, !!Exact);
	StreamResult = Ptr;
	return Success;
}

CSEXPORT csbool CSCONV Export_FParse_Token(const FString& Stream, FString& Arg, csbool UseEscape, FString& StreamResult)
{
	const TCHAR* Ptr = *Stream;
	csbool Success = FParse::Token(Ptr, Arg, !!UseEscape);
	StreamResult = Ptr;
	return Success;
}

CSEXPORT csbool CSCONV Export_FParse_AlnumToken(const FString& Stream, FString& Arg, FString& StreamResult)
{
	const TCHAR* Ptr = *Stream;
	csbool Success = FParse::AlnumToken(Ptr, Arg);
	StreamResult = Ptr;
	return Success;
}

CSEXPORT void CSCONV Export_FParse_Next(const FString& Stream, FString& StreamResult)
{
	const TCHAR* Ptr = *Stream;
	FParse::Next(&Ptr);
	StreamResult = Ptr;
}

CSEXPORT csbool CSCONV Export_FParse_Param(const FString& Stream, const FString& Param)
{
	return FParse::Param(*Stream, *Param);
}

CSEXPORT csbool CSCONV Export_FParse_QuotedString(const FString& Stream, FString& Value, int32& OutNumCharsRead)
{
	return FParse::QuotedString(*Stream, Value, &OutNumCharsRead);
}

CSEXPORT void CSCONV Export_FParse(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FParse_Command);
	REGISTER_FUNC(Export_FParse_Value_Name);
	REGISTER_FUNC(Export_FParse_Value_UInt32);
	REGISTER_FUNC(Export_FParse_Value_Guid);
	REGISTER_FUNC(Export_FParse_Value_Byte);
	REGISTER_FUNC(Export_FParse_Value_SByte);
	REGISTER_FUNC(Export_FParse_Value_UInt16);
	REGISTER_FUNC(Export_FParse_Value_Int16);
	REGISTER_FUNC(Export_FParse_Value_Float);
	REGISTER_FUNC(Export_FParse_Value_Int32);
	REGISTER_FUNC(Export_FParse_Value_Str);
	REGISTER_FUNC(Export_FParse_Value_Int64);
	REGISTER_FUNC(Export_FParse_Value_UInt64);
	REGISTER_FUNC(Export_FParse_Bool);
	REGISTER_FUNC(Export_FParse_Line);
	REGISTER_FUNC(Export_FParse_LineExtended);
	REGISTER_FUNC(Export_FParse_Token);
	REGISTER_FUNC(Export_FParse_AlnumToken);
	REGISTER_FUNC(Export_FParse_Next);
	REGISTER_FUNC(Export_FParse_Param);
	REGISTER_FUNC(Export_FParse_QuotedString);
}
CSEXPORT EFunctionFlags CSCONV Export_UFunction_Get_FunctionFlags(UFunction* instance)
{
	return instance->FunctionFlags;
}

CSEXPORT void CSCONV Export_UFunction_Set_FunctionFlags(UFunction* instance, EFunctionFlags value)
{
	instance->FunctionFlags = value;
}

CSEXPORT uint8 CSCONV Export_UFunction_Get_NumParms(UFunction* instance)
{
	return instance->NumParms;
}

CSEXPORT void CSCONV Export_UFunction_Set_NumParms(UFunction* instance, uint8 value)
{
	instance->NumParms = value;
}

CSEXPORT uint16 CSCONV Export_UFunction_Get_ParmsSize(UFunction* instance)
{
	return instance->ParmsSize;
}

CSEXPORT void CSCONV Export_UFunction_Set_ParmsSize(UFunction* instance, uint16 value)
{
	instance->ParmsSize = value;
}

CSEXPORT uint16 CSCONV Export_UFunction_Get_ReturnValueOffset(UFunction* instance)
{
	return instance->ReturnValueOffset;
}

CSEXPORT void CSCONV Export_UFunction_Set_ReturnValueOffset(UFunction* instance, int16 value)
{
	instance->ReturnValueOffset = value;
}

CSEXPORT uint16 CSCONV Export_UFunction_Get_RPCId(UFunction* instance)
{
	return instance->RPCId;
}

CSEXPORT void CSCONV Export_UFunction_Set_RPCId(UFunction* instance, uint16 value)
{
	instance->RPCId = value;
}

CSEXPORT uint16 CSCONV Export_UFunction_Get_RPCResponseId(UFunction* instance)
{
	return instance->RPCResponseId;
}

CSEXPORT void CSCONV Export_UFunction_Set_RPCResponseId(UFunction* instance, uint16 value)
{
	instance->RPCResponseId = value;
}

CSEXPORT UProperty* CSCONV Export_UFunction_Get_FirstPropertyToInit(UFunction* instance)
{
	return instance->FirstPropertyToInit;
}

CSEXPORT void CSCONV Export_UFunction_Set_FirstPropertyToInit(UFunction* instance, UProperty* value)
{
	instance->FirstPropertyToInit = value;
}

CSEXPORT FNativeFuncPtr CSCONV Export_UFunction_GetNativeFunc(UFunction* instance)
{
	return instance->GetNativeFunc();
}

CSEXPORT void CSCONV Export_UFunction_SetNativeFunc(UFunction* instance, FNativeFuncPtr InFunc)
{
	instance->SetNativeFunc(InFunc);
}

CSEXPORT void CSCONV Export_UFunction_Invoke(UFunction* instance, UObject* Obj, FFrame& Stack, RESULT_DECL)
{
	instance->Invoke(Obj, Stack, RESULT_PARAM);
}

CSEXPORT void CSCONV Export_UFunction_InitializeDerivedMembers(UFunction* instance)
{
	instance->InitializeDerivedMembers();
}

CSEXPORT UFunction* CSCONV Export_UFunction_GetSuperFunction(UFunction* instance)
{
	return instance->GetSuperFunction();
}

CSEXPORT UProperty* CSCONV Export_UFunction_GetReturnProperty(UFunction* instance)
{
	return instance->GetReturnProperty();
}

CSEXPORT csbool CSCONV Export_UFunction_HasAnyFunctionFlags(UFunction* instance, EFunctionFlags FlagsToCheck)
{
	return instance->HasAnyFunctionFlags(FlagsToCheck);
}

CSEXPORT csbool CSCONV Export_UFunction_HasAllFunctionFlags(UFunction* instance, EFunctionFlags FlagsToCheck)
{
	return instance->HasAllFunctionFlags(FlagsToCheck);
}

CSEXPORT uint64 CSCONV Export_UFunction_GetDefaultIgnoredSignatureCompatibilityFlags()
{
	return UFunction::GetDefaultIgnoredSignatureCompatibilityFlags();
}

CSEXPORT csbool CSCONV Export_UFunction_IsSignatureCompatibleWith(UFunction* instance, const UFunction* OtherFunction)
{
	return instance->IsSignatureCompatibleWith(OtherFunction);
}

CSEXPORT csbool CSCONV Export_UFunction_IsSignatureCompatibleWithFlags(UFunction* instance, const UFunction* OtherFunction, uint64 IgnoreFlags)
{
	return instance->IsSignatureCompatibleWith(OtherFunction, IgnoreFlags);
}

CSEXPORT void CSCONV Export_UFunction(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UFunction_Get_FunctionFlags);
	REGISTER_FUNC(Export_UFunction_Set_FunctionFlags);
	REGISTER_FUNC(Export_UFunction_Get_NumParms);
	REGISTER_FUNC(Export_UFunction_Set_NumParms);
	REGISTER_FUNC(Export_UFunction_Get_ParmsSize);
	REGISTER_FUNC(Export_UFunction_Set_ParmsSize);
	REGISTER_FUNC(Export_UFunction_Get_ReturnValueOffset);
	REGISTER_FUNC(Export_UFunction_Set_ReturnValueOffset);
	REGISTER_FUNC(Export_UFunction_Get_RPCId);
	REGISTER_FUNC(Export_UFunction_Set_RPCId);
	REGISTER_FUNC(Export_UFunction_Get_RPCResponseId);
	REGISTER_FUNC(Export_UFunction_Set_RPCResponseId);
	REGISTER_FUNC(Export_UFunction_Get_FirstPropertyToInit);
	REGISTER_FUNC(Export_UFunction_Set_FirstPropertyToInit);
	REGISTER_FUNC(Export_UFunction_GetNativeFunc);
	REGISTER_FUNC(Export_UFunction_SetNativeFunc);
	REGISTER_FUNC(Export_UFunction_Invoke);
	REGISTER_FUNC(Export_UFunction_InitializeDerivedMembers);
	REGISTER_FUNC(Export_UFunction_GetSuperFunction);
	REGISTER_FUNC(Export_UFunction_GetReturnProperty);
	REGISTER_FUNC(Export_UFunction_HasAnyFunctionFlags);
	REGISTER_FUNC(Export_UFunction_HasAllFunctionFlags);
	REGISTER_FUNC(Export_UFunction_GetDefaultIgnoredSignatureCompatibilityFlags);
	REGISTER_FUNC(Export_UFunction_IsSignatureCompatibleWith);
	REGISTER_FUNC(Export_UFunction_IsSignatureCompatibleWithFlags);
}
#if WITH_EDITORONLY_DATA
CSEXPORT uint8 CSCONV Export_UUserDefinedStruct_Get_Status(UUserDefinedStruct* Instance)
{
	return Instance->Status;
}

CSEXPORT void CSCONV Export_UUserDefinedStruct_Set_Status(UUserDefinedStruct* Instance, uint8 Status)
{
	Instance->Status = (TEnumAsByte<EUserDefinedStructureStatus>)Status;
}

CSEXPORT void CSCONV Export_UUserDefinedStruct_Get_ErrorMessage(UUserDefinedStruct* Instance, FString& result)
{
	result = Instance->ErrorMessage;
}

CSEXPORT void CSCONV Export_UUserDefinedStruct_Set_ErrorMessage(UUserDefinedStruct* Instance, const FString& ErrorMessage)
{
	Instance->ErrorMessage = ErrorMessage;
}

CSEXPORT UObject* CSCONV Export_UUserDefinedStruct_Get_EditorData(UUserDefinedStruct* Instance)
{
	return Instance->EditorData;
}

CSEXPORT void CSCONV Export_UUserDefinedStruct_Set_EditorData(UUserDefinedStruct* Instance, UObject* EditorData)
{
	Instance->EditorData = EditorData;
}
#endif

CSEXPORT FGuid CSCONV Export_UUserDefinedStruct_Get_Guid(UUserDefinedStruct* Instance)
{
	return Instance->Guid;
}

CSEXPORT void CSCONV Export_UUserDefinedStruct_Set_Guid(UUserDefinedStruct* Instance, const FGuid& Guid)
{
	Instance->Guid = Guid;
}

CSEXPORT void CSCONV Export_UUserDefinedStruct(RegisterFunc registerFunc)
{
#if WITH_EDITORONLY_DATA
	REGISTER_FUNC(Export_UUserDefinedStruct_Get_Status);
	REGISTER_FUNC(Export_UUserDefinedStruct_Set_Status);
	REGISTER_FUNC(Export_UUserDefinedStruct_Get_ErrorMessage);
	REGISTER_FUNC(Export_UUserDefinedStruct_Set_ErrorMessage);
	REGISTER_FUNC(Export_UUserDefinedStruct_Get_EditorData);
	REGISTER_FUNC(Export_UUserDefinedStruct_Set_EditorData);
#endif
	REGISTER_FUNC(Export_UUserDefinedStruct_Get_Guid);
	REGISTER_FUNC(Export_UUserDefinedStruct_Set_Guid);
}
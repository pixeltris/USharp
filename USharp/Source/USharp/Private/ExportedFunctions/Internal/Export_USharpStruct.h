CSEXPORT void CSCONV Export_USharpStruct_CreateGuid(USharpStruct* instance)
{
	instance->CreateGuid();
}

CSEXPORT void CSCONV Export_USharpStruct(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_USharpStruct_CreateGuid);
}
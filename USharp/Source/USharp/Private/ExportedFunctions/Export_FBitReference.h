CSEXPORT csbool CSCONV Export_FBitReference_Get(FBitReference& instance)
{
	bool Result = instance;
	return Result;
}

CSEXPORT void CSCONV Export_FBitReference_Set(FBitReference& instance, csbool Value)
{
	instance = !!Value;
}

CSEXPORT void CSCONV Export_FBitReference_AtomicSet(FBitReference* instance, csbool Value)
{
	instance->AtomicSet(!!Value);
}

CSEXPORT void CSCONV Export_FBitReference(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FBitReference_Get);
	REGISTER_FUNC(Export_FBitReference_Set);
	REGISTER_FUNC(Export_FBitReference_AtomicSet);
}
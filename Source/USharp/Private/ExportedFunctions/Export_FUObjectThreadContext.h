CSEXPORT void CSCONV Export_FUObjectThreadContext_PopInitializer()
{
	FUObjectThreadContext::Get().PopInitializer();
}

CSEXPORT void CSCONV Export_FUObjectThreadContext_PushInitializer(FObjectInitializer* Initializer)
{
	FUObjectThreadContext::Get().PushInitializer(Initializer);
}

CSEXPORT FObjectInitializer* CSCONV Export_FUObjectThreadContext_TopInitializer()
{
	return FUObjectThreadContext::Get().TopInitializer();
}

CSEXPORT int32 CSCONV Export_FUObjectThreadContext_Get_IsInConstructor()
{
	return FUObjectThreadContext::Get().IsInConstructor;
}

CSEXPORT UObject* CSCONV Export_FUObjectThreadContext_Get_ConstructedObject()
{
	return FUObjectThreadContext::Get().ConstructedObject;
}

CSEXPORT UObject* CSCONV Export_FUObjectThreadContext_Get_SerializedObject()
{
	return FUObjectThreadContext::Get().GetSerializeContext()->SerializedObject;
}

CSEXPORT void CSCONV Export_FUObjectThreadContext(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FUObjectThreadContext_PopInitializer);
	REGISTER_FUNC(Export_FUObjectThreadContext_PushInitializer);
	REGISTER_FUNC(Export_FUObjectThreadContext_TopInitializer);
	REGISTER_FUNC(Export_FUObjectThreadContext_Get_IsInConstructor);
	REGISTER_FUNC(Export_FUObjectThreadContext_Get_ConstructedObject);
	REGISTER_FUNC(Export_FUObjectThreadContext_Get_SerializedObject);
}
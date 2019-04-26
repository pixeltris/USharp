CSEXPORT int32 CSCONV Export_UEngineTypes_ConvertToCollisionChannelFromTrace(int32 TraceType)
{
	return (int32)UEngineTypes::ConvertToCollisionChannel((ETraceTypeQuery)TraceType);
}

CSEXPORT int32 CSCONV Export_UEngineTypes_ConvertToCollisionChannelFromObject(int32 ObjectType)
{
	return (int32)UEngineTypes::ConvertToCollisionChannel((EObjectTypeQuery)ObjectType);
}

CSEXPORT int32 CSCONV Export_UEngineTypes_ConvertToObjectType(int32 CollisionChannel)
{
	return (int32)UEngineTypes::ConvertToObjectType((ECollisionChannel)CollisionChannel);
}

CSEXPORT int32 CSCONV Export_UEngineTypes_ConvertToTraceType(int32 CollisionChannel)
{
	return (int32)UEngineTypes::ConvertToTraceType((ECollisionChannel)CollisionChannel);
}

CSEXPORT void CSCONV Export_UEngineTypes(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UEngineTypes_ConvertToCollisionChannelFromTrace);
	REGISTER_FUNC(Export_UEngineTypes_ConvertToCollisionChannelFromObject);
	REGISTER_FUNC(Export_UEngineTypes_ConvertToObjectType);
	REGISTER_FUNC(Export_UEngineTypes_ConvertToTraceType);
}
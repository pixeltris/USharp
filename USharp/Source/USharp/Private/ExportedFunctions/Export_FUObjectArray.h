CSEXPORT FUObjectArray* CSCONV Export_FUObjectArray_GetGUObjectArray()
{
	return &GUObjectArray;
}

CSEXPORT int32 CSCONV Export_FUObjectArray_GetObjectArrayNum(FUObjectArray* instance)
{
	return instance->GetObjectArrayNum();
}

CSEXPORT UObject* CSCONV Export_FUObjectArray_GetObjectAtIndex(FUObjectArray* instance, int32 Index)
{
	FUObjectItem* ObjectItem = instance->IndexToObject(Index);
	if(ObjectItem != nullptr)
	{
		return (UObject*)ObjectItem->Object;
	}
	return nullptr;
}

CSEXPORT void CSCONV Export_FUObjectArray(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FUObjectArray_GetGUObjectArray);
	REGISTER_FUNC(Export_FUObjectArray_GetObjectArrayNum);
	REGISTER_FUNC(Export_FUObjectArray_GetObjectAtIndex);
}
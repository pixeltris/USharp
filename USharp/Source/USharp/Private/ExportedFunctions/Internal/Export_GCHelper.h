typedef void* GCHandle;
TMap<TWeakObjectPtr<UObject>, GCHandle> ManagedObjectMap;

GCHandle(*OnAdd)(UObject*) = nullptr;
void(*OnAddExisting)(UObject*, GCHandle) = nullptr;
void(*OnRemove)(GCHandle) = nullptr;

CSEXPORT void CSCONV Export_GCHelper_Set_OnAdd(GCHandle(*handler)(UObject*))
{
	OnAdd = handler;
}

CSEXPORT void CSCONV Export_GCHelper_Set_OnAddExisting(void(*handler)(UObject*, GCHandle))
{
	OnAddExisting = handler;
}

CSEXPORT void CSCONV Export_GCHelper_Set_OnRemove(void(*handler)(GCHandle))
{
	OnRemove = handler;
}

CSEXPORT GCHandle CSCONV Export_GCHelper_Add(UObject* obj)
{
	GCHandle gcHandle = nullptr;
	if (ManagedObjectMap.Find(obj) == nullptr)
	{
		gcHandle = OnAdd(obj);
		if (gcHandle != nullptr)
		{
			ManagedObjectMap.Add(obj, gcHandle);
		}
	}
	return gcHandle;
}

CSEXPORT void CSCONV Export_GCHelper_AddExisting(UObject* obj, GCHandle gcHandle)
{
	if (gcHandle != nullptr && ManagedObjectMap.Find(obj) == nullptr)
	{
		ManagedObjectMap.Add(obj, gcHandle);
		OnAddExisting(obj, gcHandle);
	}
}

CSEXPORT void CSCONV Export_GCHelper_Remove(UObject* obj)
{
	GCHandle* gcHandle = ManagedObjectMap.Find(obj);
	if (gcHandle != nullptr)
	{
		int32 removed = ManagedObjectMap.Remove(obj);
		check(removed == 1);
		OnRemove(*gcHandle);
	}
}

CSEXPORT void CSCONV Export_GCHelper_CollectGarbage()
{
	for (auto it = ManagedObjectMap.CreateIterator(); it; ++it)
	{
		if (!it.Key().IsValid())
		{
			OnRemove(it.Value());
			it.RemoveCurrent();			
		}
	}
}

CSEXPORT void CSCONV Export_GCHelper_Clear()
{
	OnAdd = nullptr;
	OnAddExisting = nullptr;
	OnRemove = nullptr;
	ManagedObjectMap.Empty();
}

CSEXPORT void CSCONV Export_GCHelper(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_GCHelper_Set_OnAdd);
	REGISTER_FUNC(Export_GCHelper_Set_OnAddExisting);
	REGISTER_FUNC(Export_GCHelper_Set_OnRemove);
	REGISTER_FUNC(Export_GCHelper_Add);
	REGISTER_FUNC(Export_GCHelper_AddExisting);
	REGISTER_FUNC(Export_GCHelper_Remove);
	REGISTER_FUNC(Export_GCHelper_CollectGarbage);
	REGISTER_FUNC(Export_GCHelper_Clear);
}
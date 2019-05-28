typedef void* GCHandle;
struct FManagedObject
{
	GCHandle Handle;
	TWeakObjectPtr<UObject> WeakPtr;
	
	FManagedObject(GCHandle InHandle, UObject* InPtr)
		: Handle(InHandle)
		, WeakPtr(InPtr)
	{}
	
	bool IsValid()
	{
		return WeakPtr.IsValid();
	}
};
// NOTE: What happens if a UObject address is reused? Is this protected by the GC events?
TMap<UObject*, FManagedObject> ManagedObjectMap;

GCHandle(CSCONV *OnAdd)(UObject*) = nullptr;
void(CSCONV *OnRemove)(GCHandle) = nullptr;

CSEXPORT void CSCONV Export_GCHelper_Set_OnAdd(GCHandle(CSCONV *handler)(UObject*))
{
	OnAdd = handler;
}

CSEXPORT void CSCONV Export_GCHelper_Set_OnRemove(void(CSCONV *handler)(GCHandle))
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
			ManagedObjectMap.Add(obj, FManagedObject(gcHandle, obj));
		}
	}
	return gcHandle;
}

CSEXPORT void CSCONV Export_GCHelper_Remove(UObject* obj)
{
	FManagedObject* managedObject = ManagedObjectMap.Find(obj);
	if (managedObject != nullptr)
	{
		int32 removed = ManagedObjectMap.Remove(obj);
		check(removed == 1);
		OnRemove(managedObject->Handle);
	}
}

CSEXPORT void CSCONV Export_GCHelper_CollectGarbage()
{
	for (auto it = ManagedObjectMap.CreateIterator(); it; ++it)
	{
		if (!it.Value().IsValid())
		{
			OnRemove(it.Value().Handle);
			it.RemoveCurrent();
		}
	}
}

CSEXPORT void CSCONV Export_GCHelper_Clear()
{
	OnAdd = nullptr;
	OnRemove = nullptr;
	ManagedObjectMap.Empty();
}

// Maps to the UObjectBase internals
class UObjectInternal
{
public:
	virtual void PureVirt() = 0;// Make sure we have a vtable so that we map 1:1 with UObject
	EObjectFlags ObjectFlags;
	int32 InternalIndex;
	UClass* ClassPrivate;
	FName NamePrivate;
	UObject* OuterPrivate;
	// mutable TStatId - based on STATS || ENABLE_STATNAMEDEVENTS_UOBJECT
	// mutable PROFILE_CHAR* - based on ENABLE_STATNAMEDEVENTS_UOBJECT
};

CSEXPORT int32 CSCONV Export_GCHelper_GetInternalIndexOffset()
{
	UObject* Obj = UClass::StaticClass()->GetDefaultObject();
	check(Obj->GetUniqueID() == ((UObjectInternal*)Obj)->InternalIndex && Obj->GetUniqueID() != 0);
	UObjectInternal* NullObj = (UObjectInternal*)nullptr;
	return (int32)(int64)(&NullObj->InternalIndex);
}

CSEXPORT void CSCONV Export_GCHelper(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_GCHelper_Set_OnAdd);
	REGISTER_FUNC(Export_GCHelper_Set_OnRemove);
	REGISTER_FUNC(Export_GCHelper_Add);
	REGISTER_FUNC(Export_GCHelper_Remove);
	REGISTER_FUNC(Export_GCHelper_CollectGarbage);
	REGISTER_FUNC(Export_GCHelper_Clear);
	REGISTER_FUNC(Export_GCHelper_GetInternalIndexOffset);
}
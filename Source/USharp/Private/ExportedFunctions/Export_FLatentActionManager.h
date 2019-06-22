CSEXPORT void CSCONV Export_FLatentActionManager_ProcessLatentActions(FLatentActionManager* instance, UObject* InObject, float DeltaTime)
{
	instance->ProcessLatentActions(InObject, DeltaTime);
}

CSEXPORT void* CSCONV Export_FLatentActionManager_FindExistingActionUSharp(FLatentActionManager* instance, UObject* InActionObject, int32 UUID)
{
	FUSharpLatentAction* Action = instance->FindExistingAction<FUSharpLatentAction>(InActionObject, UUID);
	if (Action)
	{
		return Action->ManagedObject;
	}
	return nullptr;
}

CSEXPORT FPendingLatentAction* CSCONV Export_FLatentActionManager_FindExistingAction(FLatentActionManager* instance, UObject* InActionObject, int32 UUID)
{
	return instance->FindExistingAction<FPendingLatentAction>(InActionObject, UUID);
}

CSEXPORT void CSCONV Export_FLatentActionManager_RemoveActionsForObject(FLatentActionManager* instance, const TWeakObjectPtr<UObject>& InObject)
{
	instance->RemoveActionsForObject(InObject);
}

CSEXPORT FUSharpLatentAction* CSCONV Export_FLatentActionManager_AddNewAction(FLatentActionManager* instance, UObject* InActionObject, int32 UUID, void* ManagedObject, ManagedLatentCallbackDel InCallbackFunc)
{
	FUSharpLatentAction* Action = new FUSharpLatentAction(ManagedObject, InCallbackFunc);
	check(Action);
	// Make sure this UUID doesn't already exist (as this AddNewAction will just wipe over any existing action with this UUID, leaking memory)
	FPendingLatentAction* ExistingAction = instance->FindExistingAction<FPendingLatentAction>(InActionObject, UUID);
	check(!ExistingAction);
	instance->AddNewAction(InActionObject, UUID, Action);
	return Action;
}

CSEXPORT void CSCONV Export_FLatentActionManager_BeginFrame(FLatentActionManager* instance)
{
	instance->BeginFrame();
}

CSEXPORT int32 CSCONV Export_FLatentActionManager_GetNumActionsForObject(FLatentActionManager* instance, const TWeakObjectPtr<UObject>& InObject)
{
	return instance->GetNumActionsForObject(InObject);
}

#if WITH_EDITOR
CSEXPORT void CSCONV Export_FLatentActionManager_GetActiveUUIDs(FLatentActionManager* instance, UObject* InObject, TArray<int32>& result)
{
	TSet<int32> UUIDList;
	instance->GetActiveUUIDs(InObject, UUIDList);
	for (int32 UUID : UUIDList)
	{
		result.Add(UUID);
	}
}

CSEXPORT void CSCONV Export_FLatentActionManager_GetDescription(FLatentActionManager* instance, UObject* InObject, int32 UUID, FString& result)
{
	result = instance->GetDescription(InObject, UUID);
}
#endif

int32 NextLatentUUID = 200000000;
CSEXPORT int32 CSCONV Export_FLatentActionManager_GetNextUUID(UObject* InCallbackTarget)
{
	if (InCallbackTarget)
	{
		if (UWorld* World = GEngine->GetWorldFromContextObject(InCallbackTarget, EGetWorldErrorMode::ReturnNull))
		{
			FLatentActionManager& LatentManager = World->GetLatentActionManager();
			TSharedPtr<FLatentActionManager::FObjectActions>* ObjectActionsSharedPtr = LatentManager.ObjectToActionListMap.Find(InCallbackTarget);
			if (ObjectActionsSharedPtr)
			{
				if (FLatentActionManager::FObjectActions* ObjectActions = ObjectActionsSharedPtr->Get())
				{
					int32 NextUUID = 100000000 + ObjectActions->ActionList.Num();
					while (ObjectActions->ActionList.Find(NextUUID))
					{
						++NextUUID;
					}
					return NextUUID;
				}
			}
		}
	}
	return NextLatentUUID++;
}

CSEXPORT void CSCONV Export_FLatentActionManager(RegisterFunc registerFunc)
{	
	REGISTER_FUNC(Export_FLatentActionManager_ProcessLatentActions);
	REGISTER_FUNC(Export_FLatentActionManager_FindExistingActionUSharp);
	REGISTER_FUNC(Export_FLatentActionManager_FindExistingAction);
	REGISTER_FUNC(Export_FLatentActionManager_RemoveActionsForObject);
	REGISTER_FUNC(Export_FLatentActionManager_AddNewAction);
	REGISTER_FUNC(Export_FLatentActionManager_BeginFrame);
	REGISTER_FUNC(Export_FLatentActionManager_GetNumActionsForObject);
#if WITH_EDITOR
	REGISTER_FUNC(Export_FLatentActionManager_GetActiveUUIDs);
	REGISTER_FUNC(Export_FLatentActionManager_GetDescription);
#endif
	REGISTER_FUNC(Export_FLatentActionManager_GetNextUUID);
}
CSEXPORT csbool CSCONV Export_FTickFunction_Get_bHighPriority(FTickFunction* instance)
{
	return instance->bHighPriority;
}

CSEXPORT void CSCONV Export_FTickFunction_Set_bHighPriority(FTickFunction* instance, csbool value)
{
	instance->bHighPriority = !!value;
}

CSEXPORT csbool CSCONV Export_FTickFunction_Get_bRunOnAnyThread(FTickFunction* instance)
{
	return instance->bRunOnAnyThread;
}

CSEXPORT void CSCONV Export_FTickFunction_Set_bRunOnAnyThread(FTickFunction* instance, csbool value)
{
	instance->bRunOnAnyThread = !!value;
}

CSEXPORT void CSCONV Export_FTickFunction_RegisterTickFunction(FTickFunction* instance, ULevel* Level)
{
	instance->RegisterTickFunction(Level);
}

CSEXPORT void CSCONV Export_FTickFunction_UnRegisterTickFunction(FTickFunction* instance)
{
	instance->UnRegisterTickFunction();
}

CSEXPORT csbool CSCONV Export_FTickFunction_IsTickFunctionRegistered(FTickFunction* instance)
{
	return instance->IsTickFunctionRegistered();
}

CSEXPORT csbool CSCONV Export_FTickFunction_IsCompletionHandleValid(FTickFunction* instance)
{
	return instance->IsCompletionHandleValid();
}

CSEXPORT uint8 CSCONV Export_FTickFunction_GetActualTickGroup(FTickFunction* instance)
{
	return (uint8)instance->GetActualTickGroup();
}

CSEXPORT uint8 CSCONV Export_FTickFunction_GetActualEndTickGroup(FTickFunction* instance)
{
	return (uint8)instance->GetActualEndTickGroup();
}

CSEXPORT void CSCONV Export_FTickFunction_AddPrerequisite(FTickFunction* instance, UObject* TargetObject, struct FTickFunction& TargetTickFunction)
{
	instance->AddPrerequisite(TargetObject, TargetTickFunction);
}

CSEXPORT void CSCONV Export_FTickFunction_RemovePrerequisite(FTickFunction* instance, UObject* TargetObject, struct FTickFunction& TargetTickFunction)
{
	instance->RemovePrerequisite(TargetObject, TargetTickFunction);
}

CSEXPORT void CSCONV Export_FTickFunction_SetPriorityIncludingPrerequisites(FTickFunction* instance, csbool bInHighPriority)
{
	instance->SetPriorityIncludingPrerequisites((bool)bInHighPriority);
}

CSEXPORT TArray<struct FTickPrerequisite>& CSCONV Export_FTickFunction_GetPrerequisites(FTickFunction* instance)
{
	return instance->GetPrerequisites();
}

enum class TickFunctionType : uint8
{
	FActorComponentTickFunction,
	FActorTickFunction,
	FCharacterMovementComponentPostPhysicsTickFunction,
	FEndPhysicsTickFunction,
	FPrimitiveComponentPostPhysicsTickFunction,
	FSkeletalMeshComponentClothTickFunction,
	FSkeletalMeshComponentEndPhysicsTickFunction,
	FStartPhysicsTickFunction
};

CSEXPORT FTickFunction* CSCONV Export_FTickFunction_New(TickFunctionType Type)
{
	switch (Type)
	{
		case TickFunctionType::FActorComponentTickFunction:
			return new FActorComponentTickFunction();
		case TickFunctionType::FActorTickFunction:
			return new FActorTickFunction();
		//case TickFunctionType::FCharacterMovementComponentPostPhysicsTickFunction:
		//	return new FCharacterMovementComponentPostPhysicsTickFunction();
		//case TickFunctionType::FEndPhysicsTickFunction:
		//	return new FEndPhysicsTickFunction();
		//case TickFunctionType::FPrimitiveComponentPostPhysicsTickFunction:
		//	return new FPrimitiveComponentPostPhysicsTickFunction();
		//case TickFunctionType::FSkeletalMeshComponentClothTickFunction:
		//	return new FSkeletalMeshComponentClothTickFunction();
		//case TickFunctionType::FSkeletalMeshComponentEndPhysicsTickFunction:
		//	return new FSkeletalMeshComponentEndPhysicsTickFunction();
		//case TickFunctionType::FStartPhysicsTickFunction:
		//	return new FStartPhysicsTickFunction();
		default:
			check(0);
			break;
	}
	return nullptr;
}

CSEXPORT void CSCONV Export_FTickFunction_Delete(FTickFunction* instance)
{
	delete instance;
}

CSEXPORT void CSCONV Export_FTickFunction(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FTickFunction_Get_bHighPriority);
	REGISTER_FUNC(Export_FTickFunction_Set_bHighPriority);
	REGISTER_FUNC(Export_FTickFunction_Get_bRunOnAnyThread);
	REGISTER_FUNC(Export_FTickFunction_Set_bRunOnAnyThread);
	REGISTER_FUNC(Export_FTickFunction_RegisterTickFunction);
	REGISTER_FUNC(Export_FTickFunction_UnRegisterTickFunction);
	REGISTER_FUNC(Export_FTickFunction_IsTickFunctionRegistered);
	REGISTER_FUNC(Export_FTickFunction_IsCompletionHandleValid);
	REGISTER_FUNC(Export_FTickFunction_GetActualTickGroup);
	REGISTER_FUNC(Export_FTickFunction_GetActualEndTickGroup);
	REGISTER_FUNC(Export_FTickFunction_AddPrerequisite);
	REGISTER_FUNC(Export_FTickFunction_RemovePrerequisite);
	REGISTER_FUNC(Export_FTickFunction_SetPriorityIncludingPrerequisites);
	REGISTER_FUNC(Export_FTickFunction_GetPrerequisites);
	REGISTER_FUNC(Export_FTickFunction_New);
	REGISTER_FUNC(Export_FTickFunction_Delete);
}
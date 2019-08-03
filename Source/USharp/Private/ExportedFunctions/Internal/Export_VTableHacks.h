#include "VTableHacks.h"

GetLifetimeReplicatedPropsCallbackSig GetLifetimeReplicatedPropsCallback = nullptr;
SetupPlayerInputComponentCallbackSig SetupPlayerInputComponentCallback = nullptr;
ActorBeginPlayCallbackSig ActorBeginPlayCallback = nullptr;
ActorEndPlayCallbackSig ActorEndPlayCallback = nullptr;
ActorGetActorEyesViewPointCallbackSig ActorGetActorEyesViewPointCallback = nullptr;
ActorComponentBeginPlayCallbackSig ActorComponentBeginPlayCallback = nullptr;
ActorComponentEndPlayCallbackSig ActorComponentEndPlayCallback = nullptr;
PlayerControllerSetupInputComponentCallbackSig PlayerControllerSetupInputComponentCallback = nullptr;
PlayerControllerUpdateRotationCallbackSig PlayerControllerUpdateRotationCallback = nullptr;
GameInstanceInitCallbackSig GameInstanceInitCallback = nullptr;
SubsystemInitializeCallbackSig SubsystemInitializeCallback = nullptr;
SubsystemInitializeCallbackSig EngineSubsystemInitializeCallback = nullptr;
SubsystemDeinitializeCallbackSig SubsystemDeinitializeCallback = nullptr;
SubsystemShouldCreateSubsystemCallbackSig SubsystemShouldCreateSubsystemCallback = nullptr;

TMap<FString, void**> DummyNames;
CSEXPORT void CSCONV Export_VTableHacks_Set_VTableCallback(const FString& DummyName, void* Callback)
{
	if (DummyNames.Num() == 0)
	{
		// Use the same names as the class
		DummyNames.Add(TEXT("DummyRepProps"), (void**)&GetLifetimeReplicatedPropsCallback);
		DummyNames.Add(TEXT("DummySetupPlayerInput"), (void**)&SetupPlayerInputComponentCallback);
		DummyNames.Add(TEXT("DummyActorBeginPlay"), (void**)&ActorBeginPlayCallback);
		DummyNames.Add(TEXT("DummyActorEndPlay"), (void**)&ActorEndPlayCallback);
		DummyNames.Add(TEXT("DummyActorGetActorEyesViewPoint"), (void**)&ActorGetActorEyesViewPointCallback);
		DummyNames.Add(TEXT("DummyActorComponentBeginPlay"), (void**)&ActorComponentBeginPlayCallback);
		DummyNames.Add(TEXT("DummyActorComponentEndPlay"), (void**)&ActorComponentEndPlayCallback);
		DummyNames.Add(TEXT("DummyPlayerControllerSetupInputComponent"), (void**)&PlayerControllerSetupInputComponentCallback);
		DummyNames.Add(TEXT("DummyPlayerControllerUpdateRotation"), (void**)&PlayerControllerUpdateRotationCallback);
		DummyNames.Add(TEXT("DummyGameInstanceInit"), (void**)&GameInstanceInitCallback);
		DummyNames.Add(TEXT("DummySubsystemInitialize"), (void**)&SubsystemInitializeCallback);
		DummyNames.Add(TEXT("DummySubsystemDeinitialize"), (void**)&SubsystemDeinitializeCallback);
		DummyNames.Add(TEXT("DummySubsystemShouldCreateSubsystem"), (void**)&SubsystemShouldCreateSubsystemCallback);
	}
	
	void*** Element = DummyNames.Find(DummyName);
	if (Element != nullptr)
	{
		**Element = Callback;
	}
}

typedef void (UObject::*GetLifetimeReplicatedPropsFunc)(TArray<FLifetimeProperty>& OutLifetimeProps);
CSEXPORT void CSCONV Export_VTableHacks_CallOriginal_GetLifetimeReplicatedProps(GetLifetimeReplicatedPropsFunc Func, UObject* Obj, TArray<FLifetimeProperty>& OutLifetimeProps)
{
	(Obj->*Func)(OutLifetimeProps);
}

typedef void (UObject::*SetupPlayerInputComponentFunc)(UInputComponent* PlayerInputComponent);

CSEXPORT void CSCONV Export_VTableHacks_CallOriginal_SetupPlayerInputComponent(SetupPlayerInputComponentFunc Func, UObject* Obj, UInputComponent* PlayerInputComponent)
{
	(Obj->*Func)(PlayerInputComponent);
}

typedef void (UObject::*ActorBeginPlayFunc)();
CSEXPORT void CSCONV Export_VTableHacks_CallOriginal_ActorBeginPlay(ActorBeginPlayFunc Func, UObject* Obj)
{
	(Obj->*Func)();
}

typedef void (UObject::*ActorEndPlayFunc)(const EEndPlayReason::Type EndPlayReason);
CSEXPORT void CSCONV Export_VTableHacks_CallOriginal_ActorEndPlay(ActorEndPlayFunc Func, UObject* Obj, const EEndPlayReason::Type EndPlayReason)
{
	(Obj->*Func)(EndPlayReason);
}

typedef void (UObject::*ActorComponentBeginPlayFunc)();
CSEXPORT void CSCONV Export_VTableHacks_CallOriginal_ActorComponentBeginPlay(ActorComponentBeginPlayFunc Func, UObject* Obj)
{
	(Obj->*Func)();
}

typedef void (UObject::*ActorComponentEndPlayFunc)(const EEndPlayReason::Type EndPlayReason);
CSEXPORT void CSCONV Export_VTableHacks_CallOriginal_ActorComponentEndPlay(ActorComponentEndPlayFunc Func, UObject* Obj, const EEndPlayReason::Type EndPlayReason)
{
	(Obj->*Func)(EndPlayReason);
}

typedef void (UObject::*PlayerControllerSetupInputComponentFunc)();
CSEXPORT void CSCONV Export_VTableHacks_CallOriginal_PlayerControllerSetupInputComponent(PlayerControllerSetupInputComponentFunc Func, UObject* Obj)
{
	(Obj->*Func)();
}

typedef void (UObject::*PlayerControllerUpdateRotationFunc)(float DeltaTime);
CSEXPORT void CSCONV Export_VTableHacks_CallOriginal_PlayerControllerUpdateRotation(PlayerControllerUpdateRotationFunc Func, UObject* Obj, float DeltaTime)
{
	(Obj->*Func)(DeltaTime);
}

CSEXPORT void CSCONV Export_VTableHacks(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_VTableHacks_Set_VTableCallback);
	REGISTER_FUNC(Export_VTableHacks_CallOriginal_GetLifetimeReplicatedProps);
	REGISTER_FUNC(Export_VTableHacks_CallOriginal_SetupPlayerInputComponent);
	REGISTER_FUNC(Export_VTableHacks_CallOriginal_ActorBeginPlay);
	REGISTER_FUNC(Export_VTableHacks_CallOriginal_ActorEndPlay);
	REGISTER_FUNC(Export_VTableHacks_CallOriginal_ActorComponentBeginPlay);
	REGISTER_FUNC(Export_VTableHacks_CallOriginal_ActorComponentEndPlay);
	REGISTER_FUNC(Export_VTableHacks_CallOriginal_PlayerControllerSetupInputComponent);
	REGISTER_FUNC(Export_VTableHacks_CallOriginal_PlayerControllerUpdateRotation);
}
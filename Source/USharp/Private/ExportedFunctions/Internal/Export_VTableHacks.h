#include "VTableHacks.h"

GetLifetimeReplicatedPropsCallbackSig GetLifetimeReplicatedPropsCallback = nullptr;
SetupPlayerInputComponentCallbackSig SetupPlayerInputComponentCallback = nullptr;
ActorComponentBeginPlayCallbackSig ActorComponentBeginPlayCallback = nullptr;
ActorComponentEndPlayCallbackSig ActorComponentEndPlayCallback = nullptr;

TMap<FString, void**> DummyNames;
CSEXPORT void CSCONV Export_VTableHacks_Set_VTableCallback(const FString& DummyName, void* Callback)
{
	if (DummyNames.Num() == 0)
	{
		// Use the same names as the class
		DummyNames.Add(TEXT("DummyRepProps"), (void**)&GetLifetimeReplicatedPropsCallback);
		DummyNames.Add(TEXT("DummySetupPlayerInput"), (void**)&SetupPlayerInputComponentCallback);
		DummyNames.Add(TEXT("DummyActorComponentBeginPlay"), (void**)&ActorComponentBeginPlayCallback);
		DummyNames.Add(TEXT("DummyActorComponentEndPlay"), (void**)&ActorComponentEndPlayCallback);
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

CSEXPORT void CSCONV Export_VTableHacks(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_VTableHacks_Set_VTableCallback);
	REGISTER_FUNC(Export_VTableHacks_CallOriginal_GetLifetimeReplicatedProps);
	REGISTER_FUNC(Export_VTableHacks_CallOriginal_SetupPlayerInputComponent);
	REGISTER_FUNC(Export_VTableHacks_CallOriginal_ActorComponentBeginPlay);
	REGISTER_FUNC(Export_VTableHacks_CallOriginal_ActorComponentEndPlay);
}
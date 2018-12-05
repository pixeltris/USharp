#pragma once

#include "Engine/EngineBaseTypes.h"
#include "GameFramework/Actor.h"
#include "GameFramework/Pawn.h"
#include "Components/ActorComponent.h"
#include "VTableHacks.generated.h"

USTRUCT()
struct USHARP_API FSharpActorTickFunction : public FActorTickFunction
{
	GENERATED_USTRUCT_BODY()
	
	virtual void ExecuteTick(float DeltaTime, ELevelTick TickType, ENamedThreads::Type CurrentThread, const FGraphEventRef& MyCompletionGraphEvent) override
	{
		if (Target && !Target->IsPendingKillOrUnreachable())
		{
			if (TickType != LEVELTICK_ViewportsOnly || Target->ShouldTickIfViewportsOnly())
			{
				FScopeCycleCounterUObject ActorScope(Target);
				
				if (!Target->GetClass()->HasAnyClassFlags(CLASS_CompiledFromBlueprint))
				{
					Target->ReceiveTick(DeltaTime*Target->CustomTimeDilation);
				}
				Target->TickActor(DeltaTime*Target->CustomTimeDilation, TickType, *this);
			}
		}
	}
};

template<>
struct TStructOpsTypeTraits<FSharpActorTickFunction> : public TStructOpsTypeTraitsBase2<FSharpActorTickFunction>
{
	enum
	{
		WithCopy = false
	};
};

USTRUCT()
struct USHARP_API FSharpActorComponentTickFunction : public FActorComponentTickFunction
{
	GENERATED_USTRUCT_BODY()
	
	virtual void ExecuteTick(float DeltaTime, ELevelTick TickType, ENamedThreads::Type CurrentThread, const FGraphEventRef& MyCompletionGraphEvent) override
	{
		ExecuteTickHelper(Target, Target->bTickInEditor, DeltaTime, TickType, [this, TickType](float DilatedTime)
		{
			if (!Target->GetClass()->HasAnyClassFlags(CLASS_CompiledFromBlueprint))
			{
				Target->ReceiveTick(DilatedTime);
			}
			Target->TickComponent(DilatedTime, TickType, this);
		});
	}
};

template<>
struct TStructOpsTypeTraits<FSharpActorComponentTickFunction> : public TStructOpsTypeTraitsBase2<FSharpActorComponentTickFunction>
{
	enum
	{
		WithCopy = false
	};
};

// NOTE: We MUST define a new class for each function get ONE vtable difference in each class
// NOTE: Redefinition errors if callbacks are assigned in this file (due to VTableHacks.generated.h includes)

/////////////////////////////////////////////////////////////////////////////
// UObject::GetLifetimeReplicatedProps
/////////////////////////////////////////////////////////////////////////////

typedef void (*GetLifetimeReplicatedPropsCallbackSig)(const UObject* Obj, TArray<FLifetimeProperty>& OutLifetimeProps);
extern GetLifetimeReplicatedPropsCallbackSig GetLifetimeReplicatedPropsCallback;

UCLASS(NotBlueprintable, NotBlueprintType)
class USHARP_API UDummyRepProps1 : public UObject
{
	GENERATED_BODY()

public:
	virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override
	{
		if (GetLifetimeReplicatedPropsCallback != nullptr)
		{
			GetLifetimeReplicatedPropsCallback(this, OutLifetimeProps);
		}
	}
};

UCLASS(NotBlueprintable, NotBlueprintType)
class USHARP_API UDummyRepProps2 : public UDummyRepProps1
{
	GENERATED_BODY()
};

UCLASS(NotBlueprintable, NotBlueprintType)
class USHARP_API UDummyRepProps3 : public UDummyRepProps2
{
	GENERATED_BODY()

public:
	virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override
	{
		FMsg::Logf("", 0, FName(TEXT("USharp")), ELogVerbosity::Log, TEXT("UDummyRepProps3-GetLifetimeReplicatedProps"));
	}
};

/////////////////////////////////////////////////////////////////////////////
// APawn::SetupPlayerInputComponent
/////////////////////////////////////////////////////////////////////////////

typedef void (*SetupPlayerInputComponentCallbackSig)(APawn* Obj, UInputComponent* PlayerInputComponent);
extern SetupPlayerInputComponentCallbackSig SetupPlayerInputComponentCallback;

UCLASS(NotBlueprintable, NotBlueprintType)
class USHARP_API ADummySetupPlayerInput1 : public APawn
{
	GENERATED_BODY()

protected:
	virtual void SetupPlayerInputComponent(UInputComponent* PlayerInputComponent) override
	{
		if (SetupPlayerInputComponentCallback != nullptr)
		{
			SetupPlayerInputComponentCallback(this, PlayerInputComponent);
		}
	}
};

UCLASS(NotBlueprintable, NotBlueprintType)
class USHARP_API ADummySetupPlayerInput2 : public ADummySetupPlayerInput1
{
	GENERATED_BODY()
};

UCLASS(NotBlueprintable, NotBlueprintType)
class USHARP_API ADummySetupPlayerInput3 : public ADummySetupPlayerInput2
{
	GENERATED_BODY()

protected:
	virtual void SetupPlayerInputComponent(UInputComponent* PlayerInputComponent) override
	{
		FMsg::Logf("", 0, FName(TEXT("USharp")), ELogVerbosity::Log, TEXT("ADummySetupPlayerInput3-SetupPlayerInputComponent"));
	}
};
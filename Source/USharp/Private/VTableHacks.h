#include "Engine/EngineBaseTypes.h"
#include "GameFramework/Actor.h"
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

typedef void (*GetLifetimeReplicatedPropsCallbackSig)(const UObject* Obj, TArray<FLifetimeProperty>& OutLifetimeProps);
extern GetLifetimeReplicatedPropsCallbackSig GetLifetimeReplicatedPropsCallback;// Redefinition errors if assigned here

UCLASS()
class USHARP_API UDummyObject1 : public UObject
{
	GENERATED_BODY()

public:
	void OnGetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const
	{
		if (GetLifetimeReplicatedPropsCallback != nullptr)
		{
			GetLifetimeReplicatedPropsCallback(this, OutLifetimeProps);
		}
	}

	virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override
	{
		//FMsg::Logf("", 0, FName(TEXT("USharp")), ELogVerbosity::Log, TEXT("UDummyObject1-GetLifetimeReplicatedProps"));
		OnGetLifetimeReplicatedProps(OutLifetimeProps);
	}
};

UCLASS()
class USHARP_API UDummyObject2 : public UDummyObject1
{
	GENERATED_BODY()
};

UCLASS()
class USHARP_API UDummyObject3 : public UDummyObject2
{
	GENERATED_BODY()

public:
	virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override
	{
		FMsg::Logf("", 0, FName(TEXT("USharp")), ELogVerbosity::Log, TEXT("UDummyObject3-GetLifetimeReplicatedProps"));
	}
};
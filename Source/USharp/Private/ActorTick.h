#include "Engine/EngineBaseTypes.h"
#include "GameFramework/Actor.h"
#include "Components/ActorComponent.h"
#include "ActorTick.generated.h"

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
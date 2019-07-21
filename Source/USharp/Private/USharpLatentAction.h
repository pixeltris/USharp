#pragma once
#include "LatentActions.h"
#include "Kismet/BlueprintAsyncActionBase.h"
#include "Net/OnlineBlueprintCallProxyBase.h"
#include "GameplayTask.h"
#include "ExportedFunctionsConventions.h"
#include "USharpLatentAction.generated.h"

enum class ManagedLatentCallbackType : int32
{
	None,

	FUSharpLatentAction_UpdateOperation,
	FUSharpLatentAction_NotifyObjectDestroyed,
	FUSharpLatentAction_NotifyActionAborted,
	FUSharpLatentAction_GetDescription,
	FUSharpLatentAction_Destructor,
	
	UUSharpAsyncActionBase_Activate,
	UUSharpAsyncActionBase_RegisterWithGameInstanceWorldContext,
	UUSharpAsyncActionBase_RegisterWithGameInstance,
	UUSharpAsyncActionBase_SetReadyToDestroy,
	UUSharpAsyncActionBase_BeginDestroy,
	
	UUSharpOnlineBlueprintCallProxyBase_Activate,
	UUSharpOnlineBlueprintCallProxyBase_BeginDestroy,
	
	UUSharpGameplayTask_Activate,
	UUSharpGameplayTask_InitSimulatedTask,
	UUSharpGameplayTask_TickTask,
	UUSharpGameplayTask_ExternalConfirm,
	UUSharpGameplayTask_ExternalCancel,
	UUSharpGameplayTask_GetDebugString,
	UUSharpGameplayTask_OnDestroy,
	UUSharpGameplayTask_Pause,
	UUSharpGameplayTask_Resume,
	UUSharpGameplayTask_GenerateDebugDescription
};

typedef void(CSCONV *ManagedLatentCallbackDel)(ManagedLatentCallbackType CallbackType, const void* ThisPtr, const void* Data);
extern ManagedLatentCallbackDel UUSharpAsyncActionBaseCallback;
extern ManagedLatentCallbackDel UUSharpOnlineBlueprintCallProxyBaseCallback;
extern ManagedLatentCallbackDel UUSharpGameplayTaskCallback;

class FUSharpLatentAction : public FPendingLatentAction
{
public:
	void* ManagedObject;
	bool bManagedObjectDestroyed;
	ManagedLatentCallbackDel CallbackFunc;

	FUSharpLatentAction(void* InManagedObject, ManagedLatentCallbackDel InCallbackFunc)
		: ManagedObject(InManagedObject)
		, bManagedObjectDestroyed(false)
		, CallbackFunc(InCallbackFunc)
	{ }
	
	~FUSharpLatentAction()
	{
		if (!bManagedObjectDestroyed)
		{
			CallbackFunc(ManagedLatentCallbackType::FUSharpLatentAction_Destructor, this, nullptr);
			bManagedObjectDestroyed = true;
		}
	}
	
	virtual void UpdateOperation(FLatentResponse& Response) override
	{
		if (bManagedObjectDestroyed)
		{
			Response.DoneIf(true);
		}
		else
		{
			CallbackFunc(ManagedLatentCallbackType::FUSharpLatentAction_UpdateOperation, this, &Response);
		}
	}
	
	virtual void NotifyObjectDestroyed() override
	{
		if (!bManagedObjectDestroyed)
		{
			CallbackFunc(ManagedLatentCallbackType::FUSharpLatentAction_NotifyObjectDestroyed, this, nullptr);
		}
	}
	
	virtual void NotifyActionAborted() override
	{
		if (!bManagedObjectDestroyed)
		{
			CallbackFunc(ManagedLatentCallbackType::FUSharpLatentAction_NotifyActionAborted, this, nullptr);
		}
	}
	
#if WITH_EDITOR
	virtual FString GetDescription() const override
	{
		if (bManagedObjectDestroyed)
		{
			return FPendingLatentAction::GetDescription();
		}
		else
		{
			FString Result;
			CallbackFunc(ManagedLatentCallbackType::FUSharpLatentAction_GetDescription, this, &Result);
			return Result;
		}
	}
#endif
};

UCLASS(Abstract)
class UUSharpAsyncActionBase : public UBlueprintAsyncActionBase
{
	GENERATED_BODY()
public:
	virtual void BeginDestroy() override
	{
		// We aren't using the real destructor as that gets invoked in the middle of garbage collection which
		// causes issues with GCHelper.Find<T>
		if (UUSharpAsyncActionBaseCallback)
		{
			UUSharpAsyncActionBaseCallback(ManagedLatentCallbackType::UUSharpAsyncActionBase_BeginDestroy, this, nullptr);
		}
		Super::BeginDestroy();
	}

	virtual void Activate() override
	{
		if (UUSharpAsyncActionBaseCallback)
		{
			UUSharpAsyncActionBaseCallback(ManagedLatentCallbackType::UUSharpAsyncActionBase_Activate, this, nullptr);
		}
		else
		{
			Super::Activate();
		}
	}
	
	virtual void RegisterWithGameInstance(UObject* WorldContextObject) override
	{
		if (UUSharpAsyncActionBaseCallback)
		{
			UUSharpAsyncActionBaseCallback(ManagedLatentCallbackType::UUSharpAsyncActionBase_RegisterWithGameInstanceWorldContext, this, WorldContextObject);
		}
		else
		{
			Super::RegisterWithGameInstance(WorldContextObject);
		}
	}
	
	virtual void RegisterWithGameInstance(UGameInstance* GameInstance) override
	{
		if (UUSharpAsyncActionBaseCallback)
		{
			UUSharpAsyncActionBaseCallback(ManagedLatentCallbackType::UUSharpAsyncActionBase_RegisterWithGameInstance, this, GameInstance);
		}
		else
		{
			Super::RegisterWithGameInstance(GameInstance);
		}
	}
	
	virtual void SetReadyToDestroy() override
	{
		if (UUSharpAsyncActionBaseCallback)
		{
			UUSharpAsyncActionBaseCallback(ManagedLatentCallbackType::UUSharpAsyncActionBase_SetReadyToDestroy, this, nullptr);
		}
		else
		{
			Super::SetReadyToDestroy();
		}
	}
	
	UGameInstance* GetRegisteredWithGameInstance()
	{
		return RegisteredWithGameInstance.Get();
	}
};

UCLASS(Abstract)
class UUSharpOnlineBlueprintCallProxyBase : public UOnlineBlueprintCallProxyBase
{
	GENERATED_BODY()
public:
	virtual void BeginDestroy() override
	{
		if (UUSharpOnlineBlueprintCallProxyBaseCallback)
		{
			UUSharpOnlineBlueprintCallProxyBaseCallback(ManagedLatentCallbackType::UUSharpOnlineBlueprintCallProxyBase_BeginDestroy, this, nullptr);
		}
		Super::BeginDestroy();
	}

	virtual void Activate() override
	{
		if (UUSharpOnlineBlueprintCallProxyBaseCallback)
		{
			UUSharpOnlineBlueprintCallProxyBaseCallback(ManagedLatentCallbackType::UUSharpOnlineBlueprintCallProxyBase_Activate, this, nullptr);
		}
	}
};

UCLASS(Abstract)
class UUSharpGameplayTask : public UGameplayTask
{
	GENERATED_BODY()

protected:
	virtual void Activate() override
	{
		if (UUSharpGameplayTaskCallback)
		{
			UUSharpGameplayTaskCallback(ManagedLatentCallbackType::UUSharpGameplayTask_Activate, this, nullptr);
		}
		else
		{
			Super::Activate();
		}
	}

public:
	virtual void InitSimulatedTask(UGameplayTasksComponent& InGameplayTasksComponent) override
	{
		if (UUSharpGameplayTaskCallback)
		{
			UUSharpGameplayTaskCallback(ManagedLatentCallbackType::UUSharpGameplayTask_InitSimulatedTask, this, &InGameplayTasksComponent);
		}
		else
		{
			Super::InitSimulatedTask(InGameplayTasksComponent);
		}
	}
	
	virtual void TickTask(float DeltaTime) override
	{
		if (UUSharpGameplayTaskCallback)
		{
			UUSharpGameplayTaskCallback(ManagedLatentCallbackType::UUSharpGameplayTask_TickTask, this, &DeltaTime);
		}
		else
		{
			Super::TickTask(DeltaTime);
		}
	}
	
	virtual void ExternalConfirm(bool bEndTask) override
	{
		if (UUSharpGameplayTaskCallback)
		{
			csbool bEndTaskTemp = bEndTask;
			UUSharpGameplayTaskCallback(ManagedLatentCallbackType::UUSharpGameplayTask_ExternalConfirm, this, &bEndTaskTemp);
		}
		else
		{
			Super::ExternalConfirm(bEndTask);
		}
	}
	
	virtual void ExternalCancel() override
	{
		if (UUSharpGameplayTaskCallback)
		{
			UUSharpGameplayTaskCallback(ManagedLatentCallbackType::UUSharpGameplayTask_ExternalCancel, this, nullptr);
		}
		else
		{
			Super::ExternalCancel();
		}
	}
	
	virtual FString GetDebugString() const override
	{
		if (UUSharpGameplayTaskCallback)
		{
			FString Result;
			UUSharpGameplayTaskCallback(ManagedLatentCallbackType::UUSharpGameplayTask_GetDebugString, this, &Result);
			return Result;
		}
		else
		{
			return Super::GetDebugString();
		}
	}
	
protected:
	virtual void OnDestroy(bool bInOwnerFinished)
	{
		if (UUSharpGameplayTaskCallback)
		{
			csbool bInOwnerFinishedTemp = bInOwnerFinished;
			UUSharpGameplayTaskCallback(ManagedLatentCallbackType::UUSharpGameplayTask_OnDestroy, this, &bInOwnerFinishedTemp);
		}
		else
		{
			Super::OnDestroy(bInOwnerFinished);
		}
	}
	
	virtual void Pause()
	{
		if (UUSharpGameplayTaskCallback)
		{
			UUSharpGameplayTaskCallback(ManagedLatentCallbackType::UUSharpGameplayTask_Pause, this, nullptr);
		}
		else
		{
			Super::Pause();
		}
	}
	
	virtual void Resume()
	{
		if (UUSharpGameplayTaskCallback)
		{
			UUSharpGameplayTaskCallback(ManagedLatentCallbackType::UUSharpGameplayTask_Resume, this, nullptr);
		}
		else
		{
			Super::Resume();
		}
	}
	
public:
#if !(UE_BUILD_SHIPPING || UE_BUILD_TEST)
	virtual FString GenerateDebugDescription() const
	{
		if (UUSharpGameplayTaskCallback)
		{
			FString Result;
			UUSharpGameplayTaskCallback(ManagedLatentCallbackType::UUSharpGameplayTask_GenerateDebugDescription, this, &Result);
			return Result;
		}
		else
		{
			return Super::GenerateDebugDescription();
		}
	}
#endif

	void Internal_Base_Activate()
	{
		Super::Activate();
	}
	
	void Internal_Base_OnDestroy(csbool bInOwnerFinished)
	{
		Super::OnDestroy((bool)bInOwnerFinished);
	}
	
	void Internal_Base_Pause()
	{
		Super::Pause();
	}
	
	void Internal_Base_Resume()
	{
		Super::Resume();
	}

public:
	FName Internal_Get_InstanceName()
	{
		return InstanceName;
	}
	
	void Internal_Set_InstanceName(const FName& Value)
	{
		InstanceName = Value;
	}
	
	uint8 Internal_Get_Priority()
	{
		return Priority;
	}
	
	void Internal_Set_Priority(uint8 Value)
	{
		Priority = Value;
	}
	
	ETaskResourceOverlapPolicy Internal_Get_ResourceOverlapPolicy()
	{
		return ResourceOverlapPolicy;
	}
	
	void Internal_Set_ResourceOverlapPolicy(ETaskResourceOverlapPolicy Value)
	{
		ResourceOverlapPolicy = Value;
	}
	
	csbool Internal_Get_bTickingTask()
	{
		return (csbool)bTickingTask;
	}
	
	void Internal_Set_bTickingTask(csbool Value)
	{
		bTickingTask = Value;
	}
	
	csbool Internal_Get_bSimulatedTask()
	{
		return (csbool)bSimulatedTask;
	}
	
	void Internal_Set_bSimulatedTask(csbool Value)
	{
		bSimulatedTask = Value;
	}
	
	csbool Internal_Get_bIsSimulating()
	{
		return (csbool)bIsSimulating;
	}
	
	void Internal_Set_bIsSimulating(csbool Value)
	{
		bIsSimulating = Value;
	}
	
	csbool Internal_Get_bIsPausable()
	{
		return (csbool)bIsPausable;
	}
	
	void Internal_Set_bIsPausable(csbool Value)
	{
		bIsPausable = Value;
	}
	
	csbool Internal_Get_bCaresAboutPriority()
	{
		return (csbool)bCaresAboutPriority;
	}
	
	void Internal_Set_bCaresAboutPriority(csbool Value)
	{
		bCaresAboutPriority = Value;
	}
	
	csbool Internal_Get_bOwnedByTasksComponent()
	{
		return (csbool)bOwnedByTasksComponent;
	}
	
	void Internal_Set_bOwnedByTasksComponent(csbool Value)
	{
		bOwnedByTasksComponent = Value;
	}
	
	csbool Internal_Get_bClaimRequiredResources()
	{
		return (csbool)bClaimRequiredResources;
	}
	
	void Internal_Set_bClaimRequiredResources(csbool Value)
	{
		bClaimRequiredResources = Value;
	}
	
	csbool Internal_Get_bOwnerFinished()
	{
		return (csbool)bOwnerFinished;
	}
	
	void Internal_Set_bOwnerFinished(csbool Value)
	{
		bOwnerFinished = Value;
	}
	
	FGameplayResourceSet Internal_Get_RequiredResources()
	{
		return RequiredResources;
	}
	
	void Internal_Set_RequiredResources(FGameplayResourceSet Value)
	{
		RequiredResources = Value;
	}
	
	FGameplayResourceSet Internal_Get_ClaimedResources()
	{
		return ClaimedResources;
	}
	
	void Internal_Set_ClaimedResources(FGameplayResourceSet Value)
	{
		ClaimedResources = Value;
	}

	IGameplayTaskOwnerInterface* Internal_Get_TaskOwner()
	{
		return TaskOwner.operator->();
	}
	
	UGameplayTasksComponent* Internal_Get_TasksComponent()
	{
		return TasksComponent.Get();
	}
	
	UGameplayTask* Internal_Get_ChildTask()
	{
		return ChildTask;
	}
	
	void Internal_InitTask(IGameplayTaskOwnerInterface& InTaskOwner, uint8 InPriority)
	{
		InitTask(InTaskOwner, InPriority);
	}
	
	static IGameplayTaskOwnerInterface* Internal_ConvertToTaskOwner(UObject& OwnerObject)
	{
		return ConvertToTaskOwner(OwnerObject);
	}
	
	static IGameplayTaskOwnerInterface* Internal_ConvertToTaskOwnerActor(AActor& OwnerActor)
	{
		return ConvertToTaskOwner(OwnerActor);
	}
};
#pragma once
#include "LatentActions.h"
#include "ExportedFunctionsConventions.h"

typedef void(CSCONV *UpdateOperationDel)(FLatentResponse&);
typedef void(CSCONV *NotifyDel)();
typedef void(CSCONV *GetDescriptionDel)(FString& Str);

class FUSharpLatentAction : public FPendingLatentAction
{
public:
	void* ManagedObject;
	bool bManagedObjectDestroyed;
	UpdateOperationDel UpdateOperationFunc;
	NotifyDel NotifyObjectDestroyedFunc;
	NotifyDel NotifyActionAbortedFunc;
	GetDescriptionDel GetDescriptionFunc;
	NotifyDel DestructorFunc;

	FUSharpLatentAction(void* InManagedObject, UpdateOperationDel InUpdateOperationFunc, NotifyDel InNotifyObjectDestroyedFunc, NotifyDel InNotifyActionAbortedFunc, GetDescriptionDel InGetDescriptionFunc, NotifyDel InDestructorFunc)
		: ManagedObject(InManagedObject)
		, bManagedObjectDestroyed(false)
		, UpdateOperationFunc(InUpdateOperationFunc)
		, NotifyObjectDestroyedFunc(InNotifyObjectDestroyedFunc)
		, NotifyActionAbortedFunc(InNotifyActionAbortedFunc)
		, GetDescriptionFunc(InGetDescriptionFunc)
		, DestructorFunc(InDestructorFunc)
	{ }
	
	~FUSharpLatentAction()
	{
		if (!bManagedObjectDestroyed)
		{
			DestructorFunc();
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
			UpdateOperationFunc(Response);
		}
	}
	
	virtual void NotifyObjectDestroyed() override
	{
		if (!bManagedObjectDestroyed)
		{
			NotifyObjectDestroyedFunc();
		}
	}
	
	virtual void NotifyActionAborted() override
	{
		if (!bManagedObjectDestroyed)
		{
			NotifyActionAbortedFunc();
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
			FString Description;
			GetDescriptionFunc(Description);
			return Description;
		}
	}
#endif
};
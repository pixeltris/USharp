#pragma once

#if WITH_EDITOR

#include "CoreMinimal.h"
#if WITH_ENGINE
#include "Kismet2/KismetReinstanceUtilities.h"
#endif

class UBlueprint;

#if WITH_ENGINE

/**
* Helper class used for re-instancing native and blueprint classes after hot-reload.
* This is a copy of FHotReloadClassReinstancer with minor changes for working with USharpClass.
* - Here we only ever work with one intance of a given class (there is no new/old for USharpClass).
* - We avoid recompiling any depends which are USharpClass as they are handled elsewhere.
* - We seperate reinstancing into a two-pass process so that we can save the old CDO state in the
*   first pass and then recreate it in the second pass after the class has been recreated.
*/
class FSharpHotReloadClassReinstancer : public FBlueprintCompileReinstancer
{
	struct FCDOPropertyInfo
	{
		UProperty* OldProperty;
		UProperty* NewProperty;
	};

	struct FCDOPropertyContainer
	{
		FCDOPropertyContainer* Parent;
		TArray<FCDOPropertyContainer> Children;

		/** The property that was used to get from Parent->this (so we can get the offset to this in instanced objects in the world) */
		UProperty* OldProperty;
		UProperty* NewProperty;

		bool bUpdateRequired;
		void* OldPtr;
		void* NewPtr;
		UStruct* OldStruct;
		UStruct* NewStruct;
		TArray<FCDOPropertyInfo> ChangedProperties;

		void AddChangedProperty(FCDOPropertyInfo ChangedProperty)
		{
			ChangedProperties.Add(ChangedProperty);
			if (!bUpdateRequired)
			{
				bUpdateRequired = true;
				FCDOPropertyContainer* Container = Parent;
				while (Container != nullptr)
				{
					Container->bUpdateRequired = true;
					Container = Container->Parent;
				}
			}
		}

		FCDOPropertyContainer& AddChild()
		{
			int32 Index = Children.AddDefaulted();
			check(Index != INDEX_NONE);
			FCDOPropertyContainer& Result = Children[Index];
			Result = FCDOPropertyContainer();
			Result.Parent = this;
			return Result;
		}

		void RemoveChild(FCDOPropertyContainer& Child)
		{
			int32 Num = Children.Num();
			if (Num > 0)
			{
				// The child should be the last element (we only remove after failing to find any changed properties)
				int32 LastIndex = Num - 1;
				if (&Child == &Children[LastIndex])
				{
					Children.RemoveAt(LastIndex);
				}
				else
				{
					for (int32 Index = 0; Index < Num; ++Index)
					{
						if (&Children[Index] == &Child)
						{
							Children.RemoveAt(Index);
							break;
						}
					}
				}
			}
		}
	};

	/** Hot-reloaded version of the old class */
	UClass* NewClass;

	FCDOPropertyContainer ChangedProperties;

	/** True if the provided native class needs re-instancing */
	bool bNeedsReinstancing;

	/** Necessary for delta serialization */
	UObject* CopyOfPreviousCDO;

	/**
	* Sets the re-instancer up for new class re-instancing
	*
	* @param InNewClass Class that has changed after hot-reload
	* @param InOldClass Class before it was hot-reloaded
	*/
	void SetupNewClassReinstancing(UClass* InNewClass, UClass* InOldClass);

	/**
	* Sets the re-instancer up for old class re-instancing. Always re-creates the CDO.
	*
	* @param InOldClass Class that has NOT changed after hot-reload
	*/
	void RecreateCDOAndSetupOldClassReinstancing(UClass* InOldClass);

	/**
	* Re-creates class default object.
	*
	* @param InClass Class that has NOT changed after hot-reload.
	* @param InOuter Outer for the new CDO.
	* @param InName Name of the new CDO.
	* @param InFlags Flags of the new CDO.
	*/
	void ReconstructClassDefaultObject(UClass* InClass, UObject* InOuter, FName InName, EObjectFlags InFlags);

	/** Collects all of the changed CDO property values for the target class */
	void CollectChangedProperties();

	/**
	* Collects all of the changed CDO property values for the given container (UObject / struct / collection).
	*
	* @param Container The target container to find changed properties
	* @param SeenObjects Objects which have already been seen / processed
	*/
	void CollectChangedProperties(FCDOPropertyContainer& Container, TSet<UObject*>& SeenObjects);

	/** Updates property values on instances of the hot-reloaded class */
	void UpdateDefaultProperties();

	/** Updates property values on instances of the hot-reloaded class */
	void UpdateDefaultProperties(const FCDOPropertyContainer& Container, void* Obj);

	/** Returns true if the properties of the CDO have changed during hot-reload */
	FORCEINLINE bool DefaultPropertiesHaveChanged() const
	{
		return ChangedProperties.bUpdateRequired;
	}

public:
	/** Sets the re-instancer up to re-instance native classes */
	FSharpHotReloadClassReinstancer(UClass* InNewClass, UClass* InOldClass);

	static void Finalize();
	static void Clear();

	/** ReplaceReferencesToReconstructedCDOs is copied from HotReload.cpp */
	static void ReplaceReferencesToReconstructedCDOs();

	/** Destructor */
	virtual ~FSharpHotReloadClassReinstancer();

	/** If true, the class needs re-instancing */
	FORCEINLINE bool ClassNeedsReinstancing() const
	{
		return bNeedsReinstancing;
	}

	/** Reinstances all objects of the hot-reloaded class and update their properties to match the new CDO */
	void ReinstanceObjectsAndUpdateDefaults();

	// FSerializableObject interface
	virtual void AddReferencedObjects(FReferenceCollector& Collector) override;
	// End of FSerializableObject interface

	virtual bool IsClassObjectReplaced() const override { return true; }

	virtual void EnlistDependentBlueprintToRecompile(UBlueprint* BP, bool bBytecodeOnly) override;
	virtual void BlueprintWasRecompiled(UBlueprint* BP, bool bBytecodeOnly) override;

protected:

	// FBlueprintCompileReinstancer interface
	virtual bool ShouldPreserveRootComponentOfReinstancedActor() const override { return false; }
	// End of FBlueprintCompileReinstancer interface
};

#endif // WITH_ENGINE

#endif // WITH_EDITOR
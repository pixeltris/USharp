#if WITH_EDITOR

#include "SharpHotReloadClassReinstancer.h"
#include "Serialization/MemoryWriter.h"
#include "UObject/UObjectHash.h"
#include "UObject/UObjectIterator.h"
#include "UObject/Package.h"
#include "Serialization/ArchiveReplaceObjectRef.h"
#include "BlueprintCompilationManager.h"
#if WITH_ENGINE
#include "Engine/Blueprint.h"
#include "Engine/BlueprintGeneratedClass.h"
#endif

// For ReplaceReferencesToReconstructedCDOs
#include "Misc/QueuedThreadPool.h"
#include "Async/AsyncWork.h"

#if WITH_EDITORONLY_DATA
#include "Kismet2/KismetEditorUtilities.h"
#include "BlueprintActionDatabase.h"
#include "BlueprintEditorSettings.h"
#endif

// NOTE: Blueprint uses UEngine::CopyPropertiesForUnrelatedObjects for copying the old CDO to the new one. We don't need to do that?
//       - "Propagate the old CDO's properties to the new"
//       - See FKismetCompilerContext::CompileFunctions / FKismetCompilerContext::CleanAndSanitizeClass
//       - It is also used in the blueprint FBlueprintCompileReinstancer batcher FBlueprintCompilationManagerImpl::ReinstanceBatch

#if WITH_ENGINE

static TSet<UBlueprint*> SharpHotReloadBPSetToRecompile;
static TSet<UBlueprint*> SharpHotReloadBPSetToRecompileBytecodeOnly;
static TMap<UObject*, UObject*> GReconstructedCDOsMap;
static TMap<UClass*, UClass*> GOldToNewClassesMap;
static bool bHasAnyStructuralModification = false;

void FSharpHotReloadClassReinstancer::Finalize()
{
	ReplaceReferencesToReconstructedCDOs();

	// TODO: Compile all remaining blueprints in SharpHotReloadBPSetToRecompile / SharpHotReloadBPSetToRecompileBytecodeOnly?
	// HotReload.cpp doesn't seem to compile any hanging blueprints but it might be desirable to do so?

	Clear();
}

void FSharpHotReloadClassReinstancer::Clear()
{
	SharpHotReloadBPSetToRecompile.Empty();
	SharpHotReloadBPSetToRecompileBytecodeOnly.Empty();
	GReconstructedCDOsMap.Empty();
	bHasAnyStructuralModification = false;
}

// ReplaceReferencesToReconstructedCDOs is copied from HotReload.cpp
void FSharpHotReloadClassReinstancer::ReplaceReferencesToReconstructedCDOs()
{
	if (GReconstructedCDOsMap.Num() == 0)
	{
		return;
	}

	// Thread pool manager. We need new thread pool with increased
	// amount of stack size. Standard GThreadPool was encountering
	// stack overflow error during serialization.
	static struct FReplaceReferencesThreadPool
	{
		FReplaceReferencesThreadPool()
		{
			Pool = FQueuedThreadPool::Allocate();
			int32 NumThreadsInThreadPool = FPlatformMisc::NumberOfWorkerThreadsToSpawn();
			verify(Pool->Create(NumThreadsInThreadPool, 256 * 1024));
		}

		~FReplaceReferencesThreadPool()
		{
			Pool->Destroy();
		}

		FQueuedThreadPool* GetPool() { return Pool; }

	private:
		FQueuedThreadPool* Pool;
	} ThreadPoolManager;

	// Async task to enable multithreaded CDOs reference search.
	class FFindRefTask : public FNonAbandonableTask
	{
	public:
		explicit FFindRefTask(const TMap<UObject*, UObject*>& InReconstructedCDOsMap, int32 ReserveElements)
			: ReconstructedCDOsMap(InReconstructedCDOsMap)
		{
			ObjectsArray.Reserve(ReserveElements);
		}

		void DoWork()
		{
			for (UObject* Object : ObjectsArray)
			{
				class FReplaceCDOReferencesArchive : public FArchiveUObject
				{
				public:
					FReplaceCDOReferencesArchive(UObject* InPotentialReferencer, const TMap<UObject*, UObject*>& InReconstructedCDOsMap)
						: ReconstructedCDOsMap(InReconstructedCDOsMap)
						, PotentialReferencer(InPotentialReferencer)
					{
						ArIsObjectReferenceCollector = true;
						ArIgnoreOuterRef = true;
					}

					virtual FString GetArchiveName() const override
					{
						return TEXT("FReplaceCDOReferencesArchive");
					}

					FArchive& operator<<(UObject*& ObjRef)
					{
						UObject* Obj = ObjRef;

						if (Obj && Obj != PotentialReferencer)
						{
							if (UObject* const* FoundObj = ReconstructedCDOsMap.Find(Obj))
							{
								ObjRef = *FoundObj;
							}
						}

						return *this;
					}

					const TMap<UObject*, UObject*>& ReconstructedCDOsMap;
					UObject* PotentialReferencer;
				};

				FReplaceCDOReferencesArchive FindRefsArchive(Object, ReconstructedCDOsMap);
				Object->Serialize(FindRefsArchive);
			}
		}

		FORCEINLINE TStatId GetStatId() const
		{
			RETURN_QUICK_DECLARE_CYCLE_STAT(FFindRefTask, STATGROUP_ThreadPoolAsyncTasks);
		}

		TArray<UObject*> ObjectsArray;

	private:
		const TMap<UObject*, UObject*>& ReconstructedCDOsMap;
	};

	const int32 NumberOfThreads = FPlatformMisc::NumberOfWorkerThreadsToSpawn();
	const int32 NumObjects = GUObjectArray.GetObjectArrayNum();
	const int32 ObjectsPerTask = FMath::CeilToInt((float)NumObjects / NumberOfThreads);

	// Create tasks.
	TArray<FAsyncTask<FFindRefTask>> Tasks;
	Tasks.Reserve(NumberOfThreads);

	for (int32 TaskId = 0; TaskId < NumberOfThreads; ++TaskId)
	{
		Tasks.Emplace(GReconstructedCDOsMap, ObjectsPerTask);
	}

	// Distribute objects uniformly between tasks.
	int32 CurrentTaskId = 0;
	for (FObjectIterator ObjIter; ObjIter; ++ObjIter)
	{
		UObject* CurObject = *ObjIter;

		if (CurObject->IsPendingKill())
		{
			continue;
		}

		Tasks[CurrentTaskId].GetTask().ObjectsArray.Add(CurObject);
		CurrentTaskId = (CurrentTaskId + 1) % NumberOfThreads;
	}

	// Run async tasks in worker threads.
	for (FAsyncTask<FFindRefTask>& Task : Tasks)
	{
		Task.StartBackgroundTask(ThreadPoolManager.GetPool());
	}

	// Wait until tasks are finished
	for (FAsyncTask<FFindRefTask>& AsyncTask : Tasks)
	{
		AsyncTask.EnsureCompletion();
	}

	GReconstructedCDOsMap.Empty();
}

void FSharpHotReloadClassReinstancer::SetupNewClassReinstancing(UClass* InNewClass, UClass* InOldClass)
{
	// Set base class members to valid values
	ClassToReinstance = InNewClass;
	DuplicatedClass = InOldClass;
	OriginalCDO = InOldClass->GetDefaultObject();
	bHasReinstanced = false;
	bNeedsReinstancing = true;
	NewClass = InNewClass;

	// Collect the CDO property values
	CollectChangedProperties();

	SaveClassFieldMapping(InOldClass);

	ObjectsThatShouldUseOldStuff.Add(InOldClass); //CDO of REINST_ class can be used as archetype

	TArray<UClass*> ChildrenOfClass;
	GetDerivedClasses(InOldClass, ChildrenOfClass);
	for (auto ClassIt = ChildrenOfClass.CreateConstIterator(); ClassIt; ++ClassIt)
	{
		UClass* ChildClass = *ClassIt;
		UBlueprint* ChildBP = Cast<UBlueprint>(ChildClass->ClassGeneratedBy);
		if (ChildBP && !ChildBP->HasAnyFlags(RF_BeingRegenerated))
		{
			// If this is a direct child, change the parent and relink so the property chain is valid for reinstancing
			if (!ChildBP->HasAnyFlags(RF_NeedLoad))
			{
				if (ChildClass->GetSuperClass() == InOldClass)
				{
					ReparentChild(ChildBP);
				}

				Children.AddUnique(ChildBP);
				if (ChildBP->ParentClass == InOldClass)
				{
					ChildBP->ParentClass = NewClass;
				}
			}
			else
			{
				// If this is a child that caused the load of their parent, relink to the REINST class so that we can still serialize in the CDO, but do not add to later processing
				ReparentChild(ChildClass);
			}
		}
	}

	// Finally, remove the old class from Root so that it can get GC'd and mark it as CLASS_NewerVersionExists
	InOldClass->RemoveFromRoot();
	InOldClass->ClassFlags |= CLASS_NewerVersionExists;
}

void FSharpHotReloadClassReinstancer::ReconstructClassDefaultObject(UClass* InClass, UObject* InOuter, FName InName, EObjectFlags InFlags)
{
	// Get the parent CDO
	UClass* ParentClass = InClass->GetSuperClass();
	UObject* ParentDefaultObject = NULL;
	if (ParentClass != NULL)
	{
		ParentDefaultObject = ParentClass->GetDefaultObject(); // Force the default object to be constructed if it isn't already
	}

	// Re-create
	InClass->ClassDefaultObject = StaticAllocateObject(InClass, InOuter, InName, InFlags, EInternalObjectFlags::None, false);
	check(InClass->ClassDefaultObject);
	const bool bShouldInitializeProperties = false;
	const bool bCopyTransientsFromClassDefaults = false;
	(*InClass->ClassConstructor)(FObjectInitializer(InClass->ClassDefaultObject, ParentDefaultObject, bCopyTransientsFromClassDefaults, bShouldInitializeProperties));
}

void FSharpHotReloadClassReinstancer::RecreateCDOAndSetupOldClassReinstancing(UClass* InOldClass)
{
	// Set base class members to valid values
	ClassToReinstance = InOldClass;
	DuplicatedClass = InOldClass;
	OriginalCDO = InOldClass->GetDefaultObject();
	bHasReinstanced = false;
	bNeedsReinstancing = false;
	NewClass = InOldClass; // The class doesn't change in this case

	// Remember all the basic info about the object before we rename it
	EObjectFlags CDOFlags = OriginalCDO->GetFlags();
	UObject* CDOOuter = OriginalCDO->GetOuter();
	FName CDOName = OriginalCDO->GetFName();

	// Rename original CDO, so we can store this one as OverridenArchetypeForCDO
	// and create new one with the same name and outer.
	OriginalCDO->Rename(
		*MakeUniqueObjectName(
			GetTransientPackage(),
			OriginalCDO->GetClass(),
			*FString::Printf(TEXT("BPGC_ARCH_FOR_CDO_%s"), *InOldClass->GetName())
		).ToString(),
		GetTransientPackage(),
		REN_DoNotDirty | REN_DontCreateRedirectors | REN_NonTransactional | REN_SkipGeneratedClasses | REN_ForceNoResetLoaders);

	// Re-create the CDO, re-running its constructor
	ReconstructClassDefaultObject(InOldClass, CDOOuter, CDOName, CDOFlags);

	GReconstructedCDOsMap.Add(OriginalCDO, InOldClass->GetDefaultObject());

	// Collect the changed property values after re-constructing the CDO
	CollectChangedProperties();

	// We only want to re-instance the old class if its CDO's values have changed or any of its DSOs' property values have changed
	if (DefaultPropertiesHaveChanged())
	{
		bNeedsReinstancing = true;
		SaveClassFieldMapping(InOldClass);

		TArray<UClass*> ChildrenOfClass;
		GetDerivedClasses(InOldClass, ChildrenOfClass);
		for (auto ClassIt = ChildrenOfClass.CreateConstIterator(); ClassIt; ++ClassIt)
		{
			UClass* ChildClass = *ClassIt;
			UBlueprint* ChildBP = Cast<UBlueprint>(ChildClass->ClassGeneratedBy);
			if (ChildBP && !ChildBP->HasAnyFlags(RF_BeingRegenerated))
			{
				if (!ChildBP->HasAnyFlags(RF_NeedLoad))
				{
					Children.AddUnique(ChildBP);
					UBlueprintGeneratedClass* BPGC = Cast<UBlueprintGeneratedClass>(ChildBP->GeneratedClass);
					UObject* CurrentCDO = BPGC ? BPGC->GetDefaultObject(false) : nullptr;
					if (CurrentCDO && (OriginalCDO == CurrentCDO->GetArchetype()))
					{
						BPGC->OverridenArchetypeForCDO = OriginalCDO;
					}
				}
			}
		}
	}
}

FSharpHotReloadClassReinstancer::FSharpHotReloadClassReinstancer(UClass* InNewClass, UClass* InOldClass)
	: NewClass(nullptr)
	, bNeedsReinstancing(false)
	, CopyOfPreviousCDO(nullptr)
{
	ensure(InOldClass);
	ensure(!HotReloadedOldClass && !HotReloadedNewClass);
	HotReloadedOldClass = InOldClass;
	HotReloadedNewClass = InNewClass ? InNewClass : InOldClass;

	for (const TPair<UClass*, UClass*>& OldToNewClass : GOldToNewClassesMap)
	{
		ObjectsThatShouldUseOldStuff.Add(OldToNewClass.Key);
	}

	// If InNewClass is NULL, then the old class has not changed after hot-reload.
	// However, we still need to check for changes to its constructor code (CDO values).
	if (InNewClass && InOldClass != InNewClass)
	{
		SetupNewClassReinstancing(InNewClass, InOldClass);

		TMap<UObject*, UObject*> ClassRedirects;
		ClassRedirects.Add(InOldClass, InNewClass);

		for (TObjectIterator<UBlueprint> BlueprintIt; BlueprintIt; ++BlueprintIt)
		{
			FArchiveReplaceObjectRef<UObject> ReplaceObjectArch(*BlueprintIt, ClassRedirects, false, true, true);
			if (ReplaceObjectArch.GetCount())
			{
				EnlistDependentBlueprintToRecompile(*BlueprintIt, false);
			}
		}
	}
	else
	{
		RecreateCDOAndSetupOldClassReinstancing(InOldClass);
	}
}

FSharpHotReloadClassReinstancer::~FSharpHotReloadClassReinstancer()
{
	// Make sure the base class does not remove the DuplicatedClass from root, we not always want it.
	// For example when we're just reconstructing CDOs. Other cases are handled by HotReloadClassReinstancer.
	DuplicatedClass = nullptr;

	ensure(HotReloadedOldClass);
	HotReloadedOldClass = nullptr;
	HotReloadedNewClass = nullptr;
}

void FSharpHotReloadClassReinstancer::CollectChangedProperties()
{
	TSet<UObject*> SeenObjects;
	ChangedProperties.OldStruct = DuplicatedClass;
	ChangedProperties.NewStruct = ClassToReinstance;
	ChangedProperties.OldProperty = nullptr;
	ChangedProperties.NewProperty = nullptr;
	ChangedProperties.OldPtr = OriginalCDO;
	ChangedProperties.NewPtr = ClassToReinstance->GetDefaultObject(true);
	ChangedProperties.bUpdateRequired = false;
	ChangedProperties.Parent = nullptr;
	ChangedProperties.Children.Empty();
	ChangedProperties.ChangedProperties.Empty();
	CollectChangedProperties(ChangedProperties, SeenObjects);
}

void FSharpHotReloadClassReinstancer::CollectChangedProperties(FCDOPropertyContainer& Container, TSet<UObject*>& SeenObjects)
{
	// Related functions:
	//CopyPropertiesToCDO (PersonaUtils.cpp)
	//EditorUtilities::CopySingleProperty
	//EditorUtilities::CopyActorProperties (Editor.cpp)

	TMap<FName, FCDOPropertyInfo> Properties;
	if (Container.OldStruct == Container.NewStruct)
	{
		for (TFieldIterator<UProperty> PropertyIt(Container.OldStruct); PropertyIt; ++PropertyIt)
		{
			UProperty* Property = *PropertyIt;
			FCDOPropertyInfo& PropertyInfo = Properties.FindOrAdd(Property->GetFName());
			PropertyInfo.OldProperty = Property;
			PropertyInfo.NewProperty = Property;
		}
	}
	else
	{
		for (TFieldIterator<UProperty> PropertyIt(Container.OldStruct); PropertyIt; ++PropertyIt)
		{
			UProperty* Property = *PropertyIt;
			FCDOPropertyInfo& PropertyInfo = Properties.FindOrAdd(Property->GetFName());
			PropertyInfo.OldProperty = Property;
		}
		for (TFieldIterator<UProperty> PropertyIt(Container.NewStruct); PropertyIt; ++PropertyIt)
		{
			UProperty* Property = *PropertyIt;
			FCDOPropertyInfo& PropertyInfo = Properties.FindOrAdd(Property->GetFName());
			PropertyInfo.NewProperty = Property;
		}
	}

	for (const TPair<FName, FCDOPropertyInfo>& Pair : Properties)
	{
		const FCDOPropertyInfo& Property = Pair.Value;
		UProperty* OldProperty = Property.OldProperty;
		UProperty* NewProperty = Property.NewProperty;
		if (OldProperty == nullptr || NewProperty == nullptr)
		{
			continue;
		}

		UClass* OldPropertyClass = OldProperty->GetClass();
		UClass* NewPropertyClass = NewProperty->GetClass();

		const EClassCastFlags BaseCastFlags = CASTCLASS_UField | CASTCLASS_UProperty;

		if (OldPropertyClass->ClassCastFlags == NewPropertyClass->ClassCastFlags)
		{
			switch ((uint64)OldPropertyClass->ClassCastFlags)
			{
				// Primitive types
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UInt8Property):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UByteProperty):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UIntProperty):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UFloatProperty):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UUInt64Property):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UUInt32Property):
				case (BaseCastFlags | CASTCLASS_UBoolProperty):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UUInt16Property):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UInt64Property):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UInt16Property):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UDoubleProperty):
				case (BaseCastFlags | CASTCLASS_UEnumProperty):
				// Name / string
				case (BaseCastFlags | CASTCLASS_UNameProperty):
				case (BaseCastFlags | CASTCLASS_UStrProperty):
				// Delegates
				case (BaseCastFlags | CASTCLASS_UDelegateProperty):
				case (BaseCastFlags | CASTCLASS_UMulticastDelegateProperty):
					{
						void* OldValuePtr = OldProperty->ContainerPtrToValuePtr<void>(Container.OldPtr);
						void* NewValuePtr = NewProperty->ContainerPtrToValuePtr<void>(Container.NewPtr);
						if (!OldProperty->Identical(OldValuePtr, NewValuePtr))
						{
							Container.AddChangedProperty(Property);
						}
					}
					break;
				// Text
				case (BaseCastFlags | CASTCLASS_UTextProperty):
					{
						// UTextProperty::Identical / FText::IdenticalTo will often return true for differing values
						// (TextData->GetLocalizedString() will often return "Null" on differing strings whose values are in History)
						UTextProperty* OldTextProperty = CastChecked<UTextProperty>(OldProperty);
						UTextProperty* NewTextProperty = CastChecked<UTextProperty>(NewProperty);
						FText OldText = OldTextProperty->GetPropertyValue_InContainer(Container.OldPtr);
						FText NewText = NewTextProperty->GetPropertyValue_InContainer(Container.NewPtr);
						if (!OldText.EqualTo(NewText))
						{
							Container.AddChangedProperty(Property);
						}
					}
					break;
				// Collections
				case (BaseCastFlags | CASTCLASS_USetProperty):
				case (BaseCastFlags | CASTCLASS_UMapProperty):
				case (BaseCastFlags | CASTCLASS_UArrayProperty):
					break;
				// Interface
				case (BaseCastFlags | CASTCLASS_UInterfaceProperty):
					break;
				// UObjectPropertyBase types
				case (BaseCastFlags | CASTCLASS_UObjectPropertyBase | CASTCLASS_UObjectProperty | CASTCLASS_UClassProperty):
				case (BaseCastFlags | CASTCLASS_UObjectPropertyBase | CASTCLASS_UWeakObjectProperty):
				case (BaseCastFlags | CASTCLASS_UObjectPropertyBase | CASTCLASS_ULazyObjectProperty):
				case (BaseCastFlags | CASTCLASS_UObjectPropertyBase | CASTCLASS_USoftObjectProperty):
				case (BaseCastFlags | CASTCLASS_UObjectPropertyBase | CASTCLASS_USoftClassProperty):
					break;
				case (BaseCastFlags | CASTCLASS_UObjectPropertyBase | CASTCLASS_UObjectProperty):
					{
						UObjectProperty* OldObjectProperty = CastChecked<UObjectProperty>(OldProperty);
						UObjectProperty* NewObjectProperty = CastChecked<UObjectProperty>(NewProperty);
						if (OldObjectProperty->PropertyClass != nullptr &&
							NewObjectProperty->PropertyClass != nullptr &&
							OldObjectProperty->ContainsInstancedObjectProperty() &&
							NewObjectProperty->ContainsInstancedObjectProperty())
						{
							FString OldClassName = OldObjectProperty->PropertyClass->GetName();
							if (OldClassName.StartsWith(TEXT("USharpHotreloaded_"), ESearchCase::CaseSensitive))
							{
								OldClassName = OldClassName.RightChop(18);
							}
							if (OldClassName.Equals(NewObjectProperty->PropertyClass->GetName(), ESearchCase::CaseSensitive))
							{
								UObject* OldObject = OldObjectProperty->GetPropertyValue_InContainer(Container.OldPtr);
								UObject* NewObject = NewObjectProperty->GetPropertyValue_InContainer(Container.NewPtr);
								if (OldObject != nullptr && NewObject != nullptr && !SeenObjects.Contains(NewObject))
								{
									SeenObjects.Add(NewObject);
									FCDOPropertyContainer& ObjContainer = Container.AddChild();
									ObjContainer.OldProperty = OldProperty;
									ObjContainer.NewProperty = NewProperty;
									ObjContainer.OldPtr = OldObject;
									ObjContainer.NewPtr = NewObject;
									ObjContainer.OldStruct = OldObjectProperty->PropertyClass;
									ObjContainer.NewStruct = NewObjectProperty->PropertyClass;
									ObjContainer.Parent = &Container;
									CollectChangedProperties(ObjContainer, SeenObjects);
									if (!ObjContainer.bUpdateRequired)
									{
										Container.RemoveChild(ObjContainer);
									}
								}
							}
						}
					}
					break;
				case (BaseCastFlags | CASTCLASS_UStructProperty):
					{
						FCDOPropertyContainer& StructContainer = Container.AddChild();
						StructContainer.OldProperty = OldProperty;
						StructContainer.NewProperty = NewProperty;
						StructContainer.OldPtr = OldProperty->ContainerPtrToValuePtr<void>(Container.OldPtr);
						StructContainer.NewPtr = NewProperty->ContainerPtrToValuePtr<void>(Container.NewPtr);
						StructContainer.OldStruct = Cast<UStructProperty>(OldProperty)->Struct;
						StructContainer.NewStruct = Cast<UStructProperty>(NewProperty)->Struct;
						StructContainer.Parent = &Container;
						CollectChangedProperties(StructContainer, SeenObjects);
						if (!StructContainer.bUpdateRequired)
						{
							Container.RemoveChild(StructContainer);
						}
					}
					break;
				default:
					//check(0);
					UE_LOG(LogTemp, Fatal, TEXT("Unhandled property class cast flags %llu"), (uint64)OldPropertyClass->ClassCastFlags);
					break;
			}
		}
		else
		{
			// TODO: Handle type changes
			check(0);
		}
	}
}

void FSharpHotReloadClassReinstancer::UpdateDefaultProperties()
{
	for (FObjectIterator It(NewClass); It; ++It)
	{
		UObject* ObjectPtr = *It;

		if (ObjectPtr == ChangedProperties.OldPtr ||
			ObjectPtr == ChangedProperties.NewPtr)
		{
			// Skip the old/new CDO
			continue;
		}

		UpdateDefaultProperties(ChangedProperties, (void*)ObjectPtr);
		
		AActor* Actor = Cast<AActor>(ObjectPtr);
		if (Actor != nullptr)
		{
			Actor->MarkComponentsRenderStateDirty();
		}
		else
		{
			USceneComponent* SceneComponent = Cast<USceneComponent>(ObjectPtr);
			if (SceneComponent != nullptr)
			{
				SceneComponent->MarkRenderStateDirty();
			}
		}
	}
}

void FSharpHotReloadClassReinstancer::UpdateDefaultProperties(const FCDOPropertyContainer& Container, void* Obj)
{
	for (const FCDOPropertyInfo& Property : Container.ChangedProperties)
	{
		UProperty* OldProperty = Property.OldProperty;
		UProperty* NewProperty = Property.NewProperty;
		UClass* OldPropertyClass = OldProperty->GetClass();
		UClass* NewPropertyClass = NewProperty->GetClass();

		const EClassCastFlags BaseCastFlags = CASTCLASS_UField | CASTCLASS_UProperty;

		if (OldPropertyClass->ClassCastFlags == NewPropertyClass->ClassCastFlags)
		{
			switch ((uint64)OldPropertyClass->ClassCastFlags)
			{
				// Primitive types
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UInt8Property):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UByteProperty):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UIntProperty):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UFloatProperty):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UUInt64Property):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UUInt32Property):
				case (BaseCastFlags | CASTCLASS_UBoolProperty):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UUInt16Property):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UInt64Property):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UInt16Property):
				case (BaseCastFlags | CASTCLASS_UNumericProperty | CASTCLASS_UDoubleProperty):
				// Name / string
				case (BaseCastFlags | CASTCLASS_UNameProperty):
				case (BaseCastFlags | CASTCLASS_UStrProperty):
				// Delegates
				case (BaseCastFlags | CASTCLASS_UDelegateProperty):
				case (BaseCastFlags | CASTCLASS_UMulticastDelegateProperty):
					{
						void* OldValuePtr = OldProperty->ContainerPtrToValuePtr<void>(Container.OldPtr);
						void* NewValuePtr = NewProperty->ContainerPtrToValuePtr<void>(Obj);
						if (OldProperty->Identical(OldValuePtr, NewValuePtr))
						{
							NewProperty->CopyCompleteValue_InContainer(Obj, Container.NewPtr);
						}
					}
					break;
				// Text
				case (BaseCastFlags | CASTCLASS_UTextProperty):
					{
						UTextProperty* OldTextProperty = CastChecked<UTextProperty>(OldProperty);
						UTextProperty* NewTextProperty = CastChecked<UTextProperty>(NewProperty);
						FText OldText = OldTextProperty->GetPropertyValue_InContainer(Container.OldPtr);
						FText NewText = NewTextProperty->GetPropertyValue_InContainer(Obj);
						if (OldText.EqualTo(NewText))
						{
							NewProperty->CopyCompleteValue_InContainer(Obj, Container.NewPtr);
						}
					}
					break;
			}
		}
		else
		{
			// TODO: Handle type changes
			check(0);
		}
	}

	for (const FCDOPropertyContainer& ChildContainer : Container.Children)
	{
		void* ChildPtr = nullptr;

		const EClassCastFlags BaseCastFlags = CASTCLASS_UField | CASTCLASS_UProperty;
		switch ((uint64)ChildContainer.NewProperty->GetClass()->ClassCastFlags)
		{
			case (BaseCastFlags | CASTCLASS_UObjectPropertyBase | CASTCLASS_UObjectProperty):
				{
					UObjectProperty* ObjectProperty = CastChecked<UObjectProperty>(ChildContainer.NewProperty);
					ChildPtr = ObjectProperty->GetPropertyValue_InContainer(Obj);
				}
				break;
			default:
				{
					ChildPtr = ChildContainer.NewProperty->ContainerPtrToValuePtr<void>(Obj);
				}
				break;
		}

		if (ChildPtr != nullptr)
		{
			UpdateDefaultProperties(ChildContainer, ChildPtr);
		}
	}
}

void FSharpHotReloadClassReinstancer::ReinstanceObjectsAndUpdateDefaults()
{
	// Update the archetypes for this class. This MUST be done for component types to ensure correct lookups of the archetypes.
	// This could possibly be skipped for other types (blueprint compilation always recreates the archetypes).
	// Requires engine source modification
	/*{
		TMap<UClass*, UClass*> OldClassToNewClass;
		OldClassToNewClass.Add(DuplicatedClass, ClassToReinstance);
		FBlueprintCompilationManager::UpdateArchetypesForHotreload(OldClassToNewClass);

		bUpdatedClassArchetypes = true;
	}*/

	ReinstanceObjects(true);
	UpdateDefaultProperties();
}

void FSharpHotReloadClassReinstancer::AddReferencedObjects(FReferenceCollector& Collector)
{
	FBlueprintCompileReinstancer::AddReferencedObjects(Collector);
	Collector.AllowEliminatingReferences(false);
	Collector.AddReferencedObject(CopyOfPreviousCDO);
	Collector.AllowEliminatingReferences(true);
}

void FSharpHotReloadClassReinstancer::EnlistDependentBlueprintToRecompile(UBlueprint* BP, bool bBytecodeOnly)
{
	if (IsValid(BP))
	{
		if (bBytecodeOnly)
		{
			if (!SharpHotReloadBPSetToRecompile.Contains(BP) && !SharpHotReloadBPSetToRecompileBytecodeOnly.Contains(BP))
			{
				SharpHotReloadBPSetToRecompileBytecodeOnly.Add(BP);
			}
		}
		else
		{
			if (!SharpHotReloadBPSetToRecompile.Contains(BP))
			{
				if (SharpHotReloadBPSetToRecompileBytecodeOnly.Contains(BP))
				{
					SharpHotReloadBPSetToRecompileBytecodeOnly.Remove(BP);
				}

				SharpHotReloadBPSetToRecompile.Add(BP);
			}
		}
	}
}

void FSharpHotReloadClassReinstancer::BlueprintWasRecompiled(UBlueprint* BP, bool bBytecodeOnly)
{
	SharpHotReloadBPSetToRecompile.Remove(BP);
	SharpHotReloadBPSetToRecompileBytecodeOnly.Remove(BP);

	FBlueprintCompileReinstancer::BlueprintWasRecompiled(BP, bBytecodeOnly);
}

#endif // WITH_ENGINE

#endif // WITH_EDITOR
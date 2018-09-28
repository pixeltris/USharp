#include "SharpHotReloadClassReinstancer.h"
#include "Serialization/MemoryWriter.h"
#include "UObject/UObjectHash.h"
#include "UObject/UObjectIterator.h"
#include "UObject/Package.h"
#include "Serialization/ArchiveReplaceObjectRef.h"
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

	// Collect the original CDO property values
	SerializeCDOProperties(InOldClass->GetDefaultObject(), OriginalCDOProperties);
	// Collect the property values of the new CDO
	SerializeCDOProperties(InNewClass->GetDefaultObject(), ReconstructedCDOProperties);

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

void FSharpHotReloadClassReinstancer::SerializeCDOProperties(UObject* InObject, FSharpHotReloadClassReinstancer::FCDOPropertyData& OutData)
{
	// Creates a mem-comparable CDO data
	class FCDOWriter : public FMemoryWriter
	{
		/** Objects already visited by this archive */
		TSet<UObject*>& VisitedObjects;
		/** Output property data */
		FCDOPropertyData& PropertyData;
		/** Current subobject being serialized */
		FName SubobjectName;

	public:
		/** Serializes all script properties of the provided DefaultObject */
		FCDOWriter(FCDOPropertyData& InOutData, UObject* DefaultObject, TSet<UObject*>& InVisitedObjects, FName InSubobjectName = NAME_None)
			: FMemoryWriter(InOutData.Bytes, /* bIsPersistent = */ false, /* bSetOffset = */ true)
			, VisitedObjects(InVisitedObjects)
			, PropertyData(InOutData)
			, SubobjectName(InSubobjectName)
		{
			// Disable delta serialization, we want to serialize everything
			ArNoDelta = true;
			DefaultObject->SerializeScriptProperties(*this);
		}
		virtual void Serialize(void* Data, int64 Num) override
		{
			// Collect serialized properties so we can later update their values on instances if they change
			UProperty* SerializedProperty = GetSerializedProperty();
			if (SerializedProperty != nullptr)
			{
				FCDOProperty& PropertyInfo = PropertyData.Properties.FindOrAdd(SerializedProperty->GetFName());
				if (PropertyInfo.Property == nullptr)
				{
					PropertyInfo.Property = SerializedProperty;
					PropertyInfo.SubobjectName = SubobjectName;
					PropertyInfo.SerializedValueOffset = Tell();
					PropertyInfo.SerializedValueSize = Num;
				}
				else
				{
					PropertyInfo.SerializedValueSize += Num;
				}
			}
			FMemoryWriter::Serialize(Data, Num);
		}
		/** Serializes an object. Only name and class for normal references, deep serialization for DSOs */
		virtual FArchive& operator<<(class UObject*& InObj) override
		{
			FArchive& Ar = *this;
			if (InObj)
			{
				FName ClassName = InObj->GetClass()->GetFName();
				FName ObjectName = InObj->GetFName();
				Ar << ClassName;
				Ar << ObjectName;
				if (!VisitedObjects.Contains(InObj))
				{
					VisitedObjects.Add(InObj);
					if (Ar.GetSerializedProperty() && Ar.GetSerializedProperty()->ContainsInstancedObjectProperty())
					{
						// Serialize all DSO properties too					
						FCDOWriter DefaultSubobjectWriter(PropertyData, InObj, VisitedObjects, InObj->GetFName());
						Seek(PropertyData.Bytes.Num());
					}
				}
			}
			else
			{
				FName UnusedName = NAME_None;
				Ar << UnusedName;
				Ar << UnusedName;
			}

			return *this;
		}
		/** Serializes an FName as its index and number */
		virtual FArchive& operator<<(FName& InName) override
		{
			FArchive& Ar = *this;
			NAME_INDEX ComparisonIndex = InName.GetComparisonIndex();
			NAME_INDEX DisplayIndex = InName.GetDisplayIndex();
			int32 Number = InName.GetNumber();
			Ar << ComparisonIndex;
			Ar << DisplayIndex;
			Ar << Number;
			return Ar;
		}
		virtual FArchive& operator<<(FLazyObjectPtr& LazyObjectPtr) override
		{
			FArchive& Ar = *this;
			FUniqueObjectGuid UniqueID = LazyObjectPtr.GetUniqueID();
			Ar << UniqueID;
			return *this;
		}
		virtual FArchive& operator<<(FSoftObjectPtr& Value) override
		{
			FArchive& Ar = *this;
			FSoftObjectPath UniqueID = Value.GetUniqueID();
			Ar << UniqueID;
			return Ar;
		}
		virtual FArchive& operator<<(FSoftObjectPath& Value) override
		{
			FArchive& Ar = *this;

			FString Path = Value.ToString();

			Ar << Path;

			if (IsLoading())
			{
				Value.SetPath(MoveTemp(Path));
			}

			return Ar;
		}
		FArchive& operator<<(FWeakObjectPtr& WeakObjectPtr) override
		{
			WeakObjectPtr.Serialize(*this);
			return *this;
		}
		/** Archive name, for debugging */
		virtual FString GetArchiveName() const override { return TEXT("FCDOWriter"); }
	};
	TSet<UObject*> VisitedObjects;
	VisitedObjects.Add(InObject);
	FCDOWriter Ar(OutData, InObject, VisitedObjects);
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

	// Collect the original property values
	SerializeCDOProperties(InOldClass->GetDefaultObject(), OriginalCDOProperties);

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

	// Collect the property values after re-constructing the CDO
	SerializeCDOProperties(InOldClass->GetDefaultObject(), ReconstructedCDOProperties);

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

/** Helper for finding subobject in an array. Usually there's not that many subobjects on a class to justify a TMap */
FORCEINLINE static UObject* FindDefaultSubobject(TArray<UObject*>& InDefaultSubobjects, FName SubobjectName)
{
	for (UObject* Subobject : InDefaultSubobjects)
	{
		if (Subobject->GetFName() == SubobjectName)
		{
			return Subobject;
		}
	}
	return nullptr;
}

void FSharpHotReloadClassReinstancer::UpdateDefaultProperties()
{
	struct FPropertyToUpdate
	{
		UProperty* Property;
		FName SubobjectName;
		uint8* OldSerializedValuePtr;
		uint8* NewValuePtr;
		int64 OldSerializedSize;
	};
	/** Memory writer archive that supports UObject values the same way as FCDOWriter. */
	class FPropertyValueMemoryWriter : public FMemoryWriter
	{
	public:
		FPropertyValueMemoryWriter(TArray<uint8>& OutData)
			: FMemoryWriter(OutData)
		{}
		virtual FArchive& operator<<(class UObject*& InObj) override
		{
			FArchive& Ar = *this;
			if (InObj)
			{
				FName ClassName = InObj->GetClass()->GetFName();
				FName ObjectName = InObj->GetFName();
				Ar << ClassName;
				Ar << ObjectName;
			}
			else
			{
				FName UnusedName = NAME_None;
				Ar << UnusedName;
				Ar << UnusedName;
			}
			return *this;
		}
		virtual FArchive& operator<<(FName& InName) override
		{
			FArchive& Ar = *this;
			NAME_INDEX ComparisonIndex = InName.GetComparisonIndex();
			NAME_INDEX DisplayIndex = InName.GetDisplayIndex();
			int32 Number = InName.GetNumber();
			Ar << ComparisonIndex;
			Ar << DisplayIndex;
			Ar << Number;
			return Ar;
		}
		virtual FArchive& operator<<(FLazyObjectPtr& LazyObjectPtr) override
		{
			FArchive& Ar = *this;
			FUniqueObjectGuid UniqueID = LazyObjectPtr.GetUniqueID();
			Ar << UniqueID;
			return *this;
		}
		virtual FArchive& operator<<(FSoftObjectPtr& Value) override
		{
			FArchive& Ar = *this;
			FSoftObjectPath UniqueID = Value.GetUniqueID();
			Ar << UniqueID;
			return Ar;
		}
		virtual FArchive& operator<<(FSoftObjectPath& Value) override
		{
			FArchive& Ar = *this;

			FString Path = Value.ToString();

			Ar << Path;

			if (IsLoading())
			{
				Value.SetPath(MoveTemp(Path));
			}

			return Ar;
		}
		FArchive& operator<<(FWeakObjectPtr& WeakObjectPtr) override
		{
			WeakObjectPtr.Serialize(*this);
			return *this;
		}
	};

	// Collect default subobjects to update their properties too
	const int32 DefaultSubobjectArrayCapacity = 16;
	TArray<UObject*> DefaultSubobjectArray;
	DefaultSubobjectArray.Empty(DefaultSubobjectArrayCapacity);
	NewClass->GetDefaultObject()->CollectDefaultSubobjects(DefaultSubobjectArray);

	TArray<FPropertyToUpdate> PropertiesToUpdate;
	// Collect all properties that have actually changed
	for (const TPair<FName, FCDOProperty>& Pair : ReconstructedCDOProperties.Properties)
	{
		FCDOProperty* OldPropertyInfo = OriginalCDOProperties.Properties.Find(Pair.Key);
		if (OldPropertyInfo)
		{
			const FCDOProperty& NewPropertyInfo = Pair.Value;

			uint8* OldSerializedValuePtr = OriginalCDOProperties.Bytes.GetData() + OldPropertyInfo->SerializedValueOffset;
			uint8* NewSerializedValuePtr = ReconstructedCDOProperties.Bytes.GetData() + NewPropertyInfo.SerializedValueOffset;
			if (OldPropertyInfo->SerializedValueSize != NewPropertyInfo.SerializedValueSize ||
				FMemory::Memcmp(OldSerializedValuePtr, NewSerializedValuePtr, OldPropertyInfo->SerializedValueSize) != 0)
			{
				// Property value has changed so add it to the list of properties that need updating on instances
				FPropertyToUpdate PropertyToUpdate;
				PropertyToUpdate.Property = NewPropertyInfo.Property;
				PropertyToUpdate.NewValuePtr = nullptr;
				PropertyToUpdate.SubobjectName = NewPropertyInfo.SubobjectName;

				if (NewPropertyInfo.Property->GetOuter() == NewClass)
				{
					PropertyToUpdate.NewValuePtr = PropertyToUpdate.Property->ContainerPtrToValuePtr<uint8>(NewClass->GetDefaultObject());
				}
				else if (NewPropertyInfo.SubobjectName != NAME_None)
				{
					UObject* DefaultSubobjectPtr = FindDefaultSubobject(DefaultSubobjectArray, NewPropertyInfo.SubobjectName);
					if (DefaultSubobjectPtr && NewPropertyInfo.Property->GetOuter() == DefaultSubobjectPtr->GetClass())
					{
						PropertyToUpdate.NewValuePtr = PropertyToUpdate.Property->ContainerPtrToValuePtr<uint8>(DefaultSubobjectPtr);
					}
				}
				if (PropertyToUpdate.NewValuePtr)
				{
					PropertyToUpdate.OldSerializedValuePtr = OldSerializedValuePtr;
					PropertyToUpdate.OldSerializedSize = OldPropertyInfo->SerializedValueSize;

					PropertiesToUpdate.Add(PropertyToUpdate);
				}
			}
		}
	}
	if (PropertiesToUpdate.Num())
	{
		TArray<uint8> CurrentValueSerializedData;

		// Update properties on all existing instances of the class
		for (FObjectIterator It(NewClass); It; ++It)
		{
			UObject* ObjectPtr = *It;
			DefaultSubobjectArray.Empty(DefaultSubobjectArrayCapacity);
			ObjectPtr->CollectDefaultSubobjects(DefaultSubobjectArray);

			for (auto& PropertyToUpdate : PropertiesToUpdate)
			{
				uint8* InstanceValuePtr = nullptr;
				if (PropertyToUpdate.SubobjectName == NAME_None)
				{
					InstanceValuePtr = PropertyToUpdate.Property->ContainerPtrToValuePtr<uint8>(ObjectPtr);
				}
				else
				{
					UObject* DefaultSubobjectPtr = FindDefaultSubobject(DefaultSubobjectArray, PropertyToUpdate.SubobjectName);
					if (DefaultSubobjectPtr && PropertyToUpdate.Property->GetOuter() == DefaultSubobjectPtr->GetClass())
					{
						InstanceValuePtr = PropertyToUpdate.Property->ContainerPtrToValuePtr<uint8>(DefaultSubobjectPtr);
					}
				}

				if (InstanceValuePtr)
				{
					// Serialize current value to a byte array as we don't have the previous CDO to compare against, we only have its serialized property data
					CurrentValueSerializedData.Empty(CurrentValueSerializedData.Num() + CurrentValueSerializedData.GetSlack());
					FPropertyValueMemoryWriter CurrentValueWriter(CurrentValueSerializedData);
					PropertyToUpdate.Property->SerializeItem(CurrentValueWriter, InstanceValuePtr);

					// Update only when the current value on the instance is identical to the original CDO
					if (CurrentValueSerializedData.Num() == PropertyToUpdate.OldSerializedSize &&
						FMemory::Memcmp(CurrentValueSerializedData.GetData(), PropertyToUpdate.OldSerializedValuePtr, CurrentValueSerializedData.Num()) == 0)
					{
						// Update with the new value
						PropertyToUpdate.Property->CopyCompleteValue(InstanceValuePtr, PropertyToUpdate.NewValuePtr);
					}
				}
			}
		}
	}
}

void FSharpHotReloadClassReinstancer::ReinstanceObjectsAndUpdateDefaults()
{
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

#endif
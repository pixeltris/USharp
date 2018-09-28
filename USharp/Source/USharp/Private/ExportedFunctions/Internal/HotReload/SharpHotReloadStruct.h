#if WITH_EDITOR

#include "SharpClass.h"

// Also see SharpHotReloadStruct_VERBOSE.h which copies most of the original UserDefinedStructureCompilerUtils.cpp code.
// - It will do the UUserDefinedStruct duplication before we build our C# structs and then manually compile those struct
//   after (which also updates blueprints etc all in one pass) - it might be better to use the verbose method otherwise
//   blueprints may be compiled many times.

//#define LOCTEXT_NAMESPACE "USharpCompiler"
//DECLARE_LOG_CATEGORY(LogUSharpCompiler);

// Engine\Source\Editor\UnrealEd\Private\Kismet2\StructureEditorUtils.cpp
struct FReinstanceDataTableHelper
{
	// TODO: shell we cache the dependency?
	static TArray<UDataTable*> GetTablesDependentOnStruct(UScriptStruct* Struct)
	{
		TArray<UDataTable*> Result;
		if (Struct)
		{
			TArray<UObject*> DataTables;
			GetObjectsOfClass(UDataTable::StaticClass(), DataTables);
			for (UObject* DataTableObj : DataTables)
			{
				UDataTable* DataTable = Cast<UDataTable>(DataTableObj);
				if (DataTable && (Struct == DataTable->RowStruct))
				{
					Result.Add(DataTable);
				}
			}
		}
		return Result;
	}
};

// From FUserDefinedStructureCompilerInner::ClearStructReferencesInBP 
void ClearStructReferencesInBP(UBlueprint* FoundBlueprint, TSet<UBlueprint*>& BlueprintsToRecompile)
{
	bool bAlreadyProcessed = false;
	BlueprintsToRecompile.Add(FoundBlueprint, &bAlreadyProcessed);
	if (!bAlreadyProcessed)
	{
		for (auto Function : TFieldRange<UFunction>(FoundBlueprint->GeneratedClass, EFieldIteratorFlags::ExcludeSuper))
		{
			Function->Script.Empty();
		}
		FoundBlueprint->Status = BS_Dirty;
	}
}

// From FUserDefinedStructureCompilerInner::ReplaceStructWithTempDuplicate (Engine\Source\Editor\KismetCompiler\Private\UserDefinedStructureCompilerUtils.cpp)
void ReplaceStructWithTempDuplicate(USharpStruct* StructureToReinstance, TSet<UBlueprint*>& BlueprintsToRecompile, TArray<UUserDefinedStruct*>& ChangedStructs)
{
	USharpStruct* DuplicatedStruct = NULL;
	{
		const FString ReinstancedName = FString::Printf(TEXT("STRUCT_REINST_%s"), *StructureToReinstance->GetName());
		const FName UniqueName = MakeUniqueObjectName(GetTransientPackage(), StructureToReinstance->GetClass(), FName(*ReinstancedName));

		TGuardValue<bool> IsDuplicatingClassForReinstancing(GIsDuplicatingClassForReinstancing, true);
		DuplicatedStruct = (USharpStruct*)StaticDuplicateObject(StructureToReinstance, GetTransientPackage(), UniqueName, ~RF_Transactional);
	}

	// Remove the Standalone flag so that the duplicated struct can be cleaned up
	DuplicatedStruct->ClearFlags(RF_Standalone);

	DuplicatedStruct->Guid = StructureToReinstance->Guid;
	DuplicatedStruct->Bind();
	DuplicatedStruct->StaticLink(true);
	DuplicatedStruct->SetFlags(RF_Transient);
	DuplicatedStruct->AddToRoot();

	for (auto StructProperty : TObjectRange<UStructProperty>(RF_ClassDefaultObject, /** bIncludeDerivedClasses */ true, /** InternalExcludeFlags */ EInternalObjectFlags::PendingKill))
	{
		if (StructProperty && (StructureToReinstance == StructProperty->Struct))
		{
			if (auto OwnerClass = Cast<UBlueprintGeneratedClass>(StructProperty->GetOwnerClass()))
			{
				UBlueprint* FoundBlueprint = Cast<UBlueprint>(OwnerClass->ClassGeneratedBy);
				if (FoundBlueprint != nullptr && Cast<USharpClass>(OwnerClass) == nullptr)// Don't mess with any USharpClass instances
				{
					StructProperty->Struct = DuplicatedStruct;
					ClearStructReferencesInBP(FoundBlueprint, BlueprintsToRecompile);
				}
			}
			else if (auto OwnerStruct = Cast<UUserDefinedStruct>(StructProperty->GetOwnerStruct()))
			{
				//check(OwnerStruct != DuplicatedStruct);
				const bool bValidStruct = (OwnerStruct->GetOutermost() != GetTransientPackage())
					&& !OwnerStruct->IsPendingKill()
					&& (EUserDefinedStructureStatus::UDSS_Duplicate != OwnerStruct->Status.GetValue());

				if (bValidStruct)
				{
					ChangedStructs.AddUnique(OwnerStruct);
					StructProperty->Struct = DuplicatedStruct;
				}
			}
			else
			{
				// We are only looking for editor defined owners (UBlueprintGeneratedClass / UUserDefinedStruct). Our structs which appear on 
				// C# defined structs / classes are handled in C# and which don't need additional editor updates.  There isn't any reason to 
				// log this is an error.
				//UE_LOG(LogTemp, Error, TEXT("USharp-ReplaceStructWithTempDuplicate unknown owner"));
			}
		}
	}

	DuplicatedStruct->RemoveFromRoot();

	// CachedUDSDependencies will never contain our struct as it only holds UUserDefinedStruct instances
	// - We might need to reimplement a full search
	// - This means a full search of all UK2Node instances similar to the post process

	//for (auto Blueprint : TObjectRange<UBlueprint>(RF_ClassDefaultObject, /** bIncludeDerivedClasses */ true, /** InternalExcludeFlags */ EInternalObjectFlags::PendingKill))
	//{
	//	if (Blueprint && !BlueprintsToRecompile.Contains(Blueprint))
	//	{
	//		FBlueprintEditorUtils::EnsureCachedDependenciesUpToDate(Blueprint);
	//		if (Blueprint->CachedUDSDependencies.Contains(StructureToReinstance))
	//		{
	//			ClearStructReferencesInBP(Blueprint, BlueprintsToRecompile);
	//		}
	//	}
	//}
}

void SharpHotReloadUtils_PreUpdateStructs(TArray<UScriptStruct*>& SharpStructs, TSet<UBlueprint*>** OutBlueprintsToRecompile, TArray<UUserDefinedStruct*>** OutChangedStructs)
{
	*OutBlueprintsToRecompile = new TSet<UBlueprint*>();
	*OutChangedStructs = new TArray<UUserDefinedStruct*>();

	for (UScriptStruct* Struct : SharpStructs)
	{
		// From FStructureEditorUtils::BroadcastPreChange (Engine\Source\Editor\UnrealEd\Private\Kismet2\StructureEditorUtils.cpp)
		TArray<UDataTable*> DataTables = FReinstanceDataTableHelper::GetTablesDependentOnStruct(Struct);
		for (UDataTable* DataTable : DataTables)
		{
			DataTable->CleanBeforeStructChange();
		}

		// From FUserDefinedStructureCompilerInner::ReplaceStructWithTempDuplicate (Engine\Source\Editor\KismetCompiler\Private\UserDefinedStructureCompilerUtils.cpp)
		ReplaceStructWithTempDuplicate(Cast<USharpStruct>(Struct), **OutBlueprintsToRecompile, **OutChangedStructs);
	}
}

// TODO: Change these to be TSet instead of TArray as we are doing quite a few lookups
void SharpHotReloadUtils_PostUpdateStructs(TArray<UScriptStruct*>& SharpChangedStructsOld, TArray<UScriptStruct*>& SharpChangedStructsNew, TSet<UBlueprint*>* InBlueprintsToRecompile, TArray<UUserDefinedStruct*>* InChangedStructs)
{
	TSet<UBlueprint*>& BlueprintsToRecompile = *InBlueprintsToRecompile;
	TArray<UUserDefinedStruct*>& ChangedStructs = *InChangedStructs;

	// Update struct members in structs
	for (auto ChangedStruct : ChangedStructs)
	{
		if (ChangedStruct->EditorData != nullptr)
		{
			UUserDefinedStructEditorData* EditorData = Cast<UUserDefinedStructEditorData>(ChangedStruct->EditorData);
			if (EditorData != nullptr)
			{
				for (FStructVariableDescription& VarDesc : EditorData->VariablesDescriptions)
				{
					// SubCategoryObject doesn't need updating as it is a TSoftObjectPtr<UObject>

					if (VarDesc.ContainerType != EPinContainerType::None)
					{
						if (VarDesc.PinValueType.TerminalCategory == TEXT("struct"))
						{
							UObject* Obj = VarDesc.PinValueType.TerminalSubCategoryObject.Get();
							if (Obj != nullptr)
							{
								USharpStruct* SharpStruct = Cast<USharpStruct>(Obj);
								if (SharpStruct != nullptr)
								{
									int32 StructIndex = SharpStruct ? SharpChangedStructsOld.Find(SharpStruct) : INDEX_NONE;
									if (StructIndex >= 0)
									{
										VarDesc.PinValueType.TerminalSubCategoryObject = SharpChangedStructsNew[StructIndex];
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Update struct members in blueprints
	for (UBlueprint* Blueprint : BlueprintsToRecompile)
	{
		for (FBPVariableDescription& VarDesc : Blueprint->NewVariables)
		{
			if (VarDesc.VarType.PinCategory == TEXT("struct"))
			{
				UObject* Obj = VarDesc.VarType.PinSubCategoryObject.Get();
				if (Obj != nullptr)
				{
					USharpStruct* SharpStruct = Cast<USharpStruct>(Obj);
					if (SharpStruct != nullptr)
					{
						int32 StructIndex = SharpStruct ? SharpChangedStructsOld.Find(SharpStruct) : INDEX_NONE;
						if (StructIndex >= 0)
						{
							VarDesc.VarType.PinSubCategoryObject = SharpChangedStructsNew[StructIndex];
						}
					}
				}
			}

			if (VarDesc.VarType.ContainerType != EPinContainerType::None)
			{
				if (VarDesc.VarType.PinValueType.TerminalCategory == TEXT("struct"))
				{
					UObject* Obj = VarDesc.VarType.PinValueType.TerminalSubCategoryObject.Get();
					if (Obj != nullptr)
					{
						USharpStruct* SharpStruct = Cast<USharpStruct>(Obj);
						if (SharpStruct != nullptr)
						{
							int32 StructIndex = SharpStruct ? SharpChangedStructsOld.Find(SharpStruct) : INDEX_NONE;
							if (StructIndex >= 0)
							{
								VarDesc.VarType.PinValueType.TerminalSubCategoryObject = SharpChangedStructsNew[StructIndex];
							}
						}
					}
				}
			}
		}
	}

	for (auto ChangedStruct : ChangedStructs)
	{
		if (ChangedStruct)
		{
			// Set the state to dirty beforehand otherwise the struct will be logged as "empty" when creating the temp duplicate
			// FStructureEditorUtils::IsStructureValid
			ChangedStruct->Status = EUserDefinedStructureStatus::UDSS_Dirty;

			IKismetCompilerInterface& Compiler = FModuleManager::LoadModuleChecked<IKismetCompilerInterface>(KISMET_COMPILER_MODULENAME);
			FCompilerResultsLog Results;
			Compiler.CompileStructure(ChangedStruct, Results);
		}
	}

	{
		// UPDATE ALL THINGS DEPENDENT ON COMPILED STRUCTURES
		TSet<UBlueprint*> BlueprintsThatHaveBeenRecompiled;
		for (TObjectIterator<UK2Node> It(RF_Transient | RF_ClassDefaultObject, /** bIncludeDerivedClasses */ true, /** InternalExcludeFlags */ EInternalObjectFlags::PendingKill); It && SharpChangedStructsOld.Num(); ++It)
		{
			bool bReconstruct = false;

			UK2Node* Node = *It;

			if (Node && !Node->HasAnyFlags(RF_Transient) && !Node->IsPendingKill())
			{
				// If this is a struct operation node operation on the changed struct we must reconstruct
				if (UK2Node_StructOperation* StructOpNode = Cast<UK2Node_StructOperation>(Node))
				{
					UScriptStruct* StructInNode = StructOpNode->StructType;
					int32 StructIndex = StructInNode ? SharpChangedStructsOld.Find(StructInNode) : INDEX_NONE;
					if (StructIndex >= 0)
					{
						StructOpNode->StructType = SharpChangedStructsNew[StructIndex];
						bReconstruct = true;
					}
				}
				if (!bReconstruct)
				{
					// Look through the nodes pins and if any of them are split and the type of the split pin is a user defined struct we need to reconstruct
					for (UEdGraphPin* Pin : Node->Pins)
					{
						if (Pin->SubPins.Num() > 0)
						{
							UScriptStruct* StructType = Cast<UScriptStruct>(Pin->PinType.PinSubCategoryObject.Get());
							int32 StructIndex = StructType ? SharpChangedStructsOld.Find(StructType) : INDEX_NONE;
							if (StructIndex >= 0)
							{
								Pin->PinType.PinSubCategoryObject = SharpChangedStructsNew[StructIndex];
								bReconstruct = true;
								break;
							}
						}

					}
				}
			}

			if (bReconstruct)
			{
				if (Node->HasValidBlueprint())
				{
					UBlueprint* FoundBlueprint = Node->GetBlueprint();
					// The blueprint skeleton needs to be updated before we reconstruct the node
					// or else we may have member references that point to the old skeleton
					if (!BlueprintsThatHaveBeenRecompiled.Contains(FoundBlueprint))
					{
						BlueprintsThatHaveBeenRecompiled.Add(FoundBlueprint);
						BlueprintsToRecompile.Remove(FoundBlueprint);
						FBlueprintEditorUtils::MarkBlueprintAsStructurallyModified(FoundBlueprint);
					}
					Node->ReconstructNode();
				}
			}
		}

		for (auto BPIter = BlueprintsToRecompile.CreateIterator(); BPIter; ++BPIter)
		{
			FBlueprintEditorUtils::MarkBlueprintAsStructurallyModified(*BPIter);
		}
	}

	// From FStructureEditorUtils::BroadcastPostChange (Engine\Source\Editor\UnrealEd\Private\Kismet2\StructureEditorUtils.cpp)
	//for (UScriptStruct* Struct : SharpChangedStructs)
	for (int32 Index = 0; Index < SharpChangedStructsOld.Num(); Index++)
	{
		UScriptStruct* OldStruct = SharpChangedStructsOld[Index];
		UScriptStruct* NewStruct = SharpChangedStructsNew[Index];

		TArray<UDataTable*> DataTables = FReinstanceDataTableHelper::GetTablesDependentOnStruct(OldStruct);
		for (UDataTable* DataTable : DataTables)
		{
			DataTable->RowStruct = NewStruct;
			DataTable->RestoreAfterStructChange();
		}
	}

	delete InBlueprintsToRecompile;
	delete InChangedStructs;
}
//#undef LOCTEXT_NAMESPACE
#endif



//C++ struct editor data PreChange / PostChange:
//
//FStructureDefaultValueView
//{
//	virtual void PreChange(const class UUserDefinedStruct* Struct, FStructureEditorUtils::EStructureEditorChangeInfo Info) override
//	{
//		// No need to destroy the struct data if only the default values are changing
//		if (Info != FStructureEditorUtils::DefaultValueChanged)
//		{
//			StructData->Destroy();
//			DetailsView->SetObject(nullptr);
//			DetailsView->OnFinishedChangingProperties().Clear();
//		}
//	}
//	
//	virtual void PostChange(const class UUserDefinedStruct* Struct, FStructureEditorUtils::EStructureEditorChangeInfo Info) override
//	{
//		// If change is due to default value, then struct data was not destroyed (see PreChange) and therefore does not need to be re-initialized
//		if (Info != FStructureEditorUtils::DefaultValueChanged)
//		{
//			StructData->Initialize(UserDefinedStruct.Get());
//			DetailsView->SetObject(UserDefinedStruct.Get());
//		}
//
//		FStructureEditorUtils::Fill_MakeStructureDefaultValue(UserDefinedStruct.Get(), StructData->GetStructMemory());
//	}
//}
//
// If the target struct is the owned by the currently active editor util window recreate the layout of the window
///** FStructureEditorUtils::INotifyOnStructChanged */
//void FUserDefinedStructureDetails::PostChange(const class UUserDefinedStruct* Struct, FStructureEditorUtils::EStructureEditorChangeInfo Info)
//{
//	if (Struct && (GetUserDefinedStruct() == Struct))
//	{
//		if (Layout.IsValid())
//		{
//			Layout->OnChanged();
//		}
//	}
//}
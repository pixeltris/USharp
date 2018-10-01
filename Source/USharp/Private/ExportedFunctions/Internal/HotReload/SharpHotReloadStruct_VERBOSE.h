#if WITH_EDITOR

#include "SharpClass.h"

#define LOCTEXT_NAMESPACE "USharpCompiler"
//DECLARE_LOG_CATEGORY(LogUSharpCompiler);



// Create a straight copy of FUserDefinedStructureCompilerInner (Engine\Source\Editor\KismetCompiler\Private\UserDefinedStructureCompilerUtils.cpp)
// - This is so that we can create temp duplicate of editor structs before compiling ours and then we
//   can compile the editor structs afterward compiling ours (without it attempting to do the duplication at that point)
struct FUserDefinedStructureCompilerInner
{
	static void ClearStructReferencesInBP(UBlueprint* FoundBlueprint, TSet<UBlueprint*>& BlueprintsToRecompile)
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

	static void ReplaceStructWithTempDuplicate(
		UUserDefinedStruct* StructureToReinstance,
		TSet<UBlueprint*>& BlueprintsToRecompile,
		TArray<UUserDefinedStruct*>& ChangedStructs)
	{
		if (StructureToReinstance)
		{
			UUserDefinedStruct* DuplicatedStruct = NULL;
			{
				const FString ReinstancedName = FString::Printf(TEXT("STRUCT_REINST_%s"), *StructureToReinstance->GetName());
				const FName UniqueName = MakeUniqueObjectName(GetTransientPackage(), UUserDefinedStruct::StaticClass(), FName(*ReinstancedName));

				TGuardValue<bool> IsDuplicatingClassForReinstancing(GIsDuplicatingClassForReinstancing, true);
				DuplicatedStruct = (UUserDefinedStruct*)StaticDuplicateObject(StructureToReinstance, GetTransientPackage(), UniqueName, ~RF_Transactional);
			}

			// Remove the Standalone flag so that the duplicated struct can be cleaned up
			DuplicatedStruct->ClearFlags(RF_Standalone);

			DuplicatedStruct->Guid = StructureToReinstance->Guid;
			DuplicatedStruct->Bind();
			DuplicatedStruct->StaticLink(true);
			DuplicatedStruct->PrimaryStruct = StructureToReinstance;
			DuplicatedStruct->Status = EUserDefinedStructureStatus::UDSS_Duplicate;
			DuplicatedStruct->SetFlags(RF_Transient);
			DuplicatedStruct->AddToRoot();
			CastChecked<UUserDefinedStructEditorData>(DuplicatedStruct->EditorData)->RecreateDefaultInstance();

			for (auto StructProperty : TObjectRange<UStructProperty>(RF_ClassDefaultObject, /** bIncludeDerivedClasses */ true, /** InternalExcludeFlags */ EInternalObjectFlags::PendingKill))
			{
				if (StructProperty && (StructureToReinstance == StructProperty->Struct))
				{
					if (auto OwnerClass = Cast<UBlueprintGeneratedClass>(StructProperty->GetOwnerClass()))
					{
						if (UBlueprint* FoundBlueprint = Cast<UBlueprint>(OwnerClass->ClassGeneratedBy))
						{
							StructProperty->Struct = DuplicatedStruct;
							ClearStructReferencesInBP(FoundBlueprint, BlueprintsToRecompile);
						}
					}
					else if (auto OwnerStruct = Cast<UUserDefinedStruct>(StructProperty->GetOwnerStruct()))
					{
						check(OwnerStruct != DuplicatedStruct);
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

			for (auto Blueprint : TObjectRange<UBlueprint>(RF_ClassDefaultObject, /** bIncludeDerivedClasses */ true, /** InternalExcludeFlags */ EInternalObjectFlags::PendingKill))
			{
				if (Blueprint && !BlueprintsToRecompile.Contains(Blueprint))
				{
					FBlueprintEditorUtils::EnsureCachedDependenciesUpToDate(Blueprint);
					if (Blueprint->CachedUDSDependencies.Contains(StructureToReinstance))
					{
						ClearStructReferencesInBP(Blueprint, BlueprintsToRecompile);
					}
				}
			}
		}
	}

	static UObject* CleanAndSanitizeStruct(UUserDefinedStruct* StructToClean)
	{
		check(StructToClean);

		if (auto EditorData = Cast<UUserDefinedStructEditorData>(StructToClean->EditorData))
		{
			EditorData->CleanDefaultInstance();
		}

		UUserDefinedStruct* TransientStruct = nullptr;

		if (FStructureEditorUtils::FStructEditorManager::ActiveChange != FStructureEditorUtils::EStructureEditorChangeInfo::DefaultValueChanged)
		{
			const FString TransientString = FString::Printf(TEXT("TRASHSTRUCT_%s"), *StructToClean->GetName());
			const FName TransientName = MakeUniqueObjectName(GetTransientPackage(), UUserDefinedStruct::StaticClass(), FName(*TransientString));
			TransientStruct = NewObject<UUserDefinedStruct>(GetTransientPackage(), TransientName, RF_Public | RF_Transient);

			TArray<UObject*> SubObjects;
			GetObjectsWithOuter(StructToClean, SubObjects, true);
			SubObjects.Remove(StructToClean->EditorData);
			for (auto SubObjIt = SubObjects.CreateIterator(); SubObjIt; ++SubObjIt)
			{
				UObject* CurrSubObj = *SubObjIt;
				CurrSubObj->Rename(NULL, TransientStruct, REN_DontCreateRedirectors);
				if (UProperty* Prop = Cast<UProperty>(CurrSubObj))
				{
					FKismetCompilerUtilities::InvalidatePropertyExport(Prop);
				}
				else
				{
					FLinkerLoad::InvalidateExport(CurrSubObj);
				}
			}

			StructToClean->SetSuperStruct(NULL);
			StructToClean->Children = NULL;
			StructToClean->Script.Empty();
			StructToClean->MinAlignment = 0;
			StructToClean->RefLink = NULL;
			StructToClean->PropertyLink = NULL;
			StructToClean->DestructorLink = NULL;
			StructToClean->ScriptObjectReferences.Empty();
			StructToClean->PropertyLink = NULL;
			StructToClean->ErrorMessage.Empty();
		}

		return TransientStruct;
	}

	static void LogError(UUserDefinedStruct* Struct, FCompilerResultsLog& MessageLog, const FString& ErrorMsg)
	{
		MessageLog.Error(*ErrorMsg);
		if (Struct && Struct->ErrorMessage.IsEmpty())
		{
			Struct->ErrorMessage = ErrorMsg;
		}
	}

	static void CreateVariables(UUserDefinedStruct* Struct, const class UEdGraphSchema_K2* Schema, FCompilerResultsLog& MessageLog)
	{
		check(Struct && Schema);

		//FKismetCompilerUtilities::LinkAddedProperty push property to begin, so we revert the order
		for (int32 VarDescIdx = FStructureEditorUtils::GetVarDesc(Struct).Num() - 1; VarDescIdx >= 0; --VarDescIdx)
		{
			FStructVariableDescription& VarDesc = FStructureEditorUtils::GetVarDesc(Struct)[VarDescIdx];
			VarDesc.bInvalidMember = true;

			FEdGraphPinType VarType = VarDesc.ToPinType();

			FString ErrorMsg;
			if (!FStructureEditorUtils::CanHaveAMemberVariableOfType(Struct, VarType, &ErrorMsg))
			{
				LogError(Struct, MessageLog, FString::Printf(*LOCTEXT("StructureGeneric_Error", "Structure: %s Error: %s").ToString(), *Struct->GetFullName(), *ErrorMsg));
				continue;
			}

			UProperty* VarProperty = nullptr;

			bool bIsNewVariable = false;
			if (FStructureEditorUtils::FStructEditorManager::ActiveChange == FStructureEditorUtils::EStructureEditorChangeInfo::DefaultValueChanged)
			{
				VarProperty = FindField<UProperty>(Struct, VarDesc.VarName);
				if (!ensureMsgf(VarProperty, TEXT("Could not find the expected property (%s); was the struct (%s) unexpectedly sanitized?"), *VarDesc.VarName.ToString(), *Struct->GetName()))
				{
					VarProperty = FKismetCompilerUtilities::CreatePropertyOnScope(Struct, VarDesc.VarName, VarType, NULL, 0, Schema, MessageLog);
					bIsNewVariable = true;
				}
			}
			else
			{
				VarProperty = FKismetCompilerUtilities::CreatePropertyOnScope(Struct, VarDesc.VarName, VarType, NULL, 0, Schema, MessageLog);
				bIsNewVariable = true;
			}

			if (VarProperty == nullptr)
			{
				LogError(Struct, MessageLog, FString::Printf(*LOCTEXT("VariableInvalidType_Error", "The variable %s declared in %s has an invalid type %s").ToString(),
					*VarDesc.VarName.ToString(), *Struct->GetName(), *UEdGraphSchema_K2::TypeToText(VarType).ToString()));
				continue;
			}
			else if (bIsNewVariable)
			{
				VarProperty->SetFlags(RF_LoadCompleted);
				FKismetCompilerUtilities::LinkAddedProperty(Struct, VarProperty);
			}

			VarProperty->SetPropertyFlags(CPF_Edit | CPF_BlueprintVisible);
			if (VarDesc.bDontEditoOnInstance)
			{
				VarProperty->SetPropertyFlags(CPF_DisableEditOnInstance);
			}
			if (VarDesc.bEnableMultiLineText)
			{
				VarProperty->SetMetaData("MultiLine", TEXT("true"));
			}
			if (VarDesc.bEnable3dWidget)
			{
				VarProperty->SetMetaData(FEdMode::MD_MakeEditWidget, TEXT("true"));
			}
			VarProperty->SetMetaData(TEXT("DisplayName"), *VarDesc.FriendlyName);
			VarProperty->SetMetaData(FBlueprintMetadata::MD_Tooltip, *VarDesc.ToolTip);
			VarProperty->RepNotifyFunc = NAME_None;

			if (!VarDesc.DefaultValue.IsEmpty())
			{
				VarProperty->SetMetaData(TEXT("MakeStructureDefaultValue"), *VarDesc.DefaultValue);
			}
			VarDesc.CurrentDefaultValue = VarDesc.DefaultValue;

			VarDesc.bInvalidMember = false;

			if (VarProperty->HasAnyPropertyFlags(CPF_InstancedReference | CPF_ContainsInstancedReference))
			{
				Struct->StructFlags = EStructFlags(Struct->StructFlags | STRUCT_HasInstancedReference);
			}

			if (VarType.PinSubCategoryObject.IsValid())
			{
				const UClass* ClassObject = Cast<UClass>(VarType.PinSubCategoryObject.Get());

				if (ClassObject && ClassObject->IsChildOf(AActor::StaticClass()))
				{
					// prevent Actor variables from having default values (because Blueprint templates are library elements that can 
					// bridge multiple levels and different levels might not have the actor that the default is referencing).
					VarProperty->PropertyFlags |= CPF_DisableEditOnTemplate;
				}
				else
				{
					// clear the disable-default-value flag that might have been present (if this was an AActor variable before)
					VarProperty->PropertyFlags &= ~(CPF_DisableEditOnTemplate);
				}
			}
		}
	}

	static void InnerCompileStruct(UUserDefinedStruct* Struct, const class UEdGraphSchema_K2* K2Schema, class FCompilerResultsLog& MessageLog)
	{
		check(Struct);
		const int32 ErrorNum = MessageLog.NumErrors;

		Struct->SetMetaData(FBlueprintMetadata::MD_Tooltip, *FStructureEditorUtils::GetTooltip(Struct));

		auto EditorData = CastChecked<UUserDefinedStructEditorData>(Struct->EditorData);
		Struct->SetSuperStruct(EditorData->NativeBase);

		CreateVariables(Struct, K2Schema, MessageLog);

		Struct->Bind();
		Struct->StaticLink(true);

		if (Struct->GetStructureSize() <= 0)
		{
			LogError(Struct, MessageLog, FString::Printf(*LOCTEXT("StructurEmpty_Error", "Structure '%s' is empty ").ToString(), *Struct->GetFullName()));
		}

		FString DefaultInstanceError;
		EditorData->RecreateDefaultInstance(&DefaultInstanceError);
		if (!DefaultInstanceError.IsEmpty())
		{
			LogError(Struct, MessageLog, DefaultInstanceError);
		}

		const bool bNoErrorsDuringCompilation = (ErrorNum == MessageLog.NumErrors);
		Struct->Status = bNoErrorsDuringCompilation ? EUserDefinedStructureStatus::UDSS_UpToDate : EUserDefinedStructureStatus::UDSS_Error;
	}

	static bool ShouldBeCompiled(const UUserDefinedStruct* Struct)
	{
		if (Struct && (EUserDefinedStructureStatus::UDSS_UpToDate == Struct->Status))
		{
			return false;
		}
		return true;
	}

	static void BuildDependencyMapAndCompile(const TArray<UUserDefinedStruct*>& ChangedStructs, FCompilerResultsLog& MessageLog)
	{
		struct FDependencyMapEntry
		{
			UUserDefinedStruct* Struct;
			TSet<UUserDefinedStruct*> StructuresToWaitFor;

			FDependencyMapEntry() : Struct(NULL) {}

			void Initialize(UUserDefinedStruct* ChangedStruct, const TArray<UUserDefinedStruct*>& AllChangedStructs)
			{
				Struct = ChangedStruct;
				check(Struct);

				auto Schema = GetDefault<UEdGraphSchema_K2>();
				for (auto& VarDesc : FStructureEditorUtils::GetVarDesc(Struct))
				{
					auto StructType = Cast<UUserDefinedStruct>(VarDesc.SubCategoryObject.Get());
					if (StructType && (VarDesc.Category == Schema->PC_Struct) && AllChangedStructs.Contains(StructType))
					{
						StructuresToWaitFor.Add(StructType);
					}
				}
			}
		};

		TArray<FDependencyMapEntry> DependencyMap;
		for (auto Iter = ChangedStructs.CreateConstIterator(); Iter; ++Iter)
		{
			DependencyMap.Add(FDependencyMapEntry());
			DependencyMap.Last().Initialize(*Iter, ChangedStructs);
		}

		while (DependencyMap.Num())
		{
			int32 StructureToCompileIndex = INDEX_NONE;
			for (int32 EntryIndex = 0; EntryIndex < DependencyMap.Num(); ++EntryIndex)
			{
				if (0 == DependencyMap[EntryIndex].StructuresToWaitFor.Num())
				{
					StructureToCompileIndex = EntryIndex;
					break;
				}
			}
			check(INDEX_NONE != StructureToCompileIndex);
			UUserDefinedStruct* Struct = DependencyMap[StructureToCompileIndex].Struct;
			check(Struct);

			FUserDefinedStructureCompilerInner::CleanAndSanitizeStruct(Struct);
			FUserDefinedStructureCompilerInner::InnerCompileStruct(Struct, GetDefault<UEdGraphSchema_K2>(), MessageLog);

			DependencyMap.RemoveAtSwap(StructureToCompileIndex);

			for (auto EntryIter = DependencyMap.CreateIterator(); EntryIter; ++EntryIter)
			{
				(*EntryIter).StructuresToWaitFor.Remove(Struct);
			}
		}
	}

	// Only change made to FUserDefinedStructureCompilerInner in order to pre compile / compile structs
	// in a way which doesn't conflict with C# defined structs
	static void PreCompileStructs(TSet<UBlueprint*>& BlueprintsToRecompile, TArray<UUserDefinedStruct*>& ChangedStructs)
	{
		for (int32 StructIdx = 0; StructIdx < ChangedStructs.Num(); ++StructIdx)
		{
			UUserDefinedStruct* ChangedStruct = ChangedStructs[StructIdx];
			if (ChangedStruct)
			{
				ChangedStruct->Status = EUserDefinedStructureStatus::UDSS_Dirty;
				FStructureEditorUtils::BroadcastPreChange(ChangedStruct);
				FUserDefinedStructureCompilerInner::ReplaceStructWithTempDuplicate(ChangedStruct, BlueprintsToRecompile, ChangedStructs);
			}
		}
	}

	static void CompileStructs(TArray<UUserDefinedStruct*>& ChangedStructs)
	{
		FCompilerResultsLog Results;

		// COMPILE IN PROPER ORDER
		FUserDefinedStructureCompilerInner::BuildDependencyMapAndCompile(ChangedStructs, Results);
	}
};



























































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

	DuplicatedStruct->Guid = Guid;
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

	FUserDefinedStructureCompilerInner::PreCompileStructs(**OutBlueprintsToRecompile, **OutChangedStructs);
}

void SharpHotReloadUtils_PostUpdateStructs(TArray<UScriptStruct*>& SharpChangedStructs, TSet<UBlueprint*>* InBlueprintsToRecompile, TArray<UUserDefinedStruct*>* InChangedStructs)
{
	TSet<UBlueprint*>& BlueprintsToRecompile = *InBlueprintsToRecompile;
	TArray<UUserDefinedStruct*>& ChangedStructs = *InChangedStructs;

	// Compile editor defined structs
	FUserDefinedStructureCompilerInner::CompileStructs(ChangedStructs);

	// UPDATE ALL THINGS DEPENDENT ON COMPILED STRUCTURES
	TSet<UBlueprint*> BlueprintsThatHaveBeenRecompiled;
	for (TObjectIterator<UK2Node> It(RF_Transient | RF_ClassDefaultObject, /** bIncludeDerivedClasses */ true, /** InternalExcludeFlags */ EInternalObjectFlags::PendingKill); It && SharpChangedStructs.Num(); ++It)
	{
		bool bReconstruct = false;

		UK2Node* Node = *It;

		if (Node && !Node->HasAnyFlags(RF_Transient) && !Node->IsPendingKill())
		{
			// If this is a struct operation node operation on the changed struct we must reconstruct
			if (UK2Node_StructOperation* StructOpNode = Cast<UK2Node_StructOperation>(Node))
			{
				UScriptStruct* StructInNode = Cast<UScriptStruct>(StructOpNode->StructType);
				if (StructInNode && (SharpChangedStructs.Contains(StructInNode) || ChangedStructs.Contains(StructInNode)))
				{
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
						if (StructType && (SharpChangedStructs.Contains(StructType) || ChangedStructs.Contains(StructType)))
						{
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

	// From FStructureEditorUtils::BroadcastPostChange (Engine\Source\Editor\UnrealEd\Private\Kismet2\StructureEditorUtils.cpp)
	for (UScriptStruct* Struct : SharpChangedStructs)
	{
		TArray<UDataTable*> DataTables = FReinstanceDataTableHelper::GetTablesDependentOnStruct(Struct);
		for (UDataTable* DataTable : DataTables)
		{
			DataTable->RestoreAfterStructChange();
		}
	}

	// Broadcast editor struct changed
	for (auto ChangedStruct : ChangedStructs)
	{
		if (ChangedStruct)
		{
			FStructureEditorUtils::BroadcastPostChange(ChangedStruct);
			ChangedStruct->MarkPackageDirty();
		}
	}

	delete InBlueprintsToRecompile;
	delete InChangedStructs;
}
#undef LOCTEXT_NAMESPACE
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
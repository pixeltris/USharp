#if WITH_EDITOR

// Based on FUserDefinedStructureCompilerUtils::CompileStruct
void SharpHotReloadUtils_UpdateDelegates(TArray<UFunction*>& Delegates)
{
	if (Delegates.Num() == 0)
	{
		return;
	}

	TMap<FName, UFunction*> DelegatesByName;
	for (UFunction* Delegate : Delegates)
	{		
		FName DelegateName = Delegate->GetFName();
		check(!DelegatesByName.Contains(DelegateName));// Delegate names are expected to be unique
		DelegatesByName.Add(DelegateName, Delegate);
	}

	// FBlueprintEditorUtils::UpdateDelegatesInBlueprint might be useful here?

	TSet<UBlueprint*> BlueprintsThatHaveBeenRecompiled;
	for (TObjectIterator<UK2Node> It(RF_Transient | RF_ClassDefaultObject, /** bIncludeDerivedClasses */ true, /** InternalExcludeFlags */ EInternalObjectFlags::PendingKill); It; ++It)
	{
		bool bReconstruct = false;

		UK2Node* Node = *It;

		if (Node && !Node->HasAnyFlags(RF_Transient) && !Node->IsPendingKill())
		{
			UFunction* Function = nullptr;

			if (UK2Node_Event* EventNode = Cast<UK2Node_Event>(Node))
			{
				Function = EventNode->EventReference.ResolveMember<UFunction>(Node->GetBlueprintClassFromNode());
			}
			else if (UK2Node_CallFunction* CallNode = Cast<UK2Node_CallFunction>(Node))
			{
				Function = CallNode->FunctionReference.ResolveMember<UFunction>(Node->GetBlueprintClassFromNode());
			}
			else if (UK2Node_BaseMCDelegate* DelegateNode = Cast<UK2Node_BaseMCDelegate>(Node))
			{
				if (UK2Node_CallDelegate* CallDelegateNode = Cast<UK2Node_CallDelegate>(Node))
				{
					Function = DelegateNode->GetDelegateSignature(false);
				}
				else if (Cast<UK2Node_AddDelegate>(Node) || Cast<UK2Node_AddDelegate>(Node))
				{
					// Rough copy of FParamsChangedHelper::EditDelegates (here we don't reconstruct the main node as there shouldn't be
					// any visual changes which need updating)
					if (UEdGraphPin* DelegateInPin = DelegateNode->GetDelegatePin())
					{
						for (UEdGraphPin* DelegateOutPin : DelegateInPin->LinkedTo)
						{
							if (UK2Node_CustomEvent* CustomEventNode = (DelegateOutPin ? Cast<UK2Node_CustomEvent>(DelegateOutPin->GetOwningNode()) : nullptr))
							{
								CustomEventNode->ReconstructNode();
							}
						}
					}
				}
			}
			
			if (Function != nullptr && Function->HasAllFunctionFlags(FUNC_Delegate))
			{
				UFunction** DelegatePtr = DelegatesByName.Find(Function->GetFName());
				if (DelegatePtr != nullptr)
				{
					UFunction* Delegate = *DelegatePtr;
					if (Delegate->GetOutermost() == Function->GetOutermost())
					{
						bReconstruct = true;
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
					FBlueprintEditorUtils::MarkBlueprintAsStructurallyModified(FoundBlueprint);
				}
				Node->ReconstructNode();
			}
		}
	}
}

#endif
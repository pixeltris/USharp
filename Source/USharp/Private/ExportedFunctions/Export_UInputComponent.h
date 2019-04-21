CSEXPORT int32 CSCONV Export_UInputComponent_Get_Priority(UInputComponent* instance)
{
	return instance->Priority;
}

CSEXPORT void CSCONV Export_UInputComponent_Set_Priority(UInputComponent* instance, int32 value)
{
	instance->Priority = value;
}

CSEXPORT csbool CSCONV Export_UInputComponent_Get_bBlockInput(UInputComponent* instance)
{
	return instance->Priority;
}

CSEXPORT void CSCONV Export_UInputComponent_Set_bBlockInput(UInputComponent* instance, csbool value)
{
	instance->bBlockInput = value;
}

CSEXPORT void CSCONV Export_UInputComponent_ConditionalBuildKeyMap(UInputComponent* instance, UPlayerInput* PlayerInput)
{
	instance->ConditionalBuildKeyMap(PlayerInput);
}

CSEXPORT float CSCONV Export_UInputComponent_GetAxisValue(UInputComponent* instance, const FName& AxisName)
{
	return instance->GetAxisValue(AxisName);
}

CSEXPORT float CSCONV Export_UInputComponent_GetAxisKeyValue(UInputComponent* instance, const FKey& AxisKey)
{
	return instance->GetAxisKeyValue(AxisKey);
}

CSEXPORT void CSCONV Export_UInputComponent_GetVectorAxisValue(UInputComponent* instance, const FKey& AxisKey, FVector& result)
{
	result = instance->GetVectorAxisValue(AxisKey);
}

CSEXPORT csbool CSCONV Export_UInputComponent_HasBindings(UInputComponent* instance)
{
	return instance->HasBindings();
}

CSEXPORT FInputActionBinding& CSCONV Export_UInputComponent_AddActionBinding(UInputComponent* instance, const FInputActionBinding& Binding)
{
	return instance->AddActionBinding(Binding);
}

CSEXPORT void CSCONV Export_UInputComponent_ClearActionBindings(UInputComponent* instance)
{
	instance->ClearActionBindings();
}

CSEXPORT FInputActionBinding& CSCONV Export_UInputComponent_GetActionBinding(UInputComponent* instance, int32 BindingIndex)
{
	return instance->GetActionBinding(BindingIndex);
}

CSEXPORT int32 CSCONV Export_UInputComponent_GetNumActionBindings(UInputComponent* instance)
{
	return instance->GetNumActionBindings();
}

CSEXPORT void CSCONV Export_UInputComponent_RemoveActionBinding(UInputComponent* instance, int32 BindingIndex)
{
	instance->RemoveActionBinding(BindingIndex);
}

CSEXPORT void CSCONV Export_UInputComponent_RemoveActionBindingByName(UInputComponent* instance, const FName& Name)
{
	int32 Count = instance->GetNumActionBindings();
	for (int32 Index = Count - 1; Index >= 0; --Index)
	{
		FInputActionBinding& Binding = instance->GetActionBinding(Index);
		if (Binding.GetActionName() == Name)
		{
			instance->RemoveActionBinding(Index);
		}
	}
}

CSEXPORT void CSCONV Export_UInputComponent_RemoveActionBindingByHandle(UInputComponent* instance, FInputActionBinding& Value)
{
	int32 Count = instance->GetNumActionBindings();
	for (int32 Index = Count - 1; Index >= 0; --Index)
	{
		FInputActionBinding& Binding = instance->GetActionBinding(Index);
		if (&Binding == &Value)
		{
			instance->RemoveActionBinding(Index);
		}
	}
}

CSEXPORT void CSCONV Export_UInputComponent_ClearBindingValues(UInputComponent* instance)
{
	instance->ClearBindingValues();
}

CSEXPORT FInputActionBinding& CSCONV Export_UInputComponent_BindAction(UInputComponent* instance, const FName& ActionName, uint8 KeyEvent, UObject* Object, UFunction* Func)
{
	FInputActionBinding AB(ActionName, (EInputEvent)KeyEvent);
	AB.ActionDelegate.BindDelegate(Object, Func->GetFName());
	return instance->AddActionBinding(AB);
}

CSEXPORT FInputAxisBinding& CSCONV Export_UInputComponent_BindAxis(UInputComponent* instance, const FName& AxisName, UObject* Object, UFunction* Func)
{
	FInputAxisBinding AB(AxisName);
	AB.AxisDelegate.BindDelegate(Object, Func->GetFName());
	instance->AxisBindings.Add(AB);
	return instance->AxisBindings.Last();
}

CSEXPORT FInputAxisBinding& CSCONV Export_UInputComponent_BindAxisName(UInputComponent* instance, const FName& AxisName)
{
	return instance->BindAxis(AxisName);
}

CSEXPORT FInputVectorAxisBinding& CSCONV Export_UInputComponent_BindVectorAxis(UInputComponent* instance, const FKey& AxisKey, UObject* Object, UFunction* Func)
{
	FInputVectorAxisBinding AB(AxisKey);
	AB.AxisDelegate.BindDelegate(Object, Func->GetFName());
	instance->VectorAxisBindings.Add(AB);
	return instance->VectorAxisBindings.Last();
}

CSEXPORT FInputVectorAxisBinding& CSCONV Export_UInputComponent_BindVectorAxisKey(UInputComponent* instance, const FKey& AxisKey)
{
	return instance->BindVectorAxis(AxisKey);
}

CSEXPORT FInputKeyBinding& CSCONV Export_UInputComponent_BindKey(UInputComponent* instance, const FKey& Key, uint8 KeyEvent, UObject* Object, UFunction* Func)
{
	FInputKeyBinding KB(FInputChord(Key, false, false, false, false), (EInputEvent)KeyEvent);
	KB.KeyDelegate.BindDelegate(Object, Func->GetFName());
	instance->KeyBindings.Add(KB);
	return instance->KeyBindings.Last();
}

CSEXPORT FInputKeyBinding& CSCONV Export_UInputComponent_BindKeyChord(UInputComponent* instance, const FKey& Key, csbool bShift, csbool bCtrl, csbool bAlt, csbool bCmd, uint8 KeyEvent, UObject* Object, UFunction* Func)
{
	FInputKeyBinding KB(FInputChord(Key, (bool)bShift, (bool)bCtrl, (bool)bAlt, (bool)bCmd), (EInputEvent)KeyEvent);
	KB.KeyDelegate.BindDelegate(Object, Func->GetFName());
	instance->KeyBindings.Add(KB);
	return instance->KeyBindings.Last();
}

CSEXPORT FInputTouchBinding& CSCONV Export_UInputComponent_BindTouch(UInputComponent* instance, uint8 KeyEvent, UObject* Object, UFunction* Func)
{
	FInputTouchBinding TB((EInputEvent)KeyEvent);
	TB.TouchDelegate.BindDelegate(Object, Func->GetFName());
	instance->TouchBindings.Add(TB);
	return instance->TouchBindings.Last();
}

CSEXPORT FInputGestureBinding& CSCONV Export_UInputComponent_BindGesture(UInputComponent* instance, FKey& GestureKey, UObject* Object, UFunction* Func)
{
	FInputGestureBinding GB(GestureKey);
	GB.GestureDelegate.BindDelegate(Object, Func->GetFName());
	instance->GestureBindings.Add(GB);
	return instance->GestureBindings.Last();
}

CSEXPORT void CSCONV Export_UInputComponent(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UInputComponent_Get_Priority);
	REGISTER_FUNC(Export_UInputComponent_Set_Priority);
	REGISTER_FUNC(Export_UInputComponent_Get_bBlockInput);
	REGISTER_FUNC(Export_UInputComponent_Set_bBlockInput);
	REGISTER_FUNC(Export_UInputComponent_ConditionalBuildKeyMap);
	REGISTER_FUNC(Export_UInputComponent_GetAxisValue);
	REGISTER_FUNC(Export_UInputComponent_GetAxisKeyValue);
	REGISTER_FUNC(Export_UInputComponent_GetVectorAxisValue);
	REGISTER_FUNC(Export_UInputComponent_HasBindings);
	REGISTER_FUNC(Export_UInputComponent_AddActionBinding);
	REGISTER_FUNC(Export_UInputComponent_ClearActionBindings);
	REGISTER_FUNC(Export_UInputComponent_GetActionBinding);
	REGISTER_FUNC(Export_UInputComponent_GetNumActionBindings);
	REGISTER_FUNC(Export_UInputComponent_RemoveActionBinding);
	REGISTER_FUNC(Export_UInputComponent_RemoveActionBindingByName);
	REGISTER_FUNC(Export_UInputComponent_RemoveActionBindingByHandle);
	REGISTER_FUNC(Export_UInputComponent_ClearBindingValues);
	REGISTER_FUNC(Export_UInputComponent_BindAction);
	REGISTER_FUNC(Export_UInputComponent_BindAxis);
	REGISTER_FUNC(Export_UInputComponent_BindAxisName);
	REGISTER_FUNC(Export_UInputComponent_BindVectorAxis);
	REGISTER_FUNC(Export_UInputComponent_BindVectorAxisKey);
	REGISTER_FUNC(Export_UInputComponent_BindKey);
	REGISTER_FUNC(Export_UInputComponent_BindKeyChord);
	REGISTER_FUNC(Export_UInputComponent_BindTouch);
	REGISTER_FUNC(Export_UInputComponent_BindGesture);
}
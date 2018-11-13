CSEXPORT uint8 CSCONV Export_FInputKeyBinding_Get_KeyEvent(FInputKeyBinding* instance)
{
	return instance->KeyEvent;
}

CSEXPORT void CSCONV Export_FInputKeyBinding_Set_KeyEvent(FInputKeyBinding* instance, uint8 value)
{
	instance->KeyEvent = (TEnumAsByte<EInputEvent>)value;
}

CSEXPORT void CSCONV Export_FInputKeyBinding_Get_ChordEx(FInputKeyBinding* instance, FKey& OutKey, csbool& OutShift, csbool& OutCtrl, csbool& OutAlt, csbool& OutCmd)
{
	const FInputChord& Chord = instance->Chord;
	OutKey = Chord.Key;
	OutShift = Chord.bShift;
	OutCtrl = Chord.bCtrl;
	OutAlt = Chord.bAlt;
	OutCmd = Chord.bCmd;
}

CSEXPORT void CSCONV Export_FInputKeyBinding_Set_ChordEx(FInputKeyBinding* instance, FKey& InKey, csbool InShift, csbool InCtrl, csbool InAlt, csbool InCmd)
{
	FInputChord& Chord = instance->Chord;
	Chord.Key = InKey;
	Chord.bShift = InShift;
	Chord.bCtrl = InCtrl;
	Chord.bAlt = InAlt;
	Chord.bCmd = InCmd;
}

CSEXPORT FInputActionUnifiedDelegate& CSCONV Export_FInputKeyBinding_Get_KeyDelegate(FInputKeyBinding* instance)
{
	return instance->KeyDelegate;
}

CSEXPORT void CSCONV Export_FInputKeyBinding(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FInputKeyBinding_Get_KeyEvent);
	REGISTER_FUNC(Export_FInputKeyBinding_Set_KeyEvent);
	REGISTER_FUNC(Export_FInputKeyBinding_Get_ChordEx);
	REGISTER_FUNC(Export_FInputKeyBinding_Set_ChordEx);
	REGISTER_FUNC(Export_FInputKeyBinding_Get_KeyDelegate);
}
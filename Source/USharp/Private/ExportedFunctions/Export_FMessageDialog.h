CSEXPORT EAppReturnType::Type CSCONV Export_FMessageDialog_Open(EAppMsgType::Type MessageType, const FString& Message, const FString& OptTitle)
{
	FText MessageText = FText::FromString(Message);
	if (!OptTitle.IsEmpty())
	{
		FText OptTitleText = FText::FromString(OptTitle);
		return FMessageDialog::Open(MessageType, MessageText, &OptTitleText);
	}
	else
	{
		return FMessageDialog::Open(MessageType, MessageText, nullptr);
	}
}

CSEXPORT void CSCONV Export_FMessageDialog_Log(const FString& Message, const FString& CategoryName, ELogVerbosity::Type Verbosity)
{
	//Engine\Source\Runtime\Core\Public\Misc\AssertionMacros.h
	//Engine\Source\Runtime\Core\Public\Misc\OutputDevice.h
	//FMsg::Logf_Internal(__FILE__, __LINE__, CategoryName.GetCategoryName(), ELogVerbosity::Verbosity, Format, ##__VA_ARGS__);
	
	// __FILE__ __LINE__ not too useful here
	FMsg::Logf(__FILE__, __LINE__, FName(*CategoryName), Verbosity, TEXT("%s"), *Message);
}

CSEXPORT void CSCONV Export_FMessageDialog_FocusOutputLogTab()
{
	// Engine/Source/Developer/OutputLog/Private/OutputLogModule.cpp
	const FName OutputLogTab(TEXT("OutputLog"));
	FGlobalTabmanager::Get()->InvokeTab(OutputLogTab);
}

CSEXPORT void CSCONV Export_FMessageDialog(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FMessageDialog_Open);
	REGISTER_FUNC(Export_FMessageDialog_Log);
	REGISTER_FUNC(Export_FMessageDialog_FocusOutputLogTab);
}
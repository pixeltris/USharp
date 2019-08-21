CSEXPORT void CSCONV Export_FVariantData_FromStringEx(FVariantData* instance, FString& ValueStr)
{
	int32 Index;
	if (!ValueStr.FindChar('|', Index))
	{
		return;
	}
	FString Val = ValueStr.Left(Index);
	EOnlineKeyValuePairDataType::Type DataType = (EOnlineKeyValuePairDataType::Type)FCString::Atoi(*Val);
	ValueStr = ValueStr.RightChop(Index + 1);
	
	switch (DataType)
	{
		case EOnlineKeyValuePairDataType::Int32:
			{
				int32 IntVal = FCString::Atoi(*ValueStr);
				instance->SetValue(IntVal);
			}
			break;
		case EOnlineKeyValuePairDataType::UInt32:
			{
				uint64 IntVal = FCString::Strtoui64(*ValueStr, nullptr, 10);
				instance->SetValue((uint32)IntVal);
			}
			break;
		case EOnlineKeyValuePairDataType::Int64:
			{
				int64 IntVal = FCString::Atoi64(*ValueStr);
				instance->SetValue(IntVal);
			}
			break;
		case EOnlineKeyValuePairDataType::UInt64:
			{
				uint64 IntVal = FCString::Strtoui64(*ValueStr, nullptr, 10);
				instance->SetValue(IntVal);
			}
			break;
		case EOnlineKeyValuePairDataType::Double:
			{
				double DoubleVal = FCString::Atod(*ValueStr);
				instance->SetValue(DoubleVal);
			}
			break;
		case EOnlineKeyValuePairDataType::String:
			{
				instance->SetValue(ValueStr);
			}
			break;
		case EOnlineKeyValuePairDataType::Float:
			{
				double FloatVal = FCString::Atof(*ValueStr);
				instance->SetValue(FloatVal);
			}
			break;
		case EOnlineKeyValuePairDataType::Blob:
			{
				TArray<uint8> Buffer;
				TArray<FString> Splitted;
				ValueStr.ParseIntoArray(Splitted, TEXT(","), true);
				for (const FString& Str : Splitted)
				{
					Buffer.Add((uint8)FCString::Atoi(*Str));
				}
				instance->SetValue(Buffer);
			}
			break;
		case EOnlineKeyValuePairDataType::Json:
			{
				instance->SetJsonValueFromString(ValueStr);
			}
			break;
		case EOnlineKeyValuePairDataType::Bool:
			{
				instance->SetValue(ValueStr.Equals(TEXT("true"), ESearchCase::IgnoreCase));
			}
			break;
	}
}

CSEXPORT void CSCONV Export_FVariantData_ToStringEx(const FVariantData* instance, FString& ValueStr)
{
	ValueStr = FString::Printf(TEXT("%d|"), (int32)instance->GetType());
	switch (instance->GetType())
	{
		case EOnlineKeyValuePairDataType::Blob:
			{
				TArray<uint8> Buffer;
				instance->GetValue(Buffer);
				int32 Count = Buffer.Num();
				for (int32 i = 0; i < Count; i++)
				{
					ValueStr += FString::Printf(TEXT("%d"), (int32)Buffer[i]);
					if (i < Count - 1)
					{
						ValueStr += TEXT(",");
					}
				}
			}
			break;
		default:
			ValueStr += instance->ToString();
			break;
	}
}

CSEXPORT void CSCONV Export_FVariantData(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FVariantData_FromStringEx);
	REGISTER_FUNC(Export_FVariantData_ToStringEx);
}
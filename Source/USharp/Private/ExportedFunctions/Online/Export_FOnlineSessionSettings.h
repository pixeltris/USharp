CSEXPORT int32 CSCONV Export_FOnlineSessionSettings_Get_NumPublicConnections(FOnlineSessionSettings* instance)
{
	return instance->NumPublicConnections;
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings_Set_NumPublicConnections(FOnlineSessionSettings* instance, int32 value)
{
	instance->NumPublicConnections = value;
}

CSEXPORT int32 CSCONV Export_FOnlineSessionSettings_Get_NumPrivateConnections(FOnlineSessionSettings* instance)
{
	return instance->NumPrivateConnections;
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings_Set_NumPrivateConnections(FOnlineSessionSettings* instance, int32 value)
{
	instance->NumPrivateConnections = value;
}

CSEXPORT csbool CSCONV Export_FOnlineSessionSettings_Get_bShouldAdvertise(FOnlineSessionSettings* instance)
{
	return instance->bShouldAdvertise;
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings_Set_bShouldAdvertise(FOnlineSessionSettings* instance, csbool value)
{
	instance->bShouldAdvertise = (bool)value;
}

CSEXPORT csbool CSCONV Export_FOnlineSessionSettings_Get_bAllowJoinInProgress(FOnlineSessionSettings* instance)
{
	return instance->bAllowJoinInProgress;
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings_Set_bAllowJoinInProgress(FOnlineSessionSettings* instance, csbool value)
{
	instance->bAllowJoinInProgress = (bool)value;
}

CSEXPORT csbool CSCONV Export_FOnlineSessionSettings_Get_bIsLANMatch(FOnlineSessionSettings* instance)
{
	return instance->bIsLANMatch;
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings_Set_bIsLANMatch(FOnlineSessionSettings* instance, csbool value)
{
	instance->bIsLANMatch = (bool)value;
}

CSEXPORT csbool CSCONV Export_FOnlineSessionSettings_Get_bIsDedicated(FOnlineSessionSettings* instance)
{
	return instance->bIsDedicated;
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings_Set_bIsDedicated(FOnlineSessionSettings* instance, csbool value)
{
	instance->bIsDedicated = (bool)value;
}

CSEXPORT csbool CSCONV Export_FOnlineSessionSettings_Get_bUsesStats(FOnlineSessionSettings* instance)
{
	return instance->bUsesStats;
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings_Set_bUsesStats(FOnlineSessionSettings* instance, csbool value)
{
	instance->bUsesStats = (bool)value;
}

CSEXPORT csbool CSCONV Export_FOnlineSessionSettings_Get_bAllowInvites(FOnlineSessionSettings* instance)
{
	return instance->bAllowInvites;
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings_Set_bAllowInvites(FOnlineSessionSettings* instance, csbool value)
{
	instance->bAllowInvites = (bool)value;
}

CSEXPORT csbool CSCONV Export_FOnlineSessionSettings_Get_bUsesPresence(FOnlineSessionSettings* instance)
{
	return instance->bUsesPresence;
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings_Set_bUsesPresence(FOnlineSessionSettings* instance, csbool value)
{
	instance->bUsesPresence = (bool)value;
}

CSEXPORT csbool CSCONV Export_FOnlineSessionSettings_Get_bAllowJoinViaPresence(FOnlineSessionSettings* instance)
{
	return instance->bAllowJoinViaPresence;
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings_Set_bAllowJoinViaPresence(FOnlineSessionSettings* instance, csbool value)
{
	instance->bAllowJoinViaPresence = (bool)value;
}

CSEXPORT csbool CSCONV Export_FOnlineSessionSettings_Get_bAllowJoinViaPresenceFriendsOnly(FOnlineSessionSettings* instance)
{
	return instance->bAllowJoinViaPresenceFriendsOnly;
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings_Set_bAllowJoinViaPresenceFriendsOnly(FOnlineSessionSettings* instance, csbool value)
{
	instance->bAllowJoinViaPresenceFriendsOnly = (bool)value;
}

CSEXPORT csbool CSCONV Export_FOnlineSessionSettings_Get_bAntiCheatProtected(FOnlineSessionSettings* instance)
{
	return instance->bAntiCheatProtected;
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings_Set_bAntiCheatProtected(FOnlineSessionSettings* instance, csbool value)
{
	instance->bAntiCheatProtected = (bool)value;
}

CSEXPORT int32 CSCONV Export_FOnlineSessionSettings_Get_BuildUniqueId(FOnlineSessionSettings* instance)
{
	return instance->BuildUniqueId;
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings_Set_BuildUniqueId(FOnlineSessionSettings* instance, int32 value)
{
	instance->BuildUniqueId = (bool)value;
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings_Get_Settings(FOnlineSessionSettings* instance, TArray<FString>& keys, TArray<FString>& values)
{
	for (const TPair<FName, FOnlineSessionSetting>& Setting : instance->Settings)
	{
		keys.Add(Setting.Key.ToString());
		FString ValueStr = FString::Printf(TEXT("%d|%d|%d|"), (int32)Setting.Value.AdvertisementType, Setting.Value.ID, (int32)Setting.Value.Data.GetType());
		
		FString ValueDataStr;
		Export_FVariantData_ToStringEx(&Setting.Value.Data, ValueDataStr);
		ValueStr += ValueDataStr;
	}
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings_Set_Settings(FOnlineSessionSettings* instance, TArray<FString>& keys, TArray<FString>& values)
{
	instance->Settings.Empty();
	if (keys.Num() == values.Num())
	{
		int32 Count = keys.Num();
		for (int32 i = 0; i < Count; i++)
		{
			FOnlineSessionSetting Setting;
		
			FName KeyName(*keys[i]);
			FString ValueStr = values[i];
			int32 Index;
			
			if (!ValueStr.FindChar('|', Index))
			{
				continue;
			}
			FString Val = ValueStr.Left(Index);
			Setting.AdvertisementType = (EOnlineDataAdvertisementType::Type)FCString::Atoi(*Val);
			ValueStr = ValueStr.RightChop(Index + 1);
			
			if (!ValueStr.FindChar('|', Index))
			{
				continue;
			}
			Val = ValueStr.Left(Index);
			Setting.ID = FCString::Atoi(*Val);
			ValueStr = ValueStr.RightChop(Index + 1);
			
			Export_FVariantData_FromStringEx(&Setting.Data, ValueStr);
			
			instance->Settings.Add(KeyName, Setting);
		}
	}
}

CSEXPORT void CSCONV Export_FOnlineSessionSettings(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FOnlineSessionSettings_Get_NumPublicConnections);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Set_NumPublicConnections);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Get_NumPrivateConnections);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Set_NumPrivateConnections);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Get_bShouldAdvertise);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Set_bShouldAdvertise);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Get_bAllowJoinInProgress);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Set_bAllowJoinInProgress);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Get_bIsLANMatch);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Set_bIsLANMatch);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Get_bIsDedicated);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Set_bIsDedicated);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Get_bUsesStats);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Set_bUsesStats);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Get_bAllowInvites);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Set_bAllowInvites);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Get_bUsesPresence);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Set_bUsesPresence);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Get_bAllowJoinViaPresence);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Set_bAllowJoinViaPresence);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Get_bAllowJoinViaPresenceFriendsOnly);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Set_bAllowJoinViaPresenceFriendsOnly);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Get_bAntiCheatProtected);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Set_bAntiCheatProtected);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Get_BuildUniqueId);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Set_BuildUniqueId);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Get_Settings);
	REGISTER_FUNC(Export_FOnlineSessionSettings_Set_Settings);
}
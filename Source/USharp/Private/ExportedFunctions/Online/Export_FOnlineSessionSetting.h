CSEXPORT FVariantData* CSCONV Export_FOnlineSessionSetting_Get_Data(FOnlineSessionSetting* instance)
{
	return &instance->Data;
}

CSEXPORT int32 CSCONV Export_FOnlineSessionSetting_Get_AdvertisementType(FOnlineSessionSetting* instance)
{
	return (int32)instance->AdvertisementType;
}

CSEXPORT void CSCONV Export_FOnlineSessionSetting_Set_AdvertisementType(FOnlineSessionSetting* instance, int32 value)
{
	instance->AdvertisementType = (EOnlineDataAdvertisementType::Type)value;
}

CSEXPORT int32 CSCONV Export_FOnlineSessionSetting_Get_ID(FOnlineSessionSetting* instance)
{
	return instance->ID;
}

CSEXPORT void CSCONV Export_FOnlineSessionSetting_Set_ID(FOnlineSessionSetting* instance, int32 value)
{
	instance->ID = value;
}

CSEXPORT void CSCONV Export_FOnlineSessionSetting(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FOnlineSessionSetting_Get_Data);
	REGISTER_FUNC(Export_FOnlineSessionSetting_Get_AdvertisementType);
	REGISTER_FUNC(Export_FOnlineSessionSetting_Set_AdvertisementType);
	REGISTER_FUNC(Export_FOnlineSessionSetting_Get_ID);
	REGISTER_FUNC(Export_FOnlineSessionSetting_Set_ID);
}
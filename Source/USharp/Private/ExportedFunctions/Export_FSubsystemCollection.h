CSEXPORT void CSCONV Export_FSubsystemCollection_Initialize(FSubsystemCollectionBase& instance)
{
	instance.Initialize();
}

CSEXPORT void CSCONV Export_FSubsystemCollection_Deinitialize(FSubsystemCollectionBase& instance)
{
	instance.Deinitialize();
}

CSEXPORT csbool CSCONV Export_FSubsystemCollection_InitializeDependency(FSubsystemCollectionBase& instance, TSubclassOf<USubsystem> SubsystemClass)
{
	return instance.InitializeDependency(SubsystemClass);
}

CSEXPORT USubsystem* CSCONV Export_FSubsystemCollection_GetSubsystem(FSubsystemCollection<USubsystem>& instance, TSubclassOf<USubsystem> SubsystemClass)
{
	return instance.GetSubsystem(SubsystemClass);
}

CSEXPORT void CSCONV Export_FSubsystemCollection(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FSubsystemCollection_Initialize);
	REGISTER_FUNC(Export_FSubsystemCollection_Deinitialize);
	REGISTER_FUNC(Export_FSubsystemCollection_InitializeDependency);
	REGISTER_FUNC(Export_FSubsystemCollection_GetSubsystem);
}

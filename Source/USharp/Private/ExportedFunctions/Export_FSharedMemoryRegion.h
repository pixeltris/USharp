CSEXPORT void CSCONV Export_FSharedMemoryRegion_GetName(FGenericPlatformMemory::FSharedMemoryRegion* instance, FString& result)
{
	result = FString(instance->GetName());
}

CSEXPORT void* CSCONV Export_FSharedMemoryRegion_GetAddress(FGenericPlatformMemory::FSharedMemoryRegion* instance)
{
	return instance->GetAddress();
}

CSEXPORT SIZE_T CSCONV Export_FSharedMemoryRegion_GetSize(FGenericPlatformMemory::FSharedMemoryRegion* instance)
{
	return instance->GetSize();
}

CSEXPORT void CSCONV Export_FSharedMemoryRegion(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FSharedMemoryRegion_GetName);
	REGISTER_FUNC(Export_FSharedMemoryRegion_GetAddress);
	REGISTER_FUNC(Export_FSharedMemoryRegion_GetSize);
}
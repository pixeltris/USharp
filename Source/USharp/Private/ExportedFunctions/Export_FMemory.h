CSEXPORT void* CSCONV Export_FMemory_Memmove(void* Dest, const void* Src, uint64 Count)
{
	return FMemory::Memmove(Dest, Src, (SIZE_T)Count);
}

CSEXPORT int32 CSCONV Export_FMemory_Memcmp(const void* Buf1, const void* Buf2, uint64 Count)
{
	return FMemory::Memcmp(Buf1, Buf2, (SIZE_T)Count);
}

CSEXPORT void* CSCONV Export_FMemory_Memset(void* Dest, uint8 Char, uint64 Count)
{
	return FMemory::Memset(Dest, Char, (SIZE_T)Count);
}

CSEXPORT void* CSCONV Export_FMemory_Memzero(void* Dest, uint64 Count)
{
	return FMemory::Memzero(Dest, (SIZE_T)Count);
}

CSEXPORT void* CSCONV Export_FMemory_Memcpy(void* Dest, const void* Src, uint64 Count)
{
	return FMemory::Memcpy(Dest, Src, (SIZE_T)Count);
}

CSEXPORT void* CSCONV Export_FMemory_BigBlockMemcpy(void* Dest, const void* Src, uint64 Count)
{
	return FMemory::BigBlockMemcpy(Dest, Src, (SIZE_T)Count);
}

CSEXPORT void* CSCONV Export_FMemory_StreamingMemcpy(void* Dest, const void* Src, uint64 Count)
{
	return FMemory::StreamingMemcpy(Dest, Src, (SIZE_T)Count);
}

CSEXPORT void CSCONV Export_FMemory_Memswap(void* Ptr1, void* Ptr2, uint64 Size)
{
	FMemory::Memswap(Ptr1, Ptr2, (SIZE_T)Size);
}

CSEXPORT void* CSCONV Export_FMemory_SystemMalloc(uint64 Size)
{
	return FMemory::SystemMalloc((SIZE_T)Size);
}

CSEXPORT void CSCONV Export_FMemory_SystemFree(void* Ptr)
{
	FMemory::SystemFree(Ptr);
}

CSEXPORT void* CSCONV Export_FMemory_Malloc(uint64 Count, uint32 Alignment)
{
	return FMemory::Malloc((SIZE_T)Count, Alignment);
}

CSEXPORT void* CSCONV Export_FMemory_Realloc(void* Original, uint64 Count, uint32 Alignment)
{
	return FMemory::Realloc(Original, (SIZE_T)Count, Alignment);
}

CSEXPORT void CSCONV Export_FMemory_Free(void* Original)
{
	FMemory::Free(Original);
}

CSEXPORT uint64 CSCONV Export_FMemory_GetAllocSize(void* Original)
{
	return FMemory::GetAllocSize(Original);
}

CSEXPORT uint64 CSCONV Export_FMemory_QuantizeSize(uint64 Count, uint32 Alignment)
{
	return FMemory::QuantizeSize(Count, Alignment);
}

CSEXPORT void CSCONV Export_FMemory_Trim()
{
	FMemory::Trim();
}

CSEXPORT void CSCONV Export_FMemory_SetupTLSCachesOnCurrentThread()
{
	FMemory::SetupTLSCachesOnCurrentThread();
}

CSEXPORT void CSCONV Export_FMemory_ClearAndDisableTLSCachesOnCurrentThread()
{
	FMemory::ClearAndDisableTLSCachesOnCurrentThread();
}

CSEXPORT void* CSCONV Export_FMemory_GPUMalloc(uint64 Count, uint32 Alignment)
{	
	return FMemory::GPUMalloc(Count, Alignment);
}

CSEXPORT void* CSCONV Export_FMemory_GPURealloc(void* Original, uint64 Count, uint32 Alignment)
{
	return FMemory::GPURealloc(Original, Count, Alignment);
}

CSEXPORT void CSCONV Export_FMemory_GPUFree(void* Original)
{
	FMemory::GPUFree(Original);
}

CSEXPORT void CSCONV Export_FMemory_TestMemory()
{
	FMemory::TestMemory();
}

CSEXPORT void CSCONV Export_FMemory_EnablePurgatoryTests()
{
	FMemory::EnablePurgatoryTests();
}

// There are also a few useful functions in FPlatformMemory

CSEXPORT csbool CSCONV Export_FMemory_PageProtect(void* const Ptr, const SIZE_T Size, const csbool bCanRead, const csbool bCanWrite)
{
	return FPlatformMemory::PageProtect(Ptr, Size, bCanRead, bCanWrite);
}

CSEXPORT FGenericPlatformMemory::FSharedMemoryRegion* CSCONV Export_FMemory_MapNamedSharedMemoryRegion(const FString& Name, csbool bCreate, uint32 AccessMode, SIZE_T Size)
{
	return FPlatformMemory::MapNamedSharedMemoryRegion(Name, bCreate, AccessMode, Size);
}

CSEXPORT csbool CSCONV Export_FMemory_UnmapNamedSharedMemoryRegion(FGenericPlatformMemory::FSharedMemoryRegion* MemoryRegion)
{
	return FPlatformMemory::UnmapNamedSharedMemoryRegion(MemoryRegion);
}

CSEXPORT void CSCONV Export_FMemory(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FMemory_Memmove);
	REGISTER_FUNC(Export_FMemory_Memcmp);
	REGISTER_FUNC(Export_FMemory_Memset);
	REGISTER_FUNC(Export_FMemory_Memzero);
	REGISTER_FUNC(Export_FMemory_Memcpy);
	REGISTER_FUNC(Export_FMemory_BigBlockMemcpy);
	REGISTER_FUNC(Export_FMemory_StreamingMemcpy);
	REGISTER_FUNC(Export_FMemory_Memswap);
	REGISTER_FUNC(Export_FMemory_SystemMalloc);
	REGISTER_FUNC(Export_FMemory_SystemFree);
	REGISTER_FUNC(Export_FMemory_Malloc);
	REGISTER_FUNC(Export_FMemory_Realloc);
	REGISTER_FUNC(Export_FMemory_Free);
	REGISTER_FUNC(Export_FMemory_GetAllocSize);
	REGISTER_FUNC(Export_FMemory_QuantizeSize);
	REGISTER_FUNC(Export_FMemory_Trim);
	REGISTER_FUNC(Export_FMemory_SetupTLSCachesOnCurrentThread);
	REGISTER_FUNC(Export_FMemory_ClearAndDisableTLSCachesOnCurrentThread);
	REGISTER_FUNC(Export_FMemory_GPUMalloc);
	REGISTER_FUNC(Export_FMemory_GPURealloc);
	REGISTER_FUNC(Export_FMemory_GPUFree);
	REGISTER_FUNC(Export_FMemory_TestMemory);
	REGISTER_FUNC(Export_FMemory_EnablePurgatoryTests);
	REGISTER_FUNC(Export_FMemory_PageProtect);
	REGISTER_FUNC(Export_FMemory_MapNamedSharedMemoryRegion);
	REGISTER_FUNC(Export_FMemory_UnmapNamedSharedMemoryRegion);
}
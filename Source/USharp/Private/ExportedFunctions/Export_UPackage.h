CSEXPORT void CSCONV Export_UPackage_Get_FileName(UPackage* instance, FName& result)
{
	result = instance->FileName;
}

CSEXPORT void CSCONV Export_UPackage_Set_FileName(UPackage* instance, const FName& value)
{
	instance->FileName = value;
}

#if WITH_EDITORONLY_DATA
CSEXPORT class UMetaData* CSCONV Export_UPackage_Get_MetaData(UPackage* instance)
{
	return instance->MetaData;
}
#endif

CSEXPORT float CSCONV Export_UPackage_GetLoadTime(UPackage* instance)
{
	return instance->GetLoadTime();
}

#if WITH_EDITORONLY_DATA
CSEXPORT void CSCONV Export_UPackage_GetFolderName(UPackage* instance, FName& result)
{
	result = instance->GetFolderName();
}
#endif

CSEXPORT void CSCONV Export_UPackage_SetDirtyFlag(UPackage* instance, csbool bIsDirty)
{
	instance->SetDirtyFlag(!!bIsDirty);
}

CSEXPORT csbool CSCONV Export_UPackage_IsDirty(UPackage* instance)
{
	return instance->IsDirty();
}

CSEXPORT void CSCONV Export_UPackage_MarkAsFullyLoaded(UPackage* instance)
{
	instance->MarkAsFullyLoaded();
}

CSEXPORT csbool CSCONV Export_UPackage_IsFullyLoaded(UPackage* instance)
{
	return instance->IsFullyLoaded();
}

CSEXPORT void CSCONV Export_UPackage_FullyLoad(UPackage* instance)
{
	instance->FullyLoad();
}

CSEXPORT csbool CSCONV Export_UPackage_ContainsMap(UPackage* instance)
{
	return instance->ContainsMap();
}

CSEXPORT void CSCONV Export_UPackage_SetPackageFlags(UPackage* instance, uint32 NewFlags)
{
	instance->SetPackageFlags(NewFlags);
}

CSEXPORT void CSCONV Export_UPackage_ClearPackageFlags(UPackage* instance, uint32 NewFlags)
{
	instance->ClearPackageFlags(NewFlags);
}

CSEXPORT csbool CSCONV Export_UPackage_HasAnyPackageFlags(UPackage* instance, uint32 FlagsToCheck)
{
	return instance->HasAnyPackageFlags(FlagsToCheck);
}

CSEXPORT csbool CSCONV Export_UPackage_HasAllPackagesFlags(UPackage* instance, uint32 FlagsToCheck)
{
	return instance->HasAllPackagesFlags(FlagsToCheck);
}

CSEXPORT uint32 CSCONV Export_UPackage_GetPackageFlags(UPackage* instance)
{
	return instance->GetPackageFlags();
}

CSEXPORT void CSCONV Export_UPackage_GetGuid(UPackage* instance, FGuid& result)
{
	result = instance->GetGuid();
}

CSEXPORT void CSCONV Export_UPackage_MakeNewGuid(UPackage* instance)
{
	instance->MakeNewGuid();
}

CSEXPORT void CSCONV Export_UPackage_SetGuid(UPackage* instance, const FGuid& NewGuid)
{
	instance->SetGuid(NewGuid);
}

CSEXPORT int64 CSCONV Export_UPackage_GetFileSize(UPackage* instance)
{
	return instance->GetFileSize();
}

CSEXPORT class UMetaData* CSCONV Export_UPackage_GetMetaData(UPackage* instance)
{
	return instance->GetMetaData();
}

CSEXPORT void CSCONV Export_UPackage_WaitForAsyncFileWrites()
{
	UPackage::WaitForAsyncFileWrites();
}

CSEXPORT void CSCONV Export_UPackage(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UPackage_Get_FileName);
	REGISTER_FUNC(Export_UPackage_Set_FileName);
#if WITH_EDITORONLY_DATA
	REGISTER_FUNC(Export_UPackage_Get_MetaData);
#endif
	REGISTER_FUNC(Export_UPackage_GetLoadTime);
#if WITH_EDITORONLY_DATA
	REGISTER_FUNC(Export_UPackage_GetFolderName);
#endif
	REGISTER_FUNC(Export_UPackage_SetDirtyFlag);
	REGISTER_FUNC(Export_UPackage_IsDirty);
	REGISTER_FUNC(Export_UPackage_MarkAsFullyLoaded);
	REGISTER_FUNC(Export_UPackage_IsFullyLoaded);
	REGISTER_FUNC(Export_UPackage_FullyLoad);
	REGISTER_FUNC(Export_UPackage_ContainsMap);
	REGISTER_FUNC(Export_UPackage_SetPackageFlags);
	REGISTER_FUNC(Export_UPackage_ClearPackageFlags);
	REGISTER_FUNC(Export_UPackage_HasAnyPackageFlags);
	REGISTER_FUNC(Export_UPackage_HasAllPackagesFlags);
	REGISTER_FUNC(Export_UPackage_GetPackageFlags);
	REGISTER_FUNC(Export_UPackage_GetGuid);
	REGISTER_FUNC(Export_UPackage_MakeNewGuid);
	REGISTER_FUNC(Export_UPackage_SetGuid);
	REGISTER_FUNC(Export_UPackage_GetFileSize);
	REGISTER_FUNC(Export_UPackage_GetMetaData);
	REGISTER_FUNC(Export_UPackage_WaitForAsyncFileWrites);
}
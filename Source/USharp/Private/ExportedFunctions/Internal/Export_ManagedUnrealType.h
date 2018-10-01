struct FManagedUnrealType
{
	FString Hash;
	void* Obj;// UClass / UScriptStruct / UEnum / UFunction

	static TMap<FString, FManagedUnrealType>& GetTypes()
	{
		// <Path, FManagedUnrealType>
		static TMap<FString, FManagedUnrealType> Types;
		return Types;
	}

	static bool GetType(FString Path, FString& OutHash, void*& OutObj)
	{
		FManagedUnrealType* TypeInfo = GetTypes().Find(Path);
		if (TypeInfo != nullptr)
		{
			OutHash = TypeInfo->Hash;
			OutObj = TypeInfo->Obj;
			return true;
		}
		OutHash = TEXT("");
		OutObj = nullptr;
		return false;
	}

	static void AddType(FString Path, FString Hash, void* Obj)
	{
		FManagedUnrealType TypeInfo;
		TypeInfo.Hash = Hash;
		TypeInfo.Obj = Obj;
		GetTypes().Add(Path, TypeInfo);
	}

	static void RemoveType(FString Path)
	{
		GetTypes().Remove(Path);
	}
};

CSEXPORT csbool CSCONV Export_ManagedUnrealType_GetType(const FString& Path, FString& OutHash, void*& OutObj)
{
	return FManagedUnrealType::GetType(Path, OutHash, OutObj);
}

CSEXPORT void CSCONV Export_ManagedUnrealType_AddType(const FString& Path, const FString& Hash, void* Obj)
{
	FManagedUnrealType::AddType(Path, Hash, Obj);
}

CSEXPORT void CSCONV Export_ManagedUnrealType_RemoveType(const FString& Path)
{
	FManagedUnrealType::RemoveType(Path);
}

CSEXPORT void CSCONV Export_ManagedUnrealType(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_ManagedUnrealType_GetType);
	REGISTER_FUNC(Export_ManagedUnrealType_AddType);
	REGISTER_FUNC(Export_ManagedUnrealType_RemoveType);
}
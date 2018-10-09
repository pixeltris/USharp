#pragma once

#include "Engine/BlueprintGeneratedClass.h"
#include "SharpClass.generated.h"

struct FSharpFunctionLookup
{
	UFunction* Function;
	FNativeFuncPtr Pointer;
	int32 Index;

	FSharpFunctionLookup(UFunction* InFunction, FNativeFuncPtr InPointer)
		: Function(InFunction)
		, Pointer(InPointer)
		, Index(-1)
	{}
};

UCLASS()
class USHARP_API USharpClass : public UClass
{
	GENERATED_BODY()

public:
	// UClass::FuncMap / UClass::NativeFunctionLookupTable are somewhat interconnected but they are seperated which means
	// hotreload lookups are slower. Combine / cache them to improve lookup speeds.
	TMap<FName, FSharpFunctionLookup> FuncMapEx;
	
	UClass::ClassConstructorType ManagedConstructor;
	UClass::ClassConstructorType NativeParentConstructor;

	//virtual void Link(FArchive& Ar, bool bRelinkExistingProperties) override;
};

// Custom UScriptStruct type for the following:
// - GetStructTypeHash (required for getting the struct hash as we aren't using CppStructOps)
// - Guid (required for hotreloading of structs so that the serialization can map the old / new struct)
UCLASS()
class USHARP_API USharpStruct : public UScriptStruct
{
	GENERATED_BODY()

public:
	FGuid Guid;

	// UObject interface.
	virtual void PostDuplicate(bool bDuplicateForPIE) override;
	virtual void PostLoad() override;
	// End of UObject interface.

	void CreateGuid();

	// UScriptStruct interface.
	virtual uint32 GetStructTypeHash(const void* Src) const override;
	virtual FGuid GetCustomGuid() const override;
	// End of UScriptStruct interface.
};
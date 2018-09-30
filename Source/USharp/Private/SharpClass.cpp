#include "SharpClass.h"

//void USharpClass::Link(FArchive& Ar, bool bRelinkExistingProperties)
//{
//	EClassFlags OldClassFlags = ClassFlags;
//	ClassFlags = ClassFlags & (~(CLASS_Intrinsic | CLASS_Native));
//	UClass::Link(Ar, bRelinkExistingProperties);
//	ClassFlags = OldClassFlags;
//}

void USharpStruct::PostLoad()
{
	Super::PostLoad();
	CreateGuid();
}

void USharpStruct::PostDuplicate(bool bDuplicateForPIE)
{
	Super::PostDuplicate(bDuplicateForPIE);
	if (!bDuplicateForPIE)
	{
		Guid = FGuid::NewGuid();
	}
}

void USharpStruct::CreateGuid()
{
	if (!Guid.IsValid() && (GetFName() != NAME_None))
	{
		const FString HashString = GetFName().ToString();
		ensure(HashString.Len());

		const uint32 BufferLength = HashString.Len() * sizeof(HashString[0]);
		uint32 HashBuffer[5];
		FSHA1::HashBuffer(*HashString, BufferLength, reinterpret_cast<uint8*>(HashBuffer));
		Guid = FGuid(HashBuffer[1], HashBuffer[2], HashBuffer[3], HashBuffer[4]);
	}
}

uint32 USharpStruct::GetStructTypeHash(const void* Src) const
{
	return UUserDefinedStruct::GetUserDefinedStructTypeHash(Src, this);
}

FGuid USharpStruct::GetCustomGuid() const
{
	return Guid;
}
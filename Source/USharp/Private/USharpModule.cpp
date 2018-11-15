#include "USharpPCH.h"
#include "CSharpLoader.h"
#include "SharpClass.h"

#define LOCTEXT_NAMESPACE "FUSharpModule"

class FUSharpModule : public IUSharp
{
public:
	virtual void StartupModule() override
	{
		FString AssemblyPath = TEXT("UnrealEngine.Runtime.dll");
		FString LoaderPath = TEXT("Loader.dll");
		FString Args = TEXT("");
		bool bLoaderEnabled = true;
		
		// If USharpClass isn't referenced it doesn't get included? TODO: Look into
		if (USharpClass::StaticClass() == nullptr)
		{
			FText Title = FText::FromString(TEXT("Error"));
			FText Msg = FText::FromString(TEXT("Failed to find USharpClass!"));
			FMessageDialog::Open(EAppMsgType::Ok, Msg, &Title);
		}
		
		if (!CSharpLoader::GetInstance()->Load(AssemblyPath, Args, LoaderPath, bLoaderEnabled))
		{
			UE_LOG(LogTemp, Log, TEXT("USharp failed to load"));
		}
	}

	virtual void ShutdownModule() override
	{
	}
};

IMPLEMENT_MODULE( FUSharpModule, USharp )

#undef LOCTEXT_NAMESPACE
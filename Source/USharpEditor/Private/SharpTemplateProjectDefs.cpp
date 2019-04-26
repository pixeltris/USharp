#if WITH_EDITOR

#include "SharpTemplateProjectDefs.h"
#include "EditorStyleSet.h"
#include "Misc/Paths.h"
#include "Misc/FileHelper.h"
#include "GameProjectGenerationModule.h"
#include "GameProjectUtils.h"

#define LOCTEXT_NAMESPACE "USharpEditor"

void USharpTemplateProjectDefs::RegisterTemplate()
{
	FGameProjectGenerationModule::Get().RegisterTemplateCategory(
		TEXT("USharp"),
		LOCTEXT("USharpCategory_Name", "C#"),
		LOCTEXT("USharpCategory_Description", "C# (USharp)"),
		FEditorStyle::GetBrush("GameProjectDialog.BlueprintIcon"),
		FEditorStyle::GetBrush("GameProjectDialog.BlueprintImage"));
	
	// A seperate tab for "C#/C++" may be useful at a later date when there enough templates to demand it
	/*FGameProjectGenerationModule::Get().RegisterTemplateCategory(
		TEXT("USharpCpp"),
		LOCTEXT("USharpCppCategory_Name", "C#/C++"),
		LOCTEXT("USharpCppCategory_Description", "C#/C++ (USharp)"),
		FEditorStyle::GetBrush("GameProjectDialog.BlueprintIcon"),
		FEditorStyle::GetBrush("GameProjectDialog.BlueprintImage"));*/
}

void USharpTemplateProjectDefs::AddConfigValues(TArray<FTemplateConfigValue>& ConfigValuesToSet, const FString& TemplateName, const FString& ProjectName, bool bShouldGenerateCode) const
{
	Super::AddConfigValues(ConfigValuesToSet, TemplateName, ProjectName, bShouldGenerateCode);
	
	// Redirects for code written in C#
	const FString ActiveGameNameRedirectsValue_LongName = FString::Printf(TEXT("(OldGameName=\"/Script/%s-Managed\",NewGameName=\"/Script/%s-Managed\")"), *TemplateName, *ProjectName);
	const FString ActiveGameNameRedirectsValue_ShortName = FString::Printf(TEXT("(OldGameName=\"%s-Managed\",NewGameName=\"/Script/%s-Managed\")"), *TemplateName, *ProjectName);
	new (ConfigValuesToSet) FTemplateConfigValue(TEXT("DefaultEngine.ini"), TEXT("/Script/Engine.Engine"), TEXT("+ActiveGameNameRedirects"), *ActiveGameNameRedirectsValue_LongName, /*InShouldReplaceExistingValue=*/false);
	new (ConfigValuesToSet) FTemplateConfigValue(TEXT("DefaultEngine.ini"), TEXT("/Script/Engine.Engine"), TEXT("+ActiveGameNameRedirects"), *ActiveGameNameRedirectsValue_ShortName, /*InShouldReplaceExistingValue=*/false);
}

bool USharpTemplateProjectDefs::PreGenerateProject(const FString& DestFolder, const FString& SrcFolder, const FString& NewProjectFile, const FString& TemplateFile, bool bShouldGenerateCode, FText& OutFailReason)
{
	FGuid ProjGuid = FGuid::NewGuid();
	FGuid SlnGuid = FGuid::NewGuid();
	FString ProjGuidStr = ProjGuid.ToString(EGuidFormats::DigitsWithHyphens);
	FString SlnGuidStr = SlnGuid.ToString(EGuidFormats::DigitsWithHyphens);
	
	// Set the sln/csproj guid
	TArray<FString> Files;
	IFileManager::Get().FindFilesRecursive(Files, *DestFolder, TEXT("*.sln"), true, false, false);
	IFileManager::Get().FindFilesRecursive(Files, *DestFolder, TEXT("*.csproj"), true, false, false);
	for (const FString& File : Files)
	{
		FString FileContents;
		if (FFileHelper::LoadFileToString(FileContents, *File))
		{
			FileContents = FileContents.Replace(TEXT("%USHARP_CSPROJ_GUID%"), *ProjGuidStr, ESearchCase::CaseSensitive);
			FileContents = FileContents.Replace(TEXT("%USHARP_SLN_GUID%"), *SlnGuidStr, ESearchCase::CaseSensitive);
			FText FailReason;
			if (!GameProjectUtils::WriteOutputFile(File, FileContents, FailReason))
			{
				OutFailReason = FText::FromString(FString::Printf(TEXT("Couldn't write GUID to %s"), *File));
				return false;
			}
		}
	}
	
	return Super::PreGenerateProject(DestFolder, SrcFolder, NewProjectFile, TemplateFile, bShouldGenerateCode, OutFailReason);
}

bool USharpTemplateProjectDefs::PostGenerateProject(const FString& DestFolder, const FString& SrcFolder, const FString& NewProjectFile, const FString& TemplateFile, bool bShouldGenerateCode, FText& OutFailReason)
{
	return Super::PostGenerateProject(DestFolder, SrcFolder, NewProjectFile, TemplateFile, bShouldGenerateCode, OutFailReason);
}

#undef LOCTEXT_NAMESPACE

#endif
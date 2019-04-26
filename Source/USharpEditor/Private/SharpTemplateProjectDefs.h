#pragma once
#if WITH_EDITORONLY_DATA

#include "DefaultTemplateProjectDefs.h"
#include "SharpTemplateProjectDefs.generated.h"

UCLASS()
class USharpTemplateProjectDefs : public UDefaultTemplateProjectDefs
{
	GENERATED_BODY()

public:
	static void RegisterTemplate();
	
	virtual void AddConfigValues(TArray<FTemplateConfigValue>& ConfigValuesToSet, const FString& TemplateName, const FString& ProjectName, bool bShouldGenerateCode) const override;
	virtual bool PreGenerateProject(const FString& DestFolder, const FString& SrcFolder, const FString& NewProjectFile, const FString& TemplateFile, bool bShouldGenerateCode, FText& OutFailReason) override;
	virtual bool PostGenerateProject(const FString& DestFolder, const FString& SrcFolder, const FString& NewProjectFile, const FString& TemplateFile, bool bShouldGenerateCode, FText& OutFailReason) override;
};

#endif
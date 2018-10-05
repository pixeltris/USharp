#include "CSharpProjectGeneration.h"

#include "USharpPCH.h"
#include "CSharpLoader.h"

#include "Core.h"

FString pluginsBaseDir;
FString managedGameSolutionTemplateDir;
FString managedGameSolutionFinalDir;
FString genFileFullPath;
FString projectGeneratedVariablesFullFilePath;

bool CSharpProjectGeneration::GenerateProject() {
	//All this should only be done at edit time not in player builds
#if WITH_EDITOR
	pluginsBaseDir = CSharpLoader::GetPluginBinariesDir();

	managedGameSolutionTemplateDir = FPaths::Combine(*pluginsBaseDir, TEXT("../") TEXT("ManagedGameSolutionTemplate"));
	managedGameSolutionFinalDir = FPaths::Combine(FPaths::ProjectDir(), TEXT("Managed"));

	managedGameSolutionTemplateDir = FPaths::ConvertRelativePathToFull(managedGameSolutionTemplateDir);

	genFileFullPath = FPaths::Combine(managedGameSolutionFinalDir, TEXT(".usharpGenerated"));
	projectGeneratedVariablesFullFilePath = FPaths::Combine(managedGameSolutionFinalDir, TEXT("USharp.ProjectGeneratedVariables.props"));

	CopySolutionTemplate();
#endif // IS_MONOLITHIC
	return true;
}

bool CSharpProjectGeneration::CopySolutionTemplate() {

	if (!FPaths::FileExists(genFileFullPath)) {
		auto & fileManager = IFileManager::Get();
		if (!FPaths::DirectoryExists(managedGameSolutionFinalDir)) {
			fileManager.MakeDirectory(*managedGameSolutionFinalDir);
		}
		auto & platfromFileHandler = FPlatformFileManager::Get().GetPlatformFile();
		
		auto sucess = true;

		sucess = sucess && platfromFileHandler.CopyDirectoryTree(*managedGameSolutionFinalDir, *managedGameSolutionTemplateDir, false);
		sucess = sucess && GenerateProjectVariablesFile();

		if (sucess) {
			auto & fileStream = *fileManager.CreateFileWriter(*genFileFullPath, EFileWrite::FILEWRITE_None);
			//TODO: write something meaningfull
			auto text = FString("generated\n");
			fileStream << text;
			fileStream.Close();
		}
	}

	return true;
}

bool CSharpProjectGeneration::GenerateProjectVariablesFile() {
	FString allLines;
	FFileHelper::LoadFileToString(allLines, *projectGeneratedVariablesFullFilePath);

	auto projectPath = FPaths::GameDir();
	auto projectFileName = FPaths::GetBaseFilename(FPaths::GetProjectFilePath());

	//TODO: add optional Constants
	allLines = allLines.Replace(TEXT("#Constants#"), TEXT(""), ESearchCase::IgnoreCase);

	allLines = allLines.Replace(TEXT("#GamePath#"), *projectPath, ESearchCase::IgnoreCase);
	allLines = allLines.Replace(TEXT("#GameName#"), *projectFileName, ESearchCase::IgnoreCase);

	FFileHelper::SaveStringToFile(allLines, *projectGeneratedVariablesFullFilePath);

	return true;
}

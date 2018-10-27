#include "CSharpProjectGeneration.h"

#include "USharpPCH.h"
#include "CSharpLoader.h"

#include "Core.h"

FString pluginsBinariesDir;
FString managedGameSolutionTemplateDir;
FString managedGameSolutionFinalDir;
FString genFileFullPath;
FString projectGeneratedVariablesFullFilePath;

bool CSharpProjectGeneration::GenerateProject() {
	//All this should only be done at edit time not in player builds
#if WITH_EDITOR
	pluginsBinariesDir = FPaths::GetPath(FModuleManager::Get().GetModuleFilename("USharp"));
	pluginsBinariesDir = FPaths::ConvertRelativePathToFull(FPaths::Combine(*pluginsBinariesDir, TEXT("..")));

	managedGameSolutionTemplateDir = FPaths::Combine(*pluginsBinariesDir, TEXT(".."), TEXT("ManagedGameSolutionTemplate"));
	managedGameSolutionFinalDir = FPaths::Combine(FPaths::ProjectDir(), TEXT("Managed"));

	managedGameSolutionTemplateDir = FPaths::ConvertRelativePathToFull(managedGameSolutionTemplateDir);

	genFileFullPath = FPaths::Combine(managedGameSolutionFinalDir, TEXT(".usharpGenerated"));
	projectGeneratedVariablesFullFilePath = FPaths::Combine(managedGameSolutionFinalDir, TEXT("USharp.ProjectGeneratedVariables.props"));

	CopySolutionTemplate();
#endif
	return true;
}

bool CSharpProjectGeneration::CopySolutionTemplate() {
	auto sucess = true;

	if (!FPaths::FileExists(genFileFullPath)) {
		auto & fileManager = IFileManager::Get();
		if (!FPaths::DirectoryExists(managedGameSolutionFinalDir)) {
			fileManager.MakeDirectory(*managedGameSolutionFinalDir);
		}
		auto & platfromFileHandler = FPlatformFileManager::Get().GetPlatformFile();

		sucess = sucess && platfromFileHandler.CopyDirectoryTree(*managedGameSolutionFinalDir, *managedGameSolutionTemplateDir, false);
		sucess = sucess && GenerateProjectVariablesFile();

		if (sucess) {
			auto & fileStream = *fileManager.CreateFileWriter(*genFileFullPath, EFileWrite::FILEWRITE_None);
			//TODO: write something meaningfull into the file
			auto text = FString("generated\n");
			fileStream << text;
			fileStream.Close();
		}
	} else {
		//always regenerate the path variables. those may be different from case to case for each developer and engine installation
		sucess = sucess && GenerateProjectVariablesFile();
	}

	return sucess;
}

bool CSharpProjectGeneration::GenerateProjectVariablesFile() {
	FString allLines;
	FFileHelper::LoadFileToString(allLines, *projectGeneratedVariablesFullFilePath);

	auto projectPath = FPaths::ProjectDir();
	auto projectFileName = FPaths::GetBaseFilename(FPaths::GetProjectFilePath());
	auto pluginPath = FPaths::ConvertRelativePathToFull(FPaths::Combine(pluginsBinariesDir, TEXT("..")));

	//TODO: add optional Constants
	allLines = allLines.Replace(TEXT("#Constants#"), TEXT(""), ESearchCase::IgnoreCase);

	allLines = allLines.Replace(TEXT("#PluginPath#"), *pluginPath, ESearchCase::IgnoreCase);
	allLines = allLines.Replace(TEXT("#GamePath#"), *projectPath, ESearchCase::IgnoreCase);
	allLines = allLines.Replace(TEXT("#GameName#"), *projectFileName, ESearchCase::IgnoreCase);

	FFileHelper::SaveStringToFile(allLines, *projectGeneratedVariablesFullFilePath);

	return true;
}

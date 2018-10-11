using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    internal abstract class CodeManager
    {
        public CodeGenerator CodeGenerator { get; private set; }
        public CodeGeneratorSettings Settings
        {
            get { return CodeGenerator.Settings; }
        }

        public string GameSlnPath { get; private set; }
        public string GameProjPath { get; private set; }

        protected virtual string LogCategory
        {
            get { return "CodeManager"; }
        }

        public static CodeManager Create(CodeGenerator codeGenerator)
        {
            CodeManager codeManager = null;
            switch (FPlatformProperties.GetPlatform())
            {
                case EPlatform.Windows:
                    codeManager = new FileWriterCodeManager();
                    break;
                default:
                    throw new NotImplementedException();
            }
            if (codeManager != null)
            {
                codeManager.CodeGenerator = codeGenerator;
            }
            return codeManager;
        }

        public void OnBeginGenerateModules()
        {
            Settings.GetProjectName();

            // Cache some strings we will be needing
            string projectName = Settings.GetProjectName();
            GameSlnPath = Path.Combine(Settings.GetManagedDir(), "ManagedGameCode" + ".sln");
            GameProjPath = Path.Combine(Settings.GetManagedDir(), "GameCode", "GameCode" + ".csproj");

            OnBegin();
        }

        public void OnEndGenerateModules()
        {
            OnEnd();
        }

        protected virtual void OnBegin()
        {
        }

        protected virtual void OnEnd()
        {
        }

        public void OnCodeGenerated(CodeGenerator.UnrealModuleInfo module, UnrealModuleType moduleAssetType, string typeName, string path, CSharpTextBuilder code)
        {
            // Note: path will be empty if using combined enums file or global delegates files
            string root, directory, moduleName, assetName, memberName;
            FPackageName.GetPathInfo(path, out root, out directory, out moduleName, out assetName, out memberName);
            //Log("path:'{0}' root:'{1}' directory:'{2}' asset:'{3}' member:'{4}'", path, root, directory, assetName, memberName);

            string rootFolderName = GetRootFolderName(path, root, module.Type, moduleAssetType);
            if (string.IsNullOrEmpty(rootFolderName) && !string.IsNullOrEmpty(path))
            {
                Log(ELogVerbosity.Error, "Unknown asset root '{0}' ModuleType:'{1}' ModuleAssetType:'{2}' Path:'{3}'",
                    root, module.Type, moduleAssetType, path);
                return;
            }

            string name = Settings.UseTypeNameAsSourceFileName || string.IsNullOrEmpty(assetName) ? typeName : assetName;

            string sourceFilePath = null;
            string projPath = null;
            string slnPath = null;

            switch (module.Type)
            {
                case UnrealModuleType.Game:
                    {
                        string relativeSourceFilePath = null;
                        if (EmulateGameFolderStructure(moduleAssetType))
                        {
                            relativeSourceFilePath = Path.Combine(directory, name + ".cs");
                        }
                        else
                        {
                            relativeSourceFilePath = name + ".cs";
                        }
                        sourceFilePath = Path.Combine(Settings.GetGeneratedCodeDir(), rootFolderName, relativeSourceFilePath);
                        slnPath = GameSlnPath;
                        projPath = GameProjPath;
                    }
                    break;

                case UnrealModuleType.EnginePlugin:
                case UnrealModuleType.Engine:
                    {
                        bool mergeAsPluginProj = false;
                        bool mergeAsUnrealProj = false;
                        switch (Settings.EngineProjMerge)
                        {
                            case CodeGeneratorSettings.ManagedEngineProjMerge.Engine:
                                if (module.Type == UnrealModuleType.Engine)
                                {
                                    mergeAsUnrealProj = true;
                                }
                                break;
                            case CodeGeneratorSettings.ManagedEngineProjMerge.Plugins:
                                if (module.Type == UnrealModuleType.EnginePlugin)
                                {
                                    mergeAsPluginProj = true;
                                }
                                break;
                            case CodeGeneratorSettings.ManagedEngineProjMerge.EngineAndPlugins:
                                if (module.Type == UnrealModuleType.EnginePlugin)
                                {
                                    mergeAsPluginProj = true;
                                }
                                else
                                {
                                    mergeAsUnrealProj = true;
                                }
                                break;
                            case CodeGeneratorSettings.ManagedEngineProjMerge.EngineAndPluginsCombined:
                                mergeAsUnrealProj = true;
                                break;
                        }
                        if (mergeAsPluginProj || mergeAsUnrealProj)
                        {
                            string projName = mergeAsUnrealProj ? "UnrealEngine.csproj" : "UnrealEngine.Plugins.csproj";
                            if (Settings.EngineProjMerge == CodeGeneratorSettings.ManagedEngineProjMerge.EngineAndPluginsCombined)
                            {
                                projPath = Path.Combine(Settings.GetManagedModulesDir(), projName);
                            }
                            else
                            {
                                projPath = Path.Combine(Settings.GetManagedModulesDir(), rootFolderName, projName);
                            }
                        }
                        else
                        {
                            projPath = Path.Combine(Settings.GetManagedModulesDir(), rootFolderName, module.Name, module.Name + ".csproj");
                        }

                        sourceFilePath = Path.Combine(Settings.GetManagedModulesDir(), rootFolderName, module.Name, name + ".cs");

                        if (Settings.ModulesLocation == CodeGeneratorSettings.ManagedModulesLocation.GameFolderCombineSln)
                        {
                            slnPath = GameSlnPath;
                        }
                        else if (Settings.ModulesLocation == CodeGeneratorSettings.ManagedModulesLocation.GameFolderCombineSlnProj)
                        {
                            slnPath = GameSlnPath;
                            projPath = GameProjPath;
                        }
                        else
                        {
                            slnPath = Path.Combine(Settings.GetManagedModulesDir(), "UnrealEngine.sln");
                        }
                    }
                    break;

                case UnrealModuleType.GamePlugin:
                    {
                        sourceFilePath = Path.Combine(Settings.GetGeneratedCodeDir(), rootFolderName, module.Name, name + ".cs");
                        slnPath = GameSlnPath;

                        if (Settings.GameProjMerge == CodeGeneratorSettings.ManagedGameProjMerge.GameAndPlugins)
                        {
                            projPath = GameProjPath;
                        }
                        else if (Settings.GameProjMerge == CodeGeneratorSettings.ManagedGameProjMerge.Plugins)
                        {
                            projPath = Path.Combine(Settings.GetManagedDir(), rootFolderName, "GamePlugins.csproj");
                        }
                        else
                        {
                            projPath = Path.Combine(Settings.GetManagedDir(), rootFolderName, module.Name, module.Name + ".csproj");
                        }
                    }
                    break;
            }

            if (!string.IsNullOrWhiteSpace(sourceFilePath))
            {
                sourceFilePath = Path.GetFullPath(sourceFilePath);
            }
            if (!string.IsNullOrWhiteSpace(projPath))
            {
                projPath = Path.GetFullPath(projPath);
            }
            if (!string.IsNullOrWhiteSpace(slnPath))
            {
                slnPath = Path.GetFullPath(slnPath);
            }

            if (string.IsNullOrWhiteSpace(sourceFilePath) || string.IsNullOrWhiteSpace(projPath) || string.IsNullOrWhiteSpace(slnPath))
            {
                Log(ELogVerbosity.Error, "Unknown output location for '{0}' '{1}'", typeName, path);
            }
            else if (!ValidateOutputPath(sourceFilePath) || !ValidateOutputPath(projPath) || !ValidateOutputPath(slnPath))
            {
                Log(ELogVerbosity.Error, "Invalid output path '{0}'", sourceFilePath);
            }
            else
            {
                //FMessage.Log(ELogVerbosity.Log, sourceFilePath + " | " + projPath + " | " + slnPath);

                try
                {
                    if (UpdateSolutionAndProject(slnPath, projPath))
                    {
                        if (!AddSourceFile(slnPath, projPath, sourceFilePath, code.ToString()))
                        {
                            Log(ELogVerbosity.Error, "Failed to add source file '{0}'", sourceFilePath);
                        }
                    }
                    else
                    {
                        Log(ELogVerbosity.Error, "Failed to create sln/csproj '{0}' '{1}'", slnPath, projPath);
                    }
                }
                catch (Exception e)
                {
                    Log(ELogVerbosity.Error, "Exception when adding source file '{0}' {1}", sourceFilePath, e);
                }
            }
        }

        protected string GetRootFolderName(string path, string root, UnrealModuleType moduleType, UnrealModuleType moduleAssetType)
        {
            switch (moduleType)
            {
                case UnrealModuleType.Game:
                    switch (moduleAssetType)
                    {
                        case UnrealModuleType.Game: return "Game";
                        case UnrealModuleType.Engine: return Path.Combine("EngineAssets");
                        case UnrealModuleType.EnginePlugin: return Path.Combine("EnginePluginAssets", root);
                        case UnrealModuleType.GamePlugin: return Path.Combine("GamePluginAssets", root);
                    }
                    return null;

                case UnrealModuleType.Engine:
                    return "Engine";

                case UnrealModuleType.EnginePlugin:
                    if (Settings.ModulesLocation == CodeGeneratorSettings.ManagedModulesLocation.ModulesFolder)
                    {
                        return "Plugins";
                    }
                    else
                    {
                        return "EnginePlugins";
                    }

                case UnrealModuleType.GamePlugin:
                    return "GamePlugins";
            }
            return null;
        }

        private bool EmulateGameFolderStructure(UnrealModuleType moduleAssetType)
        {
            switch (moduleAssetType)
            {
                case UnrealModuleType.Game: return Settings.FolderEmulation.Game;
                case UnrealModuleType.GamePlugin: return Settings.FolderEmulation.GamePluginAssets;
                case UnrealModuleType.Engine: return Settings.FolderEmulation.EngineAssets;
                case UnrealModuleType.EnginePlugin: return Settings.FolderEmulation.EnginePluginAssets;
                default: return false;
            }
        }

        public virtual bool CreateSolutionFile(string slnPath)
        {
            return true;
        }

        public virtual bool AddProjectFile(string slnPath, string projPath)
        {
            return true;
        }

        public virtual bool AddSourceFile(string slnPath, string projPath, string sourceFilePath, string code)
        {
            return true;
        }

        protected virtual bool UpdateSolutionAndProject(string slnPath, string projPath)
        {
            if (!File.Exists(slnPath) && !CreateSolutionFile(slnPath))
            {
                return false;
            }
            if (!File.Exists(projPath) && !AddProjectFile(slnPath, projPath))
            {
                return false;
            }
            return true;
        }

        protected void CreateFileDirectoryIfNotExists(string path)
        {
            string directory = Path.GetDirectoryName(path);
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch
            {
            }
        }

        protected bool CreateFileIfNotExists(string path)
        {
            if (!File.Exists(path))
            {
                CreateFileDirectoryIfNotExists(path);
                try
                {
                    File.CreateText(path).Close();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private bool ValidateOutputPath(string path)
        {
            if (FPaths.IsFileInDirectoryOrSubDirectory(path, Settings.GetManagedDir()))
            {
                return true;
            }
            if (FPaths.IsFileInDirectoryOrSubDirectory(path, Settings.GetManagedModulesDir()))
            {
                return true;
            }
            return false;
        }

        protected virtual string GetProjectFileContents(string version, string projectName, bool insideEngine, out Guid projectGuid)
        {
            string _ue4RuntimePath = Settings.EngineProjMerge ==
                CodeGeneratorSettings.ManagedEngineProjMerge.EngineAndPluginsCombined ?
                @"..\UnrealEngine.Runtime.dll" : @"..\..\..\UnrealEngine.Runtime.dll";
            projectGuid = Guid.NewGuid();
            string _fileContents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""" + version + @""" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{" + projectGuid + @"}</ProjectGuid>
    <OutputType>Library</OutputType>
    <OutputPath>bin\$(Configuration)\</OutputPath>
    <RootNamespace>" + projectName + @"</RootNamespace>
    <AssemblyName>" + projectName + @"</AssemblyName>
    <TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  </PropertyGroup>
  <ItemGroup>" + Environment.NewLine +
  @"</ItemGroup>" + Environment.NewLine;
            _fileContents +=
@"<ItemGroup>" + Environment.NewLine +
    @"<Reference Include=""" + "UnrealEngine.Runtime" + @""">
      <HintPath>" + _ue4RuntimePath + @"</HintPath>
    </Reference>
  </ItemGroup>" + Environment.NewLine;
            _fileContents +=
  @"<Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>";
            return _fileContents;
        }

        protected string GetEnginePathFromCurrentFolder(string currentPath)
        {
            // Check upwards for /Epic Games/ENGINE_VERSION/Engine/Plugins/USharp/ and extract the path from there
            string[] parentFolders = { "Modules", "Managed", "Binaries", "USharp", "Plugins", "Engine" };
            //string currentPath = GetCurrentDirectory();

            DirectoryInfo dir = Directory.GetParent(currentPath);
            if (Settings.EngineProjMerge != CodeGeneratorSettings.ManagedEngineProjMerge.EngineAndPluginsCombined)
            {
                //Directory Starts To Level Up If Merge Settings Isn't
                //Combining Engine and Plugins
                dir = dir.Parent;
                dir = dir.Parent;
            }
            for (int i = 0; i < parentFolders.Length; i++)
            {
                if (!dir.Exists || !dir.Name.Equals(parentFolders[i], StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                dir = dir.Parent;
            }

            // Make sure one of these folders exists along side the Engine folder: FeaturePacks, Samples, Templates
            if (dir.Exists && Directory.Exists(Path.Combine(dir.FullName, "Templates")))
            {
                return dir.FullName;
            }

            return null;
        }

        protected void Log(string value, params object[] args)
        {
            Log(ELogVerbosity.Log, value, args);
        }

        protected void Log(ELogVerbosity verbosity, string value, params object[] args)
        {
            FMessage.Log(verbosity, string.Format(value, args), LogCategory);
        }
    }
}

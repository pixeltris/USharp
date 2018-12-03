using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // TODO: Merge with CodeManager ?

    /// <summary>
    /// Helper for generating the managed project for the current unreal project
    /// </summary>
    static class TemplateProjectGenerator
    {
        const string targetStandardFramework = "netstandard2.0";
        const string targetNetFramework = "v4.5.2";

        /// <summary>
        /// The engine version e.g. 4.21
        /// </summary>
        static string engineVersion;

        /// <summary>
        /// The project name
        /// </summary>
        static string projectName;

        /// <summary>
        /// The name of the managed project e.g. ProjectName.Managed
        /// </summary>
        static string managedProjectName;

        /// <summary>
        /// The name of the native wrappers project e.g. ProjectName.Native
        /// </summary>
        static string nativeWrappersProjectName;

        /// <summary>
        /// /Engine/Plugins/USharp/
        /// </summary>
        static string pluginBaseDir;

        /// <summary>
        /// /Engine/Plugins/USharp/Managed/Templates/
        /// </summary>
        static string templatesDir;

        /// <summary>
        /// /ProjectName/
        /// </summary>
        static string projectDir;

        /// <summary>
        /// /ProjectName/Managed/
        /// </summary>
        static string projectManagedDir;

        /// <summary>
        /// /ProjectName/Managed/ProjectName.Managed.sln
        /// </summary>
        static string projectManagedSln;

        /// <summary>
        /// /ProjectName/Managed/ProjectName.Managed/
        /// </summary>
        static string projectManagedCsProjDir;

        /// <summary>
        /// /ProjectName/Managed/ProjectName.Managed/ProjectName.Managed.csproj
        /// </summary>
        static string projectManagedCsProj;

        /// <summary>
        /// /ProjectName/Managed/ProjectName.Native/ProjectName.Native.csproj
        /// </summary>
        static string projectNativeWrappersCsProj;

        /// <summary>
        /// /ProjectName/Managed/USharpProject.props
        /// </summary>
        static string projectManagedPropsFile;

        /// <summary>
        /// Either generates or updates the managed project for the current unreal project
        /// </summary>
        public static void Generate()
        {
            if (!Generate("BasicProject"))
            {
                UpdatePropsFile(false);
            }
        }

        private static void UpdatePropsFile(bool firstRun)
        {
            GetPaths();

            if (File.Exists(projectManagedPropsFile))
            {
                string propsText = File.ReadAllText(projectManagedPropsFile);
                bool changed = false;
                UpdateTag(ref propsText, ref changed, "UE4Version", engineVersion);
                UpdateTag(ref propsText, ref changed, "UE4ProjectName", projectName);
                UpdateTag(ref propsText, ref changed, "UE4Defines", string.Empty);
                if (firstRun)
                {
                    UpdateTag(ref propsText, ref changed, "USharpGameProjects", managedProjectName);
                }
                if (changed)
                {
                    File.WriteAllText(projectManagedPropsFile, propsText);
                }
            }
        }

        private static void UpdateTag(ref string propsText, ref bool changed, string tag, string value)
        {
            string tagStart = "<" + tag + ">";
            string tagEnd = "</" + tag + ">";

            int tagStartIndex = propsText.IndexOf(tagStart);
            int tagEndIndex = propsText.IndexOf(tagEnd);
            if (tagStartIndex >= 0 && tagEndIndex >= 0)
            {
                tagStartIndex += tagStart.Length;
                int tagValueLength = tagEndIndex - tagStartIndex;

                string tagValue = propsText.Substring(tagStartIndex, tagValueLength);
                tagValue = tagValue.Replace("\r", string.Empty);
                tagValue = tagValue.Replace("\n", string.Empty);

                if (tagValue != value)
                {
                    changed = true;
                    propsText = propsText.Remove(tagStartIndex, tagValueLength);
                    propsText = propsText.Insert(tagStartIndex, value);
                }
            }
            else
            {
                // Someone manually removed the tag? Regenerate the prop file? Or use a xml reader/writer and re-add the tag.
                FMessage.Log(ELogVerbosity.Warning, "Props file is missing the tag '" + tag + "'. The C# project may not compile. Props file: '" + 
                    projectManagedPropsFile + "'");
            }
        }

        private static bool Generate(string templateName)
        {
            if (!FBuild.WithEditor)
            {
                return false;
            }
            
            if (templateName.Equals("Shared", StringComparison.OrdinalIgnoreCase))
            {
                // The shared folder is reserved for shared content
                return false;
            }

            GetPaths();
            
            string templateDir = Path.Combine(templatesDir, templateName);
            if (!Directory.Exists(templateDir))
            {
                FMessage.Log(ELogVerbosity.Error, "Couldn't find template '" + templateName + "' to generate the managed project from.");
                return false;
            }

            if (!Directory.Exists(projectManagedDir))
            {
                Directory.CreateDirectory(projectManagedDir);
            }

            if ((File.Exists(projectManagedCsProj) && !File.Exists(projectManagedSln)) ||
                (!File.Exists(projectManagedCsProj) && File.Exists(projectManagedSln)))
            {
                // Something is wrong here. The .csproj or .sln exists without the other file. It could be that files were
                // moved around or deleted. We don't want to potentially overwrite existing files so just print a warning instead.
                FMessage.Log(ELogVerbosity.Warning,
                    "Found conflicting .sln/.csproj files when attempting to generate a C# project from the template '" + templateName + "'. " +
                    "If you were expecting project files to be generated try deleting the .sln / .csproj and reopen the editor.\n" +
                    "Solution: " + projectManagedSln + "\nProject: " + projectManagedCsProj);
                return false;
            }

            if (!File.Exists(projectManagedCsProj))
            {
                if (!Directory.Exists(projectManagedCsProjDir))
                {
                    Directory.CreateDirectory(projectManagedCsProjDir);
                }

                // Remove obj/bin directories if they exist
                string[] deleteDirs =
                {
                    Path.Combine(projectManagedCsProjDir, "obj"),
                    Path.Combine(projectManagedCsProjDir, "bin"),
                };
                foreach (string dir in deleteDirs)
                {
                    try
                    {
                        if (Directory.Exists(dir))
                        {
                            Directory.Delete(dir, true);
                        }
                    }
                    catch
                    {
                    }
                }

                // Copy the template / shared folder over to the project folder
                string[] rootDirs =
                {
                    Path.Combine(templatesDir, "Shared"),
                    templateDir
                };
                foreach (string rootDir in rootDirs)
                {
                    if (Directory.Exists(rootDir))
                    {
                        foreach (string dir in Directory.GetDirectories(rootDir))
                        {
                            DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                            if (directoryInfo.Name.Equals("Managed", StringComparison.OrdinalIgnoreCase))
                            {
                                // /Templates/XXXX/Managed/ should be copied to the target csproj directory
                                CopyFilesRecursive(directoryInfo, new DirectoryInfo(projectManagedCsProjDir), true);
                            }
                            else
                            {
                                // All other folders should be merged into the root project directory
                                CopyFilesRecursive(directoryInfo, new DirectoryInfo(Path.Combine(projectDir, directoryInfo.Name)), true);
                            }
                        }
                    }
                }

                // Copy /Templates/USharpProject.props to /ProjectName/Managed/USharpProject.props
                CopyFile(Path.Combine(templatesDir, "USharpProject.props"), projectManagedPropsFile, true);

                // This uri isn't really correct but if we don't provide "__relative__" we have the folder name in the path
                // which isn't what we want
                Uri csprojRelDir = new Uri(Path.Combine(projectManagedCsProjDir, "__relative__"), UriKind.Absolute);

                List<string> sourceFiles = new List<string>();
                foreach (string sourceFile in Directory.GetFiles(projectManagedCsProjDir, "*.cs", SearchOption.AllDirectories))
                {
                    string relativePath = csprojRelDir.MakeRelativeUri(new Uri(sourceFile, UriKind.Absolute)).ToString();
                    sourceFiles.Add(NormalizePath(relativePath));
                }

                // TODO: Determine if sdk or old style projects should be used
                bool sdkStyle = false;

                Guid solutionGuid = Guid.NewGuid();
                Guid projectGuid = Guid.NewGuid();
                GenerateProjectFile(projectGuid, sdkStyle, sourceFiles.ToArray());
                GenerateSln(solutionGuid, projectGuid, sdkStyle);

                UpdatePropsFile(true);
                return true;
            }
            return false;
        }

        private static void GetPaths()
        {
            engineVersion = FBuild.EngineMajorVersion + "." + FBuild.EngineMinorVersion;
            projectName = FApp.GetProjectName();
            managedProjectName = projectName + ".Managed";
            nativeWrappersProjectName = projectName + ".Native";

            // /Engine/Plugins/USharp/Binaries/Win64/ - move it up to /Engine/Plugins/USharp/
            pluginBaseDir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(
                FModuleManager.Instance.GetModuleFilename((FName)"USharp")), "..", ".."));

            // /Engine/Plugins/USharp/Managed/Templates/
            templatesDir = Path.Combine(pluginBaseDir, "Managed", "Templates");

            // /ProjectName/
            projectDir = Path.GetFullPath(FPaths.ProjectDir);

            // /ProjectName/Managed/
            projectManagedDir = Path.Combine(projectDir, "Managed");

            // /ProjectName/Managed/ProjectName.Managed.sln
            projectManagedSln = Path.Combine(projectManagedDir, managedProjectName + ".sln");

            // /ProjectName/Managed/ProjectName.Managed/
            projectManagedCsProjDir = Path.Combine(projectManagedDir, managedProjectName);

            // /ProjectName/Managed/ProjectName.Managed/ProjectName.Managed.csproj
            projectManagedCsProj = Path.Combine(projectManagedCsProjDir, managedProjectName + ".csproj");

            // /ProjectName/Managed/ProjectName.Native/ProjectName.Native.csproj
            projectNativeWrappersCsProj = Path.Combine(projectManagedDir, nativeWrappersProjectName, nativeWrappersProjectName + ".csproj");

            // /ProjectName/Managed/USharpProject.props
            projectManagedPropsFile = Path.Combine(projectManagedDir, "USharpProject.props");

            // Update the %appdata%/USharp/UE_XXXX file which is used by USharpProject.props to look up USharp.props
            string appDataFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "USharp", "UE_" + engineVersion + ".txt");
            if (!File.Exists(appDataFile) || File.ReadAllText(appDataFile) != pluginBaseDir)
            {
                File.WriteAllText(appDataFile, pluginBaseDir);
            }
        }

        private static void GenerateSln(Guid solutionGuid, Guid projectGuid, bool sdkStyle)
        {
            Guid projectTypeGuid = sdkStyle ?
                Guid.Parse("9A19103F-16F7-4668-BE54-9A1E7A4F7556") :
                Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");

            string relativeCsProj = managedProjectName + "\\" + managedProjectName + ".csproj";

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            stringBuilder.AppendLine("# Visual Studio 15");
            stringBuilder.AppendLine("VisualStudioVersion = 15.0.28010.2046");
            stringBuilder.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");
            stringBuilder.AppendLine("Project(\"{" + GuidToString(projectTypeGuid) + "}\") = \"" + managedProjectName +
                "\", \"" + relativeCsProj + "\", \"{" + GuidToString(projectGuid) + "}\"");
            stringBuilder.AppendLine("EndProject");
            stringBuilder.AppendLine("Global");
            stringBuilder.AppendLine("	GlobalSection(SolutionConfigurationPlatforms) = preSolution");
            stringBuilder.AppendLine("		Debug|Any CPU = Debug|Any CPU");
            stringBuilder.AppendLine("		Release|Any CPU = Release|Any CPU");
            stringBuilder.AppendLine("	EndGlobalSection");
            stringBuilder.AppendLine("	GlobalSection(ProjectConfigurationPlatforms) = postSolution");
            stringBuilder.AppendLine("		{" + GuidToString(projectGuid) + "}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
            stringBuilder.AppendLine("		{" + GuidToString(projectGuid) + "}.Debug|Any CPU.Build.0 = Debug|Any CPU");
            stringBuilder.AppendLine("		{" + GuidToString(projectGuid) + "}.Release|Any CPU.ActiveCfg = Release|Any CPU");
            stringBuilder.AppendLine("		{" + GuidToString(projectGuid) + "}.Release|Any CPU.Build.0 = Release|Any CPU");
            stringBuilder.AppendLine("	EndGlobalSection");
            stringBuilder.AppendLine("	GlobalSection(SolutionProperties) = preSolution");
            stringBuilder.AppendLine("		HideSolutionNode = FALSE");
            stringBuilder.AppendLine("	EndGlobalSection");
            stringBuilder.AppendLine("	GlobalSection(ExtensibilityGlobals) = postSolution");
            stringBuilder.AppendLine("		SolutionGuid = {" + GuidToString(solutionGuid) + "}");
            stringBuilder.AppendLine("	EndGlobalSection");
            stringBuilder.AppendLine("EndGlobal");
            File.WriteAllText(projectManagedSln, stringBuilder.ToString());
        }

        private static void GenerateProjectFile(Guid projectGuid, bool sdkStyle, string[] sourceFiles)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (sdkStyle)
            {
                stringBuilder.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
                stringBuilder.AppendLine("  <Import Project=\"$(SolutionDir)\\USharpProject.props\"/>");
                stringBuilder.AppendLine("  <PropertyGroup>");
                stringBuilder.AppendLine("    <TargetFramework>" + targetStandardFramework + "</TargetFramework>");
                stringBuilder.AppendLine("    <DebugType>pdbonly</DebugType>");
                stringBuilder.AppendLine("    <DebugSymbols>true</DebugSymbols>");
                stringBuilder.AppendLine("    <OutputPath>$(OutDir)</OutputPath>");
                stringBuilder.AppendLine("    <AssemblyName>" + managedProjectName + "</AssemblyName>");
                stringBuilder.AppendLine("  </PropertyGroup>");
                stringBuilder.AppendLine("</Project>");
            }
            else
            {
                stringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                stringBuilder.AppendLine("<Project ToolsVersion=\"14.0\" DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
                stringBuilder.AppendLine("  <Import Project=\"$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props\" Condition=\"Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')\" />");
                stringBuilder.AppendLine("  <Import Project=\"$(SolutionDir)\\USharpProject.props\"/>");
                stringBuilder.AppendLine("  <PropertyGroup>");
                stringBuilder.AppendLine("    <Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>");
                stringBuilder.AppendLine("    <Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>");
                stringBuilder.AppendLine("    <ProjectGuid>{" + GuidToString(projectGuid) + "}</ProjectGuid>");
                stringBuilder.AppendLine("    <OutputType>Library</OutputType>");
                stringBuilder.AppendLine("    <RootNamespace>" + projectName + "</RootNamespace>");
                //stringBuilder.AppendLine("    <AssemblyName>" + managedProjectName + "</AssemblyName>");// Defined by USharp.props
                stringBuilder.AppendLine("    <TargetFrameworkVersion>" + targetNetFramework  + "</TargetFrameworkVersion>");
                stringBuilder.AppendLine("  </PropertyGroup>");
                stringBuilder.AppendLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
                stringBuilder.AppendLine("    <DebugSymbols>true</DebugSymbols>");
                stringBuilder.AppendLine("    <DebugType>full</DebugType>");
                stringBuilder.AppendLine("    <Optimize>false</Optimize>");
                stringBuilder.AppendLine("    <OutputPath>$(OutDir)</OutputPath>");
                stringBuilder.AppendLine("    <AssemblyName>" + managedProjectName + "</AssemblyName>");
                stringBuilder.AppendLine("    <ErrorReport>prompt</ErrorReport>");
                stringBuilder.AppendLine("    <DefineConstants>DEBUG;TRACE</DefineConstants>");
                stringBuilder.AppendLine("    <ErrorReport>prompt</ErrorReport>");
                stringBuilder.AppendLine("    <WarningLevel>4</WarningLevel>");
                stringBuilder.AppendLine("  </PropertyGroup>");
                stringBuilder.AppendLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
                stringBuilder.AppendLine("    <DebugType>pdbonly</DebugType>");
                stringBuilder.AppendLine("    <Optimize>true</Optimize>");
                stringBuilder.AppendLine("    <OutputPath>$(OutDir)</OutputPath>");
                stringBuilder.AppendLine("    <AssemblyName>" + managedProjectName + "</AssemblyName>");
                stringBuilder.AppendLine("    <DefineConstants>TRACE</DefineConstants>");
                stringBuilder.AppendLine("    <ErrorReport>prompt</ErrorReport>");
                stringBuilder.AppendLine("    <WarningLevel>4</WarningLevel>");
                stringBuilder.AppendLine("  </PropertyGroup>");
                stringBuilder.AppendLine("  <ItemGroup>");
                stringBuilder.AppendLine("    <Reference Include=\"System\" />");
                stringBuilder.AppendLine("  </ItemGroup>");
                stringBuilder.AppendLine("  <ItemGroup>");
                foreach (string sourceFile in sourceFiles)
                {
                    stringBuilder.AppendLine("    <Compile Include=\"" + sourceFile + "\" />");
                }
                stringBuilder.AppendLine("  </ItemGroup>");
                stringBuilder.AppendLine("  <Import Project=\"$(MSBuildToolsPath)\\Microsoft.CSharp.targets\" />");
                stringBuilder.AppendLine("</Project>");
            }
            File.WriteAllText(projectManagedCsProj, stringBuilder.ToString());
        }

        /// <summary>
        /// Normalizes a file path to be used in a .sln/csproj ('\' must be used instead of '/')
        /// </summary>
        private static string NormalizePath(string path)
        {
            return path.Replace('/', '\\');
        }

        private static string GuidToString(Guid guid)
        {
            return guid.ToString().ToUpper();
        }

        // Copied from PluginInstaller (TODO: Add this to some shared file?)

        internal static void CopyFiles(DirectoryInfo source, DirectoryInfo target, bool overwrite)
        {
            CopyFiles(source, target, overwrite, false);
        }

        internal static void CopyFiles(DirectoryInfo source, DirectoryInfo target, bool overwrite, bool recursive)
        {
            if (!target.Exists)
            {
                target.Create();
            }

            if (recursive)
            {
                foreach (DirectoryInfo dir in source.GetDirectories())
                {
                    CopyFilesRecursive(dir, target.CreateSubdirectory(dir.Name), overwrite);
                }
            }
            foreach (FileInfo file in source.GetFiles())
            {
                CopyFile(file.FullName, Path.Combine(target.FullName, file.Name), overwrite);
            }
        }

        private static void CopyFilesRecursive(DirectoryInfo source, DirectoryInfo target, bool overwrite)
        {
            CopyFiles(source, target, overwrite, true);
        }

        private static void CopyFile(string sourceFileName, string destFileName, bool overwrite)
        {
            if ((overwrite || !File.Exists(destFileName)) && File.Exists(sourceFileName))
            {
                try
                {
                    File.Copy(sourceFileName, destFileName, overwrite);
                }
                catch
                {
                    Console.WriteLine("Failed to copy to '{0}'", destFileName);
                }
            }
        }
    }
}

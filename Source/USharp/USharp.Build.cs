using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace UnrealBuildTool.Rules
{
    public class USharp : ModuleRules
    {
        // If true copy the pdbs when packaging
        private bool copyPdbs = true;
        // The full output path which will be used to build paths relative to $(ProjectDir)
        private Uri outputRelativeDir;

        public USharp(ReadOnlyTargetRules Target) : base(Target)
        {
            bEnableExceptions = true;
            PCHUsage = ModuleRules.PCHUsageMode.NoSharedPCHs;
            PrivatePCHHeaderFile = "Private/USharpPCH.h";

            PublicIncludePaths.AddRange(
                new string[] {
                    // ... add public include paths required here ...
                }
                );

            PrivateIncludePaths.AddRange(
                new string[] {
                    "USharp/Private",
                    "USharp/Private/ExportedFunctions",
                    "USharp/Private/ExportedFunctions/Properties",
                    "USharp/Private/ExportedFunctions/Internal",
                    "USharp/Private/ExportedFunctions/ConsoleManager",
                    // ... add other private include paths required here ...
                }
                );

            PublicDependencyModuleNames.AddRange(
                new string[] {
                    "Core",
                    "CoreUObject",
                    "Engine",
                    "Projects",
                    "InputCore",
                    "Slate",
                    "Projects",
                    "UMG"
                    // ... add other public dependencies that you statically link with here ...
                }
                );
            if (Target.bBuildEditor)
            {
                PublicDependencyModuleNames.AddRange(
                    new string[] {
                        "Kismet",
                        "UnrealEd",
                        "BlueprintGraph",
                        "KismetCompiler",
                        "DesktopPlatform"
                    }
                    );
                DynamicallyLoadedModuleNames.AddRange(
                    new string[] {
                        //"KismetCompiler"
                    }
                    );
                /*PublicIncludePathModuleNames.AddRange(
                    new string[] {
                        "KismetCompiler"
                    }
                    );*/
            }

            PrivateDependencyModuleNames.AddRange(
                new string[] {
                    "Core",
                    // ... add private dependencies that you statically link with here ...
                }
                );

            DynamicallyLoadedModuleNames.AddRange(
                new string[] {
                    // ... add any modules that your module loads dynamically here ...
                }
                );

            // mscoree.lib doesn't auto resolve on some systems
            if (Target.Platform == UnrealTargetPlatform.Win32 ||
                Target.Platform == UnrealTargetPlatform.Win64)
            {
                Version newestVersion = default(Version);
                string newestLib = null;
                string newestInclude = null;

                string netFxDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Windows Kits", "NETFXSDK");
                if (Directory.Exists(netFxDir))
                {
                    foreach (string versionDir in Directory.GetDirectories(netFxDir))
                    {
                        string versionDirName = new DirectoryInfo(versionDir).Name;
                        Version version;
                        if (Version.TryParse(versionDirName, out version) && 
                            (newestLib == null || version > newestVersion))
                        {
                            string archDir = Target.Platform == UnrealTargetPlatform.Win32 ? "x86" : "x64";
                            string lib = Path.Combine(versionDir, "Lib", "um", archDir, "mscoree.lib");
                            string include = Path.Combine(versionDir, "Include", "um", "mscoree.h");
                            if (File.Exists(lib) && File.Exists(include))
                            {
                                newestLib = lib;
                                newestInclude = include;
                                newestVersion = version;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(newestLib) && !string.IsNullOrEmpty(newestInclude))
                {
                    PublicIncludePaths.Add(Path.GetDirectoryName(newestInclude));
                    PublicAdditionalLibraries.Add(newestLib);
                }
            }
            
            if (Target.Type == TargetType.Game && Target.ProjectFile != null)
            {
                // If this is a packaged build set up the /Managed/ folders
                SetupManagedPaths();
            }
        }
        
        private string GetPlatformString()
        {
            // Keep this up to date with CSharpLoader::GetPlatformString()
            return Target.Platform.ToString();
        }
        
        private string GetPluginManagedBinDir()
        {
            string pluginBinDir = Path.Combine(ModuleDirectory, "..", "..", "Binaries");
            return Path.Combine(pluginBinDir, "Managed");
        }
        
        private string GetMonoDir(string managedBinDir, bool checkInstallDir)
        {
            // TODO: Also check the default install path of Mono
            return Path.Combine(managedBinDir, "Runtimes", "Mono", GetPlatformString());
        }
        
        private string GetCoreCLRDir(string managedBinDir, bool checkInstallDir)
        {
            // TODO: Also check the default install path of .NET Core
            return Path.Combine(managedBinDir, "Runtimes", "CoreCLR", GetPlatformString());
        }
        
        private string GetDotNetRuntimesFile(string managedBinDir)
        {
            return Path.Combine(managedBinDir, "Runtimes", "DotNetRuntime.txt");
        }

        private void SetupManagedPaths()
        {
            // Target the game project directory
            string projectFile = Target.ProjectFile.FullName;
            if (File.Exists(projectFile) && Path.GetExtension(projectFile) == ".uproject" && Directory.Exists(ModuleDirectory))
            {
                // USharp paths (engine plugins folder)
                string pluginManagedBinDir = GetPluginManagedBinDir();
                string pluginShippingManagedBinDir = Path.Combine(pluginManagedBinDir, "Shipping");

                // Game project paths
                string projectDir = Path.GetDirectoryName(projectFile);
                string managedBinDir = Path.Combine(projectDir, "Binaries", "Managed");
                
                outputRelativeDir = new Uri(Path.GetFullPath(Path.Combine(projectDir, "Binaries")), UriKind.Absolute);

                if (!Directory.Exists(managedBinDir))
                {
                    // This might not be a USharp enabled project?
                    return;
                }
                
                // Add CoreCLR/Mono folders to RuntimeDependencies (if they exists)
                // 
                // NOTE: We need to copy the folders locally rather than directly referencing the folders in the engine
                //       plugins folder as RuntimeDependencies depends on paths being within the game project folder.
                string sourceCoreCLRDir = GetCoreCLRDir(pluginManagedBinDir, true);
                string sourceMonoDir = GetMonoDir(pluginManagedBinDir, true);
                string sourceRuntimesFile = GetDotNetRuntimesFile(pluginManagedBinDir);

                string destCoreCLRDir = GetCoreCLRDir(managedBinDir, false);
                string destMonoDir = GetMonoDir(managedBinDir, false);
                string destRuntimesFile = GetDotNetRuntimesFile(managedBinDir);

                bool copyCoreCLR = false;
                bool copyMono = false;
                bool cleanRuntimeFolders = false;// If true delete the contents of the target runtime folders before copying

                if (File.Exists(sourceRuntimesFile))
                {
                    foreach (string str in File.ReadAllLines(sourceRuntimesFile))
                    {
                        string line = str.Trim().ToLower();

                        const string packagePrefix = "package:";
                        if (line.StartsWith(packagePrefix))
                        {
                            line = line.Substring(packagePrefix.Length).Trim();

                            switch (line.ToLower())
                            {
                                case "mono":
                                    copyMono = true;
                                    break;
                                case "coreclr":
                                    copyCoreCLR = true;
                                    break;
                                case "clean":
                                    cleanRuntimeFolders = true;
                                    break;
                            }
                        }
                    }
                }
                
                switch (Target.Platform)
                {
                    case UnrealTargetPlatform.Android:
                        // Mono is currently the only supported runtime on Android. Ensure that it gets packaged.
                        copyMono = true;
                        copyCoreCLR = false;
                        break;
                }
                
                if (copyMono && !Directory.Exists(sourceMonoDir))
                {
                    copyMono = false;
                }
                
                if (copyCoreCLR && !Directory.Exists(sourceCoreCLRDir))
                {
                    copyCoreCLR = false;
                }
                
                if (Target.Platform == UnrealTargetPlatform.Android)
                {
                    // We need to store a file list somewhere as there are issues with traversing the folder hierarchy
                    // (both the .obb file structure and the .apk file structure have issues with traversal)
                    
                    // NOTE: There is currently a crash loading files outside of the obb (FFileHelper::LoadFileToStringArray / FFileHelper::LoadFileToArray)
                    bool storeInObb = true;
                    
                    StringBuilder fileListStr = new StringBuilder();
                    Uri rootRelativeDir = new Uri(Path.GetFullPath(Path.Combine(projectDir, "Binaries", "Managed", "_")), UriKind.Absolute);
                    foreach (string file in GetAllFiles(new DirectoryInfo(managedBinDir), "Runtimes"))
                    {
                        string relativePath = rootRelativeDir.MakeRelativeUri(new Uri(file, UriKind.Absolute)).ToString();
                        fileListStr.Append(relativePath + "\n");
                    }
                    // Not needed until .NET Core is supported
                    /*if ((copyMono || copyCoreCLR) && File.Exists(sourceRuntimesFile))
                    {
                        string relativePath = rootRelativeDir.MakeRelativeUri(new Uri(sourceRuntimesFile, UriKind.Absolute)).ToString();
                        fileListStr.Append(relativePath + "\n");
                    }*/
                    if (copyMono)
                    {
                        Uri monoRelativeDir = new Uri(Path.GetFullPath(Path.Combine(sourceMonoDir, "_")), UriKind.Absolute);
                        foreach (string file in Directory.GetFiles(sourceMonoDir, "*.*", SearchOption.AllDirectories))
                        {
                            string relativePath = monoRelativeDir.MakeRelativeUri(new Uri(file, UriKind.Absolute)).ToString();
                            fileListStr.Append("Runtimes/Mono/Android/" + relativePath + "\n");
                        }
                    }
                    if (copyCoreCLR)
                    {
                        Uri coreClrRelativeDir = new Uri(Path.GetFullPath(Path.Combine(sourceCoreCLRDir, "_")), UriKind.Absolute);
                        foreach (string file in Directory.GetFiles(sourceCoreCLRDir, "*.*", SearchOption.AllDirectories))
                        {
                            string relativePath = coreClrRelativeDir.MakeRelativeUri(new Uri(file, UriKind.Absolute)).ToString();
                            fileListStr.Append("Runtimes/CoreCLR/Android/" + relativePath + "\n");
                        }
                    }
                    
                    string androidFileListFile = Path.Combine(managedBinDir, "AndroidFileList.txt");
                    try
                    {
                        File.WriteAllText(androidFileListFile, fileListStr.ToString());
                    }
                    catch
                    {
                    }
                    
                    if (storeInObb)
                    {
                        // Store USharp files in the .obb
                        AddFileToRuntimeDependencies(androidFileListFile);
                    }
                    else
                    {
                        // Store USharp files in the .apk under /assets/
                        string pluginPath = Utils.MakePathRelativeTo(ModuleDirectory, Target.RelativeEnginePath);
                        AdditionalPropertiesForReceipt.Add("AndroidPlugin", Path.Combine(pluginPath, "USharp_APL.xml"));
                        return;
                    }
                }

                // Copy the shipping build version of UnrealEngine.Runtime.dll to /ProjectName/Binaries/Managed
                string shippingRuntimeDllPath = Path.Combine(pluginShippingManagedBinDir, "UnrealEngine.Runtime.dll");
                if (File.Exists(shippingRuntimeDllPath))
                {
                    File.Copy(shippingRuntimeDllPath, Path.Combine(managedBinDir, "UnrealEngine.Runtime.dll"), true);
                }

                // Add "ProjectName/Binaries/Managed/" to the RuntimeDependencies
                AddToRuntimeDependenciesRecursively(new DirectoryInfo(managedBinDir), "Runtimes");
                
                if (copyCoreCLR || copyMono)
                {
                    // Copy the runtimes text file to the output directory
                    FileInfo destRuntimesFileInfo = new FileInfo(destRuntimesFile);
                    if (!destRuntimesFileInfo.Directory.Exists)
                    {
                        Directory.CreateDirectory(destRuntimesFileInfo.DirectoryName);
                    }
                    CopyFile(sourceRuntimesFile, destRuntimesFile, true);

                    if (copyCoreCLR)
                    {
                        if (cleanRuntimeFolders)
                        {
                            CleanFolder(destCoreCLRDir);
                        }

                        // Copy the CoreCLR folder into the project binaries folder
                        CopyFilesRecursive(new DirectoryInfo(sourceCoreCLRDir), new DirectoryInfo(destCoreCLRDir), true);
                    }

                    if (copyMono)
                    {
                        if (cleanRuntimeFolders)
                        {
                            CleanFolder(destMonoDir);
                        }

                        // Copy the Mono folder into the project binaries folder
                        CopyFilesRecursive(new DirectoryInfo(sourceMonoDir), new DirectoryInfo(destMonoDir), true);
                    }
                }
            }
        }

        private void CleanFolder(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (dirInfo.Exists)
            {
                try
                {
                    dirInfo.Delete(true);
                }
                catch
                {
                }
                try
                {
                    if (!dirInfo.Exists)
                    {
                        dirInfo.Create();
                    }
                }
                catch
                {
                }
            }
        }
        
        // Adapted from PluginInstaller/Program.cs
        private void CopyFilesRecursive(DirectoryInfo source, DirectoryInfo target, bool overwrite)
        {
            if (!target.Exists)
            {
                target.Create();
            }

            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                CopyFilesRecursive(dir, target.CreateSubdirectory(dir.Name), overwrite);
            }
            foreach (FileInfo file in source.GetFiles())
            {
                CopyFile(file.FullName, Path.Combine(target.FullName, file.Name), overwrite);
            }
        }

        // Adapted from PluginInstaller/Program.cs
        private void CopyFile(string sourceFileName, string destFileName, bool overwrite)
        {
            if (!copyPdbs && Path.GetExtension(sourceFileName) == ".pdb")
            {
                return;
            }
        
            if ((overwrite || !File.Exists(destFileName)) && File.Exists(sourceFileName))
            {
                FileInfo srcFileInfo = new FileInfo(sourceFileName);
                FileInfo destFileInfo = new FileInfo(destFileName);
                if (!destFileInfo.Exists ||
                    srcFileInfo.Length != destFileInfo.Length ||
                    srcFileInfo.LastWriteTimeUtc != destFileInfo.LastWriteTimeUtc)
                {
                    // Only copy the file if it has been modified (but always add it to RuntimeDependencies)
                    try
                    {
                        File.Copy(sourceFileName, destFileName, overwrite);
                    }
                    catch
                    {
                        Console.WriteLine("USharp-CopyFile: Failed to copy to '{0}'", destFileName);
                    }
                }
                                
                if (File.Exists(destFileName))
                {
                    string relativePath = outputRelativeDir.MakeRelativeUri(new Uri(destFileName, UriKind.Absolute)).ToString();
                    RuntimeDependencies.Add("$(ProjectDir)/" + relativePath, StagedFileType.NonUFS);
                }
            }
        }

        private string[] GetAllFiles(DirectoryInfo target, params string[] ignoreDirs)
        {
            List<string> result = new List<string>();
            GetAllFiles(target, result, ignoreDirs);
            return result.ToArray();
        }
        
        private void GetAllFiles(DirectoryInfo target, List<string> result, params string[] ignoreDirs)
        {
            foreach (DirectoryInfo dir in target.GetDirectories())
            {
                if (Array.IndexOf(ignoreDirs, dir.Name) >= 0)
                {
                    result.AddRange(GetAllFiles(dir));
                }
            }
            foreach (FileInfo file in target.GetFiles())
            {
                result.Add(file.FullName);
            }
        }
        
        /// <summary>
        /// Recursively adds a folder and all files to runtime dependencies without copying
        /// </summary>
        /// <param name="target">The folder to include in the runtime dependencies (the folder must be under the project directory)</param>
        private void AddToRuntimeDependenciesRecursively(DirectoryInfo target, params string[] ignoreDirs)
        {
            if (target.Exists)
            {
                foreach (DirectoryInfo dir in target.GetDirectories())
                {
                    if (Array.IndexOf(ignoreDirs, dir.Name) >= 0)
                    {
                        AddToRuntimeDependenciesRecursively(target);
                    }
                }
                foreach (FileInfo file in target.GetFiles())
                {
                    AddFileToRuntimeDependencies(file.FullName);
                }
            }
        }

        /// <summary>
        /// Adds a file to runtime dependencies without copying
        /// </summary>
        /// <param name="destFileName">The file to include in the runtime dependencies (the file must be under the project directory)</param>
        private void AddFileToRuntimeDependencies(string destFileName)
        {
            if (!copyPdbs && Path.GetExtension(destFileName) == ".pdb")
            {
                return;
            }

            if (!File.Exists(destFileName))
            {
                Console.WriteLine("USharp-AddFileToRuntimeDependencies: Failed Adding '{0}' To Runtime.", destFileName);
                return;
            }

            string relativePath = outputRelativeDir.MakeRelativeUri(new Uri(destFileName, UriKind.Absolute)).ToString();
            RuntimeDependencies.Add("$(ProjectDir)/" + relativePath, StagedFileType.NonUFS);
        }
    }
}
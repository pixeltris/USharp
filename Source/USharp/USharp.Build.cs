using System;
using System.IO;

namespace UnrealBuildTool.Rules
{
    public class USharp : ModuleRules
    {
        // The shipping build version of the UnrealEngine.Runtime.dll
        private string shippingRuntimeDllPath;
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
                    "Projects"
                    // ... add other public dependencies that you statically link with here ...
                }
                );
            if (Target.bBuildEditor)
            {
                PublicDependencyModuleNames.AddRange(
                    new string[] {
                        "UnrealEd",
                        "BlueprintGraph",
                        "KismetCompiler"
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

            if (Target.Type == TargetType.Game && Target.ProjectFile != null)
            {
                // If this is a packaged build set up the /Managed/ folders
                SetupManagedPaths();
            }
        }

        private void SetupManagedPaths()
        {
            // Target the game project directory
            string projectFile = Target.ProjectFile.FullName;
            if (File.Exists(projectFile) && Path.GetExtension(projectFile) == ".uproject" && Directory.Exists(ModuleDirectory))
            {
                // USharp paths (engine plugins folder)
                string binDir = Path.Combine(ModuleDirectory, "..", "..", "Binaries");
                string managedBinDir = Path.Combine(binDir, "Managed");
                string shippingManagedBinDir = Path.Combine(managedBinDir, "Shipping");

                // Game project paths
                string projectDir = Path.GetDirectoryName(projectFile);
                string managedDir = Path.Combine(projectDir, "Managed", "Binaries");
                string managedOutputDir = Path.Combine(projectDir, "Binaries", "Managed");
                if (Directory.Exists(managedDir))
                {
                    outputRelativeDir = new Uri(Path.GetFullPath(Path.Combine(projectDir, "Binaries")), UriKind.Absolute);

                    if (!Directory.Exists(managedOutputDir))
                    {
                        Directory.CreateDirectory(managedOutputDir);
                    }

                    shippingRuntimeDllPath = Path.Combine(shippingManagedBinDir, "UnrealEngine.Runtime.dll");
                    if (!File.Exists(shippingRuntimeDllPath))
                    {
                        shippingRuntimeDllPath = null;
                    }

                    // Copy recursively from "ProjectName/Managed/Binaries/" to "ProjectName/Binaries/Managed/"
                    // Each file path is added to RuntimeDependencies which will be copied to the final packaged folder
                    CopyFilesRecursive(new DirectoryInfo(managedDir), new DirectoryInfo(managedOutputDir), true);

                    // Add CoreCLR/Mono folders to RuntimeDependencies (if they exists)
                    // 
                    // NOTE: We need to copy the folders locally rather than directly referencing the folders in the engine
                    //       plugins folder as RuntimeDependencies depends on paths being within the game project folder.
                    string sourceCoreCLRDir = Path.Combine(managedBinDir, "Runtimes", "CoreCLR");
                    string sourceMonoDir = Path.Combine(managedBinDir, "Runtimes", "Mono");
                    string sourceRuntimesFile = Path.Combine(managedBinDir, "Runtimes", "DotNetRuntime.txt");

                    string destCoreCLRDir = Path.Combine(managedOutputDir, "Runtimes", "CoreCLR");
                    string destMonoDir = Path.Combine(managedOutputDir, "Runtimes", "Mono");
                    string destRuntimesFile = Path.Combine(managedOutputDir, "Runtimes", "DotNetRuntime.txt");

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
                                        if (Directory.Exists(sourceMonoDir))
                                        {
                                            copyMono = true;
                                        }
                                        break;
                                    case "coreclr":
                                        if (Directory.Exists(sourceCoreCLRDir))
                                        {
                                            copyCoreCLR = true;
                                        }
                                        break;
                                    case "clean":
                                        cleanRuntimeFolders = true;
                                        break;
                                }
                            }
                        }

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
                if (Path.GetFileName(sourceFileName) == "UnrealEngine.Runtime.dll")
                {
                    if (string.IsNullOrEmpty(shippingRuntimeDllPath))
                    {
                        return;
                    }
                    else
                    {
                        // Use the shipping build version of the runtime dll
                        sourceFileName = shippingRuntimeDllPath;
                    }
                }

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

        /// <summary>
        /// Recursively adds a folder and all files to runtime dependencies without copying
        /// </summary>
        /// <param name="target">The folder to include in the runtime dependencies (the folder must be under the project directory)</param>
        private void AddToRuntimeDependenciesRecursively(DirectoryInfo target)
        {
            if (target.Exists)
            {
                foreach (DirectoryInfo dir in target.GetDirectories())
                {
                    AddToRuntimeDependenciesRecursively(dir);
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
            if(!File.Exists(destFileName))
            {
                Console.WriteLine("USharp-AddFileToRuntimeDependencies: Failed Adding '{0}' To Runtime.", destFileName);
                return;
            }

            string relativePath = outputRelativeDir.MakeRelativeUri(new Uri(destFileName, UriKind.Absolute)).ToString();
            RuntimeDependencies.Add("$(ProjectDir)/" + relativePath, StagedFileType.NonUFS);
        }
    }
}
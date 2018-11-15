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
                    "Slate"
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
            if (File.Exists(projectFile) && Path.GetExtension(projectFile) == ".uproject")
            {
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
                    
                    if (Directory.Exists(ModuleDirectory))
                    {
                        string binDir = Path.Combine(ModuleDirectory, "..", "..", "Binaries");
                        string managedBinDir = Path.Combine(binDir, "Managed");
                        string shippingManagedBinDir = Path.Combine(managedBinDir, "Shipping");
                        
                        shippingRuntimeDllPath = Path.Combine(shippingManagedBinDir, "UnrealEngine.Runtime.dll");
                        if (!File.Exists(shippingRuntimeDllPath))
                        {
                            shippingRuntimeDllPath = null;
                        }
                    }
                    
                    // Copy recursively from "ProjectName/Managed/Binaries/" to "ProjectName/Binaries/Managed/"
                    // Each file path is added to RuntimeDependencies which will be copied to the final packaged folder
                    CopyFilesRecursive(new DirectoryInfo(managedDir), new DirectoryInfo(managedOutputDir), true);
                }

                if (Directory.Exists(ModuleDirectory))
                {
                    string binDir = Path.Combine(ModuleDirectory, "..", "..", "Binaries");
                    string managedBinDir = Path.Combine(binDir, "Managed");

                    //Add CoreCLR/Mono Folders To RuntimeDependencies If They Exists
                    string sourceCoreCLRDir = Path.Combine(binDir, "CoreCLR");
                    string sourceMonoDir = Path.Combine(binDir, "Mono");
                    string sourceDotNetRuntimeTextFile = Path.Combine(managedBinDir, "DotNetRuntime" + ".txt");

                    string destCoreCLRDir = Path.Combine(projectDir, "Binaries", "CoreCLR"); 
                    string destMonoDir = Path.Combine(projectDir, "Binaries", "Mono");
                    string destDotNetRuntimeTextFile = Path.Combine(projectDir, "Binaries", "Managed", "DotNetRuntime" + ".txt");

                    bool bCopyOverCoreCLR = false;
                    bool bCopyOverMono = false;

                    if (File.Exists(sourceDotNetRuntimeTextFile))
                    {
                        foreach (string _line in File.ReadAllLines(sourceDotNetRuntimeTextFile))
                        {
                            string _adjustedLine = _line.Trim();
                            _adjustedLine = _adjustedLine.ToLower();
                            if (_adjustedLine.Equals("mono", StringComparison.OrdinalIgnoreCase))
                            {
                                if (Directory.Exists(sourceMonoDir))
                                {
                                    bCopyOverMono = true;
                                }
                            }
                            else if (_adjustedLine.Equals("coreclr", StringComparison.OrdinalIgnoreCase))
                            {
                                if (Directory.Exists(sourceCoreCLRDir))
                                {
                                    bCopyOverCoreCLR = true;
                                }
                            }
                        }

                        //Only Add Runtime Files If TextFile Contains Runtimes
                        if (bCopyOverCoreCLR == false && bCopyOverMono == false) return;

                        //Copy DotNetRuntime Text File To Project Binaries Managed Folder
                        FileInfo _destTextInfo = new FileInfo(destDotNetRuntimeTextFile);
                        if(_destTextInfo.Directory.Exists == false)
                        {
                            Directory.CreateDirectory(_destTextInfo.DirectoryName);
                        }
                        CopyFile(sourceDotNetRuntimeTextFile, destDotNetRuntimeTextFile, true);

                        if (bCopyOverCoreCLR)
                        {
                            if(Directory.Exists(destCoreCLRDir) == false)
                            {
                                Directory.CreateDirectory(destCoreCLRDir);
                            }
                            //Copy CoreCLR Folder Into Project Binaries Folder
                            CopyFilesRecursive(new DirectoryInfo(sourceCoreCLRDir), new DirectoryInfo(destCoreCLRDir), true);
                        }

                        if (bCopyOverMono)
                        {
                            if (Directory.Exists(destMonoDir) == false)
                            {
                                Directory.CreateDirectory(destMonoDir);
                            }
                            //Copy CoreCLR Folder Into Project Binaries Folder
                            CopyFilesRecursive(new DirectoryInfo(sourceMonoDir), new DirectoryInfo(destMonoDir), true);
                        }
                    }
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

                //Console.WriteLine("USharp-CopyFile: '" + sourceFileName + "');

                bool copied = false;
                try
                {
                    File.Copy(sourceFileName, destFileName, overwrite);
                    copied = true;
                }
                catch
                {
                    Console.WriteLine("USharp-CopyFile: Failed to copy to '{0}'", destFileName);
                }
                                
                if (copied)
                {
                    string relativePath = outputRelativeDir.MakeRelativeUri(new Uri(destFileName, UriKind.Absolute)).ToString();
                    RuntimeDependencies.Add("$(ProjectDir)/" + relativePath, StagedFileType.NonUFS);
                }
            }
        }

        //Add Files in Folder To Runtime Dependencies Without Copying
        private void AddToRuntimeDependenciesRecursively(DirectoryInfo target)
        {
            if (!target.Exists)
            {
                target.Create();
            }

            foreach (DirectoryInfo dir in target.GetDirectories())
            {
                AddToRuntimeDependenciesRecursively(dir);
            }
            foreach (FileInfo file in target.GetFiles())
            {
                AddFileToRuntimeDependencies(file.FullName);
            }
        }

        //Add File To Runtime Dependencies Without Copying
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
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
            PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;
        
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
    }
}
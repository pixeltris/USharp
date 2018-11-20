using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

// TODO:
// [ ] Exception handling for various actions which could fail (file handles being held etc)

namespace PluginInstaller
{
    class Program
    {
        private const string ExampleEnginePath = "C:/Epic Games/UE_4.20/";
        private static Settings settings;
        private static string msbuildPath;

        public static readonly bool IsUnix;
        public static readonly bool IsWindows;

        static Program()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    IsUnix = true;
                    break;
                default:
                    IsWindows = true;
                    break;
            }
        }

        static void Main(string[] args)
        {
            settings = Settings.Load();

            // We might be within the engines folder already, use whatever engine path we are in if that is the case
            string currentFolderEnginePath = GetEnginePathFromCurrentFolder();
            if (!string.IsNullOrEmpty(currentFolderEnginePath))
            {
                settings.EnginePath = currentFolderEnginePath;
                settings.Save();
            }

            if (string.IsNullOrEmpty(settings.EnginePath) || !Directory.Exists(settings.EnginePath))
            {
                UpdateEnginePath();
                Console.WriteLine();
            }
            
            PrintHelp();
            Console.WriteLine();

            if (!string.IsNullOrEmpty(settings.EnginePath) && Directory.Exists(settings.EnginePath))
            {
                Console.WriteLine("Targeting engine version '" + new DirectoryInfo(settings.EnginePath).Name + "'");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Failed to find the engine folder! Make sure PluginInstaller.exe is under /Engine/Plugins/USharp/Binaries/Managed/PluginInstaller/PluginInstaller.exe");
                Console.ReadLine();
                return;
            }

            UpdateUSharpPluginContentFiles(false, false);

            ProcessArgs(null, args);

            while (true)
            {
                string line = Console.ReadLine();
                string[] lineArgs = ParseArgs(line);
                if (ProcessArgs(line, lineArgs))
                {
                    break;
                }
            }
        }

        private static bool ProcessArgs(string commandLine, string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "exit":
                    case "close":
                    case "quit":
                    case "q":
                        return true;

                    case "clear":
                    case "cls":
                        Console.Clear();
                        break;                    

                    case "engine":
                        string enginePath = args.Length > 1 ? args[1] : null;
                        if (!string.IsNullOrEmpty(commandLine))
                        {
                            enginePath = commandLine.Trim();
                            int spaceIndex = enginePath.IndexOf(' ');
                            if (spaceIndex > 0)
                            {
                                enginePath = enginePath.Substring(spaceIndex + 1);
                            }
                            else
                            {
                                enginePath = null;
                            }
                        }
                        UpdateEnginePath(enginePath);
                        break;

                    case "update":
                        UpdateUSharpPluginContentFiles(true, true);
                        Console.WriteLine("Content updated");
                        break;

                    case "fullbuild":
                        UpdateUSharpPluginContentFiles(true, false);
                        CompileCs(args);
                        CompileCpp(args);
                        Console.WriteLine("done");
                        break;

                    case "build":
                        CompileCs(args);
                        CompileCpp(args);
                        Console.WriteLine("done");
                        break;

                    case "buildcs":
                        CompileCs(args);
                        Console.WriteLine("done");
                        break;

                    case "buildcustomsln":
                        if(args.Length >= 3 && 
                            !string.IsNullOrEmpty(args[1]) && File.Exists(args[1]) &&
                            !string.IsNullOrEmpty(args[2]) && File.Exists(args[2]))
                        {
                            BuildCustomSolution(args[1], args[2]);
                        }
                        break;

                    case "buildcpp":
                        CompileCpp(args);
                        Console.WriteLine("done");
                        break;

                    case "help":
                    case "?":
                        PrintHelp();
                        break;
                }
            }

            return false;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Available commands:");
            {
                Console.WriteLine("- build       builds C# and C++ projects");
            }
            if (!IsInEngineFolder())
            {
                Console.WriteLine("- fullbuild   builds C# and C++ projects and updates content files");
            }
            {
                Console.WriteLine("- buildcs     builds C# projects (Loader, AssemblyRewriter, Runtime)");
                Console.WriteLine("- buildcpp    builds C++ projects");
            }
            if (!IsInEngineFolder())
            {
                Console.WriteLine("- update      copies content files to the USharp engine plugin folder");
                Console.WriteLine("- engine      set the target engine path for current engine version (e.g. '{0}')", ExampleEnginePath);
            }
        }

        private static string[] ParseArgs(string commandLine)
        {
            char[] parmChars = commandLine.ToCharArray();
            bool inSingleQuote = false;
            bool inDoubleQuote = false;
            for (var index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '"' && !inSingleQuote)
                {
                    inDoubleQuote = !inDoubleQuote;
                    parmChars[index] = '\n';
                }
                if (parmChars[index] == '\'' && !inDoubleQuote)
                {
                    inSingleQuote = !inSingleQuote;
                    parmChars[index] = '\n';
                }
                if (!inSingleQuote && !inDoubleQuote && parmChars[index] == ' ')
                {
                    parmChars[index] = '\n';
                }
            }
            return (new string(parmChars)).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static Dictionary<string, string> GetKeyValues(string[] args)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (args != null)
            {
                foreach (string arg in args)
                {
                    int valueIndex = arg.IndexOf('=');
                    if (valueIndex > 0)
                    {
                        string key = arg.Substring(0, valueIndex);
                        string value = arg.Substring(valueIndex + 1);
                        result[arg.ToLower()] = value;
                    }
                    else
                    {
                        result[arg.ToLower()] = null;
                    }
                }
            }
            return result;
        }

        private static bool ReadYesNo()
        {
            return Console.ReadLine().ToLower().StartsWith("y");
        }

        private static string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        private static void CopyFilesRecursive(DirectoryInfo source, DirectoryInfo target, bool overwrite)
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

        static bool IsInEngineFolder()
        {
            return !string.IsNullOrEmpty(GetEnginePathFromCurrentFolder());
        }

        static string GetEnginePathFromCurrentFolder()
        {
            // Check upwards for /Epic Games/ENGINE_VERSION/Engine/Plugins/USharp/ and extract the path from there
            string[] parentFolders = { "Managed", "Binaries", "USharp", "Plugins", "Engine" };
            string currentPath = GetCurrentDirectory();

            DirectoryInfo dir = Directory.GetParent(currentPath);
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

        private static void UpdateEnginePath(string enginePath = null)
        {
            if (string.IsNullOrEmpty(enginePath))
            {
                Console.WriteLine("Enter the path for the target engine version (e.g. '{0}'): ", ExampleEnginePath);
            }

            while (true)
            {
                if (string.IsNullOrEmpty(enginePath))
                {
                    enginePath = Console.ReadLine();
                }
                if (!Directory.Exists(enginePath))
                {
                    Console.WriteLine("Couldn't find the engine path '{0}'", enginePath);
                }
                else
                {
                    if (!Directory.Exists(Path.Combine(enginePath, "../Launcher")))
                    {
                        Console.WriteLine("Couldn't find 'Launcher' in the parent folder. Are you sure this path is correct? '{0}'", enginePath);
                        if (ReadYesNo())
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                Console.Write("Enter the engine path: ");
                enginePath = null;
            }

            if (!string.IsNullOrEmpty(enginePath))
            {
                settings.EnginePath = enginePath;
                settings.Save();
                Console.WriteLine("New path: '{0}'", enginePath);
            }
        }

        /// <summary>
        /// Returns the main USharp plugin directory from the engine path
        /// </summary>
        private static string GetUSharpPluginDirectory(string enginePath)
        {
            return Path.Combine(enginePath, "Engine", "Plugins", "USharp");
        }

        /// <summary>
        /// Copy over content files which wont be copied during a build (Resources, Sources, USharp.plugin)
        /// </summary>
        /// <param name="forceUpdate">If true force update the content files, otherwise they will only be copied if they don't already exist</param>
        private static void UpdateUSharpPluginContentFiles(bool forceUpdate, bool logState)
        {
            if (IsInEngineFolder())
            {
                // If we are in the engine folder we shouldn't have to do anything as the content should be where it needs to be.
                if (logState)
                {
                    Console.WriteLine("Updating content is not required as the files are already in the engine plugins folder");
                }
                return;
            }

            if (!Directory.Exists(settings.EnginePath))
            {
                // If we are outside of the engine folder we need a valid path to copy the files over.
                if (logState)
                {
                    Console.WriteLine("Failed to update content as the engine path isn't valid. Use the \"engine\" command to set the path.");
                }
                return;
            }

            string usharpPluginsDir = GetUSharpPluginDirectory(settings.EnginePath);
            if (!Directory.Exists(usharpPluginsDir))
            {
                Directory.CreateDirectory(usharpPluginsDir);
            }

            string relativeUSharpDir = Path.Combine(GetCurrentDirectory(), "../../../");

            // Copy the Resources folder
            const string resourcesFolder = "Resources";
            CopyFilesRecursive(new DirectoryInfo(Path.Combine(relativeUSharpDir, resourcesFolder)),
                new DirectoryInfo(Path.Combine(usharpPluginsDir, resourcesFolder)), forceUpdate);

            // Copy the USharp.plugin file
            const string pluginFile = "USharp.uplugin";
            CopyFile(Path.Combine(relativeUSharpDir, pluginFile), Path.Combine(usharpPluginsDir, pluginFile), forceUpdate);

            // Copy the XXXX.Build.cs files
            const string sourceDir = "Source";
            string[] sources = { "USharp", "USharpEditor" };
            const string buildFileExtension = ".Build.cs";
            foreach (string source in sources)
            {
                string relativeSourceDir = Path.Combine(relativeUSharpDir, sourceDir, source);
                if (Directory.Exists(relativeSourceDir))
                {
                    string relativeBuildFile = Path.Combine(relativeSourceDir, source + buildFileExtension);
                    if (File.Exists(relativeBuildFile))
                    {
                        string targetSourceDir = Path.Combine(usharpPluginsDir, sourceDir, source);
                        if (!Directory.Exists(targetSourceDir))
                        {
                            Directory.CreateDirectory(targetSourceDir);
                        }

                        string targetBuildFile = Path.Combine(targetSourceDir, source + buildFileExtension);

                        // Copy the USharp.Build.cs file
                        CopyFile(relativeBuildFile, targetBuildFile, forceUpdate);
                    }
                }
            }

            // Copy the "/Managed/UnrealEngine.Runtime/UnrealEngine.Runtime/InjectedClasses" folder
            const string injectedClassesDir = "Managed/UnrealEngine.Runtime/UnrealEngine.Runtime/InjectedClasses";
            DirectoryInfo injectedClassesRelativeDir = new DirectoryInfo(Path.Combine(relativeUSharpDir, injectedClassesDir));
            if (injectedClassesRelativeDir.Exists)
            {
                CopyFilesRecursive(injectedClassesRelativeDir,
                    new DirectoryInfo(Path.Combine(usharpPluginsDir, injectedClassesDir)), forceUpdate);
            }
        }

        private static void CompileCs(string[] args)
        {
            string slnDir = Path.Combine(GetCurrentDirectory(), "../../../Managed/UnrealEngine.Runtime");
            if (!Directory.Exists(slnDir))
            {
                Console.WriteLine("Failed to find the UnrealEngine.Runtime directory");
                return;
            }
            
            string loaderProj = Path.Combine(slnDir, "Loader/Loader.csproj");
            string runtimeProj = Path.Combine(slnDir, "UnrealEngine.Runtime/UnrealEngine.Runtime.csproj");
            string assemblyRewriterProj = Path.Combine(slnDir, "UnrealEngine.AssemblyRewriter/UnrealEngine.AssemblyRewriter.csproj");
            string[] projectPaths = { loaderProj, runtimeProj, assemblyRewriterProj };
            string[] shippingBuildProjectPaths = { runtimeProj };
            string slnPath = Path.Combine(slnDir, "UnrealEngine.Runtime.sln");

            Dictionary<string, string> keyValues = GetKeyValues(args);
            bool x86Build = keyValues.ContainsKey("x86");

            foreach (string projPath in projectPaths)
            {
                if (!File.Exists(projPath))
                {
                    Console.WriteLine("Failed to find C# project '{0}'", projPath);
                    return;
                }
            }

            for (int i = 0; i < 2; i++)
            {
                bool shippingBuild = i == 1;
                string[] paths = shippingBuild ? shippingBuildProjectPaths : projectPaths;

                foreach (string projPath in paths)
                {
                    string targetName = Path.GetFileName(projPath);
                    if (shippingBuild)
                    {
                        targetName += " (Shipping)";
                    }

                    if (!BuildCs(slnPath, projPath, !shippingBuild, x86Build, null))
                    {
                        Console.WriteLine("Failed to build (see build.log) - " + targetName);
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Build successful - " + targetName);
                    }
                }
            }

            // If we are in the engine folder we shouldn't have to do anything as the binaries should be in the correct location.
            // If we are outside of the engine folder we need to copy the binaries over to the engine plugins folder.
            string currentFolderEnginePath = GetEnginePathFromCurrentFolder();
            if (string.IsNullOrEmpty(currentFolderEnginePath) && Directory.Exists(settings.EnginePath))
            {
                // Copy the entire "Binaries/Managed" folder to the engine plugins folder
                string relativeBinariesDir = Path.Combine(GetCurrentDirectory(), "../");
                if (Directory.Exists(relativeBinariesDir))
                {                    
                    string engineBinariesDir = Path.Combine(GetUSharpPluginDirectory(settings.EnginePath), "Binaries/Managed");
                    if (!Directory.Exists(engineBinariesDir))
                    {
                        // Make sure the target binaries folder exists
                        Directory.CreateDirectory(engineBinariesDir);
                    }

                    // Copy the files recursively (hopefully this doesn't cause any issues - maybe limit to 1 level deep for AssemblyRewriter)
                    CopyFilesRecursive(new DirectoryInfo(relativeBinariesDir), new DirectoryInfo(engineBinariesDir), true);
                }
            }
        }

        private static string FindMsBuildPath()
        {
            try
            {
                if (IsUnix)
                {
                    try
                    {
                        string monoPath = Process.GetCurrentProcess().MainModule.FileName;
                        if (!string.IsNullOrEmpty(monoPath) && File.Exists(monoPath) &&
                            Path.GetFileName(monoPath).ToLower().StartsWith("mono"))
                        {
                            string msbuildPath = Path.Combine(Path.GetDirectoryName(monoPath), "msbuild");
                            if (File.Exists(msbuildPath))
                            {
                                return msbuildPath;
                            }
                        }
                    }
                    catch
                    {
                    }

                    // Default linux install location
                    string linuxInstallPath = "/usr/bin/msbuild";
                    if (File.Exists(linuxInstallPath))
                    {
                        return linuxInstallPath;
                    }

                    // Default mac install location
                    string macVersionsPath = "/Library/Frameworks/Mono.framework/Versions/";
                    if (Directory.Exists(macVersionsPath))
                    {
                        string latestVersionDir = null;
                        Version latestVersion = null;

                        Dictionary<string, Version> versions = new Dictionary<string, Version>();
                        foreach (string dir in Directory.GetDirectories(macVersionsPath))
                        {
                            Version version;
                            if (Version.TryParse(new DirectoryInfo(dir).Name, out version))
                            {
                                if (latestVersion == null || version > latestVersion)
                                {
                                    latestVersionDir = dir;
                                    latestVersion = version;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(latestVersionDir))
                        {
                            string macInstallPath = Path.Combine(latestVersionDir, "bin", "msbuild");
                            if (File.Exists(macInstallPath))
                            {
                                return macInstallPath;
                            }
                        }
                    }

                    return "msbuild";
                }

                string baseMicrosoftKeyPath = @"SOFTWARE\WOW6432Node\Microsoft";
                string visualStudioRegistryKeyPath = baseMicrosoftKeyPath + @"\VisualStudio\SxS\VS7";

                //Try Obtaining the VS version of MSBuild First
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(visualStudioRegistryKeyPath))
                {
                    if (key != null)
                    {
                        string path = key.GetValue("15.0") as string;
                        if (!string.IsNullOrEmpty(path))
                        {
                            path = Path.Combine(path, "MSBuild", "15.0", "Bin", "msbuild.exe");
                            if (File.Exists(path))
                            {
                                return path;
                            }
                        }
                    }
                }

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSBUILD\ToolsVersions\4.0"))
                {
                    string path = key.GetValue("MSBuildToolsPath") as string;
                    if (!string.IsNullOrEmpty(path))
                    {
                        path = Path.Combine(path, "msbuild.exe");
                        if (File.Exists(path))
                        {
                            return path;
                        }
                    }
                }
            }
            catch
            {                
            }
            return null;
        }

        private static bool BuildCs(string solutionPath, string projectPath, bool debug, bool x86, string customDefines)
        {
            const string buildLogFile = "build.log";

            if (!string.IsNullOrEmpty(solutionPath) && File.Exists(solutionPath))
            {
                solutionPath = Path.GetFullPath(solutionPath);
            }
            if (!string.IsNullOrEmpty(projectPath) && File.Exists(projectPath))
            {
                projectPath = Path.GetFullPath(projectPath);
            }            

            if (string.IsNullOrEmpty(msbuildPath))
            {
                msbuildPath = FindMsBuildPath();
            }

            if (string.IsNullOrEmpty(msbuildPath))
            {
                File.AppendAllText(buildLogFile, "Couldn't find MSBuild path" + Environment.NewLine);
                return false;
            }
            
            string config = debug ? "Debug" : "Release";
            string platform = x86 ? "x86" : "\"Any CPU\"";
            string fileArgs = "\"" + solutionPath + "\"" + " /p:Configuration=" + config + " /p:Platform=" + platform;
            if (!string.IsNullOrEmpty(projectPath))
            {
                // '.' must be replaced with '_' for /t
                string projectName = Path.GetFileNameWithoutExtension(projectPath).Replace(".", "_");

                // Skip project references just in case (this means projects should be built in the correct order though)
                fileArgs += " /t:" + projectName + " /p:BuildProjectReferences=false";
            }
            if (!string.IsNullOrEmpty(customDefines))
            {
                Debug.Assert(!customDefines.Contains(' '));
                fileArgs += " /p:DefineConstants=" + customDefines;
            }

            File.AppendAllText(buildLogFile, "Build: " + msbuildPath + " - " + fileArgs + Environment.NewLine);

            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = msbuildPath,
                    Arguments = fileArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            output.AppendLine(e.Data);
                        }
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            errorWaitHandle.Set();
                        }
                        else
                        {
                            error.AppendLine(e.Data);
                        }
                    };

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    int timeout = 60000;
                    bool built = process.WaitForExit(timeout) && outputWaitHandle.WaitOne(timeout) && errorWaitHandle.WaitOne(timeout);

                    File.AppendAllText(buildLogFile, "Build sln '" + solutionPath + "' proj '" + projectPath + "'" + Environment.NewLine);
                    File.AppendAllText(buildLogFile, string.Empty.PadLeft(100, '-') + Environment.NewLine);
                    File.AppendAllText(buildLogFile, output.ToString() + Environment.NewLine);
                    File.AppendAllText(buildLogFile, error.ToString() + Environment.NewLine + Environment.NewLine);

                    if (!built)
                    {
                        Console.WriteLine("Failed to wait for compile.");
                    }

                    return built && process.ExitCode == 0;
                }
            }
        }

        private static void CompileCpp(string[] args)
        {
            Dictionary<string, string> keyValues = GetKeyValues(args);
            bool shippingBuild = keyValues.ContainsKey("shipping");
            bool x86Build = keyValues.ContainsKey("x86");
            bool skipCopy = keyValues.ContainsKey("nocopy");
            bool skipCleanup = keyValues.ContainsKey("noclean");

            string pluginName = "USharp";
            string pluginExtension = ".uplugin";
            string pluginExtensionTemp = ".uplugin_temp";

            string batchFileName = "RunUAT" + (IsWindows ? ".bat" : ".sh");
            string batchFilesDir = Path.Combine(settings.EnginePath, "Engine/Build/BatchFiles/");
            string batchPath = Path.Combine(batchFilesDir, batchFileName);

            string localPluginDir = Path.Combine(GetCurrentDirectory(), "../../../");
            string enginePluginDir = Path.Combine(settings.EnginePath, "Engine/Plugins/", pluginName);
            string pluginPath = Path.Combine(localPluginDir, pluginName + pluginExtension);
            //string outputDir = Path.Combine(projectBaseDir, "Build");

            string currentFolderEnginePath = GetEnginePathFromCurrentFolder();
            bool isInsideEngineFolder = !string.IsNullOrEmpty(currentFolderEnginePath);

            try
            {
                if (!File.Exists(pluginPath) && isInsideEngineFolder)
                {
                    // We might have a temp plugin file extension due to a partial previous build
                    string tempPluginPath = Path.ChangeExtension(pluginPath, pluginExtensionTemp);
                    if (File.Exists(tempPluginPath))
                    {
                        File.Move(tempPluginPath, pluginPath);
                    }
                }
            }
            catch
            {
            }

            if (!File.Exists(pluginPath))
            {
                Console.WriteLine("Failed to compile C++ project. Couldn't find the plugin '{0}'", Path.GetFullPath(pluginPath));
                return;
            }

            // Use an appdata folder instead of a local Build folder as we may be building from inside the engine folder
            // which doesn't allow compile output to be within a sub folder of /Engine/
            string usharpAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "USharp");
            if (!Directory.Exists(usharpAppData))
            {
                Directory.CreateDirectory(usharpAppData);
            }            
            string outputDir = Path.Combine(usharpAppData, "Build");

            // In 4.20 if it detects that the plugin already exists our compilation will fail (even if we are compiling in-place!)
            // Therefore we need to rename the existing .uplugin file to have a different extension so that UBT doesn't complain.
            // NOTE: For reference the build error is "Found 'USharp' plugin in two locations" ... "Plugin names must be unique."
            string existingPluginFile = Path.Combine(enginePluginDir, pluginName + pluginExtension);
            string tempExistingPluginFile = null;
            if (File.Exists(existingPluginFile))
            {
                tempExistingPluginFile = Path.ChangeExtension(existingPluginFile, pluginExtensionTemp);
                if (File.Exists(tempExistingPluginFile))
                {
                    File.Delete(tempExistingPluginFile);
                }
                File.Move(existingPluginFile, tempExistingPluginFile);
            }

            if (isInsideEngineFolder)
            {
                // Since we are compiling from within the engine plugin folder make sure to use temp changed .plugin_temp file
                pluginPath = tempExistingPluginFile;
            }

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo()
                    {
                        FileName = batchPath,
                        UseShellExecute = false,

                        // The -Platform arg is ignored? It instead compiles based on whitelisted/blacklisted? (or for all platforms if no list)
                        Arguments = string.Format("BuildPlugin -Plugin=\"{0}\" -Package=\"{1}\" -Rocket -Platform=Win64", pluginPath, outputDir)
                    };
                    process.Start();
                    process.WaitForExit();
                }

                if (!skipCopy)
                {
                    // Copy the entire contents of /Binaries/ and /Intermediate/
                    string[] copyDirs = { "Binaries", "Intermediate" };
                    foreach (string dir in copyDirs)
                    {
                        if (Directory.Exists(Path.Combine(outputDir, dir)))
                        {
                            CopyFilesRecursive(new DirectoryInfo(Path.Combine(outputDir, dir)),
                                new DirectoryInfo(Path.Combine(enginePluginDir, dir)), true);

                            // Also copy to the local path if outside of the engine?
                            //if (!isInsideEngineFolder)
                            //{
                            //    CopyFilesRecursive(new DirectoryInfo(Path.Combine(outputDir, dir)),
                            //        new DirectoryInfo(Path.Combine(localPluginDir, dir)), true);
                            //}
                        }
                    }
                }
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrEmpty(tempExistingPluginFile) && File.Exists(tempExistingPluginFile))
                    {
                        if (File.Exists(existingPluginFile))
                        {
                            File.Delete(existingPluginFile);
                        }
                        File.Move(tempExistingPluginFile, existingPluginFile);
                    }
                }
                catch
                {
                }

                if (!skipCleanup)
                {
                    try
                    {
                        if (!Directory.Exists(outputDir))
                        {
                            Directory.Delete(outputDir);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        static bool BuildCustomSolution(string slnPath, string projPath)
        {
            Console.WriteLine("Attempting to build solution: " + slnPath);
            bool buildcs = BuildCs(slnPath, projPath, true, false, null);
            if(buildcs)
            {
                Console.WriteLine("Solution was compiled successfully");
            }
            else
            {
                Console.WriteLine("There was an issue with compiling the provided solution: " + slnPath);
            }
            return buildcs;
        }
    }

    // Save the engine path so we don't have to prompt for it each time
    public class Settings
    {
        const string settingsFile = "Settings.xml";

        public string EnginePath { get; set; }        

        public static Settings Load()
        {
            try
            {
                if (File.Exists(settingsFile))
                {
                    using (XmlReader reader = XmlReader.Create(settingsFile))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                        return (Settings)serializer.Deserialize(reader);
                    }
                }
            }
            catch
            {                
            }
            return new Settings();
        }

        public void Save()
        {
            try
            {
                XmlWriterSettings xmlSettings = new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineOnAttributes = true,
                    OmitXmlDeclaration = true
                };

                using (XmlWriter writer = XmlWriter.Create(settingsFile, xmlSettings))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    serializer.Serialize(writer, this);
                }
            }
            catch
            {
            }
        }
    }
}

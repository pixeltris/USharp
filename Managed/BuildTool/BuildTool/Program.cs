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

// TODO:
// - Set build path to outside of engine folder (%appdata%/USharp/) if building from an engine sub folder
// - Temporarily modify the existing USharp .uplugin file extension to avoid the duplicate plugin error
// - Modify the .uplugin for editor/shipping (until we seperate things into editor/runtime modules)
// - Copy the entire folder of Binaries / Intermediate instead of individual files?
// - Exception handling for various actions which could fail (file handles being held etc)

namespace BuildTool
{
    class Program
    {
        private const string ExampleEnginePath = "C:/Epic Games/UE_4.20/";
        private static Settings settings;
        private static string msbuildPath;

        static void Main(string[] args)
        {
            settings = Settings.Load();

            // We might be within the engines folder already, use whatever engine path we are in if that is the case
            string currentFolderEnginePath = GetEnginePathFromCurrentFolder();
            if (!string.IsNullOrEmpty(currentFolderEnginePath))
            {
                settings.EnginePath = currentFolderEnginePath;
            }

            if (string.IsNullOrEmpty(settings.EnginePath) || Directory.Exists(settings.EnginePath))
            {
                UpdateEnginePath();

                // This is likely a first run, print the help
                if (!string.IsNullOrEmpty(settings.EnginePath) && Directory.Exists(settings.EnginePath))
                {
                    Console.WriteLine();
                    PrintHelp();
                    Console.WriteLine();
                }
            }
            
            if (!string.IsNullOrEmpty(settings.EnginePath))
            {                
                Console.WriteLine("Targeting engine version '" + new DirectoryInfo(settings.EnginePath).Name + "'");
                Console.WriteLine();
            }

            UpdateUSharpPluginContentFiles(false);

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
                        UpdateUSharpPluginContentFiles(true);
                        break;

                    case "fullbuild":
                        UpdateUSharpPluginContentFiles(true);
                        CompileCs(args);
                        CompileCpp(args);
                        break;

                    case "build":
                        CompileCs(args);
                        CompileCpp(args);
                        break;

                    case "buildcs":
                        CompileCs(args);
                        break;

                    case "buildcpp":
                        CompileCpp(args);
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
            Console.WriteLine("- build       builds both C# and C++ parts of USharp");
            Console.WriteLine("- buildcs     builds the C# part of USharp (Loader, AssemblyRewriter, Runtime)");
            Console.WriteLine("- buildcpp    builds the C++ part of USharp");
            Console.WriteLine("- engine      set the target engine path for current engine version (e.g. '{0}')", ExampleEnginePath);
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
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                CopyFilesRecursive(source, target.CreateSubdirectory(dir.Name), overwrite);
            }
            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name), overwrite);
            }
        }

        static string GetEnginePathFromCurrentFolder()
        {
            // Check upwards for /Epic Games/ENGINE_VERSION/Engine/Plugins/USharp/ and extract the path from there
            string[] parentFolders = { "BuildTool", "USharp", "Plugins", "Engine" };
            string currentPath = GetCurrentDirectory();

            DirectoryInfo dir = Directory.GetParent(currentPath);
            for (int i = 0; i < parentFolders.Length; i++)
            {
                if (!dir.Exists || !dir.Name.Equals(parentFolders[i], StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                if (i == 1 && !Directory.Exists(Path.Combine(dir.FullName, "USharp")))
                {
                    // Couldn't find a USharp sub folder, this likely isn't the expected folder setup
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
        /// Returns the main USharp directory from the engine path.
        /// If the USharp content has been dumped straight into the plugins folder this will be /Engine/Plugins/USharp/USharp/.
        /// If the USharp folder is clean this will be /Engine/Plugins/USharp/.
        /// </summary>
        private static string GetUSharpPluginDirectory(string enginePath)
        {
            string pluginFolder = Path.Combine(enginePath, "Engine", "Plugins", "USharp");
            if (Directory.Exists(Path.Combine(pluginFolder, "USharp")) &&
                Directory.Exists(Path.Combine(pluginFolder, "UnrealEngine.Runtime")))
            {
                return Path.Combine(pluginFolder, "USharp");// /Engine/Plugins/USharp/USharp/
            }
            return pluginFolder;// /Engine/Plugins/USharp/
        }

        /// <summary>
        /// Copy over content files which wont be copied during a build (Resources, Sources, USharp.plugin)
        /// </summary>
        /// <param name="forceUpdate">If true force update the content files, otherwise they will only be copied if they don't already exist</param>
        private static void UpdateUSharpPluginContentFiles(bool forceUpdate)
        {
            // If we are in the engine folder we shouldn't have to do anything as the content should be where it needs to be.
            // If we are outside of the engine folder we need to copy the files over.
            string currentFolderEnginePath = GetEnginePathFromCurrentFolder();
            if (string.IsNullOrEmpty(currentFolderEnginePath) && Directory.Exists(settings.EnginePath))
            {
                string usharpPluginsDir = GetUSharpPluginDirectory(settings.EnginePath);
                if (!Directory.Exists(usharpPluginsDir))
                {
                    Directory.CreateDirectory(usharpPluginsDir);
                }

                string relativeUSharpDir = Path.Combine(GetCurrentDirectory(), "../../USharp");

                const string resourcesFolder = "Resources";
                const string pluginFile = "USharp.uplugin";
                const string sourcesDir = "Sources/USharp";
                const string buildFile = "USharp.Build.cs";

                // Copy the Resources folder
                CopyFilesRecursive(new DirectoryInfo(Path.Combine(relativeUSharpDir, resourcesFolder)), 
                    new DirectoryInfo(Path.Combine(usharpPluginsDir, resourcesFolder)), forceUpdate);

                // Copy the USharp.plugin file
                File.Copy(Path.Combine(relativeUSharpDir, pluginFile), Path.Combine(usharpPluginsDir, pluginFile), forceUpdate);

                string targetSourcesDir = Path.Combine(usharpPluginsDir, sourcesDir);
                if (!Directory.Exists(targetSourcesDir))
                {
                    Directory.CreateDirectory(targetSourcesDir);
                }

                // Copy the USharp.Build.cs file
                File.Copy(Path.Combine(relativeUSharpDir, sourcesDir, buildFile), Path.Combine(targetSourcesDir, buildFile), forceUpdate);
            }
        }

        private static void CompileCs(string[] args)
        {
            string slnDir = Path.Combine(GetCurrentDirectory(), "../UnrealEngine.Runtime/");
            if (!Directory.Exists(slnDir))
            {
                Console.WriteLine("Failed to find the UnrealEngine.Runtime directory");
                return;
            }
            
            string loaderProj = Path.Combine(slnDir, "Loader/Loader.csproj");
            string runtimeProj = Path.Combine(slnDir, "UnrealEngine.Runtime/UnrealEngine.Runtime.csproj");
            string assemblyRewriterProj = Path.Combine(slnDir, "UnrealEngine.AssemblyRewriter/UnrealEngine.AssemblyRewriter.csproj");
            string[] projectPaths = { loaderProj, runtimeProj, assemblyRewriterProj };
            string slnPath = Path.Combine(slnDir, "UnrealEngine.Runtime.sln");

            Dictionary<string, string> keyValues = GetKeyValues(args);
            bool shippingBuild = keyValues.ContainsKey("shipping");
            bool x86Build = keyValues.ContainsKey("x86");

            foreach (string projPath in projectPaths)
            {
                if (!File.Exists(projPath))
                {
                    Console.WriteLine("Failed to find C# project '{0}'", projPath);
                    return;
                }
            }

            foreach (string projPath in projectPaths)
            {
                string customDefines = null;
                if (shippingBuild && projPath == runtimeProj)
                {
                    // This is to clear the editor defines (WITH_EDITORONLY_DATA) which gives us a runtime FName struct
                    customDefines = "BLANK_DEFINES";
                }

                if (!BuildCs(slnPath, projPath, !shippingBuild, x86Build, customDefines))
                {
                    Console.WriteLine("Failed to build " + Path.GetFileName(projPath));
                    break;
                }
                else
                {
                    Console.WriteLine("Build successful");
                }
            }

            // If we are in the engine folder we shouldn't have to do anything as the binaries should be in the correct location.
            // If we are outside of the engine folder we need to copy the binaries over to the engine plugins folder.
            string currentFolderEnginePath = GetEnginePathFromCurrentFolder();
            if (string.IsNullOrEmpty(currentFolderEnginePath) && Directory.Exists(settings.EnginePath))
            {
                string relativeBinariesDir = Path.Combine(GetCurrentDirectory(), "../../USharp/Binaries/Managed");
                if (Directory.Exists(relativeBinariesDir))
                {                    
                    string usharpBinariesDir = Path.Combine(GetUSharpPluginDirectory(settings.EnginePath), "Binaries/Managed");
                    if (!Directory.Exists(usharpBinariesDir))
                    {
                        // Make sure the target binaries folder exists
                        Directory.CreateDirectory(usharpBinariesDir);
                    }

                    // Copy the files recursively (hopefully this doesn't cause any issues - maybe limit to 1 level deep for AssemblyRewriter)
                    CopyFilesRecursive(new DirectoryInfo(relativeBinariesDir), new DirectoryInfo(usharpBinariesDir), true);
                }
            }
        }

        private static string FindMsBuildPath()
        {
            try
            {
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
            if (string.IsNullOrEmpty(msbuildPath))
            {
                msbuildPath = FindMsBuildPath();
            }

            if (string.IsNullOrEmpty(msbuildPath))
            {
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

            const string buildLogFile = "build.log";

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

            string pluginName = "USharp";
            string targetPrefix = "UE4Editor";

            string batchFileName = "RunUAT.bat";
            string pluginDir = Path.Combine(GetCurrentDirectory(), "../../USharp/");
            string enginePluginsDir = Path.Combine(settings.EnginePath, "Engine/Plugins/");
            string batchFilesDir = Path.Combine(settings.EnginePath, "Engine/Build/BatchFiles/");
            string batchPath = Path.Combine(batchFilesDir, batchFileName);

            string projectBaseDir = Path.Combine(pluginDir, pluginName);
            string projectBaseDirEngine = Path.Combine(enginePluginsDir, pluginName);
            string pluginPath = Path.Combine(projectBaseDir, pluginName + ".uplugin");
            string outputDir = Path.Combine(projectBaseDir, "Build");

            if (!File.Exists(pluginPath))
            {
                Console.WriteLine("Failed to compile C++ project. Couldn't find the plugin '{0}'", pluginPath);
                return;
            }

            // If we are in the engine folder we have to have a build target of outside of the /Engine/ folder (limitation of UBT)
            string currentFolderEnginePath = GetEnginePathFromCurrentFolder();
            bool isInsideEngineFolder = !string.IsNullOrEmpty(currentFolderEnginePath);

            // In 4.20 if it detects that the plugin already exists our compilation will fail (even if we are compiling in-place!)
            // Therefore we need to rename the existing .uplugin file to have a different extension so that UBT doesn't complain.
            // NOTE: For reference the build error is "Found 'USharp' plugin in two locations" ... "Plugin names must be unique."
            string existingPluginFile = Path.Combine(projectBaseDirEngine, pluginName + ".uplugin");
            string tempExistingPluginFile = null;
            if (File.Exists(existingPluginFile))
            {
                tempExistingPluginFile = Path.ChangeExtension(existingPluginFile, ".uplugin_temp");
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
                    // Copy files to the engine plugins dir (may be different to plugin dir)
                    CopyPluginFile(pluginName, projectBaseDirEngine, outputDir, "dll", targetPrefix);
                    CopyPluginFile(pluginName, projectBaseDirEngine, outputDir, "pdb", targetPrefix);
                    CopyPluginFile("UE4-" + pluginName, projectBaseDirEngine, outputDir, "lib", targetPrefix, false);
                    CopyPluginFile("UE4-" + pluginName + "-Win64-Shipping", projectBaseDirEngine, outputDir, "lib", targetPrefix, false);
                    CopyPluginFile(targetPrefix, projectBaseDirEngine, outputDir, "modules", targetPrefix, false);
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempExistingPluginFile))
                {
                    File.Move(tempExistingPluginFile, existingPluginFile);
                }
            }
        }

        static void CopyPluginFile(string pluginName, string projectBaseDir, string outputDir, string extension, string targetPrefix, bool includePrefix = true)
        {
            string editorDllName = pluginName + "." + extension;
            if (includePrefix)
            {
                editorDllName = targetPrefix + "-" + editorDllName;
            }
            string win64BinDir = Path.Combine(projectBaseDir, @"Binaries\Win64");
            string outputDllPath = Path.Combine(outputDir, @"Binaries\Win64\" + editorDllName);
            bool copiedFile = false;
            string copyFileError = null;
            if (!Directory.Exists(win64BinDir))
            {
                try
                {
                    Directory.CreateDirectory(win64BinDir);
                }
                catch
                {
                }
            }
            if (Directory.Exists(win64BinDir) && File.Exists(outputDllPath))
            {
                try
                {
                    File.Copy(outputDllPath, Path.Combine(win64BinDir, editorDllName), true);
                    copiedFile = true;
                }
                catch (Exception e)
                {
                    copyFileError = e.ToString();
                }
            }
            else
            {
                copyFileError = "Directory or " + extension + " not found. (" + editorDllName + ")";
            }
            if (!copiedFile)
            {
                Console.WriteLine("Failed to copy " + extension + ". Error: " + copyFileError);
            }
        }
    }

    // Save the engine path so we don't have to prompt for it each time
    class Settings
    {
        public string EnginePath { get; set; }

        public static Settings Load()
        {
            return new Settings();
        }

        public void Save()
        {
        }
    }
}

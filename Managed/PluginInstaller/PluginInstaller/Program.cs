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
        private static string msbuildPath;

        public static readonly bool IsLinux;
        public static readonly bool IsMacOS;
        public static readonly bool IsWindows;

        /// <summary>
        /// The directory path of PluginInstaller.exe
        /// </summary>
        public static string AppDirectory;

        static Program()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    if (File.Exists("/usr/lib/libc.dylib") && File.Exists("/System/Library/CoreServices/SystemVersion.plist"))
                    {
                        // This isn't exactly fool proof but msbuild does similar in NativeMethodsShared.cs
                        IsMacOS = true;
                    }
                    else
                    {
                        IsLinux = true;
                    }
                    break;
                default:
                    IsWindows = true;
                    break;
            }
        }

        static void Main(string[] args)
        {
            AppDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            string enginePath = GetEnginePathFromCurrentFolder();
            if (string.IsNullOrEmpty(enginePath) || !Directory.Exists(enginePath))
            {
                Console.WriteLine("Failed to find the engine folder! Make sure PluginInstaller.exe is under /Engine/Plugins/USharp/Binaries/Managed/PluginInstaller/PluginInstaller.exe");
                Console.ReadLine();
                return;
            }
            
            PrintHelp();
            Console.WriteLine();

            Console.WriteLine("Targeting engine version '" + new DirectoryInfo(enginePath).Name + "'");
            Console.WriteLine();

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

                    case "copyruntime":
                        if (args.Length >= 2)
                        {
                            bool minimal = args.Length >= 3 && args[2].ToLower() == "min";
                            string configFile = NetRuntimeHelper.GetConfigFile(minimal);

                            if (File.Exists(configFile))
                            {
                                switch (args[1].ToLower())
                                {
                                    case "all":
                                        NetRuntimeHelper.CopyAll(minimal);
                                        Console.WriteLine("done");
                                        break;
                                    case "mono":
                                        NetRuntimeHelper.CopyMono(minimal);
                                        Console.WriteLine("done");
                                        break;
                                    case "coreclr":
                                        NetRuntimeHelper.CopyCoreCLR(minimal);
                                        Console.WriteLine("done");
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Couldn't find '" +  configFile + "'");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Expected one of the following: all, mono, coreclr");
                        }
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
            Console.WriteLine("- build       builds C# and C++ projects");
            Console.WriteLine("- buildcs     builds C# projects (Loader, AssemblyRewriter, Runtime)");
            Console.WriteLine("- buildcpp    builds C++ projects");
            Console.WriteLine("- copyruntime [all] [mono] [coreclr] copies the given runtime(s) locally");
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

        private static string GetCurrentDirectory()
        {
            return AppDirectory;
        }

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

        internal static void CopyFilesRecursive(DirectoryInfo source, DirectoryInfo target, bool overwrite)
        {
            CopyFiles(source, target, overwrite, true);
        }

        internal static void CopyFile(string sourceFileName, string destFileName, bool overwrite)
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

        /// <summary>
        /// Returns the main USharp plugin directory from the engine path
        /// </summary>
        private static string GetUSharpPluginDirectory(string enginePath)
        {
            return Path.Combine(enginePath, "Engine", "Plugins", "USharp");
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
        }

        private static bool BuildCs(string solutionPath, string projectPath, bool debug, bool x86, string customDefines)
        {
            string buildLogFile = Path.Combine(GetCurrentDirectory(), "build.log");

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
                msbuildPath = NetRuntimeHelper.FindMsBuildPath();
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
                    UseShellExecute = false,
                    CreateNoWindow = true
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

            string enginePath = GetEnginePathFromCurrentFolder();
            string batchFileName = "RunUAT" + (IsWindows ? ".bat" : ".sh");
            string batchFilesDir = Path.Combine(enginePath, "Engine/Build/BatchFiles/");
            string batchPath = Path.Combine(batchFilesDir, batchFileName);
            
            string pluginDir = Path.Combine(enginePath, "Engine/Plugins/", pluginName);
            string pluginPath = Path.Combine(pluginDir, pluginName + pluginExtension);
            //string outputDir = Path.Combine(projectBaseDir, "Build");

            if (!File.Exists(batchPath))
            {
                Console.WriteLine("Failed to compile C++ project. Couldn't find RunUAT at '" + batchPath + "'");
                return;
            }

            try
            {
                if (!File.Exists(pluginPath))
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
            string existingPluginFile = Path.Combine(pluginDir, pluginName + pluginExtension);
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

            // Since we are compiling from within the engine plugin folder make sure to use temp changed .plugin_temp file
            pluginPath = tempExistingPluginFile;

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
                                new DirectoryInfo(Path.Combine(pluginDir, dir)), true);
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
}

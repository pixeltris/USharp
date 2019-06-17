using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;
using UnrealEngine.Engine;
using System.IO;
using System.Reflection;

namespace UnrealEngine.Runtime
{
    public partial class CodeGenerator
    {
        // For invoking the MSBuild compiler
        private static bool pluginInstallerLoaded = false;
        private static MethodInfo pluginInstallerBuildSlnMethod;

        internal static void OnNativeFunctionsRegistered()
        {
            IConsoleManager.Get().RegisterConsoleCommand("USharpGen", "USharp generate C# code", GenerateCode);

            // Move these commands somewhere else?
            IConsoleManager.Get().RegisterConsoleCommand("USharpRuntime", "Sets the .NET runtime that USharp will use (Mono/CLR)", SetDotNetRuntime);
            IConsoleManager.Get().RegisterConsoleCommand("USharpMinHotReload", "USharp hotreload will skip reintancing / CDO checks", SetMinimalHotReload);
        }

        private static unsafe void SetDotNetRuntime(string[] args)
        {
            // It's probably possible to dynamically load runtimes by adding a function to SharedRuntimeState (and slightly modifying
            // the current load checks inside of CSharpLoader::Load)
            const string enableMoreRuntimesStr = 
                "Only one runtime has been loaded. Modify /USharp/Binaries/Managed/Runtimes/DotNetRuntime.txt to add more runtimes and then reopen the editor.";

            if (args != null && args.Length > 0)
            {
                bool handled = true;
                switch (args[0].ToLower())
                {
                    case "diag":
                        {
                            // Diagnostics...
                            FMessage.Log("============================================================================================");
                            FMessage.Log("Loaded modules in " + SharedRuntimeState.CurrentRuntime + ":");
                            foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                            {
                                FMessage.Log(assembly.FullName);
                            }

                            // Also print out the loaded AppDomain instances when under the .NET Framework (CLR)
                            if (AssemblyContext.IsCLR)
                            {
                                FMessage.Log("================ Domains:");
                                string[] appDomains = AppDomainDiagnostic.GetNames();
                                if (appDomains != null)
                                {
                                    foreach (string appDomain in appDomains)
                                    {
                                        FMessage.Log(appDomain);
                                    }
                                }
                            }
                        }
                        break;

                    case "reload":
                        {
                            SharedRuntimeState.Instance->Reload = true;
                        }
                        break;

                    default:
                        handled = false;
                        break;
                }

                if (handled)
                {
                    return;
                }

                if (SharedRuntimeState.HaveMultipleRuntimesLoaded())
                {
                    EDotNetRuntime runtime = EDotNetRuntime.None;
                    switch (args[0].ToLower())
                    {
                        case "mono":
                            runtime = EDotNetRuntime.Mono;
                            break;
                        case "clr":
                            runtime = EDotNetRuntime.CLR;
                            break;
                        case "coreclr":
                            runtime = EDotNetRuntime.CoreCLR;
                            break;
                    }
                    if (runtime == EDotNetRuntime.None)
                    {
                        FMessage.Log("Unknown runtime '" + args[0] + "'. Current runtime: " + SharedRuntimeState.GetRuntimeInfo(true));
                    }
                    else if (!SharedRuntimeState.IsRuntimeLoaded(runtime))
                    {
                        FMessage.Log(runtime + " isn't loaded. Current runtime: " + SharedRuntimeState.GetRuntimeInfo(true));
                    }
                    else if (SharedRuntimeState.Instance->NextRuntime != EDotNetRuntime.None)
                    {
                        FMessage.Log("Runtime change already queued (" + SharedRuntimeState.Instance->NextRuntime.ToString() + ")");
                    }
                    else if (SharedRuntimeState.Instance->Reload)
                    {
                        FMessage.Log("The active runtime is currently reloading (" + SharedRuntimeState.Instance->ActiveRuntime + ")");
                    }
                    else if (SharedRuntimeState.Instance->ActiveRuntime == runtime)
                    {
                        FMessage.Log(runtime.ToString() + " is already the active runtime");
                    }
                    else
                    {
                        SharedRuntimeState.Instance->RuntimeCounter++;
                        SharedRuntimeState.Instance->NextRuntime = runtime;
                        FMessage.Log("Changing runtime to " + runtime.ToString() + "...");
                    }
                }
                else
                {
                    FMessage.Log(ELogVerbosity.Error, enableMoreRuntimesStr);
                    FMessage.Log("Runtime: " + SharedRuntimeState.GetRuntimeInfo(true));
                }
            }
            else
            {
                if (!SharedRuntimeState.HaveMultipleRuntimesLoaded())
                {
                    FMessage.Log(enableMoreRuntimesStr);
                }
                FMessage.Log("Runtime: " + SharedRuntimeState.GetRuntimeInfo(true));
            }
        }

        private static void SetMinimalHotReload(string[] args)
        {
            bool value;
            if (args != null && args.Length >= 1 && bool.TryParse(args[0], out value))
            {
            }
            else
            {
                value = !Native_SharpHotReloadUtils.Get_MinimalHotReload();
            }
            Native_SharpHotReloadUtils.Set_MinimalHotReload(value);
            FMessage.Log("USharpMinHotReload: " + (value ? "Enabled" : "Disabled"));
        }

        internal static void GenerateCode(string[] args)
        {
            try
            {
                bool invalidArgs = false;

                if (args.Length > 0)
                {
                    CodeGenerator codeGenerator = null;

                    switch (args[0])
                    {
                        /*case "blueprints":
                            AssetLoadMode loadMode = AssetLoadMode.Game;
                            bool clearAssetCache = false;
                            bool skipLevels = false;
                            if (args.Length > 1)
                            {
                                switch (args[1])
                                {
                                    case "game":
                                        loadMode = CodeGenerator.AssetLoadMode.Game;
                                        break;
                                    case "gameplugins":
                                        loadMode = CodeGenerator.AssetLoadMode.GamePlugins;
                                        break;
                                    case "engine":
                                        loadMode = CodeGenerator.AssetLoadMode.Engine;
                                        break;
                                    case "engineplugins":
                                        loadMode = CodeGenerator.AssetLoadMode.EnginePlugins;
                                        break;
                                    case "all":
                                        loadMode = CodeGenerator.AssetLoadMode.All;
                                        break;
                                }
                            }
                            if (args.Length > 2)
                            {
                                bool.TryParse(args[2], out clearAssetCache);
                            }
                            if (args.Length > 3)
                            {
                                bool.TryParse(args[3], out skipLevels);
                            }
                            codeGenerator = new CodeGenerator();
                            codeGenerator.GenerateCodeForBlueprints(loadMode, clearAssetCache, skipLevels);
                            break;*/

                        case "game":
                            codeGenerator = new CodeGenerator();
                            codeGenerator.GenerateCodeForModules(new UnrealModuleType[] { UnrealModuleType.Game });
                            break;

                        case "gameplugins":
                            codeGenerator = new CodeGenerator();
                            codeGenerator.GenerateCodeForModules(new UnrealModuleType[] { UnrealModuleType.GamePlugin });
                            break;

                        case "modules":
                            // Engine modules (whitelisted)
                            codeGenerator = new CodeGenerator();
                            //codeGenerator.Settings.ExportMode = CodeGeneratorSettings.CodeExportMode.All;
                            //codeGenerator.Settings.ExportAllFunctions = true;
                            //codeGenerator.Settings.ExportAllProperties = true;
                            string whitelistFile = Path.Combine(codeGenerator.Settings.GetManagedPluginSettingsDir(), "ModulesWhitelist.txt");
                            string blacklistFile = Path.Combine(codeGenerator.Settings.GetManagedPluginSettingsDir(), "ModulesBlacklist.txt");
                            if (File.Exists(whitelistFile))
                            {
                                foreach (string line in File.ReadAllLines(whitelistFile))
                                {
                                    if (!string.IsNullOrEmpty(line))
                                    {
                                        codeGenerator.ModulesNamesWhitelist.Add(line);
                                    }
                                }
                            }
                            if (File.Exists(blacklistFile))
                            {
                                foreach (string line in File.ReadAllLines(blacklistFile))
                                {
                                    if (!string.IsNullOrEmpty(line))
                                    {
                                        codeGenerator.ModulesNamesBlacklist.Add(line);
                                    }
                                }
                            }
                            codeGenerator.GenerateCodeForEngineModules();
                            break;

                        case "all_modules":
                            codeGenerator = new CodeGenerator();
                            codeGenerator.GenerateCodeForAllModules();
                            break;

                        case "engine_modules":
                            codeGenerator = new CodeGenerator();
                            codeGenerator.GenerateCodeForEngineModules();
                            break;

                        case "module":
                            if (args.Length > 1)
                            {
                                codeGenerator = new CodeGenerator();
                                // Tests / using these for types for use in this lib
                                //codeGenerator.Settings.CheckUObjectDestroyed = false;
                                //codeGenerator.Settings.GenerateIsValidSafeguards = false;
                                //codeGenerator.Settings.ExportMode = CodeGeneratorSettings.CodeExportMode.All;
                                //codeGenerator.Settings.ExportAllFunctions = true;
                                //codeGenerator.Settings.ExportAllProperties = true;
                                //codeGenerator.Settings.MergeEnumFiles = false;
                                codeGenerator.GenerateCodeForModule(args[1], true);
                            }
                            else
                            {
                                invalidArgs = true;
                            }
                            break;

                        case "compile":
                            CompileGeneratedCode();
                            break;

                        default:
                            invalidArgs = true;
                            break;
                    }
                }
                else
                {
                    invalidArgs = true;
                }

                if (invalidArgs)
                {
                    FMessage.Log(ELogVerbosity.Warning, "Invalid input. Provide one of the following: game, gameplugins, modules, module [ModuleName], compile");
                }
            }
            catch (Exception e)
            {
                FMessage.Log(ELogVerbosity.Error, "Generate code failed. Error: \n" + e);
            }
        }

        internal static bool CompileGeneratedCode()
        {
            CodeGeneratorSettings settings = new CodeGeneratorSettings();
            string slnPath = Path.GetFullPath(Path.Combine(settings.GetManagedModulesDir(), "UnrealEngine.sln"));
            string projPath = Path.GetFullPath(Path.Combine(settings.GetManagedModulesDir(), "UnrealEngine.csproj"));
            return CompileCode(slnPath, projPath);
        }

        internal static bool CompileCode(string slnPath, string projPath)
        {
            CodeGeneratorSettings settings = new CodeGeneratorSettings();
            string pluginInstallerPath = Path.GetFullPath(Path.Combine(settings.GetManagedBinDir(), "PluginInstaller", "PluginInstaller.exe"));

            if (!File.Exists(slnPath))
            {
                CommandLog(ELogVerbosity.Error, "The solution '" + slnPath + "' doesn't exist");
                return false;
            }
            if (!string.IsNullOrEmpty(projPath) && !File.Exists(projPath))
            {
                CommandLog(ELogVerbosity.Error, "The project '" + projPath + "' doesn't exist");
                return false;
            }
            if (!File.Exists(pluginInstallerPath))
            {
                CommandLog(ELogVerbosity.Error, "Plugin installer not found at '" + pluginInstallerPath + "'");
                return false;
            }

            const string typeName = "PluginInstaller.Program";
            const string methodName = "BuildCustomSolution";

            if (pluginInstallerBuildSlnMethod == null)
            {
                if (!pluginInstallerLoaded)
                {
                    pluginInstallerLoaded = true;

                    Assembly assembly = CurrentAssemblyContext.LoadFrom(pluginInstallerPath);
                    if (assembly == null)
                    {
                        CommandLog(ELogVerbosity.Error, "Failed to load the plugin installer at '" + pluginInstallerPath + "'.");
                        return false;
                    }

                    Type type = assembly.GetType(typeName);
                    if (type == null)
                    {
                        CommandLog(ELogVerbosity.Error, "Failed to resolve the plugin installer type '" + typeName + "'.");
                        return false;
                    }

                    // Set the AppDirectory path so that it can resolve the local msbuild path (if a local msbuild exists)
                    type.GetField("AppDirectory", BindingFlags.Public | BindingFlags.Static).SetValue(
                        null, Path.GetDirectoryName(pluginInstallerPath));

                    pluginInstallerBuildSlnMethod = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
                }

                if (pluginInstallerBuildSlnMethod == null)
                {
                    CommandLog(ELogVerbosity.Error, "Failed to resolve the '" + methodName + "' function in plugin installer.");
                    return false;
                }
            }

            CommandLog(ELogVerbosity.Log, "Attempting to build generated solution at " + slnPath);

            bool built = false;
            using (FScopedSlowTask slowTask = new FScopedSlowTask(100, "Compiling..."))
            {
                slowTask.MakeDialog();

                try
                {
                    built = (bool)pluginInstallerBuildSlnMethod.Invoke(null, new object[] { slnPath, projPath });
                    if (built)
                    {
                        CommandLog(ELogVerbosity.Log, "Solution was compiled successfully.");
                    }
                    else
                    {
                        CommandLog(ELogVerbosity.Error, "There was an error building the solution. Try compiling manually at " + slnPath);
                    }
                }
                catch (Exception e)
                {
                    CommandLog(ELogVerbosity.Error, "'" + methodName + "' throw an exception whilst compiling: " + e);
                }

                // This will give us one frame of 100% rather than always showing 0% (is there an alternative dialog for unknown task lengths?)
                slowTask.EnterProgressFrame(99.9f);
                slowTask.EnterProgressFrame(0.1f);
            }
            return built;
        }

        private static void CommandLog(string value, params object[] args)
        {
            CommandLog(ELogVerbosity.Log, value, args);
        }

        private static void CommandLog(ELogVerbosity verbosity, string value, params object[] args)
        {
            FMessage.Log("USharp-CodeGenerator.Commands", verbosity, string.Format(value, args));
        }
    }
}

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
        private static CodeGenerator timeSlicedCodeGenerator;

        // For invoking the MSBuild compiler
        private static bool pluginInstallerLoaded = false;
        private static MethodInfo pluginInstallerBuildSlnMethod;

        internal static void OnNativeFunctionsRegistered()
        {
            IConsoleManager.Get().RegisterConsoleCommand("USharpGen", "USharp generate C# code", GenerateCode);
            //IConsoleManager.Get().RegisterConsoleCommand("USharpGenSliced", "USharp generate C# code", GenerateCodeTimeSliced);

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

        private static void GenerateCode(string[] args)
        {
            GenerateCode(false, args);
        }

        private static void CompileGeneratedCode()
        {
            CodeGeneratorSettings settings = new CodeGeneratorSettings();
            string slnPath = Path.GetFullPath(Path.Combine(settings.GetManagedModulesDir(), "UnrealEngine.sln"));
            string projPath = Path.GetFullPath(Path.Combine(settings.GetManagedModulesDir(), "UnrealEngine.csproj"));
            string pluginInstallerPath = Path.GetFullPath(Path.Combine(settings.GetManagedBinDir(), "PluginInstaller", "PluginInstaller.exe"));

            if (!File.Exists(slnPath))
            {
                CommandLog(ELogVerbosity.Error, "The solution '" + slnPath + "' doesn't exist");
                return;
            }
            if (!File.Exists(projPath))
            {
                CommandLog(ELogVerbosity.Error, "The project '" + projPath + "' doesn't exist");
                return;
            }
            if (!File.Exists(pluginInstallerPath))
            {
                CommandLog(ELogVerbosity.Error, "Plugin installer not found at '" + pluginInstallerPath + "'");
                return;
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
                        return;
                    }
                    
                    Type type = assembly.GetType(typeName);
                    if (type == null)
                    {
                        CommandLog(ELogVerbosity.Error, "Failed to resolve the plugin installer type '" + typeName + "'.");
                        return;
                    }

                    // Set the AppDirectory path so that it can resolve the local msbuild path (if a local msbuild exists)
                    type.GetField("AppDirectory", BindingFlags.Public | BindingFlags.Static).SetValue(
                        null, Path.GetDirectoryName(pluginInstallerPath));
                    
                    pluginInstallerBuildSlnMethod = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
                }

                if (pluginInstallerBuildSlnMethod == null)
                {
                    CommandLog(ELogVerbosity.Error, "Failed to resolve the '" + methodName + "' function in plugin installer.");
                    return;
                }
            }

            CommandLog(ELogVerbosity.Log, "Attempting to build generated solution at " + slnPath);
            
            try
            {
                bool built = (bool)pluginInstallerBuildSlnMethod.Invoke(null, new object[] { slnPath, projPath });
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
        }

        private static void GenerateCodeTimeSliced(string[] args)
        {
            GenerateCode(true, args);
        }

        private static void GenerateCode(bool timeSliced, string[] args)
        {
            try
            {
                bool invalidArgs = false;

                if (args.Length > 0)
                {
                    if (args[0] == "cancel")
                    {
                        if (timeSlicedCodeGenerator != null && !timeSlicedCodeGenerator.Complete)
                        {
                            timeSlicedCodeGenerator.EndGenerateModules();
                        }
                        timeSlicedCodeGenerator = null;
                        return;
                    }

                    if (timeSlicedCodeGenerator != null)
                    {
                        FMessage.Log("Already generating code");
                        return;
                    }

                    CodeGenerator codeGenerator = null;

                    switch (args[0])
                    {
                        case "game":
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
                                    case "engine":
                                        loadMode = CodeGenerator.AssetLoadMode.Engine;
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
                            codeGenerator = new CodeGenerator(timeSliced);
                            codeGenerator.GenerateCodeForGame(loadMode, clearAssetCache, skipLevels);
                            break;

                        case "gameplugins":
                            codeGenerator = new CodeGenerator(timeSliced);
                            codeGenerator.GenerateCodeForModules(new UnrealModuleType[] { UnrealModuleType.GamePlugin });
                            break;

                        case "modules":
                            codeGenerator = new CodeGenerator(timeSliced);
                            //codeGenerator.Settings.ExportMode = CodeGeneratorSettings.CodeExportMode.All;
                            //codeGenerator.Settings.ExportAllFunctions = true;
                            //codeGenerator.Settings.ExportAllProperties = true;
                            codeGenerator.GenerateCodeForAllModules();
                            break;

                        case "module":
                            if (args.Length > 1)
                            {
                                codeGenerator = new CodeGenerator(timeSliced);
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

                    if (!invalidArgs && timeSliced && codeGenerator != null)
                    {
                        timeSlicedCodeGenerator = codeGenerator;
                        Coroutine.StartCoroutine(null, ProcessTimeSlice());
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
            catch(Exception e)
            {
                FMessage.Log(ELogVerbosity.Error, "Generate code failed. Error: \n" + e);
            }
        }

        private static System.Collections.IEnumerator ProcessTimeSlice()
        {
            if (timeSlicedCodeGenerator != null && !timeSlicedCodeGenerator.Complete && timeSlicedCodeGenerator.TimeSliced)
            {
                while (timeSlicedCodeGenerator != null && !timeSlicedCodeGenerator.Complete)
                {
                    timeSlicedCodeGenerator.Process();
                    yield return null;
                }
                if (timeSlicedCodeGenerator != null && timeSlicedCodeGenerator.Complete)
                {
                    timeSlicedCodeGenerator = null;
                }
                yield break;
            }
            else
            {
                timeSlicedCodeGenerator = null;
                yield break;
            }
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

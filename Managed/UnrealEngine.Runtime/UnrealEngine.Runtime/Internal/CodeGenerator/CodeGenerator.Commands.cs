using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;
using UnrealEngine.Engine;

namespace UnrealEngine.Runtime
{
    public partial class CodeGenerator
    {
        private static CodeGenerator timeSlicedCodeGenerator = null;

        internal static void OnNativeFunctionsRegistered()
        {
            IConsoleManager.Get().RegisterConsoleCommand("USharpGen", "USharp generate C# code", GenerateCode);
            //IConsoleManager.Get().RegisterConsoleCommand("USharpGenSliced", "USharp generate C# code", GenerateCodeTimeSliced);

            IConsoleManager.Get().RegisterConsoleCommand("USharpMinHotReload", "USharp hotreload will skip reintancing / CDO checks", SetMinimalHotReload);
            IConsoleManager.Get().RegisterConsoleCommand("USharpCreateManagedSolution", "USharp Generate Managed Solution For Game Project", GenerateManagedSolutionForGameProject);
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

        private static void GenerateManagedSolutionForGameProject(string[] args)
        {
            CodeGenerator codeGenerator = new CodeGenerator(false);
            CodeManager codeManager = CodeManager.Create(codeGenerator);
            codeManager.OnBeginGenerateModules();
            if(codeManager.AddProjectFile(codeManager.GameSlnPath, codeManager.GameProjPath))
            {
                var _modulesDir = new System.IO.DirectoryInfo(codeManager.Settings.GetManagedModulesDir());
                string _modulesFullPath = "";
                if (_modulesDir != null)
                {
                    _modulesFullPath = _modulesDir.FullName;
                }
                string _exampleScriptCopyFromPath = System.IO.Path.Combine(_modulesFullPath, @"..\..\..\Managed\Examples\", "Test" + ".cs");
                if (System.IO.File.Exists(_exampleScriptCopyFromPath))
                {
                    string _exampleCode;
                    using(System.IO.StreamReader _reader = new System.IO.StreamReader(_exampleScriptCopyFromPath))
                    {
                        _exampleCode = _reader.ReadToEnd();
                    }
                    string _exampleScriptCopyToPath = System.IO.Path.Combine(codeManager.Settings.GetManagedDir(), "Source", "Test" + ".cs");
                    codeManager.AddSourceFile(codeManager.GameSlnPath, codeManager.GameProjPath, _exampleScriptCopyToPath, _exampleCode, true);
                }
            }
        }

        private static void GenerateCode(string[] args)
        {
            GenerateCode(false, args);
        }

        private static void GenerateCodeTimeSliced(string[] args)
        {
            GenerateCode(true, args);
        }

        private static void GenerateCode(bool timeSliced, string[] args)
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
                FMessage.Log(ELogVerbosity.Warning, "Invalid input. Provide one of the following: game, gameplugins, modules, module [ModuleName]");
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
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime.Native
{
    // #pragma warning disable 649 - field is never assigned
    // #pragma warning disable 108 - hiding inherited member (ToString)

    static class NativeFunctions
    {
        // <NativeFunctionName, FieldInfo(Delegate)>
        // When our callback is called we do a lookup using the NativeFunctionName
        // and then create a delegate from the given function pointer (based on the
        // FieldInfo.FieldType) and assign the delegate value to that FieldInfo
        private static Dictionary<string, FieldInfo> functions = new Dictionary<string, FieldInfo>();

        /// <summary>
        /// Function defined in native code which calls the managed callback RegisterFunction on all native functions to be registerd
        /// </summary>
        /// <param name="registerFuncCallback">Function pointer to our RegisterFunction method in managed code</param>
        delegate void Del_RegisterFunctions(IntPtr registerFuncCallback);

        /// <summary>
        /// Managed RegisterFunction callback which is called from native code for every native function to be registered
        /// </summary>
        /// <param name="func">The native function to be registered</param>
        /// <param name="name">The name of the function</param>
        delegate void Del_RegisterFunction(IntPtr func, string name);

        /// <summary>
        /// Our managed RegisterFunction callback (hold onto a static instance to avoid GC from cleaning it up)
        /// </summary>
        private static Del_RegisterFunction registerFunction = new Del_RegisterFunction(RegisterFunction);

        internal static FFeedbackContext codeGenContext;

        /// <summary>
        /// Register all native functions with managed code
        /// </summary>
        /// <param name="registerFunctionsAddr">The address of the RegisterFunctions method defined in native code</param>
        public static void RegisterFunctions(IntPtr registerFunctionsAddr)
        {
            if (!EntryPoint.Preloaded)
            {
                string namespaceName = typeof(NativeFunctions).Namespace;
                foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (type.IsClass && type.IsAbstract && type.IsSealed && type.Name.StartsWith("Native_") && type.Namespace == namespaceName)
                    {
                        // Native_FName -> Export_FName_XXXXX
                        string nativeFunctionPrefix = "Export" + type.Name.Replace("Native", string.Empty) + "_";

                        foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                        {
                            if (field.IsStatic && field.FieldType.IsSubclassOf(typeof(Delegate)))
                            {
                                functions.Add(nativeFunctionPrefix + field.Name, field);
                            }
                        }
                    }
                }

                // Call the native RegisterFunctions method with our managed RegisterFunction as the callback
                Del_RegisterFunctions registerFunctions = (Del_RegisterFunctions)Marshal.GetDelegateForFunctionPointer(
                    registerFunctionsAddr, typeof(Del_RegisterFunctions));
                
                registerFunctions(Marshal.GetFunctionPointerForDelegate(registerFunction));                

                // Highest (these may be called from a thread other than the game thread, don't access UObject methods)
                FBuild.OnNativeFunctionsRegistered();
                FGlobals.OnNativeFunctionsRegistered();
                Classes.OnNativeFunctionsRegistered();
                BoolMarshaler.OnNativeFunctionsRegistered();
                FStringMarshaler.OnNativeFunctionsRegistered();
                InputCore.FKey.OnNativeFunctionsRegistered();
                FFrame.OnNativeFunctionsRegistered();

                // Validate native struct sizes match the managed struct sizes before running any handlers
                // NOTE: This MUST come after FStringMarshaler.OnNativeFunctionsRegistered as this requires TCHAR size
                StructValidator.ValidateStructs();
            }

            if (!EntryPoint.Preloading)
            {
                Debug.Assert(FThreading.IsInGameThread(), "Load/hotreload should be on the game thread");

                // Highest = Used for initializing very important data required by other functions.
                // VeryHigh = Used for initializing data before UObject classes are loaded.
                // High = This is when UObject classes are loaded. Don't access UObjects at this or any higher priority.
                // Medium = UObject classes are loaded and available to use at this priority.

                // If GEngine is null we need to bind OnPostEngineInit to load anything which requires GEngine
                if (FGlobals.GEngine == IntPtr.Zero)
                {
                    FCoreDelegates.OnPostEngineInit.Bind(OnPostEngineInit);
                }

                // Highest
                GCHelper.OnNativeFunctionsRegistered();
                FMessage.OnNativeFunctionsRegistered();
                Engine.FTimerManagerCache.OnNativeFunctionsRegistered();
                WorldTimeHelper.OnNativeFunctionsRegistered();
                if (FGlobals.GEngine != IntPtr.Zero)
                {
                    // Needs GEngine for binding delegates
                    StaticVarManager.OnNativeFunctionsRegistered();
                    Coroutine.OnNativeFunctionsRegistered();
                }

                // VeryHigh
                NativeReflection.OnNativeFunctionsRegistered();// Requires Classes to be initialized

                // High
                OnNativeFunctionsRegistered();

                // Low
                EngineLoop.OnNativeFunctionsRegistered();

                // Lowest
                CodeGenerator.OnNativeFunctionsRegistered();
#if WITH_USHARP_TESTS
                Tests.Tests.OnNativeFunctionsRegistered();
#endif
            }
        }

        /// <summary>
        /// Callback where GEngine is initialized and is safe to use
        /// </summary>
        private static void OnPostEngineInit()
        {
            StaticVarManager.OnNativeFunctionsRegistered();
            Coroutine.OnNativeFunctionsRegistered();
        }

        private static void RegisterFunction(IntPtr func, string name)
        {
            FieldInfo field;
            if (functions.TryGetValue(name, out field))
            {
                try
                {
                    // Create a delegate from the function pointer and set the FieldInfo value to that delegate
                    field.SetValue(null, Marshal.GetDelegateForFunctionPointer(func, field.FieldType));
                }
                catch (Exception e)
                {
                    string error = "Failed to register native function \"" + name + "\" exception: " + e;
                    Debug.WriteLine(error);
                }
            }
        }

        private static void OnNativeFunctionsRegistered()
        {
            bool reloading = HotReload.IsReloading;
            HotReload.MinimalReload = Native_SharpHotReloadUtils.Get_MinimalHotReload();

            FMessage.Log("Runtime: " + SharedRuntimeState.GetRuntimeInfo(false));

            // HACK: Removing EPackageFlags.EditorOnly on the USharp package so that C# classes aren't tagged as
            //       EObjectMark.EditorOnly. The correct thing to do would be to seperate USharp into seperate 
            //       Editor/Runtime modules.
            IntPtr package = NativeReflection.FindPackage(IntPtr.Zero, "/Script/USharp");
            if (package != IntPtr.Zero)
            {
                Native_UPackage.ClearPackageFlags(package, EPackageFlags.EditorOnly);
            }

            using (var timing = HotReload.Timing.Create(HotReload.Timing.UnrealTypes_Load))
            {
                UnrealTypes.Load();
            }

            if (HotReload.IsReloading)
            {
                HotReload.OnPreReloadBegin();
            }

            using (var timing = HotReload.Timing.Create(HotReload.Timing.UnrealTypes_LoadNative))
            {
                // Load the underlying native type info for generated types (class address/properties/functions/offsets)
                UnrealTypes.LoadNative();
            }

            using (var timing = HotReload.Timing.Create(HotReload.Timing.UClass_Load))
            {
                // Load native classes
                UClass.Load();
            }

            using (var timing = HotReload.Timing.Create(HotReload.Timing.NativeFunctions_GenerateAndCompileMissingAssemblies))
            {
                // Update the C# game project props file
                ProjectProps.Update();

                // Prompt to compile the C# engine wrapper code / C# game code (if it isn't already compiled)
                GenerateAndCompileMissingAssemblies();
            }

            // If any assemblies are loaded make sure to load their unreal types
            if (!AssemblyContext.IsCoreCLR || CurrentAssemblyContext.Reference.IsInvalid)
            {
                // .NET Core should resolve with AssemblyLoadContext.Resolving (unless the contexts aren't set up)
                CurrentAssemblyContext.AssemblyResolve += CurrentDomain_AssemblyResolve;
            }
            CurrentAssemblyContext.AssemblyLoad += OnAssemblyLoad;
            CurrentAssemblyContext.Resolving += CurrentAssemblyContext_Resolving;

            using (var timing = HotReload.Timing.Create(HotReload.Timing.NativeFunctions_LoadAssemblies))
            {
                // Load managed assemblies (game assembly, and any others which may need loading)
                LoadAssemblies();
            }

            if (HotReload.IsReloading)
            {
                HotReload.OnPreReloadEnd();
            }
            
            using (var timing = HotReload.Timing.Create(HotReload.Timing.ManagedUnrealModuleInfo_Load))
            {
                // Load managed module infos
                ManagedUnrealModuleInfo.Load();
            }

            using (var timing = HotReload.Timing.Create(HotReload.Timing.ManagedUnrealTypes_Load))
            {
                // Load / register managed unreal types
                ManagedUnrealTypes.Load();
            }

            using (var timing = HotReload.Timing.Create(HotReload.Timing.HotReload_OnReload))
            {
                // Let HotReload handle reloading if this is a reload
                if (HotReload.IsReloading)
                {
                    HotReload.OnReload();
                }
            }

            // Clear the hot-reload data store if it isn't cleared already
            if (HotReload.Data != null)
            {
                HotReload.Data.Close();
                HotReload.Data = null;
            }
            
            if (FBuild.WithEditor && reloading)
            {
                using (var timing = HotReload.Timing.Create(HotReload.Timing.UObject_CollectGarbage))
                {
                    // If we are hotreloading collect garbage to clean up trashed types / reinstanced objects
                    UObject.CollectGarbage(GCHelper.GarbageCollectionKeepFlags, true);
                }

                if (!ManagedUnrealTypes.SkipBroadcastHotReload)
                {
                    using (var timing = HotReload.Timing.Create(HotReload.Timing.SharpHotReloadUtils_BroadcastOnHotReload))
                    {
                        // Broadcast the native OnHotReload (if we don't do this we would need to reimplement various 
                        // handlers to ensure correct reloading. One example is FBlueprintActionDatabase::ReloadAll 
                        // which needs to be called otherwise the action database will hold onto our old class members
                        // and would produce erros when opening blueprints).

                        // true will show the C++ "Hot Reload Complete!" notification (are there any other differences?)
                        //Native_SharpHotReloadUtils.BroadcastOnHotReload(true);

                        // The notification rendering gets messed up the longer hotreload takes. Wait 1 frame to ensure
                        // that the notification gets fully rendered (though the audio still seems to mess up)
                        Coroutine.StartCoroutine(null, DeferBroadcastHotReload());
                    }
                }
            }

            using (var timing = HotReload.Timing.Create(HotReload.Timing.GC_Collect))
            {
                // We likely created a bunch of garbage, best to clean it up now.
                GC.Collect();
            }
        }

        private static System.Collections.IEnumerator DeferBroadcastHotReload()
        {
            yield return Coroutine.WaitForFrames(1);
            Native_SharpHotReloadUtils.BroadcastOnHotReload(true);
        }

        private static Assembly CurrentAssemblyContext_Resolving(AssemblyName arg)
        {
            return OnAssemblyResolve(arg.FullName);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return OnAssemblyResolve(args.Name);
        }

        private static Assembly OnAssemblyResolve(string assemblyName)
        {
            if (!string.IsNullOrEmpty(UnrealTypes.GameAssemblyDirectory))
            {
                // Strip down the full assembly name down to a name which could be used as the file name
                // "UnrealEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
                for (int i = 0; i < 3; i++)
                {
                    int index = assemblyName.LastIndexOf(',');
                    if (index >= 0)
                    {
                        assemblyName = assemblyName.Substring(0, index);
                    }
                }
                assemblyName += ".dll";

                string assemblyPath = Path.Combine(UnrealTypes.GameAssemblyDirectory, assemblyName);
                if (File.Exists(assemblyPath))
                {
                    // Need to use LoadFrom instead of LoadFile to for shadow copying to work properly
                    Assembly assembly = CurrentAssemblyContext.LoadFrom(assemblyPath);
                    return assembly;
                }
            }
            return null;
        }

        private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Assembly currentAssembly = typeof(NativeFunctions).Assembly;
            Assembly assembly = args.LoadedAssembly;
            AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
            foreach (AssemblyName assemblyName in referencedAssemblies)
            {
                if (assemblyName.FullName == currentAssembly.FullName)
                {
                    try
                    {
                        FMessage.Log("Load managed assembly: '" + assembly.FullName + "'");

                        // This is an unreal assembly. Load the unreal types.
                        ManagedUnrealModuleInfo.PreProcessAssembly(assembly);//UnrealTypes.Load(assembly);
                        UClass.Load(assembly);
                    }
                    catch (Exception e)
                    {
                        FMessage.Log("Failed to process assembly " + assembly.FullName + " error: " + e);

                        // Break if we are debugging as the exception may be silently handled
                        Debugger.Break();
                    }
                    break;
                }
            }
        }

        private static void LoadAssemblies()
        {
            string gameAssemblyFileName = null;
            const string gameAssemblySuffix = ".Managed.dll";

            // Add to assemblyPaths for each possible location the main managed assembly can be.
            // If a managed assembly is found just use the found assembly path (and its references).
            HashSet<string> assemblyPaths = new HashSet<string>();

            EPlatform platform = FPlatformProperties.GetPlatform();
            if (platform == EPlatform.Android)
            {
                string externalFilePath = FGlobals.AndroidFile.ExternalFilePath;
                string managedBinDir = Path.Combine(externalFilePath, "USharp", "Managed");

                string projectName;
                string projectFileName = FPaths.ProjectFilePath;
                if (!string.IsNullOrEmpty(projectFileName))
                {
                    projectName = Path.GetFileNameWithoutExtension(projectFileName);
                }

                string assemblyPath = Path.GetFullPath(Path.Combine(managedBinDir, projectFileName + gameAssemblySuffix));
                assemblyPaths.Add(assemblyPath);

                if (!File.Exists(assemblyPath))
                {
                    assemblyPath = Path.GetFullPath(Path.Combine(managedBinDir, FGlobals.InternalProjectName + gameAssemblySuffix));
                    assemblyPaths.Add(assemblyPath);
                }

                if (File.Exists(assemblyPath))
                {
                    gameAssemblyFileName = assemblyPath;
                }
            }
            else
            {
                string projectFileName = FPaths.ProjectFilePath;
                if (!string.IsNullOrEmpty(projectFileName))
                {
                    projectFileName = Path.GetFileNameWithoutExtension(projectFileName);
                    string projectManagedBinDir = Path.Combine(FPaths.ProjectDir, "Binaries", "Managed");
                    if (FBuild.WithEditor && !Directory.Exists(projectManagedBinDir))
                    {
                        Directory.CreateDirectory(projectManagedBinDir);
                    }
                    string assemblyPath = Path.GetFullPath(Path.Combine(projectManagedBinDir, projectFileName + gameAssemblySuffix));
                    if (File.Exists(assemblyPath))
                    {
                        gameAssemblyFileName = assemblyPath;
                    }
                    assemblyPaths.Add(assemblyPath);
                }

                string projectName = FGlobals.InternalProjectName;
                if (string.IsNullOrEmpty(gameAssemblyFileName) && !string.IsNullOrEmpty(projectName))
                {
                    string path = Path.GetFullPath(projectName + gameAssemblySuffix);
                    assemblyPaths.Add(path);
                    if (!File.Exists(path))
                    {
                        path = Path.GetFullPath(Path.Combine("../", "Managed", projectName + gameAssemblySuffix));
                    }

                    if (File.Exists(path))
                    {
                        gameAssemblyFileName = path;
                    }
                    assemblyPaths.Add(path);
                }
            }

            // We either need to load all unreal assemblies or hold some metadata to know what modules to load dynamically
            // as GCHelper needs to resolve types. Resolving all assemblies is tricky without forcefully loading all dependencies
            // which may be undesirable.
            if (!string.IsNullOrEmpty(gameAssemblyFileName) && File.Exists(gameAssemblyFileName))
            {
                // Clear the assembly paths as we have found a concrete target game assembly
                assemblyPaths.Clear();

                assemblyPaths.Add(gameAssemblyFileName);

                UnrealTypes.GameAssemblyPath = Path.GetFullPath(gameAssemblyFileName);
                UnrealTypes.GameAssemblyDirectory = Path.GetDirectoryName(UnrealTypes.GameAssemblyPath);

                //string pdbFileName = Path.ChangeExtension(gameAssemblyFileName, ".pdb");
                //byte[] assemblyBuffer = File.ReadAllBytes(gameAssemblyFileName);
                //byte[] pdbBuffer = File.Exists(pdbFileName) ? File.ReadAllBytes(pdbFileName) : null;
                //Assembly assembly = Assembly.Load(assemblyBuffer, pdbBuffer);

                // Need to use LoadFrom instead of LoadFile to for shadow copying to work properly
                Assembly assembly = CurrentAssemblyContext.LoadFrom(gameAssemblyFileName);

                string[] dependencies = ResolveAssemblyDependencies(assembly);
                foreach (string assemblyPath in dependencies)
                {
                    assemblyPaths.Add(assemblyPath);
                }
            }

            EntryPoint.HotReloadAssemblyPaths = assemblyPaths.ToArray();
        }
        
        private static string[] ResolveAssemblyDependencies(Assembly assembly)
        {
            return new string[0];
        }

        private static void GenerateAndCompileMissingAssemblies()
        {
            string projectFileName = FPaths.ProjectFilePath;

            if (!FBuild.WithEditor || string.IsNullOrEmpty(projectFileName))
            {
                return;
            }

            // NOTE: This is called before ManagedUnrealModuleInfo.Load / ManagedUnrealTypes.Load so some things may not be accessible.
            //       Maybe call load then unload after this is finished?

            // NOTE: Most of the functions called here use FSlowTask but the UI isn't available until the engine is fully initialized.
            //       Instead use FFeedbackContext to display a progress bar (FDesktopPlatformModule::Get()->GetNativeFeedbackContext())
            // TODO: Redirect some of the FSlowTask messages to the log, so that the "show log" button displays useful info?
            codeGenContext = FFeedbackContext.GetGetDesktopFeedbackContext();

            string dialogTitle = "USharp";

            CodeGeneratorSettings settings = new CodeGeneratorSettings();

            string engineWrapperDllPath = Path.Combine(settings.GetManagedModulesDir(), "bin", "Debug", "UnrealEngine.dll");
            string engineWrapperSlnPath = Path.Combine(settings.GetManagedModulesDir(), "UnrealEngine.sln");
            bool compileEngineWrapperCode = false;
            bool hasCompiledEngineWrapperCode = false;
            if (!File.Exists(engineWrapperSlnPath))
            {
                if (FMessage.OpenDialog(EAppMsgType.YesNo, "C# engine wrapper code not found. Generate it now?", dialogTitle) == EAppReturnType.Yes)
                {
                    codeGenContext.BeginSlowTask("Generating C# engine wrapper code (this might take a while...)", true);
                    CodeGenerator.GenerateCode(new string[] { "modules" });
                    codeGenContext.EndSlowTask();
                    settings.CopyCodeGeneratorVersionFile();
                    compileEngineWrapperCode = true;
                }
            }
            else
            {
                int latestVersion = settings.GetCodeGeneratorVersion(false);
                int version = settings.GetCodeGeneratorVersion(true);
                if (latestVersion > 0 && (version <= 0 || latestVersion > version))
                {
                    if (FMessage.OpenDialog(EAppMsgType.YesNo, "C# engine wrapper code is outdated. Regenerate it?", dialogTitle) == EAppReturnType.Yes)
                    {
                        try
                        {
                            string dir = settings.GetManagedModulesDir();
                            if (Directory.Exists(dir))
                            {
                                Directory.Delete(dir, true);
                            }
                        }
                        catch
                        {
                        }

                        codeGenContext.BeginSlowTask("Generating C# engine wrapper code (this might take a while...)", true);
                        CodeGenerator.GenerateCode(new string[] { "modules" });
                        codeGenContext.EndSlowTask();
                        settings.CopyCodeGeneratorVersionFile();
                        compileEngineWrapperCode = true;
                    }
                }
            }
            if (compileEngineWrapperCode || (!File.Exists(engineWrapperDllPath) && File.Exists(engineWrapperSlnPath)))
            {
                if (compileEngineWrapperCode ||
                    FMessage.OpenDialog(EAppMsgType.YesNo, "C# engine wrapper code isn't compiled. Compile it now?", dialogTitle) == EAppReturnType.Yes)
                {
                    codeGenContext.BeginSlowTask("Compiling C# engine wrapper code (this might take a while...)", true);
                    bool compiled = CodeGenerator.CompileGeneratedCode();
                    codeGenContext.EndSlowTask();

                    if (compiled)
                    {
                        hasCompiledEngineWrapperCode = true;
                    }
                    else
                    {
                        WarnCompileFailed(settings, null, dialogTitle);
                    }
                }
            }

            string projectName = Path.GetFileNameWithoutExtension(projectFileName);
            string gameSlnPath = Path.Combine(settings.GetManagedDir(), projectName + ".Managed.sln");
            string gameDllPath = Path.Combine(FPaths.ProjectDir, "Binaries", "Managed", projectName + ".Managed.dll");

            try
            {
                // Delete the stale C# game code dll if we created new engine wrappers (this should prompt for a compile)
                if (hasCompiledEngineWrapperCode && File.Exists(gameDllPath))
                {
                    File.Delete(gameDllPath);

                }
            }
            catch
            {
            }

            if (!File.Exists(gameSlnPath) &&
                FMessage.OpenDialog(EAppMsgType.YesNo, "USharp is enabled but the C# game project files weren't found. Generate them now?", dialogTitle) == EAppReturnType.Yes)
            {
                TemplateProjectGenerator.Generate();
            }

            if (File.Exists(gameSlnPath) && !File.Exists(gameDllPath) &&
                FMessage.OpenDialog(EAppMsgType.YesNo, "C# game project code isn't compiled. Compile it now?", dialogTitle) == EAppReturnType.Yes)
            {
                codeGenContext.BeginSlowTask("Compiling C# game project code (this might take a while...)", true);
                gameSlnPath = Path.GetFullPath(gameSlnPath);
                bool compiled = CodeGenerator.CompileCode(gameSlnPath, null);
                codeGenContext.EndSlowTask();

                if (!compiled)
                {
                    WarnCompileFailed(settings, gameSlnPath, dialogTitle);
                }
            }

            codeGenContext = null;
        }

        private static void WarnCompileFailed(CodeGeneratorSettings settings, string slnPath, string dialogTitle)
        {
            string logPath = Path.GetFullPath(Path.Combine(settings.GetManagedBinDir(), "PluginInstaller", "build.log"));
            if (string.IsNullOrEmpty(slnPath))
            {
                slnPath = Path.GetFullPath(Path.Combine(settings.GetManagedModulesDir(), "UnrealEngine.sln"));
            }
            FMessage.OpenDialog("Compile failed.\nBuild log: '" + logPath + "'\nsln: '" + slnPath + "'");
        }
    }
}

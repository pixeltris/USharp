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
                StructValidator.ValidateStructs();// Validate native struct sizes match the managed struct sizes before running any handlers
                BoolMarshaler.OnNativeFunctionsRegistered();
                FStringMarshaler.OnNativeFunctionsRegistered();
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
                Engine.FTimerManagerCache.OnNativeFunctionsRegistered();
                if (FGlobals.GEngine != IntPtr.Zero)
                {
                    // StaticVarManager needs GEngine for binding delegates
                    StaticVarManager.OnNativeFunctionsRegistered();
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

            // If any assemblies are loaded make sure to load their unreal types
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;

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
                        Native_SharpHotReloadUtils.BroadcastOnHotReload(true);
                    }
                }
            }

            using (var timing = HotReload.Timing.Create(HotReload.Timing.GC_Collect))
            {
                // We likely created a bunch of garbage, best to clean it up now.
                GC.Collect();
            }
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
            const string gameAssemblySuffix = "-Managed.dll";

            // Add to assemblyPaths for each possible location the main managed assembly can be.
            // If a managed assembly is found just use the found assembly path (and its references).
            HashSet<string> assemblyPaths = new HashSet<string>();

            string projectFileName = FPaths.ProjectFilePath;
            if (!string.IsNullOrEmpty(projectFileName))
            {
                projectFileName = Path.GetFileNameWithoutExtension(projectFileName);
                string projectManagedBinDir = Path.Combine(FPaths.ProjectDir, "Managed", "Binaries");
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

            // We either need to load all unreal assemblies or hold some metadata to know what modules to load dynamically
            // as GCHelper needs to resolve types. Resolving all assemblies is tricky without forcefully loading all dependencies
            // which may be undesirable.
            if (!string.IsNullOrEmpty(gameAssemblyFileName) && File.Exists(gameAssemblyFileName))
            {
                // Clear the assembly paths as we have found a concrete target game assembly
                assemblyPaths.Clear();

                assemblyPaths.Add(gameAssemblyFileName);

                UnrealTypes.GameAssemblyPath = Path.GetFullPath(gameAssemblyFileName);
                
                //string pdbFileName = Path.ChangeExtension(gameAssemblyFileName, ".pdb");
                //byte[] assemblyBuffer = File.ReadAllBytes(gameAssemblyFileName);
                //byte[] pdbBuffer = File.Exists(pdbFileName) ? File.ReadAllBytes(pdbFileName) : null;
                //Assembly assembly = Assembly.Load(assemblyBuffer, pdbBuffer);

                // Need to use LoadFrom instead of LoadFile to for shadow copying to work properly
                Assembly assembly = Assembly.LoadFrom(gameAssemblyFileName);

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
    }

    /// <summary>
    /// Used for bool interop between C# and C++
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct csbool
    {
        private int val;
        public bool Value
        {
            get { return val != 0; }
            set { val = value ? 1 : 0; }
        }

        public csbool(int value)
        {
            val = value == 0 ? 0 : 1;
        }

        public csbool(bool value)
        {
            val = value ? 1 : 0;
        }

        public static implicit operator csbool(bool value)
        {
            return new csbool(value);
        }

        public static implicit operator bool(csbool value)
        {
            return value.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    // BoolInteropNotes:
    // Any structs which we want to pass between managed and native code with bools needs to be properly converted
    // due to sizeof(bool) being implementation defined.
    //
    // Keep this list up to date and check functions are using the proper conversions
    // FImplementedInterface
    // FModuleStatus
    // FCopyPropertiesForUnrelatedObjectsParams
}

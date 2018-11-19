using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

// NOTE: Do not change the name of this (entry point lookup is "UnrealEngine.EntryPoint.DllMain")
namespace UnrealEngine
{
    /// <summary>
    /// This is a basic loader used to load the main assembly along with automatic reloading on assembly file changes
    /// </summary>
    public class EntryPoint
    {
        // Preload the next assembly context (AppDomain) in another thread as this can take ~1 second which would
        // slow down hotreloading if we didn't preload it.
        private static Runtime.AssemblyContextRef preloadedContextRef = Runtime.AssemblyContextRef.Invalid;
        private static AutoResetEvent preloadContextWaitHandle;
        private static bool preloadFailed;
        internal const string preloadEntryPointDataName = "EntryPoint";// Used to cache the entry point method on prereload

        private static Runtime.AssemblyContextRef mainContextRef = Runtime.AssemblyContextRef.Invalid;
        private static int appDomainCount;
        private static string mainAssemblyPath;
        private static string mainAssemblyDirectory;
        private static string currentAssemblyDirectory;
       
        private static string errorMsgBoxTitle = "C# Loader Error";
        private static string unloadErrorMsgBoxTitle = "C# Unload Error";
        
        private static string entryPointType = "UnrealEngine.EntryPoint";
        private static string entryPointMethod = "DllMain";
        private static string entryPointArg = null;
        private static string unloadMethod = "Unload";

        // If true the assembly will be loaded directly (hot hotreload support).
        public static bool LoadAssemblyWithoutContexts = false;

        // If this is true the loader assemblies will be shadow copied.
        public static bool ShadowCopyAssembly = true;

        private static DateTime lastAssemblyUpdate;
        private static Dictionary<string, FileSystemWatcher> assemblyWatchers = new Dictionary<string, FileSystemWatcher>();

        public static int DllMain(string arg)
        {
            try
            {
                Args args = new Args(arg);

                SharedRuntimeState.Initialize((IntPtr)args.GetInt64("RuntimeState"));
                Runtime.AssemblyContext.Initialize();

                mainAssemblyPath = args.GetString("MainAssembly");
                if (!string.IsNullOrEmpty(mainAssemblyPath))
                {
                    if (string.IsNullOrEmpty(mainAssemblyPath) || !File.Exists(mainAssemblyPath))
                    {
                        return (int)AssemblyLoaderError.MainAssemblyNotFound;
                    }
                    mainAssemblyDirectory = Path.GetDirectoryName(mainAssemblyPath);
                }
                else
                {
                    return (int)AssemblyLoaderError.MainAssemblyPathNotProvided;
                }

                IntPtr addTickerAddr = (IntPtr)args.GetInt64("AddTicker");
                IntPtr isInGameThreadAddr = (IntPtr)args.GetInt64("IsInGameThread");
                if (addTickerAddr == IntPtr.Zero || isInGameThreadAddr == IntPtr.Zero)
                {
                    return (int)AssemblyLoaderError.GameThreadHelpersNull;
                }
                GameThreadHelper.Init(addTickerAddr, isInGameThreadAddr, OnRuntimeChanged);

                Debug.Assert(GameThreadHelper.IsInGameThread());

                entryPointArg = arg;

                string currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
                string currentAssemblyFileName = Path.GetFileNameWithoutExtension(currentAssemblyPath);
                currentAssemblyDirectory = Path.GetDirectoryName(currentAssemblyPath);

                if (!IsSameOrSubDirectory(currentAssemblyDirectory, mainAssemblyDirectory))
                {
                    return (int)AssemblyLoaderError.MainAssemblyPathNotProvided;
                }

                // If there is already a loaded runtime only do a pre-load
                if (SharedRuntimeState.GetLoadedRuntimes() != EDotNetRuntime.None)
                {
                    Debug.Assert(mainContextRef.IsInvalid);

                    // Make sure the main assembly path exists
                    if (!File.Exists(mainAssemblyPath))
                    {
                        return (int)AssemblyLoaderError.LoadFailed;
                    }

                    // Make sure we are using assmbly contexts loadding otherwise hotreload wont work which defeats the purpose of
                    // using multiple runtimes
                    if (LoadAssemblyWithoutContexts)
                    {
                        return (int)AssemblyLoaderError.LoadFailed;
                    }

                    // Preload now and then do a full load when NextRuntime is set to this runtime type
                    PreloadNextContext();

                    // Watch for assembly changes (the paths should have been set up by the full load in the other runtime)
                    UpdateAssemblyWatchers();

                    return 0;
                }

                unsafe
                {
                    SharedRuntimeState.Instance->ActiveRuntime = SharedRuntimeState.CurrentRuntime;
                }

                bool loaded;
                if (LoadAssemblyWithoutContexts)
                {
                    loaded = LoadWithoutUsingContexts();
                }
                else
                {
                    loaded = ReloadMainContext();
                }
                if (!loaded)
                {
                    unsafe
                    {
                        SharedRuntimeState.Instance->ActiveRuntime = EDotNetRuntime.None;
                    }
                    return (int)AssemblyLoaderError.LoadFailed;
                }
            }
            catch (Exception e)
            {
                string exceptionStr = "Entry point exception (Loader): " + e;
                if (SharedRuntimeState.Initialized)
                {
                    SharedRuntimeState.LogError(exceptionStr);
                    SharedRuntimeState.MessageBox(exceptionStr, errorMsgBoxTitle);
                }
                return (int)AssemblyLoaderError.Exception;
            }

            return 0;
        }

        private static unsafe void OnRuntimeChanged()
        {
            if (SharedRuntimeState.IsActiveRuntime)
            {
                if (!File.Exists(mainAssemblyPath))
                {
                    SharedRuntimeState.SetHotReloadData(null);                    

                    // Cancel the runtime swap
                    SharedRuntimeState.Instance->NextRuntime = EDotNetRuntime.None;
                    SharedRuntimeState.Instance->IsActiveRuntimeComplete = 0;
                    SharedRuntimeState.Instance->Reload = false;
                    return;
                }

                if (SharedRuntimeState.Instance->Reload)
                {
                    // This is a reload as opposed to a runtime swap, reload it now and return
                    SharedRuntimeState.Instance->Reload = false;
                    ReloadMainContext();
                    return;
                }

                if (!mainContextRef.IsInvalid)
                {
                    UnloadMainContext();
                }
                Debug.Assert(mainContextRef.IsInvalid, "UnloadMainContext failed?");
                Debug.Assert(!LoadAssemblyWithoutContexts, "Assembly context loading is required in order to swap runtimes");
                
                SharedRuntimeState.Instance->IsActiveRuntimeComplete = 1;
            }
            else if (SharedRuntimeState.Instance->NextRuntime == SharedRuntimeState.CurrentRuntime)
            {
                SharedRuntimeState.Instance->ActiveRuntime = SharedRuntimeState.CurrentRuntime;
                SharedRuntimeState.Instance->NextRuntime = EDotNetRuntime.None;
                SharedRuntimeState.Instance->IsActiveRuntimeComplete = 0;
                ReloadMainContext();
            }
        }

        private static void UpdateAssemblyWatchers()
        {
            string[] assemblyPaths = SharedRuntimeState.GetHotReloadAssemblyPaths();
            if (assemblyPaths != null)
            {
                HashSet<string> newAssemblyPaths = new HashSet<string>();
                HashSet<string> removedAssemblyPaths = new HashSet<string>();

                foreach (string assemblyPath in assemblyWatchers.Keys)
                {
                    if (!assemblyPaths.Contains(assemblyPath))
                    {
                        removedAssemblyPaths.Add(assemblyPath);
                    }
                }

                foreach (string assemblyPath in assemblyPaths)
                {
                    if (!assemblyWatchers.ContainsKey(assemblyPath))
                    {
                        newAssemblyPaths.Add(assemblyPath);
                    }
                }

                foreach (string assemblyPath in removedAssemblyPaths)
                {
                    assemblyWatchers[assemblyPath].Dispose();
                    assemblyWatchers.Remove(assemblyPath);
                }

                foreach (string assemblyPath in newAssemblyPaths)
                {
                    if (Directory.Exists(Path.GetDirectoryName(assemblyPath)))
                    {
                        FileSystemWatcher assemblyWatcher = new FileSystemWatcher();
                        assemblyWatcher.Path = Path.GetDirectoryName(assemblyPath);
                        assemblyWatcher.Filter = Path.GetFileName(assemblyPath);
                        assemblyWatcher.NotifyFilter = NotifyFilters.LastWrite;//NotifyFilters.CreationTime;
                        assemblyWatcher.EnableRaisingEvents = true;
                        assemblyWatcher.Changed += AssemblyWatcher_Changed;

                        assemblyWatchers.Add(assemblyPath, assemblyWatcher);
                    }
                }
            }
        }

        private static void AssemblyWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            // Require 500 milliseconds between updates to avoid multiple reloads
            //
            // Note: This may result in a genuine change to be missed which means
            // that some user action will be needed
            //
            // PossibleFix: Make this delayed and only catch the latest one? (will slow
            // reloads based on delay interval)
            if (lastAssemblyUpdate < DateTime.Now - TimeSpan.FromMilliseconds(500))
            {
                bool hasChanged = false;
                try
                {
                    if (File.Exists(e.FullPath))
                    {
                        using (FileStream reader = File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            if (reader.Length > 8)
                            {
                                reader.Position = reader.Length - 8;
                                byte[] buffer = new byte[8];
                                reader.Read(buffer, 0, buffer.Length);
                                long signature = BitConverter.ToInt64(buffer, 0);
                                if (signature == 3110675979262317867)// "+UEsRW++"
                                {
                                    hasChanged = true;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }

                if (hasChanged)
                {
                    ReloadMainContext();
                    lastAssemblyUpdate = DateTime.Now;
                }
            }
        }

        private static bool ReloadMainContext(bool threaded = true)
        {
            if (!GameThreadHelper.IsInGameThread())
            {
                bool result = false;
                GameThreadHelper.Run(delegate { result = ReloadMainContext(); });
                return result;
            }

            if (!SharedRuntimeState.IsActiveRuntime)
            {
                return false;
            }

            if (!File.Exists(mainAssemblyPath))
            {
                SharedRuntimeState.SetHotReloadData(null);
                return false;
            }

            if (!mainContextRef.IsInvalid)
            {
                UnloadMainContext(threaded);
            }

            string entryPointArgEx = entryPointArg;
            bool firstLoad = preloadContextWaitHandle == null;
            if (firstLoad)
            {
                PreloadNextContext(threaded);
            }
            else
            {
                entryPointArgEx += "|Reloading=true";
            }

            preloadContextWaitHandle.WaitOne(Timeout.Infinite);
            preloadContextWaitHandle.Reset();

            if (!preloadFailed)
            {
                Debug.Assert(!preloadedContextRef.IsInvalid, "Preloaded context shouldn't be invalid");

                mainContextRef = preloadedContextRef;
                preloadedContextRef = Runtime.AssemblyContextRef.Invalid;

                entryPointArgEx += "|AssemblyContext=" + mainContextRef.Format();

                try
                {
                    AssemblyLoader loader = new AssemblyLoader(mainAssemblyPath, entryPointType, entryPointMethod, entryPointArgEx, false, mainContextRef);
                    mainContextRef.DoCallBack(loader.Load);
                    UpdateAssemblyWatchers();
                }
                catch (Exception e)
                {
                    MessageBox("Failed to create assembly context for \"" + mainAssemblyPath + "\" " +
                        Environment.NewLine + Environment.NewLine + e, errorMsgBoxTitle);
                }
            }

            PreloadNextContext(threaded);
            SharedRuntimeState.SetHotReloadData(null);
            return true;
        }

        private static bool LoadWithoutUsingContexts()
        {
            try
            {
                AssemblyLoader loader = new AssemblyLoader(
                    mainAssemblyPath, entryPointType, entryPointMethod, entryPointArg, false, Runtime.AssemblyContextRef.Invalid);
                loader.Load();
                return true;
            }
            catch (Exception e)
            {
                MessageBox("Failed to load assembly \"" + mainAssemblyPath + "\" " +
                    Environment.NewLine + Environment.NewLine + e, errorMsgBoxTitle);
                SharedRuntimeState.SetHotReloadData(null);
                return false;
            }
        }

        private static void PreloadNextContext(bool threaded = true)
        {
            if (preloadContextWaitHandle == null)
            {
                preloadContextWaitHandle = new AutoResetEvent(false);
            }
            else
            {
                preloadContextWaitHandle.Reset();
            }

            DoPreloadNextContext(threaded);
        }

        private static void DoPreloadNextContext(bool threaded)
        {
            if (threaded)
            {
                new Thread(delegate ()
                {
                    DoPreloadNextContext(false);
                }).Start();
            }
            else
            {
                preloadFailed = false;
                CreatePreloadedContext(entryPointArg + "|Preloading=true");
                preloadContextWaitHandle.Set();
            }
        }

        private static void CreatePreloadedContext(string entryPointArg)
        {
            Runtime.AssemblyContextRef contextRef = Runtime.AssemblyContextRef.Invalid;

            if (SharedRuntimeState.CurrentRuntime == EDotNetRuntime.CoreCLR)
            {
                contextRef = Runtime.AssemblyContext.Create();
            }
            else
            {
                // Seperate method to avoid issues with .NET Core
                contextRef = CreatePreloadedContextAppDomain(entryPointArg);
            }

            entryPointArg += "|AssemblyContext=" + contextRef.Format();

            if (!contextRef.IsInvalid)
            {
                Debug.Assert(preloadedContextRef.IsInvalid, "Trying to preload when there is already something preloaded");

                try
                {
                    AssemblyLoader loader = new AssemblyLoader(mainAssemblyPath, entryPointType, entryPointMethod, entryPointArg, true, contextRef);
                    contextRef.DoCallBack(loader.Load);
                    preloadedContextRef = contextRef;
                }
                catch (Exception e)
                {
                    GameThreadHelper.Run(delegate ()// For stylized message box (as we may not be in the game thread)
                    {
                        MessageBox("Failed to create assembly context for \"" + mainAssemblyPath + "\" " +
                            Environment.NewLine + Environment.NewLine + e, errorMsgBoxTitle);
                    });
                    preloadFailed = true;
                    UnloadContext(contextRef);
                }
            }
        }

        private static Runtime.AssemblyContextRef CreatePreloadedContextAppDomain(string entryPointArgEx)
        {
            string appDomainName = "Domain" + Environment.TickCount + " " + appDomainCount++;

            AppDomainSetup appDomainSetup = new AppDomainSetup();
            appDomainSetup.ApplicationBase = currentAssemblyDirectory;
            appDomainSetup.ApplicationName = appDomainName;
            if (ShadowCopyAssembly)
            {
                appDomainSetup.ShadowCopyFiles = "true";

                // Main assembly must be in the same or sub directory (limitation of PrivateBinPath)
                string subDirectory;
                IsSameOrSubDirectory(currentAssemblyDirectory, mainAssemblyDirectory, out subDirectory);
                if (!string.IsNullOrEmpty(subDirectory))
                {
                    appDomainSetup.PrivateBinPath = subDirectory;
                }
            }

            //AppDomain preloadAppDomain = AppDomain.CreateDomain(appDomainName, null, mainAssemblyDirectory, ".", false);
            AppDomain preloadAppDomain = AppDomain.CreateDomain(appDomainName, null, appDomainSetup);
            return Runtime.AssemblyContext.Create(preloadAppDomain);
        }

        private static void UnloadMainContext(bool threaded = true)
        {
            if (!mainContextRef.IsInvalid)
            {
                try
                {
                    AssemblyUnloader unloader = new AssemblyUnloader(entryPointType, unloadMethod, mainContextRef);
                    mainContextRef.DoCallBack(unloader.Unload);
                }
                catch (Exception e)
                {
                    MessageBox("HotReload Unload failed for \"" + mainAssemblyPath + "\" " +
                        Environment.NewLine + Environment.NewLine + e, unloadErrorMsgBoxTitle);
                }
                
                Runtime.AssemblyContextRef oldContextRef = mainContextRef;
                mainContextRef = Runtime.AssemblyContextRef.Invalid;

                UnloadContext(oldContextRef, threaded);
            }            
        }

        private static void UnloadContext(Runtime.AssemblyContextRef contextRef, bool threaded = true)
        {
            if (threaded)
            {
                // Run the Unload on a seperate thread to avoid waiting for it.
                new Thread(delegate ()
                {
                    UnloadContext(contextRef, false);
                }).Start();
            }
            else
            {
                Exception exception = null;
                bool unloaded = false;

                if (SharedRuntimeState.CurrentRuntime == EDotNetRuntime.CoreCLR)
                {
                    WeakReference weakRef = contextRef.GetWeakReference();

                    try
                    {
                        // Just fire and hope for the best
                        contextRef.Unload();
                        unloaded = true;
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }

                    if (unloaded && weakRef != null)
                    {
                        for (int i = 0; i < 15 && weakRef.IsAlive; i++)
                        {
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            if (i > 10)
                            {
                                Thread.Sleep(100);
                            }
                        }

                        if (weakRef.IsAlive)
                        {
                            exception = new Exception(".NET Core couldn't unload the AssemblyLoadContext. There is likely some global event" +
                                " which is being bound to. You will crash on the next load due to the state not being cleaned up properly.");
                            unloaded = false;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            contextRef.Unload();
                            unloaded = true;
                            break;
                        }
                        catch (CannotUnloadAppDomainException e)
                        {
                            exception = e;
                        }
                        catch (AppDomainUnloadedException e)
                        {
                            exception = e;
                        }

                        // Give it a little more time and then try again
                        Thread.Sleep(300);
                    }
                }

                if (!unloaded)
                {
                    GameThreadHelper.Run(delegate ()// For stylized message box (as we may not be in the game thread)
                    {
                        MessageBox("Failed to unload assembly context for \"" + mainAssemblyPath + "\" " +
                            Environment.NewLine + Environment.NewLine + exception, unloadErrorMsgBoxTitle);
                    });
                }
            }
        }

        private static bool IsSameOrSubDirectory(string basePath, string path)
        {
            string subDirectory;
            return IsSameOrSubDirectory(basePath, path, out subDirectory);
        }

        private static bool IsSameOrSubDirectory(string basePath, string path, out string subDirectory)
        {
            DirectoryInfo di = new DirectoryInfo(Path.GetFullPath(path).TrimEnd('\\', '/'));
            DirectoryInfo diBase = new DirectoryInfo(Path.GetFullPath(basePath).TrimEnd('\\', '/'));

            subDirectory = null;
            while (di != null)
            {
                if (di.FullName.Equals(diBase.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else
                {
                    if (string.IsNullOrEmpty(subDirectory))
                    {
                        subDirectory = di.Name;
                    }
                    else
                    {
                        subDirectory = Path.Combine(di.Name, subDirectory);
                    }
                    di = di.Parent;
                }
            }
            return false;
        }

        private static unsafe void MessageBox(string text, string title)
        {
            SharedRuntimeState.MessageBox(text, title);
        }

        private static void MessageBox(string text)
        {
            MessageBox(text, "C# Loader Error");
        }
    }

    public enum LoadMode
    {
        /// <summary>
        /// The assembly will be loaded using a regular LoadFile call
        /// </summary>
        Normal,

        /// <summary>
        /// The assembly will be loaded using shadow copying
        /// </summary>
        ShadowCopy,

        /// <summary>
        /// The assembly will be loaded from a buffer
        /// </summary>
        Buffer
    }

    enum AssemblyLoaderError
    {
        MainAssemblyPathNotProvided = 1000,
        MainAssemblyNotFound,
        MainAssemblyNotInSubFolder,
        GameThreadHelpersNull,
        LoadFailed,
        Exception
    }

    [Serializable]
    class AssemblyLoader
    {
        private string entryPointType;
        private string entryPointMethod;
        private string entryPointArg;

        private string assemblyPath;

        private bool isPreloading;
        private KeyValuePair<long, long> assemblyContextRef;

        public AssemblyLoader(string path, string entryPointType, string entryPointMethod, string entryPointArg, 
            bool isPreloading, Runtime.AssemblyContextRef assemblyContextRef)
        {
            this.entryPointType = entryPointType;
            this.entryPointMethod = entryPointMethod;
            this.entryPointArg = entryPointArg;
            this.assemblyPath = path;
            this.isPreloading = isPreloading;
            this.assemblyContextRef = assemblyContextRef;
        }

        public void Load()
        {
            // Don't save the MethodInfo if this is CoreCLR as this will keep the target assembly alive
            MethodInfo dllMainMethod = null;
            if (SharedRuntimeState.CurrentRuntime != EDotNetRuntime.CoreCLR)
            {
                dllMainMethod = AppDomain.CurrentDomain.GetData(EntryPoint.preloadEntryPointDataName) as MethodInfo;
            }

            if (dllMainMethod != null)
            {             
                dllMainMethod.Invoke(null, new object[] { entryPointArg });
            }
            else
            {
                Assembly assembly = ((Runtime.AssemblyContextRef)assemblyContextRef).LoadFrom(assemblyPath);

                Type entryPoint = assembly.GetType(entryPointType);
                if (entryPoint != null)
                {
                    dllMainMethod = entryPoint.GetMethod(entryPointMethod);
                    if (dllMainMethod != null)
                    {
                        dllMainMethod.Invoke(null, new object[] { entryPointArg });

                        if (isPreloading && SharedRuntimeState.CurrentRuntime != EDotNetRuntime.CoreCLR)
                        {
                            // If this is a preload cache the entrypoint for the main load
                            AppDomain.CurrentDomain.SetData(EntryPoint.preloadEntryPointDataName, dllMainMethod);
                        }
                    }
                    else
                    {
                        throw new Exception("Failed to find entry point method " + entryPointType + "." + entryPointMethod);
                    }
                }
                else
                {
                    throw new Exception("Failed to find entry point type " + entryPointType);
                }
            }
        }
    }

    [Serializable]
    class AssemblyUnloader
    {
        private string entryPointType;
        private string unloadMethod;
        private KeyValuePair<long, long> assemblyContextRef;

        public AssemblyUnloader(string entryPointType, string unloadMethod, Runtime.AssemblyContextRef assemblyContextRef)
        {
            this.entryPointType = entryPointType;
            this.unloadMethod = unloadMethod;
            this.assemblyContextRef = assemblyContextRef;
        }

        public void Unload()
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;

            Assembly[] assemblies = ((Runtime.AssemblyContextRef)assemblyContextRef).GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.FullName.StartsWith("UnrealEngine.Runtime"))
                {
                    Type type = assembly.GetType(entryPointType, false);
                    if (type != null)
                    {
                        MethodInfo method = type.GetMethod(unloadMethod, bindingFlags);
                        if (method.GetParameters().Length == 0)
                        {
                            method.Invoke(null, null);
                        }
                    }
                    break;
                }
            }
        }
    }
}

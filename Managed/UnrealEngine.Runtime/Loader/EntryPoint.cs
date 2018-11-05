using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

// NOTE: Do not change the name of this (entry point lookup is "UnrealEngine.EntryPoint.DllMain")
namespace UnrealEngine
{
    /// <summary>
    /// This is a basic loader used to load the main assembly along with automatic reloading on assembly file changes
    /// </summary>
    public class EntryPoint
    {
        // Preload the next app domain in another thread as this can take ~1 second which would
        // slow down hotreloading if we didn't preload it.
        private static AppDomain preloadAppDomain;
        private static AutoResetEvent preloadAppDomainWaitHandle;
        private static bool preloadFailed;
        internal const string preloadEntryPointDataName = "EntryPoint";// Used to cache the entry point method on prereload

        private static AppDomain appDomain;
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

        // If true an AppDomain wont be created and the assembly will be loaded current AppDomain.
        public static bool LoadAssemblyWithoutAppDomain = false;

        // If this is true the loader assemblies will be shadow copied.
        public static bool ShadowCopyAssembly = true;

        private static DateTime lastAssemblyUpdate;
        private static Dictionary<string, FileSystemWatcher> assemblyWatchers = new Dictionary<string, FileSystemWatcher>();

        public static int DllMain(string arg)
        {
            mainAssemblyPath = null;

            Args args = new Args(arg);
            mainAssemblyPath = args.GetString("MainAssembly");
            SharedRuntimeState.Initialize((IntPtr)args.GetInt64("RuntimeState"));
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

            if (SharedRuntimeState.IsMonoRuntime && SharedRuntimeState.IsRuntimeLoaded(EDotNetRuntime.CLR))
            {
                Debug.Assert(appDomain == null);

                // Make sure the main assembly path exists
                if (!File.Exists(mainAssemblyPath))
                {
                    return (int)AssemblyLoaderError.LoadFailed;
                }

                // Make sure we are using AppDomain loadding otherwise hotreload wont work which defeats the purpose of
                // using multiple runtimes
                if (LoadAssemblyWithoutAppDomain)
                {
                    return (int)AssemblyLoaderError.LoadFailed;
                }

                // Mono will be preloaded only. It will wait until the next runtime is set to target Mono at which point
                // it will do a full load.
                PreloadNextAppDomain();

                // Watch for assembly changes (the paths should have been set up by the full load in the CLR runtime)
                UpdateAssemblyWatchers();

                return 0;
            }

            unsafe
            {
                SharedRuntimeState.Instance->ActiveRuntime = SharedRuntimeState.CurrentRuntime;
            }

            if (!ReloadAppDomain())
            {
                unsafe
                {
                    SharedRuntimeState.Instance->ActiveRuntime = EDotNetRuntime.None;
                }
                return (int)AssemblyLoaderError.LoadFailed;
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
                    return;
                }

                if (appDomain != null)
                {
                    UnloadAppDomain();
                }
                Debug.Assert(appDomain == null, "UnloadAppDomain failed?");
                Debug.Assert(!LoadAssemblyWithoutAppDomain, "AppDomain loading is required in order to swap runtimes");

                PreloadNextAppDomain();
                SharedRuntimeState.Instance->IsActiveRuntimeComplete = 1;
            }
            else
            {
                SharedRuntimeState.Instance->ActiveRuntime = SharedRuntimeState.CurrentRuntime;
                SharedRuntimeState.Instance->NextRuntime = EDotNetRuntime.None;
                SharedRuntimeState.Instance->IsActiveRuntimeComplete = 0;
                ReloadAppDomain();
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
                    ReloadAppDomain();
                    lastAssemblyUpdate = DateTime.Now;
                }
            }
        }

        private static bool ReloadAppDomain()
        {
            if (!GameThreadHelper.IsInGameThread())
            {
                bool result = false;
                GameThreadHelper.Run(delegate { result = ReloadAppDomain(); });
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

            if (appDomain != null)
            {
                UnloadAppDomain();
            }

            if (LoadAssemblyWithoutAppDomain)
            {
                try
                {
                    AssemblyLoader loader = new AssemblyLoader(mainAssemblyPath, entryPointType, entryPointMethod, entryPointArg, false);
                    loader.Load();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failed to load assembly \"" + mainAssemblyPath + "\" " +
                        Environment.NewLine + Environment.NewLine + e, errorMsgBoxTitle);
                    SharedRuntimeState.SetHotReloadData(null);
                    return false;
                }
            }
            else
            {
                string entryPointArgEx = entryPointArg;
                bool firstLoad = preloadAppDomainWaitHandle == null;
                if (firstLoad)
                {
                    PreloadNextAppDomain();
                }
                else
                {
                    entryPointArgEx += "|Reloading=true";
                }

                preloadAppDomainWaitHandle.WaitOne(Timeout.Infinite);
                preloadAppDomainWaitHandle.Reset();

                if (!preloadFailed)
                {
                    appDomain = preloadAppDomain;
                    preloadAppDomain = null;

                    try
                    {
                        AssemblyLoader loader = new AssemblyLoader(mainAssemblyPath, entryPointType, entryPointMethod, entryPointArgEx, false);
                        appDomain.DoCallBack(loader.Load);
                        UpdateAssemblyWatchers();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Failed to create AppDomain for \"" + mainAssemblyPath + "\" " +
                            Environment.NewLine + Environment.NewLine + e, errorMsgBoxTitle);
                    }
                }
            }

            if (!LoadAssemblyWithoutAppDomain)
            {
                PreloadNextAppDomain();
            }
            SharedRuntimeState.SetHotReloadData(null);
            return true;
        }

        private static void PreloadNextAppDomain()
        {
            if (preloadAppDomainWaitHandle == null)
            {
                preloadAppDomainWaitHandle = new AutoResetEvent(false);
            }
            else
            {
                preloadAppDomainWaitHandle.Reset();
            }

            new Thread(delegate ()
            {
                preloadFailed = false;

                string entryPointArgEx = entryPointArg + "|Preloading=true";

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

                //preloadAppDomain = AppDomain.CreateDomain(appDomainName, null, mainAssemblyDirectory, ".", false);
                preloadAppDomain = AppDomain.CreateDomain(appDomainName, null, appDomainSetup);
                try
                {
                    AssemblyLoader loader = new AssemblyLoader(mainAssemblyPath, entryPointType, entryPointMethod, entryPointArgEx, true);
                    preloadAppDomain.DoCallBack(loader.Load);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failed to create AppDomain for \"" + mainAssemblyPath + "\" " +
                        Environment.NewLine + Environment.NewLine + e, errorMsgBoxTitle);
                    preloadFailed = true;
                    UnloadAppDomain(preloadAppDomain);
                }

                preloadAppDomainWaitHandle.Set();
            }).Start();
        }

        private static void UnloadAppDomain()
        {
            if (appDomain != null)
            {
                try
                {
                    AssemblyUnloader unloader = new AssemblyUnloader(entryPointType, unloadMethod);                    
                    appDomain.DoCallBack(unloader.Unload);
                }
                catch (Exception e)
                {
                    MessageBox.Show("HotReload Unload failed for \"" + mainAssemblyPath + "\" " +
                        Environment.NewLine + Environment.NewLine + e, unloadErrorMsgBoxTitle);
                }
                
                AppDomain oldAppDomain = appDomain;
                appDomain = null;

                UnloadAppDomain(oldAppDomain);
            }            
        }

        private static void UnloadAppDomain(AppDomain domain)
        {
            // Run the AppDomain.Unload on a seperate thread to avoid waiting for it.
            new Thread(delegate ()
            {
                Exception exception = null;

                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        AppDomain.Unload(domain);
                        domain = null;
                        break;
                    }
                    catch (Exception e)
                    {
                        exception = e;

                        // Give it a little more time and then try again
                        Thread.Sleep(300);
                    }
                }

                if (domain != null)
                {
                    domain = null;
                    MessageBox.Show("Failed to unload AppDomain for \"" + mainAssemblyPath + "\" " +
                        Environment.NewLine + Environment.NewLine + exception, unloadErrorMsgBoxTitle);
                }
            }).Start();
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
        LoadFailed
    }

    [Serializable]
    class AssemblyLoader
    {
        private string entryPointType;
        private string entryPointMethod;
        private string entryPointArg;

        private string assemblyPath;

        private bool isPreloading;

        public AssemblyLoader(string path, string entryPointType, string entryPointMethod, string entryPointArg, bool isPreloading)
        {
            this.entryPointType = entryPointType;
            this.entryPointMethod = entryPointMethod;
            this.entryPointArg = entryPointArg;
            this.assemblyPath = path;
            this.isPreloading = isPreloading;
        }

        public void Load()
        {
            MethodInfo dllMainMethod = AppDomain.CurrentDomain.GetData(EntryPoint.preloadEntryPointDataName) as MethodInfo;
            if (dllMainMethod != null)
            {
                Type entryPoint = dllMainMethod.DeclaringType;
                
                dllMainMethod.Invoke(null, new object[] { entryPointArg });
            }
            else
            {
                Assembly assembly = null;

                //assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyPath));
                assembly = Assembly.LoadFrom(assemblyPath);

                Type entryPoint = assembly.GetType(entryPointType);
                if (entryPoint != null)
                {
                    dllMainMethod = entryPoint.GetMethod(entryPointMethod);
                    if (dllMainMethod != null)
                    {
                        dllMainMethod.Invoke(null, new object[] { entryPointArg });

                        if (isPreloading)
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

        public AssemblyUnloader(string entryPointType, string unloadMethod)
        {
            this.entryPointType = entryPointType;
            this.unloadMethod = unloadMethod;
        }

        public void Unload()
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName == typeof(AssemblyUnloader).Assembly.FullName)
                {
                    // Skip the loader assembly
                    continue;
                }

                Type type = assembly.GetType(entryPointType, false);
                if (type != null)
                {
                    MethodInfo method = type.GetMethod(unloadMethod, bindingFlags);
                    if (method.GetParameters().Length == 0)
                    {
                        method.Invoke(null, null);
                        break;
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
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
        private static string assemblyPath;
        private static string mainAssemblyDirectory;
        private static string currentAssemblyDirectory;
       
        private static string errorMsgBoxTitle = "C# Loader Error";
        private static string unloadErrorMsgBoxTitle = "C# Unload Error";
        
        private static string entryPointType = "UnrealEngine.EntryPoint";
        private static string entryPointMethod = "DllMain";
        private static string entryPointArg = null;
        private static string unloadMethod = "Unload";

        internal const string hotReloadAssemblyPathsName = "HotReloadAssemblyPaths";
        internal const string hotReloadDataName = "HotReloadData";        
        private static byte[] hotreloadData;

        // If true an AppDomain wont be created and the assembly will be loaded current AppDomain.
        public static bool LoadAssemblyWithoutAppDomain = false;

        // If this is true the loader assemblies will be shadow copied.
        public static bool ShadowCopyAssembly = true;

        private static DateTime lastAssemblyUpdate;
        private static Dictionary<string, FileSystemWatcher> assemblyWatchers = new Dictionary<string, FileSystemWatcher>();

        public static int DllMain(string arg)
        {
            assemblyPath = null;

            Args args = new Args(arg);
            assemblyPath = args.GetString("MainAssembly");
            if (!string.IsNullOrEmpty(assemblyPath))
            {
                if (string.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath))
                {
                    return (int)AssemblyLoaderError.MainAssemblyNotFound;
                }
                mainAssemblyDirectory = Path.GetDirectoryName(assemblyPath);
            }
            else
            {
                return (int)AssemblyLoaderError.MainAssemblyPathNotProvided;
            }

            IntPtr asyncTaskAddr = (IntPtr)args.GetInt64("AsyncTask");
            IntPtr isInGameThreadAddr = (IntPtr)args.GetInt64("IsInGameThread");
            if (asyncTaskAddr == IntPtr.Zero || isInGameThreadAddr == IntPtr.Zero)
            {
                return (int)AssemblyLoaderError.GameThreadAsyncNull;
            }
            GameThreadHelper.Init(asyncTaskAddr, isInGameThreadAddr);

            entryPointArg = arg;

            string currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
            string currentAssemblyFileName = Path.GetFileNameWithoutExtension(currentAssemblyPath);
            currentAssemblyDirectory = Path.GetDirectoryName(currentAssemblyPath);

            if (!IsSameOrSubDirectory(currentAssemblyDirectory, mainAssemblyDirectory))
            {
                return (int)AssemblyLoaderError.MainAssemblyPathNotProvided;
            }

            if (!ReloadAppDomain())
            {
                return (int)AssemblyLoaderError.LoadFailed;
            }

            return 0;
        }

        private static void UpdateAssemblyWatchers(string[] assemblyPaths)
        {
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

            if (!File.Exists(assemblyPath))
            {
                hotreloadData = null;
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
                    AssemblyLoader loader = new AssemblyLoader(assemblyPath, entryPointType, entryPointMethod, entryPointArg, hotreloadData, false);
                    loader.Load();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failed to load assembly \"" + assemblyPath + "\" " +
                        Environment.NewLine + Environment.NewLine + e, errorMsgBoxTitle);
                    hotreloadData = null;
                    return false;
                }
            }
            else
            {
                string entryPointArgEx = entryPointArg;
                bool firstLoad = preloadAppDomainWaitHandle == null;
                if (firstLoad)
                {
                    PreloadNextAppDomain(true);
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
                        AssemblyLoader loader = new AssemblyLoader(assemblyPath, entryPointType, entryPointMethod, entryPointArgEx, hotreloadData, false);
                        appDomain.DoCallBack(loader.Load);
                        UpdateAssemblyWatchers(appDomain.GetData(hotReloadAssemblyPathsName) as string[]);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Failed to create AppDomain for \"" + assemblyPath + "\" " +
                            Environment.NewLine + Environment.NewLine + e, errorMsgBoxTitle);
                    }
                }
            }

            if (!LoadAssemblyWithoutAppDomain)
            {
                PreloadNextAppDomain(false);
            }
            hotreloadData = null;
            return true;
        }

        private static void PreloadNextAppDomain(bool firstLoad)
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
                if (!firstLoad)
                {
                    entryPointArgEx += "|Reloading=true";
                }

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
                    AssemblyLoader loader = new AssemblyLoader(assemblyPath, entryPointType, entryPointMethod, entryPointArgEx, hotreloadData, true);
                    preloadAppDomain.DoCallBack(loader.Load);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failed to create AppDomain for \"" + assemblyPath + "\" " +
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
                    hotreloadData = appDomain.GetData(hotReloadDataName) as byte[];
                    appDomain.SetData(hotReloadDataName, null);
                }
                catch (Exception e)
                {
                    MessageBox.Show("HotReload Unload failed for \"" + assemblyPath + "\" " +
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
                    MessageBox.Show("Failed to unload AppDomain for \"" + assemblyPath + "\" " +
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
        GameThreadAsyncNull,
        LoadFailed
    }

    [Serializable]
    class AssemblyLoader
    {
        private string entryPointType;
        private string entryPointMethod;
        private string entryPointArg;

        private string assemblyPath;
        private byte[] hotReloadData;

        private bool isPreloading;

        public AssemblyLoader(string path, string entryPointType, string entryPointMethod, string entryPointArg, byte[] hotReloadData, bool isPreloading)
        {
            this.entryPointType = entryPointType;
            this.entryPointMethod = entryPointMethod;
            this.entryPointArg = entryPointArg;
            this.assemblyPath = path;
            this.hotReloadData = hotReloadData;
            this.isPreloading = isPreloading;
        }

        public void Load()
        {
            MethodInfo dllMainMethod = AppDomain.CurrentDomain.GetData(EntryPoint.preloadEntryPointDataName) as MethodInfo;
            if (dllMainMethod != null)
            {
                Type entryPoint = dllMainMethod.DeclaringType;
                entryPoint.GetField(EntryPoint.hotReloadDataName).SetValue(null, hotReloadData);
                
                dllMainMethod.Invoke(null, new object[] { entryPointArg });

                AppDomain.CurrentDomain.SetData(EntryPoint.hotReloadAssemblyPathsName,
                    entryPoint.GetField(EntryPoint.hotReloadAssemblyPathsName).GetValue(null) as string[]);
            }
            else
            {
                Assembly assembly = null;

                //assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyPath));
                assembly = Assembly.LoadFrom(assemblyPath);

                Type entryPoint = assembly.GetType(entryPointType);
                if (entryPoint != null)
                {
                    if (!isPreloading)
                    {
                        entryPoint.GetField(EntryPoint.hotReloadDataName).SetValue(null, hotReloadData);
                    }

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

                    if (!isPreloading)
                    {
                        AppDomain.CurrentDomain.SetData(EntryPoint.hotReloadAssemblyPathsName,
                            entryPoint.GetField(EntryPoint.hotReloadAssemblyPathsName).GetValue(null) as string[]);
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
                        AppDomain.CurrentDomain.SetData(EntryPoint.hotReloadDataName, method.Invoke(null, null));
                        break;
                    }
                }
            }
        }
    }

    class Args
    {
        private Dictionary<string, string> args = new Dictionary<string, string>();

        public Args(string arg)
        {
            if (arg != null)
            {
                string[] splitted = arg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in splitted)
                {
                    int equalsIndex = str.IndexOf('=');
                    if (equalsIndex > 0)
                    {
                        string key = str.Substring(0, equalsIndex).Trim();
                        string value = str.Substring(equalsIndex + 1).Trim();
                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                        {
                            args[key] = value;
                        }
                    }
                }
            }
        }

        public string this[string key]
        {
            get { return GetString(key); }
        }

        public bool Contains(string key)
        {
            return args.ContainsKey(key);
        }

        public string GetString(string key)
        {
            string value;
            args.TryGetValue(key, out value);
            return value;
        }

        public bool GetBool(string key)
        {
            string valueStr;
            bool value;
            if (args.TryGetValue(key, out valueStr) && bool.TryParse(valueStr, out value))
            {
                return value;
            }
            return false;
        }

        public int GetInt32(string key)
        {
            string valueStr;
            int value;
            if (args.TryGetValue(key, out valueStr) && int.TryParse(valueStr, out value))
            {
                return value;
            }
            return 0;
        }

        public long GetInt64(string key)
        {
            string valueStr;
            long value;
            if (args.TryGetValue(key, out valueStr) && long.TryParse(valueStr, out value))
            {
                return value;
            }
            return 0;
        }
    }

    static class GameThreadHelper
    {
        public delegate void FSimpleDelegate();
        private delegate void Del_AsyncTask(FSimpleDelegate func, int threadType);
        private static Del_AsyncTask asyncTask;

        private delegate csbool Del_IsInGameThread();
        private static Del_IsInGameThread isInGameThread;

        public static void Init(IntPtr asyncTaskAddr, IntPtr isInGameThreadAddr)
        {
            asyncTask = (Del_AsyncTask)Marshal.GetDelegateForFunctionPointer(asyncTaskAddr, typeof(Del_AsyncTask));
            isInGameThread = (Del_IsInGameThread)Marshal.GetDelegateForFunctionPointer(isInGameThreadAddr, typeof(Del_IsInGameThread));
        }

        public static bool IsInGameThread()
        {
            return isInGameThread();
        }

        public static void Run(FSimpleDelegate callback)
        {
            if (IsInGameThread())
            {
                callback();
            }
            else
            {
                using (AutoResetEvent waitHandle = new AutoResetEvent(false))
                {
                    asyncTask(delegate
                    {
                        callback();
                        waitHandle.Set();
                    }, 0);
                    waitHandle.WaitOne(Timeout.Infinite);
                }
            }
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
}
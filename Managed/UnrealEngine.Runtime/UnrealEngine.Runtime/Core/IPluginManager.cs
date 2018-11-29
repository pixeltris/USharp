using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Projects\Public\Interfaces\IPluginManager.h - IPluginManager / IPlugin / FPluginStatus / EPluginLoadedFrom / EPluginType
    // Engine\Source\Runtime\Projects\Private\PluginManager.h - FPluginManager / FPlugin
    // Engine\Source\Runtime\Projects\Public\ModuleDescriptor.h - FModuleDescriptor / FModuleContextInfo / ELoadingPhase / EHostType

    /// <summary>
    /// PluginManager manages available code and content extensions (both loaded and not loaded).
    /// </summary>
    public class IPluginManager
    {
        public IntPtr Address { get; private set; }

        private static IPluginManager instance;
        public static IPluginManager Instance
        {
            get { return Get(); }
        }

        /// <summary>
        /// Access singleton instance.
        /// </summary>
        /// <returns>Reference to the singleton object.</returns>
        public static IPluginManager Get()
        {
            if (instance == null)
            {
                instance = new IPluginManager();
                instance.Address = Native_IPluginManager.Get();
            }
            return instance;
        }

        /// <summary>
        /// Updates the list of plugins.
        /// </summary>
        public void RefreshPluginsList()
        {
            Native_IPluginManager.RefreshPluginsList(Address);
        }

        /// <summary>
        /// Loads all plug-ins
        /// </summary>
        /// <param name="loadingPhase">Which loading phase we're loading plug-in modules from. Only modules that are configured to be
        /// loaded at the specified loading phase will be loaded during this call.</param>
        public bool LoadModulesForEnabledPlugins(ELoadingPhase loadingPhase)
        {
            return Native_IPluginManager.LoadModulesForEnabledPlugins(Address, (int)loadingPhase);
        }

        /// <summary>
        /// Get the localization paths for all enabled plugins.
        /// </summary>
        /// <returns>The localization paths for all enabled plugins.</returns>
        public string[] GetLocalizationPathsForEnabledPlugins()
        {
            using (TArrayUnsafe<string> resultUnsafe = new TArrayUnsafe<string>())
            {
                Native_IPluginManager.GetLocalizationPathsForEnabledPlugins(Address, resultUnsafe.Address);
                return resultUnsafe.ToArray();
            }
        }

        /// <summary>
        /// Checks if all the required plug-ins are available. If not, will present an error dialog the first time a plug-in is loaded or this function is called.
        /// </summary>
        /// <returns>true if all the required plug-ins are available.</returns>
        public bool AreRequiredPluginsAvailable()
        {
            return Native_IPluginManager.AreRequiredPluginsAvailable(Address);
        }

        /// <summary>
        /// Checks whether modules for the enabled plug-ins are up to date.
        /// </summary>
        /// <param name="incompatibleModules">Incompatible module names.</param>
        /// <returns>true if the enabled plug-in modules are up to date.</returns>
        public bool CheckModuleCompatibility(out string[] incompatibleModules)
        {
            // !IS_MONOLITHIC
            if (Native_IPluginManager.CheckModuleCompatibility == null)
            {
                incompatibleModules = null;
                return true;
            }

            using (TArrayUnsafe<string> incompatibleModulesUnsafe = new TArrayUnsafe<string>())
            {
                bool result = Native_IPluginManager.CheckModuleCompatibility(Address, incompatibleModulesUnsafe.Address);
                incompatibleModules = incompatibleModulesUnsafe.ToArray();
                return result;
            }
        }

        /// <summary>
        /// Finds information for an enabled plugin.
        /// </summary>
        /// <param name="name">The plugin's information, or nullptr.</param>
        /// <returns></returns>
        public IPlugin FindPlugin(string name)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            {
                FSharedPtr sharedPtr;
                Native_IPluginManager.FindPlugin(Address, ref nameUnsafe.Array, out sharedPtr);
                if (sharedPtr.IsValid())
                {
                    return new IPlugin(sharedPtr);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets an array of all the enabled plugins.
        /// </summary>
        /// <returns>Array of the enabled plugins.</returns>
        public IPlugin[] GetEnabledPlugins()
        {
            using (TArrayUnsafe<FSharedPtr> resultUnsafe = new TArrayUnsafe<FSharedPtr>())
            {
                Native_IPluginManager.GetEnabledPlugins(Address, resultUnsafe.Address);
                return GetPluginArray(resultUnsafe);
            }
        }

        /// <summary>
        /// Gets an array of all enabled plugins that can have content.
        /// </summary>
        /// <returns>Array of plugins with IsEnabled() and CanContainContent() both true.</returns>
        public IPlugin[] GetEnabledPluginsWithContent()
        {
            using (TArrayUnsafe<FSharedPtr> resultUnsafe = new TArrayUnsafe<FSharedPtr>())
            {
                Native_IPluginManager.GetEnabledPluginsWithContent(Address, resultUnsafe.Address);
                return GetPluginArray(resultUnsafe);
            }
        }

        /// <summary>
        /// Gets an array of all the discovered plugins.
        /// </summary>
        /// <returns>Array of the discovered plugins.</returns>
        public IPlugin[] GetDiscoveredPlugins()
        {
            using (TArrayUnsafe<FSharedPtr> resultUnsafe = new TArrayUnsafe<FSharedPtr>())
            {
                Native_IPluginManager.GetDiscoveredPlugins(Address, resultUnsafe.Address);
                return GetPluginArray(resultUnsafe);
            }
        }

        /// <summary>
        /// Stores the specified path, utilizing it in future search passes when 
        /// searching for available plugins. Optionally refreshes the manager after 
        /// the new path has been added.
        /// </summary>
        /// <param name="extraDiscoveryPath">The path you want searched for additional plugins.</param>
        /// <param name="refresh">Signals the function to refresh the plugin database after the new path has been added</param>
        public void AddPluginSearchPath(string extraDiscoveryPath, bool refresh = true)
        {
            using (FStringUnsafe extraDiscoveryPathUnsafe = new FStringUnsafe(extraDiscoveryPath))
            {
                Native_IPluginManager.AddPluginSearchPath(Address, ref extraDiscoveryPathUnsafe.Array, refresh);
            }
        }

        /// <summary>
        /// Gets an array of plugins that loaded their own content pak file
        /// </summary>
        public IPlugin[] GetPluginsWithPakFile()
        {
            using (TArrayUnsafe<FSharedPtr> resultUnsafe = new TArrayUnsafe<FSharedPtr>())
            {
                Native_IPluginManager.GetPluginsWithPakFile(Address, resultUnsafe.Address);
                return GetPluginArray(resultUnsafe);
            }
        }

        // TODO: FNewPluginMountedEvent / OnNewPluginCreated / OnNewPluginMounted

        public void MountNewlyCreatedPlugin(string pluginName)
        {
            using (FStringUnsafe pluginNameUnsafe = new FStringUnsafe(pluginName))
            {
                Native_IPluginManager.MountNewlyCreatedPlugin(Address, ref pluginNameUnsafe.Array);
            }
        }

        private IPlugin[] GetPluginArray(TArrayUnsafe<FSharedPtr> sharedPtrs)
        {
            int count = sharedPtrs.Count;
            IPlugin[] result = new IPlugin[count];
            for (int i = 0; i < count; i++)
            {
                FSharedPtr sharedPtr = sharedPtrs[i];
                if (sharedPtr.IsValid())
                {
                    result[i] = new IPlugin(sharedPtr);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Information about an enabled plugin.
    /// </summary>
    public class IPlugin : IDisposable
    {
        private bool disposed;
        private FSharedPtr sharedPtr;

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public string Name
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_IPlugin.GetName(sharedPtr.Object, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Path to the plugin's descriptor.
        /// </summary>
        public string DescriptorFileName
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_IPlugin.GetDescriptorFileName(sharedPtr.Object, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Path to the plugin's base directory.
        /// </summary>
        public string BaseDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_IPlugin.GetBaseDir(sharedPtr.Object, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Path to the plugin's content directory.
        /// </summary>
        public string ContentDir
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_IPlugin.GetContentDir(sharedPtr.Object, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// The mounted root path for assets in this plugin's content folder; typically /PluginName/.
        /// </summary>
        public string MountedAssetPath
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_IPlugin.GetMountedAssetPath(sharedPtr.Object, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// The plugin type
        /// </summary>
        public EPluginType PluginType
        {
            get
            {
                return (EPluginType)Native_IPlugin.GetPluginType(sharedPtr.Object);
            }
        }

        /// <summary>
        /// True if the plugin is currently enabled.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return Native_IPlugin.IsEnabled(sharedPtr.Object);
            }
        }

        /// <summary>
        /// True if the plugin is currently enabled by default.
        /// </summary>
        public bool IsEnabledByDefault
        {
            get
            {
                return Native_IPlugin.IsEnabledByDefault(sharedPtr.Object);
            }
        }

        /// <summary>
        /// True if the plugin should be hidden.
        /// </summary>
        public bool IsHidden
        {
            get
            {
                return Native_IPlugin.IsHidden(sharedPtr.Object);
            }
        }

        /// <summary>
        /// True if the plugin can contain content.
        /// </summary>
        public bool CanContainContent
        {
            get
            {
                return Native_IPlugin.CanContainContent(sharedPtr.Object);
            }
        }

        /// <summary>
        /// Where the plugin was loaded from
        /// </summary>
        public EPluginLoadedFrom LoadedFrom
        {
            get
            {
                return (EPluginLoadedFrom)Native_IPlugin.GetLoadedFrom(sharedPtr.Object);
            }
        }

        internal IPlugin(FSharedPtr sharedPtr)
        {
            this.sharedPtr = sharedPtr;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                Debug.Assert(sharedPtr.IsValid() && sharedPtr.ReferenceController != IntPtr.Zero);
                sharedPtr.ReleaseSharedReference(ESPMode.ThreadSafe);
                disposed = true;
            }
        }

        // Maybe create a IPluginCollection struct/class so we can use a using statement instead of explicit dispose?
        public static void Dispose(IPlugin[] plugins)
        {
            if (plugins != null)
            {
                for (int i = 0; i < plugins.Length; i++)
                {
                    plugins[i].Dispose();
                }
            }
        }
    }

    /// <summary>
    /// Enum for where a plugin is loaded from
    /// </summary>
    public enum EPluginLoadedFrom
    {
        /// <summary>
        /// Plugin is built-in to the engine
        /// </summary>
        Engine,

        /// <summary>
        /// Project-specific plugin, stored within a game project directory
        /// </summary>
        Project
    }

    /// <summary>
    /// Enum for the type of a plugin
    /// </summary>
    public enum EPluginType
    {
        /// <summary>
        /// Plugin is built-in to the engine
        /// </summary>
        Engine,

        /// <summary>
        /// Standard enterprise plugin
        /// </summary>
        Enterprise,

        /// <summary>
        /// Project-specific plugin, stored within a game project directory
        /// </summary>
        Project,

        /// <summary>
        /// Plugin found in an external directory (found in an AdditionalPluginDirectory listed in the project file, or referenced on the command line)
        /// </summary>
        External,

        /// <summary>
        /// Project-specific mod plugin
        /// </summary>
        Mod
    }

    /// <summary>
    /// Phase at which this module should be loaded during startup.
    /// </summary>
    public enum ELoadingPhase
    {
        /// <summary>
        /// Loaded before the engine is fully initialized, immediately after the config system has been initialized.  Necessary only for very low-level hooks
        /// </summary>
        PostConfigInit,

        /// <summary>
        /// Loaded before coreUObject for setting up manual loading screens, used for our chunk patching system
        /// </summary>
        PreEarlyLoadingScreen,

        /// <summary>
        /// Loaded before the engine is fully initialized for modules that need to hook into the loading screen before it triggers
        /// </summary>
        PreLoadingScreen,

        /// <summary>
        /// Right before the default phase
        /// </summary>
        PreDefault,

        /// <summary>
        /// Loaded at the default loading point during startup (during engine init, after game modules are loaded.)
        /// </summary>
        Default,

        /// <summary>
        /// Right after the default phase
        /// </summary>
        PostDefault,

        /// <summary>
        /// After the engine has been initialized
        /// </summary>
        PostEngineInit,

        /// <summary>
        /// Do not automatically load this module
        /// </summary>
        None
    }
}

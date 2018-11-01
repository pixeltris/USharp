using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Implements the module manager.
    /// 
    /// The module manager is used to load and unload modules, as well as to keep track of all of the
    /// modules that are currently loaded. You can access this singleton using FModuleManager::Get().
    /// </summary>
    public class FModuleManager
    {
        public IntPtr Address { get; internal set; }

        private static FModuleManager instance;
        public static FModuleManager Get()
        {
            if (instance == null)
            {
                instance = new FModuleManager();
                instance.Address = Native_FModuleManager.Get();
            }
            return instance;
        }

        public static FModuleManager Instance
        {
            get { return Get(); }
        }

        /// <summary>
        /// Abandons a loaded module, leaving it loaded in memory but no longer tracking it in the module manager.
        /// </summary>
        /// <param name="moduleName">The name of the module to abandon.  Should not include path, extension or platform/configuration info.  This is just the "module name" part of the module file name.</param>
        public void AbandonModule(FName moduleName)
        {
            Native_FModuleManager.AbandonModule(Address, ref moduleName);
        }

        /// <summary>
        /// Adds a module to our list of modules, unless it's already known.
        /// </summary>
        /// <param name="moduleName">The base name of the module file.  Should not include path, extension or platform/configuration info.  This is just the "name" part of the module file name.  Names should be globally unique.</param>
        public void AddModule(FName moduleName)
        {
            Native_FModuleManager.AddModule(Address, ref moduleName);
        }

        /// <summary>
        /// Checks whether the specified module is currently loaded.
        /// 
        /// This is an O(1) operation.
        /// </summary>
        /// <param name="moduleName">The base name of the module file.  Should not include path, extension or platform/configuration info.  This is just the "module name" part of the module file name.  Names should be globally unique.</param>
        /// <returns>true if module is currently loaded, false otherwise.</returns>
        public bool IsModuleLoaded(FName moduleName)
        {
            return Native_FModuleManager.IsModuleLoaded(Address, ref moduleName);
        }

        /// <summary>
        /// Loads the specified module.
        /// </summary>
        /// <param name="moduleName">The base name of the module file.  Should not include path, extension or platform/configuration info.  This is just the "module name" part of the module file name.  Names should be globally unique.</param>
        /// <returns>The loaded module, or nullptr if the load operation failed.</returns>
        public IntPtr LoadModule(FName moduleName)
        {
            return Native_FModuleManager.LoadModule(Address, ref moduleName);
        }

        /// <summary>
        /// Loads the specified module, checking to ensure it exists.
        /// </summary>
        /// <param name="moduleName">The base name of the module file.  Should not include path, extension or platform/configuration info.  This is just the "module name" part of the module file name.  Names should be globally unique.</param>
        /// <returns>The loaded module, or nullptr if the load operation failed.</returns>
        public IntPtr LoadModuleChecked(FName moduleName)
        {
            return Native_FModuleManager.LoadModuleChecked(Address, ref moduleName);
        }

        /// <summary>
        /// Loads a module in memory then calls PostLoad.
        /// </summary>
        /// <param name="moduleName">The name of the module to load.</param>
        /// <returns>true on success, false otherwise.</returns>
        public bool LoadModuleWithCallback(FName moduleName)
        {
            return Native_FModuleManager.LoadModuleWithCallback(Address, ref moduleName, IntPtr.Zero);
        }

        /// <summary>
        /// Loads the specified module and returns a result.
        /// </summary>
        /// <param name="moduleName">The base name of the module file.  Should not include path, extension or platform/configuration info.  This is just the "module name" part of the module file name.  Names should be globally unique.</param>
        /// <param name="failureReason">Will contain the result.</param>
        /// <returns>The loaded module (null if the load operation failed).</returns>
        public IntPtr LoadModuleWithFailureReason(FName moduleName, out EModuleLoadResult failureReason)
        {
            return Native_FModuleManager.LoadModuleWithFailureReason(Address, ref moduleName, out failureReason);
        }

        /// <summary>
        /// Queries information about a specific module name.
        /// </summary>
        /// <param name="moduleName">Module to query status for.</param>
        /// <param name="outModuleStatus">Status of the specified module.</param>
        /// <returns>true if the module was found and the OutModuleStatus is valid, false otherwise.</returns>
        public bool QueryModule(FName moduleName, out FModuleStatus outModuleStatus)
        {
            FModuleStatusNative outModuleStatusUnsafe = new FModuleStatusNative();
            bool result = Native_FModuleManager.QueryModule(Address, ref moduleName, ref outModuleStatusUnsafe);
            outModuleStatus = new FModuleStatus(outModuleStatusUnsafe);
            outModuleStatusUnsafe.Dispose();
            return result;
        }

        /// <summary>
        /// Queries information about all of the currently known modules.
        /// </summary>
        /// <returns>Status of all modules.</returns>
        public FModuleStatus[] QueryModules()
        {
            List<FModuleStatus> modules = new List<FModuleStatus>();
            using (TArrayUnsafe<FModuleStatusNative> modulesUnsafe = new TArrayUnsafe<FModuleStatusNative>())
            {
                Native_FModuleManager.QueryModules(Address, modulesUnsafe.Address);
                foreach (FModuleStatusNative moduleStatusUnsafe in modulesUnsafe)
                {
                    modules.Add(new FModuleStatus(moduleStatusUnsafe));
                }
            }
            return modules.ToArray();
        }

        /// <summary>
        /// Finds module files on the disk for loadable modules matching the specified wildcard.
        /// </summary>
        /// <param name="wildcardWithoutExtension">Filename part (no path, no extension, no build config info) to search for.</param>
        /// <returns>List of modules found.</returns>
        public FName[] FindModules(string wildcardWithoutExtension)
        {
            using (FStringUnsafe wildcardWithoutExtensionUnsafe = new FStringUnsafe(wildcardWithoutExtension))
            using (TArrayUnsafe<FName> modulesUnsafe = new TArrayUnsafe<FName>())
            {
                Native_FModuleManager.FindModules(Address, ref wildcardWithoutExtensionUnsafe.Array, modulesUnsafe.Address);
                return modulesUnsafe.ToArray();
            }
        }

        /// <summary>
        /// Determines if a module with the given name exists, regardless of whether it is currently loaded.
        /// </summary>
        /// <param name="moduleName">Name of the module to look for.</param>
        /// <returns>Whether the module exists.</returns>
        public bool ModuleExists(string moduleName)
        {
            using (FStringUnsafe moduleNameUnsafe = new FStringUnsafe(moduleName))
            {
                return Native_FModuleManager.ModuleExists(Address, ref moduleNameUnsafe.Array);
            }
        }

        /// <summary>
        /// Gets the number of loaded modules.
        /// </summary>
        /// <returns>The number of modules.</returns>
        public int GetModuleCount()
        {
            return Native_FModuleManager.GetModuleCount(Address);
        }

        /// <summary>
        /// Called by the engine at startup to let the Module Manager know that it's now
        /// safe to process new UObjects discovered by loading C++ modules.
        /// </summary>
        public void StartProcessingNewlyLoadedObjects()
        {
            Native_FModuleManager.StartProcessingNewlyLoadedObjects(Address);
        }

        /// <summary>
        /// Adds an engine binaries directory.
        /// </summary>
        public void AddBinariesDirectory(string inDirectory, bool isGameDirectory)
        {
            using (FStringUnsafe inDirectoryUnsafe = new FStringUnsafe(inDirectory))
            {
                Native_FModuleManager.AddBinariesDirectory(Address, ref inDirectoryUnsafe.Array, isGameDirectory);
            }
        }

        /// <summary>
        /// Set the game binaries directory
        /// </summary>
        /// <param name="inDirectory">The game binaries directory.</param>
        public void SetGameBinariesDirectory(string inDirectory)
        {
            using (FStringUnsafe inDirectoryUnsafe = new FStringUnsafe(inDirectory))
            {
                Native_FModuleManager.SetGameBinariesDirectory(Address, ref inDirectoryUnsafe.Array);
            }
        }

        /// <summary>
        /// Gets the game binaries directory
        /// </summary>
        public string GetGameBinariesDirectory()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FModuleManager.GetGameBinariesDirectory(Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Checks to see if the specified module exists and is compatible with the current engine version. 
        /// </summary>
        /// <param name="inModuleName">The base name of the module file.</param>
        /// <returns>true if module exists and is up to date, false otherwise.</returns>
        public bool IsModuleUpToDate(FName inModuleName)
        {
            // !IS_MONOLITHIC
            if (Native_FModuleManager.IsModuleUpToDate == null)
            {
                // Default to true or false?
                return false;
            }

            return Native_FModuleManager.IsModuleUpToDate(Address, ref inModuleName);
        }

        /// <summary>
        /// Determines whether the specified module contains UObjects.  The module must already be loaded into
        /// memory before calling this function.
        /// </summary>
        /// <param name="moduleName">ModuleName Name of the loaded module to check.</param>
        /// <returns>True if the module was found to contain UObjects, or false if it did not (or wasn't loaded.)</returns>
        public bool DoesLoadedModuleHaveUObjects(FName moduleName)
        {
            return Native_FModuleManager.DoesLoadedModuleHaveUObjects(Address, ref moduleName);
        }

        /// <summary>
        /// Gets the filename for a module. The return value is a full path of a module known to the module manager.
        /// </summary>
        public string GetModuleFilename(FName moduleName)
        {
            // !IS_MONOLITHIC
            if (Native_FModuleManager.IsModuleUpToDate == null)
            {
                return null;
            }

            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FModuleManager.GetModuleFilename(Address, ref moduleName, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }
    }

    /// <summary>
    /// Enumerates reasons for failed module loads.
    /// </summary>
    public enum EModuleLoadResult : int
    {
        /// <summary>
        /// Module loaded successfully.
        /// </summary>
        Success,

        /// <summary>
        /// The specified module file could not be found.
        /// </summary>
        FileNotFound,

        /// <summary>
        /// The specified module file is incompatible with the module system.
        /// </summary>
        FileIncompatible,

        /// <summary>
        /// The operating system failed to load the module file.
        /// </summary>
        CouldNotBeLoadedByOS,

        /// <summary>
        /// Module initialization failed.
        /// </summary>
        FailedToInitialize
    }
}

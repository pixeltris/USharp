using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Use this instead of AppDomain.GetAssemblies / Assembly.LoadFrom (this is so that hotreload works on .NET Core)
    /// </summary>
    public static class CurrentAssemblyContext
    {
        private static bool initialized = false;
        public static AssemblyContextRef Reference { get; private set; }

        // .NET Core only!
        private static Action<KeyValuePair<long, long>> onUnloading = OnUnloading;
        private static Func<KeyValuePair<long, long>, AssemblyName, Assembly> onResolving = OnResolving;

        public static event Func<AssemblyName, Assembly> Resolving;
        public static event ResolveEventHandler AssemblyResolve;
        public static event AssemblyLoadEventHandler AssemblyLoad;

        // Used on .NET Core to store our own versions of Assembly.CodeBase as we can't set it up ourselves
        private static Dictionary<Assembly, string> assemblyPaths;// Contains the original case preserved path
        private static Dictionary<string, Assembly> assemblyPathsReverse;// ToLower version of the path for lookups

        internal static void Initialize(AssemblyContextRef reference)
        {
            Debug.Assert(!initialized);

            initialized = true;
            Reference = reference;

            if (AssemblyContext.IsCoreCLR && !reference.IsInvalid)
            {
                // No need to remove these events, they will be cleared up when the unload is called on the context
                AssemblyContextProxy.AddUnloadingEvent(reference, onUnloading);
                AssemblyContextProxy.AddUnloadingResolving(reference, onResolving);

                assemblyPaths = new Dictionary<Assembly, string>();
                assemblyPathsReverse = new Dictionary<string, Assembly>();
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        public static string GetFilePath(Assembly assembly)
        {
            if (AssemblyContext.IsCoreCLR && !Reference.IsInvalid)
            {
                string path;
                if (assemblyPaths.TryGetValue(assembly, out path))
                {
                    return path;
                }
            }
            else
            {
                string path = new Uri(assembly.CodeBase).LocalPath;
                if (File.Exists(path))
                {
                    return Path.GetFullPath(path);
                }
            }
            return null;
        }

        public static Assembly[] GetAssemblies()
        {
            if (!initialized)
            {
                return AppDomain.CurrentDomain.GetAssemblies();
            }
            return Reference.GetAssemblies();
        }

        public static Assembly[] GetAllAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public static Assembly LoadFrom(string assemblyPath)
        {
            if (!initialized)
            {
                return Assembly.LoadFrom(assemblyPath);
            }
            if (AssemblyContext.IsCoreCLR && !Reference.IsInvalid)
            {
                // TODO: Use custom "shadow copying" as visual studio will hold onto the pdb file.

                // NOTE: This will destroy Assembly.CodeBase information but CodeBase wouldn't be too useful anyway
                //       if we implemented our own shadow copying.
                if (File.Exists(assemblyPath))
                {
                    string originalAssemblyPath = Path.GetFullPath(assemblyPath);
                    assemblyPath = Path.GetFullPath(assemblyPath).ToLower();

                    Assembly existingAssembly;
                    if (assemblyPathsReverse.TryGetValue(assemblyPath, out existingAssembly))
                    {
                        return existingAssembly;
                    }

                    FileStream stream = File.OpenRead(assemblyPath);
                    FileStream pdbStream = null;
                    
                    try
                    {
                        string pdbFile = Path.ChangeExtension(assemblyPath, ".pdb");
                        if (File.Exists(pdbFile))
                        {
                            pdbStream = File.OpenRead(pdbFile);
                        }
                
                        Assembly assembly = Reference.LoadFromStream(stream, pdbStream);
                        assemblyPaths[assembly] = originalAssemblyPath;
                        assemblyPathsReverse[assemblyPath] = assembly;
                        return assembly;
                    }
                    finally
                    {
                        stream.Close();
                        if (pdbStream != null)
                        {
                            pdbStream.Close();
                        }
                    }
                }
                return null;
            }
            return Reference.LoadFrom(assemblyPath);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (AssemblyResolve != null)
            {
                return AssemblyResolve(sender, args);
            }
            return null;
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (AssemblyLoad != null)
            {
                AssemblyLoad(sender, args);
            }
        }

        private static void OnUnloading(KeyValuePair<long, long> contextRefPair)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.AssemblyLoad -= CurrentDomain_AssemblyLoad;
        }

        private static Assembly OnResolving(KeyValuePair<long, long> contextRefPair, AssemblyName assemblyName)
        {
            if (Resolving != null)
            {
                return Resolving(assemblyName);
            }
            return null;
        }
    }
}

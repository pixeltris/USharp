using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnrealEngine.Runtime
{
    public static class UnrealTypes
    {
        private static HashSet<Type> cctorCalled = new HashSet<Type>();        

        /// <summary>
        /// If true the underlying native type info (addresses/properties/offsets/functions) for types will be loaded when 
        /// accessed rather than when USharp is first initialized. Lazy loading will save time where there are many types but
        /// only a few are actually accessed in managed code. However this comes at a drawback of additional time spent when
        /// types are first accessed at runtime. This could result in hitches depending on the size of the type and how many 
        /// types are being loaded at that time.<para/>
        /// Generally this should be true in the editor for faster hotreload times.
        /// </summary>
        public static bool LazyLoadingEnabled = true;

        /// <summary>
        /// If true Load has been called and the main collections have been initialized.
        /// </summary>
        private static bool gatheredUnrealTypes = false;

        private static HashSet<Assembly> processedAssemblies = new HashSet<Assembly>();
        public static Dictionary<Assembly, List<Type>> Assemblies { get; private set; }
        public static Dictionary<Assembly, Dictionary<Type, USharpPathAttribute>> AssembliesManagedTypes { get; private set; }
        public static Dictionary<Assembly, Dictionary<Type, UMetaPathAttribute>> AssembliesNativeTypes { get; private set; }
        public static Dictionary<Assembly, Type> AssemblySerializedModuleInfo { get; private set; }
        public static Dictionary<Type, UUnrealTypePathAttribute> All { get; private set; }
        public static Dictionary<Type, USharpPathAttribute> Managed { get; private set; }
        public static Dictionary<Type, UMetaPathAttribute> Native { get; private set; }
        public static Dictionary<string, Type> AllByPath { get; private set; }
        public static Dictionary<string, Type> ManagedByPath { get; private set; }
        public static Dictionary<string, Type> NativeByPath { get; private set; }

        public static string GameAssemblyPath { get; internal set; }

        static UnrealTypes()
        {
            Assemblies = new Dictionary<Assembly, List<Type>>();
            AssembliesManagedTypes = new Dictionary<Assembly, Dictionary<Type, USharpPathAttribute>>();
            AssembliesNativeTypes = new Dictionary<Assembly, Dictionary<Type, UMetaPathAttribute>>();
            AssemblySerializedModuleInfo = new Dictionary<Assembly, Type>();
            All = new Dictionary<Type, UUnrealTypePathAttribute>();
            Managed = new Dictionary<Type, USharpPathAttribute>();
            Native = new Dictionary<Type, UMetaPathAttribute>();
            AllByPath = new Dictionary<string, Type>();
            ManagedByPath = new Dictionary<string, Type>();
            NativeByPath = new Dictionary<string, Type>();
        }

        public static void Clear()
        {
            gatheredUnrealTypes = false;
            Assemblies.Clear();
            AssembliesManagedTypes.Clear();
            AssembliesNativeTypes.Clear();
            AssemblySerializedModuleInfo.Clear();
            processedAssemblies.Clear();
            All.Clear();
            Managed.Clear();
            Native.Clear();
            AllByPath.Clear();
            ManagedByPath.Clear();
            NativeByPath.Clear();
        }

        /// <summary>
        /// Gathers all Unreal types (native and managed types) in all loaded assemblies.
        /// </summary>
        public static void Load()
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                LoadInternal(thisAssembly, assembly);
            }

            gatheredUnrealTypes = true;
        }

        /// <summary>
        /// Gathers all Unreal types (native and managed types) in the given assembly.
        /// </summary>
        public static void Load(Assembly assembly)
        {
            LoadInternal(Assembly.GetExecutingAssembly(), assembly);
        }

        private static void LoadInternal(Assembly thisAssembly, Assembly assembly)
        {
            if (processedAssemblies.Contains(assembly))
            {
                return;
            }
            processedAssemblies.Add(assembly);

            bool referencesThisAssembly = false;
            if (assembly == thisAssembly)
            {
                referencesThisAssembly = true;
            }
            else
            {
                foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
                {
                    if (assemblyName.FullName == thisAssembly.FullName)
                    {
                        referencesThisAssembly = true;
                        break;
                    }
                }
            }
            if (!referencesThisAssembly)
            {
                return;
            }
            
            List<Type> types = new List<Type>();
            Dictionary<Type, UMetaPathAttribute> nativeTypes = new Dictionary<Type, UMetaPathAttribute>();
            Dictionary<Type, USharpPathAttribute> managedTypes = new Dictionary<Type, USharpPathAttribute>();
            Type assemblyModuleInfoType = null;

            foreach (Type type in assembly.GetTypes())
            {
                UUnrealTypePathAttribute pathAttribute = type.GetCustomAttribute<UUnrealTypePathAttribute>(false);
                if (pathAttribute != null && !string.IsNullOrEmpty(pathAttribute.Path))
                {
                    USharpPathAttribute sharpPathAttribute = pathAttribute as USharpPathAttribute;
                    if (sharpPathAttribute != null)
                    {
                        AllByPath[pathAttribute.Path] = type;
                        ManagedByPath[pathAttribute.Path] = type;

                        All[type] = sharpPathAttribute;
                        Managed[type] = sharpPathAttribute;

                        types.Add(type);
                        managedTypes[type] = sharpPathAttribute;
                    }
                    else
                    {
                        UMetaPathAttribute metaPathAttribute = pathAttribute as UMetaPathAttribute;
                        if (metaPathAttribute != null)
                        {
                            AllByPath[pathAttribute.Path] = type;
                            NativeByPath[pathAttribute.Path] = type;

                            All[type] = metaPathAttribute;
                            Native[type] = metaPathAttribute;

                            types.Add(type);
                            nativeTypes[type] = metaPathAttribute;
                        }
                    }
                }

                if (typeof(ISerializedManagedUnrealModuleInfo).IsAssignableFrom(type) &&
                    type != typeof(ISerializedManagedUnrealModuleInfo))
                {
                    assemblyModuleInfoType = type;
                }
            }

            if (types.Count > 0)
            {
                Assemblies[assembly] = types;
                AssembliesManagedTypes[assembly] = managedTypes;
                AssembliesNativeTypes[assembly] = nativeTypes;
                if (assemblyModuleInfoType != null)
                {
                    AssemblySerializedModuleInfo[assembly] = assemblyModuleInfoType;
                }
            }
        }

        public static bool IsUnrealType(Type type)
        {
            return All.ContainsKey(type);
        }

        public static bool IsManagedUnrealType(Type type)
        {
            return Managed.ContainsKey(type);
        }

        public static bool IsNativeUnrealType(Type type)
        {
            return Native.ContainsKey(type);
        }

        public static UUnrealTypePathAttribute GetPathAttribute(Type type)
        {
            UUnrealTypePathAttribute attribute;
            All.TryGetValue(type, out attribute);
            return attribute;
        }

        public static UMetaPathAttribute GetNativePathAttribute(Type type)
        {
            UMetaPathAttribute attribute;
            Native.TryGetValue(type, out attribute);
            return attribute;
        }

        public static USharpPathAttribute GetManagedPathAttribute(Type type)
        {
            USharpPathAttribute attribute;
            Managed.TryGetValue(type, out attribute);
            return attribute;
        }

        public static void OnCCtorCalled(Type type)
        {
            cctorCalled.Add(type);
        }

        public static bool HasCCtorBeenCalled(Type type)
        {
            return cctorCalled.Contains(type);
        }

        /// <summary>
        /// Loads the underlying native type info for generated types (types tagged with UMetaPath)
        /// This loads the class address/properties/functions/offsets
        /// </summary>
        public static void LoadNative()
        {
            // Clear NativeReflectionCached just in case it is holding onto old values.
            NativeReflectionCached.Clear();

            foreach (KeyValuePair<Type, UMetaPathAttribute> type in Native)
            {
                LoadNative(type.Key, type.Value);
            }
        }

        /// <summary>
        /// Loads the underlying native type info for generated types (types tagged with UMetaPath)
        /// This loads the class address/properties/functions/offsets
        /// </summary>
        internal static void LoadNative(Assembly assembly)
        {
            // Clear NativeReflectionCached just in case it is holding onto old values.
            NativeReflectionCached.Clear();

            Dictionary<Type, UMetaPathAttribute> nativeTypes;
            if (AssembliesNativeTypes.TryGetValue(assembly, out nativeTypes))
            {
                foreach (KeyValuePair<Type, UMetaPathAttribute> type in nativeTypes)
                {
                    LoadNative(type.Key, type.Value);
                }
            }
        }

        /// <summary>
        /// Loads underlying native type info for the given generated type (types tagged with UMetaPath)
        /// This loads the class address/properties/functions/offsets
        /// </summary>
        private static void LoadNative(Type type, UMetaPathAttribute pathAttribute)
        {
            UnrealInterfacePool.LoadType(type);

            bool lazyLoad = LazyLoadingEnabled && !HasCCtorBeenCalled(type);
            if (!lazyLoad)
            {
                // If this is an interface get the default implementation type which will hold the loader method
                Type targetType = type;
                if (pathAttribute.InterfaceImpl != null)
                {
                    targetType = pathAttribute.InterfaceImpl;
                }

                MethodInfo method = targetType.GetMethod(CodeGeneratorSettings.LoadNativeTypeMethodName,
                    BindingFlags.Static | BindingFlags.NonPublic);

                if (method != null)
                {
                    method.Invoke(null, null);
                }
            }
        }

        /// <summary>
        /// Returns true if the given native type can lazily load properties/offsets/functions when the type is first
        /// accessed rather than when USharp is first initialized.
        /// </summary>
        public static bool CanLazyLoadNativeType(Type type)
        {
            return LazyLoadingEnabled && gatheredUnrealTypes;
        }

        /// <summary>
        /// Returns true if the given managed type can lazily load properties/offsets/functions when the type is first
        /// accessed rather than when USharp is first initialized.
        /// </summary>
        public static bool CanLazyLoadManagedType(Type type)
        {
            return LazyLoadingEnabled && gatheredUnrealTypes && ManagedUnrealTypes.IsTypeRegistered(type);
        }

        /// <summary>
        /// Returns true if the given type can lazily load properties/offsets/functions when the type is first
        /// accessed rather than when USharp is first initialized.
        /// </summary>
        public static bool CanLazyLoadType(Type type)
        {
            if (!LazyLoadingEnabled)
            {
                return false;
            }
            UUnrealTypePathAttribute attribute;
            if (All.TryGetValue(type, out attribute))
            {
                if (attribute.IsManagedType)
                {
                    return CanLazyLoadManagedType(type);
                }
                else
                {
                    return CanLazyLoadNativeType(type);
                }
            }
            return false;
        }
    }

    public delegate void UnrealTypesInitializedHandler();
}

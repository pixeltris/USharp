using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnrealEngine.Engine;
using UnrealEngine.Runtime.ManagedUnrealTypeInfoExceptions;

namespace UnrealEngine.Runtime
{
    public partial class ManagedUnrealModuleInfo : ManagedUnrealReflectionBase
    {
        /// <summary>
        /// If true skip various forms of validation of type flags / attributes
        /// </summary>
        public static readonly bool SkipValidation = false;

        /// <summary>
        /// If true attempt to resolve blittable struct state after processing all types (used on structs
        /// which failed to resolve their blittable struct state due to having a member which referenced another
        /// struct which hadn't finished loading at the time it was referenced).
        /// 
        /// It should be noted that UScriptStruct Bind/StaticLink seem to not allow a POD struct to contain
        /// another POD struct so its possible that the resolving isn't required if we are being safe and copying
        /// the behaviour of native code which requires a full copy of all members of the struct instead of using 
        /// POD copies (memcpy).
        /// </summary>
        public static readonly bool UseBlittableStructResolving = false;

        public static List<ManagedUnrealModuleInfo> Modules { get; private set; }
        public static Dictionary<Type, ManagedUnrealModuleInfo> ModulesByType { get; private set; }
        public static Dictionary<Assembly, ManagedUnrealModuleInfo> ModulesByAssembly { get; private set; }

        /// <summary>
        /// The assembly name as defined by the assembly
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// The module name which is a sanitized version of the assembly name compatible as an unreal module name
        /// </summary>
        public string ModuleName { get; set; }

        public List<ManagedUnrealTypeInfo> Classes { get; set; }
        public List<ManagedUnrealTypeInfo> Structs { get; set; }
        public List<ManagedUnrealEnumInfo> Enums { get; set; }
        public List<ManagedUnrealTypeInfo> Interfaces { get; set; }
        public List<ManagedUnrealTypeInfo> Delegates { get; set; }

        public HashSet<string> ReferencedAssemblies { get; set; }

        // These aren't serialized. They are populated from the Classes,Structs,Enums,etc
        public Dictionary<Type, ManagedUnrealTypeInfo> TypeInfosByType { get; private set; }
        public Dictionary<ManagedUnrealTypeInfo, Type> TypesByTypeInfo { get; private set; }
        public Dictionary<string, ManagedUnrealTypeInfo> TypeInfosByPath { get; private set; }

        private Dictionary<string, List<Type>> typesByName = new Dictionary<string, List<Type>>();

        // Share the code settings between all modules
        private static CodeGeneratorSettings codeSettings;

        // The seen assemblies (used by pre-processed assembly info (AllTypesByPath, AllTypeInfosByPath, 
        // AllKnownUnrealTypes, AllKnownBlittableTypes, AllKnownNonBlittableTypes))
        private static HashSet<Assembly> seenAssemblies = new HashSet<Assembly>();

        /// <summary>
        /// Holds all types by path (both C# defined and native). 
        /// </summary>
        internal static Dictionary<string, Type> AllTypesByPath { get; set; }

        /// <summary>
        /// Holds all type infos by path (C# defined types only, native types don't have ManagedUnrealTypeInfo)
        /// </summary>
        internal static Dictionary<string, ManagedUnrealTypeInfo> AllTypeInfosByPath { get; set; }

        /// <summary>
        /// ALL known unreal types by path (obtained from UMetaPath / USharpPath attributes on all loaded assemblies)
        /// </summary>
        public static Dictionary<string, Type> AllKnownUnrealTypes { get; private set; }

        /// <summary>
        /// ALL known blittable unreal types by path (types tagged with UMetaPath / USharpPath and don't have FromNative / ToNative marshalers)
        /// </summary>
        public static Dictionary<string, Type> AllKnownBlittableTypes { get; private set; }

        /// <summary>
        /// ALL known non-blittable unreal types by path (types tagged with UMetaPath / USharpPath and have FromNative / ToNative marshalers)
        /// </summary>
        public static Dictionary<string, Type> AllKnownNonBlittableTypes { get; private set; }

        // Cache used for finding if a struct type will have a ctor/dtor and thus an init/destroy requirement.
        // This is used for typed defined in managed code rather than the ones defined in native code / BP.
        // TODO: Clear types belonging to a module when it is loaded a second time.
        internal static Dictionary<Type, EStructFlags> resolvedStructCtorDtorFlags = new Dictionary<Type, EStructFlags>();

        // Cache function flags to avoid processing flags multiple times where we want to obtain base function flags
        private static Dictionary<MethodInfo, CachedFunctionFlagInfo> cachedFunctionFlags =
            new Dictionary<MethodInfo, CachedFunctionFlagInfo>();
        private static ManagedUnrealFunctionInfo cachedDummyFunctionInfo = new ManagedUnrealFunctionInfo();// Just for flags

        // Cache class flags to avoid processing flags multiple times where we want to obtain base class flags
        // Also cache the class config name can be inherited
        private static Dictionary<Type, KeyValuePair<EClassFlags, ManagedUnrealTypeInfoFlags>> cachedClassFlags =
            new Dictionary<Type, KeyValuePair<EClassFlags, ManagedUnrealTypeInfoFlags>>();
        private static Dictionary<Type, string> cachedClassConfigName = new Dictionary<Type, string>();
        private static ManagedUnrealTypeInfo cachedDummyClassInfo = new ManagedUnrealTypeInfo();// Just for flags

        // Cache the "blueprintability" (BlueprintType / Blueprintable) for each type
        private static Dictionary<Type, Blueprintability> cachedBlueprintability = new Dictionary<Type, Blueprintability>();

        // Cache the size of enums for validation when used by properties which are exposed to blueprint
        // (this is roughly the size of the enum in bytes - based on what it can fit into)
        private static Dictionary<Type, byte> cachedCalculatedEnumSize = new Dictionary<Type, byte>();

        // Cache implemented interfaces which inherit from IInterface. This is used to avoid multiple lookups
        // for getting interface info. It also ensures it only contains the interfaces which the given type implements
        // not the full list of every interfaces available on the given type.
        private static Dictionary<Type, List<Type>> cachedImplementedInterfaces = new Dictionary<Type, List<Type>>();

        // Cache the implemented interface mapping for Unreal types.
        // <Type (which implements interfaces), <type method, interface method>>
        private static Dictionary<Type, Dictionary<MethodInfo, MethodInfo>> cachedImplementedInterfacesFunctionMap =
            new Dictionary<Type, Dictionary<MethodInfo, MethodInfo>>();

        // A list of types which aren't exportable. Used to avoid attempting to export types multiple times.
        private static HashSet<Type> unexportableTypes = new HashSet<Type>();

        public ManagedUnrealModuleInfo()
        {
            Classes = new List<ManagedUnrealTypeInfo>();
            Structs = new List<ManagedUnrealTypeInfo>();
            Enums = new List<ManagedUnrealEnumInfo>();
            Interfaces = new List<ManagedUnrealTypeInfo>();
            Delegates = new List<ManagedUnrealTypeInfo>();

            ReferencedAssemblies = new HashSet<string>();
            TypeInfosByType = new Dictionary<Type, ManagedUnrealTypeInfo>();
            TypesByTypeInfo = new Dictionary<ManagedUnrealTypeInfo, Type>();
            TypeInfosByPath = new Dictionary<string, ManagedUnrealTypeInfo>();
        }

        static ManagedUnrealModuleInfo()
        {
            Modules = new List<ManagedUnrealModuleInfo>();
            ModulesByType = new Dictionary<Type, ManagedUnrealModuleInfo>();
            ModulesByAssembly = new Dictionary<Assembly, ManagedUnrealModuleInfo>();

            seenAssemblies = new HashSet<Assembly>();
            AllTypesByPath = new Dictionary<string, Type>();
            AllTypeInfosByPath = new Dictionary<string, ManagedUnrealTypeInfo>();
            AllKnownUnrealTypes = new Dictionary<string, Type>();
            AllKnownBlittableTypes = new Dictionary<string, Type>();
            AllKnownNonBlittableTypes = new Dictionary<string, Type>();
            PreProcessAssemblies();

            codeSettings = new CodeGeneratorSettings();
            codeSettings.Load();
        }

        public static void PreProcessAssemblies()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                PreProcessAssembly(assembly);
            }
        }

        /// <summary>
        /// Pre processes a given assembly to extract basic information before loading the module info.
        /// (obtains UMetaPath / USharpPath types and paths)
        /// </summary>
        internal static void PreProcessAssembly(Assembly assembly)
        {
            if (seenAssemblies.Contains(assembly))
            {
                return;
            }

            seenAssemblies.Add(assembly);
            UnrealTypes.Load(assembly);
            
            List<Type> types;
            if (UnrealTypes.Assemblies.TryGetValue(assembly, out types))
            {
                foreach (Type type in types)
                {
                    UUnrealTypePathAttribute pathAttribute = UnrealTypes.GetPathAttribute(type);
                    if (!pathAttribute.IsManagedType)
                    {
                        // DON'T set AllTypesByPath for managed types (it will be set when loading the module info)
                        AllTypesByPath[pathAttribute.Path] = type;
                    }
                    string path = pathAttribute.Path;

                    Debug.Assert(path == "/Script/CoreUObject.Object" ? type == typeof(UObject) : true);
                    AllKnownUnrealTypes[path] = type;

                    if (type.IsValueType)
                    {
                        bool isBlittable = true;
                        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                        {
                            if (method.Name == "FromNative")
                            {
                                isBlittable = false;
                                break;
                            }
                        }
                        if (isBlittable)
                        {
                            AllKnownBlittableTypes[path] = type;
                        }
                        else
                        {
                            AllKnownNonBlittableTypes[path] = type;
                        }
                    }
                }
            }
        }

        public static void Load()
        {
            Modules.Clear();
            ModulesByType.Clear();
            ModulesByAssembly.Clear();

            seenAssemblies.Clear();
            AllTypesByPath.Clear();
            AllTypeInfosByPath.Clear();
            AllKnownUnrealTypes.Clear();
            AllKnownBlittableTypes.Clear();
            AllKnownNonBlittableTypes.Clear();
            PreProcessAssemblies();
            
            HashSet<string> seenReferences = new HashSet<string>();
            seenReferences.Add(Assembly.GetExecutingAssembly().GetName().Name);

            foreach (KeyValuePair<Assembly, Type> assembly in UnrealTypes.AssemblySerializedModuleInfo)
            {
                ManagedUnrealModuleInfo moduleInfo = LoadModuleFromAssembly(assembly.Key, assembly.Value);
                seenReferences.Add(moduleInfo.AssemblyName);
            }

            // TODO: Improve this. This likely isn't enough for anything more than trivial assembly setups.
            if (File.Exists(UnrealTypes.GameAssemblyPath))
            {
                string dir = System.IO.Path.GetDirectoryName(UnrealTypes.GameAssemblyPath);
                foreach (ManagedUnrealModuleInfo module in Modules)
                {
                    foreach (string assemblyReference in module.ReferencedAssemblies)
                    {
                        if (seenReferences.Add(assemblyReference))
                        {
                            string assemblyPath = System.IO.Path.Combine(dir, assemblyReference + ".dll");
                            if (File.Exists(assemblyPath))
                            {
                                try
                                {
                                    // NOTE: This depends on NativeFunctions.OnAssemblyLoad loading the type info
                                    Assembly.LoadFrom(assemblyPath);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public static ManagedUnrealModuleInfo LoadModuleFromAssembly(Assembly assembly)
        {
            Type serializedModuleInfoType = GetSerializedModuleInfoTypeFromAssembly(assembly);
            if (serializedModuleInfoType != null)
            {
                return LoadModuleFromAssembly(assembly, serializedModuleInfoType);
            }
            return null;
        }

        private static ManagedUnrealModuleInfo LoadModuleFromAssembly(Assembly assembly, Type serializedModuleInfoType)
        {
            string serializedModuleInfo = ((ISerializedManagedUnrealModuleInfo)Activator.CreateInstance(serializedModuleInfoType)).GetString();
            ManagedUnrealModuleInfo moduleInfo = ManagedUnrealReflectionBase.Deserialize<ManagedUnrealModuleInfo>(serializedModuleInfo);
            if (moduleInfo != null)
            {
                Modules.Add(moduleInfo);
                ModulesByAssembly.Add(assembly, moduleInfo);
                moduleInfo.OnDeserialized(assembly);
                return moduleInfo;
            }
            return null;
        }

        public static Type GetSerializedModuleInfoTypeFromAssembly(Assembly assembly)
        {
            Type serializedModuleInfoType;
            UnrealTypes.AssemblySerializedModuleInfo.TryGetValue(assembly, out serializedModuleInfoType);
            return serializedModuleInfoType;
        }

        public static bool AssemblyHasSerializedModuleInfo(Assembly assembly)
        {
            return GetSerializedModuleInfoTypeFromAssembly(assembly) != null;
        }

        public static ManagedUnrealModuleInfo CreateModuleFromAssembly(Assembly assembly)
        {
            if (!seenAssemblies.Contains(assembly))
            {
                // It could be possibly that this assembly hadn't loaded when we first initialized all of the
                // native type paths. Attempt to load them now.
                PreProcessAssembly(assembly);
            }

            ManagedUnrealModuleInfo module = null;
            if (ModulesByAssembly.TryGetValue(assembly, out module))
            {
                return module;
            }

            module = new ManagedUnrealModuleInfo();
            module.AssemblyName = assembly.GetName().Name;

            Modules.Add(module);
            ModulesByAssembly.Add(assembly, module);

            // Remove any chars which would conflict with unreal path string
            StringBuilder moduleName = new StringBuilder(module.AssemblyName);
            char[] invalidAssemblyChars = { '.' };
            for (int i = 0; i < module.AssemblyName.Length; ++i)
            {
                if (invalidAssemblyChars.Contains(module.AssemblyName[i]))
                {
                    moduleName[i] = '_';
                }
            }
            module.ModuleName = moduleName.ToString();

            foreach (Type type in assembly.GetTypes())
            {
                if (!module.TypeInfosByType.ContainsKey(type) && ManagedUnrealTypeInfo.IsExportableType(type))
                {
                    string typeName = ManagedUnrealTypeInfo.GetTypeNameWithoutPrefix(type, ManagedUnrealTypeInfo.GetTypeCode(type));

                    List<Type> types;
                    if (!module.typesByName.TryGetValue(typeName, out types))
                    {
                        module.typesByName.Add(typeName, types = new List<Type>());
                    }
                    types.Add(type);

                    module.ProcessType(type);
                }
            }

            if (UseBlittableStructResolving)
            {
                module.ResolveStructBlittableStates();
            }

            module.LateResolveMissingClassFlags();

            return module;
        }

        private void LateResolveMissingClassFlags()
        {
            // Some class flags must be resolved based on the parent flags and class properties.
            // Due to how types are loaded these flags have to be resolved after the module is created.
            // (Currently when processing any given type, if it sees an unknown type it  will immediately 
            //  start processing that type before the current one is finished.)

            foreach (ManagedUnrealTypeInfo typeInfo in Classes)
            {
                KeyValuePair<EClassFlags, ManagedUnrealTypeInfoFlags> flags;
                string classConfigName;
                if (TryGetClassFlags(typeInfo, TypesByTypeInfo[typeInfo], true, out flags, out classConfigName))
                {
                    typeInfo.ClassFlags |= flags.Key;
                    typeInfo.AdditionalFlags |= flags.Value;
                    typeInfo.ClassConfigName = classConfigName;
                }
            }
            foreach (ManagedUnrealTypeInfo typeInfo in Interfaces)
            {
                KeyValuePair<EClassFlags, ManagedUnrealTypeInfoFlags> flags;
                string classConfigName;
                if (TryGetClassFlags(typeInfo, TypesByTypeInfo[typeInfo], true, out flags, out classConfigName))
                {
                    typeInfo.ClassFlags |= flags.Key;
                    typeInfo.AdditionalFlags |= flags.Value;
                    typeInfo.ClassConfigName = classConfigName;
                }
            }
        }

        private void ResolveStructBlittableStates()
        {
            // Handle unresolved blittable state of structs after processing all types. Really we should ensure structs 
            // get processed in the correct order the first time around but the original code was set up to resolve 
            // types in a way which would break any pre-sorting of dependencies (e.g. a struct references a class which
            // which references a struct which would get processed outside of the disired dependency order). So either 
            // change struct processing code to not fully resolve members or stick with a post-process step to resolve 
            // blittable state.

            // Here we build a similar dependency map to ManagedUnrealTypes.Builder.cs
            // This assumes there can't be circular dependencies (if there are this will result in a stack overflow)

            Dictionary<Type, ManagedUnrealTypeInfo> structTypes = new Dictionary<Type, ManagedUnrealTypeInfo>();
            Dictionary<Type, HashSet<Type>> structDepends = new Dictionary<Type, HashSet<Type>>();
            foreach (KeyValuePair<Type, ManagedUnrealTypeInfo> typeInfo in TypeInfosByType)
            {
                if (typeInfo.Value.TypeCode != EPropertyType.Struct)
                {
                    continue;
                }

                structTypes.Add(typeInfo.Key, typeInfo.Value);

                HashSet<Type> dependsList = new HashSet<Type>();
                structDepends.Add(typeInfo.Key, dependsList);

                // Keep bindingFlags in sync with ProcessStructFields
                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
                foreach (FieldInfo field in typeInfo.Key.GetFields(bindingFlags))
                {
                    if (field.FieldType.IsValueType && structTypes.ContainsKey(field.FieldType))
                    {
                        dependsList.Add(field.FieldType);
                    }
                }
            }

            HashSet<Type> resolvedStructs = new HashSet<Type>();
            foreach (Type type in structDepends.Keys)
            {
                if (!resolvedStructs.Contains(type))
                {
                    ResolveStructBlittableStateAndDependencies(type, structTypes, structDepends, resolvedStructs);
                }
            }
        }

        private void ResolveStructBlittableStateAndDependencies(Type type,
            Dictionary<Type, ManagedUnrealTypeInfo> structTypes,
            Dictionary<Type, HashSet<Type>> structDepends,
            HashSet<Type> resolvedStructs)
        {
            ManagedUnrealTypeInfo typeInfo = structTypes[type];
            if (typeInfo.BlittableKind != ManagedUnrealBlittableKind.Unresolved)
            {
                return;
            }

            while (structDepends[type].Count > 0)
            {
                Type dependency = structDepends[type].First();
                if (!resolvedStructs.Contains(dependency))
                {
                    ResolveStructBlittableStateAndDependencies(dependency, structTypes, structDepends, resolvedStructs);
                }
                structDepends[type].Remove(dependency);
            }

            typeInfo.BlittableKind = ManagedUnrealBlittableKind.Blittable;
            foreach (ManagedUnrealPropertyInfo propertyInfo in typeInfo.Properties)
            {
                ManagedUnrealBlittableKind blittableKind = GetBlittableKind(propertyInfo.Type);
                if (blittableKind == ManagedUnrealBlittableKind.NotBlittable ||
                    blittableKind == ManagedUnrealBlittableKind.Unresolved)
                {
                    typeInfo.BlittableKind = ManagedUnrealBlittableKind.NotBlittable;
                    break;
                }
            }

            resolvedStructs.Add(type);
        }

        private void OnDeserialized(Assembly assembly)
        {
            if (!seenAssemblies.Contains(assembly))
            {
                // It could be possibly that this assembly hadn't loaded when we first initialized all of the
                // native type paths. Attempt to load them now.
                PreProcessAssembly(assembly);
            }

            // This link up Type / ManagedUnrealTypeInfo collections
            TypeInfosByType.Clear();
            TypesByTypeInfo.Clear();
            TypeInfosByPath.Clear();

            List<ManagedUnrealTypeInfo> allTypes = new List<ManagedUnrealTypeInfo>();
            allTypes.AddRange(Classes);
            allTypes.AddRange(Structs);
            allTypes.AddRange(Enums);
            allTypes.AddRange(Interfaces);
            allTypes.AddRange(Delegates);

            foreach (ManagedUnrealTypeInfo typeInfo in allTypes)
            {
                TypeInfosByPath.Add(typeInfo.Path, typeInfo);
            }

            List<Type> unrealTypes;
            if (UnrealTypes.Assemblies.TryGetValue(assembly, out unrealTypes))
            {
                foreach (Type type in unrealTypes)
                {
                    USharpPathAttribute pathAttribute;
                    if (UnrealTypes.Managed.TryGetValue(type, out pathAttribute))
                    {
                        ManagedUnrealTypeInfo typeInfo = TypeInfosByPath[pathAttribute.Path];
                        TypeInfosByType.Add(type, typeInfo);
                        TypesByTypeInfo.Add(typeInfo, type);
                    }
                }
            }

            foreach (KeyValuePair<ManagedUnrealTypeInfo, Type> typeInfo in TypesByTypeInfo)
            {
                ModulesByType.Add(typeInfo.Value, this);

                // Add type info to add the static collections (hopefully there are no duplicate paths)
                AllTypeInfosByPath.Add(typeInfo.Key.Path, typeInfo.Key);
                AllTypesByPath.Add(typeInfo.Key.Path, typeInfo.Value);
            }
        }

        private string GetTypePath(Type type)
        {
            return GetTypePath(type, ManagedUnrealTypeInfo.GetTypeCode(type));
        }

        private string GetTypePath(Type type, EPropertyType typeCode)
        {
            // This check might be overkill here but in order to ensure types are visible in UnrealTypes
            // the assembly must be loaded at this point or GetNativePathAttribute will return null on first access
            if (!seenAssemblies.Contains(type.Assembly))
            {
                CreateModuleFromAssembly(type.Assembly);
            }

            UMetaPathAttribute pathAttribute = UnrealTypes.GetNativePathAttribute(type);
            if (pathAttribute != null)
            {
                ReferencedAssemblies.Add(type.Assembly.GetName().Name);
                return pathAttribute.Path;
            }

            switch (typeCode)
            {
                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                case EPropertyType.Enum:
                case EPropertyType.Interface:
                case EPropertyType.Struct:
                case EPropertyType.Object:
                    ManagedUnrealTypeInfo typeInfo = FindType(type, typeCode);
                    if (typeInfo == null)
                    {
                        ProcessType(type);
                        typeInfo = FindType(type);
                    }
                    if (typeInfo != null)
                    {
                        ReferencedAssemblies.Add(type.Assembly.GetName().Name);
                        return typeInfo.Path;
                    }
                    return null;
                case EPropertyType.SoftClass:
                case EPropertyType.SoftObject:
                case EPropertyType.WeakObject:
                case EPropertyType.LazyObject:
                case EPropertyType.Class:
                    return GetTypePath(type.GenericTypeArguments[0]);
                default:
                    return null;
            }
        }

        private ManagedUnrealTypeInfo ProcessType(Type type)
        {
            if (unexportableTypes.Contains(type))
            {
                return null;
            }
            else if (!ManagedUnrealTypeInfo.IsExportableType(type))
            {
                unexportableTypes.Add(type);
                return null;
            }

            System.Diagnostics.Debug.WriteLine("Process " + type.FullName);

            ManagedUnrealModuleInfo module;
            if (!ModulesByAssembly.TryGetValue(type.Assembly, out module))
            {
                module = CreateModuleFromAssembly(type.Assembly);
            }

            if (module != this)
            {
                module.ProcessType(type);
                return null;
            }

            ManagedUnrealTypeInfo typeInfo = CreateTypeInfoDecl(type);
            if (typeInfo == null)
            {
                return null;
            }

            switch (typeInfo.TypeCode)
            {
                case EPropertyType.Object:
                    Classes.Add(typeInfo);
                    break;
                case EPropertyType.Struct:
                    Structs.Add(typeInfo);
                    break;
                case EPropertyType.Enum:
                    ManagedUnrealEnumInfo enumInfo = typeInfo as ManagedUnrealEnumInfo;
                    if (enumInfo == null)
                    {
                        return null;
                    }
                    Enums.Add(enumInfo);
                    break;
                case EPropertyType.Interface:
                    Interfaces.Add(typeInfo);
                    break;
                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    Delegates.Add(typeInfo);
                    break;
                default:
                    return null;
            }
            TypeInfosByType.Add(type, typeInfo);
            TypesByTypeInfo.Add(typeInfo, type);
            TypeInfosByPath.Add(typeInfo.Path, typeInfo);
            ModulesByType.Add(type, this);

            // Add type info to add the static collections (hopefully there are no duplicate paths)
            AllTypeInfosByPath.Add(typeInfo.Path, typeInfo);
            AllTypesByPath.Add(typeInfo.Path, type);

            // Create the type info body after adding it to the collections so that properties can resolve properly
            CreateTypeInfoBody(typeInfo, type);

            return typeInfo;
        }

        private string GetUniqueMemberName(ManagedUnrealTypeInfo typeInfo, ManagedUnrealPropertyInfo propertyInfo, MemberInfo member)
        {
            return member.Name;
        }

        private string GetUniqueMemberName(ManagedUnrealTypeInfo typeInfo, ManagedUnrealFunctionInfo functionInfo, MemberInfo member)
        {
            string name = member.Name;
            if (codeSettings.UseExplicitImplementationMethods && functionInfo.IsBlueprintEvent && functionInfo.IsOverride &&
                member.Name.EndsWith(codeSettings.VarNames.ImplementationMethod))
            {
                name = name.Substring(0, name.Length - codeSettings.VarNames.ImplementationMethod.Length);
            }
            return name;
        }

        private string GetUniqueTypeName(ManagedUnrealTypeInfo typeInfo, Type type)
        {
            string typeName = ManagedUnrealTypeInfo.GetTypeNameWithoutPrefix(type, typeInfo.TypeCode);

            switch (typeInfo.TypeCode)
            {
                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    // Unreal requires the suffix "__DelegateSignature" to exist on delegates in various places
                    // See references to HEADER_GENERATED_DELEGATE_SIGNATURE_SUFFIX TEXT("__DelegateSignature")
                    typeName += "__DelegateSignature";
                    break;
            }

            List<Type> typesWithName;
            if (typesByName.TryGetValue(typeName, out typesWithName))
            {
                if (typesWithName.Count > 1)
                {
                    throw new ManagedUnrealTypeInfoException("TODO: Handle type name conflicts. Conflicting types: " +
                        Environment.NewLine + string.Join(Environment.NewLine, typesWithName.Select(x => x.FullName)));
                }
            }

            return typeName;
        }

        private ManagedUnrealTypeInfo CreateTypeInfoDecl(Type type)
        {
            EPropertyType typeCode = ManagedUnrealTypeInfo.GetTypeCode(type);
            switch (typeCode)
            {
                case EPropertyType.Object:
                case EPropertyType.Enum:
                case EPropertyType.Interface:
                case EPropertyType.Struct:
                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    return CreateTypeInfoDecl(type, typeCode);
            }
            return null;
        }

        private ManagedUnrealTypeInfo CreateTypeInfoDecl(Type type, EPropertyType typeCode)
        {
            ManagedUnrealTypeInfo typeInfo = null;
            if (typeCode == EPropertyType.Enum)
            {
                typeInfo = new ManagedUnrealEnumInfo();
            }
            else
            {
                typeInfo = new ManagedUnrealTypeInfo();
            }
            typeInfo.TypeCode = typeCode;

            switch (typeCode)
            {
                case EPropertyType.Object:
                case EPropertyType.Enum:
                case EPropertyType.Interface:
                case EPropertyType.Struct:
                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    typeInfo.FullName = type.FullName;
                    typeInfo.Name = GetUniqueTypeName(typeInfo, type);
                    typeInfo.Path = "/Script/" + ModuleName + "." + typeInfo.Name;
                    break;
            }

            if (typeCode == EPropertyType.Struct && type.IsSubclassOf(typeof(StructAsClass)))
            {
                typeInfo.IsStructAsClass = true;
            }

            if (typeInfo.IsClass || typeInfo.IsInterface)
            {
                // This will handle the all of the class attributes and cache the result
                KeyValuePair<EClassFlags, ManagedUnrealTypeInfoFlags> flags;
                string classConfig;
                if (!TryGetClassFlags(typeInfo, type, false, out flags, out classConfig))
                {
                    return null;
                }
                typeInfo.ClassFlags = flags.Key;
                typeInfo.AdditionalFlags = flags.Value;
                typeInfo.ClassConfigName = classConfig;
            }
            else
            {
                var attributes = type.GetCustomAttributes<ManagedUnrealAttributeBase>(false);
                if (attributes != null)
                {
                    foreach (ManagedUnrealAttributeBase attribute in attributes)
                    {
                        switch (typeCode)
                        {
                            case EPropertyType.Struct:
                                attribute.ProcessStruct(typeInfo);
                                break;
                            case EPropertyType.Enum:
                                attribute.ProcessEnum(typeInfo);
                                break;
                            case EPropertyType.Delegate:
                            case EPropertyType.MulticastDelegate:
                                attribute.ProcessDelegate(typeInfo);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        if (attribute.InvalidTarget)
                        {
                            if (attribute.InvalidTargetReason != null)
                            {
                                throw new InvalidManagedUnrealAttributeException(type, attribute);
                            }
                            return null;
                        }
                    }
                }

                // Only need to do this for struct / enum (class handles it when calling TryGetClassFlags)
                SetBlueprintability(typeInfo, type);
            }

            return typeInfo;
        }

        private void CreateTypeInfoBody(ManagedUnrealTypeInfo typeInfo, Type type)
        {
            switch (typeInfo.TypeCode)
            {
                case EPropertyType.Object:
                    UpdateBaseType(typeInfo, type);
                    ProcessProperties(typeInfo, type);
                    ProcessFunctions(typeInfo, type);
                    typeInfo.BlittableKind = codeSettings.UObjectAsBlittableType ?
                        ManagedUnrealBlittableKind.Blittable : ManagedUnrealBlittableKind.NotBlittable;
                    ValidateClass(typeInfo, type);
                    break;

                case EPropertyType.Struct:
                    UpdateBaseType(typeInfo, type);
                    if (typeInfo.IsStructAsClass)
                    {
                        ProcessProperties(typeInfo, type);
                        typeInfo.BlittableKind = ManagedUnrealBlittableKind.NotBlittable;
                    }
                    else
                    {
                        if (typeInfo.BlittableKind != ManagedUnrealBlittableKind.ForceBlittable)
                        {
                            typeInfo.BlittableKind = ManagedUnrealBlittableKind.Unresolved;
                        }

                        ProcessStructFields(typeInfo, type);

                        if (typeInfo.BlittableKind == ManagedUnrealBlittableKind.Unresolved)
                        {
                            bool allMembersBlittable = true;
                            foreach (ManagedUnrealPropertyInfo propertyInfo in typeInfo.Properties)
                            {
                                ManagedUnrealBlittableKind blittableKind = GetBlittableKind(propertyInfo.Type);
                                if (blittableKind == ManagedUnrealBlittableKind.NotBlittable)
                                {
                                    typeInfo.BlittableKind = ManagedUnrealBlittableKind.NotBlittable;
                                    allMembersBlittable = false;
                                    break;
                                }
                                else if (blittableKind == ManagedUnrealBlittableKind.Unresolved)
                                {
                                    allMembersBlittable = false;
                                    break;
                                }
                            }
                            if (allMembersBlittable)
                            {
                                typeInfo.BlittableKind = ManagedUnrealBlittableKind.Blittable;
                            }
                            else if (!UseBlittableStructResolving)
                            {
                                typeInfo.BlittableKind = ManagedUnrealBlittableKind.NotBlittable;
                            }
                        }
                    }
                    break;

                case EPropertyType.Interface:
                    UpdateBaseType(typeInfo, type);
                    ProcessFunctions(typeInfo, type);
                    break;

                case EPropertyType.Enum:
                    ManagedUnrealEnumInfo enumInfo = typeInfo as ManagedUnrealEnumInfo;
                    UpdateBaseType(enumInfo, type);
                    ProcessEnumValues(enumInfo, type);
                    break;

                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    ProcessDelegateSignature(typeInfo, type);
                    break;
            }
        }

        /// <summary>
        /// Returns a list of implemented interfaces which inherit from IInterface
        /// </summary>
        private List<Type> GetImplementedInterfaces(Type type)
        {
            List<Type> interfaces;
            if (cachedImplementedInterfaces.TryGetValue(type, out interfaces))
            {
                return interfaces;
            }

            List<Type> allInterfaces = new List<Type>();
            List<Type> unrealInterfaces = new List<Type>();
            allInterfaces.AddRange(type.GetInterfaces());

            if (type.BaseType != null)
            {
                foreach (Type baseTypeInterface in type.BaseType.GetInterfaces())
                {
                    allInterfaces.Remove(baseTypeInterface);
                }
            }

            foreach (Type typeInterface in allInterfaces)
            {
                EPropertyType typeCode = ManagedUnrealTypeInfo.GetTypeCode(typeInterface);
                if (typeCode == EPropertyType.Interface)
                {
                    unrealInterfaces.Add(typeInterface);
                }
            }

            cachedImplementedInterfaces.Add(type, unrealInterfaces);
            return unrealInterfaces;
        }

        private void UpdateBaseType(ManagedUnrealTypeInfo typeInfo, Type type)
        {
            Dictionary<Type, EPropertyType> baseTypes = new Dictionary<Type, EPropertyType>();

            if (type.BaseType != null)
            {
                baseTypes.Add(type.BaseType, ManagedUnrealTypeInfo.GetTypeCode(type));
            }
            foreach (Type interfaceType in GetImplementedInterfaces(type))
            {
                baseTypes.Add(interfaceType, EPropertyType.Interface);
            }

            bool isEnum = typeInfo as ManagedUnrealEnumInfo != null;
            typeInfo.BaseTypes.Clear();

            foreach (KeyValuePair<Type, EPropertyType> baseType in new Dictionary<Type, EPropertyType>(baseTypes))
            {
                switch (baseType.Value)
                {
                    case EPropertyType.Object:
                    case EPropertyType.Interface:
                        string path = GetTypePath(baseType.Key, baseType.Value);
                        if (!isEnum && !string.IsNullOrEmpty(path))
                        {
                            typeInfo.BaseTypes.Add(new ManagedUnrealTypeInfoReference(baseType.Value, path));
                        }
                        break;

                    case EPropertyType.Int8:
                    case EPropertyType.Byte:
                    case EPropertyType.Int16:
                    case EPropertyType.UInt16:
                    case EPropertyType.Int:
                    case EPropertyType.UInt32:
                    case EPropertyType.Int64:
                    case EPropertyType.UInt64:
                        if (isEnum)
                        {
                            typeInfo.BaseTypes.Add(new ManagedUnrealTypeInfoReference(baseType.Value));
                        }
                        break;
                }
            }
        }

        private void ProcessDelegateSignature(ManagedUnrealTypeInfo typeInfo, Type type)
        {
            List<Type> delegateSignatureTypes = new List<Type>();

            Type[] nested = type.GetNestedTypes();
            foreach (Type nestedType in type.GetNestedTypes())
            {
                if (nestedType.IsSubclassOf(typeof(Delegate)))
                {
                    delegateSignatureTypes.Add(nestedType);
                }
            }

            if (delegateSignatureTypes.Count != 1)
            {
                // "Delegate signature" being the nested delegate type e.g. "public delegate void Signature(int param1, int param2);"
                throw new InvalidUnrealDelegateException(type,
                    "Bad delegate signature count . Found: " + delegateSignatureTypes.Count + " Expected: 1. (The \"delegate signature\" is the " +
                    "singular nested delegate typically with a name of \"Signature\" which defines the signature of your delegate)");
            }

            MethodInfo method = delegateSignatureTypes[0].GetMethod("Invoke");

            ManagedUnrealFunctionInfo functionInfo = CreateFunction(typeInfo, type, method);
            if (functionInfo != null)
            {
                functionInfo.Flags |= EFunctionFlags.Public | EFunctionFlags.Delegate;
                if (typeInfo.TypeCode == EPropertyType.MulticastDelegate)
                {
                    functionInfo.Flags |= EFunctionFlags.MulticastDelegate;
                }

                functionInfo.Name = GetUniqueMemberName(typeInfo, functionInfo, method);
                functionInfo.Path = typeInfo.Path + ":" + functionInfo.Name;
                typeInfo.Functions.Add(functionInfo);

                ValidateDelegate(typeInfo, type, functionInfo, method);
            }
        }

        private void ProcessEnumValues(ManagedUnrealEnumInfo enumInfo, Type type)
        {
            byte calculatedSize;
            Dictionary<string, ulong> namesValues = type.GetEnumNamesValues(out calculatedSize);
            foreach (KeyValuePair<string, ulong> nameValue in namesValues)
            {
                ManagedUnrealEnumValueInfo valueInfo = new ManagedUnrealEnumValueInfo();
                valueInfo.Name = nameValue.Key;
                valueInfo.Value = nameValue.Value;
                enumInfo.EnumValues.Add(valueInfo);
            }

            if (!cachedCalculatedEnumSize.ContainsKey(type))
            {
                cachedCalculatedEnumSize.Add(type, calculatedSize);
            }
            
            SetBlueprintability(enumInfo, type);
            ValidateEnum(enumInfo, type);
        }

        private byte GetEnumByteSize(Type type)
        {
            byte enumSize;
            if (!cachedCalculatedEnumSize.TryGetValue(type, out enumSize))
            {
                enumSize = type.GetEnumByteSize();
                cachedCalculatedEnumSize.Add(type, enumSize);
            }
            return enumSize;
        }

        private void ProcessStructFields(ManagedUnrealTypeInfo typeInfo, Type type)
        {
            int numFields = 0;// Total number of fields
            int numExportedFields = 0;// Total number of fields to be exported

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            foreach (FieldInfo field in type.GetFields(bindingFlags))
            {
                numFields++;
                ManagedUnrealPropertyInfo propertyInfo = CreateProperty(field);
                if (propertyInfo != null)
                {
                    numExportedFields++;
                    propertyInfo.IsField = true;
                    propertyInfo.Name = GetUniqueMemberName(typeInfo, propertyInfo, field);
                    propertyInfo.Path = typeInfo.Path + ":" + propertyInfo.Name;
                    typeInfo.Properties.Add(propertyInfo);

                    ValidateProperty(typeInfo, type, propertyInfo, field.FieldType, field);
                }
            }

            // Can only make a struct blittable if all of its fields are exported
            if (numFields != numExportedFields)
            {
                typeInfo.BlittableKind = ManagedUnrealBlittableKind.NotBlittable;
            }

            ValidateNoUnrealExposedProperties(typeInfo, type);
        }

        private void ProcessProperties(ManagedUnrealTypeInfo typeInfo, Type type)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            foreach (PropertyInfo property in type.GetProperties(bindingFlags))
            {
                ManagedUnrealPropertyInfo propertyInfo = CreateProperty(property);
                if (propertyInfo != null)
                {
                    // force members to be 'blueprint read only' if in a const class
                    if (typeInfo.ClassFlags.HasFlag(EClassFlags.Const))
                    {
                        propertyInfo.Flags |= EPropertyFlags.BlueprintReadOnly;
                    }

                    propertyInfo.Name = GetUniqueMemberName(typeInfo, propertyInfo, property);
                    propertyInfo.Path = typeInfo.Path + ":" + propertyInfo.Name;
                    typeInfo.Properties.Add(propertyInfo);

                    ValidateProperty(typeInfo, type, propertyInfo, property.PropertyType, property);
                }
            }

            ValidateNoUnrealExposedFields(typeInfo, type);
        }

        private bool IsPropertyExposedToBlueprint(ManagedUnrealPropertyInfo propertyInfo)
        {
            // Checking if the type has ANY of these flags
            EPropertyFlags flags = EPropertyFlags.BlueprintVisible | EPropertyFlags.BlueprintAssignable |
                EPropertyFlags.BlueprintCallable;
            return (propertyInfo.Flags & flags) != 0;
        }

        private bool IsTypeExposedToBlueprint(ManagedUnrealTypeInfo typeInfo)
        {
            // Checking if the type has ANY of these flags
            ManagedUnrealTypeInfoFlags flags =
                ManagedUnrealTypeInfoFlags.BlueprintableHierarchical |
                ManagedUnrealTypeInfoFlags.BlueprintTypeHierarchical;
            return (typeInfo.AdditionalFlags & flags) != 0;
        }

        private bool IsFunctionExposedToBlueprint(ManagedUnrealFunctionInfo functionInfo)
        {
            // Checking if the function has ANY of these flags
            EFunctionFlags flags = EFunctionFlags.BlueprintCallable | EFunctionFlags.BlueprintEvent;
            return (functionInfo.Flags & flags) != 0;
        }

        private ManagedUnrealPropertyInfo CreateProperty(Type type)
        {
            ManagedUnrealPropertyInfo propertyInfo = new ManagedUnrealPropertyInfo();

            if (type.IsByRef && type.HasElementType)
            {
                type = type.GetElementType();
            }

            EPropertyType typeCode = ManagedUnrealTypeInfo.GetTypeCode(type);

            ManagedUnrealTypeInfoReference propertyTypeInfo = null;
            switch (typeCode)
            {
                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:

                case EPropertyType.SoftClass:
                case EPropertyType.SoftObject:
                case EPropertyType.WeakObject:
                case EPropertyType.LazyObject:
                case EPropertyType.Class:

                case EPropertyType.Object:
                case EPropertyType.Enum:
                case EPropertyType.Interface:
                case EPropertyType.Struct:
                    string typePath = GetTypePath(type, typeCode);
                    if (!string.IsNullOrEmpty(typePath))
                    {
                        propertyTypeInfo = new ManagedUnrealTypeInfoReference(typeCode, typePath);
                    }
                    break;

                case EPropertyType.InternalNativeFixedSizeArray:
                case EPropertyType.InternalManagedFixedSizeArray:
                    propertyTypeInfo = new ManagedUnrealTypeInfoReference(typeCode);
                    if (type.IsArray)
                    {
                        CreateGenericArgsTypeRefs(propertyInfo, type.GetElementType());
                    }
                    else
                    {
                        CreateGenericArgsTypeRefs(propertyInfo, type.GenericTypeArguments[0]);
                    }
                    break;

                case EPropertyType.Array:
                    propertyTypeInfo = new ManagedUnrealTypeInfoReference(typeCode);
                    CreateGenericArgsTypeRefs(propertyInfo, type.GenericTypeArguments[0]);
                    break;
                case EPropertyType.Set:
                    propertyTypeInfo = new ManagedUnrealTypeInfoReference(typeCode);
                    CreateGenericArgsTypeRefs(propertyInfo, type.GenericTypeArguments[0]);
                    if (!ManagedUnrealTypeInfo.HasGetTypeHash(propertyInfo.GenericArgs[0].TypeCode))
                    {
                        return null;
                    }
                    break;
                case EPropertyType.Map:
                    {
                        propertyTypeInfo = new ManagedUnrealTypeInfoReference(typeCode);
                        CreateGenericArgsTypeRefs(propertyInfo, type.GenericTypeArguments[0], type.GenericTypeArguments[1]);
                        if (!ManagedUnrealTypeInfo.HasGetTypeHash(propertyInfo.GenericArgs[0].TypeCode))
                        {
                            return null;
                        }
                    }
                    break;

                case EPropertyType.Bool:
                case EPropertyType.Int8:
                case EPropertyType.Int16:
                case EPropertyType.Int:
                case EPropertyType.Int64:
                case EPropertyType.Byte:
                case EPropertyType.UInt16:
                case EPropertyType.UInt32:
                case EPropertyType.UInt64:
                case EPropertyType.Double:
                case EPropertyType.Float:
                case EPropertyType.Name:
                case EPropertyType.Text:
                case EPropertyType.Str:
                    propertyTypeInfo = new ManagedUnrealTypeInfoReference(typeCode);
                    break;

                default:
                    // Unhandled type code
                    return null;
            }

            if (propertyTypeInfo == null)
            {
                return null;
            }

            foreach (ManagedUnrealTypeInfoReference arg in propertyInfo.GenericArgs)
            {
                if (arg.TypeCode == EPropertyType.Unknown)
                {
                    return null;
                }
            }

            propertyInfo.Type = propertyTypeInfo;
            return propertyInfo;
        }

        private void CreateGenericArgsTypeRefs(ManagedUnrealPropertyInfo propertyInfo, params Type[] typeArgs)
        {
            propertyInfo.GenericArgs.AddRange(CreateGenericArgsTypeRefs(typeArgs));
        }

        private ManagedUnrealTypeInfoReference[] CreateGenericArgsTypeRefs(params Type[] typeArgs)
        {
            ManagedUnrealTypeInfoReference[] result = new ManagedUnrealTypeInfoReference[typeArgs.Length];
            for (int i = 0; i < typeArgs.Length; i++)
            {
                Type type = typeArgs[i];
                ManagedUnrealTypeInfoReference typeRef = new ManagedUnrealTypeInfoReference();
                typeRef.TypeCode = ManagedUnrealTypeInfo.GetTypeCode(type);
                typeRef.Path = GetTypePath(type, typeRef.TypeCode);
                result[i] = typeRef;
            }
            return result;
        }

        private ManagedUnrealPropertyInfo CreateProperty(MethodInfo method, ParameterInfo parameter)
        {
            ManagedUnrealPropertyInfo propertyInfo = CreateProperty(parameter.ParameterType);
            if (propertyInfo == null)
            {
                return null;
            }

            if (parameter.ParameterType.IsArray)
            {
                // TODO: Possible to handle arrays as an alternative for TArray parameters?
                return null;
            }

            if (parameter.IsOut)
            {
                propertyInfo.IsOut = true;
            }
            else if (parameter.ParameterType.IsByRef)
            {
                propertyInfo.IsByRef = true;
            }

            var attributes = parameter.GetCustomAttributes<ManagedUnrealAttributeBase>(false);
            if (attributes != null)
            {
                foreach (ManagedUnrealAttributeBase attribute in attributes)
                {
                    // Change this to ProcessParam / ProcessReturnProp ?
                    attribute.ProcessProperty(propertyInfo);
                    if (attribute.InvalidTarget)
                    {
                        if (attribute.InvalidTargetReason != null)
                        {
                            throw new InvalidManagedUnrealAttributeException(method, parameter, attribute);
                        }
                        return null;
                    }
                }
            }

            return propertyInfo;
        }

        private ManagedUnrealPropertyInfo CreateProperty(MemberInfo member)
        {
            Type type = null;

            PropertyInfo property = member as PropertyInfo;
            if (property != null)
            {
                type = property.PropertyType;
            }

            FieldInfo field = member as FieldInfo;
            if (field != null)
            {
                type = field.FieldType;
            }

            if (type == null)
            {
                return null;
            }

            ManagedUnrealPropertyInfo propertyInfo = CreateProperty(type);
            if (propertyInfo == null)
            {
                return null;
            }

            if (property != null)
            {
                MethodInfo propMethod = property.GetMethod != null ? property.GetMethod : property.SetMethod;
                if (propMethod.IsPublic)
                {
                    propertyInfo.IsPublic = true;
                }
                else if (propMethod.IsPrivate)
                {
                    propertyInfo.IsPrivate = true;
                }
                else
                {
                    // Should be protected (method.IsFamily)
                    propertyInfo.IsProtected = true;
                }
            }
            else if (field != null)
            {
                if (field.IsPublic)
                {
                    propertyInfo.IsPublic = true;
                }
                else if (field.IsPrivate)
                {
                    propertyInfo.IsPrivate = true;
                }
                else
                {
                    // Should be protected (field.IsFamily)
                    propertyInfo.IsProtected = true;
                }
            }
            
            var attributes = member.GetCustomAttributes<ManagedUnrealAttributeBase>(false);
            if (attributes != null)
            {
                foreach (ManagedUnrealAttributeBase attribute in attributes)
                {
                    attribute.ProcessProperty(propertyInfo);
                    if (attribute.InvalidTarget)
                    {
                        if (attribute.InvalidTargetReason != null)
                        {
                            throw new InvalidManagedUnrealAttributeException(member, attribute);
                        }
                        return null;
                    }
                }
            }

            if (propertyInfo.AdditionalFlags.HasFlag(ManagedUnrealPropertyFlags.BlueprintGetter) &&
                !propertyInfo.AdditionalFlags.HasFlag(ManagedUnrealPropertyFlags.BlueprintSetter))
            {
                // If we saw a BlueprintGetter but did not see BlueprintSetter or 
                // or BlueprintReadWrite then treat as BlueprintReadOnly
                propertyInfo.Flags |= EPropertyFlags.BlueprintReadOnly;
            }

            // Make sure this property meets the requirements to be exposed
            switch (ManagedUnrealVisibility.PropertyRequirement)
            {
                case ManagedUnrealVisibility.Requirement.MainAttribute:
                    if (!propertyInfo.AdditionalFlags.HasFlag(ManagedUnrealPropertyFlags.UProperty))
                    {
                        return null;
                    }
                    break;
                case ManagedUnrealVisibility.Requirement.AnyAttribute:
                    if (!member.HasCustomAttribute<ManagedUnrealAttributeBase>(false) &&
                        !member.HasCustomAttribute<UMetaAttribute>(false))
                    {
                        return null;
                    }
                    break;
            }

            if (ManagedUnrealVisibility.Members != ManagedUnrealVisibility.Member.None)
            {
                if ((propertyInfo.Flags & (EPropertyFlags.BlueprintVisible | EPropertyFlags.BlueprintReadOnly)) == 0)
                {
                    if (ManagedUnrealVisibility.Members.HasFlag(ManagedUnrealVisibility.Member.BlueprintVisible))
                    {
                        propertyInfo.Flags |= EPropertyFlags.BlueprintVisible;
                    }
                    else if (ManagedUnrealVisibility.Members.HasFlag(ManagedUnrealVisibility.Member.BlueprintVisibleReadOnly))
                    {
                        propertyInfo.Flags |= EPropertyFlags.BlueprintReadOnly;
                    }
                }
                if ((propertyInfo.Flags & (EPropertyFlags.Edit)) == 0)
                {
                    if (ManagedUnrealVisibility.Members.HasFlag(ManagedUnrealVisibility.Member.EditorVisible))
                    {
                        propertyInfo.Flags |= EPropertyFlags.Edit;
                    }
                    else if (ManagedUnrealVisibility.Members.HasFlag(ManagedUnrealVisibility.Member.EditorVisibleReadOnly))
                    {
                        propertyInfo.Flags |= EPropertyFlags.Edit | EPropertyFlags.EditConst;
                    }
                }
            }

            return propertyInfo;
        }

        private bool IsMethodCompilerGenerated(MethodInfo method)
        {
            return method.GetCustomAttribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>(false) != null;
        }

        private void ProcessFunctions(ManagedUnrealTypeInfo typeInfo, Type type)
        {
            if (!typeInfo.IsInterface)
            {
                MethodInfo initializer = type.GetMethod("Initialize", new Type[] { typeof(FObjectInitializer) });
                if (initializer != null)
                {
                    if (initializer.DeclaringType == type)
                    {
                        typeInfo.OverridesObjectInitializer = true;
                        typeInfo.OverridesObjectInitializerHierarchical = true;
                    }
                    else if (initializer.DeclaringType != typeof(UObject))
                    {
                        typeInfo.OverridesObjectInitializerHierarchical = true;
                    }
                }
            }

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            BindingFlags baseBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (MethodInfo method in type.GetMethods(bindingFlags))
            {
                if (method.IsSpecialName)
                {
                    continue;
                }

                if (method.Name.EndsWith(codeSettings.VarNames.RPCValidate) &&
                    type.GetMethod(method.Name.Substring(0,
                    method.Name.Length - codeSettings.VarNames.RPCValidate.Length), baseBindingFlags) != null)
                {
                    // Skip net validation methods
                    continue;
                }

                if (!codeSettings.UseExplicitImplementationMethods &&
                    method.Name.EndsWith(codeSettings.VarNames.ImplementationMethod))
                {
                    MethodInfo originalMethod = type.GetMethod(method.Name.Substring(0,
                        method.Name.Length - codeSettings.VarNames.ImplementationMethod.Length), baseBindingFlags);

                    if (originalMethod != null)
                    {
                        // Skip "_Implementation" methods when not using them explicitly
                        continue;
                    }
                }

                ManagedUnrealFunctionInfo functionInfo = CreateFunction(typeInfo, type, method);
                if (functionInfo != null)
                {
                    ValidateFunction(typeInfo, type, functionInfo, method);

                    if (functionInfo.IsImplementation && !functionInfo.IsOverride)
                    {
                        // This is the base "_Implementation" method. It is easier to deal with only one C# method per 
                        // target UFunction method for the assembly rewriter. Therefore skip it here and process it
                        // when processing the definition method.

                        // This check essentially means method names ending in "_Implementation" are reserved
                        Debug.Assert(codeSettings.UseExplicitImplementationMethods);

                        continue;
                    }

                    functionInfo.Name = GetUniqueMemberName(typeInfo, functionInfo, method);
                    functionInfo.Path = typeInfo.Path + ":" + functionInfo.Name;
                    typeInfo.Functions.Add(functionInfo);
                }
            }
        }

        /// <summary>
        /// Returns the method defined in a interface which the given method is implementing (or null if not applicable)
        /// (this is talking about regular interface implementation functions, not "_Implementation" functions).
        /// </summary>
        private MethodInfo GetBaseInterfaceMethod(MethodInfo method)
        {
            Type type = method.DeclaringType;
            Dictionary<MethodInfo, MethodInfo> functionMap;
            if (!cachedImplementedInterfacesFunctionMap.TryGetValue(type, out functionMap))
            {
                List<Type> implementedInterfaces = GetImplementedInterfaces(type);
                if (implementedInterfaces.Count > 0)
                {
                    functionMap = new Dictionary<MethodInfo, MethodInfo>();

                    foreach (Type implementedInterface in implementedInterfaces)
                    {
                        InterfaceMapping map = type.GetInterfaceMap(implementedInterface);
                        for (int i = 0; i < map.InterfaceMethods.Length; i++)
                        {
                            functionMap[map.TargetMethods[i]] = map.InterfaceMethods[i];
                        }
                    }

                    cachedImplementedInterfacesFunctionMap.Add(type, functionMap);
                }
                else
                {
                    return method;
                }
            }

            MethodInfo result;
            if (functionMap.TryGetValue(method, out result))
            {
                return result;
            }
            return method;
        }

        /// <summary>
        /// Returns the method non-_Implementation/non-_Validate method for the given method (or returns itself).
        /// </summary>
        private MethodInfo GetOriginalMethodDefinition(MethodInfo method)
        {
            // Only consider getting the original method if this is a base method definition
            if (method.GetBaseDefinition() == method)
            {
                string originalMethodName = null;
                if (method.Name.EndsWith(codeSettings.VarNames.ImplementationMethod))
                {
                    originalMethodName = method.Name.Substring(0,
                        method.Name.Length - codeSettings.VarNames.ImplementationMethod.Length);
                }
                else if (method.Name.EndsWith(codeSettings.VarNames.RPCValidate))
                {
                    originalMethodName = method.Name.Substring(0,
                        method.Name.Length - codeSettings.VarNames.RPCValidate.Length);
                }
                if (!string.IsNullOrEmpty(originalMethodName))
                {
                    // Find the first function which has a UFunction attribute with the given method name
                    foreach (MethodInfo baseTypeMethod in method.DeclaringType.GetMethods(
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                    {
                        if (baseTypeMethod.Name == originalMethodName)
                        {
                            return baseTypeMethod;
                        }
                    }
                }
            }
            return method;
        }

        /// <summary>
        /// Gets the function flags. This assumes ProcessFunction will only set flags and nothing else.
        /// </summary>
        private bool TryGetFunctionFlags(ManagedUnrealFunctionInfo functionInfo, MethodInfo method, out CachedFunctionFlagInfo outFlags)
        {
            if (cachedFunctionFlags.TryGetValue(method, out outFlags))
            {
                return true;
            }

            MethodInfo originalMethod = GetOriginalMethodDefinition(method);

            if (originalMethod != method &&
                cachedFunctionFlags.TryGetValue(originalMethod, out outFlags))
            {
                cachedFunctionFlags.Add(method, outFlags);
                return true;
            }

            if (functionInfo == null)
            {
                functionInfo = cachedDummyFunctionInfo;
                functionInfo.Flags = 0;
                functionInfo.AdditionalFlags = 0;
            }

            var attributes = originalMethod.GetCustomAttributes<ManagedUnrealAttributeBase>(false);
            if (attributes != null)
            {
                foreach (ManagedUnrealAttributeBase attribute in attributes)
                {
                    attribute.ProcessFunction(functionInfo);
                    attribute.ProcessFunctionParams(functionInfo);
                    if (attribute.InvalidTarget)
                    {
                        if (attribute.InvalidTargetReason != null)
                        {
                            throw new InvalidManagedUnrealAttributeException(originalMethod, attribute);
                        }
                        outFlags = null;
                        return false;
                    }
                }
            }

            EFunctionFlags flags = functionInfo.Flags;
            ManagedUnrealFunctionFlags additionalFlags = functionInfo.AdditionalFlags;
            string originalName = functionInfo.OriginalName;

            // Get inherited base function flags
            MethodInfo baseMethod = method.GetBaseDefinition();
            if (baseMethod == method)
            {
                baseMethod = GetBaseInterfaceMethod(method);
            }
            if (baseMethod != method)
            {
                // More works needs to be done to support complex situations where we can have "new" modifiers
                // on functions / properties as well as defining flags in parent functions. For now inherit all
                // flags from the base-most method and selectively include flags defined in the current method.

                CachedFunctionFlagInfo baseFlags;
                if (TryGetFunctionFlags(null, baseMethod, out baseFlags))
                {
                    originalName = baseFlags.OriginalName;

                    additionalFlags = (baseFlags.AdditionalFlags & ManagedUnrealFunctionFlags.FuncInherit);
                    if (baseMethod.DeclaringType.IsInterface)
                    {
                        additionalFlags |= ManagedUnrealFunctionFlags.InterfaceImplementation;
                    }

                    // Allow SealedEvent to be set so that we can at least make the function sealed / final if needed.
                    bool isSealedEvent = flags.HasFlag(EFunctionFlags.Final);
                    flags |= baseFlags.Flags & EFunctionFlags.FuncInherit;
                    if (isSealedEvent)
                    {
                        flags |= EFunctionFlags.Final;
                    }
                }
            }
            else
            {
                if (originalMethod.IsPublic)
                {
                    flags |= EFunctionFlags.Public;
                }
                else if (originalMethod.IsPrivate)
                {
                    flags |= EFunctionFlags.Private;
                }
                else
                {
                    // Should be protected (method.IsFamily)
                    flags |= EFunctionFlags.Protected;
                }
            }

            if (ManagedUnrealVisibility.Members.HasFlag(ManagedUnrealVisibility.Member.BlueprintCallable))
            {
                flags |= EFunctionFlags.BlueprintCallable;
            }

            outFlags = new CachedFunctionFlagInfo(flags, additionalFlags, originalName);
            cachedFunctionFlags.Add(originalMethod, outFlags);
            if (method != originalMethod)
            {
                cachedFunctionFlags.Add(method, outFlags);
            }
            return true;
        }

        private void SetBlueprintability(ManagedUnrealTypeInfo typeInfo, Type type)
        {
            Blueprintability bpState = GetBlueprintability(type, typeInfo.TypeCode);
            if (bpState.HasFlag(Blueprintability.BlueprintType))
            {
                typeInfo.AdditionalFlags |= ManagedUnrealTypeInfoFlags.BlueprintTypeHierarchical;
            }
            if (bpState.HasFlag(Blueprintability.ManagedTypeBlueprintType))
            {
                typeInfo.AdditionalFlags |= ManagedUnrealTypeInfoFlags.BlueprintTypeStateManaged;
            }
            if (bpState.HasFlag(Blueprintability.Blueprintable))
            {
                typeInfo.AdditionalFlags |= ManagedUnrealTypeInfoFlags.BlueprintableHierarchical;
            }
            if (bpState.HasFlag(Blueprintability.ManagedTypeBlueprintable))
            {
                typeInfo.AdditionalFlags |= ManagedUnrealTypeInfoFlags.BlueprintableStateManaged;
            }
        }

        private Blueprintability GetBlueprintability(Type type, EPropertyType typeCode)
        {
            // Also see CodeGenerator.StructExporter.cs GetBlueprintability()

            const Blueprintability blueprintTypeMask = 
                Blueprintability.BlueprintType | Blueprintability.NotBlueprintType | Blueprintability.ManagedTypeBlueprintType;
            const Blueprintability blueprintableMask = 
                Blueprintability.Blueprintable | Blueprintability.NotBlueprintable | Blueprintability.ManagedTypeBlueprintable;

            Blueprintability state;
            if (cachedBlueprintability.TryGetValue(type, out state))
            {
                return state;
            }

            state = Blueprintability.None;

            if (typeCode == EPropertyType.Object || typeCode == EPropertyType.Interface)
            {
                if (type.IsSameOrSubclassOf(typeof(UBlueprintFunctionLibrary)))
                {
                    // UBlueprintFunctionLibrary functions are always visible (even if marked NotBlueprintType)
                    state = Blueprintability.BlueprintType;
                }
            }

            // TODO: Get actual UMeta attributes, this will miss anything manually tagged with UMeta.
            if ((state & blueprintTypeMask) == 0)
            {
                bool blueprintSpawnable = type.HasCustomAttribute<BlueprintSpawnableComponent>(false);
                bool blueprintType = type.HasCustomAttribute<BlueprintTypeAttribute>(false);
                bool notBlueprintType = type.HasCustomAttribute<NotBlueprintTypeAttribute>(false);
                if (blueprintSpawnable || blueprintType)
                {
                    state |= Blueprintability.BlueprintType;
                    if (!UnrealTypes.IsNativeUnrealType(type))
                    {
                        state |= Blueprintability.ManagedTypeBlueprintType;
                    }
                }
                else if (notBlueprintType)
                {
                    state |= Blueprintability.NotBlueprintType;
                    if (!UnrealTypes.IsNativeUnrealType(type))
                    {
                        state |= Blueprintability.ManagedTypeBlueprintType;
                    }
                }
            }
            if ((state & blueprintableMask) == 0)
            {
                bool blueprintable = type.HasCustomAttribute<BlueprintableAttribute>(false);
                bool notBlueprintable = type.HasCustomAttribute<NotBlueprintableAttribute>(false);
                if (blueprintable)
                {
                    state |= Blueprintability.Blueprintable;
                    if (!UnrealTypes.IsNativeUnrealType(type))
                    {
                        state |= Blueprintability.ManagedTypeBlueprintable;
                    }
                }
                else if (notBlueprintable)
                {
                    state |= Blueprintability.NotBlueprintable;
                    if (!UnrealTypes.IsNativeUnrealType(type))
                    {
                        state |= Blueprintability.ManagedTypeBlueprintable;
                    }
                }
            }

            if (typeCode == EPropertyType.Object || typeCode == EPropertyType.Interface)
            {
                // Don't inherit the state from UObject
                if (type.BaseType != null && type.BaseType != typeof(UObject))
                {
                    Blueprintability baseState = GetBlueprintability(type.BaseType, EPropertyType.Object);
                    if ((state & blueprintTypeMask) == 0)
                    {
                        state |= (baseState & blueprintTypeMask);
                    }
                    if ((state & blueprintableMask) == 0)
                    {
                        state |= (baseState & blueprintableMask);
                    }
                }
                foreach (Type interfaceType in type.GetInterfaces())
                {
                    Blueprintability interfaceState = GetBlueprintability(interfaceType, EPropertyType.Interface);
                    if ((state & blueprintTypeMask) == 0)
                    {
                        state |= (interfaceState & blueprintTypeMask);
                    }
                    if ((state & blueprintableMask) == 0)
                    {
                        state |= (interfaceState & blueprintableMask);
                    }
                }
            }

            // Fall-back to the default visibility if it isn't defined by on a managed type explicitly
            ManagedUnrealVisibility.Type defaultTypeVisibility = ManagedUnrealVisibility.Type.None;
            switch (typeCode)
            {
                case EPropertyType.Object:
                    defaultTypeVisibility = ManagedUnrealVisibility.Class;
                    break;
                case EPropertyType.Interface:
                    defaultTypeVisibility = ManagedUnrealVisibility.Interface;
                    break;
                case EPropertyType.Struct:
                    defaultTypeVisibility = ManagedUnrealVisibility.Struct;
                    break;
                case EPropertyType.Enum:
                    defaultTypeVisibility = ManagedUnrealVisibility.Enum;
                    break;
            }
            if (defaultTypeVisibility.HasFlag(ManagedUnrealVisibility.Type.BlueprintType) &&
                !state.HasFlag(Blueprintability.ManagedTypeBlueprintType) &&
                !UnrealTypes.IsNativeUnrealType(type))
            {
                state &= ~blueprintTypeMask;
                state |= Blueprintability.BlueprintType | Blueprintability.ManagedTypeBlueprintType;
            }
            if (defaultTypeVisibility.HasFlag(ManagedUnrealVisibility.Type.Blueprintable) &&
                !state.HasFlag(Blueprintability.ManagedTypeBlueprintable) &&
                !UnrealTypes.IsNativeUnrealType(type))
            {
                state &= ~blueprintableMask;
                state |= Blueprintability.Blueprintable | Blueprintability.ManagedTypeBlueprintable;
            }

            cachedBlueprintability.Add(type, state);
            return state;
        }

        private bool TryGetClassFlags(ManagedUnrealTypeInfo typeInfo, Type type, bool lateResolve,
            out KeyValuePair<EClassFlags, ManagedUnrealTypeInfoFlags> outFlags, out string classConfigName)
        {
            const EClassFlags inheritFlags = EClassFlags.ScriptInherit;// Inherit or ScriptInherit?
            const string inheritConfig = "inherit";// Use parents config name

            EClassFlags flags = 0;
            ManagedUnrealTypeInfoFlags additionalFlags = 0;
            bool changed = false;

            if (cachedClassFlags.TryGetValue(type, out outFlags))
            {
                flags = outFlags.Key;
                additionalFlags = outFlags.Value;
                cachedClassConfigName.TryGetValue(type, out classConfigName);
            }
            else
            {
                changed = true;
                if (typeInfo == null)
                {
                    typeInfo = cachedDummyClassInfo;
                    typeInfo.Flags = 0;
                    typeInfo.AdditionalFlags = 0;
                    typeInfo.ClassConfigName = null;
                }
                if (typeInfo.IsInterface)
                {
                    typeInfo.ClassFlags |= EClassFlags.Interface | EClassFlags.Abstract;
                }
                
                var attributes = type.GetCustomAttributes<ManagedUnrealAttributeBase>(false);
                if (attributes != null)
                {
                    foreach (ManagedUnrealAttributeBase attribute in attributes)
                    {
                        if (typeInfo.IsInterface)
                        {
                            attribute.ProcessInterface(typeInfo);
                        }
                        else
                        {
                            attribute.ProcessClass(typeInfo);
                        }
                        if (attribute.InvalidTarget)
                        {
                            if (attribute.InvalidTargetReason != null)
                            {
                                throw new InvalidManagedUnrealAttributeException(type, attribute);
                            }
                            outFlags = new KeyValuePair<EClassFlags, ManagedUnrealTypeInfoFlags>(0, 0);
                            classConfigName = null;
                            return false;
                        }
                    }
                }

                flags = typeInfo.ClassFlags;
                additionalFlags = typeInfo.AdditionalFlags;
                classConfigName = typeInfo.ClassConfigName;
                Blueprintability bpState = GetBlueprintability(type, typeInfo.TypeCode);
                if (bpState.HasFlag(Blueprintability.BlueprintType))
                {
                    additionalFlags |= ManagedUnrealTypeInfoFlags.BlueprintTypeHierarchical;
                }
                if (bpState.HasFlag(Blueprintability.Blueprintable))
                {
                    additionalFlags |= ManagedUnrealTypeInfoFlags.BlueprintableHierarchical;
                }

                // Predefined flags should be the fully defined flags for the given class / interface
                bool hasPredefinedFlags = false;
                
                UUnrealTypePathAttribute pathAttribute = UnrealTypes.GetPathAttribute(type);
                if (pathAttribute != null && !string.IsNullOrEmpty(pathAttribute.Path))
                {
                    UClassAttribute classAttribute = type.GetCustomAttribute<UClassAttribute>(false);
                    if (classAttribute != null)
                    {
                        flags = (EClassFlags)classAttribute.Flags;
                        additionalFlags |= ManagedUnrealTypeInfoFlags.UClass |
                            ManagedUnrealTypeInfoFlags.HasLateResolvedClassFlags;
                        hasPredefinedFlags = true;
                    }

                    UInterfaceAttribute interfaceAttribute = type.GetCustomAttribute<UInterfaceAttribute>(false);
                    if (interfaceAttribute != null)
                    {
                        flags = (EClassFlags)interfaceAttribute.Flags;
                        additionalFlags |= ManagedUnrealTypeInfoFlags.UInterface |
                            ManagedUnrealTypeInfoFlags.HasLateResolvedClassFlags;
                        hasPredefinedFlags = true;
                    }

                    // Assuming the casing of this always stays the same
                    if (pathAttribute.Path == "/Script/Engine.Actor")
                    {
                        additionalFlags |= ManagedUnrealTypeInfoFlags.Actor;
                    }
                }

                if (!hasPredefinedFlags)
                {
                    if (type.BaseType != null && type.BaseType != typeof(object))
                    {
                        KeyValuePair<EClassFlags, ManagedUnrealTypeInfoFlags> baseFlags;
                        string baseClassConfig;
                        if (TryGetClassFlags(null, type.BaseType, lateResolve, out baseFlags, out baseClassConfig))
                        {
                            flags |= baseFlags.Key & inheritFlags;
                            if (!string.IsNullOrEmpty(baseClassConfig) &&
                                (string.IsNullOrEmpty(classConfigName) || classConfigName.Equals(inheritConfig, StringComparison.CurrentCultureIgnoreCase)))
                            {
                                classConfigName = baseClassConfig;
                            }
                            if (baseFlags.Value.HasFlag(ManagedUnrealTypeInfoFlags.Actor))
                            {
                                additionalFlags |= ManagedUnrealTypeInfoFlags.Actor;
                            }
                        }
                    }
                    foreach (Type interfaceType in type.GetInterfaces())
                    {
                        if (interfaceType.GetCustomAttribute<UInterfaceAttribute>(false) != null)
                        {
                            additionalFlags |= ManagedUnrealTypeInfoFlags.ImplementsInterface;

                            KeyValuePair<EClassFlags, ManagedUnrealTypeInfoFlags> interfaceFlags;
                            string interfaceConfig;// Can an interface have a config? It has to properties to config.
                            if (TryGetClassFlags(null, interfaceType, lateResolve, out interfaceFlags, out interfaceConfig))
                            {
                                flags |= interfaceFlags.Key & inheritFlags;
                                if (!string.IsNullOrEmpty(interfaceConfig) &&
                                    (string.IsNullOrEmpty(classConfigName) || classConfigName.Equals(inheritConfig, StringComparison.CurrentCultureIgnoreCase)))
                                {
                                    classConfigName = interfaceConfig;
                                }
                            }
                        }
                    }
                }
            }

            if (lateResolve && !additionalFlags.HasFlag(ManagedUnrealTypeInfoFlags.HasLateResolvedClassFlags))
            {
                // Not resolving interfaces here. They shouldn't contain any additional information.
                changed = true;

                KeyValuePair<EClassFlags, ManagedUnrealTypeInfoFlags> baseFlags;
                string baseClassConfig;
                if (type.BaseType != null && type.BaseType != typeof(object) &&
                    TryGetClassFlags(null, type.BaseType, lateResolve, out baseFlags, out baseClassConfig))
                {
                    flags |= baseFlags.Key & inheritFlags;
                    if (!string.IsNullOrEmpty(baseClassConfig) &&
                        (string.IsNullOrEmpty(classConfigName) || classConfigName.Equals(inheritConfig, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        classConfigName = baseClassConfig;
                    }
                }

                if (typeInfo != null && !flags.HasFlag(EClassFlags.Config))
                {
                    foreach (ManagedUnrealPropertyInfo propertyInfo in typeInfo.Properties)
                    {
                        if (propertyInfo.Flags.HasFlag(EPropertyFlags.Config))
                        {
                            flags |= EClassFlags.Config;
                        }
                        if ((propertyInfo.Flags &
                            (EPropertyFlags.ContainsInstancedReference | EPropertyFlags.InstancedReference)) != 0)
                        {
                            flags |= EClassFlags.HasInstancedReference;
                        }
                    }
                }

                // Class needs to specify which ini file is going to be used if it contains config variables.
                if (flags.HasFlag(EClassFlags.Config) && string.IsNullOrEmpty(classConfigName))
                {
                    classConfigName = "Engine"; // NAME_Engine is just "Engine"?
                    throw new ValidateUnrealClassFailedException(type,
                        "Classes with config / globalconfig member variables need to specify config file.");
                }

                additionalFlags |= ManagedUnrealTypeInfoFlags.HasLateResolvedClassFlags;
            }

            outFlags = new KeyValuePair<EClassFlags, ManagedUnrealTypeInfoFlags>(flags, additionalFlags);
            if (changed)
            {
                cachedClassFlags[type] = outFlags;
                cachedClassConfigName[type] = classConfigName;
            }

            return true;
        }

        private ManagedUnrealFunctionInfo CreateFunction(ManagedUnrealTypeInfo typeInfo, Type type, MethodInfo method)
        {
            if (method.GetCustomAttribute<UFunctionIgnoreAttribute>(false) != null)
            {
                // Early exit if we don't want this to be a UFunction
                return null;
            }

            if (method.IsGenericMethod)
            {
                // Generics aren't supported
                return null;
            }

            ManagedUnrealFunctionInfo functionInfo = new ManagedUnrealFunctionInfo();

            functionInfo.IsStatic = method.IsStatic;
            functionInfo.IsOverride = method.GetBaseDefinition() != method;

            // What is the CORRECT way to do this? Sort of hacked this until it worked for all methods
            // including implemented non-virtual interface functions.
            functionInfo.IsVirtual = method.IsVirtual && 
                (!method.IsFinal) && !method.IsAbstract &&
                !functionInfo.IsOverride;// Use either "virtual" or "override", not both

            functionInfo.IsImplementation = method.Name.EndsWith(codeSettings.VarNames.ImplementationMethod);

            // Get the cached function flags which may have already been obtained. This will resolve
            // the function flags to include the base function flags.
            CachedFunctionFlagInfo flags;
            if (!TryGetFunctionFlags(functionInfo, method, out flags))
            {
                return null;
            }
            functionInfo.Flags |= flags.Flags;
            functionInfo.AdditionalFlags |= flags.AdditionalFlags;
            functionInfo.OriginalName = flags.OriginalName;

            // non-static functions in a const class must be const themselves
            if (!functionInfo.IsStatic && typeInfo.ClassFlags.HasFlag(EClassFlags.Const))
            {
                functionInfo.Flags |= EFunctionFlags.Const;
            }

            if (!typeInfo.IsDelegate)
            {
                // Make sure this function meets the requirements to be exposed
                switch (ManagedUnrealVisibility.FunctionRequirement)
                {
                    case ManagedUnrealVisibility.Requirement.MainAttribute:
                        if (!functionInfo.AdditionalFlags.HasFlag(ManagedUnrealFunctionFlags.UFunction))
                        {
                            return null;
                        }
                        break;
                    case ManagedUnrealVisibility.Requirement.AnyAttribute:
                        if (!functionInfo.AdditionalFlags.HasFlag(ManagedUnrealFunctionFlags.UFunction) &&
                            !method.HasCustomAttribute<ManagedUnrealAttributeBase>(false) &&
                            !method.HasCustomAttribute<UMetaAttribute>(false))
                        {
                            return null;
                        }
                        break;
                }
            }

            if (functionInfo.IsOverride && !functionInfo.IsBlueprintEvent)
            {
                // Override methods which don't have any contribution to the reflection system should be skipped
                // - The base method should suffice for the function to be visible and exporting the override
                //   function wouldn't provide any useful information. The desired logic of this may change in the
                //   future but be careful with the rewriting of the IL for the UFunction/invoker if doing so.
                return null;
            }

            if (method.ReturnType != typeof(void))
            {
                functionInfo.ReturnProp = CreateProperty(method.ReturnType);
                if (functionInfo.ReturnProp == null)
                {
                    throw new InvalidUnrealFunctionReturnTypeException(method, method.ReturnType);
                }
                functionInfo.Flags |= EFunctionFlags.HasOutParms;
                functionInfo.ReturnProp.IsFunctionReturnValue = true;
                functionInfo.ReturnProp.Name = UFunction.ReturnValuePropName;

                ValidateFunctionParam(typeInfo, type, functionInfo.ReturnProp, method.ReturnType,
                    method.ReturnParameter, functionInfo, method);
            }

            foreach (ParameterInfo parameter in method.GetParameters())
            {
                ManagedUnrealPropertyInfo paramInfo = CreateProperty(method, parameter);
                if (paramInfo == null)
                {
                    throw new InvalidUnrealFunctionParamTypeException(method, parameter);
                }
                if (paramInfo.Flags.HasFlag(EPropertyFlags.OutParm))
                {
                    functionInfo.Flags |= EFunctionFlags.HasOutParms;
                }
                paramInfo.IsFunctionParam = true;
                paramInfo.Name = parameter.Name;
                functionInfo.Params.Add(paramInfo);

                ValidateFunctionParam(typeInfo, type, paramInfo, parameter.ParameterType, parameter, functionInfo, method);
            }

            if (codeSettings.UseExplicitImplementationMethods && !functionInfo.IsImplementation)
            {
                if ((functionInfo.IsBlueprintEvent || functionInfo.IsRPC) && functionInfo.IsVirtual && !typeInfo.IsInterface)
                {
                    throw new InvalidUnrealFunctionException(method, "BlueprintEvent/RPC method definitions shouldn't be virtual when using explicit " +
                        codeSettings.VarNames.ImplementationMethod + " methods");
                }

                if (functionInfo.IsBlueprintEvent && !functionInfo.IsOverride)
                {
                    Debug.Assert(typeInfo.IsInterface || !functionInfo.IsVirtual);
                    // This function isn't actually virtual but for the sake of stating that this is the base 
                    // BlueprintEvent method tag it as virtual (this is done because the base-most _Implementation
                    // method is skipped to ensure only 1 method is processed per definition). Though this means that
                    // our type info is our of sync with the C# type info.
                    functionInfo.IsVirtual = true;
                }
            }

            return functionInfo;
        }

        private ManagedUnrealBlittableKind GetBlittableKind(ManagedUnrealTypeInfoReference typeRef)
        {
            switch (typeRef.TypeCode)
            {
                // Need some way of knowing the size of bool. There is are fixed size bools in Stack.h
                // - Check how they are used on UBoolProperty (CPT_Bool, CPT_Bool8, CPT_Bool16, CPT_Bool32, CPT_Bool64)
                // - It is possible that this is script VM-only so it knows how to read bools on the stack for functions
                // - Engine\Source\Runtime\CoreUObject\Public\UObject\Stack.h EPropertyType
                //case EPropertyType.Bool:

                case EPropertyType.Int8:
                case EPropertyType.Int16:
                case EPropertyType.Int:
                case EPropertyType.Int64:
                case EPropertyType.Byte:
                case EPropertyType.UInt16:
                case EPropertyType.UInt32:
                case EPropertyType.UInt64:
                case EPropertyType.Double:
                case EPropertyType.Float:
                case EPropertyType.Name:
                    return ManagedUnrealBlittableKind.Blittable;
                case EPropertyType.Object:
                    return codeSettings.UObjectAsBlittableType ?
                        ManagedUnrealBlittableKind.Blittable : ManagedUnrealBlittableKind.NotBlittable;
                case EPropertyType.Struct:
                    ManagedUnrealTypeInfo structInfo = ManagedUnrealTypeInfo.FindTypeInfoByPath(typeRef.Path);
                    if (structInfo == null)
                    {
                        if (AllKnownBlittableTypes.ContainsKey(typeRef.Path))
                        {
                            return ManagedUnrealBlittableKind.Blittable;
                        }
                        else if (AllKnownNonBlittableTypes.ContainsKey(typeRef.Path))
                        {
                            return ManagedUnrealBlittableKind.NotBlittable;
                        }
                        else
                        {
                            throw new ManagedUnrealTypeInfoException("How did we obtain typeRef if we can't find it now?");
                        }
                    }
                    else
                    {
                        return structInfo.BlittableKind;
                    }
                default:
                    return ManagedUnrealBlittableKind.NotBlittable;
            }
        }

        public ManagedUnrealTypeInfo FindType(Type type)
        {
            return FindType(type, EPropertyType.Unknown);
        }

        public ManagedUnrealTypeInfo FindType(Type type, EPropertyType typeCode)
        {
            ManagedUnrealTypeInfo typeInfo;
            if (TypeInfosByType.TryGetValue(type, out typeInfo))
            {
                if (typeInfo != null && (typeInfo.TypeCode == typeCode || typeCode == EPropertyType.Unknown))
                {
                    return typeInfo;
                }
            }
            else
            {
                ManagedUnrealModuleInfo module = FindModule(type);
                if (module != null && module != this)
                {
                    return module.FindType(type, typeCode);
                }
            }
            return null;
        }

        public ManagedUnrealTypeInfo FindStruct(Type type)
        {
            return FindType(type, EPropertyType.Struct);
        }

        public ManagedUnrealTypeInfo FindClass(Type type)
        {
            return FindType(type, EPropertyType.Object);
        }

        public ManagedUnrealEnumInfo FindEnum(Type type)
        {
            return FindType(type, EPropertyType.Enum) as ManagedUnrealEnumInfo;
        }

        public static ManagedUnrealModuleInfo FindModule(Type type)
        {
            ManagedUnrealModuleInfo module;
            ModulesByType.TryGetValue(type, out module);
            return module;
        }

        /// <summary>
        /// Defines the "blueprintability" (BlueprintType / Blueprintable) for a given type.
        /// This is passed through the hierarchy until both values are determined.
        /// </summary>
        [Flags]
        enum Blueprintability
        {
            None = 0x00000000,
            Blueprintable = 0x00000001,
            BlueprintType = 0x00000002,
            NotBlueprintable = 0x0000004,
            NotBlueprintType = 0x0000008,

            /// <summary>
            /// A type defined in C# defined the (Not)Blueprintable state
            /// </summary>
            ManagedTypeBlueprintable = 0x0000010,
            /// <summary>
            /// A type defined in C# defined the (Not)BlueprintType state
            /// </summary>
            ManagedTypeBlueprintType = 0x0000020,
        }

        class CachedFunctionFlagInfo
        {
            public EFunctionFlags Flags;
            public ManagedUnrealFunctionFlags AdditionalFlags;
            public string OriginalName;

            public CachedFunctionFlagInfo(EFunctionFlags flags, ManagedUnrealFunctionFlags additionalFlags, string originalName)
            {
                Flags = flags;
                AdditionalFlags = additionalFlags;
                OriginalName = originalName;
            }
        }
    }

    public partial class ManagedUnrealTypeInfo : ManagedUnrealReflectionBase
    {
        /// <summary>
        /// The fully qualified name (managed type name)
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The type code for this type (valid values: Object, Struct, Enum, Interface, Delegate, MulticastDelegate)
        /// </summary>
        public EPropertyType TypeCode { get; set; }

        /// <summary>
        /// Which Name.ini file to load Config variables out of
        /// (this maps directly to UClass.ClassConfigName)
        /// </summary>
        public string ClassConfigName { get; set; }

        /// <summary>
        /// Flags shared between ClassFlags and StructFlags
        /// </summary>
        public uint Flags { get; set; }
        [ManagedUnrealReflectIgnore]
        public EClassFlags ClassFlags
        {
            get {  return (EClassFlags)Flags; }
            set { Flags = (uint)value; }
        }
        [ManagedUnrealReflectIgnore]
        public EStructFlags StructFlags
        {
            get { return (EStructFlags)Flags; }
            set { Flags = (uint)value; }
        }

        public ManagedUnrealTypeInfoFlags AdditionalFlags { get; set; }

        /// <summary>
        /// Used to determine if this type is blittable.
        /// </summary>
        public ManagedUnrealBlittableKind BlittableKind { get; set; }

        /// <summary>
        /// Used to determine if this type is blittable. This is mostly used for structs but also states if a UObject type
        /// is blittable if the setting to use blittable UObjects is enabled.
        /// </summary>
        public bool IsBlittable
        {
            get { return BlittableKind == ManagedUnrealBlittableKind.Blittable; }
        }

        /// <summary>
        /// States if a struct is being treated as a managed class (inherits from StructAsClass)
        /// </summary>
        [ManagedUnrealReflectIgnore]
        public bool IsStructAsClass
        {
            get { return AdditionalFlags.HasFlag(ManagedUnrealTypeInfoFlags.StructAsClass); }
            set { SetFlag(ManagedUnrealTypeInfoFlags.StructAsClass, value); }
        }
        /// <summary>
        /// Initialize() is overridden which is used as the managed equivalent of the C++ constructor taking in an FObjectInitializer
        /// </summary>
        [ManagedUnrealReflectIgnore]
        public bool OverridesObjectInitializer
        {
            get { return AdditionalFlags.HasFlag(ManagedUnrealTypeInfoFlags.OverridesObjectInitializer); }
            set { SetFlag(ManagedUnrealTypeInfoFlags.OverridesObjectInitializer, value); }
        }
        /// <summary>
        /// Initialize() is overridden somewhere in the class hierarchy
        /// </summary>
        [ManagedUnrealReflectIgnore]
        public bool OverridesObjectInitializerHierarchical
        {
            get { return AdditionalFlags.HasFlag(ManagedUnrealTypeInfoFlags.OverridesObjectInitializerHierarchical); }
            set { SetFlag(ManagedUnrealTypeInfoFlags.OverridesObjectInitializerHierarchical, value); }
        }
        public bool IsStruct
        {
            get { return TypeCode == EPropertyType.Struct; }
        }
        public bool IsClass
        {
            get { return TypeCode == EPropertyType.Object; }
        }
        public bool IsInterface
        {
            get { return TypeCode == EPropertyType.Interface; }
        }

        public List<ManagedUnrealPropertyInfo> Properties { get; set; }
        public List<ManagedUnrealFunctionInfo> Functions { get; set; }
        public List<ManagedUnrealTypeInfoReference> BaseTypes { get; set; }

        public bool IsDelegate
        {
            get { return TypeCode == EPropertyType.Delegate || TypeCode == EPropertyType.MulticastDelegate; }
        }        

        public ManagedUnrealTypeInfo()
        {
            Properties = new List<ManagedUnrealPropertyInfo>();
            Functions = new List<ManagedUnrealFunctionInfo>();
            BaseTypes = new List<ManagedUnrealTypeInfoReference>();
        }

        private void SetFlag(EClassFlags flag, bool set)
        {
            if (ClassFlags.HasFlag(flag) != set)
            {
                ClassFlags ^= flag;
            }
        }

        private void SetFlag(ManagedUnrealTypeInfoFlags flag, bool set)
        {
            if (AdditionalFlags.HasFlag(flag) != set)
            {
                AdditionalFlags ^= flag;
            }
        }

        //private Dictionary<string, ManagedUnrealPropertyInfo> cachedPropertiesByName;
        //private Dictionary<string, ManagedUnrealFunctionInfo> cachedFunctionsByName;
        //private Dictionary<string, ManagedUnrealTypeInfoReference> cachedBaseTypesByPath;
        //
        //public Dictionary<string, ManagedUnrealPropertyInfo> GetPropertiesByName()
        //{
        //    if (cachedPropertiesByName != null && cachedPropertiesByName.Count == Properties.Count)
        //    {
        //        return cachedPropertiesByName;
        //    }
        //
        //    cachedPropertiesByName = new Dictionary<string, ManagedUnrealPropertyInfo>();
        //    foreach (ManagedUnrealPropertyInfo propertyInfo in Properties)
        //    {
        //        cachedPropertiesByName.Add(propertyInfo.Name, propertyInfo);
        //    }
        //    return cachedPropertiesByName;
        //}
        //
        //public Dictionary<string, ManagedUnrealFunctionInfo> GetFunctionsByName()
        //{
        //    if (cachedFunctionsByName != null && cachedFunctionsByName.Count == Functions.Count)
        //    {
        //        return cachedFunctionsByName;
        //    }
        //
        //    cachedFunctionsByName = new Dictionary<string, ManagedUnrealFunctionInfo>();
        //    foreach (ManagedUnrealFunctionInfo functionInfo in Functions)
        //    {
        //        cachedFunctionsByName.Add(functionInfo.Name, functionInfo);
        //    }
        //    return cachedFunctionsByName;
        //}
        //
        //public Dictionary<string, ManagedUnrealTypeInfoReference> GetBaseTypesByPath()
        //{
        //    if (cachedBaseTypesByPath != null && cachedBaseTypesByPath.Count == BaseTypes.Count)
        //    {
        //        return cachedBaseTypesByPath;
        //    }
        //
        //    cachedBaseTypesByPath = new Dictionary<string, ManagedUnrealTypeInfoReference>();
        //    foreach (ManagedUnrealTypeInfoReference baseTypeInfo in BaseTypes)
        //    {
        //        cachedBaseTypesByPath.Add(baseTypeInfo.Path, baseTypeInfo);
        //    }
        //    return cachedBaseTypesByPath;
        //}
    }

    public partial class ManagedUnrealEnumInfo : ManagedUnrealTypeInfo
    {
        public List<ManagedUnrealEnumValueInfo> EnumValues { get; set; }

        public ManagedUnrealEnumInfo()
        {
            EnumValues = new List<ManagedUnrealEnumValueInfo>();
        }
    }
    
    public partial class ManagedUnrealEnumValueInfo : ManagedUnrealReflectionBase
    {
        public ulong Value { get; set; }
    }
    
    public partial class ManagedUnrealPropertyInfo : ManagedUnrealReflectionBase
    {
        public ManagedUnrealTypeInfoReference Type { get; set; }
        public List<ManagedUnrealTypeInfoReference> GenericArgs { get; set; }

        /// <summary>
        /// Defines the size to be a fixed size array (must be greater than 1 to be an array)
        /// </summary>
        public int FixedSizeArrayDim { get; set; }

        /// <summary>
        /// The name of the function to be notified when this property is sent over the network 
        /// (due to this property changing in value).
        /// </summary>
        public string RepNotifyName { get; set; }

        public EPropertyFlags Flags { get; set; }

        public ManagedUnrealPropertyFlags AdditionalFlags { get; set; }

        // Hijacking the native access specifier flags, does this have any side effects?
        // - These access specifier values seem to be rarely used in native code (mostly code gen related)
        [ManagedUnrealReflectIgnore]
        public bool IsPublic
        {
            get { return Flags.HasFlag(EPropertyFlags.NativeAccessSpecifierPublic); }
            set { SetFlag(EPropertyFlags.NativeAccessSpecifierPublic, value); }
        }
        [ManagedUnrealReflectIgnore]
        public bool IsPrivate
        {
            get { return Flags.HasFlag(EPropertyFlags.NativeAccessSpecifierPrivate); }
            set { SetFlag(EPropertyFlags.NativeAccessSpecifierPrivate, value); }
        }
        [ManagedUnrealReflectIgnore]
        public bool IsProtected
        {
            get { return Flags.HasFlag(EPropertyFlags.NativeAccessSpecifierProtected); }
            set { SetFlag(EPropertyFlags.NativeAccessSpecifierProtected, value); }
        }

        [ManagedUnrealReflectIgnore]
        public bool IsField
        {
            get { return AdditionalFlags.HasFlag(ManagedUnrealPropertyFlags.Field); }
            set { SetFlag(ManagedUnrealPropertyFlags.Field, value); }
        }
        [ManagedUnrealReflectIgnore]
        public bool IsBackingFieldPreStripped
        {
            get { return AdditionalFlags.HasFlag(ManagedUnrealPropertyFlags.BackingFieldPreStripped); }
            set { SetFlag(ManagedUnrealPropertyFlags.BackingFieldPreStripped, value); }
        }
        [ManagedUnrealReflectIgnore]
        public bool IsFunctionParam
        {
            get { return AdditionalFlags.HasFlag(ManagedUnrealPropertyFlags.FunctionParam); }
            set
            {
                if (value)
                {
                    Flags |= EPropertyFlags.Parm;
                }
                else
                {
                    Flags &= ~EPropertyFlags.Parm;
                }
                SetFlag(ManagedUnrealPropertyFlags.FunctionParam, value);
            }
        }
        [ManagedUnrealReflectIgnore]
        public bool IsFunctionReturnValue
        {
            get { return AdditionalFlags.HasFlag(ManagedUnrealPropertyFlags.FunctionReturnValue); }
            set
            {
                if (value)
                {
                    Flags |= EPropertyFlags.Parm | EPropertyFlags.OutParm | EPropertyFlags.ReturnParm;
                }
                else
                {
                    Flags &= ~(EPropertyFlags.Parm | EPropertyFlags.OutParm | EPropertyFlags.ReturnParm);
                }
                SetFlag(ManagedUnrealPropertyFlags.FunctionReturnValue, value);
            }
        }
        [ManagedUnrealReflectIgnore]
        public bool IsByRef
        {
            get { return AdditionalFlags.HasFlag(ManagedUnrealPropertyFlags.ByRefParam); }
            set
            {
                if (value)
                {
                    Flags |= EPropertyFlags.OutParm | EPropertyFlags.ReferenceParm;
                }
                else
                {
                    Flags &= ~(EPropertyFlags.OutParm | EPropertyFlags.ReferenceParm);
                }
                SetFlag(ManagedUnrealPropertyFlags.ByRefParam, value);
            }
        }
        [ManagedUnrealReflectIgnore]
        public bool IsOut
        {
            get { return AdditionalFlags.HasFlag(ManagedUnrealPropertyFlags.OutParam); }
            set
            {
                if (value)
                {
                    Flags |= EPropertyFlags.OutParm;
                    Flags &= ~EPropertyFlags.ReferenceParm;// Just in case it was set
                }
                else
                {
                    Flags &= ~EPropertyFlags.OutParm;
                }
                SetFlag(ManagedUnrealPropertyFlags.OutParam, value);
            }
        }

        public bool IsFixedSizeArray
        {
            get
            {
                return Type == null ? false :
                    Type.TypeCode == EPropertyType.InternalNativeFixedSizeArray ||
                    Type.TypeCode == EPropertyType.InternalManagedFixedSizeArray;
            }
        }

        public bool IsCollection
        {
            get { return Type == null ? false : ManagedUnrealTypeInfo.IsCollectionType(Type.TypeCode); }
        }

        public bool IsDelegate
        {
            get { return Type == null ? false : ManagedUnrealTypeInfo.IsDelegateType(Type.TypeCode); }
        }

        public ManagedUnrealPropertyInfo()
        {
            GenericArgs = new List<ManagedUnrealTypeInfoReference>();
        }

        private void SetFlag(EPropertyFlags flag, bool set)
        {
            if (Flags.HasFlag(flag) != set)
            {
                Flags ^= flag;
            }
        }

        private void SetFlag(ManagedUnrealPropertyFlags flag, bool set)
        {
            if (AdditionalFlags.HasFlag(flag) != set)
            {
                AdditionalFlags ^= flag;
            }
        }
    }

    public class ManagedUnrealTypeInfoReference
    {
        public EPropertyType TypeCode { get; set; }
        public string Path { get; set; }

        public ManagedUnrealTypeInfoReference()
        {
        }

        public ManagedUnrealTypeInfoReference(EPropertyType typeCode)
            : this(typeCode, null)
        {
        }

        public ManagedUnrealTypeInfoReference(EPropertyType typeCode, string path)
        {
            TypeCode = typeCode;
            Path = path;
        }
    }

    /// <summary>
    /// Additional type flags used for working with classes / structs / delegates
    /// </summary>
    [Flags]
    public enum ManagedUnrealTypeInfoFlags
    {
        None = 0,

        /// <summary>
        /// This is a struct type but it is being treated as a managed class which inherits from StructAsClass
        /// </summary>
        StructAsClass = 0x00000001,

        /// <summary>
        /// This is a blueprint type (determined by metadata attribute, obtained hierarchically)
        /// (this is mostly used for blueprint exposed type validation)
        /// </summary>
        BlueprintTypeHierarchical = 0x00000002,

        /// <summary>
        /// This is a blueprintable type (determined by metadata attribute, obtained hierarchically)
        /// (this is mostly used for blueprint exposed type validation)
        /// </summary>
        BlueprintableHierarchical = 0x00000004,

        /// <summary>
        /// The BlueprintType state was defined by a type defined in managed code
        /// (this flag can be set with BlueprintTypeHierarchical being not set if it is NotBlueprintType)
        /// </summary>
        BlueprintTypeStateManaged = 0x00000008,

        /// <summary>
        /// The Blueprintable state was defined by a type defined in managed code
        /// (this flag can be set with BlueprintableHierarchical being not set if it is NotBlueprintable)
        /// </summary>
        BlueprintableStateManaged = 0x00000010,

        /// <summary>
        /// This type inherits from AActor (this is used for EClassFlags.EditInlineNew validation)
        /// </summary>
        Actor = 0x00000020,

        /// <summary>
        /// Only used when processing classes whilst creating module info. States that late resolved class
        /// flags have been obtained and don't require an additional lookup.
        /// </summary>
        HasLateResolvedClassFlags = 0x00000040,

        /// <summary>
        /// Tagged with [UStruct] attribute
        /// </summary>
        UStruct = 0x00000080,

        /// <summary>
        /// Tagged with [UClass] attribute
        /// </summary>
        UClass = 0x00000100,

        /// <summary>
        /// Tagged with [UInterface] attribute
        /// </summary>
        UInterface = 0x00000200,

        /// <summary>
        /// Tagged with [UDelegate] attribute
        /// </summary>
        UDelegate = 0x00000400,

        /// <summary>
        /// Tagged with [UEnum] attribute
        /// </summary>
        UEnum = 0x00000800,

        /// <summary>
        /// This is a class / interface which implements one or more interfaces
        /// </summary>
        ImplementsInterface = 0x00001000,

        /// <summary>
        /// Initialize() is overridden which is used as the managed equivalent of the C++ constructor taking in an FObjectInitializer
        /// </summary>
        OverridesObjectInitializer = 0x00002000,

        /// <summary>
        /// Initialize() is overridden somewhere in the class hierarchy
        /// </summary>
        OverridesObjectInitializerHierarchical = 0x00004000,
    }

    /// <summary>
    /// Additional property flags used for working with property info
    /// </summary>
    [Flags]
    public enum ManagedUnrealPropertyFlags
    {
        None = 0,

        /// <summary>
        /// Tagged with [UProperty] attribute
        /// </summary>
        UProperty = 0x00000001,

        /// <summary>
        /// This is a field defined in a struct
        /// </summary>
        Field = 0x00000002,

        /// <summary>
        /// Indicates that the C# property backing field has already been stripped
        /// </summary>
        BackingFieldPreStripped = 0x00000004,

        /// <summary>
        /// This is a parameter for a function
        /// </summary>
        FunctionParam = 0x00000008,

        /// <summary>
        /// This is the return result for a function
        /// </summary>
        FunctionReturnValue = 0x00000010,

        /// <summary>
        /// This is a "ref" parameter for a function (C# "ref" keyword)
        /// </summary>
        ByRefParam = 0x00000020,

        /// <summary>
        /// This is an "out" parameter for a function (C# "out" keyword)
        /// </summary>
        OutParam = 0x00000040,

        /// <summary>
        /// This has an associated getter method
        /// </summary>
        BlueprintGetter = 0x00000080,

        /// <summary>
        /// This has an associated setter method
        /// </summary>
        BlueprintSetter = 0x00000100
    }

    /// <summary>
    /// Additional function flags used for working with managed function info
    /// </summary>
    [Flags]
    public enum ManagedUnrealFunctionFlags
    {
        None = 0,

        /// <summary>
        /// Tagged with [UFunction] attribute
        /// </summary>
        UFunction = 0x00000001,

        /// <summary>
        /// This function is virtual (mutually exclusive with Override)
        /// </summary>
        Virtual = 0x00000002,

        /// <summary>
        /// This function is virtual (mutually exclusive with Virtual)
        /// </summary>
        Override = 0x00000004,

        /// <summary>
        /// This is the equivalent of BlueprintImplementableEvent which states the implementation is defined in Blueprint
        /// </summary>
        BlueprintImplemented = 0x00000008,

        /// <summary>
        /// States that this method is an "_Implementation" method (this is used as the suffix may be trimmed to remove the name)
        /// </summary>
        Implementation = 0x00000010,

        /// <summary>
        /// States that this function is a Blueprint getter for a property
        /// </summary>
        BlueprintGetter = 0x00000020,

        /// <summary>
        /// States that this function is a Blueprint setter for a property
        /// </summary>
        BlueprintSetter = 0x00000040,

        /// <summary>
        /// This function is an implementation function for an implemented interface on the owning class.
        /// (this is talking about regular interface implementation functions, not "_Implementation" functions).
        /// </summary>
        InterfaceImplementation = 0x00000080,

        /// <summary>
        /// The flags applicable to inherit from the base function flags
        /// </summary>
        FuncInherit = 0
    }

    public partial class ManagedUnrealFunctionInfo : ManagedUnrealReflectionBase
    {
        public ManagedUnrealPropertyInfo ReturnProp { get; set; }
        public List<ManagedUnrealPropertyInfo> Params { get; set; }

        public ManagedUnrealFunctionFlags AdditionalFlags { get; set; }
        [ManagedUnrealReflectIgnore]
        public bool IsVirtual
        {
            get { return AdditionalFlags.HasFlag(ManagedUnrealFunctionFlags.Virtual); }
            set { SetFlag(ManagedUnrealFunctionFlags.Virtual, value); }
        }
        [ManagedUnrealReflectIgnore]
        public bool IsOverride
        {
            get { return AdditionalFlags.HasFlag(ManagedUnrealFunctionFlags.Override); }
            set { SetFlag(ManagedUnrealFunctionFlags.Override, value); }
        }
        [ManagedUnrealReflectIgnore]
        public bool IsBlueprintImplemented
        {
            get { return AdditionalFlags.HasFlag(ManagedUnrealFunctionFlags.BlueprintImplemented); }
            set { SetFlag(ManagedUnrealFunctionFlags.BlueprintImplemented, value); }
        }
        [ManagedUnrealReflectIgnore]
        public bool IsImplementation
        {
            get { return AdditionalFlags.HasFlag(ManagedUnrealFunctionFlags.Implementation); }
            set { SetFlag(ManagedUnrealFunctionFlags.Implementation, value); }
        }

        public EFunctionFlags Flags { get; set; }
        [ManagedUnrealReflectIgnore]
        public bool IsStatic
        {
            get { return Flags.HasFlag(EFunctionFlags.Static); }
            set { SetFlag(EFunctionFlags.Static, value); }
        }
        [ManagedUnrealReflectIgnore]
        public bool IsBlueprintEvent
        {
            get { return Flags.HasFlag(EFunctionFlags.BlueprintEvent); }
            set { SetFlag(EFunctionFlags.BlueprintEvent, value); }
        }
        [ManagedUnrealReflectIgnore]
        public bool IsRPC
        {
            get { return Flags.HasFlag(EFunctionFlags.Net); }
            set { SetFlag(EFunctionFlags.Net, value); }
        }
        [ManagedUnrealReflectIgnore]
        public bool WithValidation
        {
            get { return Flags.HasFlag(EFunctionFlags.NetValidate); }
            set { SetFlag(EFunctionFlags.NetValidate, value); }
        }

        /// <summary>
        /// The original name of the function. Used for function lookup on virtual/interface functions.
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        /// Returns Name or OriginalName (if it exists).
        /// 
        /// NOTE: Make sure this is called in ManagedUnrealTypes.Builder.cs instead of using Name directly.
        /// </summary>
        public string GetName()
        {
            if (!string.IsNullOrEmpty(OriginalName))
            {
                return OriginalName;
            }
            return Name;
        }

        private void SetFlag(EFunctionFlags flag, bool set)
        {
            if (Flags.HasFlag(flag) != set)
            {
                Flags ^= flag;
            }
        }

        private void SetFlag(ManagedUnrealFunctionFlags flag, bool set)
        {
            if (AdditionalFlags.HasFlag(flag) != set)
            {
                AdditionalFlags ^= flag;
            }
        }

        public ManagedUnrealFunctionInfo()
        {
            Params = new List<ManagedUnrealPropertyInfo>();
        }
    }
    
    public abstract partial class ManagedUnrealReflectionBase
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Hash { get; set; }

        public void CreateHash()
        {
            string serialized = Serialize(this);
            using (SHA256 sha = SHA256.Create())
            {
                byte[] buffer = sha.ComputeHash(Encoding.UTF8.GetBytes(serialized));
                StringBuilder hash = new StringBuilder(buffer.Length * 2);
                for (int i = 0; i < buffer.Length; i++)
                {
                    hash.Append(buffer[i].ToString("X2"));
                }
                Hash = hash.ToString();
            }

            foreach (PropertyInfo property in GetType().GetProperties())
            {
                object propertyValue = property.GetValue(this);

                System.Collections.IList list = propertyValue as System.Collections.IList;
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ManagedUnrealReflectionBase element = list[i] as ManagedUnrealReflectionBase;
                        if (element != null)
                        {
                            element.CreateHash();
                        }
                    }
                }

                System.Collections.IDictionary dictionary = propertyValue as System.Collections.IDictionary;
                if (dictionary != null)
                {
                    foreach (object key in dictionary.Keys)
                    {
                        ManagedUnrealReflectionBase element = key as ManagedUnrealReflectionBase;
                        if (element != null)
                        {
                            element.CreateHash();
                        }
                    }

                    foreach (object value in dictionary.Values)
                    {
                        ManagedUnrealReflectionBase element = value as ManagedUnrealReflectionBase;
                        if (element != null)
                        {
                            element.CreateHash();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Helper for marshaler lookups based on type
    /// </summary>
    public struct ManagedUnrealMarshalerInfo
    {
        public ManagedUnrealMarshalerType MarshalerType;
        
        public EPropertyType Type;
        public string Path;

        public EPropertyType Arg1Type;
        public string Arg1Path;

        public EPropertyType Arg2Type;
        public string Arg2Path;

        public ManagedUnrealMarshalerInfo(ManagedUnrealPropertyInfo propertyInfo, ManagedUnrealMarshalerType marshalerType)
        {
            MarshalerType = marshalerType;
            
            Type = propertyInfo.Type.TypeCode;
            Path = propertyInfo.Type.Path;

            Arg1Type = EPropertyType.Unknown;
            Arg1Path = null;

            Arg2Type = EPropertyType.Unknown;
            Arg2Path = null;

            if (propertyInfo.GenericArgs != null)
            {
                if (propertyInfo.GenericArgs.Count >= 1)
                {
                    Arg1Type = propertyInfo.GenericArgs[0].TypeCode;
                    Arg1Path = propertyInfo.GenericArgs[0].Path;
                }

                if (propertyInfo.GenericArgs.Count >= 2)
                {
                    Arg2Type = propertyInfo.GenericArgs[1].TypeCode;
                    Arg2Path = propertyInfo.GenericArgs[1].Path;
                }

                if (propertyInfo.GenericArgs.Count >= 3)
                {
                    throw new NotImplementedException("Handle more than 2 generic args if Unreal has a type requiring it");
                }
            }
        }

        public ManagedUnrealMarshalerInfo(EPropertyType typeCode, string typePath, ManagedUnrealMarshalerType marshalerType)
        {
            MarshalerType = marshalerType;
            
            Type = typeCode;
            Path = typePath;            

            Arg1Type = EPropertyType.Unknown;
            Arg1Path = null;

            Arg2Type = EPropertyType.Unknown;
            Arg2Path = null;
        }

        public ManagedUnrealMarshalerInfo(EPropertyType typeCode, string typePath,
            EPropertyType arg1TypeCode, string arg1TypePath,
            EPropertyType arg2TypeCode, string arg2TypePath,
            ManagedUnrealMarshalerType marshalerType)
        {
            MarshalerType = marshalerType;
            
            Type = typeCode;
            Path = typePath;

            Arg1Type = arg1TypeCode;
            Arg1Path = arg1TypePath;

            Arg2Type = arg2TypeCode;
            Arg2Path = arg2TypePath;
        }
    }

    public enum ManagedUnrealMarshalerType
    {
        Default,
        ReadOnly,
        Copy
    }

    public enum ManagedUnrealBlittableKind
    {
        /// <summary>
        /// Not blittable. Requires regular marshaling.
        /// </summary>
        NotBlittable,

        /// <summary>
        /// Blittable. Can use primitive operations to copy the data.
        /// </summary>
        Blittable,

        /// <summary>
        /// Forcefully blittable. Determined by a custom struct attribute.
        /// This will use primitive operations to copy the data but could potentially cause memory corrption.
        /// </summary>
        ForceBlittable,

        /// <summary>
        /// Marshaling kind cannot be determined due to unresolved types. Check again after types have resolved.
        /// </summary>
        Unresolved
    }

    public class ManagedUnrealReflectIgnoreAttribute : Attribute
    {
    }

    public interface ISerializedManagedUnrealModuleInfo
    {
        string GetString();
    }
}
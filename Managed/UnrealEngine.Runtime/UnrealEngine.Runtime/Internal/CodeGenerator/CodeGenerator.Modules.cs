using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public partial class CodeGenerator
    {
        // Lookup for the type of module <ModuleName, EUnrealModuleType> (used for getting the namespace name)
        private Dictionary<FName, UnrealModuleType> modulesByName = new Dictionary<FName, UnrealModuleType>();

        // Cache of known namespaces by package
        private Dictionary<UPackage, CachedNamespace> namespaceCache = new Dictionary<UPackage, CachedNamespace>();

        private void UpdateModulesByName()
        {
            modulesByName.Clear();

            Dictionary<FName, string> modulePaths = FModulesPaths.FindModulePaths("*");
            foreach (KeyValuePair<FName, string> modulePath in modulePaths)
            {
                modulesByName[modulePath.Key] = UnrealModuleInfo.GetModuleType(modulePath.Value);
            }
        }

        public void GenerateCodeForAllModules()
        {            
            GenerateCodeForModules(new UnrealModuleType[]
                {
                    UnrealModuleType.Game,// Do C++ classes appear here? Or under asset registry? TODO: Look into
                    UnrealModuleType.GamePlugin,
                    UnrealModuleType.Engine,
                    UnrealModuleType.EnginePlugin
                });            
        }

        public void GenerateCodeForModules(UnrealModuleType[] moduleTypes)
        {
            BeginGenerateModules();

            Dictionary<UPackage, List<UStruct>> structsByPackage = new Dictionary<UPackage, List<UStruct>>();
            Dictionary<UPackage, List<UEnum>> enumsByPackage = new Dictionary<UPackage, List<UEnum>>();
            Dictionary<UPackage, List<UFunction>> globalFunctionsByPackage = new Dictionary<UPackage, List<UFunction>>();

            foreach (UStruct unrealStruct in new TObjectIterator<UStruct>())
            {
                if (!unrealStruct.IsA<UFunction>() && CanExportStruct(unrealStruct) && IsAvailableType(unrealStruct))
                {
                    UPackage package = unrealStruct.GetOutermost();
                    if (package != null)
                    {
                        List<UStruct> structs;
                        if (!structsByPackage.TryGetValue(package, out structs))
                        {
                            structsByPackage.Add(package, structs = new List<UStruct>());
                        }
                        structs.Add(unrealStruct);
                    }
                }
            }

            foreach (UEnum unrealEnum in new TObjectIterator<UEnum>())
            {
                if (CanExportEnum(unrealEnum) && IsAvailableType(unrealEnum))
                {
                    UPackage package = unrealEnum.GetOutermost();
                    if (package != null)
                    {
                        List<UEnum> enums;
                        if (!enumsByPackage.TryGetValue(package, out enums))
                        {
                            enumsByPackage.Add(package, enums = new List<UEnum>());
                        }
                        enums.Add(unrealEnum);
                    }
                }
            }

            UClass packageClass = UClass.GetClass<UPackage>();
            foreach (UFunction function in new TObjectIterator<UFunction>())
            {
                UObject outer = function.GetOuter();
                if (outer == null || outer.GetClass() != packageClass)
                {
                    continue;
                }

                if (function.HasAnyFunctionFlags(EFunctionFlags.Delegate | EFunctionFlags.MulticastDelegate))
                {
                    UPackage package = function.GetOutermost();
                    if (package != null)
                    {
                        List<UFunction> functions;
                        if (!globalFunctionsByPackage.TryGetValue(package, out functions))
                        {
                            globalFunctionsByPackage.Add(package, functions = new List<UFunction>());
                        }
                        functions.Add(function);
                    }
                }
                else
                {
                    FMessage.Log(ELogVerbosity.Error, string.Format("Global function which isn't a delegate '{0}'", function.GetName()));
                }
            }

            Dictionary<FName, string> modulePaths = FModulesPaths.FindModulePaths("*");

            // Make sure ModulePaths and ModuleNames are the same. Based on comments FindModules may be changed/removed in the
            // future so we will want to just use FindModulePaths. For now make sure they are synced up.
            FName[] moduleNames = FModuleManager.Get().FindModules("*");
            if (moduleNames.Length != modulePaths.Count)
            {
                FMessage.Log(ELogVerbosity.Warning, string.Format("Module count invalid. FindModules:{0} FindModulePaths:{1}",
                    moduleNames.Length, modulePaths.Count));
            }

            foreach (KeyValuePair<FName, string> modulePath in modulePaths)
            {
                string longPackageName = FPackageName.ConvertToLongScriptPackageName(modulePath.Key.PlainName);
                UPackage package = UObject.FindObjectFast<UPackage>(null, new FName(longPackageName), false, false);
                if (package != null)
                {
                    UnrealModuleInfo moduleInfo = new UnrealModuleInfo(package, modulePath.Key.PlainName, modulePath.Value);

                    if (moduleInfo.Type == UnrealModuleType.Unknown)
                    {
                        FMessage.Log(ELogVerbosity.Error, string.Format("Unknown module type on module '{0}' '{1}'",
                            moduleInfo.Name, moduleInfo.Package));
                    }
                    else if (!moduleTypes.Contains(moduleInfo.Type))
                    {
                        continue;
                    }

                    List<UStruct> structs;
                    if (!structsByPackage.TryGetValue(package, out structs))
                    {
                        structs = new List<UStruct>();
                    }

                    List<UEnum> enums;
                    if (!enumsByPackage.TryGetValue(package, out enums))
                    {
                        enums = new List<UEnum>();
                    }

                    List<UFunction> globalFunctions;
                    if (!globalFunctionsByPackage.TryGetValue(package, out globalFunctions))
                    {
                        globalFunctions = new List<UFunction>();
                    }

                    GenerateCodeForModule(moduleInfo, structs.ToArray(), enums.ToArray(), globalFunctions.ToArray());
                }
            }

            EndGenerateModules();
        }

        public void GenerateCodeForModule(string moduleName, bool loadModule)
        {
            string longPackageName = FPackageName.ConvertToLongScriptPackageName(moduleName);
            UPackage package = UObject.FindObjectFast<UPackage>(null, new FName(longPackageName), false, false);

            if (package == null && !FModuleManager.Instance.IsModuleLoaded(new FName(moduleName)))
            {
                // package is almost always non-null even when IsModuleLoaded returns false - if the package is non-null it
                // seems the module types are available so we don't have to call LoadModule (check if this is always true)

                if (!loadModule)
                {
                    return;
                }

                EModuleLoadResult loadResult;
                FModuleManager.Instance.LoadModuleWithFailureReason(new FName(moduleName), out loadResult);
                if (loadResult != EModuleLoadResult.Success)
                {
                    FMessage.Log("Failed to load module '" + moduleName + "'. Reason: " + loadResult);
                    return;
                }

                package = UObject.FindObjectFast<UPackage>(null, new FName(longPackageName), false, false);
            }

            if (package != null)
            {
                Dictionary<FName, string> modulePaths = FModulesPaths.FindModulePaths("*");
                string modulePath;
                if (!modulePaths.TryGetValue(new FName(moduleName), out modulePath))
                {
                    return;
                }

                BeginGenerateModules();
                GenerateCodeForModule(new UnrealModuleInfo(package, moduleName, modulePath));
                EndGenerateModules();
            }
        }

        private void GenerateCodeForModule(UnrealModuleInfo module)
        {
            List<UStruct> structs = new List<UStruct>();
            List<UEnum> enums = new List<UEnum>();
            List<UFunction> globalFunctions = new List<UFunction>();

            foreach (UStruct unrealStruct in new TObjectIterator<UStruct>())
            {
                if (!unrealStruct.IsA<UFunction>() && unrealStruct.IsIn(module.Package) &&
                    CanExportStruct(unrealStruct) && IsAvailableType(unrealStruct))
                {
                    structs.Add(unrealStruct);
                }
            }

            foreach (UEnum unrealEnum in new TObjectIterator<UEnum>())
            {
                if (unrealEnum.IsIn(module.Package) && CanExportEnum(unrealEnum) && IsAvailableType(unrealEnum))
                {
                    enums.Add(unrealEnum);
                }
            }

            UClass packageClass = UClass.GetClass<UPackage>();
            foreach (UFunction function in new TObjectIterator<UFunction>())
            {
                UObject outer = function.GetOuter();
                if (outer == null || outer.GetClass() != packageClass)
                {
                    continue;
                }

                if (function.HasAnyFunctionFlags(EFunctionFlags.Delegate | EFunctionFlags.MulticastDelegate) && function.IsIn(module.Package))
                {
                    globalFunctions.Add(function);
                }
            }

            GenerateCodeForModule(module, structs.ToArray(), enums.ToArray(), globalFunctions.ToArray());
        }

        private void GenerateCodeForModule(UnrealModuleInfo module, UStruct[] structs, UEnum[] enums, UFunction[] globalFunctions)
        {
            if (structs.Length == 0 && enums.Length == 0 && globalFunctions.Length == 0)
            {
                return;
            }

            BeginGenerateModule(module);

            GenerateCodeForGlobalFunctions(module, globalFunctions);

            foreach (UStruct unrealStruct in structs)
            {
                GenerateCodeForStruct(module, unrealStruct);
            }

            GenerateCodeForEnums(module, enums, Settings.MergeEnumFiles);

            EndGenerateModule(module);
        }

        private string GetModuleNamespace(UField field)
        {
            UnrealModuleType moduleAssetType;
            return GetModuleNamespace(field, out moduleAssetType);
        }

        private string GetModuleNamespace(UField field, out UnrealModuleType moduleAssetType, bool allowFoldersAsNamespace = true)
        {
            moduleAssetType = UnrealModuleType.Unknown;
            UPackage package = field.GetOutermost();
            if (package != null)
            {
                CachedNamespace cachedNamespace;
                if (namespaceCache.TryGetValue(package, out cachedNamespace))
                {
                    moduleAssetType = cachedNamespace.ModuleAssetType;
                    return cachedNamespace.Namespace;
                }

                UnrealModuleType moduleType = UnrealModuleType.Unknown;
                moduleAssetType = UnrealModuleType.Unknown;

                string packageFilename = package.FileName.ToString();
                if (string.IsNullOrEmpty(packageFilename.ToString()) || packageFilename == FName.None.ToString())
                {
                    packageFilename = field.GetPathName();
                }

                string moduleName = FPackageName.GetShortName(package.GetName());
                if (packageFilename.StartsWith("/Script"))
                {
                    if (!modulesByName.TryGetValue(new FName(moduleName), out moduleType))
                    {
                        moduleType = UnrealModuleType.Unknown;
                        FMessage.Log(ELogVerbosity.Error, string.Format("Failed to find module for module '{0}'", moduleName));
                    }
                }
                else if (packageFilename.StartsWith("/Game/"))
                {
                    moduleType = UnrealModuleType.Game;
                    moduleAssetType = UnrealModuleType.Game;
                    moduleName = FPaths.GetBaseFilename(FPaths.ProjectFilePath);// {Module} same as {Game}
                }
                else if (packageFilename.StartsWith("/Engine/"))
                {
                    moduleType = UnrealModuleType.Game;
                    moduleAssetType = UnrealModuleType.Engine;
                    moduleName = Settings.Namespaces.Default;
                }
                else
                {
                    string rootName = null;
                    if (packageFilename.Length > 1 && packageFilename[0] == '/')
                    {
                        int slashIndex = packageFilename.IndexOf('/', 1);
                        if (slashIndex >= 0)
                        {
                            rootName = packageFilename.Substring(1, slashIndex - 1);
                            moduleName = rootName;// Update ModuleName for {Module}

                            if (!modulesByName.TryGetValue(new FName(rootName), out moduleAssetType))
                            {
                                moduleAssetType = UnrealModuleType.Unknown;
                            }
                        }
                    }

                    if (moduleAssetType == UnrealModuleType.Unknown)
                    {
                        FMessage.Log(ELogVerbosity.Error, string.Format("Unknown module asset type root:'{0}' path:'{1}' name:'{2}' path2:'{3}'",
                            rootName, packageFilename, field.GetName(), field.GetPathName()));
                    }
                    moduleType = UnrealModuleType.Game;
                }

                if (moduleType != UnrealModuleType.Unknown)
                {
                    string namespaceName = GetModuleNamespace(moduleType, moduleName, moduleAssetType, allowFoldersAsNamespace, packageFilename);
                    namespaceCache[package] = new CachedNamespace(namespaceName, moduleName, moduleType, moduleAssetType);
                    return namespaceName;
                }
                else
                {
                    FMessage.Log(ELogVerbosity.Error, string.Format("Unknown module type {0} {1}", packageFilename, moduleName));
                }
            }
            return null;
        }

        private string GetModuleNamespace(UnrealModuleType moduleType, string moduleName)
        {
            // This should only be called for Engine/EnginePlugin modules (used for non-UObject types like TArray, TSubclassOf)
            if (moduleType != UnrealModuleType.Engine && moduleType != UnrealModuleType.EnginePlugin)
            {
                return null;
            }
            return GetModuleNamespace(moduleType, moduleName, UnrealModuleType.Unknown, false, string.Empty);
        }

        private string GetModuleNamespace(UnrealModuleType moduleType, string moduleName, UnrealModuleType moduleAssetType, bool allowFoldersAsNamespace, string path)
        {
            string namespaceName = null;

            switch (moduleType)
            {
                case UnrealModuleType.Game:
                    switch (moduleAssetType)
                    {
                        case UnrealModuleType.Engine:
                            namespaceName = Settings.Namespaces.EngineAsset;
                            break;
                        case UnrealModuleType.GamePlugin:
                            namespaceName = Settings.Namespaces.GamePluginAsset;
                            break;
                        case UnrealModuleType.EnginePlugin:
                            namespaceName = Settings.Namespaces.EnginePluginAsset;
                            break;
                        default:
                            namespaceName = Settings.Namespaces.Game;
                            break;
                    }
                    break;

                case UnrealModuleType.GamePlugin:
                    namespaceName = Settings.Namespaces.GamePlugin;
                    break;

                case UnrealModuleType.Engine:
                    namespaceName = Settings.Namespaces.Engine;
                    if (moduleName == "CoreUObject")
                    {
                        // Allow CoreUObject to be redirected to a common namespace name (UnrealEngine.Runtime)
                        namespaceName = GetEngineObjectNamespace();
                    }
                    break;

                case UnrealModuleType.EnginePlugin:
                    namespaceName = Settings.Namespaces.EnginePlugin;
                    break;

                default:
                    return namespaceName;
            }

            if (string.IsNullOrWhiteSpace(namespaceName))
            {
                return null;
            }

            if (namespaceName.Contains("{Default}"))
            {
                namespaceName = namespaceName.Replace("{Default}", Settings.Namespaces.Default);
            }

            if (namespaceName.Contains("{Game}"))
            {
                string gameName = FPaths.GetBaseFilename(FPaths.ProjectFilePath);
                namespaceName = namespaceName.Replace("{Game}", gameName);
            }

            if (namespaceName.Contains("{Module}"))
            {
                namespaceName = namespaceName.Replace("{Module}", moduleName);
            }

            if (namespaceName.Contains("{Folder}"))
            {
                if (allowFoldersAsNamespace && moduleAssetType != UnrealModuleType.Unknown)
                {
                    // Get the directory of the file and remove root name "/Game/"
                    string directory = FPackageName.GetLongPackagePath(path);
                    int slashIndex = directory.IndexOf('/', 1);
                    if (slashIndex >= 0)
                    {
                        directory = directory.Substring(slashIndex + 1);
                    }
                    else
                    {
                        // The path is empty or only the root name
                        directory = string.Empty;
                    }

                    // Make each '/' a part of the namespace
                    string expandedNamespace = directory.Replace("/", ".");

                    namespaceName = namespaceName.Replace("{Folder}", expandedNamespace);
                }
                else
                {
                    namespaceName = namespaceName.Replace("{Folder}", string.Empty);
                }
            }

            // Remove duplicate '.' chars
            StringBuilder result = new StringBuilder(namespaceName);
            for (int i = result.Length - 1; i >= 0; --i)
            {
                if (result[i] == '.')
                {
                    if (i == 0 || result[i - 1] == '.' || i == result.Length - 1)
                    {
                        result.Remove(i, 1);
                    }
                }
            }

            return result.ToString();
        }

        private string GetModuleName(UField field, out UnrealModuleType moduleType, out UnrealModuleType moduleAssetType)
        {
            moduleType = UnrealModuleType.Unknown;
            moduleAssetType = UnrealModuleType.Unknown;
            UPackage package = field.GetOutermost();
            if (package != null)
            {
                CachedNamespace cachedNamespace;
                if (!namespaceCache.TryGetValue(package, out cachedNamespace))
                {
                    GetModuleNamespace(field);
                    namespaceCache.TryGetValue(package, out cachedNamespace);
                }

                if (cachedNamespace != null)
                {
                    moduleType = cachedNamespace.ModuleType;
                    moduleAssetType = cachedNamespace.ModuleAssetType;
                    return cachedNamespace.ModuleName;
                }
            }
            return null;
        }

        private string GetUnrealModuleTypeString(UnrealModuleType moduleType, UnrealModuleType assetModuleType)
        {
            if (assetModuleType != UnrealModuleType.Unknown)
            {
                return assetModuleType.ToString();
            }
            return moduleType.ToString();
        }

        /// <summary>
        /// Namespace of the C# Unreal Engine runtime (UnrealEngine.Runtime)
        /// </summary>
        private string GetEngineRuntimeNamespace()
        {
            return GetModuleNamespace(UnrealModuleType.Engine, "Runtime");
        }

        private string GetEngineObjectNamespace()
        {
            // Since CoreUObject types are used everywhere redirect it into the UnrealEngine.Runtime namespace
            return GetEngineRuntimeNamespace();
            //return GetModuleNamespace(UnrealModuleType.Engine, "CoreUObject");
        }

        private string GetCollectionsNamespace()
        {
            return "System.Collections.Generic";
        }

        private List<string> GetDefaultNamespaces()
        {
            List<string> result = new List<string>();
            result.Add("System");
            result.Add("UnrealEngine.Runtime");
            return result;
        }

        private void BeginGenerateModules()
        {
            ClearState();

            // CodeGenerator.Modules.cs
            UpdateModulesByName();

            // CodeGenerator.AvailableTypes.cs
            UpdateAvailableTypes();

            // CodeGenerator.Properties.cs
            BeginGenerateModules_Properties();

            OnBeginGenerateModules();
        }

        private void EndGenerateModules()
        {
            ClearState();
            OnEndGenerateModules();
            Complete = true;
        }

        private void BeginGenerateModule(UnrealModuleInfo module)
        {
            OnBeginGenerateModule(module);
        }

        private void EndGenerateModule(UnrealModuleInfo module)
        {
            OnEndGenerateModule(module);
        }

        private void ClearState()
        {
            // CodeGenerator.Modules.cs
            modulesByName.Clear();
            namespaceCache.Clear();

            // CodeGenerator.AvailableTypes.cs
            availableTypes.Clear();

            // CodeGenerator.Properties.cs
            basicTypeNameMap.Clear();
            renamedTypes.Clear();
            selectiveMemberCategories.Clear();
            identifierCharMap.Clear();
            invalidIdentifierChars.Clear();
            identifierKeywords.Clear();

            // CodeGenerator.Enums.cs
            enumValuePrefixCache.Clear();

            // CodeGenerator.StructExporter.cs
            structInfos.Clear();
        }

        public class UnrealModuleInfo
        {
            public UPackage Package { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public UnrealModuleType Type { get; set; }

            public UnrealModuleInfo(UPackage package, string name, string path)
            {
                Package = package;
                Name = name;
                Path = path;
                Type = GetModuleType(path);
            }

            public static UnrealModuleType GetModuleType(string modulePath)
            {
                if (File.Exists(modulePath))
                {
                    string moduleDir = FPaths.GetPath(modulePath);
                    string subDir;

                    if (FPaths.DirectoryExists(moduleDir))
                    {
                        if (FPaths.IsSameOrSubDirectory(FPaths.EnginePluginsDir, moduleDir))
                        {
                            return UnrealModuleType.EnginePlugin;
                        }
                        else if (FPaths.IsSameOrSubDirectory(FPaths.Combine(FPaths.EngineDir, "Binaries"), moduleDir))
                        {
                            return UnrealModuleType.Engine;
                        }
                        else if (FPaths.IsSameOrSubDirectory(FPaths.ProjectPluginsDir, moduleDir))
                        {
                            return UnrealModuleType.GamePlugin;
                        }
                        else if (FPaths.IsSameOrSubDirectory(FPaths.ProjectDir, moduleDir, out subDir) && string.IsNullOrEmpty(subDir))
                        {
                            // Game module path is being treated as the .uproject path
                            return UnrealModuleType.Game;
                        }
                    }
                }
                return UnrealModuleType.Unknown;
            }
        }

        class CachedNamespace
        {
            public string Namespace { get; set; }
            public string ModuleName { get; set; }
            public UnrealModuleType ModuleType { get; set; }
            public UnrealModuleType ModuleAssetType { get; set; }

            public CachedNamespace(string namespaceName, string moduleName, UnrealModuleType moduleType, UnrealModuleType moduleAssetType)
            {
                Namespace = namespaceName;
                ModuleName = moduleName;
                ModuleType = moduleType;
                ModuleAssetType = moduleAssetType;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public class CodeGeneratorSettings
    {
        /// <summary>
        /// The method used for loading the underlying native type info (class address/properties/functions/offsets)
        /// </summary>
        public const string LoadNativeTypeMethodName = "LoadNativeType";

        /// <summary>
        /// The module name of the plugin written in C++
        /// </summary>
        public const string ModuleName = "USharp";

        /// <summary>
        /// States if the owner of the settings is going to generate code (enables additional settings using engine functions)
        /// </summary>
        public bool IsGeneratingCode { get; set; }

        /// <summary>
        /// Output location for managed modules
        /// </summary>
        public ManagedModulesLocation ModulesLocation { get; set; }
        public ManagedGameProjMerge GameProjMerge { get; private set; }
        public ManagedEngineProjMerge EngineProjMerge { get; private set; }

        /// <summary>
        /// Defines how module types should be found / exported (all, referenced, blueprint only) (defaults to referenced)
        /// </summary>
        public CodeExportMode ExportMode { get; set; }

        /// <summary>
        /// If true export all properties regardless of visibility
        /// </summary>
        public bool ExportAllProperties { get; set; }

        /// <summary>
        /// If true export all functions regardless of visibility
        /// </summary>
        public bool ExportAllFunctions { get; set; }

        /// <summary>
        /// If true export all structures (class / struct / function delegate signatures) regardless of visibility
        /// </summary>
        public bool ExportAllStructures { get; set; }

        /// <summary>
        /// Casing on member variables
        /// </summary>
        public CodeCasing MemberCasing { get; set; }

        /// <summary>
        /// Casing on function parameters
        /// </summary>
        public CodeCasing ParamCasing { get; set; }

        /// <summary>
        /// Type prefix settings
        /// </summary>
        public TypePrefixes Prefixes { get; private set; }

        /// <summary>
        /// Member category output on blueprint types
        /// </summary>
        public CodeMemberCategories BlueprintMemberCategories { get; set; }

        /// <summary>
        /// If true merge enums into a single file per module e.g. CoreUObjectEnums.cs
        /// </summary>
        public bool MergeEnumFiles { get; set; }

        /// <summary>
        /// Remove MAX/_MAX enum values
        /// </summary>
        public bool RemoveEnumMAX { get; set; }

        /// <summary>
        /// If true use fewer marshaling parameters on marshalers which support fewer parameters
        /// </summary>
        public bool MinimalMarshalingParams { get; set; }

        /// <summary>
        /// If true use safeguards on property/function accessors to avoid reading / writing invalid memory.
        /// This should be used if the generated C# can desync with the native type layout (hotreload / blueprint compile).
        /// This should generally be disabled in shipping builds as the C# should be up to date with the native type layout.
        /// </summary>
        public bool GenerateIsValidSafeguards { get; set; }

        /// <summary>
        /// If true insert an object destroyed check before accessing the native memory of that object 
        /// (used on UObject classes and structs which are treated as classes in C#)
        /// (this will insert a check at the start of all functions / properties)
        /// </summary>
        public bool CheckObjectDestroyed { get; set; }

        /// <summary>
        /// If true use the blueprint default function params
        /// </summary>
        public bool AllowBlueprintDefaultValueParams { get; set; }

        /// <summary>
        /// If true allow default function params to be generateed even if they are unknown e.g. default(FVector)
        /// </summary>
        public bool AllowUnknownDefaultValueParams { get; set; }

        /// <summary>
        /// The namespaces for the different types of assets / modules
        /// </summary>
        public ManagedNamespaces Namespaces { get; private set; }

        /// <summary>
        /// Emulate the folder layout as see in the engine (if false generated code will go into a single folder)
        /// </summary>
        public ManagedFolderEmulation FolderEmulation { get; private set; }

        /// <summary>
        /// Should the generated source filename use the type name instead of asset name
        /// </summary>
        public bool UseTypeNameAsSourceFileName { get; set; }

        /// <summary>
        /// Use fully qualified names instead of using statements
        /// </summary>
        public bool UseFullyQualifiedTypeNames { get; set; }

        /// <summary>
        /// Sort using statements for namespaces (used if UseFullyQualifiedTypeNames is false)
        /// </summary>
        public bool SortNamespaces { get; set; }

        /// <summary>
        /// Log into a text file which assets are loading (useful as some assets may crash on load)
        /// </summary>
        public bool LogAssetLoading { get; set; }

        /// <summary>
        /// Log all assets even if they are skipped due no changes
        /// </summary>
        public bool LogAssetLoadingVerbose { get; set; }

        /// <summary>
        /// Catch the crash when loading an asset and display which asset caused the crash
        /// </summary>
        public bool CatchCrashOnAssetLoading { get; set; }

        /// <summary>
        /// If true no documentation is generated
        /// </summary>
        public bool SkipDocumentation { get; set; }

        /// <summary>
        /// If there is no summary but there is a @return, inject the @return into the main summary
        /// </summary>
        public bool DocInjectReturnSummary { get; set; }

        /// <summary>
        /// Updates function param casing in docs relative to the ParamCasing value
        /// </summary>
        public bool DocUpdateParamCasing { get; set; }

        /// <summary>
        /// Trims trailing chars (' ' / '*')
        /// </summary>
        public bool DocTrimTrailingChars { get; set; }

        /// <summary>
        /// If true calculate the first char offset on each line in the main summary. Use the minimum char offset of all
        /// those lines as the starting offset for each line. If false the start of each line will be trimmed independently.
        /// </summary>
        public bool DocUseCommonSummaryTextOffset { get; set; }

        /// <summary>
        /// The indent to use for text builder
        /// </summary>
        public CSharpTextBuilder.IndentType IndentType { get; set; }

        /// <summary>
        /// Determines which functions should be collapsed into getters/setters
        /// </summary>
        public List<CollapsedMemberSettings> CollapsedMembers { get; private set; }

        /// <summary>
        /// If true functions / properties will be merged into a single property where it makes sense e.g. GetValue() / SetValue()
        /// </summary>
        public bool UseCollapsedMembers { get; set; }

        /// <summary>
        /// Use IList / ISet / IDictionary instead of TArrayReadWrite / TSetReadWrite / TMapReadWrite
        /// Note: If this is enabled every foreach will create garbage
        /// </summary>
        public bool UseCollectionInterfaces { get; set; }

        /// <summary>
        /// Make UObject in structs blittable. A GCHelper.Find lookup will be used for each access instead of being cached.
        /// </summary>
        public bool UObjectAsBlittableType { get; set; }

        /// <summary>
        /// Base struct members should be inlined into the parent struct (if false the base struct will be inlined as a "Base")
        /// </summary>
        public bool InlineBaseStruct { get; set; }

        /// <summary>
        /// If true "abstract" will be allowed on abstract classes. This means that if a given type is unknown but has
        /// a known abstract parent class, it will be given the upmost non-abstract class instead.
        /// e.g. UUnknownPawnAction : UPawnAction (abstract) : UObject
        /// UObject would be returned instead of the abstract UPawnAction where UUnknownPawnAction is an unknown type to C#.
        /// </summary>
        public bool UseAbstractTypes { get; set; }

        /// <summary>
        /// If true use "_Implementation" methods explicitly in a simlar way to C++. If false "_Implementation" methods will be
        /// generated / hidden.
        /// </summary>
        public bool UseExplicitImplementationMethods { get; set; }

        /// <summary>
        /// If true BlueprintEvent functions without an "_Implementation" method will be treated as BlueprintImplementableEvent.
        /// This requires UseExplicitImplementationMethods to be true.
        /// </summary>
        public bool UseImplicitBlueprintImplementableEvent { get; set; }

        /// <summary>
        /// If true call InitializeValue / DestroyValue on all parameters at the start / end of native function calls.
        /// If this is false init / destroy will only be called on parameters which are expected to require it.
        /// </summary>
        public bool LazyFunctionParamInitDestroy { get; set; }

        /// <summary>
        /// If true memzero after calling stackalloc (used on native function callers)
        /// </summary>
        public bool MemzeroStackalloc { get; set; }

        /// <summary>
        /// If true memzero after calling stackalloc (used on native function callers only when there is an "out" or "return" which aren't
        /// going to be initialized with a call to InitializeValue)
        /// NOTE: This is a seperate option from MemzeroStackalloc. If MemzeroStackalloc is true this option isn't required as all params
        /// will be memzerod.
        /// </summary>
        public bool MemzeroStackallocOnlyIfOut { get; set; }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        // StructsAsClass is incomplete and unsafe. Don't use these options.
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// If true always generate classes for UStructs instead of an actual struct. This will add overhead for small when marshaling
        /// as function parameters but will improve marshaling structs as UObject members.
        /// 
        /// NOTE: Generally this option should be disabled and structs should be classes when they are large enough to warrent it.
        /// </summary>
        public bool AlwaysGenerateStructsAsClasses { get; set; }

        /// <summary>
        /// Generate a class instead of a struct if there are are X+ properties/members on a non-blittable struct.
        /// If 0 is specified this option is unused.
        /// </summary>
        public int StructsAsClassesAtXProps_NonBlittable { get; set; }

        /// <summary>
        /// Generate a class instead of a struct if there are are X+ properties/members on a blittable struct.
        /// If 0 is specified this option is unused.
        /// </summary>
        public int StructsAsClassesAtXProps_Blittable { get; set; }

        /// <summary>
        /// A list of struct paths which should be generated as managed classes instead of a managed structs. These come with an overhead
        /// of always being allocated on the heap but also read faster when accessed on UObject instances.
        /// </summary>
        public HashSet<string> StructsAsClassesByPath { get; set; }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// If true UUserDefinedStruct is expected to be used in the editor and when it comes to rewriting the managed
        /// assembly UUserDefinedStruct editor restrictions will be placed on struct members.
        /// </summary>
        public bool UseUUserDefinedStructInEditor { get; set; }

        public VarNameSettings VarNames { get; private set; }

        // Settings files (project = defined in the projects folder, plugin = defined in USharp plugins folder)
        internal const string RenamedTypesFile = "RenameTypes.txt";// project
        internal const string SelectiveMemberCategoriesFile = "SelectiveCategories.txt";// project
        internal const string IdentifierCharMapFile = "IdentifierCharMap.txt";// project OR plugin
        internal const string IdentifierInvalidCharsFile = "IdentifierInvalidChars.txt";// project OR plugin
        internal const string IdentifierKeywordsFile = "IdentifierKeywords.txt";// project OR plugin
        internal const string StructsAsClassesFiles = "StructsAsClasses.txt";// project AND plugin

        public CodeGeneratorSettings()
        {
            Prefixes = new TypePrefixes();
            Namespaces = new ManagedNamespaces();
            FolderEmulation = new ManagedFolderEmulation();
            CollapsedMembers = new List<CollapsedMemberSettings>();
            StructsAsClassesByPath = new HashSet<string>();
            VarNames = new VarNameSettings();
            LoadDefaults();
        }

        public void LoadDefaults()
        {
            ModulesLocation = ManagedModulesLocation.ModulesFolder;

            GameProjMerge = ManagedGameProjMerge.Plugins;
            EngineProjMerge = ManagedEngineProjMerge.EngineAndPluginsCombined;

            ExportMode = CodeExportMode.Referenced;

            MemberCasing = CodeCasing.Default;
            ParamCasing = CodeCasing.Default;

            SkipDocumentation = false;
            DocInjectReturnSummary = true;
            DocUpdateParamCasing = true;
            DocTrimTrailingChars = true;
            DocUseCommonSummaryTextOffset = true;

            Prefixes.Enum.Mode = TypePrefixMode.Enforce;
            Prefixes.Generics.Mode = TypePrefixMode.Enforce;
            Prefixes.Struct.Mode = TypePrefixMode.Enforce;
            Prefixes.Actor.Mode = TypePrefixMode.Enforce;
            Prefixes.Object.Mode = TypePrefixMode.Enforce;
            Prefixes.Interface.Mode = TypePrefixMode.Enforce;

            BlueprintMemberCategories = CodeMemberCategories.SelectivePrefix;

            MergeEnumFiles = true;
            RemoveEnumMAX = true;

            MinimalMarshalingParams = true;

            GenerateIsValidSafeguards = true;
            CheckObjectDestroyed = true;

            AllowBlueprintDefaultValueParams = true;

            Namespaces.Default = "UnrealEngine";
            Namespaces.Game = "{Game}.{Folder}";
            Namespaces.GamePlugin = "{Game}.Plugins.{Module}";
            Namespaces.GamePluginAsset = "{Game}.Plugins.Assets.{Module}.{Folder}";
            Namespaces.Engine = "{Default}.{Module}";
            Namespaces.EngineAsset = "{Default}.Assets.{Folder}";
            Namespaces.EnginePlugin = "{Default}.Plugins.{Module}";
            Namespaces.EnginePluginAsset = "{Default}.Plugins.Assets.{Module}.{Folder}";

            FolderEmulation.Game = true;
            FolderEmulation.GamePluginAssets = true;
            FolderEmulation.EngineAssets = true;
            FolderEmulation.EnginePluginAssets = true;

            UseTypeNameAsSourceFileName = false;
            UseFullyQualifiedTypeNames = true;
            SortNamespaces = true;

            LogAssetLoading = true;
            LogAssetLoadingVerbose = true;
            CatchCrashOnAssetLoading = true;

            IndentType = CSharpTextBuilder.IndentType.Spaces;

            UseCollapsedMembers = false;

            UseCollectionInterfaces = false;

            UObjectAsBlittableType = true;

            InlineBaseStruct = true;

            UseAbstractTypes = false;

            UseExplicitImplementationMethods = true;
            UseImplicitBlueprintImplementableEvent = false;

            LazyFunctionParamInitDestroy = false;
            MemzeroStackalloc = true;
            MemzeroStackallocOnlyIfOut = false;

            // StructsAsClass is incomplete and unsafe. Don't use these options.
            AlwaysGenerateStructsAsClasses = false;
            StructsAsClassesAtXProps_NonBlittable = 0;
            StructsAsClassesAtXProps_Blittable = 0;

            // These collapsed members could potentially cause some strange merging of unexpected members.
            // Look out for issues and improve / disable collapsing if this becomes a problem.
            bool stripPrefixBool = false;
            CollapsedMembers.Add(new CollapsedMemberSettings()
            {
                RequiresBool = true,
                GetPrefix = "Is",
                StripPrefix = stripPrefixBool
            });
            CollapsedMembers.Add(new CollapsedMemberSettings()
            {
                RequiresBool = true,
                GetPrefix = "Has",
                StripPrefix = stripPrefixBool
            });
            CollapsedMembers.Add(new CollapsedMemberSettings()
            {
                RequiresBool = true,
                GetPrefix = "Should",
                StripPrefix = stripPrefixBool
            });
            CollapsedMembers.Add(new CollapsedMemberSettings()
            {
                RequiresBool = true,
                GetPrefix = "Can",
                StripPrefix = stripPrefixBool
            });

            CollapsedMembers.Add(new CollapsedMemberSettings()
            {
                GetPrefix = "Get",
                SetPrefix = "Set",
                StripPrefix = true,

                // GetXXX will appear as a seperate blueprint node. Only collapse if there is a setter.
                GetRequiresSet = true,

                // SetXXX overrides the default blueprint set node. Collapse the setter if there is a backing property.
                SetRequiresGet = false
            });

            VarNames = new VarNameSettings();
            VarNames.LoadNativeType = LoadNativeTypeMethodName;
            VarNames.LoadNativeTypeInjected = VarNames.LoadNativeType + "Injected";
            VarNames.ClassAddress = "classAddress";
            VarNames.IsValid = "_IsValid";
            VarNames.FunctionAddress = "_FunctionAddress";
            VarNames.InstanceFunctionAddress = "_InstanceFunctionAddress";
            VarNames.PropertyAddress = "_PropertyAddress";
            VarNames.MemberOffset = "_Offset";
            VarNames.ParamsBufferAllocation = "ParamsBufferAllocation";
            VarNames.ParamsBuffer = "ParamsBuffer";
            VarNames.ParamsSize = "_ParamsSize";
            VarNames.ReturnResult = "toReturn";
            VarNames.CollectionMarshaler = "_Marshaler";
            VarNames.CollectionMarshalerCached = "_MarshalerCached";
            VarNames.FixedSizeArrayCached = "_FixedSizeArrayCached";
            VarNames.StructSize = "_StructSize";
            VarNames.StructAddress = "_StructAddress";
            VarNames.StructCopy = "Copy";
            VarNames.UObjectBlittableName = "_ObjectPtr";
            VarNames.StructAsClassCached = "_StructCached";
            VarNames.FTextCached = "_TextCached";
            VarNames.DelegateCached = "_DelegateCached";
            VarNames.DelegateInvoker = "Invoker";
            VarNames.DelegateSignature = "Signature";
            VarNames.DelegateMarshaler = "Marshaler";
            VarNames.ImplementationMethod = "_Implementation";
            VarNames.RPCValidate = "_Validate";
            VarNames.FunctionInvoker = "__Invoker";
        }

        public string GetInjectedClassesDir()
        {
            return Path.Combine(GetUSharpBaseDir(),
                    "Managed", "UnrealEngine.Runtime", "UnrealEngine.Runtime", "Internal", "InjectedClasses");
        }

        /// <summary>
        /// Returns /USharp/
        /// </summary>
        public string GetUSharpBaseDir()
        {
            string binariesDir = GetBinDir();
            return Path.GetFullPath(Path.Combine(binariesDir, "../"));
        }

        /// <summary>
        /// Returns /USharp/Binaries/
        /// </summary>
        public string GetBinDir()
        {
            // This gives "/Binaries/XXXX/" where XXXX is the platform (Win32, Win64, Android, etc)
            string path = FPaths.GetPath(FModuleManager.Get().GetModuleFilename(new FName(ModuleName)));

            // Move this up to "/Binaries/"
            return Path.Combine(path, "../");
        }

        /// <summary>
        /// Returns /USharp/Binaries/Managed/
        /// </summary>
        public string GetManagedBinDir()
        {
            // Managed plugins should be under "/Binaries/Managed/"
            return Path.Combine(GetBinDir(), "Managed");
        }

        /// <summary>
        /// Returns /USharp/Binaries/Managed/Settings/
        /// </summary>
        public string GetManagedPluginSettingsDir()
        {
            return Path.Combine(GetManagedBinDir(), "Settings");
        }

        public string GetManagedProjectSettingsDir()
        {
            return Path.Combine(GetManagedDir(), "Settings");
        }

        /// <summary>
        /// Returns PROJECT_DIR/Intermediat/Managed/
        /// </summary>
        public string GetManagedIntermediateDir()
        {
            string directory = Path.Combine(FPaths.ProjectIntermediateDir, "Managed");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }

        /// <summary>
        /// Returns PROJECT_DIR/Managed/
        /// </summary>
        public string GetManagedDir()
        {
            string directory = Path.Combine(FPaths.ProjectDir, "Managed");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }

        public string GetManagedModulesDir()
        {
            return GetManagedModulesDir(ModulesLocation != ManagedModulesLocation.ModulesFolder);
        }

        public string GetManagedModulesDir(bool managedModulesInGameDir)
        {
            if (managedModulesInGameDir)
            {
                // Used when managed modules are put into the game folder
                return Path.Combine(GetManagedDir(), "EngineModules");
            }
            else
            {
                return Path.Combine(GetManagedBinDir(), "Modules");
            }
        }

        public string GetGeneratedCodeDir(bool isPlugin)
        {
            return Path.Combine(GetManagedDir(), GetProjectName() + (isPlugin ? ".NativePlugins" : ".Native"));
        }

        public string GetProjectName()
        {
            return Path.GetFileNameWithoutExtension(FPaths.ProjectFilePath);
        }

        /*public CollapsedFunctionSettings FindCollapsedFunctionSettings(string name)
        {
            foreach (CollapsedFunctionSettings collapsedFunctions in CollapsedFunctions)
            {

            }
        }*/

        public void Load()
        {
            StructsAsClassesByPath.Clear();
            if (IsGeneratingCode)
            {
                string[] structsAsClassesFiles =
                {
                    Path.Combine(GetManagedProjectSettingsDir(), StructsAsClassesFiles),
                    Path.Combine(GetManagedPluginSettingsDir(), StructsAsClassesFiles)
                };
                foreach (string filePath in structsAsClassesFiles)
                {
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            string[] lines = File.ReadAllLines(filePath);
                            foreach (string line in lines)
                            {
                                if (!string.IsNullOrWhiteSpace(line))
                                {
                                    // Use a tolower comparison
                                    StructsAsClassesByPath.Add(line.Trim().ToLower());
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void Save()
        {
        }

        public enum ManagedModulesLocation
        {
            /// <summary>
            /// Modules will be in the managed plugin modules folder (/Binaries/Managed/Modules)
            /// </summary>
            ModulesFolder,

            /// <summary>
            /// Modules will be in the game folder but in their own sln
            /// </summary>
            GameFolder,

            /// <summary>
            /// Modules will be in the game folder and they will also be combined into the game sln
            /// </summary>
            GameFolderCombineSln,

            /// <summary>
            /// Modules will be in the game folder and they will also be combined into the game sln and proj
            /// </summary>
            GameFolderCombineSlnProj
        }

        public enum ManagedEngineProjMerge
        {
            /// <summary>
            /// Don't merge any modules
            /// </summary>
            None,

            /// <summary>
            /// Merge engine modules (UnrealEngine.csproj)
            /// </summary>
            Engine,

            /// <summary>
            /// Merge plugin modules (UnrealEngine.Plugins.csproj)
            /// </summary>
            Plugins,

            /// <summary>
            /// Merge both engine and plugin modules (UnrealEngine.csproj / UnrealEngine.Plugins.csproj)
            /// </summary>
            EngineAndPlugins,

            /// <summary>
            /// Merge both engine and plugin modules (UnrealEngine.csproj)
            /// </summary>
            EngineAndPluginsCombined
        }

        public enum ManagedGameProjMerge
        {
            /// <summary>
            /// Don't merge any projs
            /// </summary>
            None,

            /// <summary>
            /// Merge game plugin projs
            /// </summary>
            Plugins,

            /// <summary>
            /// Merge game and game plugin projs
            /// </summary>
            GameAndPlugins
        }

        public class ManagedNamespaces
        {
            /// <summary>
            /// The value for the {Default} namespace tag - {Default} {Game} {Module} {Folder} (case sensitive)
            /// </summary>
            public string Default { get; set; }
            public string Game { get; set; }
            public string GamePlugin { get; set; }
            public string GamePluginAsset { get; set; }            
            public string Engine { get; set; }
            public string EngineAsset { get; set; }
            public string EnginePlugin { get; set; }
            public string EnginePluginAsset { get; set; }
        }

        public class ManagedFolderEmulation
        {
            public bool Game { get; set; }
            public bool GamePluginAssets { get; set; }
            public bool EngineAssets { get; set; }
            public bool EnginePluginAssets { get; set; }
        }

        public enum TypePrefixMode
        {
            Default,
            Strip,
            Enforce
        }

        public class TypePrefix
        {
            public char Char { get; set; }
            public TypePrefixMode Mode { get; set; }

            public TypePrefix()
            {
            }

            public TypePrefix(char prefix)
            {
                Char = prefix;
            }

            public TypePrefix(char prefix, TypePrefixMode mode)
            {
                Char = prefix;
                Mode = mode;
            }
        }

        public class TypePrefixes
        {
            /// <summary>
            /// Enum types "EMyEnum"
            /// </summary>
            public TypePrefix Enum { get; set; }

            /// <summary>
            /// Generic types "TArray"
            /// </summary>
            public TypePrefix Generics { get; set; }

            /// <summary>
            /// Struct / non UObject class types "FMyStruct"
            /// </summary>
            public TypePrefix Struct { get; set; }

            /// <summary>
            /// Actor types "AActor"
            /// </summary>
            public TypePrefix Actor { get; set; }

            /// <summary>
            /// Object types "UObject"
            /// </summary>
            public TypePrefix Object { get; set; }

            /// <summary>
            /// Interface types "IInterface"
            /// </summary>
            public TypePrefix Interface { get; set; }

            public TypePrefixes()
            {
                Enum = new TypePrefix('E');
                Generics = new TypePrefix('T');
                Struct = new TypePrefix('F');
                Actor = new TypePrefix('A');
                Object = new TypePrefix('U');
                Interface = new TypePrefix('I');
            }
        }

        public enum CodeCasing
        {
            /// <summary>
            /// Leave the casing as it is
            /// </summary>
            Default,

            /// <summary>
            /// Force PascalCasing
            /// </summary>
            PascalCasing,

            /// <summary>
            /// Force camelCasing
            /// </summary>
            CamelCasing
        }

        public enum CodeExportMode
        {
            /// <summary>
            /// Export all types
            /// </summary>
            All,

            /// <summary>
            /// Export blueprint types and everything referenced (chained)
            /// </summary>
            Referenced,

            /// <summary>
            /// Export blueprint types only
            /// </summary>
            BlueprintOnly
        }

        public enum CodeMemberCategories
        {
            None,

            /// <summary>
            /// Prefix members with their category name
            /// </summary>
            Prefix,

            /// <summary>
            /// Post members with their category name
            /// </summary>
            Postfix,

            ///// <summary>
            ///// TODO: Put memebers with a category defined into a nested class
            ///// </summary>
            //TODO_Nested,

            /// <summary>
            /// Use a file to determine which types should be prefixed
            /// </summary>
            SelectivePrefix,

            /// <summary>
            /// Use a file to determine which types should be postfixed
            /// </summary>
            SelectivePostfix,

            ///// <summary>
            ///// TODO: Use a file to determine which types should be nested
            ///// </summary>
            //TODO_SelectiveNested
        }

        public class CollapsedMemberSettings
        {
            // In blueprint if there is a property with a given name and a function SetXXX with the same name
            // then the blueprint will replace the SetXXX accessor with the function.

            // In blueprint if there is a property with a given name and a function GetXXX then blueprint
            // DOESN'T replace the property with the function.
            // - The extra GetXXX in blueprint is left as a seperate function

            /// <summary>
            /// Should the prefix be stripped from the name (will auto stip if both getter/setter)
            /// </summary>
            public bool StripPrefix { get; set; }

            /// <summary>
            /// The get prefix (e.g. "Get", "Is", "Has")
            /// </summary>
            public string GetPrefix { get; set; }

            /// <summary>
            /// The set prefix (e.g. "Set")
            /// </summary>
            public string SetPrefix { get; set; }

            /// <summary>
            /// Only collapse is this function uses bool
            /// </summary>
            public bool RequiresBool { get; set; }

            /// <summary>
            /// Only collapse Get if there is a Set
            /// </summary>
            public bool GetRequiresSet { get; set; }

            /// <summary>
            /// Only collapse Set if there is a Get
            /// </summary>
            public bool SetRequiresGet { get; set; }

            /// <summary>
            /// If there isn't a backing property and Get isn't found but there is a non-exportable property available
            /// then inject that property as a getter (this is only used on Set).
            /// </summary>
            public bool InjectNonExportableProperty { get; set; }
        }

        public class VarNameSettings
        {
            /// <summary>
            /// static void LoadNativeType() { }
            /// </summary>
            public string LoadNativeType { get; set; }

            /// <summary>
            /// static void LoadNativeTypeInjected(IntPtr classAddress) { }
            /// </summary>
            public string LoadNativeTypeInjected { get; set; }

            /// <summary>
            /// static IntPtr ClassAddress;
            /// </summary>
            public string ClassAddress { get; set; }

            /// <summary>
            /// static bool XXXX_IsValid; (used for safeguards if C# types are out of sync with their native equivalent)
            /// </summary>
            public string IsValid { get; set; }

            /// <summary>
            /// static IntPtr XXXX_FunctionAddress;
            /// </summary>
            public string FunctionAddress { get; set; }

            /// <summary>
            /// static IntPtr XXXX_InstanceFunctionAddress;
            /// </summary>
            public string InstanceFunctionAddress { get; set; }

            /// <summary>
            /// static IntPtr XXXX_PropertyAddress;
            /// </summary>
            public string PropertyAddress { get; set; }

            /// <summary>
            /// static int XXXX_Offset;
            /// </summary>
            public string MemberOffset { get; set; }
            
            /// <summary>
            /// TArrayCopyMarshaler&lt;T> XXXX_Marshaler;
            /// </summary>
            public string CollectionMarshaler { get; set; }

            /// <summary>
            /// TArrayReadWriteMarshaler&lt;T> XXXX_MarshalerCached;
            /// </summary>
            public string CollectionMarshalerCached { get; set; }

            /// <summary>
            /// TFixedSizeArray&lt;T> XXXX_FixedSizeArrayCached;
            /// </summary>
            public string FixedSizeArrayCached { get; set; }

            /// <summary>
            /// byte* ParamsBufferAllocation = stackalloc byte[];
            /// </summary>
            public string ParamsBufferAllocation { get; set; }

            /// <summary>
            /// IntPtr ParamsBuffer = new IntPtr(XXXX);
            /// </summary>
            public string ParamsBuffer { get; set; }

            /// <summary>
            /// static int XXXX_ParamsSize;
            /// </summary>
            public string ParamsSize { get; set; }

            /// <summary>
            /// XXXX toReturn = XXXX;
            /// </summary>
            public string ReturnResult { get; set; }

            /// <summary>
            /// static int XXXX_StructSize;
            /// </summary>
            public string StructSize { get; set; }

            /// <summary>
            /// static IntPtr XXXX_StructAddress;
            /// </summary>
            public string StructAddress { get; set; }

            /// <summary>
            /// public XXXX Copy() { ... }
            /// </summary>
            public string StructCopy { get; set; }

            /// <summary>
            /// private IntPtr XXXX_ObjectPtr;
            /// </summary>
            public string UObjectBlittableName { get; set; }

            /// <summary>
            /// FSomeStructAsClass XXXX_StructCached;
            /// </summary>
            public string StructAsClassCached { get; set; }

            /// <summary>
            /// FText XXXX_TextCached;
            /// </summary>
            public string FTextCached { get; set; }

            /// <summary>
            /// FMulticastDelegate&lt;T> XXXX_DelegateCached;
            /// </summary>
            public string DelegateCached { get; set; }

            /// <summary>
            /// private XXXX Invoker(XXXX) { ... }
            /// </summary>
            public string DelegateInvoker { get; set; }

            /// <summary>
            /// public delegate XXXX Signature;
            /// </summary>
            public string DelegateSignature { get; set; }

            /// <summary>
            /// public static class Marshaler&lt;T> { ... }
            /// </summary>
            public string DelegateMarshaler { get; set; }

            /// <summary>
            /// public void XXXX_Implementation(...) { ... }
            /// </summary>
            public string ImplementationMethod { get; set; }

            /// <summary>
            /// public void XXXX_Validate(...) { ... }
            /// </summary>
            public string RPCValidate { get; set; }

            /// <summary>
            /// public void XXXX__Invoke(IntPtr buffer, IntPtr objPtr) { ... }
            /// </summary>
            public string FunctionInvoker { get; set; }
        }
    }
}

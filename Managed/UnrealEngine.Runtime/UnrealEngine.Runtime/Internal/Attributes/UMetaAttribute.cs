using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class UMetaAttribute : Attribute
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public UMetaAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public UMetaAttribute(string key, bool value)
            : this(key, value.ToString())
        {
        }

        public UMetaAttribute(string key, int value)
            : this(key, value.ToString())
        {
        }

        public UMetaAttribute(string key, float value)
            : this(key, value.ToString())
        {
        }

        public UMetaAttribute(string key, UClass value)
            : this(key, value == null ? string.Empty : value.GetPathName())
        {
        }

        //////////////////////////////////////////////////////
        // MD
        //////////////////////////////////////////////////////

        public UMetaAttribute(MD key, string value)
            : this(UMeta.GetKey(key), value)
        {
        }

        public UMetaAttribute(MD key) 
            : this(key, true)
        {
        }

        public UMetaAttribute(MD key, bool value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MD key, int value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MD key, float value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MD key, UClass value)
            : this(UMeta.GetKey(key), value == null ? string.Empty : value.GetPathName())
        {
        }

        //////////////////////////////////////////////////////
        // MDFunc
        //////////////////////////////////////////////////////

        public UMetaAttribute(MDFunc key, string value)
            : this(UMeta.GetKey(key), value)
        {
        }

        public UMetaAttribute(MDFunc key)
            : this(key, true)
        {
        }

        public UMetaAttribute(MDFunc key, bool value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDFunc key, int value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDFunc key, float value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDFunc key, UClass value)
            : this(UMeta.GetKey(key), value == null ? string.Empty : value.GetPathName())
        {
        }

        //////////////////////////////////////////////////////
        // MDProp
        //////////////////////////////////////////////////////

        public UMetaAttribute(MDProp key, string value)
            : this(UMeta.GetKey(key), value)
        {
        }

        public UMetaAttribute(MDProp key)
            : this(key, true)
        {
        }

        public UMetaAttribute(MDProp key, bool value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDProp key, int value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDProp key, float value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDProp key, UClass value)
            : this(UMeta.GetKey(key), value == null ? string.Empty : value.GetPathName())
        {
        }

        //////////////////////////////////////////////////////
        // MDClass
        //////////////////////////////////////////////////////

        public UMetaAttribute(MDClass key, string value)
            : this(UMeta.GetKey(key), value)
        {
        }

        public UMetaAttribute(MDClass key)
            : this(key, true)
        {
        }

        public UMetaAttribute(MDClass key, bool value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDClass key, int value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDClass key, float value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDClass key, UClass value)
            : this(UMeta.GetKey(key), value == null ? string.Empty : value.GetPathName())
        {
        }

        //////////////////////////////////////////////////////
        // MDStruct
        //////////////////////////////////////////////////////

        public UMetaAttribute(MDStruct key, string value)
            : this(UMeta.GetKey(key), value)
        {
        }

        public UMetaAttribute(MDStruct key)
            : this(key, true)
        {
        }

        public UMetaAttribute(MDStruct key, bool value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDStruct key, int value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDStruct key, float value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDStruct key, UClass value)
            : this(UMeta.GetKey(key), value == null ? string.Empty : value.GetPathName())
        {
        }

        //////////////////////////////////////////////////////
        // MDEnum
        //////////////////////////////////////////////////////

        public UMetaAttribute(MDEnum key, string value)
            : this(UMeta.GetKey(key), value)
        {
        }

        public UMetaAttribute(MDEnum key)
            : this(key, true)
        {
        }

        public UMetaAttribute(MDEnum key, bool value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDEnum key, int value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDEnum key, float value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDEnum key, UClass value)
            : this(UMeta.GetKey(key), value == null ? string.Empty : value.GetPathName())
        {
        }

        //////////////////////////////////////////////////////
        // MDInterface
        //////////////////////////////////////////////////////

        public UMetaAttribute(MDInterface key, string value)
            : this(UMeta.GetKey(key), value)
        {
        }

        public UMetaAttribute(MDInterface key)
            : this(key, true)
        {
        }

        public UMetaAttribute(MDInterface key, bool value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDInterface key, int value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDInterface key, float value)
            : this(UMeta.GetKey(key), value.ToString())
        {
        }

        public UMetaAttribute(MDInterface key, UClass value)
            : this(UMeta.GetKey(key), value == null ? string.Empty : value.GetPathName())
        {
        }

        public bool AsBool()
        {
            bool result;
            bool.TryParse(GetTryParseSafeValue(), out result);
            return result;
        }

        public int AsInt()
        {
            int result;
            int.TryParse(GetTryParseSafeValue(), out result);
            return result;
        }

        public float AsFloat()
        {
            float result;
            float.TryParse(GetTryParseSafeValue(), out result);
            return result;
        }

        public UClass AsClass()
        {
            if (!string.IsNullOrEmpty(Value))
            {
                return UObject.FindObject<UClass>(ObjectOuter.AnyPackage, Value);
            }
            return null;
        }

        private string GetTryParseSafeValue()
        {
            return Value != null ? Value : string.Empty;
        }
    }

    /// <summary>
    /// Helper for working with Unreals metadata
    /// </summary>
    class UMeta
    {
        /// <summary>
        /// The target for a given piece of metadata
        /// </summary>
        public enum Target
        {
            Class,
            Interface,
            Enum,
            Struct,
            Property,// Includes all properties, function params and return results for now
            Delegate,
            Function
        }

        /// <summary>
        /// Returns the group name for the given group enum (just uses typeof(TEnum).Name)
        /// </summary>
        public static string GetGroup<TEnum>() where TEnum : struct, Enum
        {
            return typeof(TEnum).Name;
        }

        public static FName GetKeyName<TEnum>(TEnum key) where TEnum : struct, Enum
        {
            return new FName(key.ToString());
        }

        public static string GetKey<TEnum>(TEnum key) where TEnum : struct, Enum
        {
            return key.ToString();
        }

        public static TEnum ParseKey<TEnum>(string key) where TEnum : struct, Enum
        {
            TEnum result;
            Enum.TryParse(key, out result);
            return result;
        }

        private static IntPtr GetMetaDataFromObj(IntPtr obj)
        {
            if (obj == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            IntPtr package = Native.Native_UObjectBaseUtility.GetOutermost(obj);
            if (package == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            return Native.Native_UPackage.GetMetaData(package);
        }

        public static bool HasMetaData(IntPtr obj, string key)
        {
            IntPtr metadata = GetMetaDataFromObj(obj);
            if (metadata != IntPtr.Zero)
            {
                using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
                {
                    return Native.Native_UMetaData.HasValue(metadata, obj, ref keyUnsafe.Array);
                }
            }
            return false;
        }

        public static bool HasMetaData<TEnum>(IntPtr obj, TEnum key) where TEnum : struct, Enum
        {
            return HasMetaData(obj, GetKey(key));
        }

        public static void SetMetaData<T>(IntPtr obj, string key, T value)
        {
            string valueStr = null;
            UClass unrealClass = value as UClass;
            if (unrealClass != null)
            {
                valueStr = unrealClass.GetPathName();
            }
            else
            {
                valueStr = value.ToString();
            }

            IntPtr metadata = GetMetaDataFromObj(obj);
            if (metadata != IntPtr.Zero)
            {
                using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
                using (FStringUnsafe valueUnsafe = new FStringUnsafe(key))
                {
                    Native.Native_UMetaData.SetValue(metadata, obj, ref keyUnsafe.Array, ref valueUnsafe.Array);
                }
            }
        }

        public static void SetMetaData<TEnum, T>(IntPtr obj, TEnum key, T value) where TEnum : struct, Enum
        {
            SetMetaData(obj, GetKey(key), value);
        }

        public static void RemoveMetaData<TEnum>(IntPtr obj, TEnum key) where TEnum : struct, Enum
        {
            RemoveMetaData(obj, GetKey(key));
        }

        public static void RemoveMetaData(IntPtr obj, string key)
        {
            IntPtr metadata = GetMetaDataFromObj(obj);
            if (metadata != IntPtr.Zero)
            {
                using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
                {
                    Native.Native_UMetaData.RemoveValue(metadata, obj, ref keyUnsafe.Array);
                }
            }
        }
    }

    /// <summary>
    /// Common metadata values
    /// </summary>
    public enum MD
    {
        Unknown,

        /// <summary>
        /// Overrides the automatically generated tooltip from the class comment
        /// </summary>
        ToolTip,

        /// <summary>
        /// A short tooltip that is used in some contexts where the full tooltip might be overwhelming (such as the parent class picker dialog)
        /// </summary>
        ShortTooltip,

        /// <summary>
        /// The name to display for this class, property, or function instead of auto-generating it from the name.
        /// </summary>
        DisplayName,

        /// <summary>
        /// 
        /// </summary>
        FriendlyName,

        /// <summary>
        /// Specifies the category when displayed in blueprint editing tools.
        /// Usage: Category=CategoryName or Category="MajorCategory,SubCategory"
        /// </summary>
        Category,

        /// <summary>
        /// Used to hide enum values (is this used for anything else, where is this parsed?)
        /// </summary>
        Hidden
    }

    /// <summary>
    /// Metadata usable on interfaces exposed to Unreal
    /// </summary>
    public enum MDInterface
    {
        Unknown,

        /// <summary>
        /// This interface cannot be implemented by a blueprint (e.g., it has only non-exposed C++ member methods)
        /// </summary>
        CannotImplementInterfaceInBlueprint
    }

    /// <summary>
    /// Metadata usable on structs exposed to Unreal
    /// </summary>
    public enum MDEnum
    {
        Unknown,

        /// <summary>
        /// Exposes this enum as a type that can be used for variables in blueprints
        /// </summary>
        BlueprintType,

        /// <summary>
        /// The name to display for this enum instead of auto-generating it from the name.
        /// </summary>
        DisplayName,

        //////////////////////////////////////////////////////////////////////////////////////////
        // Defined in Engine\Source\Editor\BlueprintGraph\Classes\EdGraphSchema_K2.h
        //////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Metadata that identifies an integral property as a bitmask.
        /// </summary>
        Bitmask,

        /// <summary>
        /// Metadata that associates a bitmask property with a bitflag enum.
        /// </summary>
        BitmaskEnum,

        /// <summary>
        /// Metadata that identifies an enum as a set of explicitly-named bitflags.
        /// </summary>
        Bitflags,

        /// <summary>
        /// Metadata that signals to the editor that enum values correspond to mask values instead of bitshift (index) values.
        /// </summary>
        UseEnumValuesAsMaskValuesInEditor,
    }

    /// <summary>
    /// Metadata usable on structs exposed to Unreal
    /// </summary>
    public enum MDStruct
    {
        Unknown,

        /// <summary>
        /// Indicates that the struct has a custom break node (and what the path to the BlueprintCallable UFunction is) that should be used instead of the default BreakStruct node.  
        /// </summary>
        HasNativeBreak,

        /// <summary>
        /// Indicates that the struct has a custom make node (and what the path to the BlueprintCallable UFunction is) that should be used instead of the default MakeStruct node.  
        /// </summary>
        HasNativeMake,

        /// <summary>
        /// Pins in Make and Break nodes are hidden by default.
        /// </summary>
        HiddenByDefault,

        //////////////////////////////////////////////////////////////////////////////////////////
        // These are specified as USTRUCT "keywords" but are metadata
        //////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// Exposes this struct as a type that can be used for variables in blueprints
        /// </summary>
        BlueprintType,

        /// <summary>
        /// Indicates that a BlueprintType struct should not be exposed to the end user
        /// </summary>
        BlueprintInternalUseOnly
    }

    /// <summary>
    /// Metadata usable on classes exposed to Unreal
    /// </summary>
    public enum MDClass
    {
        Unknown,

        /// <summary>
        /// Used for Actor Component classes. If present indicates that it can be spawned by a Blueprint.
        /// </summary>
        BlueprintSpawnableComponent,

        /// <summary>
        /// Used for Actor and Component classes. If the native class cannot tick, Blueprint generated classes based this Actor or Component can have bCanEverTick flag overridden even if bCanBlueprintsTickByDefault is false.
        /// </summary>
        ChildCanTick,

        /// <summary>
        /// Used for Actor and Component classes. If the native class cannot tick, Blueprint generated classes based this Actor or Component can never tick even if bCanBlueprintsTickByDefault is true.
        /// </summary>
        ChildCannotTick,

        /// <summary>
        /// Used to make the first subclass of a class ignore all inherited showCategories and hideCategories commands
        /// </summary>
        IgnoreCategoryKeywordsInSubclasses,// Specified by "ComponentWrapperClass" in UCLASS() keyword

        /// <summary>
        /// For BehaviorTree nodes indicates that the class is deprecated and will display a warning when compiled.
        /// </summary>
        DeprecatedNode,

        /// <summary>
        /// Used in conjunction with DeprecatedNode or DeprecatedFunction to customize the warning message displayed to the user.
        /// </summary>
        DeprecationMessage,

        /// <summary>
        /// The name to display for this class, property, or function instead of auto-generating it from the name.
        /// </summary>
        DisplayName,

        /// <summary>
        /// The name to use for this class, property, or function when exporting it to a scripting language. May include deprecated names as additional semi-colon separated entries.
        /// </summary>
        ScriptName,// added 4.19

        /// <summary>
        /// Specifies that this class is an acceptable base class for creating blueprints.
        /// </summary>
        IsBlueprintBase,

        /// <summary>
        /// Comma delimited list of blueprint events that are not be allowed to be overridden in classes of this type
        /// </summary>
        KismetHideOverrides,

        /// <summary>
        /// Specifies interfaces that are not compatible with the class.
        /// </summary>
        ProhibitedInterfaces,

        /// <summary>
        /// Used by BlueprintFunctionLibrary classes to restrict the graphs the functions in the library can be used in to the classes specified.
        /// </summary>
        RestrictedToClasses,

        /// <summary>
        /// Indicates that when placing blueprint nodes in graphs owned by this class that the hidden world context pin should be visible because the self context of the class cannot
        /// provide the world context and it must be wired in manually
        /// </summary>
        ShowWorldContextPin,

        /// <summary>
        /// Do not spawn an object of the class using Generic Create Object node in Blueprint. It makes sense only for a BluprintType class, that is neither Actor, nor ActorComponent.
        /// </summary>
        DontUseGenericSpawnObject,

        /// <summary>
        /// Expose a proxy object of this class in Async Task node.
        /// </summary>
        ExposedAsyncProxy,

        /// <summary>
        /// Only valid on Blueprint Function Libraries. Mark the functions in this class as callable on non-game threads in an Animation Blueprint.
        /// </summary>
        BlueprintThreadSafe,

        /// <summary>
        /// Indicates the class uses hierarchical data. Used to instantiate hierarchical editing features in details panels
        /// </summary>
        UsesHierarchy,

        //////////////////////////////////////////////////////////////////////////////////////////
        // These are specified as UCLASS "keywords" but are metadata
        //////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// This keyword is used to set the actor group that the class is show in, in the editor.
        /// </summary>
        ClassGroupNames,// Specified by "classGroup"

        /// <summary>
        /// Exposes this class as a type that can be used for variables in blueprints
        /// </summary>
        BlueprintType,

        /// <summary>
        /// Prevents this class from being used for variables in blueprints
        /// </summary>
        NotBlueprintType,

        /// <summary>
        /// Exposes this class as an acceptable base class for creating blueprints. The default is NotBlueprintable, unless inherited otherwise. This is inherited by subclasses.
        /// </summary>
        Blueprintable,

        /// <summary>
        /// Specifies that this class is *NOT* an acceptable base class for creating blueprints. The default is NotBlueprintable, unless inherited otherwise. This is inherited by subclasses.
        /// </summary>
        NotBlueprintable,

        /// <summary>
        /// Shows the specified categories in a property viewer. Usage: showCategories=CategoryName or showCategories=(category0, category1, ...)
        /// </summary>
        ShowCategories,

        /// <summary>
        /// Hides the specified categories in a property viewer. Usage: hideCategories=CategoryName or hideCategories=(category0, category1, ...)
        /// </summary>
        HideCategories,

        /// <summary>
        /// Hides the specified function in a property viewer. Usage: hideFunctions=FunctionName or hideFunctions=(category0, category1, ...)
        /// </summary>
        HideFunctions,// Specified by both "ShowFunctions" and "HideFunctions"

        /// <summary>
        /// Specifies which categories should be automatically expanded in a property viewer.
        /// </summary>
        AutoExpandCategories,

        /// <summary>
        /// Specifies which categories should be automatically collapsed in a property viewer.
        /// </summary>
        AutoCollapseCategories,// Specified by both "AutoCollapseCategories" and "DontAutoCollapseCategories"

        /// <summary>
        /// A root convert limits a sub-class to only be able to convert to child classes of the first root class going up the hierarchy.
        /// </summary>
        IsConversionRoot,// Specified by "ConversionRoot"

        /// <summary>
        /// Marks this class as 'experimental' (a totally unsupported and undocumented prototype)
        /// </summary>
        Experimental,

        /// <summary>
        /// Marks this class as an 'early access' preview (while not considered production-ready, it's a step beyond 'experimental' and is being provided as a preview of things to come)
        /// </summary>
        EarlyAccessPreview,

        //////////////////////////////////////////////////////////////////////////////////////////
        // These are specified as UCLASS "keywords" (but aren't actually metadata, we use these internall)
        //////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Shows the specified function in a property viewer. Usage: showFunctions=FunctionName or showFunctions=(category0, category1, ...)
        /// </summary>
        ShowFunctions,

        /// <summary>
        /// Clears the list of auto collapse categories.
        /// </summary>
        DontAutoCollapseCategories
    }

    /// <summary>
    /// Metadata usable on fields / properties exposed to Unreal
    /// </summary>
    public enum MDProp
    {
        Unknown,

        /// <summary>
        /// Used for Subclass and SoftClass properties.  Indicates whether abstract class types should be shown in the class picker.
        /// </summary>
        AllowAbstract,

        /// <summary>
        /// Used for FSoftObjectPath properties.  Comma delimited list that indicates the class type(s) of assets to be displayed in the asset picker.
        /// </summary>
        AllowedClasses,

        /// <summary>
        /// Used for FVector properties.  It causes a ratio lock to be added when displaying this property in details panels.
        /// </summary>
        AllowPreserveRatio,

        /// <summary>
        /// Used for integer properties.  Clamps the valid values that can be entered in the UI to be between 0 and the length of the array specified.
        /// </summary>
        ArrayClamp,

        /// <summary>
        /// Used for SoftObjectPtr/SoftObjectPath properties. Comma separated list of Bundle names used inside PrimaryDataAssets to specify which bundles this reference is part of
        /// </summary>
        AssetBundles,

        /// <summary>
        /// Used for Subclass and SoftClass properties.  Indicates whether only blueprint classes should be shown in the class picker.
        /// </summary>
        BlueprintBaseOnly,

        /// <summary>
        /// Property defaults are generated by the Blueprint compiler and will not be copied when CopyPropertiesForUnrelatedObjects is called post-compile.
        /// </summary>
        BlueprintCompilerGeneratedDefaults,

        /// <summary>
        /// Used for float and integer properties.  Specifies the minimum value that may be entered for the property.
        /// </summary>
        ClampMin,

        /// <summary>
        /// Used for float and integer properties.  Specifies the maximum value that may be entered for the property.
        /// </summary>
        ClampMax,

        /// <summary>
        /// Property is serialized to config and we should be able to set it anywhere along the config hierarchy.
        /// </summary>
        ConfigHierarchyEditable,

        /// <summary>
        /// Used by FDirectoryPath properties. Indicates that the path will be picked using the Slate-style directory picker inside the game Content dir.
        /// </summary>
        ContentDir,

        /// <summary>
        /// This property is deprecated, any blueprint references to it cause a compilation warning.
        /// </summary>
        DeprecatedProperty,// added 4.20

        /// <summary>
        /// Used in conjunction with DeprecatedNode, DeprecatedProperty, or DeprecatedFunction to customize the warning message displayed to the user.
        /// </summary>
        DeprecationMessage,// added 4.20 (UProperty, existed before on functions / classes)

        /// <summary>
        /// The name to display for this class, property, or function instead of auto-generating it from the name.
        /// </summary>
        DisplayName,

        /// <summary>
        /// The name to use for this class, property, or function when exporting it to a scripting language. May include deprecated names as additional semi-colon separated entries.
        /// </summary>
        ScriptName,// added 4.19

        /// <summary>
        /// Indicates that the property should be displayed immediately after the property named in the metadata.
        /// </summary>
        DisplayAfter,// added 4.21

        /// <summary>
        /// The relative order within its category that the property should be displayed in where lower values are sorted first..
        /// If used in conjunction with DisplayAfter, specifies the priority relative to other properties with same DisplayAfter specifier.
        /// </summary>
        DisplayPriority,// added 4.21

        /// <summary>
        /// Indicates that the property is an asset type and it should display the thumbnail of the selected asset.
        /// </summary>
        DisplayThumbnail,

        /// <summary>
        /// Specifies a boolean property that is used to indicate whether editing of this property is disabled.
        /// </summary>
        EditCondition,

        /// <summary>
        /// Keeps the elements of an array from being reordered by dragging 
        /// </summary>
        EditFixedOrder,

        /// <summary>
        /// Used for FSoftObjectPath properties in conjunction with AllowedClasses. Indicates whether only the exact classes specified in AllowedClasses can be used or whether subclasses are valid.
        /// </summary>
        ExactClass,

        /// <summary>
        /// Specifies a list of categories whose functions should be exposed when building a function list in the Blueprint Editor.
        /// </summary>
        ExposeFunctionCategories,

        /// <summary>
        /// Specifies whether the property should be exposed on a Spawn Actor for the class type.
        /// </summary>
        ExposeOnSpawn,

        /// <summary>
        /// Used by FFilePath properties. Indicates the path filter to display in the file picker.
        /// </summary>
        FilePathFilter,

        /// <summary>
        /// Used by FFilePath properties. Indicates that the FilePicker dialog will output a path relative to the game directory when setting the property. An absolute path will be used when outside the game directory.
        /// </summary>
        RelativeToGameDir,// added 4.22

        /// <summary>
        /// Deprecated
        /// </summary>
        FixedIncrement,

        /// <summary>
        /// Used for FColor and FLinearColor properties. Indicates that the Alpha property should be hidden when displaying the property widget in the details.
        /// </summary>
        HideAlphaChannel,

        /// <summary>
        /// Used for Subclass and SoftClass properties. Specifies to hide the ability to change view options in the class picker
        /// </summary>
        HideViewOptions,

        /// <summary>
        /// Used for bypassing property initialization tests when the property cannot be safely tested in a deterministic fashion. Example: random numbers, guids, etc.
        /// </summary>
        IgnoreForMemberInitializationTest,// added 4.21

        /// <summary>
        /// Signifies that the bool property is only displayed inline as an edit condition toggle in other properties, and should not be shown on its own row.
        /// </summary>
        InlineEditConditionToggle,

        /// <summary>
        /// Used by FDirectoryPath properties.  Converts the path to a long package name
        /// </summary>
        LongPackageName,

        /// <summary>
        /// Used for Transform/Rotator properties (also works on arrays of them). Indicates that the property should be exposed in the viewport as a movable widget.
        /// </summary>
        MakeEditWidget,

        /// <summary>
        /// For properties in a structure indicates the default value of the property in a blueprint make structure node.
        /// </summary>
        MakeStructureDefaultValue,

        /// <summary>
        /// Used FSoftClassPath properties. Indicates the parent class that the class picker will use when filtering which classes to display.
        /// </summary>
        MetaClass,

        /// <summary>
        /// Used for Subclass and SoftClass properties. Indicates the selected class must implement a specific interface
        /// </summary>
        MustImplement,

        /// <summary>
        /// Used for numeric properties. Stipulates that the value must be a multiple of the metadata value.
        /// </summary>
        Multiple,

        /// <summary>
        /// Used for FString and FText properties.  Indicates that the edit field should be multi-line, allowing entry of newlines.
        /// </summary>
        MultiLine,

        /// <summary>
        /// Used for FString and FText properties.  Indicates that the edit field is a secret field (e.g a password) and entered text will be replaced with dots. Do not use this as your only security measure.  The property data is still stored as plain text. 
        /// </summary>
        PasswordField,

        /// <summary>
        /// Used for array properties. Indicates that the duplicate icon should not be shown for entries of this array in the property panel.
        /// </summary>
        NoElementDuplicate,

        /// <summary>
        /// Property wont have a 'reset to default' button when displayed in property windows
        /// </summary>
        NoResetToDefault,

        /// <summary>
        /// Used for integer and float properties. Indicates that the spin box element of the number editing widget should not be displayed.
        /// </summary>
        NoSpinbox,

        /// <summary>
        /// Used for Subclass properties. Indicates whether only placeable classes should be shown in the class picker.
        /// </summary>
        OnlyPlaceable,

        /// <summary>
        /// Used by FDirectoryPath properties. Indicates that the directory dialog will output a relative path when setting the property.
        /// </summary>
        RelativePath,

        /// <summary>
        /// Used by FDirectoryPath properties. Indicates that the directory dialog will output a path relative to the game content directory when setting the property.
        /// </summary>
        RelativeToGameContentDir,

        /// <summary>
        /// Flag set on a property or function to prevent it being exported to a scripting language.
        /// </summary>
        ScriptNoExport,// added 4.19

        /// <summary>
        /// Used by struct properties. Indicates that the inner properties will not be shown inside an expandable struct, but promoted up a level.
        /// </summary>
        ShowOnlyInnerProperties,

        /// <summary>
        /// Used for Subclass and SoftClass properties. Shows the picker as a tree view instead of as a list
        /// </summary>
        ShowTreeView,

        /// <summary>
        /// Used by numeric properties. Indicates how rapidly the value will grow when moving an unbounded slider.
        /// </summary>
        SliderExponent,

        /// <summary>
        /// Used by arrays of structs. Indicates a single property inside of the struct that should be used as a title summary when the array entry is collapsed.
        /// </summary>
        TitleProperty,

        /// <summary>
        /// Used for float and integer properties.  Specifies the lowest that the value slider should represent.
        /// </summary>
        UIMin,

        /// <summary>
        /// Used for float and integer properties.  Specifies the highest that the value slider should represent.
        /// </summary>
        UIMax,

        /// <summary>
        /// Used for SoftObjectPtr/SoftObjectPath properties to specify a reference should not be tracked. This reference will not be automatically cooked or saved into the asset registry for redirector/delete fixup.
        /// </summary>
        Untracked,// added 4.21

        //////////////////////////////////////////////////////////////////////////////////////////
        // Metadata usable in UPROPERTY for customizing the behavior of Persona and UMG
        //////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The property is not exposed as a data pin and is only be editable in the details panel. Applicable only to properties that will be displayed in Persona and UMG.
        /// </summary>
        NeverAsPin,

        /// <summary>
        /// The property can be exposed as a data pin, but is hidden by default. Applicable only to properties that will be displayed in Persona and UMG.
        /// </summary>
        PinHiddenByDefault,

        /// <summary>
        /// The property can be exposed as a data pin and is visible by default. Applicable only to properties that will be displayed in Persona and UMG.
        /// </summary>
        PinShownByDefault,

        /// <summary>
        /// The property is always exposed as a data pin. Applicable only to properties that will be displayed in Persona and UMG.
        /// </summary>
        AlwaysAsPin,

        /// <summary>
        /// Indicates that the property has custom code to display and should not generate a standard property widget int he details panel. Applicable only to properties that will be displayed in Persona.
        /// </summary>
        CustomizeProperty,

        //////////////////////////////////////////////////////////////////////////////////////////
        // Metadata usable in UPROPERTY for customizing the behavior of Material Expressions
        //////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Used for float properties in MaterialExpression classes. If the specified FMaterialExpression pin is not connected, this value is used instead.
        /// </summary>
        OverridingInputProperty,

        /// <summary>
        /// Used for FMaterialExpression properties in MaterialExpression classes. If specified the pin need not be connected and the value of the property marked as OverridingInputProperty will be used instead.
        /// </summary>
        RequiredInput,

        //////////////////////////////////////////////////////////////////////////////////////////
        // These are specified as UPROPERTY "keywords" but are metadata
        //////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Specifies the category of the property. Usage: Category=CategoryName.
        /// </summary>
        Category,

        // This will also flag CPF_BlueprintVisible
        /// <summary>
        /// This property has an accessor to return the value. Implies BlueprintReadOnly if BlueprintSetter or BlueprintReadWrite is not specified. (usage: BlueprintGetter=FunctionName).
        /// </summary>
        BlueprintGetter,

        // Defined by BlueprintReadWrite, also sets CPF_BlueprintVisible
        /// <summary>
        /// If true, properties defined in the C++ private scope will be accessible to blueprints
        /// </summary>
        AllowPrivateAccess,

        // This will also flag CPF_BlueprintVisible
        /// <summary>
        /// This property has an accessor to set the value. Implies BlueprintReadWrite. (usage: BlueprintSetter=FunctionName).
        /// </summary>
        BlueprintSetter,

        //////////////////////////////////////////////////////////////////////////////////////////
        // These aren't declared / commented in the main list
        //////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Defined in AddEditInlineMetaData() (Engine\Source\Programs\UnrealHeaderTool\Private\HeaderParser.cpp)
        /// </summary>
        EditInline,

        //////////////////////////////////////////////////////////////////////////////////////////
        // Blueprint only? Defined in Engine\Source\Editor\BlueprintGraph\Classes\EdGraphSchema_K2.h
        //////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// This property cannot be modified by other blueprints
        /// </summary>
        BlueprintPrivate
    }

    /// <summary>
    /// Metadata usable on functions exposed to Unreal
    /// </summary>
    public enum MDFunc
    {
        Unknown,

        /// <summary>
        /// Used with a comma-separated list of parameter names that should show up as advanced pins (requiring UI expansion).
        /// Alternatively you can set a number, which is the number of paramaters from the start that should *not* be marked as advanced (eg 'AdvancedDisplay="2"' will mark all but the first two advanced).
        /// </summary>
        AdvancedDisplay,

        /// <summary>
        /// Indicates that a BlueprintCallable function should use a Call Array Function node and that the parameters specified in the comma delimited list should be treated as wild card array properties.
        /// </summary>
        ArrayParm,

        /// <summary>
        /// Used when ArrayParm has been specified to indicate other function parameters that should be treated as wild card properties linked to the type of the array parameter.
        /// </summary>
        ArrayTypeDependentParams,

        /// <summary>
        /// The listed parameters, although passed by reference, will have an automatically-created default if their pins are left disconnected. This is a convenience feature for Blueprints.
        /// </summary>
        AutoCreateRefTerm,

        /// <summary>
        /// This function is an internal implementation detail, used to implement another function or node. It is never directly exposed in a graph.
        /// </summary>
        BlueprintInternalUseOnly,

        /// <summary>
        /// This function can only be called on 'this' in a blueprint. It cannot be called on another instance.
        /// </summary>
        BlueprintProtected,

        /// <summary>
        /// Used for BlueprintCallable functions that have a WorldContext pin to indicate that the function can be called even if the class does not implement the virtual function GetWorld().
        /// </summary>
        CallableWithoutWorldContext,

        /// <summary>
        /// Indicates that a BlueprintCallable function should use the Commutative Associative Binary node.
        /// </summary>
        CommutativeAssociativeBinaryOperator,

        /// <summary>
        /// Indicates that a BlueprintCallable function should display in the compact display mode and the name to use in that mode.
        /// </summary>
        CompactNodeTitle,

        /// <summary>
        /// The listed parameters are all treated as wildcards. This specifier requires the  UFUNCTION-level specifier,  CustomThunk, which will require the user to provide a custom exec function. In this function, the parameter types can be checked and the appropriate function calls can be made based on those parameter types. The base UFUNCTION should never be called, and should assert or log an error if it is.
        /// </summary>
        CustomStructureParam,

        /// <summary>
        /// For BlueprintCallable functions indicates that the object property named's default value should be the self context of the node
        /// </summary>
        DefaultToSelf,

        /// <summary>
        /// This function is deprecated, any blueprint references to it cause a compilation warning.
        /// </summary>
        DeprecatedFunction,

        /// <summary>
        /// Used in conjunction with DeprecatedNode or DeprecatedFunction to customize the warning message displayed to the user.
        /// </summary>
        DeprecationMessage,

        /// <summary>
        /// For BlueprintCallable functions indicates that an input exec pin should be created for each entry in the enum specified.
        /// </summary>
        ExpandEnumAsExecs,

        /// <summary>
        /// The name to display for this class, property, or function instead of auto-generating it from the name.
        /// </summary>
        DisplayName,

        /// <summary>
        /// The name to use for this class, property, or function when exporting it to a scripting language.
        /// </summary>
        ScriptName,// added 4.19

        /// <summary>
        /// Flag set on a property or function to prevent it being exported to a scripting language.
        /// </summary>
        ScriptNoExport,// added 4.19

        /// <summary>
        /// Flags a static function taking a struct or or object as its first argument so that it "hoists" the function to be a method of the struct or class when exporting it to a scripting language.
        /// The value is optional, and may specify a name override for the method. May include deprecated names as additional semi-colon separated entries.
        /// </summary>
        ScriptMethod,// added 4.20

        /// <summary>
        /// Used with ScriptMethod to denote that the return value of the function should overwrite the value of the instance that made the call (structs only, equivalent to using UPARAM(self) on the struct argument).
        /// </summary>
        ScriptMethodSelfReturn,// added 4.20

        /// <summary>
        /// Flags a static function taking a struct as its first argument so that it "hoists" the function to be an operator of the struct when exporting it to a scripting language.
        /// The value describes the kind of operator using C++ operator syntax (see below), and may contain multiple semi-colon separated values.
        /// The signature of the function depends on the operator type, and additional parameters may be passed as long as they're defaulted and the basic signature requirements are met.
        /// - For the bool conversion operator (bool) the signature must be:
        ///     bool FuncName(const FMyStruct&amp;); // FMyStruct may be passed by value rather than const-ref
        /// - For the unary conversion operators (neg(-obj)) the signature must be:
        ///     FMyStruct FuncName(const FMyStruct&); // FMyStruct may be passed by value rather than const-ref
        /// - For comparion operators (==, !=, &lt;, &lt;=, >, >=) the signature must be:
        ///     bool FuncName(const FMyStruct, OtherType); // OtherType can be any type, FMyStruct may be passed by value rather than const-ref
        /// - For mathematical operators (+, -, *, /, %, &amp;, |, ^, >>, &lt;&lt;) the signature must be:
        ///     ReturnType FuncName(const FMyStruct&amp;, OtherType); // ReturnType and OtherType can be any type, FMyStruct may be passed by value rather than const-ref
        /// - For mathematical assignment operators (+=, -=, *=, /=, %=, &amp;=, |=, ^=, >>=, &lt;&lt;=) the signature must be:
        ///     FMyStruct FuncName(const FMyStruct&amp;, OtherType); // OtherType can be any type, FMyStruct may be passed by value rather than const-ref
        /// </summary>
        ScriptOperator,// added 4.20

        /// <summary>
        /// Flags a static function returning a value so that it "hoists" the function to be a constant of its host type when exporting it to a scripting language.
        /// The constant will be hosted on the class that owns the function, but ScriptConstantHost can be used to host it on a different type (struct or class).
        /// The value is optional, and may specify a name override for the constant. May include deprecated names as additional semi-colon separated entries.
        /// </summary>
        ScriptConstant,// added 4.20

        /// <summary>
        /// Used with ScriptConstant to override the host type for a constant. Should be the name of a struct or class with no prefix, eg) Vector2D or Actor
        /// </summary>
        ScriptConstantHost,// added 4.20

        /// <summary>
        /// For BlueprintCallable functions indicates that the parameter pin should be hidden from the user's view.
        /// </summary>
        HidePin,

        /// <summary>
        /// 
        /// </summary>
        HideSpawnParms,

        /// <summary>
        /// For BlueprintCallable functions provides additional keywords to be associated with the function for search purposes.
        /// </summary>
        Keywords,

        /// <summary>
        /// Indicates that a BlueprintCallable function is Latent
        /// </summary>
        Latent,

        /// <summary>
        /// For Latent BlueprintCallable functions indicates which parameter is the LatentInfo parameter
        /// </summary>
        LatentInfo,

        /// <summary>
        /// For BlueprintCallable functions indicates that the material override node should be used
        /// </summary>
        MaterialParameterCollectionFunction,

        /// <summary>
        /// For BlueprintCallable functions indicates that the function should be displayed the same as the implicit Break Struct nodes
        /// </summary>
        NativeBreakFunc,

        /// <summary>
        /// For BlueprintCallable functions indicates that the function should be displayed the same as the implicit Make Struct nodes
        /// </summary>
        NativeMakeFunc,

        /// <summary>
        /// Used by BlueprintCallable functions to indicate that this function is not to be allowed in the Construction Script.
        /// </summary>
        UnsafeDuringActorConstruction,

        /// <summary>
        /// Used by BlueprintCallable functions to indicate which parameter is used to determine the World that the operation is occurring within.
        /// </summary>
        WorldContext,

        /// <summary>
        /// Used only by static BlueprintPure functions from BlueprintLibrary. A cast node will be automatically added for the return type and the type of the first parameter of the function.
        /// </summary>
        BlueprintAutocast,

        /// <summary>
        /// Only valid in Blueprint Function Libraries. Mark this function as an exception to the class's general BlueprintThreadSafe metadata.
        /// </summary>
        NotBlueprintThreadSafe,

        //////////////////////////////////////////////////////////////////////////////////////////
        // These don't appear in the main list of function meta data
        //////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// If true, the self pin should not be shown or connectable regardless of purity, const, etc. similar to InternalUseParam
        /// </summary>
        HideSelfPin,

        /// <summary>
        /// Indicates that a particular function parameter is for internal use only, which means it will be both hidden and not connectible.
        /// </summary>
        InternalUseParam,

        //////////////////////////////////////////////////////////////////////////////////////////
        // These are specified as UFUNCTION / UDELEGATE "keywords" but are metadata
        //////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// This function is used as the get accessor for a blueprint exposed property. Implies BlueprintPure and BlueprintCallable.
        /// </summary>
        BlueprintGetter,

        /// <summary>
        /// This function is used as the set accessor for a blueprint exposed property. Implies BlueprintCallable.
        /// </summary>
        BlueprintSetter,

        /// <summary>
        /// This function can be called in the editor on selected instances via a button in the details panel.
        /// </summary>
        CallInEditor,

        /// <summary>
        /// Specifies the category of the function when displayed in blueprint editing tools.
        /// Usage: Category=CategoryName or Category="MajorCategory,SubCategory"
        /// </summary>
        Category,
    }

    public static class MetadataExtensions
    {
        public static bool HasMetaData<TEnum>(this UField field, TEnum key) where TEnum : struct, Enum
        {
            return field.HasMetaData(UMeta.GetKey(key));
        }        

        public static string GetMetaData<TEnum>(this UField field, TEnum key) where TEnum : struct, Enum
        {
            return field.GetMetaData(UMeta.GetKey(key));
        }

        public static void SetMetaData<TEnum, T>(this UField field, TEnum key, T value) where TEnum : struct, Enum
        {
            string valueStr = null;
            UClass unrealClass = value as UClass;
            if (unrealClass != null)
            {
                valueStr = unrealClass.GetPathName();
            }
            else
            {
                valueStr = value.ToString();
            }

            field.SetMetaData(UMeta.GetKey(key), valueStr);
        }

        public static bool GetBoolMetaData<TEnum>(this UField field, TEnum key) where TEnum : struct, Enum
        {
            return field.GetBoolMetaData(UMeta.GetKey(key));
        }

        public static int GetIntMetaData<TEnum>(this UField field, TEnum key) where TEnum : struct, Enum
        {
            return field.GetIntMetaData(UMeta.GetKey(key));
        }

        public static float GetFloatMetaData<TEnum>(this UField field, TEnum key) where TEnum : struct, Enum
        {
            return field.GetFloatMetaData(UMeta.GetKey(key));
        }

        public static UClass GetClassMetaData<TEnum>(this UField field, TEnum key) where TEnum : struct, Enum
        {
            return field.GetClassMetaData(UMeta.GetKey(key));
        }

        public static bool GetBoolMetaDataHierarchical<TEnum>(this UStruct unrealStruct, TEnum key) where TEnum : struct, Enum
        {
            return unrealStruct.GetBoolMetaDataHierarchical(new FName(UMeta.GetKey(key)));
        }

        public static bool GetStringMetaDataHierarchical<TEnum>(this UStruct unrealStruct, TEnum key, out string outValue) where TEnum : struct, Enum
        {
            return unrealStruct.GetStringMetaDataHierarchical(new FName(UMeta.GetKey(key)), out outValue);
        }
    }
}

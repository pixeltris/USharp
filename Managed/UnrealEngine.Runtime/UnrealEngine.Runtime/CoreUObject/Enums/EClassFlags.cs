using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\CoreUObject\Public\UObject\ObjectMacros.h

    /// <summary>
    /// Flags describing a class.
    /// </summary>
    [Flags]
    public enum EClassFlags : uint
    {
        None = 0x00000000,

        /// <summary>
        /// Class is abstract and can't be instantiated directly.
        /// </summary>
        Abstract = 0x00000001,

        /// <summary>
        /// Save object configuration only to Default INIs, never to local INIs. Must be combined with CLASS_Config
        /// </summary>
        DefaultConfig = 0x00000002,

        /// <summary>
        /// Load object configuration at construction time.
        /// </summary>
        Config = 0x00000004,

        /// <summary>
        /// This object type can't be saved; null it out at save time.
        /// </summary>
        Transient = 0x00000008,

        /// <summary>
        /// Successfully parsed.
        /// </summary>
        Parsed = 0x00000010,

        MatchedSerializers = 0x00000020,// added 4.20

        /// <summary>
        /// All the properties on the class are shown in the advanced section (which is hidden by default) unless SimpleDisplay is specified on the property
        /// </summary>
        AdvancedDisplay = 0x00000040,

        /// <summary>
        /// Class is a native class - native interfaces will have CLASS_Native set, but not RF_MarkAsNative
        /// </summary>
        Native = 0x00000080,

        /// <summary>
        /// Don't export to C++ header.
        /// </summary>
        NoExport = 0x00000100,

        /// <summary>
        /// Do not allow users to create in the editor.
        /// </summary>
        NotPlaceable = 0x00000200,

        /// <summary>
        /// Handle object configuration on a per-object basis, rather than per-class.
        /// </summary>
        PerObjectConfig = 0x00000400,

        /// <summary>
        /// Whether SetUpRuntimeReplicationData still needs to be called for this class
        /// </summary>
        ReplicationDataIsSetUp = 0x00000800,// 4.16:PointersDefaultToWeak, 4.17:removed, 4.20:ReplicationDataIsSetUp

        /// <summary>
        /// Class can be constructed from editinline New button.
        /// </summary>
        EditInlineNew = 0x00001000,

        /// <summary>
        /// Display properties in the editor without using categories.
        /// </summary>
        CollapseCategories = 0x00002000,

        /// <summary>
        /// Class is an interface
        /// </summary>
        Interface = 0x00004000,

        /// <summary>
        /// Do not export a constructor for this class, assuming it is in the cpptext
        /// </summary>
        CustomConstructor = 0x00008000,

        /// <summary>
        /// all properties and functions in this class are const and should be exported as const
        /// </summary>
        Const = 0x00010000,

        ///// <summary>
        ///// Class flag indicating the class is having its layout changed, and therefore is not ready for a CDO to be created
        ///// </summary>
        //LayoutChanging = 0x00020000,// 4.16:PointersDefaultToAutoWeak, 4.17:removed, 4.21:LayoutChanging

        /// <summary>
        /// Indicates that the class was created from blueprint source material
        /// </summary>
        CompiledFromBlueprint = 0x00040000,

        /// <summary>
        /// Indicates that only the bare minimum bits of this class should be DLL exported/imported
        /// </summary>
        MinimalAPI = 0x00080000,

        /// <summary>
        /// Indicates this class must be DLL exported/imported (along with all of it's members)
        /// </summary>
        RequiredAPI = 0x00100000,

        /// <summary>
        /// Indicates that references to this class default to instanced. Used to be subclasses of UComponent, but now can be any UObject
        /// </summary>
        DefaultToInstanced = 0x00200000,

        /// <summary>
        /// Indicates that the parent token stream has been merged with ours.
        /// </summary>
        TokenStreamAssembled = 0x00400000,

        /// <summary>
        /// Class has component properties.
        /// </summary>
        HasInstancedReference = 0x00800000,

        /// <summary>
        /// Don't show this class in the editor class browser or edit inline new menus.
        /// </summary>
        Hidden = 0x01000000,

        /// <summary>
        /// Don't save objects of this class when serializing
        /// </summary>
        Deprecated = 0x02000000,

        /// <summary>
        /// Class not shown in editor drop down for class selection
        /// </summary>
        HideDropDown = 0x04000000,

        /// <summary>
        /// Class settings are saved to <AppData>/..../Blah.ini (as opposed to CLASS_DefaultConfig)
        /// </summary>
        GlobalUserConfig = 0x08000000,

        /// <summary>
        /// Class was declared directly in C++ and has no boilerplate generated by UnrealHeaderTool
        /// </summary>
        Intrinsic = 0x10000000,

        /// <summary>
        /// Class has already been constructed (maybe in a previous DLL version before hot-reload).
        /// </summary>
        Constructed = 0x20000000,

        /// <summary>
        /// Indicates that object configuration will not check against ini base/defaults when serialized
        /// </summary>
        ConfigDoNotCheckDefaults = 0x40000000,

        /// <summary>
        /// Class has been consigned to oblivion as part of a blueprint recompile, and a newer version currently exists.
        /// </summary>
        NewerVersionExists = 0x80000000,

        /// <summary>
        /// Flags to inherit from base class
        /// </summary>
        Inherit = Transient | DefaultConfig | Config | PerObjectConfig | ConfigDoNotCheckDefaults | NotPlaceable
                | Const | HasInstancedReference | Deprecated | DefaultToInstanced | GlobalUserConfig,

        /// <summary>
        /// these flags will be cleared by the compiler when the class is parsed during script compilation
        /// </summary>
        RecompilerClear = Inherit | Abstract | NoExport | Native | Intrinsic | TokenStreamAssembled,

        /// <summary>
        /// these flags will be cleared by the compiler when the class is parsed during script compilation
        /// </summary>
        ShouldNeverBeLoaded = Native | Intrinsic | TokenStreamAssembled,

        /// <summary>
        /// these flags will be inherited from the base class only for non-intrinsic classes
        /// </summary>
        ScriptInherit = Inherit | EditInlineNew | CollapseCategories,

        /// <summary>
        /// This is used as a mask for the flags put into generated code for "compiled in" classes.
        /// </summary>
        SaveInCompiledInClasses =
            Abstract |
            DefaultConfig |
            GlobalUserConfig |
            Config |
            Transient |
            Native |
            NotPlaceable |
            PerObjectConfig |
            ConfigDoNotCheckDefaults |
            EditInlineNew |
            CollapseCategories |
            Interface |
            DefaultToInstanced |
            HasInstancedReference |
            Hidden |
            Deprecated |
            HideDropDown |
            Intrinsic |
            AdvancedDisplay |
            Const |
            MinimalAPI |
            RequiredAPI |
            MatchedSerializers,

        AllFlags = 0xFFFFFFFF,
    }
}

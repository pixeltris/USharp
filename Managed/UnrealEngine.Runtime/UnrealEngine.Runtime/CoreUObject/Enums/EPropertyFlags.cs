using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    [Flags]
    public enum EPropertyFlags : ulong
    {
        /// <summary>
        /// Property is user-settable in the editor.
        /// </summary>
        Edit = 0x0000000000000001,

        /// <summary>
        /// This is a constant function parameter
        /// </summary>
        ConstParm = 0x0000000000000002,

        /// <summary>
        /// This property can be read by blueprint code
        /// </summary>
        BlueprintVisible = 0x0000000000000004,

        /// <summary>
        /// Object can be exported with actor.
        /// </summary>
        ExportObject = 0x0000000000000008,

        /// <summary>
        /// This property cannot be modified by blueprint code
        /// </summary>
        BlueprintReadOnly = 0x0000000000000010,

        /// <summary>
        /// Property is relevant to network replication.
        /// </summary>
        Net = 0x0000000000000020,

        /// <summary>
        /// Indicates that elements of an array can be modified, but its size cannot be changed.
        /// </summary>
        EditFixedSize = 0x0000000000000040,

        /// <summary>
        /// Function/When call parameter.
        /// </summary>
        Parm = 0x0000000000000080,

        /// <summary>
        /// Value is copied out after function call.
        /// </summary>
        OutParm = 0x0000000000000100,

        /// <summary>
        /// memset is fine for construction
        /// </summary>
        ZeroConstructor = 0x0000000000000200,

        /// <summary>
        /// Return value.
        /// </summary>
        ReturnParm = 0x0000000000000400,

        /// <summary>
        /// Disable editing of this property on an archetype/sub-blueprint
        /// </summary>
        DisableEditOnTemplate = 0x0000000000000800,

        //#define CPF_ = 0x0000000000001000

        /// <summary>
        /// Property is transient: shouldn't be saved, zero-filled at load time.
        /// </summary>
        Transient = 0x0000000000002000,

        /// <summary>
        /// Property should be loaded/saved as permanent profile.
        /// </summary>
        Config = 0x0000000000004000,

        //#define CPF_ = 0x0000000000008000

        /// <summary>
        /// Disable editing on an instance of this class
        /// </summary>
        DisableEditOnInstance = 0x0000000000010000,

        /// <summary>
        /// Property is uneditable in the editor.
        /// </summary>
        EditConst = 0x0000000000020000,

        /// <summary>
        /// Load config from base class, not subclass.
        /// </summary>
        GlobalConfig = 0x0000000000040000,

        /// <summary>
        /// Property is a component references.
        /// </summary>
        InstancedReference = 0x0000000000080000,

        //#define CPF_ 0x0000000000100000

        /// <summary>
        /// Property should always be reset to the default value during any type of duplication (copy/paste, binary duplication, etc.)
        /// </summary>
        DuplicateTransient = 0x0000000000200000,

        /// <summary>
        /// Property contains subobject references (TSubobjectPtr)
        /// </summary>
        SubobjectReference = 0x0000000000400000,

        //#define CPF_ 0x0000000000800000

        /// <summary>
        /// Property should be serialized for save games
        /// </summary>
        SaveGame = 0x0000000001000000,

        /// <summary>
        /// Hide clear (and browse) button.
        /// </summary>
        NoClear = 0x0000000002000000,

        //#define CPF_ 0x0000000004000000

        /// <summary>
        /// Value is passed by reference; CPF_OutParam and CPF_Param should also be set.
        /// </summary>
        ReferenceParm = 0x0000000008000000,

        /// <summary>
        /// MC Delegates only.  Property should be exposed for assigning in blueprint code
        /// </summary>
        BlueprintAssignable = 0x0000000010000000,

        /// <summary>
        /// Property is deprecated.  Read it from an archive, but don't save it.
        /// </summary>
        Deprecated = 0x0000000020000000,

        /// <summary>
        /// If this is set, then the property can be memcopied instead of CopyCompleteValue / CopySingleValue
        /// </summary>
        IsPlainOldData = 0x0000000040000000,

        /// <summary>
        /// Not replicated. For non replicated properties in replicated structs 
        /// </summary>
        RepSkip = 0x0000000080000000,

        /// <summary>
        /// Notify actors when a property is replicated
        /// </summary>
        RepNotify = 0x0000000100000000,

        /// <summary>
        /// interpolatable property for use with matinee
        /// </summary>
        Interp = 0x0000000200000000,

        /// <summary>
        /// Property isn't transacted
        /// </summary>
        NonTransactional = 0x0000000400000000,

        /// <summary>
        /// Property should only be loaded in the editor
        /// </summary>
        EditorOnly = 0x0000000800000000,

        /// <summary>
        /// No destructor
        /// </summary>
        NoDestructor = 0x0000001000000000,

        //#define CPF_ = 0x0000002000000000

        /// <summary>
        /// Only used for weak pointers, means the export type is autoweak
        /// </summary>
        AutoWeak = 0x0000004000000000,

        /// <summary>
        /// Property contains component references.
        /// </summary>
        ContainsInstancedReference = 0x0000008000000000,

        /// <summary>
        /// asset instances will add properties with this flag to the asset registry automatically
        /// </summary>
        AssetRegistrySearchable = 0x0000010000000000,

        /// <summary>
        /// The property is visible by default in the editor details view
        /// </summary>
        SimpleDisplay = 0x0000020000000000,

        /// <summary>
        /// The property is advanced and not visible by default in the editor details view
        /// </summary>
        AdvancedDisplay = 0x0000040000000000,

        /// <summary>
        /// property is protected from the perspective of script
        /// </summary>
        Protected = 0x0000080000000000,

        /// <summary>
        /// MC Delegates only.  Property should be exposed for calling in blueprint code
        /// </summary>
        BlueprintCallable = 0x0000100000000000,

        /// <summary>
        /// MC Delegates only.  This delegate accepts (only in blueprint) only events with BlueprintAuthorityOnly.
        /// </summary>
        BlueprintAuthorityOnly = 0x0000200000000000,

        /// <summary>
        /// Property shouldn't be exported to text format (e.g. copy/paste)
        /// </summary>
        TextExportTransient = 0x0000400000000000,

        /// <summary>
        /// Property should only be copied in PIE
        /// </summary>
        NonPIEDuplicateTransient = 0x0000800000000000,

        /// <summary>
        /// Property is exposed on spawn
        /// </summary>
        ExposeOnSpawn = 0x0001000000000000,

        /// <summary>
        /// A object referenced by the property is duplicated like a component. (Each actor should have an own instance.)
        /// </summary>
        PersistentInstance = 0x0002000000000000,

        /// <summary>
        /// Property was parsed as a wrapper class like TSubobjectOf&lt;T>, FScriptInterface etc., rather than a USomething*
        /// </summary>
        UObjectWrapper = 0x0004000000000000,

        /// <summary>
        /// This property can generate a meaningful hash value.
        /// </summary>
        HasGetValueTypeHash = 0x0008000000000000,

        /// <summary>
        /// Public native access specifier
        /// </summary>
        NativeAccessSpecifierPublic = 0x0010000000000000,

        /// <summary>
        /// Protected native access specifier
        /// </summary>
        NativeAccessSpecifierProtected = 0x0020000000000000,

        /// <summary>
        /// Private native access specifier
        /// </summary>
        NativeAccessSpecifierPrivate = 0x0040000000000000,

        /// <summary>
        /// Property shouldn't be serialized, can still be exported to text
        /// </summary>
        SkipSerialization = 0x0080000000000000,

        NativeAccessSpecifiers = NativeAccessSpecifierPublic | NativeAccessSpecifierProtected | NativeAccessSpecifierPrivate,

        ParmFlags = Parm | OutParm | ReturnParm | ReferenceParm | ConstParm,
        PropagateToArrayInner = ExportObject | PersistentInstance | InstancedReference | ContainsInstancedReference | Config | EditConst | Deprecated | EditorOnly | AutoWeak | UObjectWrapper,
        PropagateToMapValue = ExportObject | PersistentInstance | InstancedReference | ContainsInstancedReference | Config | EditConst | Deprecated | EditorOnly | AutoWeak | UObjectWrapper | Edit,
        PropagateToMapKey = ExportObject | PersistentInstance | InstancedReference | ContainsInstancedReference | Config | EditConst | Deprecated | EditorOnly | AutoWeak | UObjectWrapper | Edit,

        /// <summary>
        /// the flags that should never be set on interface properties
        /// </summary>
        InterfaceClearMask = ExportObject | InstancedReference | ContainsInstancedReference,

        /// <summary>
        /// all the properties that can be stripped for final release console builds
        /// </summary>
        DevelopmentAssets = EditorOnly,

        /// <summary>
        /// all the properties that should never be loaded or saved
        /// </summary>
        ComputedFlags = IsPlainOldData | NoDestructor | ZeroConstructor,

        AllFlags = 0xFFFFFFFFFFFFFFFF
    }
}

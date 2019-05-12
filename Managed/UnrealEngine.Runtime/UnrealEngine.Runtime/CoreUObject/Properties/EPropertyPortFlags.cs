using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Property exporting flags.
    /// </summary>
    [Flags]
    public enum EPropertyPortFlags
    {
        /// <summary>
        /// No special property exporting flags
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Indicates that property data should be wrapped in quotes (for some types of properties)
        /// </summary>
        Delimited = 0x00000002,

        /// <summary>
        /// Indicates that the object reference should be verified
        /// </summary>
        CheckReferences = 0x00000004,

        ExportsNotFullyQualified = 0x00000008,

        AttemptNonQualifiedSearch = 0x00000010,

        /// <summary>
        /// Indicates that importing values for config properties is disallowed
        /// </summary>
        RestrictImportTypes = 0x00000020,

        /// <summary>
        /// Indicates that this is a blueprint pin or something else that is saved to disk as import text
        /// </summary>
        SerializedAsImportText = 0x00000040,

        /// <summary>
        /// only include properties which are marked CPF_InstancedReference
        /// </summary>
        SubobjectsOnly = 0x00000100,

        /// <summary>
        /// Only applicable to component properties (for now)
        /// Indicates that two object should be considered identical
        /// if the property values for both objects are all identical
        /// </summary>
        DeepComparison = 0x00000200,

        /// <summary>
        /// Similar to PPF_DeepComparison, except that template components are always compared using standard object
        /// property comparison logic (basically if the pointers are different, then the property isn't identical)
        /// </summary>
        DeepCompareInstances = 0x00000400,

        /// <summary>
        /// Set if this operation is copying in memory (for copy/paste) instead of exporting to a file. There are
        /// some subtle differences between the two
        /// </summary>
        Copy = 0x00000800,

        /// <summary>
        /// Set when duplicating objects via serialization
        /// </summary>
        Duplicate = 0x00001000,

        /// <summary>
        /// Indicates that object property values should be exported without the package or class information
        /// </summary>
        SimpleObjectText = 0x00002000,

        /// <summary>
        /// parsing default properties - allow text for transient properties to be imported - also modifies ObjectProperty importing slightly for subobjects
        /// </summary>
        ParsingDefaultProperties = 0x00008000,

        /// <summary>
        /// indicates that non-categorized transient properties should be exported (by default, they would not be)
        /// </summary>
        IncludeTransient = 0x00020000,

        /// <summary>
        /// indicates that we're exporting properties for display in the property window. - used to hide EditHide items in collapsed structs
        /// </summary>
        PropertyWindow = 0x00080000,

        /// <summary>
        /// Force fully qualified object names (for debug dumping)
        /// </summary>
        DebugDump = 0x00200000,

        /// <summary>
        /// Set when duplicating objects for PIE
        /// </summary>
        DuplicateForPIE = 0x00400000,

        /// <summary>
        /// Set when exporting just an object declaration, to be followed by another call with PPF_SeparateDefine
        /// </summary>
        SeparateDeclare = 0x00800000,

        /// <summary>
        /// Set when exporting just an object definition, preceded by another call with PPF_SeparateDeclare
        /// </summary>
        SeparateDefine = 0x01000000,

        /// <summary>
        /// Used by 'watch value' while blueprint debugging
        /// </summary>
        BlueprintDebugView = 0x02000000,

        /// <summary>
        /// Exporting properties for console variables.
        /// </summary>
        ConsoleVariable = 0x04000000,

        /// <summary>
        /// Ignores CPF_Deprecated flag
        /// </summary>
        UseDeprecatedProperties = 0x08000000,

        /// <summary>
        /// Export in C++ form
        /// </summary>
        ExportCpp = 0x10000000,

        /// <summary>
        /// Ignores CPF_SkipSerialization flag when using tagged serialization
        /// </summary>
        ForceTaggedSerialization = 0x20000000,

        /// <summary>
        /// Set when duplicating objects verbatim (doesn't reset unique IDs)
        /// </summary>
        DuplicateVerbatim = 0x40000000
    }
}

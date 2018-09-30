using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Defines the default requirements for types / members to be exposed to unreal
    /// </summary>
    public static class ManagedUnrealVisibility
    {
        public static readonly Type Class = Type.Blueprintable | Type.BlueprintType;
        public static readonly Type Interface = Type.Blueprintable | Type.BlueprintType;
        public static readonly Type Struct = Type.BlueprintType;
        public static readonly Type Enum = Type.BlueprintType;
        public static readonly Member Members = Member.None;//Member.EditorVisible | Member.BlueprintVisible | Member.BlueprintCallable;

        public static readonly Requirement ClassRequirement = Requirement.None;
        public static readonly Requirement StructRequirement = Requirement.MainAttribute;
        public static readonly Requirement EnumRequirement = Requirement.MainAttribute;
        /// <summary>
        /// NOTE: At the moment interfaces MUST have the main attribute, it is checked when calling:
        ///       ManagedUnrealModuleInfo.TryGetClassFlags
        ///       ManagedUnrealTypeInfo.GetTypeCode
        /// </summary>
        public static readonly Requirement InterfaceRequirement = Requirement.MainAttribute;
        public static readonly Requirement DelegateRequirement = Requirement.None;
        public static readonly Requirement FunctionRequirement = Requirement.None;
        public static readonly Requirement PropertyRequirement = Requirement.None;

        public enum Requirement
        {
            /// <summary>
            /// No attributes are required to be exposed to Unreal.
            /// </summary>
            None,

            /// <summary>
            /// The main attribute is required to be exposed to Unreal - 
            /// [UClass] [UEnum] [UProperty] [UFunction] ...
            /// </summary>
            MainAttribute,

            /// <summary>
            /// Any Unreal attribute will result in the given type / member being exposed 
            /// (UMeta / ManagedUnrealAttributeBase inherited)
            /// </summary>
            AnyAttribute
        }

        /// <summary>
        /// States the default editor visibility of C# types (classes, structs, enums, interfaces)
        /// </summary>
        [Flags]
        public enum Type
        {
            /// <summary>
            /// The type has to be exposed to blueprint explicitly (UClass, UStruct, UEnum, UInterface)
            /// </summary>
            None = 0,

            /// <summary>
            /// The type is exposed to blueprint (UClass, UStruct, UEnum, UInterface)
            /// </summary>
            BlueprintType = 0x00000001,

            /// <summary>
            /// The type can be used as a base type for creating new blueprints (UClass, UInterface)
            /// </summary>
            Blueprintable = 0x00000002
        }

        /// <summary>
        /// States the default editor visibility of members defined in C# (properties, functions)
        /// </summary>
        [Flags]
        public enum Member
        {
            None = 0,

            /// <summary>
            /// Functions are callable from blueprint (UFunction)
            /// NOTE: This makes all functions callable, there currently isn't an attribute to override this.
            /// </summary>
            BlueprintCallable = 0x00000001,

            /// <summary>
            /// Properties are visible in the editor (UProperty)
            /// </summary>
            EditorVisible = 0x00000002,
            /// <summary>
            /// Properties are visible in blueprint (UProperty)
            /// </summary>
            BlueprintVisible = 0x00000004,

            /// <summary>
            /// Properties are visible in the editor but read-only (UProperty)
            /// </summary>
            EditorVisibleReadOnly = 0x00000008,
            /// <summary>
            /// Properties are visible in blueprint but read-only (UProperty)
            /// </summary>
            BlueprintVisibleReadOnly = 0x00000010
        }
    }
}

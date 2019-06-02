using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class UEnumAttribute : ManagedUnrealAttributeBase
    {
        public override void ProcessEnum(ManagedUnrealTypeInfo typeInfo)
        {
            typeInfo.AdditionalFlags |= ManagedUnrealTypeInfoFlags.UEnum;
        }
    }

    /// <summary>
    /// Metadata that associates a bitmask property with a bitflag enum.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class BitflagsAttribute : UMetaAttribute
    {
        public BitflagsAttribute(Type enumType)
            : base(MDEnum.BitmaskEnum, enumType.Name)
        {
        }

        public BitflagsAttribute(string enumTypeName)
            : base(MDEnum.BitmaskEnum, enumTypeName)
        {
        }
    }

    /// <summary>
    /// Metadata that signals to the editor that enum values correspond to mask values instead of bitshift (index) values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class UseEnumValuesAsMaskValuesInEditorAttribute : UMetaAttribute
    {
        public UseEnumValuesAsMaskValuesInEditorAttribute(bool enable = true)
            : base(MDEnum.UseEnumValuesAsMaskValuesInEditor, enable)
        {
        }
    }
}

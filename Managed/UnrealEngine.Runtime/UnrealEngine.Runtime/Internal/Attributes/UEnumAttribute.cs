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
}

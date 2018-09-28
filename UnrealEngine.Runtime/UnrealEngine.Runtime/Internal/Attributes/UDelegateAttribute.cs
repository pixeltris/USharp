using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UDelegateAttribute : ManagedUnrealAttributeBase
    {
        public override void ProcessDelegate(ManagedUnrealTypeInfo typeInfo)
        {
            typeInfo.AdditionalFlags |= ManagedUnrealTypeInfoFlags.UDelegate;
        }
    }
}

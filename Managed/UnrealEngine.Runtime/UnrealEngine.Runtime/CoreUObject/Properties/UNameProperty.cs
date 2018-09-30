using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes a name variable pointing into the global name table.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.NameProperty", "CoreUObject", UnrealModuleType.Engine)]
    public class UNameProperty : UProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Name; }
        }

        public override bool IsBlittableType
        {
            get { return true; }
        }

        private PropertyAccessor<FName> accessor;
        public PropertyAccessor<FName> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<FName>(this) : accessor; }
        }
    }
}

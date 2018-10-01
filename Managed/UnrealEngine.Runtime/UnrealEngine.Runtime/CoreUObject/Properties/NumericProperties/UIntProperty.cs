using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes a 32-bit signed integer variable.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.IntProperty", "CoreUObject", UnrealModuleType.Engine)]
    public class UIntProperty : UNumericProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Int; }
        }

        private PropertyAccessor<int> accessor;
        public PropertyAccessor<int> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<int>(this) : accessor; }
        }
    }
}

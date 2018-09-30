using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes an IEEE 32-bit floating point variable.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.FloatProperty", "CoreUObject", UnrealModuleType.Engine)]
    public class UFloatProperty : UNumericProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Float; }
        }

        private PropertyAccessor<float> accessor;
        public PropertyAccessor<float> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<float>(this) : accessor; }
        }
    }
}

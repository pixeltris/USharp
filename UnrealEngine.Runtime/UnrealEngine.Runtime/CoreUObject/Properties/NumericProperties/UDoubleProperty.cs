using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes an IEEE 64-bit floating point variable.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.DoubleProperty", "CoreUObject", UnrealModuleType.Engine)]
    public class UDoubleProperty : UNumericProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Double; }
        }

        private PropertyAccessor<double> accessor;
        public PropertyAccessor<double> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<double>(this) : accessor; }
        }
    }
}

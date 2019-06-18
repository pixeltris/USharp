using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes an IEEE 64-bit floating point variable.
    /// </summary>
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.DoubleProperty")]
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes a 8-bit signed integer variable.
    /// </summary>
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.Int8Property")]
    public class UInt8Property : UNumericProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Int8; }
        }

        private PropertyAccessor<sbyte> accessor;
        public PropertyAccessor<sbyte> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<sbyte>(this) : accessor; }
        }
    }
}

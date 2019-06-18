using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes a 16-bit unsigned integer variable.
    /// </summary>
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.UInt16Property")]
    public class UUInt16Property : UNumericProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.UInt16; }
        }

        private PropertyAccessor<ushort> accessor;
        public PropertyAccessor<ushort> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<ushort>(this) : accessor; }
        }
    }
}

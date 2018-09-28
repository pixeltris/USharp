using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes a 16-bit unsigned integer variable.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.UInt16Property", "CoreUObject", UnrealModuleType.Engine)]
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

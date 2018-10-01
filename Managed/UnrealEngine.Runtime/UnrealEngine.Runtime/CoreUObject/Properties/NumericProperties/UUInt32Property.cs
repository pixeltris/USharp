using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes a 32-bit unsigned integer variable.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.UInt32Property", "CoreUObject", UnrealModuleType.Engine)]
    public class UUInt32Property : UNumericProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.UInt32; }
        }

        private PropertyAccessor<uint> accessor;
        public PropertyAccessor<uint> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<uint>(this) : accessor; }
        }
    }
}

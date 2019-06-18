using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes a 64-bit signed integer variable.
    /// </summary>
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.Int64Property")]
    public class UInt64Property : UNumericProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Int64; }
        }

        private PropertyAccessor<long> accessor;
        public PropertyAccessor<long> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<long>(this) : accessor; }
        }
    }
}

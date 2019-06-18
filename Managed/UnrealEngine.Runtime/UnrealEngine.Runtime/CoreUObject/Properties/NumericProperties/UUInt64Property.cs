using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes a 64-bit unsigned integer variable.
    /// </summary>
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.UInt64Property")]
    public class UUInt64Property : UNumericProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.UInt64; }
        }

        private PropertyAccessor<ulong> accessor;
        public PropertyAccessor<ulong> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<ulong>(this) : accessor; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes a 16-bit signed integer variable.
    /// </summary>
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.Int16Property")]
    public class UInt16Property : UNumericProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Int16; }
        }

        private PropertyAccessor<short> accessor;
        public PropertyAccessor<short> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<short>(this) : accessor; }
        }
    }
}

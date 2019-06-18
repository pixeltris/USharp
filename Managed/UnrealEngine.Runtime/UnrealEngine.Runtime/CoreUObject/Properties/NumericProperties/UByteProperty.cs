using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes an unsigned byte value or 255-value enumeration variable.
    /// </summary>
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.ByteProperty")]
    public class UByteProperty : UNumericProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Byte; }
        }

        private PropertyAccessor<byte> accessor;
        public PropertyAccessor<byte> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<byte>(this) : accessor; }
        }

        private CachedUObject<UEnum> enumCached;
        public UEnum Enum
        {
            get { return enumCached.Update(Native_UByteProperty.Get_Enum(Address)); }
            set { Native_UByteProperty.Set_Enum(Address, enumCached.Set(value)); }
        }
    }
}

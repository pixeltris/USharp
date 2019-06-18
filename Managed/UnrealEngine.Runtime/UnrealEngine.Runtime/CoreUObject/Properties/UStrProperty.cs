using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes a dynamic string variable.
    /// </summary>
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.StrProperty")]
    public class UStrProperty : UProperty, IPropertyAccessor<string>
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Str; }
        }

        private PropertyAccessor<string> accessor;
        public PropertyAccessor<string> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<string>(this) : accessor; }
        }

        public string GetValuePtr(IntPtr address)
        {
            return FStringMarshaler.FromPtr(address, false);
        }

        public void SetValuePtr(IntPtr address, string value)
        {
            // This expects the existing memory is valid as this will clean the existing string
            FStringMarshaler.ToArray(address, value);
        }
    }
}

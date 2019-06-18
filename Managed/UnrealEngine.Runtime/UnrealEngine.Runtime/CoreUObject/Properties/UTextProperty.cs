using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.TextProperty")]
    public class UTextProperty : UProperty, IPropertyAccessor<FText>
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Text; }
        }

        private PropertyAccessor<FText> accessor;
        public PropertyAccessor<FText> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<FText>(this) : accessor; }
        }

        public FText GetValuePtr(IntPtr address)
        {
            return FTextMarshaler.FromNative(address);
        }

        public void SetValuePtr(IntPtr address, FText value)
        {
            FTextMarshaler.ToNative(address, value);
        }
    }
}

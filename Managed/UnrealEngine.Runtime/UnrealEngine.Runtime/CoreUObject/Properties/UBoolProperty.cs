using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    [UMetaPath("/Script/CoreUObject.BoolProperty", "CoreUObject", UnrealModuleType.Engine)]
    public class UBoolProperty : UProperty, IPropertyAccessor<bool>
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Bool; }
        }

        private PropertyAccessor<bool> accessor;
        public PropertyAccessor<bool> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<bool>(this) : accessor; }
        }

        public bool GetValuePtr(IntPtr address)
        {
            return Native_UBoolProperty.GetPropertyValue(Address, address);
        }

        public void SetValuePtr(IntPtr address, bool value)
        {
            Native_UBoolProperty.SetPropertyValue(Address, address, value);
        }
    }
}

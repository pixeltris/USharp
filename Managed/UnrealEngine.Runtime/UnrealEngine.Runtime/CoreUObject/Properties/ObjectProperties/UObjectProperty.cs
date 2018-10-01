using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    [UMetaPath("/Script/CoreUObject.ObjectProperty", "CoreUObject", UnrealModuleType.Engine)]
    public class UObjectProperty : UObjectPropertyBase, IPropertyAccessor<UObject>
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Object; }
        }

        private PropertyAccessor<UObject> accessor;
        public PropertyAccessor<UObject> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<UObject>(this) : accessor; }
        }

        public UObject GetValuePtr(IntPtr address)
        {
            return GCHelper.Find(Marshal.ReadIntPtr(address));
        }

        public void SetValuePtr(IntPtr address, UObject value)
        {
            Marshal.WriteIntPtr(address, value == null ? IntPtr.Zero : value.Address);
        }
    }
}

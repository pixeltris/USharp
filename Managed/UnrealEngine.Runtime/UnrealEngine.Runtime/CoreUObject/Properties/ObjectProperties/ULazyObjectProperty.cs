using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    [UMetaPath("/Script/CoreUObject.LazyObjectProperty", "CoreUObject", UnrealModuleType.Engine)]
    public class ULazyObjectProperty : UObjectPropertyBase, IPropertyAccessor<FLazyObjectPtr>
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.LazyObject; }
        }

        private PropertyAccessor<FLazyObjectPtr> accessor;
        public PropertyAccessor<FLazyObjectPtr> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<FLazyObjectPtr>(this) : accessor; }
        }

        public FLazyObjectPtr GetValuePtr(IntPtr address)
        {
            return Marshal.PtrToStructure<FLazyObjectPtr>(address);
        }

        public void SetValuePtr(IntPtr address, FLazyObjectPtr value)
        {
            // Use Copy to ensure a deep copy
            FLazyObjectPtr existing = Marshal.PtrToStructure<FLazyObjectPtr>(address);            
            existing.Copy(value);
            Marshal.StructureToPtr(existing, address, false);
        }
    }
}

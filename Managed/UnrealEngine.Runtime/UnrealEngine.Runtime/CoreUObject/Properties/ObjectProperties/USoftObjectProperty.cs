using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    [UMetaPath("/Script/CoreUObject.SoftObjectProperty", "CoreUObject", UnrealModuleType.Engine)]
    public class USoftObjectProperty : UObjectPropertyBase, IPropertyAccessor<FSoftObjectPtr>
    {
        private FSoftObjectPtr softObject;

        public override EPropertyType PropertyType
        {
            get { return EPropertyType.SoftObject; }
        }

        private PropertyAccessor<FSoftObjectPtr> accessor;
        public PropertyAccessor<FSoftObjectPtr> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<FSoftObjectPtr>(this) : accessor; }
        }

        public FSoftObjectPtr GetValuePtr(IntPtr address)
        {
            // Don't dispose of objectPtr as it will still be held in native memory
            FSoftObjectPtrUnsafe objectPtr = Marshal.PtrToStructure<FSoftObjectPtrUnsafe>(address);
            softObject = new FSoftObjectPtr(objectPtr.ObjectPath);
            return softObject;
        }

        public void SetValuePtr(IntPtr address, FSoftObjectPtr value)
        {
            softObject = value;

            using (FSoftObjectPtrUnsafe tempObjectPtr = new FSoftObjectPtrUnsafe(softObject.ObjectPath))
            {
                // Use Copy to ensure a deep copy
                FSoftObjectPtrUnsafe existing = Marshal.PtrToStructure<FSoftObjectPtrUnsafe>(address);
                existing.Copy(tempObjectPtr);

                // Copy the structure back to into the address
                Marshal.StructureToPtr(existing, address, false);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes a dynamic array.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.ArrayProperty", "CoreUObject", UnrealModuleType.Engine)]
    public class UArrayProperty : UProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Array; }
        }

        // NOTE: We probably a custom IPropertyAccessor
        private PropertyAccessor<FScriptArray> accessor;
        public PropertyAccessor<FScriptArray> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<FScriptArray>(this) : accessor; }
        }

        private CachedUObject<UProperty> inner;
        public UProperty Inner
        {
            get { return inner.Update(Native_UArrayProperty.Get_Inner(Address)); }
            set { Native_UArrayProperty.Set_Inner(Address, inner.Set(value)); }
        }
    }
}

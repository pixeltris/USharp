using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    [UMetaPath("/Script/CoreUObject.SetProperty", "CoreUObject", UnrealModuleType.Engine)]
    public class USetProperty : UProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Set; }
        }
        
        private PropertyAccessor<FScriptSet> accessor;
        public PropertyAccessor<FScriptSet> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<FScriptSet>(this) : accessor; }
        }

        private CachedUObject<UProperty> elementProp;
        public UProperty ElementProp
        {
            get { return elementProp.Update(Native_USetProperty.Get_ElementProp(Address)); }
            set { Native_USetProperty.Set_ElementProp(Address, elementProp.Set(value)); }
        }

        public FScriptSetLayout SetLayout
        {
            get { return Native_USetProperty.Get_SetLayout(Address); }
            set { Native_USetProperty.Set_SetLayout(Address, value); }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.MapProperty")]
    public class UMapProperty : UProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Map; }
        }
        
        private PropertyAccessor<FScriptMap> accessor;
        public PropertyAccessor<FScriptMap> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<FScriptMap>(this) : accessor; }
        }

        private CachedUObject<UProperty> keyProp;
        public UProperty KeyProp
        {
            get { return keyProp.Update(Native_UMapProperty.Get_KeyProp(Address)); }
            set { Native_UMapProperty.Set_KeyProp(Address, keyProp.Set(value)); }
        }

        private CachedUObject<UProperty> valueProp;
        public UProperty ValueProp
        {
            get { return valueProp.Update(Native_UMapProperty.Get_ValueProp(Address)); }
            set { Native_UMapProperty.Set_ValueProp(Address, valueProp.Set(value)); }
        }

        public FScriptMapLayout MapLayout
        {
            get { return Native_UMapProperty.Get_MapLayout(Address); }
            set { Native_UMapProperty.Set_MapLayout(Address, value); }
        }
    }    
}

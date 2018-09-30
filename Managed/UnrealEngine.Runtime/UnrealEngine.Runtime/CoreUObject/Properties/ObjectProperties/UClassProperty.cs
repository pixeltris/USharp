using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    [UMetaPath("/Script/CoreUObject.ClassProperty", "CoreUObject", UnrealModuleType.Engine)]
    public class UClassProperty : UObjectProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Class; }
        }

        private CachedUObject<UClass> metaClass;
        public UClass MetaClass
        {
            get { return metaClass.Update(Native_UClassProperty.Get_MetaClass(Address)); }
            set { Native_UClassProperty.Set_MetaClass(Address, metaClass.Set(value)); }
        }

        /// <summary>
        /// Setter function for this property's MetaClass member. Favor this function 
        /// whilst loading (since, to handle circular dependencies, we defer some 
        /// class loads and use a placeholder class instead). It properly handles 
        /// deferred loading placeholder classes (so they can properly be replaced 
        /// later).
        /// </summary>
        /// <param name="newMetaClass">The MetaClass you want this property set with.</param>
        public void SetMetaClass(UClass newMetaClass)
        {
            Native_UClassProperty.SetMetaClass(Address, newMetaClass == null ? IntPtr.Zero : newMetaClass.Address);
        }
    }
}

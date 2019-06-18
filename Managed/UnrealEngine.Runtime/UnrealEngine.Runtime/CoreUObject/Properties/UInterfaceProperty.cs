using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// This variable type provides safe access to a native interface pointer.  The data class for this variable is FScriptInterface, and is exported to auto-generated
    /// script header files as a TScriptInterface.
    /// </summary>
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.InterfaceProperty")]
    public class UInterfaceProperty : UProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Interface; }
        }

        private CachedUObject<UClass> interfaceClass;
        public UClass InterfaceClass
        {
            get { return interfaceClass.Update(Native_UInterfaceProperty.Get_InterfaceClass(Address)); }
            set { Native_UInterfaceProperty.Set_InterfaceClass(Address, interfaceClass.Set(value)); }
        }

        /// <summary>
        /// Setter function for this property's InterfaceClass member. Favor this 
        /// function whilst loading (since, to handle circular dependencies, we defer 
        /// some class loads and use a placeholder class instead). It properly 
        /// handles deferred loading placeholder classes (so they can properly be 
        /// replaced later).
        /// </summary>
        /// <param name="newInterfaceClass">The InterfaceClass you want this property set with.</param>
        public void SetMetaClass(UClass newInterfaceClass)
        {
            Native_UInterfaceProperty.SetInterfaceClass(Address, newInterfaceClass == null ? IntPtr.Zero : newInterfaceClass.Address);
        }
    }
}
